using EnterChatWeb.Models.ExtraModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnterChatWeb.Controllers;
using Microsoft.EntityFrameworkCore;
using EnterChatWeb.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

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
                GroupChatMessage message = new GroupChatMessage
                {
                        UserID = user_id,
                        CompanyID = comp_id,
                        Text = response,
                        CreationDate = DateTime.Now
                };
                await chatContext.GroupChatMessages.AddAsync(message);
                await chatContext.SaveChangesAsync();
                

                foreach (var socket in _sockets)
                {
                    if (socket.Value.State != WebSocketState.Open)
                    {
                        continue;
                    }

                    await SendStringAsync(socket.Value, response, ct);
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
            /*var optionsBuilder = new DbContextOptionsBuilder<EnterChatContext>();
            optionsBuilder.UseSqlite("Server=(localdb)\\mssqllocaldb;Database=EnterChatWebDB;");
            using (var context = new EnterChatContext(optionsBuilder.Options))
            {
                GroupChatMessage message = new GroupChatMessage
                {
                    UserID = 1,
                    CompanyID = 1,
                    Text = data,
                    CreationDate = DateTime.Now
                };
                context.GroupChatMessages.AddAsync(message);
                context.SaveChangesAsync();
            }*/
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
