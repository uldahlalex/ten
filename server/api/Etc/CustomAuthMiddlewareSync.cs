using System.Security.Claims;
using api.Services;

namespace api.Etc;

public class CustomAuthMiddlewareSync<TJwtService>(
    RequestDelegate next,
    ILogger<CustomAuthMiddlewareSync<TJwtService>> logger)
    where TJwtService : class, IJwtService
{
    public async Task InvokeAsync(HttpContext context, TJwtService jwtService)
    {
        //Skip if AllowAnonymous
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
        if (allowAnonymous != null)
        {
            await next(context);
            return;
        }
        
            var authHeader = context.Request.Headers["Authorization"].ToString();
  
            var token = authHeader.Substring("Bearer ".Length).Trim();
              
            var result = jwtService.VerifyJwt(token); // Synchronous call
                    
            
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, result.Id)
            }, "jwt"));




        

        await next(context);
    }
}