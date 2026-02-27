using Volo.Abp.Identity;

namespace MultiLayerAbp;

public static class MultiLayerAbpConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = "1q2w3E*";
}
