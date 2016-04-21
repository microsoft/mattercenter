using Microsoft.Legal.MatterCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter
{
    public interface IValidationFunctions
    {
        GenericResponseVM IsMatterValid(MatterInformationVM matterInformation, int methodNumber, MatterConfigurations matterConfigurations);
    }
}
