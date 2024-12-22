namespace RecrutingApi.Authentication
{
    public class ApiKeyAuthenticaitonMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticaitonMiddleware(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var apikey = _configuration.GetValue<string>(Authentication.ApiKey);
            if (!context.Request.Headers.TryGetValue(Authentication.ApiKeyHeader, out var incallApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key missing");
                return;
            }

            if (!apikey.Equals(incallApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Api Key");
                return;
            }
            await next(context);
        }
    }
}