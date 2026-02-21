namespace ApiGateway.Middlewares
{
    public class RequestMetadataMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                    ?.ToLower() ?? "prod";

            var correlationId = $"web-{env}-{Guid.CreateVersion7()}";
            var requestId = Guid.CreateVersion7().ToString();

            context.Items["CorrelationId"] = correlationId;
            context.Items["RequestId"] = requestId;

            context.Response.Headers["X-Correlation-Id"] = correlationId;
            context.Response.Headers["X-Request-Id"] = requestId;

            await next(context);
        }
    }
}
