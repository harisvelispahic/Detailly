using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Shared;
using Detailly.Domain.Entities.Vehicle;
using Detailly.Domain.Entities.Sales;
using Microsoft.AspNetCore.Identity;

namespace Detailly.Infrastructure.Database.Seeders;

public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        await context.Database.EnsureCreatedAsync();

        await SeedProductCategoriesAsync(context);
        await SeedVehicleCategoriesAsync(context);
        await SeedUsersAsync(context);
        await SeedProductsAsync(context);
        await SeedServicePackageItemsAsync(context);
        await SeedUserAddressesAsync(context);
        await SeedLocationsAsync(context);
        await SeedServicePackagesAsync(context);
        await SeedVehiclesAsync(context);
        await SeedEmployeeShiftsAsync(context);
        await SeedBookingsAsync(context);
        await SeedReviewsAsync(context);
        await SeedReactionsAsync(context);
    }

    // ─────────────────────────── PRODUCT CATEGORIES ───────────────────────────

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
        Console.WriteLine("✅ Seed: product categories.");
    }

    // ─────────────────────────── VEHICLE CATEGORIES ───────────────────────────

    private static async Task SeedVehicleCategoriesAsync(DatabaseContext context)
    {
        if (await context.VehicleCategories.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        context.VehicleCategories.AddRange(
            new VehicleCategoryEntity { Name = "Sedan",        Description = "Standard passenger sedans used as the pricing baseline for detailing services.",                     BasePriceMultiplier = 1.00m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "Hatchback",    Description = "Compact hatchback vehicles with slightly reduced detailing effort compared to sedans.",               BasePriceMultiplier = 0.95m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "SUV",          Description = "Sport utility vehicles with larger body dimensions and moderately increased service complexity.",     BasePriceMultiplier = 1.15m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "Sports Car",   Description = "Performance-oriented vehicles requiring premium handling and paint-safe processes.",                  BasePriceMultiplier = 1.20m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "Coupe",        Description = "Two-door passenger vehicles with a sportier profile than a sedan.",                                  BasePriceMultiplier = 1.05m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "Roadster",     Description = "Two-seat open-top vehicles requiring more delicate exterior and interior handling.",                  BasePriceMultiplier = 1.10m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "Van",          Description = "Larger multi-purpose vans with increased surface area and interior cleaning effort.",                 BasePriceMultiplier = 1.25m, CreatedAtUtc = now },
            new VehicleCategoryEntity { Name = "Pickup Truck", Description = "Full-size pickup trucks with extra surface area and bed cleaning requirements.",                      BasePriceMultiplier = 1.30m, CreatedAtUtc = now }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: vehicle categories.");
    }

    // ─────────────────────────── USERS ───────────────────────────

    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        if (await context.ApplicationUsers.AnyAsync())
            return;

        var hasher = new PasswordHasher<ApplicationUserEntity>();
        var now = DateTime.UtcNow;

        ApplicationUserEntity Make(
            string email, string password,
            string first, string last, string username, string phone,
            bool isAdmin = false, bool isManager = false, bool isEmployee = false, bool isFleet = false,
            string? company = null, decimal balance = 0m, int pct = 10)
        {
            var u = new ApplicationUserEntity
            {
                Email = email,
                PasswordHash = hasher.HashPassword(null!, password),
                FirstName = first,
                LastName = last,
                Username = username,
                Phone = phone,
                CompanyName = company,
                IsAdmin = isAdmin,
                IsManager = isManager,
                IsEmployee = isEmployee,
                IsFleet = isFleet,
                IsEnabled = true,
                CreatedAtUtc = now
            };
            u.Wallet = new WalletEntity
            {
                Balance = balance,
                Currency = CurrencyName.BAM,
                TotalDeposited = balance,
                PercentageAdded = pct,
                ApplicationUser = u,
                CreatedAtUtc = now
            };
            u.Cart = new CartEntity
            {
                IsEmpty = true,
                TotalAmount = 0m,
                Status = CartStatus.Active,
                ApplicationUser = u,
                CreatedAtUtc = now
            };
            return u;
        }

        context.ApplicationUsers.AddRange(
            // ── Administrators ──────────────────────────────────────────
            Make("admin@detailly.local",              "Admin123!",    "System",    "Administrator", "admin",        "+38761111000", isAdmin: true,    company: "Detailly",                    balance: 1000m, pct: 20),
            Make("haris.velispahic@edu.fit.ba",       "haris123",     "Haris",     "Velispahic",    "haris123",     "+38761111006", isAdmin: true,                                            balance: 1000m),
            Make("danis.music@edu.fit.ba",            "danis123",     "Danis",     "Music",         "danis123",     "+38761111007", isAdmin: true,                                            balance: 1000m),

            // ── Managers ────────────────────────────────────────────────
            Make("manager@detailly.local",            "Manager123!",  "Operations","Manager",       "manager",      "+38761111001", isManager: true,  company: "Detailly",                    balance: 750m,  pct: 15),
            Make("selma.bradaric@detailly.local",     "Manager123!",  "Selma",     "Bradaric",      "sbradaric",    "+38761222001", isManager: true,  company: "Detailly",                    balance: 600m,  pct: 15),

            // ── Employees ───────────────────────────────────────────────
            Make("employee@detailly.local",           "Employee123!", "Workshop",  "Technician",    "employee",     "+38761111002", isEmployee: true,                                         balance: 150m),
            Make("amir.hodzic@detailly.local",        "Employee123!", "Amir",      "Hodzic",        "ahodzic",      "+38761222002", isEmployee: true,                                         balance: 120m),
            Make("tarik.kevric@detailly.local",       "Employee123!", "Tarik",     "Kevric",        "tkevric",      "+38761222003", isEmployee: true,                                         balance: 100m),
            Make("emir.muric@detailly.local",          "Employee123!", "Emir",      "Murić",         "emuric",       "+38761222004", isEmployee: true,                                         balance: 110m),
            Make("jasmin.tahirovic@detailly.local",    "Employee123!", "Jasmin",    "Tahirović",     "jtahirovic",   "+38761222005", isEmployee: true,                                         balance: 90m),
            Make("amra.basic@detailly.local",          "Employee123!", "Amra",      "Bašić",         "abasic",       "+38761222006", isEmployee: true,                                         balance: 95m),
            Make("nedzad.kukic@detailly.local",        "Employee123!", "Nedžad",    "Kukić",         "nkukic",       "+38761222007", isEmployee: true,                                         balance: 105m),
            Make("maja.petrovic@detailly.local",       "Employee123!", "Maja",      "Petrović",      "mpetrovic",    "+38761222008", isEmployee: true,                                         balance: 85m),
            Make("senad.kadic@detailly.local",         "Employee123!", "Senad",     "Kadić",         "skadic",       "+38761222009", isEmployee: true,                                         balance: 115m),
            Make("lamija.hadzic@detailly.local",       "Employee123!", "Lamija",    "Hadžić",        "lhadzic",      "+38761222010", isEmployee: true,                                         balance: 88m),

            // ── Fleet clients ────────────────────────────────────────────
            Make("fleet@detailly.local",              "Fleet123!",    "Fleet",     "Client",        "fleetclient",  "+38761111003", isFleet: true,    company: "Bosna Logistics",              balance: 2500m, pct: 20),
            Make("nedim.ajanovic@sarajevo-trans.ba",  "Fleet123!",    "Nedim",     "Ajanovic",      "najanovic",    "+38761333001", isFleet: true,    company: "Sarajevo Transport d.o.o.",    balance: 3000m, pct: 20),

            // ── Regular clients ──────────────────────────────────────────
            Make("client@detailly.local",             "Client123!",   "Demo",      "Customer",      "client",       "+38761111004",                                                           balance: 320m),
            Make("lejla.kovacevic@gmail.com",         "Client123!",   "Lejla",     "Kovacevic",     "lkovacevic",   "+38761444001",                                                           balance: 150m),
            Make("mirza.begovic@gmail.com",           "Client123!",   "Mirza",     "Begovic",       "mbegovic",     "+38761444002",                                                           balance: 80m),

            // ── Swagger dummy (API testing) ──────────────────────────────
            Make("string",                            "string",       "Swagger",   "Dummy",         "swagger-dummy","+38761111005", isAdmin: true,                                            balance: 100m)
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: users.");
    }

    // ─────────────────────────── PRODUCTS ───────────────────────────

    private static async Task SeedProductsAsync(DatabaseContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var cats = await context.ProductCategories
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        var extId = cats["Exterior Care"];
        var intId = cats["Interior Care"];
        var accId = cats["Accessories"];
        var ppId  = cats["Paint Protection"];

        ProductEntity P(string name, string desc, decimal price, int catId, string tags, int stock = 30, int reorderLvl = 10, int reorderQty = 20) =>
            new ProductEntity
            {
                Name = name,
                Description = desc,
                ProductNumber = Guid.NewGuid().ToString("N"),
                Price = price,
                Currency = CurrencyName.BAM,
                CategoryId = catId,
                IsEnabled = true,
                Tags = tags,
                CreatedAtUtc = now,
                Inventory = new InventoryEntity
                {
                    QuantityInStock = stock,
                    ReorderLevel = reorderLvl,
                    ReorderQuantity = reorderQty,
                    CreatedAtUtc = now
                }
            };

        context.Products.AddRange(
            // ── Exterior Care ────────────────────────────────────────────
            P("pH Neutral Snow Foam",         "Safe pre-wash foam that loosens traffic film and light grime without stripping existing protection layers.",                   24.90m, extId, "pre-wash,snow-foam,paint-safe",          45, 10, 25),
            P("Citrus Pre-Wash Degreaser",    "Concentrated citrus degreaser for heavy traffic film, bugs, and road grime removal before the contact wash.",                  16.90m, extId, "pre-wash,degreaser,citrus",               35, 10, 20),
            P("Two-Bucket Wash Shampoo",      "Highly lubricating car shampoo designed for the two-bucket method, safe on coatings and waxes.",                              18.50m, extId, "shampoo,hand-wash,safe",                  50, 12, 24),
            P("Iron Remover Fallout Spray",   "pH-reactive iron remover that turns purple on contact and dissolves metallic contamination from paint and wheels.",            19.90m, extId, "iron-remover,decontamination,fallout",    30,  8, 18),
            P("Bug and Tar Remover",          "Solvent-based cleaner for road tar, bug splatter, and adhesive residue — safe on painted surfaces.",                          17.90m, extId, "tar,bugs,remover,exterior",               28,  8, 16),
            P("Waterless Car Wash Spray",     "Spray-on formula for light dust and fingerprint removal between full washes. No water required.",                              22.50m, extId, "waterless,quick-detailer,maintenance",    25,  8, 15),

            // ── Interior Care ────────────────────────────────────────────
            P("Interior All-Purpose Cleaner", "Versatile interior cleaner for dashboards, plastics, vinyl, and door panels.",                                                 19.50m, intId, "interior,cleaner,plastic,vinyl",          38, 12, 24),
            P("Leather Cleaner & Conditioner","Two-in-one leather care product that cleans and conditions seats and trim without silicone or residue.",                        28.90m, intId, "leather,cleaner,conditioner",             30, 10, 20),
            P("Fabric & Upholstery Cleaner",  "Water-based foam cleaner for fabric seats, carpets, and headliners. Safe on all textiles.",                                    21.50m, intId, "fabric,upholstery,carpet,interior",       25,  8, 16),
            P("Odour Eliminator Spray",       "Enzymatic odour neutralizer for musty, smoke, or pet smells — safe to spray directly on fabrics.",                            15.90m, intId, "odour,smell,interior,fabric",             20,  5, 12),
            P("Dashboard Protectant Spray",   "UV-blocking dashboard and trim protectant that leaves a natural, non-greasy finish.",                                          13.90m, intId, "dashboard,protectant,uv,trim",            32, 10, 18),

            // ── Accessories ──────────────────────────────────────────────
            P("Microfiber Drying Towel",      "Large twisted-loop microfiber towel for streak-free drying with high absorbency and safe paint contact.",                      29.90m, accId, "microfiber,drying,towel",                 60, 15, 30),
            P("Clay Bar (200g)",              "Medium-grade detailing clay for removing bonded surface contamination before polishing or protection.",                         24.90m, accId, "clay,decontamination,paint-prep",         28,  8, 16),
            P("Foam Lance (Universal)",       "Universal foam cannon compatible with standard garden hose adapters for thick snow foam application.",                          54.90m, accId, "foam-lance,pre-wash,accessory",           15,  4, 10),
            P("Detailing Brush Set (5 pcs)",  "Five soft-bristle brushes for vents, emblems, gaps, and tight interior areas.",                                               19.90m, accId, "brushes,detailing,interior,tools",        22,  6, 12),
            P("Foam Applicator Pads (6 pcs)", "Round foam applicator pads for waxes, sealants, and ceramic sprays applied by hand.",                                          9.90m, accId, "applicator,pads,wax,sealant",             40, 10, 20),
            P("Wheel Woolie Brush Set",       "Long-handle and barrel brushes for safe wheel and rim cleaning without scratching.",                                           17.90m, accId, "wheel,brush,cleaning,accessory",          18,  5, 12),

            // ── Paint Protection ─────────────────────────────────────────
            P("SiO2 Ceramic Spray Sealant",   "Quick-application spray sealant that boosts gloss, hydrophobicity, and short-term paint protection.",                         34.90m, ppId,  "sealant,ceramic,sio2,protection",         28,  8, 18),
            P("Synthetic Paste Wax",          "Long-lasting synthetic paste wax for deep gloss and durable paint protection with easy on-off application.",                  39.90m, ppId,  "wax,paste,protection,gloss",              22,  6, 14),
            P("Carnauba Liquid Wax",          "Premium liquid carnauba wax delivering warm, rich gloss and short-term protection with minimal effort.",                      32.90m, ppId,  "carnauba,wax,liquid,gloss",               25,  8, 16),
            P("Paint Sealant Spray",          "Polymer-based spray sealant for fast protection on freshly clayed or polished paintwork.",                                    27.90m, ppId,  "sealant,polymer,spray,protection",        30,  8, 18),
            P("Graphene Coating Top-Up",      "Spray-on graphene-infused maintenance coat for extending the life of a base ceramic or graphene coating.",                    49.90m, ppId,  "graphene,coating,maintenance,topup",      18,  5, 10)
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: products.");
    }

    // ─────────────────────────── SERVICE PACKAGE ITEMS ───────────────────────────

    private static async Task SeedServicePackageItemsAsync(DatabaseContext context)
    {
        if (await context.ServicePackageItems.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        context.ServicePackageItems.AddRange(
            new ServicePackageItemEntity { Name = "Pre-Wash / Snow Foam",              Price =  10.00m, Description = "Foam-based pre-wash that softens dirt and reduces contact-wash risk.",                                      DurationMinutes =  20, RequiredEmployees = 1, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Hand Wash (Two-Bucket Method)",     Price =  20.00m, Description = "Careful hand wash using the two-bucket method with safe drying techniques.",                               DurationMinutes =  35, RequiredEmployees = 1, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Iron Fallout Decontamination",      Price =  15.00m, Description = "Chemical decontamination step for removing embedded iron particles from paint and wheels.",                 DurationMinutes =  25, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Tar and Adhesive Removal",          Price =  15.00m, Description = "Targeted treatment for tar spots, road residue, and stubborn adhesive contamination.",                      DurationMinutes =  25, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Clay Bar Treatment",                Price =  25.00m, Description = "Mechanical decontamination for a smoother paint surface before polishing or protection.",                   DurationMinutes =  40, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "One-Step Machine Polishing",        Price =  80.00m, Description = "Single-stage polish that improves gloss and corrects light paint defects.",                                 DurationMinutes = 120, RequiredEmployees = 1, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Two-Step Paint Correction",         Price = 140.00m, Description = "Advanced two-stage machine polishing for deeper defect correction and a refined finish.",                   DurationMinutes = 240, RequiredEmployees = 1, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Paint Sealant Protection",          Price =  25.00m, Description = "Synthetic paint sealant that adds gloss and hydrophobic behavior for several weeks.",                       DurationMinutes =  30, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Carnauba Wax Finish",               Price =  30.00m, Description = "Warm gloss wax protection for customers who prefer a classic hand-applied finish.",                         DurationMinutes =  30, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Ceramic Coating - 1 Year",          Price = 180.00m, Description = "Entry-level ceramic protection with surface preparation and coating application.",                           DurationMinutes = 180, RequiredEmployees = 1, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Ceramic Coating - 3 Years",         Price = 320.00m, Description = "Longer-term ceramic coating with enhanced durability and gloss retention, requires two technicians.",        DurationMinutes = 300, RequiredEmployees = 2, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Wheel Deep Cleaning and Protection",Price =  30.00m, Description = "Detailed wheel treatment: barrel cleaning, face cleaning, and a protection layer.",                         DurationMinutes =  45, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Tire Dressing",                     Price =  10.00m, Description = "Finishing step for a clean, rich tire appearance.",                                                         DurationMinutes =  15, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Interior Deep Cleaning",            Price =  60.00m, Description = "Comprehensive interior service: vacuuming, plastics, fabric surfaces, and finishing wipe-down.",            DurationMinutes =  90, RequiredEmployees = 1, IsAddon = false, IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Leather Cleaning and Conditioning", Price =  45.00m, Description = "Safe leather cleansing followed by conditioning to preserve softness and appearance.",                      DurationMinutes =  60, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Engine Bay Cleaning",               Price =  35.00m, Description = "Degreasing and cleaning of the engine bay with plastic and rubber dressing.",                               DurationMinutes =  45, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Headlight Restoration",             Price =  40.00m, Description = "Wet sanding and polishing of oxidized headlight lenses, finished with UV sealant.",                         DurationMinutes =  60, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now },
            new ServicePackageItemEntity { Name = "Glass Water Repellent Treatment",   Price =  20.00m, Description = "Hydrophobic glass coating applied to all windows and mirrors for improved wet-weather visibility.",          DurationMinutes =  20, RequiredEmployees = 1, IsAddon = true,  IsActive = true, CreatedAtUtc = now }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: service package items.");
    }

    // ─────────────────────────── USER ADDRESSES ───────────────────────────

    private static async Task SeedUserAddressesAsync(DatabaseContext context)
    {
        if (await context.Addresses.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        context.Addresses.AddRange(
            new AddressEntity { Street = "Zmaja od Bosne 12",           City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8563m, Longitude = 18.4131m, ApplicationUserId = users["admin"],        CreatedAtUtc = now },
            new AddressEntity { Street = "Braće Fejića 12a",            City = "Mostar",     PostalCode = "88000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.3438m, Longitude = 17.8078m, ApplicationUserId = users["manager"],      CreatedAtUtc = now },
            new AddressEntity { Street = "Grbavička 1",                 City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8497m, Longitude = 18.3985m, ApplicationUserId = users["sbradaric"],    CreatedAtUtc = now },
            new AddressEntity { Street = "Mehmedalije Maka Dizdara 21", City = "Tuzla",      PostalCode = "75000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 44.5384m, Longitude = 18.6671m, ApplicationUserId = users["employee"],     CreatedAtUtc = now },
            new AddressEntity { Street = "Fra Anđela Zvizdovića 1",     City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8556m, Longitude = 18.4090m, ApplicationUserId = users["ahodzic"],      CreatedAtUtc = now },
            new AddressEntity { Street = "Hamdije Kreševljakovića 12",  City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8602m, Longitude = 18.4254m, ApplicationUserId = users["tkevric"],      CreatedAtUtc = now },
            new AddressEntity { Street = "Bulevar Mira 45",             City = "Banja Luka", PostalCode = "78000", Region = "Republika Srpska",                     Country = "Bosnia and Herzegovina", Latitude = 44.7722m, Longitude = 17.1910m, ApplicationUserId = users["fleetclient"],  CreatedAtUtc = now },
            new AddressEntity { Street = "Džemala Bijedića 89",         City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8452m, Longitude = 18.3884m, ApplicationUserId = users["najanovic"],    CreatedAtUtc = now },
            new AddressEntity { Street = "Titova 31",                   City = "Zenica",     PostalCode = "72000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 44.2034m, Longitude = 17.9077m, ApplicationUserId = users["client"],       CreatedAtUtc = now },
            new AddressEntity { Street = "Aleja Lipa 22",               City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8481m, Longitude = 18.3756m, ApplicationUserId = users["lkovacevic"],   CreatedAtUtc = now },
            new AddressEntity { Street = "Safeta Bašića 2",             City = "Ilidža",     PostalCode = "71210", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8295m, Longitude = 18.3108m, ApplicationUserId = users["mbegovic"],     CreatedAtUtc = now },
            new AddressEntity { Street = "Skenderija 10",               City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8577m, Longitude = 18.4200m, ApplicationUserId = users["haris123"],     CreatedAtUtc = now },
            new AddressEntity { Street = "Ferhadija 5",                 City = "Sarajevo",   PostalCode = "71000", Region = "Federation of Bosnia and Herzegovina", Country = "Bosnia and Herzegovina", Latitude = 43.8593m, Longitude = 18.4323m, ApplicationUserId = users["danis123"],     CreatedAtUtc = now }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: user addresses.");
    }

    // ─────────────────────────── LOCATIONS ───────────────────────────

    private static async Task SeedLocationsAsync(DatabaseContext context)
    {
        if (await context.Locations.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        // Shop addresses have no ApplicationUserId (they are business addresses, not user-owned)
        var sarajevoAddr = new AddressEntity
        {
            Street = "Džemala Bijedića 185",
            City = "Sarajevo",
            PostalCode = "71000",
            Region = "Federation of Bosnia and Herzegovina",
            Country = "Bosnia and Herzegovina",
            Latitude = 43.8421m,
            Longitude = 18.3879m,
            CreatedAtUtc = now
        };
        var mostarAddr = new AddressEntity
        {
            Street = "Bulevar Branislava Nušića 2",
            City = "Mostar",
            PostalCode = "88000",
            Region = "Federation of Bosnia and Herzegovina",
            Country = "Bosnia and Herzegovina",
            Latitude = 43.3481m,
            Longitude = 17.8093m,
            CreatedAtUtc = now
        };

        context.Addresses.AddRange(sarajevoAddr, mostarAddr);
        await context.SaveChangesAsync();

        var sarajevo = new LocationEntity
        {
            Name = "Detailly Sarajevo",
            Description = "Main detailing studio in Sarajevo. Three bays with full ceramic coating facilities.",
            TotalBays = 3,
            AddressId = sarajevoAddr.Id,
            IsTemporarilyClosed = false,
            CreatedAtUtc = now
        };
        var mostar = new LocationEntity
        {
            Name = "Detailly Mostar",
            Description = "Second branch serving clients in Mostar and the wider Herzegovina region.",
            TotalBays = 2,
            AddressId = mostarAddr.Id,
            IsTemporarilyClosed = false,
            CreatedAtUtc = now
        };

        context.Locations.AddRange(sarajevo, mostar);
        await context.SaveChangesAsync();

        // Sarajevo: Mon–Sat 08:00–18:00, Sun closed  (DayOfWeek: 0=Sun … 6=Sat)
        var sarajevoHours = Enumerable.Range(0, 7).Select(d => new LocationOpeningHoursEntity
        {
            ShopLocationId = sarajevo.Id,
            DayOfWeek = d,
            IsClosed = d == 0,
            OpenTimeUtc  = d == 0 ? null : new TimeSpan(8, 0, 0),
            CloseTimeUtc = d == 0 ? null : new TimeSpan(18, 0, 0),
            CreatedAtUtc = now
        });

        // Mostar: Mon–Fri 08:00–17:00, Sat 09:00–14:00, Sun closed
        var mostarHours = Enumerable.Range(0, 7).Select(d => new LocationOpeningHoursEntity
        {
            ShopLocationId = mostar.Id,
            DayOfWeek = d,
            IsClosed = d == 0,
            OpenTimeUtc  = d == 0 ? null : d == 6 ? new TimeSpan(9, 0, 0)  : new TimeSpan(8, 0, 0),
            CloseTimeUtc = d == 0 ? null : d == 6 ? new TimeSpan(14, 0, 0) : new TimeSpan(17, 0, 0),
            CreatedAtUtc = now
        });

        context.LocationOpeningHours.AddRange(sarajevoHours.Concat(mostarHours));
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: locations + opening hours.");
    }

    // ─────────────────────────── SERVICE PACKAGES ───────────────────────────

    private static async Task SeedServicePackagesAsync(DatabaseContext context)
    {
        if (await context.ServicePackages.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var itemIds = await context.ServicePackageItems
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        // Save packages first so EF generates their IDs, then add assignments separately
        // (ServicePackageItemAssignments has private set; ServicePackageId is required)
        var packages = new[]
        {
            new ServicePackageEntity { Name = "Quick Exterior Wash",                    Price =  40.00m, BaseDurationMinutes =  70, BaseRequiredEmployees = 1, Description = "A fast, safe exterior wash: snow foam pre-soak, two-bucket hand wash, and tire dressing.",                                                                                           CreatedAtUtc = now },
            new ServicePackageEntity { Name = "Exterior Detail",                        Price = 120.00m, BaseDurationMinutes = 185, BaseRequiredEmployees = 1, Description = "Complete exterior detailing with clay bar decontamination, paint sealant, deep wheel cleaning, and tire dressing.",                                                                  CreatedAtUtc = now },
            new ServicePackageEntity { Name = "Interior Detail",                        Price = 105.00m, BaseDurationMinutes = 150, BaseRequiredEmployees = 1, Description = "In-depth interior clean: full cabin vacuuming, plastic and fabric surface treatment, leather clean and conditioning.",                                                               CreatedAtUtc = now },
            new ServicePackageEntity { Name = "Full Detail",                            Price = 240.00m, BaseDurationMinutes = 375, BaseRequiredEmployees = 1, Description = "Our most popular package — complete exterior and interior detailing, iron fallout removal, clay bar, one-step polishing, and wheel care.",                                          CreatedAtUtc = now },
            new ServicePackageEntity { Name = "Paint Correction & Ceramic Coating (1 yr)", Price = 375.00m, BaseDurationMinutes = 515, BaseRequiredEmployees = 1, Description = "Full decontamination, two-step paint correction, and a 1-year ceramic coating for lasting protection and showroom gloss.",                                                   CreatedAtUtc = now },
            new ServicePackageEntity { Name = "Elite Ceramic Coating (3 yr)",           Price = 515.00m, BaseDurationMinutes = 635, BaseRequiredEmployees = 2, Description = "The ultimate protection service: two-step correction followed by our premium 3-year ceramic coating applied by two technicians.",                                                    CreatedAtUtc = now },
        };

        context.ServicePackages.AddRange(packages);
        await context.SaveChangesAsync();

        var pkgIds = await context.ServicePackages
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        int Pkg(string name) => pkgIds[name];
        int Item(string name) => itemIds[name];

        ServicePackageItemAssignmentEntity Link(string pkg, string item) =>
            new ServicePackageItemAssignmentEntity
            {
                ServicePackageId = Pkg(pkg),
                ServicePackageItemId = Item(item),
                CreatedAtUtc = now
            };

        context.ServicePackageItemAssignments.AddRange(
            // Quick Exterior Wash
            Link("Quick Exterior Wash", "Pre-Wash / Snow Foam"),
            Link("Quick Exterior Wash", "Hand Wash (Two-Bucket Method)"),
            Link("Quick Exterior Wash", "Tire Dressing"),

            // Exterior Detail
            Link("Exterior Detail", "Pre-Wash / Snow Foam"),
            Link("Exterior Detail", "Hand Wash (Two-Bucket Method)"),
            Link("Exterior Detail", "Clay Bar Treatment"),
            Link("Exterior Detail", "Paint Sealant Protection"),
            Link("Exterior Detail", "Wheel Deep Cleaning and Protection"),
            Link("Exterior Detail", "Tire Dressing"),

            // Interior Detail
            Link("Interior Detail", "Interior Deep Cleaning"),
            Link("Interior Detail", "Leather Cleaning and Conditioning"),

            // Full Detail
            Link("Full Detail", "Pre-Wash / Snow Foam"),
            Link("Full Detail", "Hand Wash (Two-Bucket Method)"),
            Link("Full Detail", "Iron Fallout Decontamination"),
            Link("Full Detail", "Clay Bar Treatment"),
            Link("Full Detail", "One-Step Machine Polishing"),
            Link("Full Detail", "Interior Deep Cleaning"),
            Link("Full Detail", "Wheel Deep Cleaning and Protection"),

            // Paint Correction & Ceramic Coating (1 yr)
            Link("Paint Correction & Ceramic Coating (1 yr)", "Pre-Wash / Snow Foam"),
            Link("Paint Correction & Ceramic Coating (1 yr)", "Hand Wash (Two-Bucket Method)"),
            Link("Paint Correction & Ceramic Coating (1 yr)", "Clay Bar Treatment"),
            Link("Paint Correction & Ceramic Coating (1 yr)", "Two-Step Paint Correction"),
            Link("Paint Correction & Ceramic Coating (1 yr)", "Ceramic Coating - 1 Year"),

            // Elite Ceramic Coating (3 yr)
            Link("Elite Ceramic Coating (3 yr)", "Pre-Wash / Snow Foam"),
            Link("Elite Ceramic Coating (3 yr)", "Hand Wash (Two-Bucket Method)"),
            Link("Elite Ceramic Coating (3 yr)", "Clay Bar Treatment"),
            Link("Elite Ceramic Coating (3 yr)", "Two-Step Paint Correction"),
            Link("Elite Ceramic Coating (3 yr)", "Ceramic Coating - 3 Years")
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: service packages.");
    }

    // ─────────────────────────── VEHICLES ───────────────────────────

    private static async Task SeedVehiclesAsync(DatabaseContext context)
    {
        if (await context.Vehicles.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        var cats = await context.VehicleCategories
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        int Cat(string name) => cats[name];

        context.Vehicles.AddRange(
            // ── client ──────────────────────────────────────────────────
            new VehicleEntity { Brand = "Volkswagen",    Model = "Golf VII",       YearOfManufacture = 2018, LicencePlate = "A12-345", ApplicationUserId = users["client"],       VehicleCategoryId = Cat("Hatchback"), CreatedAtUtc = now },

            // ── lkovacevic ──────────────────────────────────────────────
            new VehicleEntity { Brand = "Toyota",        Model = "Corolla",        YearOfManufacture = 2021, LicencePlate = "E11-222", ApplicationUserId = users["lkovacevic"],   VehicleCategoryId = Cat("Sedan"),     CreatedAtUtc = now },

            // ── mbegovic ────────────────────────────────────────────────
            new VehicleEntity { Brand = "BMW",           Model = "X3 xDrive20d",   YearOfManufacture = 2020, LicencePlate = "K34-567", ApplicationUserId = users["mbegovic"],     VehicleCategoryId = Cat("SUV"),       CreatedAtUtc = now },

            // ── haris123 ────────────────────────────────────────────────
            new VehicleEntity { Brand = "Audi",          Model = "A4 2.0 TDI",     YearOfManufacture = 2019, LicencePlate = "T55-123", ApplicationUserId = users["haris123"],     VehicleCategoryId = Cat("Sedan"),     CreatedAtUtc = now },

            // ── danis123 ────────────────────────────────────────────────
            new VehicleEntity { Brand = "Skoda",         Model = "Octavia III",    YearOfManufacture = 2020, LicencePlate = "T66-456", ApplicationUserId = users["danis123"],     VehicleCategoryId = Cat("Sedan"),     CreatedAtUtc = now },

            // ── fleetclient (Bosna Logistics) — three vans ──────────────
            new VehicleEntity { Brand = "Mercedes-Benz", Model = "Sprinter 314",   YearOfManufacture = 2019, LicencePlate = "B01-001", ApplicationUserId = users["fleetclient"],  VehicleCategoryId = Cat("Van"),       CreatedAtUtc = now },
            new VehicleEntity { Brand = "Mercedes-Benz", Model = "Sprinter 314",   YearOfManufacture = 2020, LicencePlate = "B01-002", ApplicationUserId = users["fleetclient"],  VehicleCategoryId = Cat("Van"),       CreatedAtUtc = now },
            new VehicleEntity { Brand = "Volkswagen",    Model = "Transporter T6",  YearOfManufacture = 2021, LicencePlate = "B01-003", ApplicationUserId = users["fleetclient"],  VehicleCategoryId = Cat("Van"),       CreatedAtUtc = now },

            // ── najanovic (Sarajevo Transport) — two vans ───────────────
            new VehicleEntity { Brand = "Ford",          Model = "Transit Custom",  YearOfManufacture = 2022, LicencePlate = "C10-100", ApplicationUserId = users["najanovic"],    VehicleCategoryId = Cat("Van"),       CreatedAtUtc = now },
            new VehicleEntity { Brand = "Ford",          Model = "Transit Custom",  YearOfManufacture = 2022, LicencePlate = "C10-101", ApplicationUserId = users["najanovic"],    VehicleCategoryId = Cat("Van"),       CreatedAtUtc = now }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: vehicles.");
    }

    // ─────────────────────────── EMPLOYEE SHIFTS ───────────────────────────

    private static async Task SeedEmployeeShiftsAsync(DatabaseContext context)
    {
        if (await context.EmployeeShifts.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        var locations = await context.Locations
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        int sarajevo = locations["Detailly Sarajevo"];
        int mostar   = locations["Detailly Mostar"];

        EmployeeShiftEntity Shift(string username, int locationId, EmployeeWorkMode mode, DateTime start) =>
            new EmployeeShiftEntity
            {
                EmployeeId     = users[username],
                ShopLocationId = locationId,
                EmployeeWorkMode = mode,
                StartUtc       = start,
                EndUtc         = start.AddHours(10),
                CreatedAtUtc   = now
            };

        // Past week  Mon-Fri 2026-05-04 → 2026-05-08  (covers completed bookings on 05, 06, 07)
        var pastDays = new[] { 4, 5, 6, 7, 8 }
            .Select(d => new DateTime(2026, 5, d, 6, 0, 0, DateTimeKind.Utc)).ToArray();

        // Future week  Mon-Fri 2026-05-11 → 2026-05-15  (covers confirmed bookings on 12, 14)
        var futureDays = new[] { 11, 12, 13, 14, 15 }
            .Select(d => new DateTime(2026, 5, d, 6, 0, 0, DateTimeKind.Utc)).ToArray();

        var shifts = new List<EmployeeShiftEntity>();

        // ahodzic + tkevric — Sarajevo InShop, both weeks (they work the completed bookings)
        foreach (var day in pastDays.Concat(futureDays))
        {
            shifts.Add(Shift("ahodzic", sarajevo, EmployeeWorkMode.InShop, day));
            shifts.Add(Shift("tkevric", sarajevo, EmployeeWorkMode.InShop, day));
        }

        // employee, emuric, jtahirovic — Sarajevo InShop, future week (cover confirmed bookings)
        foreach (var day in futureDays)
        {
            shifts.Add(Shift("employee",   sarajevo, EmployeeWorkMode.InShop, day));
            shifts.Add(Shift("emuric",     sarajevo, EmployeeWorkMode.InShop, day));
            shifts.Add(Shift("jtahirovic", sarajevo, EmployeeWorkMode.InShop, day));
            shifts.Add(Shift("lhadzic",    sarajevo, EmployeeWorkMode.InShop, day));
        }

        // abasic + nkukic — Mostar InShop, both weeks
        foreach (var day in pastDays.Concat(futureDays))
        {
            shifts.Add(Shift("abasic", mostar, EmployeeWorkMode.InShop, day));
            shifts.Add(Shift("nkukic", mostar, EmployeeWorkMode.InShop, day));
        }

        // mpetrovic + skadic — Sarajevo Mobile, both weeks
        foreach (var day in pastDays.Concat(futureDays))
        {
            shifts.Add(Shift("mpetrovic", sarajevo, EmployeeWorkMode.Mobile, day));
            shifts.Add(Shift("skadic",    sarajevo, EmployeeWorkMode.Mobile, day));
        }

        context.EmployeeShifts.AddRange(shifts);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: employee shifts.");
    }

    // ─────────────────────────── BOOKINGS + PAYMENTS ───────────────────────────

    private static async Task SeedBookingsAsync(DatabaseContext context)
    {
        if (await context.Bookings.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        var vehicles = await context.Vehicles
            .AsNoTracking()
            .ToDictionaryAsync(x => x.LicencePlate, x => x.Id);

        var packages = await context.ServicePackages
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        var packageItems = await context.ServicePackageItems
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x);

        int sarajevo = (await context.Locations
            .AsNoTracking()
            .FirstAsync(l => l.Name == "Detailly Sarajevo")).Id;

        // ── 1. Booking entities ──────────────────────────────────────────────
        // b1: client  Quick Exterior Wash + Wheel Deep addon    70 BAM  completed
        var b1 = new BookingEntity { CustomerId = users["client"],     ShopLocationId = sarajevo, ServicePackageId = packages["Quick Exterior Wash"], TotalPrice = 70.00m,  StartUtc = new DateTime(2026, 5, 5,  7,  0, 0, DateTimeKind.Utc), EndUtc = new DateTime(2026, 5, 5,  8, 55, 0, DateTimeKind.Utc),  RequiredEmployees = 1, RequiredBays = 1, Status = BookingStatus.Completed, ServiceMode = ServiceMode.InShop, CreatedAtUtc = now };
        // b2: lkovacevic  Interior Detail                      105 BAM  completed
        var b2 = new BookingEntity { CustomerId = users["lkovacevic"], ShopLocationId = sarajevo, ServicePackageId = packages["Interior Detail"],     TotalPrice = 105.00m, StartUtc = new DateTime(2026, 5, 5,  9, 30, 0, DateTimeKind.Utc), EndUtc = new DateTime(2026, 5, 5,  12,  0, 0, DateTimeKind.Utc), RequiredEmployees = 1, RequiredBays = 1, Status = BookingStatus.Completed, ServiceMode = ServiceMode.InShop, CreatedAtUtc = now };
        // b3: mbegovic  Quick Exterior Wash                     40 BAM  completed
        var b3 = new BookingEntity { CustomerId = users["mbegovic"],   ShopLocationId = sarajevo, ServicePackageId = packages["Quick Exterior Wash"], TotalPrice = 40.00m,  StartUtc = new DateTime(2026, 5, 6,  7,  0, 0, DateTimeKind.Utc), EndUtc = new DateTime(2026, 5, 6,  8, 10, 0, DateTimeKind.Utc),  RequiredEmployees = 1, RequiredBays = 1, Status = BookingStatus.Completed, ServiceMode = ServiceMode.InShop, CreatedAtUtc = now };
        // b4: haris123  Full Detail + Carnauba addon           270 BAM  completed
        var b4 = new BookingEntity { CustomerId = users["haris123"],   ShopLocationId = sarajevo, ServicePackageId = packages["Full Detail"],         TotalPrice = 270.00m, StartUtc = new DateTime(2026, 5, 7,  7,  0, 0, DateTimeKind.Utc), EndUtc = new DateTime(2026, 5, 7, 13, 45, 0, DateTimeKind.Utc),  RequiredEmployees = 1, RequiredBays = 1, Status = BookingStatus.Completed, ServiceMode = ServiceMode.InShop, CreatedAtUtc = now };
        // b5: danis123  Exterior Detail                        120 BAM  confirmed (future)
        var b5 = new BookingEntity { CustomerId = users["danis123"],   ShopLocationId = sarajevo, ServicePackageId = packages["Exterior Detail"],     TotalPrice = 120.00m, StartUtc = new DateTime(2026, 5, 12, 7,  0, 0, DateTimeKind.Utc), EndUtc = new DateTime(2026, 5, 12, 10,  5, 0, DateTimeKind.Utc), RequiredEmployees = 1, RequiredBays = 1, Status = BookingStatus.Confirmed, ServiceMode = ServiceMode.InShop, CreatedAtUtc = now };
        // b6: client    Interior Detail                        105 BAM  confirmed (future)
        var b6 = new BookingEntity { CustomerId = users["client"],     ShopLocationId = sarajevo, ServicePackageId = packages["Interior Detail"],     TotalPrice = 105.00m, StartUtc = new DateTime(2026, 5, 14, 7,  0, 0, DateTimeKind.Utc), EndUtc = new DateTime(2026, 5, 14,  9, 30, 0, DateTimeKind.Utc), RequiredEmployees = 1, RequiredBays = 1, Status = BookingStatus.Confirmed, ServiceMode = ServiceMode.InShop, CreatedAtUtc = now };

        context.Bookings.AddRange(b1, b2, b3, b4, b5, b6);
        await context.SaveChangesAsync();

        // ── 2. Vehicle + employee assignments, add-on items ──────────────────
        context.BookingVehicleAssignments.AddRange(
            new BookingVehicleAssignmentEntity { BookingId = b1.Id, VehicleId = vehicles["A12-345"], CreatedAtUtc = now },
            new BookingVehicleAssignmentEntity { BookingId = b2.Id, VehicleId = vehicles["E11-222"], CreatedAtUtc = now },
            new BookingVehicleAssignmentEntity { BookingId = b3.Id, VehicleId = vehicles["K34-567"], CreatedAtUtc = now },
            new BookingVehicleAssignmentEntity { BookingId = b4.Id, VehicleId = vehicles["T55-123"], CreatedAtUtc = now },
            new BookingVehicleAssignmentEntity { BookingId = b5.Id, VehicleId = vehicles["T66-456"], CreatedAtUtc = now },
            new BookingVehicleAssignmentEntity { BookingId = b6.Id, VehicleId = vehicles["A12-345"], CreatedAtUtc = now }
        );

        context.BookingEmployeeAssignments.AddRange(
            new BookingEmployeeAssignmentEntity { BookingId = b1.Id, EmployeeId = users["ahodzic"],  AssignedAtUtc = now, CreatedAtUtc = now },
            new BookingEmployeeAssignmentEntity { BookingId = b2.Id, EmployeeId = users["tkevric"],  AssignedAtUtc = now, CreatedAtUtc = now },
            new BookingEmployeeAssignmentEntity { BookingId = b3.Id, EmployeeId = users["ahodzic"],  AssignedAtUtc = now, CreatedAtUtc = now },
            new BookingEmployeeAssignmentEntity { BookingId = b4.Id, EmployeeId = users["tkevric"],  AssignedAtUtc = now, CreatedAtUtc = now },
            new BookingEmployeeAssignmentEntity { BookingId = b5.Id, EmployeeId = users["employee"], AssignedAtUtc = now, CreatedAtUtc = now },
            new BookingEmployeeAssignmentEntity { BookingId = b6.Id, EmployeeId = users["employee"], AssignedAtUtc = now, CreatedAtUtc = now }
        );

        var wheelDeep = packageItems["Wheel Deep Cleaning and Protection"];
        var carnauba  = packageItems["Carnauba Wax Finish"];

        context.BookingItems.AddRange(
            new BookingItemEntity { BookingId = b1.Id, ServicePackageItemId = wheelDeep.Id, PriceSnapshot = wheelDeep.Price, DurationMinutesSnapshot = wheelDeep.DurationMinutes, RequiredEmployeesSnapshot = wheelDeep.RequiredEmployees, IsAddon = true, CreatedAtUtc = now },
            new BookingItemEntity { BookingId = b4.Id, ServicePackageItemId = carnauba.Id,  PriceSnapshot = carnauba.Price,  DurationMinutesSnapshot = carnauba.DurationMinutes,  RequiredEmployeesSnapshot = carnauba.RequiredEmployees,  IsAddon = true, CreatedAtUtc = now }
        );

        await context.SaveChangesAsync();

        // ── 3. Payment transactions ──────────────────────────────────────────
        var walletsByUserId = await context.Wallet
            .AsNoTracking()
            .ToDictionaryAsync(w => w.ApplicationUserId, w => w.Id);

        context.PaymentTransactions.AddRange(
            new PaymentTransactionEntity { Amount = b1.TotalPrice, TransactionType = TransactionType.Payment, Status = PaymentTransactionStatus.Paid, TransactionDate = new DateTime(2026, 5, 5,  6, 50, 0, DateTimeKind.Utc), Provider = "Wallet", Description = "Booking payment via wallet.", WalletId = walletsByUserId[users["client"]],     BookingId = b1.Id, CreatedAtUtc = now },
            new PaymentTransactionEntity { Amount = b2.TotalPrice, TransactionType = TransactionType.Payment, Status = PaymentTransactionStatus.Paid, TransactionDate = new DateTime(2026, 5, 5,  9, 20, 0, DateTimeKind.Utc), Provider = "Wallet", Description = "Booking payment via wallet.", WalletId = walletsByUserId[users["lkovacevic"]], BookingId = b2.Id, CreatedAtUtc = now },
            new PaymentTransactionEntity { Amount = b3.TotalPrice, TransactionType = TransactionType.Payment, Status = PaymentTransactionStatus.Paid, TransactionDate = new DateTime(2026, 5, 6,  6, 50, 0, DateTimeKind.Utc), Provider = "Wallet", Description = "Booking payment via wallet.", WalletId = walletsByUserId[users["mbegovic"]],   BookingId = b3.Id, CreatedAtUtc = now },
            new PaymentTransactionEntity { Amount = b4.TotalPrice, TransactionType = TransactionType.Payment, Status = PaymentTransactionStatus.Paid, TransactionDate = new DateTime(2026, 5, 7,  6, 50, 0, DateTimeKind.Utc), Provider = "Wallet", Description = "Booking payment via wallet.", WalletId = walletsByUserId[users["haris123"]],   BookingId = b4.Id, CreatedAtUtc = now },
            new PaymentTransactionEntity { Amount = b5.TotalPrice, TransactionType = TransactionType.Payment, Status = PaymentTransactionStatus.Paid, TransactionDate = new DateTime(2026, 5, 9, 14,  0, 0, DateTimeKind.Utc), Provider = "Wallet", Description = "Booking payment via wallet.", WalletId = walletsByUserId[users["danis123"]],   BookingId = b5.Id, CreatedAtUtc = now },
            new PaymentTransactionEntity { Amount = b6.TotalPrice, TransactionType = TransactionType.Payment, Status = PaymentTransactionStatus.Paid, TransactionDate = new DateTime(2026, 5, 10, 10,  0, 0, DateTimeKind.Utc), Provider = "Wallet", Description = "Booking payment via wallet.", WalletId = walletsByUserId[users["client"]],     BookingId = b6.Id, CreatedAtUtc = now }
        );
        await context.SaveChangesAsync();

        // ── 4. Deduct wallet balances ────────────────────────────────────────
        // client: b1 (70) + b6 (105) = 175 deducted from 320 → 145 remaining
        var deductions = new Dictionary<int, decimal>
        {
            { users["client"],     175.00m },
            { users["lkovacevic"], 105.00m },
            { users["mbegovic"],    40.00m },
            { users["haris123"],   270.00m },
            { users["danis123"],   120.00m }
        };

        var wallets = await context.Wallet
            .Where(w => deductions.Keys.Contains(w.ApplicationUserId))
            .ToListAsync();

        foreach (var w in wallets)
            w.Balance -= deductions[w.ApplicationUserId];

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: bookings + payments.");
    }

    // ─────────────────────────── REVIEWS ───────────────────────────

    private static async Task SeedReviewsAsync(DatabaseContext context)
    {
        if (await context.Reviews.AnyAsync())
            return;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        var packages = await context.ServicePackages
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        var completed = await context.Bookings
            .AsNoTracking()
            .Where(b => b.Status == BookingStatus.Completed)
            .ToListAsync();

        BookingEntity? Find(int customerId, int packageId) =>
            completed.FirstOrDefault(b => b.CustomerId == customerId && b.ServicePackageId == packageId);

        var b1 = Find(users["client"],     packages["Quick Exterior Wash"]);
        var b2 = Find(users["lkovacevic"], packages["Interior Detail"]);
        var b3 = Find(users["mbegovic"],   packages["Quick Exterior Wash"]);
        var b4 = Find(users["haris123"],   packages["Full Detail"]);

        if (b1 is null || b2 is null || b3 is null || b4 is null)
            return;

        // Reviews must be created within 7 days of booking.EndUtc (all are within window)
        context.Reviews.AddRange(
            new ReviewEntity { BookingId = b1.Id, ServicePackageId = packages["Quick Exterior Wash"], CustomerId = users["client"],     Rating = 5, Description = "Excellent service — car looked spotless after. Quick and professional.",               IsDeleted = false, CreatedAtUtc = new DateTime(2026, 5, 6,  9,  0, 0, DateTimeKind.Utc) },
            new ReviewEntity { BookingId = b2.Id, ServicePackageId = packages["Interior Detail"],     CustomerId = users["lkovacevic"], Rating = 4, Description = "Very thorough interior clean. The leather feels great. Will definitely return.",       IsDeleted = false, CreatedAtUtc = new DateTime(2026, 5, 7, 10,  0, 0, DateTimeKind.Utc) },
            new ReviewEntity { BookingId = b3.Id, ServicePackageId = packages["Quick Exterior Wash"], CustomerId = users["mbegovic"],   Rating = 4, Description = "Quick and well done. Good value for money.",                                           IsDeleted = false, CreatedAtUtc = new DateTime(2026, 5, 7,  9, 30, 0, DateTimeKind.Utc) },
            new ReviewEntity { BookingId = b4.Id, ServicePackageId = packages["Full Detail"],         CustomerId = users["haris123"],   Rating = 5, Description = "Outstanding result. The paint correction made a night-and-day difference. Coming back for ceramic.", IsDeleted = false, CreatedAtUtc = new DateTime(2026, 5, 8,  8,  0, 0, DateTimeKind.Utc) }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: reviews.");
    }

    // ─────────────────────────── REACTIONS ───────────────────────────

    private static async Task SeedReactionsAsync(DatabaseContext context)
    {
        if (await context.Reactions.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await context.ApplicationUsers
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Username, x => x.Id);

        var packages = await context.ServicePackages
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        // Unique constraint: (CustomerId, ServicePackageId) — one reaction per user per package
        context.Reactions.AddRange(
            new ReactionEntity { CustomerId = users["danis123"],   ServicePackageId = packages["Quick Exterior Wash"],                       ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["mbegovic"],   ServicePackageId = packages["Quick Exterior Wash"],                       ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["client"],     ServicePackageId = packages["Exterior Detail"],                           ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["client"],     ServicePackageId = packages["Full Detail"],                               ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["lkovacevic"], ServicePackageId = packages["Full Detail"],                               ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["lkovacevic"], ServicePackageId = packages["Interior Detail"],                           ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["haris123"],   ServicePackageId = packages["Elite Ceramic Coating (3 yr)"],              ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["najanovic"],  ServicePackageId = packages["Paint Correction & Ceramic Coating (1 yr)"], ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["danis123"],   ServicePackageId = packages["Interior Detail"],                           ReactionType = ReactionType.Like,    CreatedAtUtc = now },
            new ReactionEntity { CustomerId = users["mbegovic"],   ServicePackageId = packages["Full Detail"],                               ReactionType = ReactionType.Dislike, CreatedAtUtc = now }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seed: reactions.");
    }
}
