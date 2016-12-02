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
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        
        Task<GenericResponseVM> GetConfigurationsAsync(string siteCollectionUrl);        
        IList<FieldUserValue> ResolveUserNames(Client client, IList<string> userNames);
        IList<string> RoleCheck(string url, string listName, string columnName);
        List<Tuple<int, Principal>> CheckUserSecurity(Client client, Matter matter, IList<string> userIds);
        PropertyValues GetStampedProperties(ClientContext clientContext, string libraryname);
        
        IEnumerable<RoleAssignment> FetchUserPermissionForLibrary(ClientContext clientContext, string libraryname);
        string GetMatterName(ClientContext clientContext, string matterName);
        int RetrieveItemId(ClientContext clientContext, string matterLandingPageRepositoryName, string originalMatterName);
        List<string> MatterAssociatedLists(ClientContext clientContext, string matterName, MatterConfigurations matterConfigurations = null);
        bool UpdatePermission(ClientContext clientContext, Matter matter, List<string> users, string loggedInUserTitle, bool isListItem, string listName, int matterLandingPageId, bool isEditMode);
        bool UpdateMatterStampedProperties(ClientContext clientContext, MatterDetails matterDetails, Matter matter, PropertyValues matterStampedProperties, bool isEditMode);
        void AssignRemoveFullControl(ClientContext clientContext, Matter matter, string loggedInUser, int listItemId, List<string> listExists, bool assignFullControl, bool hasFullPermission);
        bool RevertMatterUpdates(Client client, Matter matter, MatterRevertList matterRevertListObject, bool isEditMode, IEnumerable<RoleAssignment> userPermissionOnLibrary);
        void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList);
        bool AddItem(ClientContext clientContext, List list, IList<string> columns, IList<object> values);
        GenericResponseVM DeleteMatter(Client client, Matter matter);
        GenericResponseVM ValidateTeamMembers(ClientContext clientContext, Matter matter, IList<string> userId);
        GenericResponseVM SaveConfigurationToList(MatterConfigurations matterConfigurations, ClientContext clientContext, string cachedItemModifiedDate);
        ListItem GetItem(ClientContext clientContext, string listName, string listQuery);
        bool SetPermission(ClientContext clientContext, IList<IList<string>> assignUserName, IList<string> permissions, string listName);
        IList<ContentType> GetContentTypeData(ClientContext clientContext, IList<string> contentTypes, Client client, Matter matter);
        GenericResponseVM AssignContentTypeHelper(MatterMetadata matterMetadata, ClientContext clientContext, IList<ContentType> contentTypeCollection, Client client, Matter matter);
        List<string> Exists(Client client, ReadOnlyCollection<string> lists);
        bool IsPageExists(ClientContext clientContext, string pageUrl);
        bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission);
        bool CreateList(ClientContext clientContext, ListInformation listInformation);
        bool BreakPermission(ClientContext clientContext, string libraryName, bool isCopyRoleAssignment);
        string AddOneNote(ClientContext clientContext, string clientAddressPath, string oneNoteLocation, string listName, string oneNoteTitle);
        bool SaveMatter(Client client, Matter matter, string matterListName, MatterConfigurations matterConfigurations, string matterSiteURL);
        int CreateWebPartPage(ClientContext clientContext, string pageName, string layout, string masterpagelistName, string listName, string pageTitle);
        bool BreakItemPermission(ClientContext clientContext, string listName, int listItemId, bool isCopyRoleAssignment);
        bool SetItemPermission(ClientContext clientContext, IList<IList<string>> assignUserName, string listName, int listItemId, IList<string> permissions);
        string[] ConfigureXMLCodeOfWebParts(Client client, Matter matter, ClientContext clientContext, string pageName, Uri uri,
            Web web, MatterConfigurations matterConfigurations);
        bool AddWebPart(ClientContext clientContext, LimitedWebPartManager limitedWebPartManager, WebPartDefinition webPartDefinition,
            string[] webParts, string[] zones);
        GenericResponseVM ShareMatterToExternalUser(MatterInformationVM matterInformation);

        GenericResponseVM UpdateMatter(MatterInformationVM matterInformation);
        bool CanCreateMatter(Client client);

        /// <summary>
        /// The implementatioon of this method will save matter configutations in sharepoint list
        /// </summary>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        GenericResponseVM SaveConfigurations(ClientContext clientContext, MatterConfigurations matterConfigurations);

        bool OneNoteUrlExists(MatterInformationVM matterInformation);
    }
}
