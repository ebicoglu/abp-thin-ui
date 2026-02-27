using Volo.Abp.Modularity;

namespace MultiLayerAbp;

[DependsOn(
    typeof(MultiLayerAbpApplicationModule),
    typeof(MultiLayerAbpDomainTestModule)
)]
public class MultiLayerAbpApplicationTestModule : AbpModule
{

}
