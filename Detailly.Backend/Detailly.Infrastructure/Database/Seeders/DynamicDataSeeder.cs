using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder koji se pokreće u runtime-u,
/// obično pri startu aplikacije (npr. u Program.cs).
/// Koristi se za unos demo/test podataka koji nisu dio migracije.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // Osiguraj da baza postoji (bez migracija)
        await context.Database.EnsureCreatedAsync();

        await SeedProductCategoriesAsync(context);
        await SeedUsersAsync(context);
        await SeedVehicleCategoriesAsync(context);
    }

    private static async Task SeedProductCategoriesAsync(DatabaseContext context)
    {
        if (!await context.ProductCategories.AnyAsync())
        {
            context.ProductCategories.AddRange(
                new ProductCategoryEntity
                {
                    Name = "Računari (demo)",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductCategoryEntity
                {
                    Name = "Mobilni uređaji (demo)",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: product categories added.");
        }
    }

    /// <summary>
    /// Kreira demo korisnike ako ih još nema u bazi.
    /// </summary>
    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        if (await context.ApplicationUsers.AnyAsync())
            return;

        var hasher = new PasswordHasher<ApplicationUserEntity>();

        var admin = new ApplicationUserEntity
        {
            Email = "admin@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "Admin123!"),
            IsAdmin = true,
            IsEnabled = true,
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin"
        };

        var user = new ApplicationUserEntity
        {
            Email = "manager@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "User123!"),
            IsManager = true,
            IsEnabled = true,
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin"
        };

        var dummyForSwagger = new ApplicationUserEntity
        {
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            IsEmployee = true,
            IsEnabled = true,
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin"
        };
        var dummyForTests = new ApplicationUserEntity
        {
            Email = "test",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            IsEmployee = true,
            IsEnabled = true,
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin"
        };
        var haris = new ApplicationUserEntity
        {
            Email = "haris.velispahic@edu.fit.ba",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            IsAdmin = true,
            IsEmployee = true,
            IsEnabled = true,
            AddressId = null,
            FirstName = "Haris",
            LastName = "Velispahic",
            Username = "haris123"
        };
        var danis = new ApplicationUserEntity
        {
            Email = "danis.music@edu.fit.ba",
            PasswordHash = hasher.HashPassword(null!, "danis123"),
            IsAdmin = true,
            IsEmployee = true,
            IsEnabled = true,
            AddressId = null,
            FirstName = "Danis",
            LastName = "Music",
            Username = "danis123"
        };
        context.ApplicationUsers.AddRange(admin, user, dummyForSwagger, dummyForTests);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dynamic seed: demo users added.");
    }

    private static async Task SeedVehicleCategoriesAsync(DatabaseContext context)
    {
        if (!await context.VehicleCategories.AnyAsync())
        {
            context.VehicleCategories.AddRange(
                new VehicleCategoryEntity
                {
                    Name = "SUV",
                    Description = "",
                    BasePriceMultiplier = 1.0m,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VehicleCategoryEntity
                {
                    Name = "Sedan",
                    Description = "",
                    BasePriceMultiplier = 0.9m,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VehicleCategoryEntity
                {
                    Name = "Roadster",
                    Description = "",
                    BasePriceMultiplier = 0.9m,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VehicleCategoryEntity
                {
                    Name = "Hatchback",
                    Description = "",
                    BasePriceMultiplier = 0.9m,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VehicleCategoryEntity
                {
                    Name = "Van",
                    Description = "",
                    BasePriceMultiplier = 0.9m,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VehicleCategoryEntity
                {
                    Name = "Sports car",
                    Description = "",
                    BasePriceMultiplier = 0.9m,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: vehicle categories added.");
        }
    }
}