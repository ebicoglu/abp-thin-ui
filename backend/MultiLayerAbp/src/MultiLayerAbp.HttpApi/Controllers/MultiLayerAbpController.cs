using MultiLayerAbp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MultiLayerAbp.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MultiLayerAbpController : AbpControllerBase
{
    protected MultiLayerAbpController()
    {
        LocalizationResource = typeof(MultiLayerAbpResource);
    }
}
