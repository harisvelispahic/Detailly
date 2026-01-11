using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Shared;
using Detailly.Domain.Entities.Vehicle;
using Detailly.Domain.Entities.Booking; // ✅ DODANO
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        await SeedProductsAsync(context);
        await SeedAddressesAsync(context);
        await SeedServicePackageItemsAsync(context); 
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
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin",

            IsAdmin = true,
            IsFleet = false,
        };
        admin.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = admin,
        };

        var user = new ApplicationUserEntity
        {
            Email = "manager@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "User123!"),
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin",

            IsManager = true,
            IsFleet = false
        };
        user.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = user,
        };

        var dummyForSwagger = new ApplicationUserEntity
        {
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin",

            IsAdmin = true,
            IsFleet = false
        };
        dummyForSwagger.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = dummyForSwagger,
        };
        var dummyForTests = new ApplicationUserEntity
        {
            Email = "test",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin",

            IsEmployee = true,
            IsFleet = true
        };
        dummyForTests.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = dummyForTests,
        };
        var haris = new ApplicationUserEntity
        {
            Email = "haris.velispahic@edu.fit.ba",
            PasswordHash = hasher.HashPassword(null!, "haris123"),
            AddressId = null,
            FirstName = "Haris",
            LastName = "Velispahic",
            Username = "haris123",

            IsAdmin = true,
            IsFleet = false
        };
        haris.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = haris,
        };
        var danis = new ApplicationUserEntity
        {
            Email = "danis.music@edu.fit.ba",
            PasswordHash = hasher.HashPassword(null!, "danis123"),
            AddressId = null,
            FirstName = "Admin",
            LastName = "Admin",
            Username = "admin",

            IsAdmin = true,
            IsFleet = false
        };
        danis.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = danis,
        };
        context.ApplicationUsers.AddRange(admin, user, dummyForSwagger, dummyForTests, haris, danis);
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

    private static async Task SeedProductsAsync(DatabaseContext context)
    {
        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(
                new ProductEntity
                {
                    Name = "Proizvod 1",
                    Description = "...",
                    ProductNumber = new Guid().ToString(),
                    Price = 100m,
                    Currency = CurrencyName.BAM,
                    CategoryId = 1
                },
                new ProductEntity
                {
                    Name = "Proizvod 2",
                    Description = "...",
                    ProductNumber = new Guid().ToString(),
                    Price = 200m,
                    Currency = CurrencyName.EUR,
                    CategoryId = 2
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: products added.");
        }
    }

    private static async Task SeedAddressesAsync(DatabaseContext context)
    {
        if (!await context.Addresses.AnyAsync())
        {
            context.Addresses.AddRange(
                new AddressEntity
                {
                    Country = "Bosnia and Herzegovina",
                    Region = "Zeničko-dobojski kanton",
                    City = "Kakanj",
                    PostalCode = "72240",
                    Street = "Alije Izetbegovića, bb"
                },
                new AddressEntity
                {
                    Country = "Bosnia and Herzegovina",
                    Region = "Hercegovačko-neretvanski kanton",
                    City = "Mostar",
                    PostalCode = "88000",
                    Street = "Maršala Tita, bb"
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: products added.");
        }
    }

    
    private static async Task SeedServicePackageItemsAsync(DatabaseContext context)
    {
        
        if (await context.ServicePackageItems.AnyAsync())
            return;

        context.ServicePackageItems.AddRange(
            new ServicePackageItemEntity { Name = "Prewash / Snow Foam", Price = 10.00m, Description = "Predpranje pjenom za sigurnije ručno pranje." },
            new ServicePackageItemEntity { Name = "Ručni wash (2-bucket)", Price = 20.00m, Description = "Ručno pranje metodom 2 kante + safe drying." },
            new ServicePackageItemEntity { Name = "Decontamination – Iron Remover", Price = 15.00m, Description = "Uklanjanje metalnih čestica s laka i felgi." },
            new ServicePackageItemEntity { Name = "Decontamination – Tar Remover", Price = 15.00m, Description = "Uklanjanje katrana i tvrdokornih mrlja." },
            new ServicePackageItemEntity { Name = "Clay bar tretman", Price = 25.00m, Description = "Mehanička dekontaminacija za glatku površinu." },
            new ServicePackageItemEntity { Name = "Poliranje – One-step (light correction)", Price = 80.00m, Description = "Jednostepeno poliranje za sjaj i blagu korekciju." },
            new ServicePackageItemEntity { Name = "Poliranje – Two-step (medium correction)", Price = 140.00m, Description = "Dvostepeno poliranje za jaču korekciju i finiš." },
            new ServicePackageItemEntity { Name = "Zaštita laka – Sealant (6–8 sedmica)", Price = 25.00m, Description = "Brza zaštita i hidrofobnost." },
            new ServicePackageItemEntity { Name = "Zaštita laka – Carnauba Wax", Price = 30.00m, Description = "Topli sjaj + zaštita (kraći vijek)." },
            new ServicePackageItemEntity { Name = "Keramički premaz – 1 godina", Price = 180.00m, Description = "Keramička zaštita s pripremom (osnovni nivo)." },
            new ServicePackageItemEntity { Name = "Keramički premaz – 3 godine", Price = 320.00m, Description = "Naprednija keramika + bolja trajnost." },
            new ServicePackageItemEntity { Name = "Felge – dubinsko čišćenje + zaštita", Price = 30.00m, Description = "Detaljno čišćenje felgi i zaštitni sloj." },
            new ServicePackageItemEntity { Name = "Gume – dressing", Price = 10.00m, Description = "Obnova izgleda guma (saten/shine)." },
            new ServicePackageItemEntity { Name = "Enterijer – dubinsko čišćenje", Price = 60.00m, Description = "Usisavanje + čišćenje plastika + tekstil." },
            new ServicePackageItemEntity { Name = "Koža – čišćenje + kondicioniranje", Price = 45.00m, Description = "Čišćenje kože + zaštita/kondicioner." }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: service package items added.");
    }
}
