using Microsoft.Extensions.Localization;
using MultiLayerAbp.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MultiLayerAbp;

[Dependency(ReplaceServices = true)]
public class MultiLayerAbpBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MultiLayerAbpResource> _localizer;

    public MultiLayerAbpBrandingProvider(IStringLocalizer<MultiLayerAbpResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
