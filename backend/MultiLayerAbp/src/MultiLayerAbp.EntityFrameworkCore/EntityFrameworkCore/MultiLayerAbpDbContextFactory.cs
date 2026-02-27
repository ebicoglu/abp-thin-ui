using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MultiLayerAbp.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MultiLayerAbpDbContextFactory : IDesignTimeDbContextFactory<MultiLayerAbpDbContext>
{
    public MultiLayerAbpDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        MultiLayerAbpEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<MultiLayerAbpDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new MultiLayerAbpDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MultiLayerAbp.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
