using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;


namespace Microsoft.Legal.MatterCenter.Service
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
    }
}
