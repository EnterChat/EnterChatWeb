using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnterChatWeb.Models;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EnterChatWeb.Data
{
    public class ChatWebSocketMiddleWare
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly RequestDelegate _next;

        public ChatWebSocketMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, EnterChatContext chatContext)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            CancellationToken ct = context.RequestAborted;
            WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = Guid.NewGuid().ToString();

            _sockets.TryAdd(socketId, currentSocket);

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var response = await ReceiveStringAsync(currentSocket, ct);
                if (string.IsNullOrEmpty(response))
                {
                    if (currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }

                    continue;
                }
                int user_id = Int32.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                int comp_id = Int32.Parse(context.User.FindFirst("CompanyID").Value);
                User user = await chatContext.Users.Where(u => u.ID == user_id).FirstOrDefaultAsync();
                Worker worker = await chatContext.Workers.Where(w => w.ID == user.WorkerID).FirstOrDefaultAsync();
                int index_stick = response.IndexOf('|');

                string idRaw = response.Substring(0, index_stick);

                string trueMessage = response.Remove(0, index_stick + 1);

                if (idRaw.Equals("NoN"))
                {
                    GroupChatMessage message = new GroupChatMessage
                    {
                        UserID = user_id,
                        CompanyID = comp_id,
                        Text = trueMessage,
                        CreationDate = DateTime.Now
                    };
                    await chatContext.GroupChatMessages.AddAsync(message);
                    await chatContext.SaveChangesAsync();
                }
                else
                {
                    TopicMessage topicMessage = new TopicMessage
                    {
                        TopicID = Convert.ToInt32(idRaw),
                        UserID = user_id,
                        CompanyID = comp_id,
                        Text = trueMessage,
                        CreationDate = DateTime.Now
                    };
                    await chatContext.TopicMessages.AddAsync(topicMessage);
                    await chatContext.SaveChangesAsync();
                }

                string responceMessage = worker.FirstName + " " + worker.SecondName + ": " + trueMessage;

                foreach (var socket in _sockets)
                {
                    if (socket.Value.State != WebSocketState.Open)
                    {
                        continue;
                    }

                    await SendStringAsync(socket.Value, responceMessage, ct);
                }
            }
            WebSocket dummy;
            _sockets.TryRemove(socketId, out dummy);

            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }

        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {

            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
