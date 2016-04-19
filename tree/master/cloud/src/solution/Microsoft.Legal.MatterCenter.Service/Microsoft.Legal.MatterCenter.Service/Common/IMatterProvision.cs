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
        GenericResponseVM CreateMatter();
    }
}
