using Volo.Abp.Modularity;

namespace MultiLayerAbp;

[DependsOn(
    typeof(MultiLayerAbpDomainModule),
    typeof(MultiLayerAbpTestBaseModule)
)]
public class MultiLayerAbpDomainTestModule : AbpModule
{

}
