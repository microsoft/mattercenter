using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Service
{
    public interface IEditFunctions
    {
        GenericResponseVM CheckSecurityGroupInTeamMembers(Client client, Matter matter, IList<string> userId);
        bool ValidateFullControlPermission(Matter matter);        
    }
}
