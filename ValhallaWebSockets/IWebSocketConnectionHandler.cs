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
			WebSocket socket,
			HttpContext httpContext,
			CancellationToken cancellationToken = default);

		ValueTask OnDisconnectedAsync(
			WebSocket socket,
			CancellationToken cancellationToken = default);

		ValueTask OnReceiveAsync(
			WebSocket socket,
			ArraySegment<byte> buffer,
			CancellationToken cancellationToken = default);

		ValueTask OnCloseAsync(
			WebSocket socket,
			WebSocketCloseStatus? closeStatus,
			string? closeDescription,
			CancellationToken cancellationToken = default);
	}
}
