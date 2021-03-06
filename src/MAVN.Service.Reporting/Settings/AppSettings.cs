using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.Campaign.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.PartnerManagement.Client;

namespace MAVN.Service.Reporting.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public ReportSettings ReportService { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileService { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementService { get; set; }

        public CampaignServiceClientSettings CampaignService { get; set; }
    }
}
