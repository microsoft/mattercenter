// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-prd
// Created          : 03-06-2015
//
// ***********************************************************************
// <copyright file="MatterProvision.svc.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines service for Matter Provision App.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.ProviderService.CommonHelper;
    using Microsoft.Legal.MatterCenter.ProviderService.HelperClasses;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.WebParts;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    #endregion

    /// <summary>
    /// Provides the operation contracts used for provisioning a matter.
    /// </summary>
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MatterProvision
    {
        #region Operation Contracts
        /// <summary>
        /// Gets the hierarchy of terms along with the specific custom properties of each term from term store.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Serialized string of Term store JSON object</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetTaxonomyData(RequestObject requestObject, Client client, TermStoreDetails details)
        {
            string returnValue = ConstantStrings.FALSE;
            try
            {
                if (null != requestObject && null != client & null != details && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, null, null, null, int.Parse(ConstantStrings.ProvisionMatterCommonValidation, CultureInfo.InvariantCulture), null);
                    if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                    {
                        using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                        {
                            string cacheValue = string.Empty, key = string.Empty;
                            if (details.TermSetName == ServiceConstantStrings.PracticeGroupTermSetName)
                            {
                                key = ServiceConstantStrings.CACHE_MATTER_TYPE;
                            }
                            else if (details.TermSetName == ServiceConstantStrings.ClientTermSetName)
                            {
                                key = ServiceConstantStrings.CACHE_CLIENTS;
                            }
                            cacheValue = ServiceUtility.GetOrSetCachedValue(key);
                            if (cacheValue.Equals(ConstantStrings.FALSE))
                            {
                                returnValue = TermStoreHelperFunctions.GetTaxonomyHierarchy(clientContext, details);
                                //// Check if error has returned while processing data                                
                                if (!ServiceUtility.CheckValueHasErrors(returnValue))
                                {
                                    ServiceUtility.GetOrSetCachedValue(key, returnValue);
                                }
                            }
                            else
                            {
                                returnValue = cacheValue;
                            }
                        }
                    }
                    else
                    {
                        returnValue = ProvisionMatterValidation;
                    }
                }
                else
                {
                    returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
                }
            }
            catch (Exception exception)
            {
                returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return returnValue;
        }

        /// <summary>
        /// Get roles from the SharePoint list.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <returns>Serialized string of Roles JSON object</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetRoleData(RequestObject requestObject, Client client)
        {
            string returnValue = ConstantStrings.FALSE;
            if (null != requestObject && null != client && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, null, null, null, int.Parse(ConstantStrings.ProvisionMatterCommonValidation, CultureInfo.InvariantCulture), null);
                if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                {
                    try
                    {
                        // SharePoint Helper Method to retrieve list items
                        using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                        {
                            returnValue = ServiceUtility.GetOrSetCachedValue(ServiceConstantStrings.CACHE_ROLES);
                            if (returnValue.Equals(ConstantStrings.FALSE))
                            {
                                IList<Role> roles = new List<Role>();
                                ListItemCollection collListItem = Lists.GetData(clientContext, ServiceConstantStrings.DMSRoleListName, ServiceConstantStrings.DMSRoleQuery);
                                ///// Check if NULL and greater than 0
                                roles = ProvisionHelperFunctions.GetRoleDataUtility(roles, collListItem);
                                returnValue = JsonConvert.SerializeObject(roles);
                                //// Check if return value has errors
                                if (!ServiceUtility.CheckValueHasErrors(returnValue))
                                {
                                    //// Cache the value if there is no error in return value
                                    ServiceUtility.GetOrSetCachedValue(ServiceConstantStrings.CACHE_ROLES, returnValue);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    }
                }
                else
                {
                    returnValue = ProvisionMatterValidation;
                }
            }
            else
            {
                returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return returnValue;
        }

        /// <summary>
        /// Gets different permission levels to be used on the site.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <returns>Serialized string of Permissions JSON object</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetPermissionLevels(RequestObject requestObject, Client client)
        {
            string returnValue = ConstantStrings.FALSE;
            if (null != requestObject && null != client && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, null, null, null, int.Parse(ConstantStrings.ProvisionMatterCommonValidation, CultureInfo.InvariantCulture), null);
                if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                {
                    returnValue = ProvisionHelperFunctions.GetPermissionLevelUtility(requestObject, client, returnValue);
                }
                else
                {
                    returnValue = ProvisionMatterValidation;
                }
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return returnValue;
        }

        /// <summary>
        /// Gets users from the site based on the search term.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="searchObject">Search object containing search term</param>
        /// <returns>Serialized string of Users JSON object</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetUsers(RequestObject requestObject, Client client, SearchObject searchObject)
        {
            string returnValue = ConstantStrings.FALSE;
            if (null != requestObject && null != client && null != searchObject && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, null, null, null, int.Parse(ConstantStrings.ProvisionMatterCommonValidation, CultureInfo.InvariantCulture), null);
                if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                {
                    try
                    {
                        searchObject.SearchTerm = (!string.IsNullOrWhiteSpace(searchObject.SearchTerm)) ? searchObject.SearchTerm : string.Empty;
                        using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                        {
                            IList<PeoplePickerUser> foundUsers = SharePointHelper.SearchUsers(clientContext, searchObject.SearchTerm);
                            IList<Users> users = new List<Users>();
                            if (null != foundUsers && 0 != foundUsers.Count)
                            {
                                users = ProvisionHelperFunctions.FilterUsers(users, foundUsers);
                            }
                            else
                            {
                                Users noResult = new Users()
                                {
                                    Name = TextConstants.PeoplePickerNoResults,
                                    LogOnName = string.Empty,
                                    Email = string.Empty,
                                    EntityType = string.Empty,
                                    ProviderName = string.Empty,
                                    EntityData = new EntityData()
                                    {
                                        Department = string.Empty,
                                        Email = string.Empty,
                                        Title = string.Empty
                                    }
                                };
                                users.Add(noResult);
                            }
                            returnValue = JsonConvert.SerializeObject(users);
                        }
                    }
                    catch (Exception exception)
                    {
                        ///// SharePoint Specific Exception
                        returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    }
                }
                else
                {
                    returnValue = ProvisionMatterValidation;
                }
            }
            else
            {
                returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return returnValue;
        }

        /// <summary>
        /// Checks if a matter with a specific name exists already for a particular client.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>true if success else false</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string CheckMatterExists(RequestObject requestObject, Client client, Matter matter, bool hasErrorOccurred, MatterConfigurations matterConfigurations = null)
        {
            string returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.DeleteMatterCode, ConstantStrings.TRUE);
            if (null != requestObject && null != client && null != matter && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, null, matter, null, int.Parse(ConstantStrings.ProvisionMatterCheckMatterExists, CultureInfo.InvariantCulture), null);
                if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                {
                    try
                    {
                        if (!hasErrorOccurred)
                        {
                            using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                            {
                                List<string> listExists = ProvisionHelperFunctions.CheckListsExist(clientContext, matter.Name, matterConfigurations);
                                if (0 < listExists.Count)
                                {
                                    string listName = !string.Equals(matter.Name, listExists[0]) ? listExists[0].Contains(ConstantStrings.Underscore) ? listExists[0].Split(ConstantStrings.Underscore[0]).Last() : ConstantStrings.Matter : ConstantStrings.Matter;
                                    returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.MatterLibraryExistsCode, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.ErrorDuplicateMatter, listName) + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + ConstantStrings.MatterPrerequisiteCheck.LibraryExists);
                                }
                                else
                                {
                                    Uri clientUri = new Uri(client.Url);
                                    string requestedUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", clientUri.AbsolutePath, ConstantStrings.ForwardSlash, ServiceConstantStrings.MatterLandingPageRepositoryName.Replace(ConstantStrings.Space, string.Empty), ConstantStrings.ForwardSlash, matter.Name, ConstantStrings.AspxExtension);
                                    if (ConstantStrings.TRUE == SearchHelperFunctions.PageExists(requestedUrl, clientContext))
                                    {
                                        returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.MatterLandingExistsCode, ServiceConstantStrings.ErrorDuplicateMatterLandingPage + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + ConstantStrings.MatterPrerequisiteCheck.MatterLandingPageExists);  // Return when matter landing page is present
                                    }
                                }
                            }
                        }
                        else
                        {
                            returnValue = ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                        }
                    }
                    catch (Exception exception)
                    {
                        returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    }
                }
                else
                {
                    returnValue = ProvisionMatterValidation;
                }
            }
            else
            {
                returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return returnValue;
        }

        /// <summary>
        /// Creates a matter (document library) using specified matter details.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterConfigurations">Matter Configurations object</param>
        /// <param name="userId">CSS Ids of the assigned users</param>
        /// <returns>JSON object with matter URL and true or false flag</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string CreateMatter(RequestObject requestObject, Client client, Matter matter, MatterConfigurations matterConfigurations, IList<string> userId, bool isErrorOccurred)
        {
            try
            {
                string result = string.Empty;
                if (null != requestObject && null != client && null != matter && (null != requestObject.RefreshToken || null != requestObject.SPAppToken || null != client.Url) && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        if (Lists.CheckPermissionOnList(ServiceUtility.GetClientContext(null, new Uri(client.Url), requestObject.RefreshToken), ServiceConstantStrings.MatterConfigurationsList, PermissionKind.EditListItems))
                        {
                            string matterURL = ConstantStrings.FALSE;
                            string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, null, int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture), matterConfigurations);
                            if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                            {
                                string matterValidation = CheckMatterExists(requestObject, client, matter, isErrorOccurred, matterConfigurations);
                                if (matterValidation.ToUpperInvariant().Contains(ConstantStrings.TRUE.ToUpperInvariant()))
                                {
                                    if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                                    {
                                        if (Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture))
                                        {
                                            matterURL = EditMatterHelperFunctions.CheckSecurityGroupInTeamMembers(clientContext, matter, userId);
                                            if (string.Equals(matterURL, ConstantStrings.FALSE, StringComparison.OrdinalIgnoreCase))
                                            {
                                                matterURL = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictIdentifiedCode, TextConstants.IncorrectInputConflictIdentifiedMessage);
                                            }
                                        }
                                        else
                                        {
                                            matterURL = ConstantStrings.TRUE;
                                        }
                                        if (string.Equals(matterURL, ConstantStrings.TRUE, StringComparison.OrdinalIgnoreCase))
                                        {
                                            matterURL = ProvisionHelperFunctions.CreateMatterUtility(requestObject, client, matter, clientContext, matterURL, matterConfigurations);
                                        }
                                    }
                                }
                                else
                                {
                                    matterURL = matterValidation;
                                }
                                result = matterURL;
                            }
                            else
                            {
                                result = ProvisionMatterValidation;
                            }
                        }
                        else
                        {
                            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.UserNotSiteOwnerCode, ServiceConstantStrings.UserNotSiteOwnerMessage);
                        }
                    }
                }
                else
                {
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
                }
                return result;
            }
            catch (Exception exception)
            {
                return Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Assigns specified permission to the list of users on specified matter.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterConfigurations">Matter Configurations object</param>
        /// <returns>true if success else false</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string AssignUserPermissions(RequestObject requestObject, Client client, Matter matter, MatterConfigurations matterConfigurations)
        {
            string returnValue = ConstantStrings.FALSE;
            if (null != requestObject && null != client && null != matter && null != client.Url && null != matterConfigurations && (null != requestObject.RefreshToken || null != requestObject.SPAppToken) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    string calendarName = string.Concat(matter.Name, ServiceConstantStrings.CalendarNameSuffix);
                    string oneNoteLibraryName = string.Concat(matter.Name, ServiceConstantStrings.OneNoteLibrarySuffix);
                    string taskLibraryName = string.Concat(matter.Name, ServiceConstantStrings.TaskNameSuffix);
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {

                        string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, null, int.Parse(ConstantStrings.ProvisionMatterAssignUserPermissions, CultureInfo.InvariantCulture), null);
                        if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                        {

                            if (!string.IsNullOrWhiteSpace(matter.Name))
                            {
                                //Assign permission for Matter library
                                returnValue = Convert.ToString(Lists.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, matter.Name), CultureInfo.CurrentCulture);

                                //Assign permission for OneNote library 
                                Lists.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, oneNoteLibraryName);

                                // Assign permission to calendar list if it is selected
                                if (ServiceConstantStrings.IsCreateCalendarEnabled && matterConfigurations.IsCalendarSelected)
                                {
                                    string returnValueCalendar = Convert.ToString(Lists.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, calendarName), CultureInfo.CurrentCulture);
                                    if (!Convert.ToBoolean(returnValueCalendar, CultureInfo.InvariantCulture))
                                    {
                                        MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeCalendarCreation, TextConstants.ErrorMessageCalendarCreation);
                                        throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                                    }
                                }

                                // Assign permission to task list if it is selected
                                if (matterConfigurations.IsTaskSelected)
                                {
                                    string returnValueTask = Convert.ToString(Lists.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, taskLibraryName), CultureInfo.CurrentCulture);
                                    if (!Convert.ToBoolean(returnValueTask, CultureInfo.InvariantCulture))
                                    {
                                        MatterCenterException customException = new MatterCenterException(TextConstants.ErrorMessageTaskCreation, TextConstants.ErrorCodeAddTaskList);
                                        throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                                    }
                                }
                            }
                        }
                        else
                        {
                            returnValue = ProvisionMatterValidation;
                            ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ///// Web Exception
                    ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                    returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            // To avoid the invalid symbol error while parsing the JSON, return the response in lower case 
            return returnValue.ToLower(CultureInfo.CurrentUICulture);
        }

        /// <summary>     
        /// Creates matter landing page on matter creation.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterConfigurations">Matter Configurations object</param>
        /// <returns>true if success else false</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string CreateMatterLandingPage(RequestObject requestObject, Client client, Matter matter, MatterConfigurations matterConfigurations)
        {
            int matterLandingPageId;
            string response = string.Empty;
            string result = string.Empty;
            if (null != requestObject && null != client && null != matter && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, null, int.Parse(ConstantStrings.ProvisionMatterMatterLandingPage, CultureInfo.InvariantCulture), matterConfigurations);
                        if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                        {
                            Uri uri = new Uri(client.Url);
                            Web web = clientContext.Web;

                            //// Create Matter Landing Web Part Page
                            string pageName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", matter.MatterGuid, ConstantStrings.AspxExtension);
                            matterLandingPageId = Page.CreateWebPartPage(clientContext, pageName, ConstantStrings.DefaultLayout, ConstantStrings.MasterPageGallery, ServiceConstantStrings.MatterLandingPageRepositoryName, matter.Name);
                            if (0 <= matterLandingPageId)
                            {
                                bool isCopyRoleAssignment = ProvisionHelperFunctions.CopyRoleAssignment(matter.Conflict.Identified, matter.Conflict.SecureMatter);
                                Lists.BreakItemPermission(clientContext, ServiceConstantStrings.MatterLandingPageRepositoryName, matterLandingPageId, isCopyRoleAssignment);
                                Lists.SetItemPermission(clientContext, matter.AssignUserEmails, ServiceConstantStrings.MatterLandingPageRepositoryName, matterLandingPageId, matter.Permissions);
                                //// Configure All Web Parts
                                string[] webParts = MatterLandingHelperFunction.ConfigureXMLCodeOfWebParts(requestObject, client, matter, clientContext, pageName, uri, web, matterConfigurations);
                                Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, ConstantStrings.ForwardSlash, ServiceConstantStrings.MatterLandingPageRepositoryName.Replace(ConstantStrings.Space, string.Empty), ConstantStrings.ForwardSlash, pageName));
                                clientContext.Load(file);
                                clientContext.ExecuteQuery();
                                LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                                WebPartDefinition webPartDefinition = null;
                                string[] zones = { ConstantStrings.HeaderZone, ConstantStrings.TopZone, ConstantStrings.RightZone, ConstantStrings.TopZone, ConstantStrings.RightZone, ConstantStrings.RightZone, ConstantStrings.FooterZone, ConstantStrings.RightZone, ConstantStrings.RightZone };
                                Page.AddWebPart(clientContext, limitedWebPartManager, webPartDefinition, webParts, zones);
                                response = ConstantStrings.TRUE;
                            }
                            else
                            {
                                MatterCenterException customException = new MatterCenterException(ServiceConstantStrings.ErrorCodeMatterLandingPageExists, ServiceConstantStrings.ErrorCodeMatterLandingPageExists);
                                throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                            }
                        }
                        else
                        {
                            response = ProvisionMatterValidation;
                            ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                        }
                    }

                    result = response;
                }
                catch (Exception exception)
                {
                    ////Generic Exception
                    ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return result;
        }

        /// <summary>
        /// Assigns specified content types to the specified matter (document library).
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <returns>true if success else false</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string AssignContentType(RequestObject requestObject, MatterMetadata matterMetadata)
        {
            string returnValue = ConstantStrings.FALSE;
            if (null != requestObject && null != matterMetadata && null != matterMetadata.Client && null != matterMetadata.Matter && (null != requestObject.RefreshToken || null != requestObject.SPAppToken) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                // For each value in the list of Content Type Names
                // Add that content Type to the Library
                Matter matter = matterMetadata.Matter;
                Client client = matterMetadata.Client;
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        string ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, null, int.Parse(ConstantStrings.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture), null);
                        if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                        {
                            // Returns the selected Content types from the Site Content Types
                            IList<ContentType> contentTypeCollection = SharePointHelper.GetContentTypeData(clientContext, matter.ContentTypes, requestObject, client, matter);

                            if (null != contentTypeCollection && matter.ContentTypes.Count == contentTypeCollection.Count && !string.IsNullOrWhiteSpace(matter.Name))
                            {
                                returnValue = ProvisionHelperFunctions.AssignContentTypeHelper(requestObject, matterMetadata, clientContext, contentTypeCollection, client, matter);
                            }
                            else
                            {
                                MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeContentTypes, TextConstants.ErrorMessageContentTypes);
                                throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                            }
                        }
                        else
                        {
                            returnValue = ProvisionMatterValidation;
                            ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ///// SharePoint Specific Exception
                    ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                    returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return returnValue;
        }

        /// <summary>
        /// Stamps properties to the created matter.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object</param>
        /// <param name="matterDetails">Matter details object which has data of properties to be stamped</param>
        /// <param name="matterProvisionChecks">Matter provision flag object which hold boolean values</param>
        /// <param name="matterConfigurations">Object Holding configuration for the matter</param>
        /// <returns>true if success else false</returns>               
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string UpdateMetadataForList(RequestObject requestObject, Client client, Matter matter, MatterDetails matterDetails, MatterProvisionFlags matterProvisionChecks, MatterConfigurations matterConfigurations)
        {
            string result = ConstantStrings.FALSE;
            string properties = ConstantStrings.FALSE;
            string ProvisionMatterValidation = string.Empty;
            if (null != requestObject && null != client && null != matter && null != matterDetails && (null != requestObject.RefreshToken || null != requestObject.SPAppToken) && (null != matterProvisionChecks) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                {
                    string shareMatterFlag = string.Empty;
                    ProvisionMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, matterDetails, int.Parse(ConstantStrings.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture), matterConfigurations);
                    if (string.IsNullOrWhiteSpace(ProvisionMatterValidation))
                    {
                        try
                        {
                            var props = clientContext.Web.Lists.GetByTitle(matter.Name).RootFolder.Properties;
                            Dictionary<string, string> propertyList = new Dictionary<string, string>();
                            propertyList = ProvisionHelperFunctions.SetStampProperty(client, matter, matterDetails);
                            clientContext.Load(props);
                            clientContext.ExecuteQuery();
                            Lists.SetPropertBagValuesForList(clientContext, props, matter.Name, propertyList);
                            if (matterProvisionChecks.SendEmailFlag)
                            {
                                shareMatterFlag = ProvisionHelperFunctions.ShareMatter(requestObject, client, matter, matterDetails, matterProvisionChecks.MatterLandingFlag, matterConfigurations);
                            }
                            else
                            {
                                shareMatterFlag = ConstantStrings.TRUE;
                            }
                            properties = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, shareMatterFlag);
                        }

                        catch (Exception exception)
                        {
                            properties = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                            ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                        }
                    }
                    else
                    {
                        ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                        properties = ProvisionMatterValidation;
                    }
                    result = properties;
                }
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the stamped properties from matter library property bag.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object</param>
        /// <returns>Matter stamped properties</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string RetrieveMatterStampedProperties(RequestObject requestObject, Client client, Matter matter)
        {
            string result = string.Empty;
            if (null != requestObject && null != client && null != matter && !string.IsNullOrWhiteSpace(matter.Name) && (null != requestObject.RefreshToken || null != requestObject.SPAppToken) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                PropertyValues matterStampedProperties = null;
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        // Get all the stamped properties from matter library
                        matterStampedProperties = EditMatterHelperFunctions.FetchMatterStampedProperties(clientContext, matter.Name);

                        Dictionary<string, object> stampedPropertyValues = matterStampedProperties.FieldValues;
                        if (0 < stampedPropertyValues.Count)
                        {
                            string matterCenterUsers = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterCenterUsers);
                            string matterCenterUserEmails = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterCenterUserEmails);
                            List<List<string>> matterCenterUserCollection = new List<List<string>>();
                            List<List<string>> matterCenterUserEmailsCollection = new List<List<string>>();
                            if (!string.IsNullOrWhiteSpace(matterCenterUsers))
                            {
                                matterCenterUserCollection = EditMatterHelperFunctions.GetMatterAssignedUsers(matterCenterUsers);
                            }
                            if (!string.IsNullOrWhiteSpace(matterCenterUserEmails))
                            {
                                matterCenterUserEmailsCollection = EditMatterHelperFunctions.GetMatterAssignedUsers(matterCenterUserEmails);
                            }
                            MatterStampedDetails matterStampedDetails = new MatterStampedDetails()
                            {
                                IsNewMatter = stampedPropertyValues.ContainsKey(ServiceConstantStrings.StampedPropertyIsConflictIdentified) ? ConstantStrings.TRUE : ConstantStrings.FALSE,
                                MatterObject = new Matter()
                                {
                                    Id = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterID),
                                    Name = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterName),
                                    Description = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterDescription),
                                    DefaultContentType = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyDefaultContentType),
                                    DocumentTemplateCount = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyDocumentTemplateCount).Split(new string[] { ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                                    Roles = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterCenterRoles).Split(new string[] { ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                                    Permissions = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyMatterCenterPermissions).Split(new string[] { ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                                    BlockUserNames = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyBlockedUsers).Split(new string[] { ConstantStrings.Semicolon }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                                    AssignUserNames = matterCenterUserCollection.ToList<IList<string>>(),
                                    AssignUserEmails = matterCenterUserEmailsCollection.ToList<IList<string>>(),
                                    Conflict = new Conflict()
                                    {
                                        CheckBy = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyConflictCheckBy),
                                        CheckOn = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyConflictCheckDate),
                                        Identified = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyIsConflictIdentified),
                                        SecureMatter = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertySecureMatter),
                                    }
                                },
                                MatterDetailsObject = EditMatterHelperFunctions.ExtractMatterDetails(stampedPropertyValues),
                                ClientObject = new Client()
                                {
                                    Id = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyClientID),
                                    Name = EditMatterHelperFunctions.GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyClientName),
                                }
                            };
                            result = JsonConvert.SerializeObject(matterStampedDetails);
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
        /// Used to get the default matter values for the client from the SharePoint list
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="siteCollectionPath">Site collection path</param>
        /// <returns>JSON structure with default values of matters for the client</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetDefaultMatterConfigurations(RequestObject requestObject, string siteCollectionPath)
        {
            string result = string.Empty, settingsUpdatedDate = string.Empty;
            int errorCodeModifiedDate = 0;  // Error code to be set if no list item found in Matter Configuration list
            if (null != requestObject && !string.IsNullOrWhiteSpace(siteCollectionPath) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(siteCollectionPath), requestObject.RefreshToken))
                    {
                        if (Lists.CheckPermissionOnList(ServiceUtility.GetClientContext(null, new Uri(siteCollectionPath), requestObject.RefreshToken), ServiceConstantStrings.MatterConfigurationsList, PermissionKind.EditListItems))
                        {
                            string listQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MatterConfigurationsListQuery, SearchConstants.ManagedPropertyTitle, ServiceConstantStrings.MatterConfigurationTitleValue);
                            ListItem settingsItem = Lists.GetData(clientContext, ServiceConstantStrings.MatterConfigurationsList, listQuery).FirstOrDefault();
                            if (null != settingsItem)
                            {
                                settingsUpdatedDate = Convert.ToString(settingsItem[ServiceConstantStrings.ColumnNameModifiedDate], CultureInfo.InvariantCulture);
                                result = HttpUtility.HtmlDecode(string.Concat(Convert.ToString(settingsItem[ServiceConstantStrings.MatterConfigurationColumn], CultureInfo.InvariantCulture), ConstantStrings.Pipe, ConstantStrings.DOLLAR, ConstantStrings.Pipe, settingsUpdatedDate));
                            }
                            else
                            {
                                settingsUpdatedDate = Convert.ToString(errorCodeModifiedDate, CultureInfo.InvariantCulture);
                            }
                        }
                        else
                        {
                            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.UserNotSiteOwnerCode, ServiceConstantStrings.UserNotSiteOwnerMessage);
                        }
                    }
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }

            if (string.IsNullOrEmpty(result))
            {
                MatterConfigurations configurations = new MatterConfigurations();
                configurations.IsCalendarSelected = true;
                configurations.IsConflictCheck = true;
                configurations.IsEmailOptionSelected = true;
                configurations.IsMatterDescriptionMandatory = true;
                configurations.IsRestrictedAccessSelected = true;
                configurations.IsRSSSelected = true;
                configurations.IsTaskSelected = true;
                result = JsonConvert.SerializeObject(configurations);
                result = string.Concat(result, ConstantStrings.Pipe, ConstantStrings.DOLLAR, ConstantStrings.Pipe, settingsUpdatedDate);
            }
            return result;
        }

        /// <summary>
        /// Save Matter Configurations back to SharePoint list
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="siteCollectionPath">Site Collection Path</param>
        /// <param name="matterConfigurations">Matter Configurations object</param>
        /// <param name="userId">User Ids</param>
        /// <param name="cachedItemModifiedDate">Date and time when user loaded the client settings page to configure default values</param>
        /// <returns>Error message or success</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string SaveMatterConfigurations(RequestObject requestObject, string siteCollectionPath, MatterConfigurations matterConfigurations, IList<string> userId, string cachedItemModifiedDate)
        {
            string result = string.Empty;
            if (null != requestObject && !string.IsNullOrWhiteSpace(siteCollectionPath) && null != matterConfigurations && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(siteCollectionPath), requestObject.RefreshToken))
                    {
                        Matter matter = new Matter();
                        matter.AssignUserNames = SettingsHelper.GetUserList(matterConfigurations.MatterUsers);
                        matter.AssignUserEmails = SettingsHelper.GetUserList(matterConfigurations.MatterUserEmails);

                        if (0 < matter.AssignUserNames.Count)
                        {
                            result = EditMatterHelperFunctions.ValidateTeamMembers(clientContext, matter, userId);
                        }
                        if (string.IsNullOrEmpty(result))
                        {
                            result = SettingsHelper.SaveConfigurationToList(matterConfigurations, clientContext, cachedItemModifiedDate);
                            bool tempResult = false;
                            if (Boolean.TryParse(result, out tempResult))
                            {
                                if (tempResult)
                                {
                                    string listQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MatterConfigurationsListQuery, SearchConstants.ManagedPropertyTitle, ServiceConstantStrings.MatterConfigurationTitleValue);
                                    ListItem settingsItem = Lists.GetData(clientContext, ServiceConstantStrings.MatterConfigurationsList, listQuery).FirstOrDefault();
                                    if (null != settingsItem)
                                    {
                                        cachedItemModifiedDate = Convert.ToString(settingsItem[ServiceConstantStrings.ColumnNameModifiedDate], CultureInfo.InvariantCulture);
                                    }
                                    result = string.Concat(result, ConstantStrings.Pipe, ConstantStrings.DOLLAR, ConstantStrings.Pipe, cachedItemModifiedDate);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return result;

        }

        /// <summary>
        /// Updated matter permission details.
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="client">Client object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="matterDetails">Matter Object containing Matter metadata</param>
        /// <param name="editMode">Edit/Add mode</param>
        /// <param name="userId">User Id information</param>
        /// <returns>Status of operation</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string UpdateMatterDetails(RequestObject requestObject, Client client, Matter matter, MatterDetails matterDetails, string editMode, IList<string> userId)
        {
            string result = ConstantStrings.TRUE;
            if (null != requestObject && null != client && null != matter && null != matterDetails && (null != requestObject.RefreshToken || null != requestObject.SPAppToken) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                IEnumerable<RoleAssignment> userPermissionOnLibrary = null;
                PropertyValues matterStampedProperties = null;
                int listItemId = -1;
                bool isEditMode = false;
                string editMatterValidation = string.Empty;
                string loggedInUserName = string.Empty;
                try
                {
                    isEditMode = Convert.ToBoolean(editMode, CultureInfo.InvariantCulture);
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        List<string> listExists = ProvisionHelperFunctions.CheckListsExist(clientContext, matter.Name);
                        bool hasFullPermission = false;
                        try
                        {
                            editMatterValidation = ValidationHelperFunctions.ProvisionMatterValidation(requestObject, client, clientContext, matter, matterDetails, int.Parse(ConstantStrings.EditMatterPermission, CultureInfo.InvariantCulture), null);
                            if (string.IsNullOrWhiteSpace(editMatterValidation))
                            {
                                if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                                {
                                    if (0 == matter.AssignUserEmails.Count())
                                    {
                                        result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserNamesCode, TextConstants.IncorrectInputUserNamesMessage);
                                    }
                                    else
                                    {
                                        result = EditMatterHelperFunctions.ValidateTeamMembers(clientContext, matter, userId);
                                        if (string.IsNullOrEmpty(result))
                                        {
                                            result = ConstantStrings.TRUE;
                                            if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                                            {
                                                if (Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture))
                                                {
                                                    result = EditMatterHelperFunctions.CheckSecurityGroupInTeamMembers(clientContext, matter, userId);
                                                }
                                            }
                                            else
                                            {
                                                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictIdentifiedCode, TextConstants.IncorrectInputConflictIdentifiedMessage);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictIdentifiedCode, TextConstants.IncorrectInputConflictIdentifiedMessage);
                                }
                                if (string.Equals(result, ConstantStrings.TRUE, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Get matter stamped properties
                                    matterStampedProperties = EditMatterHelperFunctions.FetchMatterStampedProperties(clientContext, matter.Name);
                                    loggedInUserName = EditMatterHelperFunctions.GetUserUpdatingMatter(clientContext);
                                    bool isFullControlPresent = EditMatterHelperFunctions.ValidateFullControlPermission(matter);
                                    if (isFullControlPresent)
                                    {
                                        // Get matter library current permissions
                                        userPermissionOnLibrary = EditMatterHelperFunctions.FetchUserPermission(clientContext, matter.Name);
                                        // Check if OneNote library, calendar, and matter landing page exists as separate objects
                                        string originalMatterName = EditMatterHelperFunctions.GetMatterName(clientContext, matter.Name);
                                        listItemId = Lists.RetrieveItemId(clientContext, ServiceConstantStrings.MatterLandingPageRepositoryName, originalMatterName);
                                        List<string> usersToRemove = EditMatterHelperFunctions.RetrieveMatterUsers(userPermissionOnLibrary);
                                        // Provide logged in user as full control on matter
                                        // 	Check whether logged in user has full permission on new permission changes
                                        hasFullPermission = EditMatterHelperFunctions.CheckFullPermissionInAssignList(matter.AssignUserEmails, matter.Permissions, loggedInUserName);
                                        EditMatterHelperFunctions.AssignRemoveFullControl(clientContext, matter, loggedInUserName, listItemId, listExists, true, hasFullPermission);
                                        if (listExists.Contains(matter.Name))
                                        {
                                            result = EditMatterHelperFunctions.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name, -1, isEditMode);
                                        }
                                        if (listExists.Contains(matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix))
                                        {
                                            result = EditMatterHelperFunctions.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix, -1, isEditMode);
                                        }
                                        if (listExists.Contains(matter.Name + ServiceConstantStrings.CalendarNameSuffix))
                                        {
                                            result = EditMatterHelperFunctions.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + ServiceConstantStrings.CalendarNameSuffix, -1, isEditMode);
                                        }
                                        if (listExists.Contains(matter.Name + ServiceConstantStrings.TaskNameSuffix))
                                        {
                                            result = EditMatterHelperFunctions.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + ServiceConstantStrings.TaskNameSuffix, -1, isEditMode);
                                        }
                                        result = EditMatterHelperFunctions.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, true, ServiceConstantStrings.MatterLandingPageRepositoryName, listItemId, isEditMode);
                                        // Update matter metadata
                                        result = EditMatterHelperFunctions.UpdateMatterStampedProperties(clientContext, matterDetails, matter, matterStampedProperties, isEditMode);
                                    }
                                    else
                                    {
                                        result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectInputSelfPermissionRemoval, ServiceConstantStrings.ErrorEditMatterMandatoryPermission);
                                    }
                                }
                            }
                            else
                            {
                                result = editMatterValidation;
                            }
                        }
                        catch (Exception exception)
                        {
                            MatterRevertList matterRevertListObject = new MatterRevertList()
                            {
                                MatterLibrary = matter.Name,
                                MatterOneNoteLibrary = matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix,
                                MatterCalendar = matter.Name + ServiceConstantStrings.CalendarNameSuffix,
                                MatterTask = matter.Name + ServiceConstantStrings.TaskNameSuffix,
                                MatterSitePages = ServiceConstantStrings.MatterLandingPageRepositoryName
                            };
                            EditMatterHelperFunctions.RevertMatterUpdates(requestObject, client, matter, clientContext, matterRevertListObject, loggedInUserName, userPermissionOnLibrary, listItemId, isEditMode);
                            result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                        }
                        finally
                        {
                            // Remove full control for logged in users	
                            EditMatterHelperFunctions.AssignRemoveFullControl(clientContext, matter, loggedInUserName, listItemId, listExists, false, hasFullPermission);
                        }
                    }
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return result;
        }

        /// <summary>
        /// Checks if security group exists in team members list in case conflict are identified.
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="client">Client object</param>
        /// <param name="matter">Matter object</param>
        /// <returns>Validation result</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string CheckSecurityGroupExists(RequestObject requestObject, Client client, Matter matter, IList<string> userId)
        {
            string result = ConstantStrings.TRUE;
            if (null != requestObject && null != client && null != matter && (null != requestObject.RefreshToken || null != requestObject.SPAppToken || null != client.Url) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        if (0 == matter.AssignUserEmails.Count())
                        {
                            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserNamesCode, TextConstants.IncorrectInputUserNamesMessage);
                        }
                        else
                        {
                            result = EditMatterHelperFunctions.ValidateTeamMembers(clientContext, matter, userId);
                        }
                        if (string.IsNullOrEmpty(result))
                        {
                            result = ConstantStrings.TRUE;
                            if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                            {
                                if (0 == matter.AssignUserEmails.Count())
                                {
                                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserNamesCode, TextConstants.IncorrectInputUserNamesMessage);
                                }
                                else
                                {
                                    if (Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture))
                                    {
                                        result = EditMatterHelperFunctions.CheckSecurityGroupInTeamMembers(clientContext, matter, userId);
                                    }
                                }
                            }
                            else
                            {
                                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictIdentifiedCode, TextConstants.IncorrectInputConflictIdentifiedMessage);
                            }
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }
            return result;
        }
        #endregion
    }
}
