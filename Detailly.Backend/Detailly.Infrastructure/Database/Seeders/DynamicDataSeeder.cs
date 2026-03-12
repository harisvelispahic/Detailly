using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Shared;
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
        if (await context.ProductCategories.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        context.ProductCategories.AddRange(
            new ProductCategoryEntity
            {
                Name = "Exterior Care",
                Description = "Products used for washing, decontamination, drying, and protecting the vehicle exterior.",
                IsEnabled = true,
                CreatedAtUtc = now
            },
            new ProductCategoryEntity
            {
                Name = "Interior Care",
                Description = "Products intended for cleaning, restoring, and protecting dashboards, seats, carpets, and trim.",
                IsEnabled = true,
                CreatedAtUtc = now
            },
            new ProductCategoryEntity
            {
                Name = "Accessories",
                Description = "Useful detailing accessories such as brushes, towels, applicators, bottles, and tools.",
                IsEnabled = true,
                CreatedAtUtc = now
            },
            new ProductCategoryEntity
            {
                Name = "Paint Protection",
                Description = "Sealants, waxes, ceramic sprays, and coating-related products for long-term paint protection.",
                IsEnabled = true,
                CreatedAtUtc = now
            }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: product categories added.");
    }

    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        if (await context.ApplicationUsers.AnyAsync())
            return;

        var hasher = new PasswordHasher<ApplicationUserEntity>();
        var now = DateTime.UtcNow;

        var admin = new ApplicationUserEntity
        {
            Email = "admin@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "Admin123!"),
            FirstName = "System",
            LastName = "Administrator",
            Username = "admin",
            Phone = "+38761111000",
            CompanyName = "Detailly",
            IsAdmin = true,
            IsFleet = false,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        admin.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 20,
            ApplicationUser = admin,
            CreatedAtUtc = now
        };

        var manager = new ApplicationUserEntity
        {
            Email = "manager@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "Manager123!"),
            FirstName = "Operations",
            LastName = "Manager",
            Username = "manager",
            Phone = "+38761111001",
            CompanyName = "Detailly",
            IsManager = true,
            IsFleet = false,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        manager.Wallet = new WalletEntity
        {
            Balance = 750m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 750m,
            PercentageAdded = 15,
            ApplicationUser = manager,
            CreatedAtUtc = now
        };

        var employee = new ApplicationUserEntity
        {
            Email = "employee@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "Employee123!"),
            FirstName = "Workshop",
            LastName = "Technician",
            Username = "employee",
            Phone = "+38761111002",
            IsEmployee = true,
            IsFleet = false,
            IsEnabled = true,
            EmployeeWorkMode = EmployeeWorkMode.InShop,
            CreatedAtUtc = now
        };
        employee.Wallet = new WalletEntity
        {
            Balance = 150m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 150m,
            PercentageAdded = 10,
            ApplicationUser = employee,
            CreatedAtUtc = now
        };

        var fleetClient = new ApplicationUserEntity
        {
            Email = "fleet@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "Fleet123!"),
            FirstName = "Fleet",
            LastName = "Client",
            Username = "fleetclient",
            Phone = "+38761111003",
            CompanyName = "Bosna Logistics",
            IsFleet = true,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        fleetClient.Wallet = new WalletEntity
        {
            Balance = 2500m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 2500m,
            PercentageAdded = 20,
            ApplicationUser = fleetClient,
            CreatedAtUtc = now
        };

        var demoClient = new ApplicationUserEntity
        {
            Email = "client@detailly.local",
            PasswordHash = hasher.HashPassword(null!, "Client123!"),
            FirstName = "Demo",
            LastName = "Customer",
            Username = "client",
            Phone = "+38761111004",
            IsFleet = false,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        demoClient.Wallet = new WalletEntity
        {
            Balance = 320m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 320m,
            PercentageAdded = 10,
            ApplicationUser = demoClient,
            CreatedAtUtc = now
        };

        var swaggerDummy = new ApplicationUserEntity
        {
            //Email = "string@detailly.local",
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            FirstName = "Swagger",
            LastName = "Dummy",
            Username = "swagger-dummy",
            Phone = "+38761111005",
            IsAdmin = true,
            IsFleet = false,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        swaggerDummy.Wallet = new WalletEntity
        {
            Balance = 100m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 100m,
            PercentageAdded = 10,
            ApplicationUser = swaggerDummy,
            CreatedAtUtc = now
        };

        var haris = new ApplicationUserEntity
        {
            Email = "haris.velispahic@edu.fit.ba",
            PasswordHash = hasher.HashPassword(null!, "haris123"),
            FirstName = "Haris",
            LastName = "Velispahic",
            Username = "haris123",
            Phone = "+38761111006",
            IsAdmin = true,
            IsFleet = false,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        haris.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 10,
            ApplicationUser = haris,
            CreatedAtUtc = now
        };
        var danis = new ApplicationUserEntity
        {
            Email = "danis.music@edu.fit.ba",
            PasswordHash = hasher.HashPassword(null!, "danis123"),
            FirstName = "Danis",
            LastName = "Music",
            Username = "danis123",
            Phone = "+38761111007",
            IsAdmin = true,
            IsFleet = false,
            IsEnabled = true,
            CreatedAtUtc = now
        };
        danis.Wallet = new WalletEntity
        {
            Balance = 1000m,
            Currency = CurrencyName.BAM,
            TotalDeposited = 1000m,
            PercentageAdded = 10,
            ApplicationUser = danis,
            CreatedAtUtc = now
        };

        context.ApplicationUsers.AddRange(admin, manager, employee, fleetClient, demoClient, swaggerDummy, haris, danis);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dynamic seed: demo users added.");
    }

    private static async Task SeedVehicleCategoriesAsync(DatabaseContext context)
    {
        if (await context.VehicleCategories.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        context.VehicleCategories.AddRange(
            new VehicleCategoryEntity
            {
                Name = "SUV",
                Description = "Sport utility vehicles with larger body dimensions and moderately increased service complexity.",
                BasePriceMultiplier = 1.15m,
                CreatedAtUtc = now
            },
            new VehicleCategoryEntity
            {
                Name = "Sedan",
                Description = "Standard passenger sedans used as the pricing baseline for many detailing services.",
                BasePriceMultiplier = 1.00m,
                CreatedAtUtc = now
            },
            new VehicleCategoryEntity
            {
                Name = "Roadster",
                Description = "Two-seat open-top vehicles that require more delicate exterior and interior handling.",
                BasePriceMultiplier = 1.10m,
                CreatedAtUtc = now
            },
            new VehicleCategoryEntity
            {
                Name = "Hatchback",
                Description = "Compact hatchback vehicles with slightly reduced detailing effort compared to sedans.",
                BasePriceMultiplier = 0.95m,
                CreatedAtUtc = now
            },
            new VehicleCategoryEntity
            {
                Name = "Van",
                Description = "Larger multi-purpose vans with increased surface area and interior cleaning effort.",
                BasePriceMultiplier = 1.25m,
                CreatedAtUtc = now
            },
            new VehicleCategoryEntity
            {
                Name = "Sports Car",
                Description = "Performance-oriented vehicles that often require premium handling and paint-safe processes.",
                BasePriceMultiplier = 1.20m,
                CreatedAtUtc = now
            }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: vehicle categories added.");
    }

    private static async Task SeedProductsAsync(DatabaseContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var categories = await context.ProductCategories
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        var exteriorCareId = categories["Exterior Care"];
        var interiorCareId = categories["Interior Care"];
        var accessoriesId = categories["Accessories"];
        var paintProtectionId = categories["Paint Protection"];

        var products = new List<ProductEntity>
        {
            new ProductEntity
            {
                Name = "pH Neutral Snow Foam",
                Description = "A safe pre-wash foam designed to loosen traffic film, dust, and light grime without stripping existing protection layers.",
                ProductNumber = Guid.NewGuid().ToString("N"),
                Price = 24.90m,
                Currency = CurrencyName.BAM,
                CategoryId = exteriorCareId,
                IsEnabled = true,
                Tags = "pre-wash,snow-foam,paint-safe",
                CreatedAtUtc = now,
                Inventory = new InventoryEntity
                {
                    QuantityInStock = 45,
                    ReorderLevel = 10,
                    ReorderQuantity = 25,
                    IsDeleted = false,
                    CreatedAtUtc = now
                },
                Images = new List<ImageEntity>
                {
                    new ImageEntity
                    {
                        ImageUrl = "https://images.detailly.local/products/ph-neutral-snow-foam-main.jpg",
                        AltText = "Bottle of pH Neutral Snow Foam",
                        IsThumbnail = true,
                        DisplayOrder = 1,
                        CreatedAtUtc = now
                    }
                }
            },
            new ProductEntity
            {
                Name = "Interior All-Purpose Cleaner",
                Description = "Versatile interior cleaner for dashboards, plastics, vinyl, door panels, and general cabin maintenance.",
                ProductNumber = Guid.NewGuid().ToString("N"),
                Price = 19.50m,
                Currency = CurrencyName.BAM,
                CategoryId = interiorCareId,
                IsEnabled = true,
                Tags = "interior,cleaner,plastic,vinyl",
                CreatedAtUtc = now,
                Inventory = new InventoryEntity
                {
                    QuantityInStock = 38,
                    ReorderLevel = 12,
                    ReorderQuantity = 24,
                    IsDeleted = false,
                    CreatedAtUtc = now
                },
                Images = new List<ImageEntity>
                {
                    new ImageEntity
                    {
                        ImageUrl = "https://images.detailly.local/products/interior-apc-main.jpg",
                        AltText = "Interior All-Purpose Cleaner spray bottle",
                        IsThumbnail = true,
                        DisplayOrder = 1,
                        CreatedAtUtc = now
                    }
                }
            },
            new ProductEntity
            {
                Name = "Microfiber Drying Towel",
                Description = "Large twisted-loop microfiber towel optimized for streak-free drying with high absorbency and safe paint contact.",
                ProductNumber = Guid.NewGuid().ToString("N"),
                Price = 29.90m,
                Currency = CurrencyName.BAM,
                CategoryId = accessoriesId,
                IsEnabled = true,
                Tags = "microfiber,drying,towel,accessory",
                CreatedAtUtc = now,
                Inventory = new InventoryEntity
                {
                    QuantityInStock = 60,
                    ReorderLevel = 15,
                    ReorderQuantity = 30,
                    IsDeleted = false,
                    CreatedAtUtc = now
                },
                Images = new List<ImageEntity>
                {
                    new ImageEntity
                    {
                        ImageUrl = "https://images.detailly.local/products/microfiber-drying-towel-main.jpg",
                        AltText = "Folded microfiber drying towel",
                        IsThumbnail = true,
                        DisplayOrder = 1,
                        CreatedAtUtc = now
                    }
                }
            },
            new ProductEntity
            {
                Name = "SiO2 Ceramic Spray Sealant",
                Description = "Quick-application spray sealant that boosts gloss, water behavior, and short-term paint protection after maintenance washes.",
                ProductNumber = Guid.NewGuid().ToString("N"),
                Price = 34.90m,
                Currency = CurrencyName.BAM,
                CategoryId = paintProtectionId,
                IsEnabled = true,
                Tags = "sealant,ceramic,sio2,protection",
                CreatedAtUtc = now,
                Inventory = new InventoryEntity
                {
                    QuantityInStock = 28,
                    ReorderLevel = 8,
                    ReorderQuantity = 18,
                    IsDeleted = false,
                    CreatedAtUtc = now
                },
                Images = new List<ImageEntity>
                {
                    new ImageEntity
                    {
                        ImageUrl = "https://images.detailly.local/products/sio2-ceramic-spray-main.jpg",
                        AltText = "SiO2 Ceramic Spray Sealant bottle",
                        IsThumbnail = true,
                        DisplayOrder = 1,
                        CreatedAtUtc = now
                    }
                }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dynamic seed: products with inventory and images added.");
    }

    private static async Task SeedAddressesAsync(DatabaseContext context)
    {
        if (await context.Addresses.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        context.Addresses.AddRange(
            new AddressEntity
            {
                Street = "Zmaja od Bosne 12",
                City = "Sarajevo",
                PostalCode = "71000",
                Region = "Federation of Bosnia and Herzegovina",
                Country = "Bosnia and Herzegovina",
                Latitude = 43.8563m,
                Longitude = 18.4131m,
                ApplicationUserId = users["admin"],
                CreatedAtUtc = now
            },
            new AddressEntity
            {
                Street = "Braće Fejića 12a",
                City = "Mostar",
                PostalCode = "88000",
                Region = "Federation of Bosnia and Herzegovina",
                Country = "Bosnia and Herzegovina",
                Latitude = 43.3438m,
                Longitude = 17.8078m,
                ApplicationUserId = users["manager"],
                CreatedAtUtc = now
            },
            new AddressEntity
            {
                Street = "Mehmedalije Maka Dizdara 21",
                City = "Tuzla",
                PostalCode = "75000",
                Region = "Federation of Bosnia and Herzegovina",
                Country = "Bosnia and Herzegovina",
                Latitude = 44.5384m,
                Longitude = 18.6671m,
                ApplicationUserId = users["employee"],
                CreatedAtUtc = now
            },
            new AddressEntity
            {
                Street = "Bulevar Mira 45",
                City = "Banja Luka",
                PostalCode = "78000",
                Region = "Republika Srpska",
                Country = "Bosnia and Herzegovina",
                Latitude = 44.7722m,
                Longitude = 17.1910m,
                ApplicationUserId = users["fleetclient"],
                CreatedAtUtc = now
            },
            new AddressEntity
            {
                Street = "Titova 31",
                City = "Zenica",
                PostalCode = "72000",
                Region = "Federation of Bosnia and Herzegovina",
                Country = "Bosnia and Herzegovina",
                Latitude = 44.2034m,
                Longitude = 17.9077m,
                ApplicationUserId = users["client"],
                CreatedAtUtc = now
            }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: addresses added.");
    }

    private static async Task SeedServicePackageItemsAsync(DatabaseContext context)
    {
        if (await context.ServicePackageItems.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        context.ServicePackageItems.AddRange(
            new ServicePackageItemEntity
            {
                Name = "Pre-Wash / Snow Foam",
                Price = 10.00m,
                Description = "Foam-based pre-wash step that softens dirt and reduces contact wash risk.",
                DurationMinutes = 20,
                RequiredEmployees = 1,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Hand Wash (Two-Bucket Method)",
                Price = 20.00m,
                Description = "Careful hand wash using the two-bucket method and safe drying techniques.",
                DurationMinutes = 35,
                RequiredEmployees = 1,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Iron Fallout Decontamination",
                Price = 15.00m,
                Description = "Chemical decontamination step for removing embedded iron particles from paint and wheels.",
                DurationMinutes = 25,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Tar and Adhesive Removal",
                Price = 15.00m,
                Description = "Targeted treatment for tar spots, road residue, and stubborn adhesive contamination.",
                DurationMinutes = 25,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Clay Bar Treatment",
                Price = 25.00m,
                Description = "Mechanical decontamination for a smoother paint surface before polishing or protection.",
                DurationMinutes = 40,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "One-Step Machine Polishing",
                Price = 80.00m,
                Description = "Single-stage polish that improves gloss and corrects light paint defects.",
                DurationMinutes = 120,
                RequiredEmployees = 1,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Two-Step Paint Correction",
                Price = 140.00m,
                Description = "More advanced machine polishing for deeper correction and refined finishing.",
                DurationMinutes = 240,
                RequiredEmployees = 1,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Paint Sealant Protection",
                Price = 25.00m,
                Description = "Synthetic paint sealant that adds gloss and hydrophobic behavior for several weeks.",
                DurationMinutes = 30,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Carnauba Wax Finish",
                Price = 30.00m,
                Description = "Warm gloss wax protection for customers who prefer a classic finish.",
                DurationMinutes = 30,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Ceramic Coating - 1 Year",
                Price = 180.00m,
                Description = "Entry-level ceramic protection package with surface preparation and coating application.",
                DurationMinutes = 180,
                RequiredEmployees = 1,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Ceramic Coating - 3 Years",
                Price = 320.00m,
                Description = "Longer-term ceramic coating solution with enhanced durability and gloss retention.",
                DurationMinutes = 300,
                RequiredEmployees = 2,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Wheel Deep Cleaning and Protection",
                Price = 30.00m,
                Description = "Detailed wheel treatment including barrel cleaning, face cleaning, and protection layer.",
                DurationMinutes = 45,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Tire Dressing",
                Price = 10.00m,
                Description = "Finishing step for restoring a clean, rich tire appearance.",
                DurationMinutes = 15,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Interior Deep Cleaning",
                Price = 60.00m,
                Description = "Comprehensive interior service covering vacuuming, plastics, fabric surfaces, and finishing.",
                DurationMinutes = 90,
                RequiredEmployees = 1,
                IsAddon = false,
                IsActive = true,
                CreatedAtUtc = now
            },
            new ServicePackageItemEntity
            {
                Name = "Leather Cleaning and Conditioning",
                Price = 45.00m,
                Description = "Safe leather cleansing followed by conditioning to preserve softness and appearance.",
                DurationMinutes = 60,
                RequiredEmployees = 1,
                IsAddon = true,
                IsActive = true,
                CreatedAtUtc = now
            }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: service package items added.");
    }
}