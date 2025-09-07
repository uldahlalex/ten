using System.Security.Authentication;
using System.Security.Claims;
using api.Services;
using Microsoft.AspNetCore.Authorization;

namespace api.Etc;

public class CustomAuthMiddlewareSync<T>(
    RequestDelegate next,
    ILogger<CustomAuthMiddlewareSync<T>> logger)
    where T : class, IJwtService
{
    public async Task InvokeAsync(HttpContext context, T auth)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>();
        if (allowAnonymous != null)
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
            throw new AuthenticationException("Authentication missing");

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrWhiteSpace(token))
            throw new AuthenticationException("Authentication malformatted");

        var result = await auth.VerifyJwtOrThrow(token);

        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, result.Id)
        }, "jwt"));


        await next(context);
    }
}