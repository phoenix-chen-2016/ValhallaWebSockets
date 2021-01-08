using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Valhalla.WebSockets
{
	internal class WebSocketManagerMiddleware<THandler> where THandler : class, IWebSocketConnectionHandler
	{
		private readonly ILogger<WebSocketManagerMiddleware<THandler>> m_Logger;
		private readonly RequestDelegate m_Next;
		private readonly THandler m_ConnectionHandler;

		public WebSocketManagerMiddleware(
			RequestDelegate next,
			THandler handler,
			ILogger<WebSocketManagerMiddleware<THandler>> logger)
		{
			m_Next = next;
			m_ConnectionHandler = handler ?? throw new ArgumentNullException(nameof(handler));
			m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.WebSockets.IsWebSocketRequest)
				return;

			var cancelToken = context.RequestAborted;

			var socket = await context.WebSockets.AcceptWebSocketAsync();

			if (socket == null)
				return;

			await m_ConnectionHandler.OnConnectedAsync(context, socket, cancelToken);

			await ReceiveAsync(context, socket, m_ConnectionHandler, cancelToken);
		}

		private async Task ReceiveAsync(
			HttpContext context,
			WebSocket socket,
			THandler handler,
			CancellationToken cancellationToken)
		{
			var receiveBuffer = WebSocket.CreateServerBuffer(4 * 1024);
			var dataBuffer = new List<byte>(8 * 1024);

			try
			{
				while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
				{
					var result = await socket.ReceiveAsync(
						buffer: receiveBuffer,
						cancellationToken);

					if (result == null)
						continue;

					dataBuffer.AddRange(new ArraySegment<byte>(receiveBuffer.Array!, 0, result.Count));

					switch (result.MessageType)
					{
						case WebSocketMessageType.Close:
							await handler.OnCloseAsync(
								context,
								socket,
								result.CloseStatus,
								result.CloseStatusDescription,
								cancellationToken);
							dataBuffer.Clear();
							break;
						default:
							if (result.EndOfMessage)
							{
								await handler.OnReceiveAsync(
									context,
									socket,
									new ArraySegment<byte>(dataBuffer.ToArray(), 0, dataBuffer.Count),
									cancellationToken);
								dataBuffer.Clear();
							}
							break;
					}
				}
			}
			finally
			{
				await handler.OnDisconnectedAsync(context, socket, cancellationToken);
			}
		}
	}
}
