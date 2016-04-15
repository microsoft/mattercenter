using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;


namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface IUsersDetails
    {
        Users GetLoggedInUserDetails(ClientContext clientContext);
    }
}
