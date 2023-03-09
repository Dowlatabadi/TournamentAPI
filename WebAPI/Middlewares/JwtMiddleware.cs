namespace Tournament.WebAPI.Middlewares;

using Tournament.WebAPI.Services;
using Tournament.WebAPI.Authorization;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var username = jwtUtils.ValidateToken(token);
        if (username != null)
        {
            // attach username to context on successful jwt validation
            context.Items["Username"] = username;
        }

        await _next(context);
    }
}
