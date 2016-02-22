// ***********************************************************************
// <copyright file="MatterProvisionHelperUtility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 04-27-2015
//
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    #endregion

    /// <summary>
    /// This class provides meta data related information for matter provision.
    /// </summary>
    internal static class MatterProvisionHelperUtility
    {
        /// <summary>
        /// Breaks permission for user
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="matterName">Matter name</param>
        /// <param name="copyPermissionsFromParent">Copy permissions from parent</param>
        /// <param name="calendarName">Calendar name</param>
        /// <returns>Boolean value</returns>
        internal static bool BreakPermission(ClientContext clientContext, string matterName, bool copyPermissionsFromParent, string calendarName = null)
        {
            bool flag = false;
            try
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                string matterOrCalendar = !string.IsNullOrWhiteSpace(calendarName) ? calendarName : matterName;
                List list = web.Lists.GetByTitle(matterOrCalendar);
                clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();
                if (!list.HasUniqueRoleAssignments)
                {
                    list.BreakRoleInheritance(copyPermissionsFromParent, true);
                    list.Update();
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + "Matter name: " + exception.Message + matterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
            return flag;
        }

        /// <summary>
        /// Gets content type data
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="contentTypeNames">Content type names</param>
        /// <returns>IList object</returns>
        internal static IList<ContentType> GetContentTypeData(ClientContext clientContext, IList<string> contentTypeNames)
        {
            ContentTypeCollection contentTypeCollection = null;
            IList<ContentType> selectedContentTypeCollection = null;
            if (null != clientContext && null != contentTypeNames)
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                contentTypeCollection = web.ContentTypes;
                clientContext.Load(contentTypeCollection);
                clientContext.ExecuteQuery();
                selectedContentTypeCollection = new List<ContentType>();
                foreach (string contentTypeName in contentTypeNames)
                {
                    foreach (ContentType contentType in contentTypeCollection)
                    {
                        if (string.Equals(contentTypeName, contentType.Name))
                        {
                            selectedContentTypeCollection.Add(contentType);
                        }
                    }
                }
            }
            return selectedContentTypeCollection;
        }

        /// <summary>
        /// Sets the default content type
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="list">List object</param>
        /// <param name="defaultContentType">Default content type</param>
        internal static void SetDefaultContentType(ClientContext clientContext, List list, string defaultContentType)
        {
            ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
            clientContext.Load(currentContentTypeOrder);
            clientContext.ExecuteQuery();

            IList<ContentTypeId> updatedContentTypeOrder = new List<ContentTypeId>();
            int contentCount = 0, contentSwap = 0;
            foreach (ContentType contentType in currentContentTypeOrder)
            {
                if (contentType.Name.Equals(defaultContentType))
                {
                    contentSwap = contentCount;
                }
                if (!contentType.Name.Equals("Folder"))
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

        /// <summary>
        /// Retrieves the list of content types that are to be associated with the matter
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
        /// Assigns permissions for user
        /// </summary>
        /// <param name="clientcontext">Client context</param>
        /// <param name="matterName">Matter name</param>
        /// <param name="users">Users object</param>
        /// <param name="permission">Permission object</param>
        /// <param name="calendarName">Calendar name</param>
        /// <returns>String value</returns>
        internal static string AssignUserPermissions(ClientContext clientcontext, string matterName, List<string> users, string permission, string calendarName = null)
        {
            {
                string returnvalue = "false";
                try
                {
                    List<string> permissions = new List<string>();
                    permissions.Add(permission);
                    Microsoft.SharePoint.Client.Web web = clientcontext.Web;
                    clientcontext.Load(web.RoleDefinitions);
                    string matterOrCalendar = !string.IsNullOrWhiteSpace(calendarName) ? calendarName : matterName;
                    List list = web.Lists.GetByTitle(matterOrCalendar);
                    clientcontext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientcontext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments)
                    {
                        if (null != permissions && null != users) //matter.permissions=read/limited access/contribute/ full control/ view only
                        {
                            int position = 0;
                            foreach (string rolename in permissions)
                            {
                                UpdateUserPermission(clientcontext, matterName, users, list, rolename);
                                position++;
                            }
                        }
                        // success. return a success code
                        returnvalue = "true";
                    }
                }
                // web exception
                catch (Exception exception)
                {
                    Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "assigning Permission"));
                    Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "Matter name: " + matterName + "\nStacktrace: " + exception.StackTrace);
                    throw;
                }
                return returnvalue;
            }
        }

        /// <summary>
        /// Updates user permission
        /// </summary>
        /// <param name="clientcontext">Client context</param>
        /// <param name="matterName">Matter name</param>
        /// <param name="users">Users object</param>
        /// <param name="list">List object</param>
        /// <param name="rolename">Role name</param>
        private static void UpdateUserPermission(ClientContext clientcontext, string matterName, List<string> users, List list, string rolename)
        {
            try
            {
                RoleDefinitionCollection roleDefinitions = clientcontext.Web.RoleDefinitions;
                RoleDefinition role = (from roleDef in roleDefinitions
                                       where roleDef.Name == rolename
                                       select roleDef).First();

                foreach (string user in users)
                {
                    //get the user object
                    Principal userprincipal = clientcontext.Web.EnsureUser(user);
                    //create the role definition binding collection
                    RoleDefinitionBindingCollection roledefinitionbindingcollection = new RoleDefinitionBindingCollection(clientcontext);
                    //add the role definition to the collection
                    roledefinitionbindingcollection.Add(role);
                    //create a role assignment with the user and role definition
                    list.RoleAssignments.Add(userprincipal, roledefinitionbindingcollection);
                }
                //execute the query to add everything
                clientcontext.ExecuteQuery();
            }
            // SharePoint specific exception
            catch (ClientRequestException clientRequestException)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + clientRequestException.Message + "Matter name: " + matterName + "\nStacktrace: " + clientRequestException.StackTrace);
                throw;
            }
            // SharePoint specific exception
            catch (ServerException serverException)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + serverException.Message + "Matter name: " + matterName + "\nStacktrace: " + serverException.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get a client context
        /// </summary>
        /// <param name="siteURl">Site Url</param>
        /// <param name="configVal">Configuration values from excel</param>
        /// <returns>client context</returns>
        internal static ClientContext GetClientContext(string siteURl, Dictionary<string, string> configVal)
        {
            string login = configVal["Username"];
            string password = configVal["Password"];
            bool isDeployedOnAzure = Convert.ToBoolean(configVal["IsDeployedOnAzure"], CultureInfo.InvariantCulture);
            ClientContext clientContext = null;
            try
            {
                using (clientContext = new ClientContext(siteURl))
                {
                    using (var securePassword = new SecureString())
                    {
                        foreach (char c in password)
                        {
                            securePassword.AppendChar(c);
                        }
                        object onlineCredentials;
                        if (isDeployedOnAzure)
                        {
                            onlineCredentials = new SharePointOnlineCredentials(login, securePassword);
                            clientContext.Credentials = (SharePointOnlineCredentials)onlineCredentials; // Secure the crdentials and generate the SharePoint Online Credentials                    
                        }
                        else
                        {
                            onlineCredentials = new NetworkCredential(login, securePassword);
                            clientContext.Credentials = (NetworkCredential)onlineCredentials; // Assign On Premise credentials to the Client Context
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return clientContext;
        }

        /// <summary>
        /// Resolve user names
        /// </summary>
        /// <param name="configVal">Dictionary object</param>
        /// <param name="tempUserNames">Temporary user names</param>
        /// <returns>IList object</returns>
        internal static IList<FieldUserValue> ResolveUserNames(Dictionary<string, string> configVal, string tempUserNames)
        {
            IList<string> userNames = new List<string>();
            if (!string.IsNullOrEmpty(tempUserNames) && string.Empty != tempUserNames.Split(';')[0].Trim())
            {
                foreach (string temp in tempUserNames.Split(';'))
                {
                    userNames.Add(temp);
                }

                List<FieldUserValue> userList = new List<FieldUserValue>();
                if (null != userNames)
                {
                    foreach (string userName in userNames)
                    {
                        if (!string.IsNullOrWhiteSpace(userName))
                        {
                            using (ClientContext clientContext = GetClientContext(configVal["CatalogSiteURL"], configVal))
                            {
                                User user = clientContext.Web.EnsureUser(userName.Trim());
                                // Only Fetch the User ID which is required
                                clientContext.Load(user); //, u => u.Id);
                                clientContext.ExecuteQuery();
                                // Add the user to the first element of the FieldUserValue array.
                                FieldUserValue tempUser = new FieldUserValue();
                                tempUser.LookupId = user.Id;
                                userList.Add(tempUser);
                            }
                        }
                    }
                }
                return userList;
            }
            return null;
        }

        /// <summary>
        /// Gets the encoded value for the search index property
        /// </summary>
        /// <param name="keys">List object</param>
        /// <returns>String value</returns>
        internal static string GetEncodedValueForSearchIndexProperty(List<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (keys != null)
            {
                foreach (string current in keys)
                {
                    stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                    stringBuilder.Append('|');
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Function to get the WssID for the Practice group, Area of law and Subarea of law terms
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <param name="fields">Field Collection object</param>
        /// <returns>An Object containing metadata for Matter</returns>
        internal static MatterMetadata GetWSSId(ClientContext clientContext, MatterMetadata matterMetadata, FieldCollection fields)
        {
            ClientResult<TaxonomyFieldValue> practiceGroupResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.PracticeGroupTerm.Id);
            ClientResult<TaxonomyFieldValue> areaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.AreaTerm.Id);
            ClientResult<TaxonomyFieldValue> subareaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.SubareaTerm.Id);
            clientContext.ExecuteQuery();

            matterMetadata.PracticeGroupTerm.WssId = practiceGroupResult.Value.WssId;
            matterMetadata.AreaTerm.WssId = areaOfLawResult.Value.WssId;
            matterMetadata.SubareaTerm.WssId = subareaOfLawResult.Value.WssId;
            return matterMetadata;
        }

        /// <summary>
        /// Assigns content type
        /// </summary>
        /// <param name="clientcontext">SP client context</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        internal static void AssignContentType(ClientContext clientcontext, MatterMetadata matterMetadata)
        {
            // For each value in the list of Content Type Names
            // Add that content Type to the Library
            string defaultContentType = matterMetadata.ContentTypes[0];
            try
            {
                // Returns the selected Content types from the Site Content Types
                IList<ContentType> contentTypeCollection = GetContentTypeData(clientcontext, matterMetadata.ContentTypes);
                if (null != contentTypeCollection)
                {
                    Microsoft.SharePoint.Client.Web web = clientcontext.Web;
                    List matterList = web.Lists.GetByTitle(matterMetadata.Matter.MatterName);
                    FieldCollection fields = GetContentType(clientcontext, contentTypeCollection, matterList);
                    matterMetadata = GetWSSId(clientcontext, matterMetadata, fields);
                    MatterProvisionHelper.SetFieldValues(matterMetadata, fields);
                    clientcontext.ExecuteQuery();
                    SetDefaultContentType(clientcontext, matterList, defaultContentType);
                    MatterProvisionHelper.CreateView(clientcontext, matterList);
                }
            }
            // SharePoint Specific Exception
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "assigning ContentType"));
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "Matter name: " + matterMetadata.Matter.MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
        }
    }
}
