using Detailly.Application.Abstractions;
using Detailly.Application.Abstractions.Payments;
using Detailly.Infrastructure.Common;
using Detailly.Infrastructure.Database;
using Detailly.Infrastructure.Payments.Stripe;
using Detailly.Shared.Constants;
using Detailly.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Detailly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // Typed ConnectionStrings + validation
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // DbContext: InMemory for test environments; SQL Server otherwise
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            if (env.IsTest())
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");

                return;
            }

            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.Main;

            Console.WriteLine($"🚨 USING CONNECTION STRING: {cs}");

            options.UseSqlServer(cs);
        });

        // IAppDbContext mapping
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // Identity hasher
        services.AddScoped<IPasswordHasher<ApplicationUserEntity>, PasswordHasher<ApplicationUserEntity>>();

        // Token service (reads JwtOptions via IOptions<JwtOptions>)
        services.AddTransient<IJwtTokenService, JwtTokenService>();

        // HttpContext accessor + current user
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();

        // Stripe service
        //services.AddScoped<IStripeService, FakeStripeService>();
        services.AddScoped<IStripeService, StripeService>();

        // Webhook verifier
        services.AddScoped<IWebhookVerifier, WebhookVerifier>();

        // Stripe webhook parser
        services.AddScoped<IStripeWebhookParser, StripeWebhookParser>();


        // TimeProvider (if used in handlers/services)
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}