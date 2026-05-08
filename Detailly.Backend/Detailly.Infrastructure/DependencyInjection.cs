using Detailly.Application.Abstractions;
using Detailly.Application.Abstractions.Booking;
using Detailly.Application.Abstractions.Payments;
using Detailly.Application.Abstractions.PDF;
using Detailly.Infrastructure.Background;
using Detailly.Infrastructure.Booking;
using Detailly.Infrastructure.Cloudinary;
using Detailly.Infrastructure.Common;
using Detailly.Infrastructure.Database;
using Detailly.Infrastructure.ExternalAuth;
using Detailly.Infrastructure.Payments.Stripe;
using Detailly.Infrastructure.PDF;
using Detailly.Shared.Constants;
using Detailly.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
using System.Net.Http;

namespace Detailly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Cloudinary
        services.AddOptions<CloudinaryOptions>()
            .Bind(configuration.GetSection(CloudinaryOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        // PDF generators
        services.AddScoped<IBookingsPdfGenerator, BookingsPdfGenerator>();
        services.AddScoped<IShiftsPdfGenerator, ShiftsPdfGenerator>();

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

        // Booking hold expiry cleanup service
        services.AddHostedService<BookingHoldExpiryCleanupService>();

        // OpenRouteService options (mobile pricing + API key)
        services.AddOptions<OpenRouteServiceOptions>()
            .Bind(configuration.GetSection(OpenRouteServiceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Fleet discount options
        services.AddOptions<FleetDiscountOptions>()
            .Bind(configuration.GetSection(FleetDiscountOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Road distance service (OpenRouteService Directions API)
        services.AddHttpClient<IRoadDistanceService, OrsRoadDistanceService>();

        // Booking quote service, for calculating price and availability without creating a booking
        services.AddScoped<IBookingQuoteService, BookingQuoteService>();

        // External auth (Google OAuth cookie exchange)
        services.AddScoped<IExternalAuthService, ExternalAuthService>();

        // TimeProvider (if used in handlers/services)
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}