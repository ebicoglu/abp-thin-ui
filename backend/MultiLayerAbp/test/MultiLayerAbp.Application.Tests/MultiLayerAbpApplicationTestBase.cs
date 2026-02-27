using Volo.Abp.Modularity;

namespace MultiLayerAbp;

public abstract class MultiLayerAbpApplicationTestBase<TStartupModule> : MultiLayerAbpTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
