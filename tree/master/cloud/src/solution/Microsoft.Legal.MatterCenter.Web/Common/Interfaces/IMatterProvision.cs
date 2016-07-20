using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IMatterProvision: ISharedProvision
    {
        GenericResponseVM UpdateMatter(MatterInformationVM matterInformation);
        GenericResponseVM UpdateMatterMetadata(MatterMetdataVM matterMetadata);        
        GenericResponseVM DeleteMatter(MatterVM matterVM);
        GenericResponseVM SavConfigurations(SaveConfigurationsVM saveConfigurationsVM);
        MatterStampedDetails GetStampedProperties(MatterVM matterVM);
        GenericResponseVM AssignUserPermissions(MatterMetdataVM matterMetadata);
        GenericResponseVM AssignContentType(MatterMetadata matterMetadata);
        Task<SearchResponseVM> GetMatters(SearchRequestVM searchRequestVM);
        GenericResponseVM CheckMatterExists(MatterMetdataVM matterMetadataVM);
        GenericResponseVM CheckSecurityGroupExists(MatterInformationVM matterInformationVM);
        GenericResponseVM CreateMatter(MatterMetdataVM matterMetadataVM);
        GenericResponseVM CreateMatterLandingPage(MatterMetdataVM matterMetadataVM);
        GenericResponseVM ShareMatterToExternalUser(MatterInformationVM matterInformation);
    }
}
