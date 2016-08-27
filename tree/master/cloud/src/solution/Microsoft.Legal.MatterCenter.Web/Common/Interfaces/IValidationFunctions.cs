using Microsoft.Legal.MatterCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IValidationFunctions
    {
        GenericResponseVM IsMatterValid(MatterInformationVM matterInformation, int methodNumber, MatterConfigurations matterConfigurations);
        GenericResponseVM MatterDetailsValidation(Matter matter, Client client, int methodNumber,
            MatterConfigurations matterConfigurations);
        GenericResponseVM MatterMetadataValidation(Matter matter, Client client,
            int methodNumber, MatterConfigurations matterConfigurations);
        List<string> CheckListExists(Client client, string matterName, MatterConfigurations matterConfigurations = null);
        GenericResponseVM RoleCheck(Matter matter);
    }
}
