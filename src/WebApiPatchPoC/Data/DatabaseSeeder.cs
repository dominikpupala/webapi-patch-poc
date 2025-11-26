using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApiPatchPoC.Data;

// Development-only database initialization and seeding utility.
// In production use migration tools (DbUp, EF Migrations, etc.) instead.
internal static class DatabaseSeeder
{
    public static async Task Initialize(string connectionString, ILogger logger)
    {
        try
        {
            await EnsureDatabaseExists(connectionString, logger);
            await EnsureSchemaExists(connectionString, logger);
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Failed to initialize database. Please check your connection string and ensure the database server is running");
            throw;
        }
    }

    private static async Task EnsureDatabaseExists(string connectionString, ILogger logger)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        if (string.IsNullOrEmpty(databaseName))
        {
            throw new InvalidOperationException("Database name not specified in connection string.");
        }

        builder.InitialCatalog = "master";
        await using var connection = new SqlConnection(builder.ToString());

        var databaseExists = await CheckDatabase(connection, databaseName);
        if (databaseExists)
        {
            logger.LogInformation("Database '{DatabaseName}' already exists", databaseName);
            return;
        }

        logger.LogInformation("Database '{DatabaseName}' not found. Creating database", databaseName);
        await CreateDatabase(connection, databaseName);
        logger.LogInformation("Database '{DatabaseName}' created successfully", databaseName);
    }

    private static async Task<bool> CheckDatabase(SqlConnection connection, string databaseName)
    {
        const string sql = """
            SELECT TOP 1 1
            FROM sys.databases
            WHERE name = @DatabaseName;
            """;

        var result = await connection.QuerySingleOrDefaultAsync<int?>(new CommandDefinition(sql, new { DatabaseName = databaseName }));
        return result.HasValue;
    }

    private static async Task CreateDatabase(SqlConnection connection, string databaseName)
    {
        const string sql = """
            DECLARE @QuotedName NVARCHAR(128) = QUOTENAME(@DatabaseName); 
            EXEC('CREATE DATABASE ' + @QuotedName);
            """;

        var cmd = new CommandDefinition(sql, new { DatabaseName = databaseName });
        await connection.ExecuteAsync(cmd);
    }

    private static async Task EnsureSchemaExists(string connectionString, ILogger logger)
    {
        await using var connection = new SqlConnection(connectionString);

        logger.LogInformation("Checking database schema");

        var tableExists = await CheckTableExists(connection);
        if (tableExists)
        {
            logger.LogInformation("Products table already exists. Skipping initialization");
            return;
        }

        logger.LogInformation("Products table not found. Creating schema");
        await CreateSchema(connection);
        logger.LogInformation("Schema created successfully");

        logger.LogInformation("Seeding initial data");
        await SeedData(connection);
        logger.LogInformation("Data seeded successfully");
    }

    private static async Task<bool> CheckTableExists(IDbConnection connection)
    {
        const string sql = """
            SELECT TOP 1 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @Schema
            AND TABLE_NAME = @TableName
            """;

        var result = await connection.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { Schema = "dbo", TableName = "Products" }));
        return result.HasValue;
    }

    private static async Task CreateSchema(IDbConnection connection)
    {
        const string createTableSql = """
            CREATE TABLE dbo.Products (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Sku VARCHAR(50) NOT NULL UNIQUE,
                Name NVARCHAR(200) NOT NULL,
                ImgUri VARCHAR(500) NOT NULL,
                Price DECIMAL(18,2) NOT NULL,
                Description NVARCHAR(MAX) NULL
            );
            """;

        await connection.ExecuteAsync(createTableSql);
    }

    private static async Task SeedData(IDbConnection connection)
    {
        var products = new[]
        {
            // Peripherals
            new { Sku = "PC-MOUSE-001", Name = "Wireless Mouse", ImgUri = "https://images.example.com/mouse.jpg", Price = 29.99m, Description = "Ergonomic wireless mouse with USB receiver" },
            new { Sku = "PC-MOUSE-002", Name = "Gaming Mouse RGB", ImgUri = "https://images.example.com/gaming-mouse.jpg", Price = 59.99m, Description = "High-precision gaming mouse with customizable RGB lighting" },
            new { Sku = "PC-MOUSE-003", Name = "Trackball Mouse", ImgUri = "https://images.example.com/trackball.jpg", Price = 49.99m, Description = "Ergonomic trackball mouse for precision work" },
            new { Sku = "PC-KB-001", Name = "Mechanical Keyboard", ImgUri = "https://images.example.com/keyboard.jpg", Price = 89.99m, Description = "RGB mechanical keyboard with Cherry MX switches" },
            new { Sku = "PC-KB-002", Name = "Wireless Keyboard", ImgUri = "https://images.example.com/wireless-kb.jpg", Price = 45.99m, Description = "Compact wireless keyboard with long battery life" },
            new { Sku = "PC-KB-003", Name = "Ergonomic Keyboard", ImgUri = "https://images.example.com/ergo-kb.jpg", Price = 79.99m, Description = "Split ergonomic keyboard for comfortable typing" },
            new { Sku = "PC-CAM-001", Name = "Webcam HD", ImgUri = "https://images.example.com/webcam.jpg", Price = 79.99m, Description = "1080p HD webcam with built-in microphone" },
            new { Sku = "PC-CAM-002", Name = "Webcam 4K", ImgUri = "https://images.example.com/webcam-4k.jpg", Price = 149.99m, Description = "4K Ultra HD webcam with autofocus" },
            new { Sku = "PC-PAD-001", Name = "Mouse Pad XL", ImgUri = "https://images.example.com/mousepad.jpg", Price = 19.99m, Description = "Extra large gaming mouse pad" },
            new { Sku = "PC-PAD-002", Name = "RGB Mouse Pad", ImgUri = "https://images.example.com/rgb-pad.jpg", Price = 34.99m, Description = "RGB illuminated gaming mouse pad with USB hub" },
            new { Sku = "PC-TAB-001", Name = "Graphics Tablet", ImgUri = "https://images.example.com/tablet.jpg", Price = 149.99m, Description = "Digital drawing tablet with pressure sensitivity" },
            new { Sku = "PC-TAB-002", Name = "Graphics Tablet Pro", ImgUri = "https://images.example.com/tablet-pro.jpg", Price = 399.99m, Description = "Professional graphics tablet with 8192 pressure levels" },

            // Audio
            new { Sku = "AUD-HS-001", Name = "Headset", ImgUri = "https://images.example.com/headset.jpg", Price = 59.99m, Description = "Noise-cancelling wireless headset" },
            new { Sku = "AUD-HS-002", Name = "Gaming Headset", ImgUri = "https://images.example.com/gaming-headset.jpg", Price = 89.99m, Description = "7.1 surround sound gaming headset with RGB" },
            new { Sku = "AUD-HS-003", Name = "Studio Headphones", ImgUri = "https://images.example.com/studio-hp.jpg", Price = 199.99m, Description = "Professional studio monitoring headphones" },
            new { Sku = "AUD-MIC-001", Name = "Microphone", ImgUri = "https://images.example.com/mic.jpg", Price = 119.99m, Description = "USB condenser microphone for streaming" },
            new { Sku = "AUD-MIC-002", Name = "Microphone XLR", ImgUri = "https://images.example.com/mic-xlr.jpg", Price = 249.99m, Description = "Professional XLR condenser microphone" },
            new { Sku = "AUD-SPK-001", Name = "Bluetooth Speaker", ImgUri = "https://images.example.com/speaker.jpg", Price = 69.99m, Description = "Portable Bluetooth speaker with 360° sound" },
            new { Sku = "AUD-SPK-002", Name = "Desktop Speakers", ImgUri = "https://images.example.com/desktop-spk.jpg", Price = 129.99m, Description = "Premium 2.1 desktop speaker system" },
            new { Sku = "AUD-INT-001", Name = "Audio Interface", ImgUri = "https://images.example.com/interface.jpg", Price = 179.99m, Description = "USB audio interface with 2 inputs" },

            // Displays
            new { Sku = "DISP-MON-001", Name = "Monitor 27\"", ImgUri = "https://images.example.com/monitor.jpg", Price = 299.99m, Description = "27-inch 4K IPS monitor with USB-C" },
            new { Sku = "DISP-MON-002", Name = "Monitor 32\" Curved", ImgUri = "https://images.example.com/monitor-curved.jpg", Price = 449.99m, Description = "32-inch curved gaming monitor 165Hz" },
            new { Sku = "DISP-MON-003", Name = "Monitor 24\" FHD", ImgUri = "https://images.example.com/monitor-24.jpg", Price = 179.99m, Description = "24-inch Full HD monitor for productivity" },
            new { Sku = "DISP-MON-004", Name = "Portable Monitor", ImgUri = "https://images.example.com/portable-mon.jpg", Price = 199.99m, Description = "15.6-inch portable USB-C monitor" },

            // Storage
            new { Sku = "STOR-SSD-001", Name = "External SSD 1TB", ImgUri = "https://images.example.com/ssd.jpg", Price = 129.99m, Description = "Portable 1TB SSD with USB 3.2" },
            new { Sku = "STOR-SSD-002", Name = "External SSD 2TB", ImgUri = "https://images.example.com/ssd-2tb.jpg", Price = 229.99m, Description = "Portable 2TB SSD with USB-C 3.2 Gen 2" },
            new { Sku = "STOR-HDD-001", Name = "External HDD 4TB", ImgUri = "https://images.example.com/hdd.jpg", Price = 99.99m, Description = "4TB external hard drive for backup" },
            new { Sku = "STOR-NVME-001", Name = "NVMe SSD 500GB", ImgUri = "https://images.example.com/nvme.jpg", Price = 79.99m, Description = "Internal NVMe M.2 SSD 500GB" },
            new { Sku = "STOR-CASE-001", Name = "HDD Enclosure", ImgUri = "https://images.example.com/enclosure.jpg", Price = 24.99m, Description = "USB 3.0 external drive enclosure" },

            // Cables & Adapters
            new { Sku = "CBL-HDMI-001", Name = "HDMI Cable 2m", ImgUri = "https://images.example.com/hdmi.jpg", Price = 12.99m, Description = "High-speed HDMI 2.1 cable" },
            new { Sku = "CBL-USBC-001", Name = "USB-C Cable 3m", ImgUri = "https://images.example.com/usbc-cable.jpg", Price = 15.99m, Description = "USB-C to USB-C cable 100W PD" },
            new { Sku = "CBL-DP-001", Name = "DisplayPort Cable", ImgUri = "https://images.example.com/dp-cable.jpg", Price = 18.99m, Description = "DisplayPort 1.4 cable 8K support" },
            new { Sku = "CBL-ETH-001", Name = "Ethernet Cable 5m", ImgUri = "https://images.example.com/ethernet.jpg", Price = 9.99m, Description = "Cat 6 Ethernet cable" },

            // Accessories
            new { Sku = "ACC-HUB-001", Name = "USB-C Hub", ImgUri = "https://images.example.com/hub.jpg", Price = 49.99m, Description = "Multi-port USB-C hub with HDMI and SD card reader" },
            new { Sku = "ACC-HUB-002", Name = "USB Hub 7-Port", ImgUri = "https://images.example.com/usb-hub.jpg", Price = 29.99m, Description = "7-port USB 3.0 hub with power adapter" },
            new { Sku = "ACC-STAND-001", Name = "Laptop Stand", ImgUri = "https://images.example.com/stand.jpg", Price = 39.99m, Description = "Aluminum laptop stand with adjustable height" },
            new { Sku = "ACC-STAND-002", Name = "Monitor Arm", ImgUri = "https://images.example.com/monitor-arm.jpg", Price = 89.99m, Description = "Adjustable monitor arm for ergonomic positioning" },
            new { Sku = "ACC-LAMP-001", Name = "Desk Lamp", ImgUri = "https://images.example.com/lamp.jpg", Price = 34.99m, Description = "LED desk lamp with touch control and wireless charging" },
            new { Sku = "ACC-ORG-001", Name = "Cable Organizer", ImgUri = "https://images.example.com/organizer.jpg", Price = 14.99m, Description = "Cable management box for desk organization" },
            new { Sku = "ACC-HOLD-001", Name = "Phone Holder", ImgUri = "https://images.example.com/holder.jpg", Price = 16.99m, Description = "Adjustable phone holder for desk" },
            new { Sku = "ACC-CLN-001", Name = "Screen Cleaner Kit", ImgUri = "https://images.example.com/cleaner.jpg", Price = 9.99m, Description = "Screen cleaning solution with microfiber cloth" },
            new { Sku = "ACC-CUSH-001", Name = "Ergonomic Chair Cushion", ImgUri = "https://images.example.com/cushion.jpg", Price = 34.99m, Description = "Memory foam seat cushion for office chairs" },
            new { Sku = "ACC-REST-001", Name = "Wrist Rest", ImgUri = "https://images.example.com/wrist-rest.jpg", Price = 19.99m, Description = "Ergonomic keyboard wrist rest with memory foam" },
            new { Sku = "ACC-DESK-001", Name = "Desk Mat", ImgUri = "https://images.example.com/desk-mat.jpg", Price = 29.99m, Description = "Large leather desk mat protector" },
            new { Sku = "ACC-FAN-001", Name = "Laptop Cooling Pad", ImgUri = "https://images.example.com/cooling-pad.jpg", Price = 39.99m, Description = "Laptop cooling pad with adjustable fans" },

            // Charging
            new { Sku = "CHG-WLESS-001", Name = "Wireless Charger", ImgUri = "https://images.example.com/charger.jpg", Price = 24.99m, Description = "Fast wireless charging pad" },
            new { Sku = "CHG-PWR-001", Name = "Power Bank 20000mAh", ImgUri = "https://images.example.com/powerbank.jpg", Price = 44.99m, Description = "High-capacity power bank with fast charging" },
            new { Sku = "CHG-PWR-002", Name = "Power Bank 30000mAh", ImgUri = "https://images.example.com/powerbank-30k.jpg", Price = 69.99m, Description = "Ultra-high capacity power bank with laptop charging" },
            new { Sku = "CHG-MULT-001", Name = "Multi-Device Charger", ImgUri = "https://images.example.com/multi-charger.jpg", Price = 54.99m, Description = "6-port USB charging station" },
            new { Sku = "CHG-GAN-001", Name = "GaN Charger 65W", ImgUri = "https://images.example.com/gan-charger.jpg", Price = 49.99m, Description = "Compact GaN USB-C charger 65W" }
        };

        const string insertSql = """
            INSERT INTO dbo.Products (Sku, Name, ImgUri, Price, Description)
            VALUES (@Sku, @Name, @ImgUri, @Price, @Description);
            """;

        await connection.ExecuteAsync(insertSql, products);
    }
}
