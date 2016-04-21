using Microsoft.Legal.MatterCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Service
{
    public interface IMatterProvision
    {
        GenericResponseVM UpdateMatter(MatterInformationVM matterInformation);
        GenericResponseVM UpdateMatterMetadata(MatterMetdataVM matterMetadata);
        GenericResponseVM CreateMatter();
        GenericResponseVM DeleteMatter(Client client, Matter matter);
    }
}
