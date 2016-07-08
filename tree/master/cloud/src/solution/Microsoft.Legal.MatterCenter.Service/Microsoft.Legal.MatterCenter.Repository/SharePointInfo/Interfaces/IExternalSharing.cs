using Microsoft.Legal.MatterCenter.Models;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface IExternalSharing
    {
        GenericResponseVM ShareMatter(MatterInformationVM matterInformation);
        
    }
}
