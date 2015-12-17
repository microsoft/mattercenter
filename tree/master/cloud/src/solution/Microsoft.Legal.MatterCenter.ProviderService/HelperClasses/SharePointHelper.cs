// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-prd
// Created          : 06-19-2014
//
// <copyright file="SharePointHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to perform SharePoint functionalities.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    #endregion

    /// <summary>
    /// Provide methods to perform SharePoint functionalities.
    /// </summary>
    internal static class SharePointHelper
    {       
        /// <summary>
        /// Gets the list of content types along with their properties.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="contentTypesNames">List of Content Type Names associated with matter</param>
        /// <param name="requestObject">Request Object</param>
        /// <param name="client">Client Object</param>
        /// <param name="matter">Matter Object</param>
        /// <returns>List of content types</returns>
        internal static IList<ContentType> GetContentTypeData(ClientContext clientContext, IList<string> contentTypesNames, RequestObject requestObject = null, Client client = null, Matter matter = null)
        {
            ContentTypeCollection contentTypeCollection = null;
            IList<ContentType> selectedContentTypeCollection = new List<ContentType>();
            try
            {
                if (null != clientContext && null != contentTypesNames)
                {
                    Web web = clientContext.Web;
                    contentTypeCollection = web.ContentTypes;
                    clientContext.Load(contentTypeCollection, contentType => contentType.Include(thisContentType => thisContentType.Name).Where(currContentType => currContentType.Group == ServiceConstantStrings.OneDriveContentTypeGroup));
                    clientContext.ExecuteQuery();
                    selectedContentTypeCollection = GetContentTypeList(contentTypesNames, contentTypeCollection.ToList());
                }
            }
            catch (Exception exception)
            {
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            return selectedContentTypeCollection;
        }

        /// <summary>
        /// Generates the list of Content Types that are associated with matter.
        /// </summary>
        /// <param name="contentTypesNames">List of Content Type names that user has selected</param>
        /// <param name="contentTypeList">Content Types listed in Content Type hub under Matter Center group</param>
        /// <returns>List of Content Types associated with matter</returns>
        private static IList<ContentType> GetContentTypeList(IList<string> contentTypesNames, List<ContentType> contentTypeList)
        {
            IList<ContentType> selectedContentTypeCollection = new List<ContentType>();
            ContentType selectedContentType = null;
            foreach (string contentTypeName in contentTypesNames)
            {
                selectedContentType = (from currContentType in contentTypeList
                                       where currContentType.Name.ToUpperInvariant().Equals(contentTypeName.ToUpperInvariant())
                                       select currContentType).ToList().FirstOrDefault();
                if (null != selectedContentType)
                {
                    selectedContentTypeCollection.Add(selectedContentType);
                }
            }
            return selectedContentTypeCollection;
        }        

        /// <summary>
        /// Bulk resolves the specified users.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="userNames">Name of the users to be resolved</param>
        /// <returns>List of resolved users</returns>
        public static IList<FieldUserValue> ResolveUserNames(ClientContext clientContext, IList<string> userNames)
        {
            List<FieldUserValue> userList = new List<FieldUserValue>();
            if (null != clientContext && null != userNames)
            {
                foreach (string userName in userNames)
                {
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        User user = clientContext.Web.EnsureUser(userName.Trim());
                        ///// Only Fetch the User ID which is required
                        clientContext.Load(user, u => u.Id);
                        clientContext.ExecuteQuery();
                        ///// Add the user to the first element of the FieldUserValue array.
                        FieldUserValue tempUser = new FieldUserValue();
                        tempUser.LookupId = user.Id;
                        userList.Add(tempUser);
                    }
                }
            }
            return userList;
        }      

        /// <summary>
        /// Places service call to search user based on the search term.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Set of users returned by people picker service based on the search term</returns>
        internal static IList<PeoplePickerUser> SearchUsers(ClientContext clientContext, string searchTerm)
        {
            List<PeoplePickerUser> userResult = null;
            string results = string.Empty;
            int peoplePickerMaxRecords;
            try
            {
                ClientPeoplePickerQueryParameters queryParams = new ClientPeoplePickerQueryParameters();
                queryParams.AllowMultipleEntities = Convert.ToBoolean(ServiceConstantStrings.PeoplePickerAllowMultipleEntities, CultureInfo.InvariantCulture);
                queryParams.MaximumEntitySuggestions = Convert.ToInt32(ServiceConstantStrings.PeoplePickerMaximumEntitySuggestions, CultureInfo.InvariantCulture);
                queryParams.PrincipalSource = PrincipalSource.All;
                queryParams.PrincipalType = PrincipalType.User | PrincipalType.SecurityGroup;
                queryParams.QueryString = searchTerm;
                peoplePickerMaxRecords = Convert.ToInt32(ServiceConstantStrings.PeoplePickerMaxRecords, CultureInfo.InvariantCulture);

                ClientResult<string> clientResult = ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(clientContext, queryParams);
                clientContext.ExecuteQuery();
                results = clientResult.Value;
                userResult = JsonConvert.DeserializeObject<List<PeoplePickerUser>>(results).Where(result => (string.Equals(result.EntityType, ConstantStrings.PeoplePickerEntityTypeUser, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.Description)) || (!string.Equals(result.EntityType, ConstantStrings.PeoplePickerEntityTypeUser, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.EntityData.Email))).Take(peoplePickerMaxRecords).ToList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return userResult;
        } 
    }
}