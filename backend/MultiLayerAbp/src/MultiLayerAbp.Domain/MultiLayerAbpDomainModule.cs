using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiLayerAbp.Localization;
using MultiLayerAbp.MultiTenancy;
using System;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Caching;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.LanguageManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Saas;
using Volo.Abp.Gdpr;

namespace MultiLayerAbp;

[DependsOn(
    typeof(MultiLayerAbpDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpOpenIddictProDomainModule),
    typeof(SaasDomainModule),
    typeof(TextTemplateManagementDomainModule),
    typeof(LanguageManagementDomainModule),
    typeof(VoloAbpCommercialSuiteTemplatesModule),
    typeof(AbpGdprDomainModule),
    typeof(BlobStoringDatabaseDomainModule)
    )]
public class MultiLayerAbpDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });



#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
    }
}
