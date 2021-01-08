using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Valhalla.WebSockets
{
	public interface IWebSocketConnectionHandler
	{
		ValueTask OnConnectedAsync(
			HttpContext httpContext,
			WebSocket socket,
			CancellationToken cancellationToken);

		ValueTask OnDisconnectedAsync(
			HttpContext httpContext,
			WebSocket socket,
			CancellationToken cancellationToken);

		ValueTask OnReceiveAsync(
			HttpContext httpContext,
			WebSocket socket,
			ReadOnlyMemory<byte> buffer,
			CancellationToken cancellationToken);

		ValueTask OnCloseAsync(
			HttpContext httpContext,
			WebSocket socket,
			WebSocketCloseStatus? closeStatus,
			string? closeDescription,
			CancellationToken cancellationToken);
	}
}
