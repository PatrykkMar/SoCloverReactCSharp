using Microsoft.AspNetCore.SignalR;

namespace SoClover.Server.Filters
{
    public class HubErrorFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Hub Error]: {ex.Message}");
                await invocationContext.Hub.Clients.Caller.SendAsync("Error", ex.Message);
                return null;
            }
        }
    }
}