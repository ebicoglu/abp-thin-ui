using Volo.Abp.Modularity;

namespace MultiLayerAbp;

/* Inherit from this class for your domain layer tests. */
public abstract class MultiLayerAbpDomainTestBase<TStartupModule> : MultiLayerAbpTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
