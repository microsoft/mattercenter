// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Repository
// Author           : v-lapedd
// Created          : 07-07-2016
//***************************************************************************

// ***********************************************************************
// <copyright file="TaxonomyHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to get information related to user</summary>

using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

using Microsoft.Legal.MatterCenter.Models;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This interface constains all the methods which help in getting or updating user details  or permissions in SharePoint
    /// </summary>
    public interface IUsersDetails
    {
        Users GetLoggedInUserDetails(ClientContext clientContext);
        Users GetLoggedInUserDetails(Client client);
        IList<FieldUserValue> ResolveUserNames(Client client, IList<string> userNames);
        IList<FieldUserValue> ResolveUserNames(ClientContext clientContext, IList<string> userNames);
        List<Tuple<int, Principal>> GetUserPrincipal(Client client, Matter matter, IList<string> userIds);
        Task<Users> GetUserProfilePicture(Client client);
        bool CheckUserPresentInMatterCenter(string clientUrl, string email);
        bool CheckUserPresentInMatterCenter(ClientContext clientContext, string email);
        /// <summary>
        ///  This method will check whether login user is part of owner group or not
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        bool IsLoginUserOwner(Client client);
    }
}
