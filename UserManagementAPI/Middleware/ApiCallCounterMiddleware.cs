using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class ApiCallCounterMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, int> RouteCounts = new();

        public ApiCallCounterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "unknown";
            RouteCounts.AddOrUpdate(path, 1, (_, count) => count + 1);

            // Expose the counts via a header for debugging
            context.Response.Headers["X-Api-Call-Count"] = RouteCounts[path].ToString();

            await _next(context);
        }

        public static IReadOnlyDictionary<string, int> GetRouteCounts() => RouteCounts;
    }
}