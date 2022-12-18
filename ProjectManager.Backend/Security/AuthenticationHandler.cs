#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ProjectManager.Backend.Apis;

namespace ProjectManager.Backend.Security;

public static class AuthenticationExtensions {
    public static AuthenticationBuilder AddCustomAuthentication(this IServiceCollection services, bool configureSwagger = false) {
        var builder = services
            .AddAuthentication("CustomScheme")
            .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("CustomScheme", _ => { });

        if (configureSwagger) {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("CustomScheme", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "CustomScheme"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "CustomScheme"
                        },
                        Scheme = "oauth2",
                        Name = "CustomScheme",
                        In = ParameterLocation.Header,
                    },
                    ArraySegment<string>.Empty
                }});
            });

        }

        return builder;
    }
}

public sealed class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly ITokenApi _tokens;

    public AuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ITokenApi tokens)
        : base(options, logger, encoder, clock) {
        _tokens = tokens;
    }
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        string tokenId = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(tokenId)) tokenId = Request.Query["token"];
        string clientIp = Context.Connection.RemoteIpAddress?.ToString();
        return !_tokens.ValidateToken(tokenId, clientIp) ? AuthenticateResult.Fail("Token invalid") : AuthenticateResult.Success(GenerateTicket(tokenId));
    }

    private AuthenticationTicket GenerateTicket(string tokenId) {
        var user = _tokens.GetUserFromToken(tokenId);
        var claims = new List<Claim> {
            new("TokenId", tokenId),
            new("UserId", user.UserId)
        };

        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity(claims, Scheme.Name));
        return new AuthenticationTicket(principal, Scheme.Name);
    }

}

public static class ClaimsPrincipalExtensions {
    public static string GetTokenId(this ClaimsPrincipal principal) => principal.FindFirstValue("TokenId");
    public static string GetUserId(this ClaimsPrincipal principal) => principal.FindFirstValue("UserId");
}