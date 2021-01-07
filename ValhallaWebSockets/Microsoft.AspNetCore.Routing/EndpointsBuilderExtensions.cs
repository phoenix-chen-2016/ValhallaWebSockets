using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Valhalla.WebSockets;

namespace Microsoft.AspNetCore.Routing
{
	public static class EndpointsBuilderExtensions
	{
		public static IEndpointConventionBuilder MapWebSocketManager<THandler>(this IEndpointRouteBuilder endpoints, PathString path)
			where THandler : class, IWebSocketConnectionHandler
		{
			return endpoints.Map(
				path,
				endpoints.CreateApplicationBuilder()
					.UseMiddleware<WebSocketManagerMiddleware<THandler>>(
						ActivatorUtilities.CreateInstance<THandler>(endpoints.ServiceProvider))
					.Build());
		}
	}
}
