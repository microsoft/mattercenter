// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="IMatterRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// This interface contains all the matter related functionalities
// ***********************************************************************

using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// Interface matter repository contains all matter related methods such as finding matter, creating matter, pin, inpin, update matter etc
    /// </summary>
    public interface IMatterRepository: ICommonRepository
    {
        Task<SearchResponseVM> GetMattersAsync(SearchRequestVM searchRequestVM);
        Task<List<FolderData>> GetFolderHierarchyAsync(MatterData matterData);
        Task<IList<Role>> GetRolesAsync(Client client);
        Task<IList<Role>> GetPermissionLevelsAsync(Client client);
        Task<IList<Users>> GetUsersAsync(SearchRequestVM searchRequestVM);
        Task<GenericResponseVM> GetConfigurationsAsync(string siteCollectionUrl);        
        IList<FieldUserValue> ResolveUserNames(Client client, IList<string> userNames);
        IList<string> RoleCheck(string url, string listName, string columnName);
        List<Tuple<int, Principal>> CheckUserSecurity(Client client, Matter matter, IList<string> userIds);
        PropertyValues GetStampedProperties(ClientContext clientContext, string libraryname);
        Users GetLoggedInUserDetails(ClientContext clientContext);
        IEnumerable<RoleAssignment> FetchUserPermissionForLibrary(ClientContext clientContext, string libraryname);
        string GetMatterName(ClientContext clientContext, string matterName);
        int RetrieveItemId(ClientContext clientContext, string matterLandingPageRepositoryName, string originalMatterName);
        List<string> MatterAssociatedLists(ClientContext clientContext, string matterName, MatterConfigurations matterConfigurations = null);
        bool UpdatePermission(ClientContext clientContext, Matter matter, List<string> users, string loggedInUserTitle, bool isListItem, string listName, int matterLandingPageId, bool isEditMode);
        bool UpdateMatterStampedProperties(ClientContext clientContext, MatterDetails matterDetails, Matter matter, PropertyValues matterStampedProperties, bool isEditMode);
        void AssignRemoveFullControl(ClientContext clientContext, Matter matter, string loggedInUser, int listItemId, List<string> listExists, bool assignFullControl, bool hasFullPermission);
        bool RevertMatterUpdates(Client client, Matter matter, ClientContext clientContext, MatterRevertList matterRevertListObject, string loggedInUserTitle, IEnumerable<RoleAssignment> oldUserPermissions, int matterLandingPageId, bool isEditMode);
        void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList);
        bool AddItem(ClientContext clientContext, List list, IList<string> columns, IList<object> values);
        GenericResponseVM DeleteMatter(Client client, Matter matter);
        GenericResponseVM ValidateTeamMembers(ClientContext clientContext, Matter matter, IList<string> userId);
        GenericResponseVM SaveConfigurationToList(MatterConfigurations matterConfigurations, ClientContext clientContext, string cachedItemModifiedDate);
        ListItem GetItem(ClientContext clientContext, string listName, string listQuery);
        bool SetPermission(ClientContext clientContext, IList<IList<string>> assignUserName, IList<string> permissions, string listName);

    }
}
