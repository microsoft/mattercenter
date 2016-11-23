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
        /// <summary>
        /// This method will get logged in user infor object
        /// </summary>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        Users GetLoggedInUserDetails(ClientContext clientContext);

        /// <summary>
        /// This method will get logged in user infor object
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Users GetLoggedInUserDetails(Client client);


        /// <summary>
        /// This method will return all users for the filter criteria that user has specified in the UI
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        Task<IList<Users>> GetUsersAsync(SearchRequestVM searchRequestVM);

        /// <summary>
        /// Thios method will get all roles such as attorney journal etc which are configured in the catalog site collection
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Task<IList<Role>></returns>
        Task<IList<Role>> GetRolesAsync(Client client);

        /// <summary>
        /// This method will get all permissions levels that are configured for a given site collection
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Task<IList<Role>></returns>
        Task<IList<Role>> GetPermissionLevelsAsync(Client client);

        /// <summary>
        /// This method will get current user profile picture of the current logged in user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Users</returns>
        Users GetUserProfilePicture(Client client);

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="clientUrl"></param>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        bool CheckUserPresentInMatterCenter(Client client);


        /// <summary>
        ///  This method will check whether login user is part of owner group or not
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        bool IsLoginUserOwner(Client client);
    }
}
