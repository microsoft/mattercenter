using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IMatterProvision
    {
        GenericResponseVM UpdateMatter(MatterInformationVM matterInformation);
        GenericResponseVM UpdateMatterMetadata(MatterMetdataVM matterMetadata);
        GenericResponseVM CreateMatter();
        GenericResponseVM DeleteMatter(Client client, Matter matter);
        GenericResponseVM SavConfigurations(SaveConfigurationsVM saveConfigurationsVM);
        PropertyValues GetStampedProperties(MatterVM matterVM);
        GenericResponseVM AssignUserPermissions(MatterMetdataVM matterMetadata);
        GenericResponseVM AssignContentType(MatterMetadata matterMetadata);
        Task<IEnumerable<MatterData>> GetMatters(SearchRequestVM searchRequestVM);
    }
}
