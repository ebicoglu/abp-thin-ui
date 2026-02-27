using Volo.Abp.Settings;

namespace MultiLayerAbp.Settings;

public class MultiLayerAbpSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MultiLayerAbpSettings.MySetting1));
    }
}
