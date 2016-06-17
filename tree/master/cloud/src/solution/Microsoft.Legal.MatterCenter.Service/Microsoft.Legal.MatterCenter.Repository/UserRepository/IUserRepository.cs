using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRepository
    {
        Users GetLoggedInUserDetails(ClientContext clientContext);
        Users GetLoggedInUserDetails(Client client);
        Task<IList<Users>> GetUsersAsync(SearchRequestVM searchRequestVM);
        Task<IList<Role>> GetRolesAsync(Client client);
        Task<IList<Role>> GetPermissionLevelsAsync(Client client);
        Users GetUserProfilePicture(Client client);
    }
}
