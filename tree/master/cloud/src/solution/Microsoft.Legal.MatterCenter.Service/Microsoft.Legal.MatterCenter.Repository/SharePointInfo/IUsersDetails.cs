using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface IUsersDetails
    {
        Users GetLoggedInUserDetails(ClientContext clientContext);
        Users GetLoggedInUserDetails(Client client);
        IList<FieldUserValue> ResolveUserNames(Client client, IList<string> userNames);
        List<Tuple<int, Principal>> GetUserPrincipal(Client client, Matter matter, IList<string> userIds);
    }
}
