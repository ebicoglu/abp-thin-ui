using MultiLayerAbp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MultiLayerAbp.Permissions;

public class MultiLayerAbpPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MultiLayerAbpPermissions.GroupName);

        myGroup.AddPermission(MultiLayerAbpPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(MultiLayerAbpPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        var booksPermission = myGroup.AddPermission(MultiLayerAbpPermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(MultiLayerAbpPermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(MultiLayerAbpPermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(MultiLayerAbpPermissions.Books.Delete, L("Permission:Books.Delete"));
        //Define your own permissions here. Example:
        //myGroup.AddPermission(MultiLayerAbpPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MultiLayerAbpResource>(name);
    }
}
