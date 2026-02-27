using MultiLayerAbp.Localization;
using Volo.Abp.Application.Services;

namespace MultiLayerAbp;

/* Inherit your application services from this class.
 */
public abstract class MultiLayerAbpAppService : ApplicationService
{
    protected MultiLayerAbpAppService()
    {
        LocalizationResource = typeof(MultiLayerAbpResource);
    }
}
