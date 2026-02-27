using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MultiLayerAbp.Data;
using Volo.Abp.DependencyInjection;

namespace MultiLayerAbp.EntityFrameworkCore;

public class EntityFrameworkCoreMultiLayerAbpDbSchemaMigrator
    : IMultiLayerAbpDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMultiLayerAbpDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the MultiLayerAbpDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MultiLayerAbpDbContext>()
            .Database
            .MigrateAsync();
    }
}
