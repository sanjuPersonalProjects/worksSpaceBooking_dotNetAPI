using Serilog;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;


namespace WorkSpaceBooking1.Middlewares
{
    public class LoggingMiddleware
    {
        


            private readonly RequestDelegate _next;

            public LoggingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var stopwatch = Stopwatch.StartNew();

                // Log the incoming request
                Log.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

                await _next(context); // Call the next middleware

                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;

                // Log the outgoing response
                Log.Information("Handled request: {Method} {Path} responded with {StatusCode} in {ResponseTime}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    responseTime);
            }
        }
    }

