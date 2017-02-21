using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMatterProvision: ISharedProvision
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterInformation"></param>
        /// <returns></returns>
        GenericResponseVM UpdateMatter(MatterInformationVM matterInformation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterMetadata"></param>
        /// <returns></returns>
        GenericResponseVM UpdateMatterMetadata(MatterMetdataVM matterMetadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterVM"></param>
        /// <returns></returns>   
        GenericResponseVM DeleteMatter(MatterVM matterVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveConfigurationsVM"></param>
        /// <returns></returns>
        GenericResponseVM SavConfigurations(MatterConfigurations matterConfiguration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterVM"></param>
        /// <returns></returns>
        MatterStampedDetails GetStampedProperties(MatterVM matterVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterMetadata"></param>
        /// <returns></returns>
        GenericResponseVM AssignUserPermissions(MatterMetdataVM matterMetadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterMetadata"></param>
        /// <returns></returns>
        GenericResponseVM AssignContentType(MatterMetadata matterMetadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        Task<SearchResponseVM> GetMatters(SearchRequestVM searchRequestVM, ClientContext clientContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        GenericResponseVM CheckMatterExists(MatterMetdataVM matterMetadataVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterInformationVM"></param>
        /// <returns></returns>
        GenericResponseVM CheckSecurityGroupExists(MatterInformationVM matterInformationVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        GenericResponseVM CreateMatter(MatterMetdataVM matterMetadataVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        GenericResponseVM CreateMatterLandingPage(MatterMetdataVM matterMetadataVM);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matterInformation"></param>
        /// <returns></returns>
        GenericResponseVM ShareMatterToExternalUser(MatterInformationVM matterInformation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        bool CanCreateMatter(Client client);

        /// <summary>
        /// The implementatioon of this method will save matter configutations in sharepoint list
        /// </summary>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        GenericResponseVM SaveConfigurations(MatterConfigurations matterConfigurations);
        GenericResponseVM DeleteUserFromMatter(MatterInformationVM matterInformation);
    }
}
