using Detailly.Infrastructure.Common;
using Detailly.Shared.Constants;
using Detailly.Shared.Dtos;
using Detailly.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Detailly.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // Controllers + uniform BadRequest
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(opts =>
            {
                opts.InvalidModelStateResponseFactory = ctx =>
                {
                    var msg = string.Join("; ",
                        ctx.ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                                 ? "Validation error"
                                                 : e.ErrorMessage));
                    return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new ErrorDto
                    {
                        Code = "validation.failed",
                        Message = msg
                    });
                };
            })
            .AddNewtonsoftJson(opts =>
                opts.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc);

        // Typed options + validation on startup
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // JWT auth (reads from IOptions<JwtOptions>)
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer((o) =>
        {
            var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

            o.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddCookie("External") // <— TEMP cookie for external login
        .AddGoogle("Google", options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            options.CallbackPath = "/auth/external/google/callback";
            options.SignInScheme = "External";

            // SameSite=None requires Secure — set Lax so the correlation cookie
            // survives the cross-origin redirect from Google on both HTTP and HTTPS
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            o.AddPolicy(AuthPolicies.AdminOnly,
                p => p.RequireClaim("is_admin", "true"));

            o.AddPolicy(AuthPolicies.ManagerOnly,
                p => p.RequireClaim("is_manager", "true"));

            o.AddPolicy(AuthPolicies.EmployeeOnly,
                p => p.RequireClaim("is_employee", "true"));

            o.AddPolicy(AuthPolicies.FleetOnly,
                p => p.RequireClaim("is_fleet", "true"));

            o.AddPolicy(AuthPolicies.AdminOrManager,
                p => p.RequireAssertion(ctx =>
                    ctx.User.HasClaim("is_admin", "true") ||
                    ctx.User.HasClaim("is_manager", "true")));

            o.AddPolicy(AuthPolicies.Staff,
                p => p.RequireAssertion(ctx =>
                    ctx.User.HasClaim("is_admin", "true") ||
                    ctx.User.HasClaim("is_manager", "true") ||
                    ctx.User.HasClaim("is_employee", "true")));

            // "Any client" = authenticated AND (fleet true OR fleet false)
            // Since we only store "is_fleet", this basically means "authenticated user"
            o.AddPolicy(AuthPolicies.AnyClient,
                p => p.RequireAssertion(ctx =>
                    ctx.User.HasClaim("is_fleet", "true") ||
                    ctx.User.HasClaim("is_fleet", "false")));

            // StandardClientOnly = explicitly not fleet
            o.AddPolicy(AuthPolicies.StandardClientOnly,
                p => p.RequireClaim("is_fleet", "false"));
        });

        // Swagger with Bearer auth
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Detailly API", Version = "v1" });
            var xml = Path.Combine(AppContext.BaseDirectory, "Detailly.API.xml");
            if (File.Exists(xml))
                c.IncludeXmlComments(xml, includeControllerXmlComments: true);

            var bearer = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Unesi JWT token. Format: **Bearer {token}**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            c.AddSecurityDefinition("Bearer", bearer);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { { bearer, Array.Empty<string>() } });
        });

        services.AddExceptionHandler<DetaillyExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}