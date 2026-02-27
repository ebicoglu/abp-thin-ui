using MultiLayerAbp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MultiLayerAbp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MultiLayerAbpEntityFrameworkCoreModule),
    typeof(MultiLayerAbpApplicationContractsModule)
)]
public class MultiLayerAbpDbMigratorModule : AbpModule
{
}
