// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-sauve
// Created          : 06-03-2015
//
// ***********************************************************************
// <copyright file="SearchHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods for SharePoint Search functionality.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.Security.Application;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Search.Query;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    #endregion

    /// <summary>
    /// Provide methods for SharePoint Search functionality.
    /// </summary>
    internal static class SearchHelperFunctions
    {
        /// <summary>
        /// Retrieves folder hierarchy from document library.
        /// </summary>
        /// <param name="list">SharePoint library</param>
        /// <param name="listItems">List items</param>
        /// <param name="allFolders">List of all folders of type folder data</param>
        /// <returns>List of folders of type folder data</returns>
        internal static List<FolderData> GetFolderAssignment(List list, ListItemCollection listItems, List<FolderData> allFolders)
        {
            FolderData folderData = new FolderData()
            {
                Name = list.RootFolder.Name,
                URL = list.RootFolder.ServerRelativeUrl,
                ParentURL = null
            };
            allFolders.Add(folderData);
            foreach (var listItem in listItems)
            {
                folderData = new FolderData()
                {
                    Name = Convert.ToString(listItem[ServiceConstantStrings.ColumnNameFileLeafRef], CultureInfo.InvariantCulture),
                    URL = Convert.ToString(listItem[ServiceConstantStrings.ColumnNameFileRef], CultureInfo.InvariantCulture),
                    ParentURL = Convert.ToString(listItem[ServiceConstantStrings.ColumnNameFileDirRef], CultureInfo.InvariantCulture)
                };

                allFolders.Add(folderData);
            }

            return allFolders;
        }

        /// <summary>
        /// Returns the Keyword query object with refiners added for sorting
        /// </summary>
        /// <param name="keywordQuery">Keyword object</param>
        /// <param name="sortByProperty">Property by which sort is applied</param>
        /// <param name="sortDirection">Direction in which sort is applied (0 --> Ascending order and 1 --> Descending order)</param>
        /// <returns>Keyword object with sorting refiners applied</returns>
        internal static KeywordQuery AddSortingRefiner(KeywordQuery keywordQuery, string sortByProperty, int sortDirection)
        {
            if (0 == sortDirection)
            {
                keywordQuery.SortList.Add(sortByProperty, Microsoft.SharePoint.Client.Search.Query.SortDirection.Ascending);
            }
            else if (1 == sortDirection)
            {
                keywordQuery.SortList.Add(sortByProperty, Microsoft.SharePoint.Client.Search.Query.SortDirection.Descending);
            }
            return keywordQuery;
        }

        /// <summary>
        /// Defines the sorting property and direction for querying SharePoint Search.
        /// </summary>
        /// <param name="keywordQuery">Keyword object</param>
        /// <param name="searchObject">Search object</param>
        /// <param name="isMatterSearch">Boolean flag which determines current search is for matters or documents</param>
        /// <returns></returns>
        internal static KeywordQuery GetSortByProperty(KeywordQuery keywordQuery, SearchObject searchObject, Boolean isMatterSearch)
        {
            string matterIDRefiner = string.Empty;
            ////Sorting by specified property  0 --> Ascending order and 1 --> Descending order
            if (!string.IsNullOrWhiteSpace(searchObject.Sort.ByProperty))
            {
                keywordQuery = AddSortingRefiner(keywordQuery, searchObject.Sort.ByProperty, searchObject.Sort.Direction);
                //// Add Matter ID property as second level sort for Client.MatterID column based on Search Matter or Search Document
                if (SearchConstants.ManagedPropertyClientID == searchObject.Sort.ByProperty || SearchConstants.ManagedPropertyDocumentClientId == searchObject.Sort.ByProperty)
                {
                    if (isMatterSearch)
                    {
                        matterIDRefiner = SearchConstants.ManagedPropertyMatterId;
                    }
                    else
                    {
                        matterIDRefiner = SearchConstants.ManagedPropertyDocumentMatterId;
                    }
                    keywordQuery = AddSortingRefiner(keywordQuery, matterIDRefiner, searchObject.Sort.Direction);
                }
            }
            return keywordQuery;
        }

        /// <summary>
        /// Forms filter query for the specified property and data list.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="dataList">List of values as data</param>
        /// <returns>Filter query</returns>
        internal static string FormFilterQuery(string propertyName, IList<string> dataList)
        {
            string previousFilterValues = string.Empty;
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(propertyName) && null != dataList)
                {
                    if (1 == dataList.Count)
                    {
                        previousFilterValues = string.Concat(propertyName, ConstantStrings.COLON);
                    }
                    else
                    {
                        previousFilterValues = string.Concat(propertyName, ConstantStrings.COLON, ConstantStrings.Space, ConstantStrings.OpeningBracket, ConstantStrings.OperatorOR, ConstantStrings.OpeningBracket);
                    }
                    for (int counter = 0; counter < dataList.Count; counter++)
                    {
                        if (0 < counter)
                        {
                            previousFilterValues += ConstantStrings.Comma;
                        }
                        previousFilterValues += string.Concat(ConstantStrings.DoubleQuote, dataList[counter], ConstantStrings.DoubleQuote);
                    }
                    if (1 != dataList.Count)
                    {
                        previousFilterValues += string.Concat(ConstantStrings.ClosingBracket, ConstantStrings.ClosingBracket);
                    }
                }
                result = previousFilterValues;
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Gets pinned matters/documents specific to user.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing client URL</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="listColumnName">Name of the list column</param>
        /// <param name="isShowDocument">Process Document (true) or Matter (false)</param>
        /// <returns>JSON structure with the meta-data of pinned Matter/Document for requested user</returns>
        internal static string ShowPinData(RequestObject requestObject, Client client, string listName, string listColumnName, bool isShowDocument)
        {
            string result = string.Empty;
            if (null != requestObject && null != client && !string.IsNullOrWhiteSpace(listName) && !string.IsNullOrWhiteSpace(listColumnName))
            {
                ////Holds logged-in user alias
                string userAlias = string.Empty;

                ////Object to store all the list items retrieved from SharePoint list
                ListItemCollection listItems;

                ////Stores the JSON structure with the meta-data of pinned matter/document
                string userPinnedDetails = string.Empty;

                ////Stores the count of pinned matters/documents for the requested user
                int userPinnedDetailCount = 0;

                try
                {
                    ////Object to store list item details as per pinned matter/document
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        ////Get logged in user alias
                        Users currentUserDetail = UIUtility.GetLoggedInUserDetails(clientContext);
                        userAlias = currentUserDetail.LogOnName;
                        listItems = Lists.GetData(clientContext, listName, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.UserPinnedDetailsQuery, ServiceConstantStrings.PinnedListColumnUserAlias, userAlias, listColumnName));
                        clientContext.Load(listItems);
                        clientContext.ExecuteQuery();

                        ////ListItems are present in the list
                        if (null != listItems && listItems.Count > 0)
                        {
                            ////Since we are maintaining only single list item per user, listItems collection will have only one object; hence accessing first object
                            ////Check if column holds null or empty string. If non empty, pinned matter/document exists
                            if (!string.IsNullOrEmpty(Convert.ToString(listItems[0][listColumnName], CultureInfo.InvariantCulture)))
                            {
                                string userPinnedMatter = Convert.ToString(listItems[0][listColumnName], CultureInfo.InvariantCulture);
                                if (isShowDocument)
                                {
                                    Dictionary<string, DocumentData> userpinnedMatterCollection = JsonConvert.DeserializeObject<Dictionary<string, DocumentData>>(userPinnedMatter);
                                    userPinnedDetailCount = userpinnedMatterCollection.Count;
                                    userPinnedDetails = JsonConvert.SerializeObject(userpinnedMatterCollection.Values.Reverse(), Newtonsoft.Json.Formatting.Indented);
                                    userPinnedDetails = string.Concat(userPinnedDetails, ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR, userPinnedDetailCount);
                                }
                                else
                                {
                                    Dictionary<string, MatterData> userpinnedMatterCollection = JsonConvert.DeserializeObject<Dictionary<string, MatterData>>(userPinnedMatter);
                                    userPinnedDetailCount = userpinnedMatterCollection.Count;
                                    userPinnedDetails = JsonConvert.SerializeObject(userpinnedMatterCollection.Values.Reverse(), Newtonsoft.Json.Formatting.Indented);
                                    userPinnedDetails = string.Concat(userPinnedDetails, ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR, userPinnedDetailCount);
                                }
                                result = userPinnedDetails;
                            }
                            else
                            {
                                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, 0, ConstantStrings.NoPinnedMessage);
                            }
                        }
                        else
                        {
                            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, 0, ConstantStrings.NoPinnedMessage);
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
                result = TextConstants.MessageNoInputs;
            }
            return result;
        }

        /// <summary>
        /// Gets the current user pinned details.
        /// </summary>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <param name="getUserPinnedDetails">This is an object that contains the details of the specific pinned matter/document.</param>
        /// <returns>This returns an object that contains the details of the specific pinned matter/document.</returns>
        internal static PinUnpinDetails GetCurrentUserPinnedDetails(bool isMatterView, object getUserPinnedDetails)
        {
            PinUnpinDetails userPinnedDetails = new PinUnpinDetails();
            try
            {
                if (isMatterView)
                {
                    userPinnedDetails.UserPinnedMatterData = (MatterData)getUserPinnedDetails;
                }
                else
                {
                    userPinnedDetails.UserPinnedDocumentData = (DocumentData)getUserPinnedDetails;
                }

                userPinnedDetails.ListName = isMatterView ? ServiceConstantStrings.UserPinnedMatterListName : ServiceConstantStrings.UserPinnedDocumentListName;
                userPinnedDetails.PinnedListColumnDetails = isMatterView ? ServiceConstantStrings.PinnedListColumnMatterDetails : ServiceConstantStrings.PinnedListColumnDocumentDetails;
                userPinnedDetails.URL = isMatterView ? userPinnedDetails.UserPinnedMatterData.MatterUrl : userPinnedDetails.UserPinnedDocumentData.DocumentUrl;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            return userPinnedDetails;
        }

        /// <summary>
        /// Pins the record and associate to logged-in user.
        /// </summary>
        /// <param name="clientContext">The client context object</param>
        /// <param name="getUserPinnedDetails">This is an object that contains the details of the specific pinned matter/document.</param>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <returns>It returns a string object, that contains the execution status of the PinThisRecord function.</returns>
        internal static string PinThisRecord(ClientContext clientContext, object getUserPinnedDetails, bool isMatterView)
        {
            string result = string.Empty;
            if (null != clientContext)
            {
                bool status = false;
                string userAlias = string.Empty;
                string pinnedDetailsJson = string.Empty;
                ListItemCollection listItems;

                PinUnpinDetails userPinnedDetails = GetCurrentUserPinnedDetails(isMatterView, getUserPinnedDetails);

                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(userPinnedDetails.ListName);
                    Users currentUserDetail = UIUtility.GetLoggedInUserDetails(clientContext);
                    userAlias = currentUserDetail.LogOnName;
                    listItems = Lists.GetData(clientContext, userPinnedDetails.ListName, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.UserPinnedDetailsQuery, ServiceConstantStrings.PinnedListColumnUserAlias, userAlias, userPinnedDetails.PinnedListColumnDetails));
                    ////Pinned matter/document(s) exists for users
                    if (null != listItems && 0 < listItems.Count)
                    {
                        ////Logic to create pinned matter/document
                        if (isMatterView)
                        {
                            string userPinnedMatter = !string.IsNullOrEmpty(Convert.ToString(listItems[0][ServiceConstantStrings.PinnedListColumnMatterDetails], CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][ServiceConstantStrings.PinnedListColumnMatterDetails], CultureInfo.InvariantCulture) : string.Empty;
                            // Check if empty entry is retrieved
                            if (!string.IsNullOrWhiteSpace(userPinnedMatter))
                            {
                                Dictionary<string, MatterData> userpinnedMatterCollection = JsonConvert.DeserializeObject<Dictionary<string, MatterData>>(userPinnedMatter);
                                // Check if matter is already pinned
                                if (!userpinnedMatterCollection.ContainsKey(userPinnedDetails.UserPinnedMatterData.MatterUrl))
                                {
                                    userpinnedMatterCollection.Add(userPinnedDetails.UserPinnedMatterData.MatterUrl, userPinnedDetails.UserPinnedMatterData);
                                    pinnedDetailsJson = JsonConvert.SerializeObject(userpinnedMatterCollection, Newtonsoft.Json.Formatting.Indented);
                                }
                                else
                                {
                                    status = true;
                                }
                            }
                            else
                            {
                                pinnedDetailsJson = GetFirstPinnedMatter(userPinnedDetails);
                            }
                        }
                        else
                        {
                            string userPinnedDocument = !string.IsNullOrEmpty(Convert.ToString(listItems[0][ServiceConstantStrings.PinnedListColumnDocumentDetails], CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][ServiceConstantStrings.PinnedListColumnDocumentDetails], CultureInfo.InvariantCulture) : string.Empty;
                            if (!string.IsNullOrWhiteSpace(userPinnedDocument))
                            {
                                Dictionary<string, DocumentData> userpinnedDocumentCollection = JsonConvert.DeserializeObject<Dictionary<string, DocumentData>>(userPinnedDocument);
                                // Check if document is already pinned
                                if (!userpinnedDocumentCollection.ContainsKey(userPinnedDetails.URL))
                                {
                                    userpinnedDocumentCollection.Add(userPinnedDetails.URL, userPinnedDetails.UserPinnedDocumentData);
                                    pinnedDetailsJson = JsonConvert.SerializeObject(userpinnedDocumentCollection, Newtonsoft.Json.Formatting.Indented);
                                }
                                else
                                {
                                    status = true;
                                }
                            }
                            else
                            {
                                pinnedDetailsJson = GetFirstPinnedDocument(userPinnedDetails);
                            }
                        }

                        // Run update query only when status is false
                        if (!status)
                        {
                            ////We are maintaining single list item entry for user
                            listItems[0][userPinnedDetails.PinnedListColumnDetails] = pinnedDetailsJson;
                            listItems[0].Update();
                            clientContext.ExecuteQuery();
                            status = true;
                        }
                    }
                    else
                    {
                        ////No pinned matter/document(s) for logged in user. Create pinned matter/document for the user.
                        ////Create pin request
                        if (isMatterView)
                        {
                            pinnedDetailsJson = GetFirstPinnedMatter(userPinnedDetails);
                        }
                        else
                        {
                            pinnedDetailsJson = GetFirstPinnedDocument(userPinnedDetails);
                        }
                        ////Logic to create list item entry for user
                        ListItemCreationInformation listItemInformation = new ListItemCreationInformation();
                        ListItem listItem = list.AddItem(listItemInformation);
                        listItem[ServiceConstantStrings.PinnedListColumnUserAlias] = userAlias;
                        listItem[userPinnedDetails.PinnedListColumnDetails] = pinnedDetailsJson;
                        listItem.Update();
                        clientContext.ExecuteQuery();
                        listItem.BreakRoleInheritance(false, true);     // Remove inheriting permissions on item
                        clientContext.ExecuteQuery();
                        status = true;
                    }
                    result = Convert.ToString(status, CultureInfo.InvariantCulture);
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = TextConstants.MessageNoInputs;
            }
            return result;
        }

        /// <summary>
        /// Gets the first pinned document serialized JSON object
        /// </summary>        
        /// <param name="userPinnedDetails">User document pin details object</param>
        /// <returns></returns>
        private static string GetFirstPinnedDocument(PinUnpinDetails userPinnedDetails)
        {
            Dictionary<string, DocumentData> userFirstPinnedDocument = new Dictionary<string, DocumentData>();
            userFirstPinnedDocument.Add(userPinnedDetails.URL, userPinnedDetails.UserPinnedDocumentData);
            string pinnedDetailsJson = JsonConvert.SerializeObject(userFirstPinnedDocument, Newtonsoft.Json.Formatting.Indented);
            return pinnedDetailsJson;
        }

        /// <summary>
        /// Gets the first pinned matter serialized JSON object
        /// </summary>
        /// <param name="userPinnedDetails">User matter pin details object</param>
        /// <returns>JSON format of pin data</returns>
        private static string GetFirstPinnedMatter(PinUnpinDetails userPinnedDetails)
        {
            Dictionary<string, MatterData> userFirstPinnedMatter = new Dictionary<string, MatterData>();
            userFirstPinnedMatter.Add(userPinnedDetails.UserPinnedMatterData.MatterUrl, userPinnedDetails.UserPinnedMatterData);
            string pinnedDetailsJson = JsonConvert.SerializeObject(userFirstPinnedMatter, Newtonsoft.Json.Formatting.Indented);
            return pinnedDetailsJson;
        }

        /// <summary>
        /// Removes the record and dissociate from logged-in user.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="getUserPinnedDetails">This is an object that contains the details of the specific pinned matter/document.</param>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <returns>It returns a string object, that contains the execution status of the function.</returns>
        internal static string RemoveThisRecord(ClientContext clientContext, object getUserPinnedDetails, bool isMatterView)
        {
            string result = string.Empty;
            if (null != clientContext)
            {
                bool status = false;
                string userAlias = string.Empty;
                ListItemCollection listItems;

                PinUnpinDetails userPinnedDetails = GetCurrentUserPinnedDetails(isMatterView, getUserPinnedDetails);

                try
                {
                    Users currentUserDetail = UIUtility.GetLoggedInUserDetails(clientContext);
                    userAlias = currentUserDetail.LogOnName;
                    listItems = Lists.GetData(clientContext, userPinnedDetails.ListName, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.UserPinnedDetailsQuery, ServiceConstantStrings.PinnedListColumnUserAlias, userAlias, userPinnedDetails.PinnedListColumnDetails));

                    ////Pinned matter/document(s) exists for users
                    if (null != listItems && 0 < listItems.Count)
                    {
                        ////Logic to create pinned matter/document
                        if (isMatterView)
                        {
                            string userPinnedMatter = !string.IsNullOrEmpty(Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], CultureInfo.InvariantCulture) : string.Empty;
                            Dictionary<string, MatterData> userpinnedMatterCollection = JsonConvert.DeserializeObject<Dictionary<string, MatterData>>(userPinnedMatter);

                            if (!string.IsNullOrWhiteSpace(userPinnedDetails.UserPinnedMatterData.MatterName) && userpinnedMatterCollection.ContainsKey(userPinnedDetails.UserPinnedMatterData.MatterName))
                            {
                                ////Only 1 pinned request for user
                                if (1 == userpinnedMatterCollection.Count)
                                {
                                    ////We are maintaining single list item entry for user
                                    listItems[0].DeleteObject();
                                }
                                else
                                {
                                    ////Matter already exists
                                    userpinnedMatterCollection.Remove(userPinnedDetails.UserPinnedMatterData.MatterName);
                                    string updatedMatter = JsonConvert.SerializeObject(userpinnedMatterCollection, Newtonsoft.Json.Formatting.Indented);
                                    ////We are maintaining single list item entry for user
                                    listItems[0][ServiceConstantStrings.PinnedListColumnMatterDetails] = updatedMatter;
                                    listItems[0].Update();
                                }
                            }
                        }
                        else
                        {
                            string userPinnedDocument = !string.IsNullOrEmpty(Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], CultureInfo.InvariantCulture) : string.Empty;
                            Dictionary<string, DocumentData> userpinnedDocumentCollection = JsonConvert.DeserializeObject<Dictionary<string, DocumentData>>(userPinnedDocument);
                            if (!string.IsNullOrWhiteSpace(userPinnedDetails.URL) && userpinnedDocumentCollection.ContainsKey(userPinnedDetails.URL))
                            {
                                ////Only 1 pinned request for user
                                if (1 == userpinnedDocumentCollection.Count)
                                {
                                    ////We are maintaining single list item entry for user
                                    listItems[0].DeleteObject();
                                }
                                else
                                {
                                    //// Matter already exists
                                    userpinnedDocumentCollection.Remove(userPinnedDetails.URL);
                                    string updatedDocument = JsonConvert.SerializeObject(userpinnedDocumentCollection, Newtonsoft.Json.Formatting.Indented);

                                    ////We are maintaining single list item entry for user
                                    listItems[0][userPinnedDetails.PinnedListColumnDetails] = updatedDocument;
                                    listItems[0].Update();
                                }
                            }
                        }

                        clientContext.ExecuteQuery();

                        status = true;
                    }
                    result = Convert.ToString(status, CultureInfo.InvariantCulture);
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            else
            {
                result = TextConstants.MessageNoInputs;
            }
            return result;
        }

        /// <summary>
        /// Prepares and returns the keyword query to get data from SharePoint Search based on filtering condition.
        /// </summary>
        /// <param name="client">The client object</param>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <param name="filterCondition">The filter condition.</param>
        /// <param name="managedProperty">The managed property.</param>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <returns>It returns a Keyword Query object.</returns>
        internal static KeywordQuery KeywordQueryMetrics(Client client, SearchObject searchObject, KeywordQuery keywordQuery, string filterCondition, string managedProperty, bool isMatterView)
        {
            KeywordQuery result = null;
            try
            {
                if (ServiceConstantStrings.IsTenantDeployment)
                {
                    keywordQuery.QueryText = searchObject.SearchTerm;
                }
                else
                {
                    keywordQuery.QueryText = "(" + searchObject.SearchTerm + " AND site:" + client.Url + ")";
                }

                keywordQuery.RefinementFilters.Add(filterCondition);
                if (isMatterView)
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ConstantStrings.COLON, ConstantStrings.DoubleQuote, true, ConstantStrings.DoubleQuote));
                }
                else
                {

                    string[] invalidExtensions = ServiceConstantStrings.FindDocumentInvalidExtensions.Split(',');
                    string chunk = string.Empty;

                    foreach (string extension in invalidExtensions)
                    {
                        chunk = chunk + "equals" + ConstantStrings.OpeningBracket + ConstantStrings.DoubleQuote + extension + ConstantStrings.DoubleQuote + ConstantStrings.ClosingBracket + ConstantStrings.Comma;
                    }
                    chunk = chunk.Remove(chunk.Length - 1);

                    keywordQuery.RefinementFilters.Add(string.Concat("not" + ConstantStrings.OpeningBracket + "FileType", ConstantStrings.COLON, ConstantStrings.OperatorOR + ConstantStrings.OpeningBracket +
                                                                                                            chunk + ConstantStrings.ClosingBracket + ConstantStrings.ClosingBracket
                                                                                                            ));
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ConstantStrings.COLON, "equals", ConstantStrings.OpeningBracket + ConstantStrings.DoubleQuote +
                                                                                                            "1" + ConstantStrings.DoubleQuote + ConstantStrings.ClosingBracket
                                                                                                            ));
                }

                keywordQuery.TrimDuplicates = false;
                if (0 < searchObject.PageNumber && 0 < searchObject.ItemsPerPage)
                {
                    keywordQuery.StartRow = (searchObject.PageNumber - 1) * searchObject.ItemsPerPage;
                    keywordQuery.RowLimit = searchObject.ItemsPerPage;
                }

                result = keywordQuery;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Assigns the keyword query values.
        /// </summary>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <param name="managedProperties">The managed properties.</param>
        /// <returns>It returns a Keyword Query object.</returns>
        internal static KeywordQuery AssignKeywordQueryValues(KeywordQuery keywordQuery, List<string> managedProperties)
        {
            KeywordQuery result = null;
            if (keywordQuery != null)
            {
                keywordQuery.SelectProperties.Clear();
                foreach (string selectProperties in managedProperties)
                {
                    keywordQuery.SelectProperties.Add(selectProperties);
                }
                result = keywordQuery;
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Returns the query to filter the matters/ documents for common filters.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <param name="isMatterView">Flag to identify matters/documents view.</param>
        /// <returns>It returns a keyword query object.</returns>
        internal static KeywordQuery FilterCommonDetails(SearchObject searchObject, KeywordQuery keywordQuery, bool isMatterView)
        {
            if (null != searchObject && null != keywordQuery)
            {

                if (null != searchObject.Filters.DateFilters)
                {
                    string lastModifiedTime = SearchConstants.ManagedPropertyLastModifiedTime;
                    //// Add refiner for Modified date value
                    if (!isMatterView)
                    {
                        lastModifiedTime = SearchConstants.ManagedPropertyDocumentLastModifiedTime;
                    }
                    keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.DateFilters.ModifiedFromDate, searchObject.Filters.DateFilters.ModifiedToDate, lastModifiedTime);

                    ////// Add refiner for Created date value
                    keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.DateFilters.CreatedFromDate, searchObject.Filters.DateFilters.CreatedToDate, SearchConstants.ManagedPropertyCreated);
                }
            }
            return keywordQuery;
        }

        /// <summary>
        /// Adds date refinement filter to the keyword query object
        /// </summary>        
        /// <param name="keywordQuery">The keyword query</param>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="managedProperty">Managed property name</param>
        /// <returns>Returns a keyword query object</returns>
        internal static KeywordQuery AddDateRefinementFilter(KeywordQuery keywordQuery, string fromDate, string toDate, string managedProperty)
        {
            if (!string.IsNullOrWhiteSpace(fromDate) && !string.IsNullOrWhiteSpace(toDate))
            {
                keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ConstantStrings.COLON, ConstantStrings.OperatorRange, ConstantStrings.OpeningBracket, fromDate, ConstantStrings.Comma, toDate, ConstantStrings.ClosingBracket));
            }
            else if (string.IsNullOrWhiteSpace(fromDate) && !string.IsNullOrWhiteSpace(toDate))
            {
                keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ConstantStrings.COLON, ConstantStrings.OperatorRange, ConstantStrings.OpeningBracket, ConstantStrings.MinDate, ConstantStrings.Comma, toDate, ConstantStrings.ClosingBracket));
            }
            else if (!string.IsNullOrWhiteSpace(fromDate) && string.IsNullOrWhiteSpace(toDate))
            {
                keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ConstantStrings.COLON, ConstantStrings.OperatorRange, ConstantStrings.OpeningBracket, fromDate, ConstantStrings.Comma, ConstantStrings.MaxDate, ConstantStrings.ClosingBracket));
            }
            return keywordQuery;
        }

        /// <summary>
        /// Prepares and returns the query to filter the documents.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <returns>It returns a Keyword Query object.</returns>
        internal static KeywordQuery FilterDocuments(SearchObject searchObject, KeywordQuery keywordQuery)
        {
            string filterValues = string.Empty;
            if (null != searchObject && null != keywordQuery)
            {
                if (null != searchObject.Filters)
                {
                    keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.FromDate, searchObject.Filters.ToDate, SearchConstants.ManagedPropertyCreated);
                    if (null != searchObject.Filters.DocumentAuthor && !string.IsNullOrWhiteSpace(searchObject.Filters.DocumentAuthor))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyAuthor, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.DocumentAuthor, ConstantStrings.DoubleQuote));
                    }

                    if (0 < searchObject.Filters.ClientsList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.ClientsList[0]))
                    {
                        filterValues = SearchHelperFunctions.FormFilterQuery(SearchConstants.ManagedPropertyDocumentClientName, searchObject.Filters.ClientsList);
                        keywordQuery.RefinementFilters.Add(filterValues);
                    }

                    /* New refinement filters for list view control */

                    if (!string.IsNullOrWhiteSpace(searchObject.Filters.Name))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyFileName, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.Name, ConstantStrings.DoubleQuote));
                    }

                    if (!string.IsNullOrWhiteSpace(searchObject.Filters.ClientName))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyDocumentClientName, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.ClientName, ConstantStrings.DoubleQuote));
                    }

                    if (null != searchObject.Filters.DocumentCheckoutUsers && !string.IsNullOrWhiteSpace(searchObject.Filters.DocumentCheckoutUsers))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyDocumentCheckOutUser, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.DocumentCheckoutUsers, ConstantStrings.DoubleQuote));
                    }
                }
                keywordQuery = FilterCommonDetails(searchObject, keywordQuery, false);
            }
            return keywordQuery;
        }

        /// <summary>
        /// Returns the query to filter the matters.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <returns>It returns a keyword query object.</returns>
        internal static KeywordQuery FilterMattersUtility(SearchObject searchObject, KeywordQuery keywordQuery)
        {
            if (null != searchObject && null != keywordQuery && null != searchObject.Filters)
            {
                /* New refinement filters for list view control */
                if (!string.IsNullOrWhiteSpace(searchObject.Filters.Name))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyMatterName, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.Name, ConstantStrings.DoubleQuote));
                }

                if (!string.IsNullOrWhiteSpace(searchObject.Filters.ClientName))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyClientName, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.ClientName, ConstantStrings.DoubleQuote));
                }

                if (null != searchObject.Filters.ResponsibleAttorneys && !string.IsNullOrWhiteSpace(searchObject.Filters.ResponsibleAttorneys))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertyResponsibleAttorney, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.ResponsibleAttorneys, ConstantStrings.DoubleQuote));
                }

                if (!string.IsNullOrWhiteSpace(searchObject.Filters.SubareaOfLaw))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(SearchConstants.ManagedPropertySubAreaOfLaw, ConstantStrings.COLON, ConstantStrings.DoubleQuote, searchObject.Filters.SubareaOfLaw, ConstantStrings.DoubleQuote));
                }
                if (null != searchObject.Filters.DateFilters)
                {
                    ////// Add refiner for Open date value
                    keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.DateFilters.OpenDateFrom, searchObject.Filters.DateFilters.OpenDateTo, SearchConstants.ManagedPropertyOpenDate);
                }
            }

            return keywordQuery;
        }

        /// <summary>
        /// Returns the query to filter the matters.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <returns>It returns a keyword query object.</returns>
        internal static KeywordQuery FilterMatters(SearchObject searchObject, KeywordQuery keywordQuery)
        {
            string filterValues = string.Empty;
            if (null != searchObject && null != keywordQuery)
            {
                if (null != searchObject.Filters)
                {
                    if (null != searchObject.Filters.AOLList && 0 < searchObject.Filters.AOLList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.AOLList[0]))
                    {
                        filterValues = SearchHelperFunctions.FormFilterQuery(SearchConstants.ManagedPropertyAreaOfLaw, searchObject.Filters.AOLList);
                        keywordQuery.RefinementFilters.Add(filterValues);
                    }

                    if (null != searchObject.Filters.PGList && 0 < searchObject.Filters.PGList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.PGList[0]))
                    {
                        filterValues = SearchHelperFunctions.FormFilterQuery(SearchConstants.ManagedPropertyPracticeGroup, searchObject.Filters.PGList);
                        keywordQuery.RefinementFilters.Add(filterValues);
                    }
                    keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.FromDate, searchObject.Filters.ToDate, SearchConstants.ManagedPropertyOpenDate);
                    if (null != searchObject.Filters.ClientsList && 0 < searchObject.Filters.ClientsList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.ClientsList[0]))
                    {
                        filterValues = SearchHelperFunctions.FormFilterQuery(SearchConstants.ManagedPropertyClientName, searchObject.Filters.ClientsList);
                        keywordQuery.RefinementFilters.Add(filterValues);
                    }
                }

                keywordQuery = FilterMattersUtility(searchObject, keywordQuery);

                keywordQuery = SearchHelperFunctions.FilterCommonDetails(searchObject, keywordQuery, true);
            }
            return keywordQuery;
        }

        /// <summary>
        /// Fires query on SharePoint Search and fills the result data.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <param name="searchObject">The search object.</param>
        /// <param name="isMatterSearch">The flag to determine weather call is from Search Matter or Search Document.</param>
        /// <param name="managedProperties">List of managed properties</param>
        /// <returns>It returns a string object, that contains all the results combined with dollar pipe dollar separator.</returns>
        internal static string FillResultData(ClientContext clientContext, KeywordQuery keywordQuery, SearchObject searchObject, Boolean isMatterSearch, List<string> managedProperties)
        {
            string result = string.Empty;
            Boolean isReadOnly;
            try
            {
                if (null != searchObject.Sort)
                {
                    keywordQuery.EnableSorting = true;
                    keywordQuery = SearchHelperFunctions.GetSortByProperty(keywordQuery, searchObject, isMatterSearch);
                }
                SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                ClientResult<ResultTableCollection> resultsTableCollection = searchExecutor.ExecuteQuery(keywordQuery);
                Users currentLoggedInUser = UIUtility.GetLoggedInUserDetails(clientContext);

                if (null != resultsTableCollection && null != resultsTableCollection.Value && 0 < resultsTableCollection.Value.Count && null != resultsTableCollection.Value[0].ResultRows)
                {
                    if (isMatterSearch && 0 < resultsTableCollection.Value.Count && null != resultsTableCollection.Value[0].ResultRows && (!string.IsNullOrWhiteSpace(currentLoggedInUser.Name) || !string.IsNullOrWhiteSpace(currentLoggedInUser.Email)))
                    {
                        foreach (IDictionary<string, object> matterMetadata in resultsTableCollection.Value[0].ResultRows)
                        {
                            isReadOnly = false;
                            if (null != matterMetadata)
                            {
                                // Decode matter properties
                                DecodeMatterProperties(matterMetadata);
                                string readOnlyUsers = Convert.ToString(matterMetadata[SearchConstants.ManagedPropertyBlockedUploadUsers], CultureInfo.InvariantCulture);
                                if (!string.IsNullOrWhiteSpace(readOnlyUsers))
                                {
                                    isReadOnly = IsUserReadOnlyForMatter(isReadOnly, currentLoggedInUser.Name, currentLoggedInUser.Email, readOnlyUsers);
                                }
                                matterMetadata.Add(TextConstants.IsReadOnlyUser, isReadOnly);
                            }
                        }
                    }
                    else
                    {
                        /*Keeping the code to clean the author values*/
                        foreach (IDictionary<string, object> documentMetadata in resultsTableCollection.Value[0].ResultRows)
                        {
                            if (null != documentMetadata)
                            {
                                string authorData = Convert.ToString(documentMetadata[SearchConstants.ManagedPropertyAuthor], CultureInfo.InvariantCulture);
                                int ltIndex = authorData.IndexOf(ConstantStrings.OpeningAngularBracket, StringComparison.Ordinal);
                                int gtIndex = authorData.IndexOf(ConstantStrings.ClosingAngularBracket, StringComparison.Ordinal);
                                authorData = (0 <= ltIndex && ltIndex < gtIndex) ? authorData.Remove(ltIndex, (gtIndex - ltIndex) + 1) : authorData;
                                authorData = authorData.Replace(ServiceConstantStrings.DoubleQuotes, string.Empty);
                                documentMetadata[SearchConstants.ManagedPropertyAuthor] = authorData.Trim();
                            }
                        }
                    }
                    if (1 < resultsTableCollection.Value.Count)
                    {
                        result = string.Concat(JsonConvert.SerializeObject(resultsTableCollection.Value[0].ResultRows), ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR, resultsTableCollection.Value[0].TotalRows, ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR, JsonConvert.SerializeObject(resultsTableCollection.Value[1].ResultRows));
                    }
                    else
                    {
                        if (0 == resultsTableCollection.Value[0].TotalRows)
                        {
                            result = SearchHelperFunctions.NoDataRow(managedProperties);
                        }
                        else
                        {
                            result = string.Concat(JsonConvert.SerializeObject(resultsTableCollection.Value[0].ResultRows), ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR, resultsTableCollection.Value[0].TotalRows);
                        }
                    }
                }
                else
                {
                    result = SearchHelperFunctions.NoDataRow(managedProperties);
                }
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Function to return no data row
        /// </summary>
        /// <param name="managedProperties">Managed properties information</param>
        /// <returns>No data row</returns>
        internal static string NoDataRow(List<string> managedProperties)
        {

            Dictionary<string, string> noDataObject = new Dictionary<string, string>();
            managedProperties.Add(ServiceConstantStrings.PathFieldName);
            foreach (string managedProperty in managedProperties)
            {
                if (!noDataObject.ContainsKey(managedProperty))
                {
                    noDataObject.Add(managedProperty, string.Empty);
                }
            }

            string result = string.Concat(ServiceConstantStrings.OpenSquareBrace, JsonConvert.SerializeObject(noDataObject), ServiceConstantStrings.CloseSquareBrace, ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR, 0);

            return result;

        }

        /// <summary>
        /// Checks if logged-in user has read permission on matter.
        /// </summary>
        /// <param name="isReadOnly">Flag indicating if user has read permission on matter</param>
        /// <param name="currentLoggedInUser">Current logged-in user name</param>
        /// <param name="currentLoggedInUserEmail">Current logged-in user email</param>
        /// <param name="readOnlyUsers">List of read only user for matter</param>
        /// <returns>Flag indicating if user has read permission on matter</returns>
        private static bool IsUserReadOnlyForMatter(Boolean isReadOnly, string currentLoggedInUser, string currentLoggedInUserEmail, string readOnlyUsers)
        {
            List<string> readOnlyUsersList = readOnlyUsers.Trim().Split(new string[] { ConstantStrings.Semicolon }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> currentReadOnlyUser = (from readOnlyUser in readOnlyUsersList
                                                where string.Equals(readOnlyUser.Trim(), currentLoggedInUser.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(readOnlyUser.Trim(), currentLoggedInUserEmail.Trim(), StringComparison.OrdinalIgnoreCase)
                                                select readOnlyUser).ToList();
            if (null != currentReadOnlyUser && 0 < currentReadOnlyUser.Count)
            {
                isReadOnly = true;
            }
            return isReadOnly;
        }

        /// <summary>
        /// Decodes matter properties before sending them to UI
        /// </summary>
        /// <param name="matterMetadata">Dictionary object contains matter meta data</param>
        internal static void DecodeMatterProperties(IDictionary<string, object> matterMetadata)
        {

            // Decode matter properties
            matterMetadata[SearchConstants.ManagedPropertyTitle] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyTitle]);
            matterMetadata[SearchConstants.ManagedPropertySiteName] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertySiteName]);
            matterMetadata[SearchConstants.ManagedPropertyDescription] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyDescription]);
            matterMetadata[SearchConstants.ManagedPropertyPracticeGroup] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyPracticeGroup]);
            matterMetadata[SearchConstants.ManagedPropertyAreaOfLaw] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyAreaOfLaw]);
            matterMetadata[SearchConstants.ManagedPropertySubAreaOfLaw] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertySubAreaOfLaw]);
            matterMetadata[SearchConstants.ManagedPropertyCustomTitle] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyCustomTitle]);
            matterMetadata[SearchConstants.ManagedPropertyPath] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyPath]);
            matterMetadata[SearchConstants.ManagedPropertyMatterName] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyMatterName]);
            matterMetadata[SearchConstants.ManagedPropertyOpenDate] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyOpenDate]);
            matterMetadata[SearchConstants.ManagedPropertyClientName] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyClientName]);
            matterMetadata[SearchConstants.ManagedPropertyBlockedUploadUsers] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyBlockedUploadUsers]);
            matterMetadata[SearchConstants.ManagedPropertyResponsibleAttorney] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyResponsibleAttorney]);
            matterMetadata[SearchConstants.ManagedPropertyClientID] = DecodeValues(matterMetadata[SearchConstants.ManagedPropertyClientID]);
        }

        /// <summary>
        /// Provides the required matter properties
        /// </summary>
        /// <param name="value">Matter Properties object</param>
        /// <returns>Decoded String</returns>
        internal static string DecodeValues(object value)
        {
            return null != value ? HttpUtility.HtmlDecode(Convert.ToString(value, CultureInfo.InvariantCulture)) : string.Empty;
        }

        /// <summary>
        /// Encodes the pinned user details
        /// </summary>
        /// <param name="value">Matter properties</param>
        /// <returns>Encoded String</returns>
        public static string EncodeValues(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? Encoder.HtmlEncode(value.Trim()) : string.Empty;
        }

        /// <summary>
        /// Populate meta-data and perform pin operation for matter or document based upon the passed object.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matterData">Matter object containing Matter data</param>
        /// <param name="documentData">Document object containing Document data</param>
        /// <returns>Status of update</returns>
        internal static string PopulateMetadeta(RequestObject requestObject, Client client, MatterData matterData, DocumentData documentData)
        {
            string status = string.Empty;
            bool isMatterCall = null != matterData ? true : false;

            try
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                {
                    status = isMatterCall ? SearchHelperFunctions.PinThisRecord(clientContext, matterData, true) : SearchHelperFunctions.PinThisRecord(clientContext, documentData, false);
                }
            }
            catch (Exception exception)
            {
                status = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return status;
        }

        /// <summary>
        /// Removes pinned item from user pinned details.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matterData">Matter object containing Matter data</param>
        /// <param name="documentData">Document object containing Document data</param>
        /// <returns>Status of update</returns>
        internal static string UnpinItem(RequestObject requestObject, Client client, MatterData matterData, DocumentData documentData)
        {
            string status = string.Empty;
            bool isMatterCall = null != matterData ? true : false;
            try
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                {
                    if (isMatterCall)
                    {
                        status = SearchHelperFunctions.RemoveThisRecord(clientContext, matterData, true);
                    }
                    else
                    {
                        status = SearchHelperFunctions.RemoveThisRecord(clientContext, documentData, false);
                    }
                }
            }
            catch (Exception exception)
            {
                status = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return status;
        }

        /// <summary>
        /// Encodes search results before saving to the list.
        /// </summary>
        /// <param name="searchDetails">SavedSearchDetails object containing Current search details</param>
        /// <param name="isMatterSearch">Flag for matter or document search (true = matter, false = document)</param>
        internal static void EncodeSearchDetailsUtility(FilterObject searchDetails, Boolean isMatterSearch)
        {
            if (isMatterSearch && null != searchDetails.Name) // Encode name only in case of Matter search
            {
                searchDetails.Name = Encoder.HtmlEncode(searchDetails.Name);
            }

            if (null != searchDetails.ResponsibleAttorneys)
            {
                searchDetails.ResponsibleAttorneys = Encoder.HtmlEncode(searchDetails.ResponsibleAttorneys);
            }

            if (null != searchDetails.SubareaOfLaw)
            {
                searchDetails.SubareaOfLaw = Encoder.HtmlEncode(searchDetails.SubareaOfLaw);
            }
        }

        /// <summary>
        /// Encodes search results before saving to the list.
        /// </summary>
        /// <param name="searchDetails">SavedSearchDetails object containing Current search details</param>
        /// <param name="isMatterSearch">Flag for matter or document search (true = matter, false = document)</param>
        internal static void EncodeSearchDetails(FilterObject searchDetails, Boolean isMatterSearch)
        {
            // Encode all the values which are coming from the JS file
            searchDetails.FromDate = (null != searchDetails.FromDate) ? Encoder.HtmlEncode(searchDetails.FromDate) : string.Empty;
            searchDetails.ToDate = (null != searchDetails.ToDate) ? Encoder.HtmlEncode(searchDetails.ToDate) : string.Empty;

            if (null != searchDetails.AOLList)
            {
                IList<string> encodedAOLList = new List<string>();
                foreach (string aolList in searchDetails.AOLList)
                {
                    if (!string.IsNullOrWhiteSpace(aolList))
                    {
                        encodedAOLList.Add(Encoder.HtmlEncode(aolList));
                    }
                }
                searchDetails.AOLList = encodedAOLList;
            }

            if (null != searchDetails.PGList)
            {
                IList<string> encodedPGList = new List<string>();
                foreach (string pgList in searchDetails.PGList)
                {
                    if (!string.IsNullOrWhiteSpace(pgList))
                    {
                        encodedPGList.Add(Encoder.HtmlEncode(pgList));
                    }
                }
                searchDetails.PGList = encodedPGList;
            }


            if (null != searchDetails.ClientsList)
            {
                IList<string> encodedClientsList = new List<string>();
                foreach (string clientsList in searchDetails.ClientsList)
                {
                    if (!string.IsNullOrWhiteSpace(clientsList))
                    {
                        encodedClientsList.Add(Encoder.HtmlEncode(clientsList));
                    }
                }
                searchDetails.ClientsList = encodedClientsList;
            }

            EncodeSearchDetailsUtility(searchDetails, isMatterSearch);
        }

        /// <summary>
        /// Checks if the requested page exists or not.
        /// </summary>
        /// <param name="requestedUrl">URL of the page, for which check is to be performed</param>
        /// <param name="clientContext">ClientContext for SharePoint</param>
        /// <returns>true or false string based upon the existence of the page, referred in requestedUrl</returns>
        internal static string PageExists(string requestedUrl, ClientContext clientContext)
        {
            string pageExists = ConstantStrings.FALSE;
            try
            {
                string[] requestedUrls = requestedUrl.Split(new string[] { ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries);
                if (1 < requestedUrls.Length)
                {
                    foreach (string url in requestedUrls)
                    {
                        if (Page.IsFileExists(clientContext, url))
                        {
                            pageExists = ConstantStrings.TRUE + ConstantStrings.DOLLAR + ConstantStrings.SymbolHash + ConstantStrings.DOLLAR + url;
                            break;
                        }
                    }
                }
                else
                {
                    pageExists = Page.IsFileExists(clientContext, requestedUrl) ? ConstantStrings.TRUE : ConstantStrings.FALSE;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return pageExists;
        }

        /// <summary>
        /// Fetches contextual help content from SPList.
        /// </summary>
        /// <param name="result">String object which stores json object in string format</param>
        /// <param name="selectedSectionIDs">String object which contains selected section id</param>
        /// <param name="sectionID">Collections of section id for contextual help functionality</param>
        /// <param name="contextHelpCollection">Collection of ContextHelpData</param>
        /// <param name="clientContext">ClientContext for SharePoint</param>
        /// <param name="contextualHelpSectionListItems">List collection object for contextual help section list</param>
        /// <returns></returns>
        internal static ListItemCollection FetchContextualHelpContentUtility(ref string result, ref string selectedSectionIDs, IList<string> sectionID, ref List<ContextHelpData> contextHelpCollection, ClientContext clientContext, ListItemCollection contextualHelpSectionListItems)
        {
            string[] contextualHelpLinksQueryParts = ServiceConstantStrings.ContextualHelpQueryIncludeOrCondition.Split(';');
            ListItemCollection contextualHelpLinksListItems;
            foreach (ListItem oListItem in contextualHelpSectionListItems)
            {
                // Retrieve and save content from MatterCenterHelpSectionList
                sectionID.Add(Convert.ToString(oListItem[ServiceConstantStrings.ContextualHelpSectionColumnSectionID], CultureInfo.InvariantCulture));
            }

            // Using section ids, create caml query which will retrieve links from MatterCenterHelpLinksList
            for (int index = 0; index < sectionID.Count; index++)
            {
                if (2 > index)
                {
                    selectedSectionIDs = string.Concat(selectedSectionIDs, String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[0], ServiceConstantStrings.ContextualHelpSectionColumnSectionID, sectionID[index]));
                }
                else
                {
                    selectedSectionIDs = String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[1], selectedSectionIDs);
                    selectedSectionIDs = string.Concat(selectedSectionIDs, String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[0], ServiceConstantStrings.ContextualHelpSectionColumnSectionID, sectionID[index]));
                }
            }
            if (1 < sectionID.Count)
            {
                selectedSectionIDs = String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[1], selectedSectionIDs);
            }

            // get Contextual Help links form MatterCenterHelpLinksList
            contextualHelpLinksListItems = Lists.GetData(clientContext, ServiceConstantStrings.MatterCenterHelpLinksListName, String.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.RetrieveContextualHelpLinksQuery, selectedSectionIDs));
            //If these exists any links for contextual help flyout
            if (null != contextualHelpLinksListItems && 0 < contextualHelpLinksListItems.Count)
            {
                foreach (ListItem oListItem in contextualHelpLinksListItems)
                {
                    foreach (ListItem oListItemHelpSection in contextualHelpSectionListItems)
                    {
                        if (Convert.ToString(oListItemHelpSection[ServiceConstantStrings.ContextualHelpSectionColumnSectionID], CultureInfo.InvariantCulture) == ((Microsoft.SharePoint.Client.FieldLookupValue)(oListItem[ServiceConstantStrings.ContextualHelpLinksColumnSectionID])).LookupValue)
                        {
                            string currentLinkOrder = Convert.ToString(oListItem[ServiceConstantStrings.ContextualHelpLinksColumnLinkOrder], CultureInfo.InvariantCulture);
                            string currentLinkTitle = Convert.ToString(oListItem[ServiceConstantStrings.ContextualHelpLinksColumnLinkTitle], CultureInfo.InvariantCulture);
                            string currentLinkUrl = ((Microsoft.SharePoint.Client.FieldUrlValue)oListItem[ServiceConstantStrings.ContextualHelpLinksColumnLinkURL]).Url;
                            string currentPageName = Convert.ToString(oListItemHelpSection[ServiceConstantStrings.ContextualHelpSectionColumnPageName], CultureInfo.InvariantCulture);
                            string numberOfColumns = Convert.ToString(oListItemHelpSection[ServiceConstantStrings.ContextualHelpSectionColumnNumberOfColumns], CultureInfo.InvariantCulture);

                            ContextHelpData contextData = new ContextHelpData
                            {
                                ContextSection = new ContextHelpSection
                                {
                                    SectionID = Convert.ToString(oListItemHelpSection[ServiceConstantStrings.ContextualHelpSectionColumnSectionID], CultureInfo.InvariantCulture),
                                    SectionTitle = Convert.ToString(oListItemHelpSection[ServiceConstantStrings.ContextualHelpSectionColumnSectionTitle], CultureInfo.InvariantCulture),
                                    SectionOrder = Convert.ToString(oListItemHelpSection[ServiceConstantStrings.ContextualHelpSectionColumnSectionOrder], CultureInfo.InvariantCulture),
                                    PageName = currentPageName,
                                    NumberOfColumns = numberOfColumns
                                },
                                LinkOrder = currentLinkOrder,
                                LinkTitle = currentLinkTitle,
                                LinkURL = currentLinkUrl
                            };
                            contextHelpCollection.Add(contextData);
                        }
                    }
                }
            }

            contextHelpCollection = contextHelpCollection.OrderBy(c => c.ContextSection.SectionOrder).ThenBy(c => c.LinkOrder).ToList();

            // Serialize the object
            result = JsonConvert.SerializeObject(contextHelpCollection);
            return contextualHelpLinksListItems;
        }
    }
}