// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-rijadh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="ProvisionHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods involved in matter provisioning.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.ProviderService.HelperClasses;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.Security.Application;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provide methods involved in matter provisioning.
    /// </summary>
    internal static class ProvisionHelperFunctions
    {
        /// <summary>
        /// Assigns field values for specified content types to the specified matter (document library).
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <param name="clientContext">SP client context</param>
        /// <param name="contentTypeCollection">Collection of content types</param>
        /// <param name="client">Client Object</param>
        /// <param name="matter">Matter Object</param>
        /// <returns>true if success else false</returns>
        internal static string AssignContentTypeHelper(RequestObject requestObject, MatterMetadata matterMetadata, ClientContext clientContext, IList<ContentType> contentTypeCollection, Client client, Matter matter)
        {
            Web web = clientContext.Web;
            List matterList = web.Lists.GetByTitle(matter.Name);
            SetFieldValues(clientContext, contentTypeCollection, matterList, matterMetadata);
            clientContext.ExecuteQuery();
            SetDefaultContentType(clientContext, matterList, requestObject, client, matter);
            string[] viewColumnList = ServiceConstantStrings.ViewColumnList.Split(new string[] { ConstantStrings.Semicolon }, StringSplitOptions.RemoveEmptyEntries).Select(listEntry => listEntry.Trim()).ToArray();
            string strQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.ViewOrderByQuery, ServiceConstantStrings.ViewOrderByColumn);
            bool isViewCreated = Lists.AddView(clientContext, matterList, viewColumnList, ServiceConstantStrings.ViewName, strQuery);
            return (string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, Convert.ToString(isViewCreated, CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// Assigns field values for specified content types to the specified matter (document library).
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="contentTypeCollection">Collection of content types</param>
        /// <param name="matterList">List containing matters</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        internal static void SetFieldValues(ClientContext clientContext, IList<ContentType> contentTypeCollection, List matterList, MatterMetadata matterMetadata)
        {
            FieldCollection fields = GetContentType(clientContext, contentTypeCollection, matterList);
            if (null != fields)
            {
                matterMetadata = ProvisionHelperFunctionsUtility.GetWSSId(clientContext, matterMetadata, fields);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientId).DefaultValue = matterMetadata.Client.Id;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientId).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientId).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientId).Update();
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientName).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientName).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientName).DefaultValue = matterMetadata.Client.Name;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnClientName).Update();
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterId).DefaultValue = matterMetadata.Matter.Id;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterId).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterId).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterId).Update();
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterName).DefaultValue = matterMetadata.Matter.Name;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterName).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterName).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnMatterName).Update();
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnPracticeGroup).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnPracticeGroup).DefaultValue = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MetadataDefaultValue, matterMetadata.PracticeGroupTerm.WssId, matterMetadata.PracticeGroupTerm.TermName, matterMetadata.PracticeGroupTerm.Id);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnPracticeGroup).Update();
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnAreaOfLaw).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnAreaOfLaw).DefaultValue = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MetadataDefaultValue, matterMetadata.AreaTerm.WssId, matterMetadata.AreaTerm.TermName, matterMetadata.AreaTerm.Id);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnAreaOfLaw).Update();
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnSubareaOfLaw).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnSubareaOfLaw).DefaultValue = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MetadataDefaultValue, matterMetadata.SubareaTerm.WssId, matterMetadata.SubareaTerm.TermName, matterMetadata.SubareaTerm.Id);
                fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnSubareaOfLaw).Update();
            }
        }

        /// <summary>
        /// Sets the default content type based on user selection.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="list">Name of the list</param>
        /// <param name="requestObject">Request Object</param>
        /// <param name="client">Client Object</param>
        /// <param name="matter">Matter Object</param>
        internal static void SetDefaultContentType(ClientContext clientContext, List list, RequestObject requestObject, Client client, Matter matter)
        {
            int contentCount = 0, contentSwap = 0;
            try
            {
                ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
                clientContext.Load(currentContentTypeOrder);
                clientContext.ExecuteQuery();
                IList<ContentTypeId> updatedContentTypeOrder = new List<ContentTypeId>();
                foreach (ContentType contentType in currentContentTypeOrder)
                {
                    if (0 == string.Compare(contentType.Name, matter.DefaultContentType, StringComparison.OrdinalIgnoreCase))
                    {
                        contentSwap = contentCount;
                    }

                    if (0 != string.Compare(contentType.Name, ServiceConstantStrings.HiddenContentType, StringComparison.OrdinalIgnoreCase))
                    {
                        updatedContentTypeOrder.Add(contentType.Id);
                        contentCount++;
                    }
                }
                if (updatedContentTypeOrder.Count > contentSwap)
                {
                    ContentTypeId documentContentType = updatedContentTypeOrder[0];
                    updatedContentTypeOrder[0] = updatedContentTypeOrder[contentSwap];
                    updatedContentTypeOrder.RemoveAt(contentSwap);
                    updatedContentTypeOrder.Add(documentContentType);
                }
                list.RootFolder.UniqueContentTypeOrder = updatedContentTypeOrder;
                list.RootFolder.Update();
                list.Update();
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Saves the matter details in centralized list.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterListName">Name of list where matter creation entry is logged</param>
        /// <param name="matterConfigurations">matterConfigurations object consist of configuration of matter</param>
        /// <param name="clientContext">Client context</param>
        /// <returns>true if success else false</returns>
        internal static string SaveMatterDetails(RequestObject requestObject, Client client, Matter matter, string matterListName, MatterConfigurations matterConfigurations, ClientContext clientContext)
        {
            string returnFlag = ConstantStrings.FALSE;
            try
            {
                if (!string.IsNullOrWhiteSpace(matterListName) && null != requestObject && null != matter && null != client)
                {
                    FieldUserValue tempUser = null;
                    List<FieldUserValue> blockUserList = null;
                    List<List<FieldUserValue>> assignUserList = null;

                    List<string> columnNames = new List<string>()
                        {
                            ServiceConstantStrings.MattersListColumnTitle,
                            ServiceConstantStrings.MattersListColumnClientName,
                            ServiceConstantStrings.MattersListColumnClientID,
                            ServiceConstantStrings.MattersListColumnMatterName,
                            ServiceConstantStrings.MattersListColumnMatterID
                        };
                    List<object> columnValues = new List<object>()
                        {
                            string.Concat(client.Name, ConstantStrings.Underscore, matter.Name),
                            client.Name,
                            client.Id,
                            matter.Name,
                            matter.Id
                        };

                    if (matterConfigurations.IsConflictCheck)
                    {

                        if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.CheckBy))
                        {
                            tempUser = SharePointHelper.ResolveUserNames(clientContext, new List<string>() { matter.Conflict.CheckBy }).FirstOrDefault();
                            columnNames.Add(ServiceConstantStrings.MattersListColumnConflictCheckBy);
                            columnValues.Add(tempUser);
                            if (!string.IsNullOrWhiteSpace(matter.Conflict.CheckOn))
                            {
                                columnNames.Add(ServiceConstantStrings.MattersListColumnConflictCheckOn);
                                columnValues.Add(Convert.ToDateTime(matter.Conflict.CheckOn, CultureInfo.InvariantCulture));
                            }

                            columnNames.Add(ServiceConstantStrings.MattersListColumnConflictIdentified);
                            columnValues.Add(Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture));
                        }

                        if (null != matter.BlockUserNames)
                        {
                            blockUserList = new List<FieldUserValue>();
                            blockUserList = SharePointHelper.ResolveUserNames(clientContext, matter.BlockUserNames).ToList();
                            columnNames.Add(ServiceConstantStrings.MattersListColumnBlockUsers);
                            columnValues.Add(blockUserList);
                        }
                    }

                    if (null != matter.AssignUserEmails)
                    {
                        assignUserList = new List<List<FieldUserValue>>();
                        foreach (IList<string> assignUsers in matter.AssignUserEmails)
                        {
                            List<FieldUserValue> tempAssignUserList = SharePointHelper.ResolveUserNames(clientContext, assignUsers).ToList();
                            assignUserList.Add(tempAssignUserList);
                        }

                        if (0 != assignUserList.Count && null != matter.Roles && 0 != matter.Roles.Count)
                        {
                            int assignPosition = 0;
                            List<FieldUserValue> managingAttorneyList = new List<FieldUserValue>();
                            List<FieldUserValue> teamMemberList = new List<FieldUserValue>();
                            foreach (string role in matter.Roles)
                            {
                                switch (role)
                                {
                                    case ConstantStrings.ManagingAttorneyValue:
                                        managingAttorneyList.AddRange(assignUserList[assignPosition]);
                                        break;
                                    default:
                                        teamMemberList.AddRange(assignUserList[assignPosition]);
                                        break;
                                }

                                assignPosition++;
                            }

                            columnNames.Add(ServiceConstantStrings.MattersListColumnManagingAttorney);
                            columnValues.Add(managingAttorneyList);
                            columnNames.Add(ServiceConstantStrings.MattersListColumnSupport);
                            columnValues.Add(teamMemberList);
                        }
                    }

                    Microsoft.SharePoint.Client.Web web = clientContext.Web;
                    List matterList = web.Lists.GetByTitle(matterListName);
                    // To avoid the invalid symbol error while parsing the JSON, return the response in lower case
                    returnFlag = Convert.ToString(Lists.AddItem(clientContext, matterList, columnNames, columnValues), CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentUICulture);
                }
            }
            catch (Exception exception)
            {
                //// SharePoint Specific Exception
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            return returnFlag;
        }

        /// <summary>
        /// Retrieves the list of content types that are to be associated with the matter.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="contentTypeCollection">Collection of content types</param>
        /// <param name="matterList">List containing matters</param>
        /// <returns>Content types in Field Collection object</returns>
        internal static FieldCollection GetContentType(ClientContext clientContext, IList<ContentType> contentTypeCollection, List matterList)
        {
            foreach (ContentType contenttype in contentTypeCollection)
            {
                matterList.ContentTypesEnabled = true;
                matterList.ContentTypes.AddExistingContentType(contenttype);
            }

            matterList.Update();
            FieldCollection fields = matterList.Fields;
            clientContext.Load(fields);
            clientContext.Load(matterList);
            clientContext.ExecuteQuery();
            return fields;
        }

        /// <summary>
        /// Generates list of users for sending email.
        /// </summary>
        /// <param name="matter">Matter details</param>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="userList">List of users associated with the matter</param>
        /// <returns>List of users to whom mail is to be sent</returns>
        internal static List<FieldUserValue> GenerateMailList(Matter matter, ClientContext clientContext, ref List<FieldUserValue> userList)
        {
            List<FieldUserValue> result = null;
            try
            {
                List<FieldUserValue> userEmailList = new List<FieldUserValue>();
                if (null != matter.AssignUserEmails)
                {
                    foreach (IList<string> userNames in matter.AssignUserEmails)
                    {
                        userList = SharePointHelper.ResolveUserNames(clientContext, userNames).ToList();
                        foreach (FieldUserValue userEmail in userList)
                        {
                            userEmailList.Add(userEmail);
                        }
                    }
                }
                result = userEmailList;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                List<FieldUserValue> userEmailList = new List<FieldUserValue>();
                result = userEmailList;
            }
            return result;
        }

        /// <summary>
        /// Generates list of users permissions which are allowed on created matter.
        /// </summary>
        /// <param name="roles">List of Roles present in the SharePoint Permissions</param>
        /// <param name="web">SharePoint Web Object</param>
        /// <returns>List of permissions which are allowed on the created matter</returns>
        internal static List<Role> GetAllowedUserPermissions(List<Role> roles, Web web)
        {
            string userAllowedPermissions = ServiceConstantStrings.UserPermissions;
            List<Role> result = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(userAllowedPermissions))
                {
                    //// Get the user permissions from the Resource file
                    List<string> userPermissions = userAllowedPermissions.ToUpperInvariant().Trim().Split(new string[] { ConstantStrings.Comma }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //// Filter only the allowed roles using LINQ query
                    List<RoleDefinition> roleDefinition = (from webRole in web.RoleDefinitions.ToList()
                                                           where userPermissions.Contains(webRole.Name.ToUpperInvariant())
                                                           select webRole).ToList();
                    foreach (RoleDefinition role in roleDefinition)
                    {
                        Role tempRole = new Role();
                        tempRole.Name = role.Name;
                        tempRole.Id = Convert.ToString(role.Id, CultureInfo.InvariantCulture);
                        roles.Add(tempRole);
                    }
                }
                result = roles.OrderBy(role => role.Name).ToList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                List<Role> GetAllowedUserPermissionsError = new List<Role>();
                result = GetAllowedUserPermissionsError;
            }
            return result;
        }

        /// <summary>
        /// Iterates through the list of users and maps required properties.
        /// </summary>
        /// <param name="users">List of users</param>
        /// <param name="userSet">Users obtained from people picker service based on the search term</param>
        /// <returns>Returns resultant list of users for matter provision</returns>
        internal static IList<Users> FilterUsers(IList<Users> users, IList<PeoplePickerUser> userSet)
        {
            foreach (PeoplePickerUser item in userSet)
            {
                Users tempUser = new Users();
                tempUser.Name = Convert.ToString(item.DisplayText, CultureInfo.InvariantCulture);
                tempUser.LogOnName = Convert.ToString(item.Key, CultureInfo.InvariantCulture);
                tempUser.Email = string.Equals(item.EntityType, ConstantStrings.PeoplePickerEntityTypeUser, StringComparison.OrdinalIgnoreCase) ? Convert.ToString(item.Description, CultureInfo.InvariantCulture) : Convert.ToString(item.EntityData.Email, CultureInfo.InvariantCulture);
                tempUser.EntityType = Convert.ToString(item.EntityType, CultureInfo.InvariantCulture);
                tempUser.ProviderName = Convert.ToString(item.ProviderName, CultureInfo.InvariantCulture);
                tempUser.EntityData = new EntityData()
                {
                    Department = string.IsNullOrWhiteSpace(item.EntityData.Department) ? string.Empty : item.EntityData.Department,
                    Email = string.IsNullOrWhiteSpace(item.EntityData.Email) ? tempUser.Email : item.EntityData.Email,
                    Title = string.IsNullOrWhiteSpace(item.EntityData.Title) ? string.Empty : item.EntityData.Title
                };
                users.Add(tempUser);
            }
            return users;
        }

        /// <summary>
        /// Returns roles from list.
        /// </summary>
        /// <param name="roles">list of roles</param>
        /// <param name="collListItem">list of item collection</param>
        /// <returns>resultant list of roles</returns>
        internal static IList<Role> GetRoleDataUtility(IList<Role> roles, ListItemCollection collListItem)
        {
            if (null != collListItem && 0 != collListItem.Count)
            {
                foreach (ListItem item in collListItem)
                {
                    Role tempRole = new Role();
                    tempRole.Id = Convert.ToString(item[ServiceConstantStrings.ColumnNameGuid], CultureInfo.InvariantCulture);
                    tempRole.Name = Convert.ToString(item[ServiceConstantStrings.RoleListColumnRoleName], CultureInfo.InvariantCulture);
                    tempRole.Mandatory = Convert.ToBoolean(item[ServiceConstantStrings.RoleListColumnIsRoleMandatory], CultureInfo.InvariantCulture);
                    roles.Add(tempRole);
                }
            }
            return roles;
        }

        /// <summary>
        /// Provides the permission levels present in the site.
        /// </summary>
        /// <param name="requestObject">object of type RequestObject</param>
        /// <param name="client">object of client</param>
        /// <param name="returnValue">value to be returned</param>
        /// <returns>permission levels</returns>
        internal static string GetPermissionLevelUtility(RequestObject requestObject, Client client, string returnValue)
        {
            try
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                {
                    List<Role> roles = new List<Role>();
                    Web web = clientContext.Web;
                    clientContext.Load(web.RoleDefinitions, roledefinitions => roledefinitions.Include(thisRole => thisRole.Name, thisRole => thisRole.Id));
                    clientContext.ExecuteQuery();
                    roles = ProvisionHelperFunctions.GetAllowedUserPermissions(roles, web);
                    returnValue = JsonConvert.SerializeObject(roles);
                }
            }
            catch (Exception exception)
            {
                ///// SharePoint Specific Exception
                returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return returnValue;
        }

        /// <summary>
        /// Utility function to create matter.
        /// </summary>
        /// <param name="requestObject">object of type request object</param>
        /// <param name="client">object of client</param>
        /// <param name="matter">object of matter type</param>
        /// <param name="clientContext">client context information</param>
        /// <param name="matterURL">URL of matter</param>
        /// <returns>Matter URL</returns>
        internal static string CreateMatterUtility(RequestObject requestObject, Client client, Matter matter, ClientContext clientContext, string matterURL, MatterConfigurations matterConfigurations)
        {
            try
            {
                Uri centralListURL = new Uri(string.Concat(ServiceConstantStrings.CentralRepositoryUrl, ConstantStrings.ForwardSlash, ConstantStrings.Lists, ConstantStrings.ForwardSlash, ServiceConstantStrings.DMSMatterListName)); // Central Repository List URL                   
                IList<string> documentLibraryFolders = new List<string>();
                Dictionary<string, bool> documentLibraryVersioning = new Dictionary<string, bool>();
                Uri clientUrl = new Uri(client.Url);

                ListInformation listInformation = new ListInformation();
                listInformation.name = matter.Name;
                listInformation.description = matter.Description;
                listInformation.folderNames = matter.FolderNames;
                listInformation.isContentTypeEnable = true;
                listInformation.versioning = new VersioningInfo();
                listInformation.versioning.EnableVersioning = ServiceConstantStrings.IsMajorVersionEnable;
                listInformation.versioning.EnableMinorVersions = ServiceConstantStrings.IsMinorVersionEnable;
                listInformation.versioning.ForceCheckout = ServiceConstantStrings.IsForceCheckOut;
                listInformation.Path = matter.MatterGuid;

                Lists.Create(clientContext, listInformation);

                documentLibraryVersioning.Add("EnableVersioning", false);
                documentLibraryFolders.Add(matter.MatterGuid);
                listInformation.name = matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix;
                listInformation.folderNames = documentLibraryFolders;
                listInformation.versioning.EnableVersioning = false;
                listInformation.versioning.EnableMinorVersions = false;
                listInformation.versioning.ForceCheckout = false;
                listInformation.Path = matter.MatterGuid + ServiceConstantStrings.OneNoteLibrarySuffix;
                Lists.Create(clientContext, listInformation);

                bool isCopyRoleAssignment = CopyRoleAssignment(matter.Conflict.Identified, matter.Conflict.SecureMatter);
                //create calendar list if create calendar flag is enabled and break its permissions
                string calendarName = string.Concat(matter.Name, ServiceConstantStrings.CalendarNameSuffix);
                string taskListName = string.Concat(matter.Name, ServiceConstantStrings.TaskNameSuffix);
                if (ServiceConstantStrings.IsCreateCalendarEnabled && matterConfigurations.IsCalendarSelected)
                {
                    ListInformation calendarInformation = new ListInformation();
                    calendarInformation.name = calendarName;
                    calendarInformation.isContentTypeEnable = false;
                    calendarInformation.templateType = ConstantStrings.CalendarName;
                    calendarInformation.Path = ServiceConstantStrings.TitleListsPath + matter.MatterGuid + ServiceConstantStrings.CalendarNameSuffix;

                    if (Lists.Create(clientContext, calendarInformation))
                    {
                        Lists.BreakPermission(clientContext, calendarName, isCopyRoleAssignment);
                    }
                    else
                    {
                        MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeAddCalendarList, TextConstants.ErrorMessageAddCalendarList);
                        throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                    }
                }
                if (matterConfigurations.IsTaskSelected)
                {
                    ListInformation taskListInformation = new ListInformation();
                    taskListInformation.name = taskListName;
                    taskListInformation.isContentTypeEnable = false;
                    taskListInformation.templateType = ConstantStrings.TaskListTemplateType;
                    taskListInformation.Path = ServiceConstantStrings.TitleListsPath + matter.MatterGuid + ServiceConstantStrings.TaskNameSuffix;
                    if (Lists.Create(clientContext, taskListInformation))
                    {
                        Lists.BreakPermission(clientContext, taskListName, isCopyRoleAssignment);
                    }
                    else
                    {
                        MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeAddTaskList, TextConstants.ErrorMessageAddTaskList);
                        throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                    }
                }
                string oneNoteUrl = string.Concat(clientUrl.AbsolutePath, ConstantStrings.ForwardSlash, matter.MatterGuid, ServiceConstantStrings.OneNoteLibrarySuffix, ConstantStrings.ForwardSlash, matter.MatterGuid);
                matterURL = Lists.AddOneNote(clientContext, client.Url, oneNoteUrl, matter.MatterGuid, matter.Name);
                matterURL = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, client.Url);
                if (null != matter.Conflict)
                {
                    //Break permission for Matter library
                    Lists.BreakPermission(clientContext, matter.Name, isCopyRoleAssignment);

                    //Break permission for OneNote document library
                    string oneNoteLibraryName = string.Concat(matter.Name, ServiceConstantStrings.OneNoteLibrarySuffix);
                    Lists.BreakPermission(clientContext, oneNoteLibraryName, isCopyRoleAssignment);
                }
                string roleCheck = ValidationHelperFunctions.RoleCheck(requestObject, matter, client);
                if (string.IsNullOrEmpty(roleCheck))
                {
                    string centralList = Convert.ToString(centralListURL, CultureInfo.InvariantCulture);
                    string matterSiteURL = centralList.Substring(0, centralList.LastIndexOf(string.Concat(ConstantStrings.ForwardSlash, ConstantStrings.Lists, ConstantStrings.ForwardSlash), StringComparison.OrdinalIgnoreCase));
                    string matterListName = centralList.Substring(centralList.LastIndexOf(ConstantStrings.ForwardSlash, StringComparison.OrdinalIgnoreCase) + 1);
                    ClientContext listClientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(matterSiteURL), requestObject.RefreshToken);
                    ProvisionHelperFunctions.SaveMatterDetails(requestObject, client, matter, matterListName, matterConfigurations, listClientContext);
                }
                else
                {
                    matterURL = roleCheck;
                }
            }
            catch (Exception exception)
            {
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                matterURL = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return matterURL;
        }

        /// <summary>
        /// Function to delete the matter in case of failure.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>Result of operation: Matter Deleted or not</returns>
        internal static string DeleteMatter(RequestObject requestObject, Client client, Matter matter)
        {
            string result = ConstantStrings.FALSE;

            if (null != requestObject && null != client && null != matter)
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        if (null != clientContext && !string.IsNullOrWhiteSpace(matter.Name))
                        {
                            string stampResult = Lists.GetPropertyValueForList(clientContext, matter.Name, ServiceConstantStrings.StampedPropertySuccess);
                            if (0 != string.Compare(stampResult, ConstantStrings.TRUE, StringComparison.OrdinalIgnoreCase))
                            {
                                IList<string> lists = new List<string>();
                                lists.Add(matter.Name);
                                lists.Add(string.Concat(matter.Name, ServiceConstantStrings.CalendarNameSuffix));
                                lists.Add(string.Concat(matter.Name, ServiceConstantStrings.OneNoteLibrarySuffix));
                                lists.Add(string.Concat(matter.Name, ServiceConstantStrings.TaskNameSuffix));
                                bool bListDeleted = Lists.Delete(clientContext, lists);
                                if (bListDeleted)
                                {
                                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.DeleteMatterCode, TextConstants.MatterDeletedSuccessfully);
                                }
                                else
                                {
                                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.DeleteMatterCode, ServiceConstantStrings.MatterNotPresent);
                                }
                                Uri clientUri = new Uri(client.Url);
                                string matterLandingPageUrl = string.Concat(clientUri.AbsolutePath, ConstantStrings.ForwardSlash, ServiceConstantStrings.MatterLandingPageRepositoryName.Replace(ConstantStrings.Space, string.Empty), ConstantStrings.ForwardSlash, matter.MatterGuid, ConstantStrings.AspxExtension);
                                Page.Delete(clientContext, matterLandingPageUrl);
                            }
                            else
                            {
                                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.MatterLibraryExistsCode, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.ErrorDuplicateMatter, ConstantStrings.Matter) + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + ConstantStrings.MatterPrerequisiteCheck.LibraryExists);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            return result;
        }

        /// <summary>
        /// Creates an item in the specific list with the list of users to whom the matter will be shared.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object</param>
        /// <param name="matterDetails">Details of matter</param>
        /// <param name="matterLandingFlag">Flag to determine if Matter landing page exists</param>
        /// <param name="matterConfigurations">Object holding configuration for the matter</param>
        /// <returns>true if success else false</returns>
        internal static string ShareMatter(RequestObject requestObject, Client client, Matter matter, MatterDetails matterDetails, string matterLandingFlag, MatterConfigurations matterConfigurations)
        {
            string returnFlag = ConstantStrings.FALSE;

            if (null != requestObject && null != client && null != matter && null != matterDetails)
            {
                try
                {

                    Uri mailListURL = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", ConstantStrings.ProvisionMatterAppURL, ConstantStrings.ForwardSlash, ConstantStrings.Lists, ConstantStrings.ForwardSlash, ConstantStrings.SendMailListName));
                    string centralMailListURL = Convert.ToString(mailListURL, CultureInfo.InvariantCulture);
                    string mailSiteURL = centralMailListURL.Substring(0, centralMailListURL.LastIndexOf(string.Concat(ConstantStrings.ForwardSlash, ConstantStrings.Lists, ConstantStrings.ForwardSlash), StringComparison.OrdinalIgnoreCase));
                    ///// Retrieve the specific site where the Mail List is present along with the required List Name
                    if (null != mailListURL && null != client.Url)
                    {
                        if (!string.IsNullOrWhiteSpace(mailSiteURL))
                        {
                            returnFlag = ProvisionHelperFunctions.ShareMatterUtility(requestObject, client, matter, matterDetails, mailSiteURL, centralMailListURL, matterLandingFlag, matterConfigurations);
                        }
                    }
                }

                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            return returnFlag;
        }

        /// <summary>
        /// Function to share the matter.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object</param>
        /// <param name="matterDetails">Matter object containing Matter data details</param>
        /// <param name="mailSiteURL">URL of the site</param>
        /// <param name="centralMailListURL">URL of the Send Mail list</param>
        /// <param name="matterLandingFlag">Flag to determine if Matter landing page exists</param>
        /// <param name="matterConfigurations">Object holding configuration for the matter</param>
        /// <returns>Result of operation: Matter Shared successfully or not</returns>        
        internal static string ShareMatterUtility(RequestObject requestObject, Client client, Matter matter, MatterDetails matterDetails, string mailSiteURL, string centralMailListURL, string matterLandingFlag, MatterConfigurations matterConfigurations)
        {
            string shareFlag = ConstantStrings.FALSE;
            string mailListName = centralMailListURL.Substring(centralMailListURL.LastIndexOf(ConstantStrings.ForwardSlash, StringComparison.OrdinalIgnoreCase) + 1);
            string matterLocation = string.Concat(client.Url, ConstantStrings.ForwardSlash, matter.Name);
            string ProvisionMatterValidation = string.Empty;
            if (!string.IsNullOrWhiteSpace(mailSiteURL))
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(mailSiteURL), requestObject.RefreshToken))
                {

                    ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, null, int.Parse(ConstantStrings.ProvisionMatterShareMatter, CultureInfo.InvariantCulture), matterConfigurations);
                    if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                    {
                        // Get the current logged in User
                        clientContext.Load(clientContext.Web.CurrentUser);
                        clientContext.ExecuteQuery();
                        string matterMailBody, blockUserNames;
                        // Generate Mail Subject
                        string matterMailSubject = string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailSubject, matter.Id, matter.Name, clientContext.Web.CurrentUser.Title);

                        // Logic to Create Mail body
                        // Step 1: Create Matter Information
                        // Step 2: Create Team Information
                        // Step 3: Create Access Information
                        // Step 4: Create Conflict check Information based on the conflict check flag and create mail body

                        // Step 1: Create Matter Information
                        string defaultContentType = string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailDefaultContentTypeHtmlChunk, matter.DefaultContentType);
                        string matterType = string.Join(";", matter.ContentTypes.ToArray()).TrimEnd(';').Replace(matter.DefaultContentType, defaultContentType);

                        // Step 2: Create Team Information
                        string secureMatter = ConstantStrings.FALSE.ToUpperInvariant() == matter.Conflict.SecureMatter.ToUpperInvariant() ? ConstantStrings.NO : ConstantStrings.YES;
                        string mailBodyTeamInformation = string.Empty;
                        mailBodyTeamInformation = TeamMembersPermissionInformation(matterDetails, mailBodyTeamInformation);

                        // Step 3: Create Access Information
                        if (ConstantStrings.TRUE == matterLandingFlag)
                        {
                            matterLocation = string.Concat(client.Url, ConstantStrings.ForwardSlash, ServiceConstantStrings.MatterLandingPageRepositoryName.Replace(ConstantStrings.Space, string.Empty), ConstantStrings.ForwardSlash, matter.MatterGuid, ConstantStrings.AspxExtension);
                        }
                        string oneNotePath = string.Concat(client.Url, ConstantStrings.ForwardSlash, matter.MatterGuid, ServiceConstantStrings.OneNoteLibrarySuffix, ConstantStrings.ForwardSlash, matter.MatterGuid, ConstantStrings.ForwardSlash, matter.MatterGuid);

                        // Step 4: Create Conflict check Information based on the conflict check flag and create mail body
                        if (matterConfigurations.IsConflictCheck)
                        {
                            string conflictIdentified = ConstantStrings.FALSE.ToUpperInvariant() == matter.Conflict.Identified.ToUpperInvariant() ? ConstantStrings.NO : ConstantStrings.YES;
                            blockUserNames = string.Join(";", matter.BlockUserNames.ToArray()).Trim().TrimEnd(';');

                            blockUserNames = !String.IsNullOrEmpty(blockUserNames) ? string.Format(CultureInfo.InvariantCulture, "<div>{0}: {1}</div>", "Conflicted User", blockUserNames) : string.Empty;
                            matterMailBody = string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailBodyMatterInformation, client.Name, client.Id, matter.Name, matter.Id, matter.Description, matterType) + string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailBodyConflictCheck, ConstantStrings.YES, matter.Conflict.CheckBy, Convert.ToDateTime(matter.Conflict.CheckOn, CultureInfo.InvariantCulture).ToString(ServiceConstantStrings.MatterCenterDateFormat, CultureInfo.InvariantCulture), conflictIdentified) + string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailBodyTeamMembers, secureMatter, mailBodyTeamInformation, blockUserNames, client.Url, oneNotePath, matter.Name, matterLocation, matter.Name);
                        }
                        else
                        {
                            blockUserNames = string.Empty;
                            matterMailBody = string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailBodyMatterInformation, client.Name, client.Id, matter.Name, matter.Id, matter.Description, matterType) + string.Format(CultureInfo.InvariantCulture, TextConstants.MatterMailBodyTeamMembers, secureMatter, mailBodyTeamInformation, blockUserNames, client.Url, oneNotePath, matter.Name, matterLocation, matter.Name);
                        }

                        Web web = clientContext.Web;
                        List mailList = web.Lists.GetByTitle(mailListName);
                        List<FieldUserValue> userList = new List<FieldUserValue>();
                        List<FieldUserValue> userEmailList = ProvisionHelperFunctions.GenerateMailList(matter, clientContext, ref userList);
                        ///// Add the Matter URL in list
                        FieldUrlValue matterPath = new FieldUrlValue()
                        {
                            Url = string.Concat(client.Url.Replace(String.Concat(ConstantStrings.HTTPS, ConstantStrings.COLON, ConstantStrings.ForwardSlash, ConstantStrings.ForwardSlash), String.Concat(ConstantStrings.HTTP, ConstantStrings.COLON, ConstantStrings.ForwardSlash, ConstantStrings.ForwardSlash)), ConstantStrings.ForwardSlash, matter.Name, ConstantStrings.ForwardSlash, matter.Name),
                            Description = matter.Name
                        };
                        List<string> columnNames = new List<string>() { ServiceConstantStrings.ShareListColumnMatterPath, ServiceConstantStrings.ShareListColumnMailList, TextConstants.ShareListColumnMailBody, TextConstants.ShareListColumnMailSubject };
                        List<object> columnValues = new List<object>() { matterPath, userEmailList, matterMailBody, matterMailSubject };
                        // To avoid the invalid symbol error while parsing the JSON, return the response in lower case 
                        shareFlag = Convert.ToString(Lists.AddItem(clientContext, mailList, columnNames, columnValues), CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentUICulture);
                    }
                }
            }
            return shareFlag;
        }

        /// <summary>
        /// Provides the team members and their respective permission details.
        /// </summary>
        /// <param name="matterDetails">Matter Details object</param>
        /// <param name="mailBodyTeamInformation">Team members permission information</param>
        /// <returns>Team members permission information</returns>
        private static string TeamMembersPermissionInformation(MatterDetails matterDetails, string mailBodyTeamInformation)
        {
            if (null != matterDetails && !string.IsNullOrWhiteSpace(matterDetails.RoleInformation))
            {
                Dictionary<string, string> roleInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(matterDetails.RoleInformation);

                foreach (KeyValuePair<string, string> entry in roleInformation)
                {
                    mailBodyTeamInformation = string.Format(CultureInfo.InvariantCulture, ConstantStrings.RoleInfoHtmlChunk, entry.Key, entry.Value) + mailBodyTeamInformation;
                }
            }
            return mailBodyTeamInformation;
        }

        /// <summary>
        /// Function to create dictionary object for stamp property 
        /// </summary>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterDetails">Matter details object which has data of properties to be stamped</param>
        /// <returns>returns dictionary object</returns>
        internal static Dictionary<string, string> SetStampProperty(Client client, Matter matter, MatterDetails matterDetails)
        {
            string matterCenterPermission = string.Join(ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, matter.Permissions);
            string matterCenterRoles = string.Join(ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, matter.Roles);
            string documentTemplateCount = string.Join(ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, matter.DocumentTemplateCount);
            string matterCenterUsers = string.Empty;
            string matterCenterUserEmails = string.Empty;
            string separator = ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR;
            foreach (IList<string> userNames in matter.AssignUserNames)
            {
                matterCenterUsers += string.Join(ConstantStrings.Semicolon, userNames.Where(user => !string.IsNullOrWhiteSpace(user))) + separator;
            }
            // Removed $|$ from end of the string 
            matterCenterUsers = matterCenterUsers.Substring(0, matterCenterUsers.Length - separator.Length);
            foreach (IList<string> userEmails in matter.AssignUserEmails)
            {
                matterCenterUserEmails += string.Join(ConstantStrings.Semicolon, userEmails.Where(user => !string.IsNullOrWhiteSpace(user))) + separator;
            }
            // Removed $|$ from end of the string 
            matterCenterUserEmails = matterCenterUserEmails.Substring(0, matterCenterUserEmails.Length - separator.Length);
            List<string> keys = new List<string>();
            Dictionary<string, string> propertyList = new Dictionary<string, string>();
            keys.Add(ServiceConstantStrings.StampedPropertyPracticeGroup);
            keys.Add(ServiceConstantStrings.StampedPropertyAreaOfLaw);
            keys.Add(ServiceConstantStrings.StampedPropertySubAreaOfLaw);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterName);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterID);
            keys.Add(ServiceConstantStrings.StampedPropertyClientName);
            keys.Add(ServiceConstantStrings.StampedPropertyClientID);
            keys.Add(ServiceConstantStrings.StampedPropertyResponsibleAttorney);
            keys.Add(ServiceConstantStrings.StampedPropertyResponsibleAttorneyEmail);
            keys.Add(ServiceConstantStrings.StampedPropertyTeamMembers);
            keys.Add(ServiceConstantStrings.StampedPropertyIsMatter);
            keys.Add(ServiceConstantStrings.StampedPropertyOpenDate);
            keys.Add(ServiceConstantStrings.StampedPropertySecureMatter);
            keys.Add(ServiceConstantStrings.StampedPropertyBlockedUploadUsers);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterDescription);
            keys.Add(ServiceConstantStrings.StampedPropertyConflictCheckDate);
            keys.Add(ServiceConstantStrings.StampedPropertyConflictCheckBy);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterCenterRoles);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterCenterPermissions);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterCenterUsers);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterCenterUserEmails);
            keys.Add(ServiceConstantStrings.StampedPropertyDefaultContentType);
            keys.Add(ServiceConstantStrings.StampedPropertyIsConflictIdentified);
            keys.Add(ServiceConstantStrings.StampedPropertyDocumentTemplateCount);
            keys.Add(ServiceConstantStrings.StampedPropertyBlockedUsers);
            keys.Add(ServiceConstantStrings.StampedPropertyMatterGUID);

            propertyList.Add(ServiceConstantStrings.StampedPropertyPracticeGroup, Encoder.HtmlEncode(matterDetails.PracticeGroup));
            propertyList.Add(ServiceConstantStrings.StampedPropertyAreaOfLaw, Encoder.HtmlEncode(matterDetails.AreaOfLaw));
            propertyList.Add(ServiceConstantStrings.StampedPropertySubAreaOfLaw, Encoder.HtmlEncode(matterDetails.SubareaOfLaw));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterName, Encoder.HtmlEncode(matter.Name));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterID, Encoder.HtmlEncode(matter.Id));
            propertyList.Add(ServiceConstantStrings.StampedPropertyClientName, Encoder.HtmlEncode(client.Name));
            propertyList.Add(ServiceConstantStrings.StampedPropertyClientID, Encoder.HtmlEncode(client.Id));
            propertyList.Add(ServiceConstantStrings.StampedPropertyResponsibleAttorney, Encoder.HtmlEncode(matterDetails.ResponsibleAttorney));
            propertyList.Add(ServiceConstantStrings.StampedPropertyResponsibleAttorneyEmail, Encoder.HtmlEncode(matterDetails.ResponsibleAttorneyEmail));
            propertyList.Add(ServiceConstantStrings.StampedPropertyTeamMembers, Encoder.HtmlEncode(matterDetails.TeamMembers));
            propertyList.Add(ServiceConstantStrings.StampedPropertyIsMatter, ConstantStrings.TRUE);
            propertyList.Add(ServiceConstantStrings.StampedPropertyOpenDate, Encoder.HtmlEncode(DateTime.Now.ToString(ServiceConstantStrings.ValidDateFormat, CultureInfo.InvariantCulture)));
            propertyList.Add(ServiceConstantStrings.PropertyNameVtiIndexedPropertyKeys, Encoder.HtmlEncode(ServiceUtility.GetEncodedValueForSearchIndexProperty(keys)));
            propertyList.Add(ServiceConstantStrings.StampedPropertySecureMatter, (matter.Conflict != null) ? (matter.Conflict.SecureMatter != null) ? Encoder.HtmlEncode(matter.Conflict.SecureMatter) : "False" : "False");
            propertyList.Add(ServiceConstantStrings.StampedPropertyBlockedUploadUsers, Encoder.HtmlEncode(string.Join(";", matterDetails.UploadBlockedUsers)));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterDescription, Encoder.HtmlEncode(matter.Description));
            propertyList.Add(ServiceConstantStrings.StampedPropertyConflictCheckDate, (string.IsNullOrEmpty(matter.Conflict.CheckOn)) ? "" : Encoder.HtmlEncode(Convert.ToDateTime(matter.Conflict.CheckOn, CultureInfo.InvariantCulture).ToString(ServiceConstantStrings.ValidDateFormat, CultureInfo.InvariantCulture)));
            propertyList.Add(ServiceConstantStrings.StampedPropertyConflictCheckBy, Encoder.HtmlEncode(matter.Conflict.CheckBy));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterRoles, Encoder.HtmlEncode(matterCenterRoles));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterPermissions, Encoder.HtmlEncode(matterCenterPermission));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterUsers, Encoder.HtmlEncode(matterCenterUsers));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterUserEmails, Encoder.HtmlEncode(matterCenterUserEmails));
            propertyList.Add(ServiceConstantStrings.StampedPropertyDefaultContentType, Encoder.HtmlEncode(matter.DefaultContentType));
            propertyList.Add(ServiceConstantStrings.StampedPropertyIsConflictIdentified, Encoder.HtmlEncode(matter.Conflict.Identified));
            propertyList.Add(ServiceConstantStrings.StampedPropertyDocumentTemplateCount, Encoder.HtmlEncode(documentTemplateCount));
            propertyList.Add(ServiceConstantStrings.StampedPropertyBlockedUsers, Encoder.HtmlEncode(string.Join(";", matter.BlockUserNames)));
            propertyList.Add(ServiceConstantStrings.StampedPropertyMatterGUID, Encoder.HtmlEncode(matter.MatterGuid));
            propertyList.Add(ServiceConstantStrings.StampedPropertySuccess, ConstantStrings.TRUE);
            return propertyList;
        }

        /// <summary>
        /// Checks whether to retain previous users while breaking permission
        /// </summary>
        /// <param name="conflictIdentified">Conflict identified information</param>
        /// <param name="matterSecured">Security information</param>
        /// <returns>Flag to indicate whether to retain the previous users</returns>
        internal static bool CopyRoleAssignment(string conflictIdentified, string matterSecured)
        {
            bool isBreakPermission = true;
            if (Convert.ToBoolean(conflictIdentified, CultureInfo.InvariantCulture) || Convert.ToBoolean(matterSecured, CultureInfo.InvariantCulture))
            {
                isBreakPermission = false;
            }
            return isBreakPermission;
        }

        /// <summary>
        /// Checks if the lists exist
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="matterName">List name</param>
        /// <returns></returns>
        internal static List<string> CheckListsExist(ClientContext clientContext, string matterName, MatterConfigurations matterConfigurations = null)
        {
            List<string> lists = new List<string>();
            lists.Add(matterName);
            lists.Add(matterName + ServiceConstantStrings.OneNoteLibrarySuffix);
            if (null == matterConfigurations || matterConfigurations.IsCalendarSelected)
            {
                lists.Add(matterName + ServiceConstantStrings.CalendarNameSuffix);
            }
            if (null == matterConfigurations || matterConfigurations.IsTaskSelected)
            {
                lists.Add(matterName + ServiceConstantStrings.TaskNameSuffix);
            }
            List<string> listExists = Lists.Exists(clientContext, new ReadOnlyCollection<string>(lists));
            return listExists;
        }

    }
}