// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-lapedd
// Created          : 04-09-2016
//***************************************************************************
// <copyright file="Search.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to perform SharePoint search functionalities</summary>
// ***********************************************************************

using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This class contains all the methods which are related to spo search
    /// </summary>
    public class Search:ISearch
    {
        private GeneralSettings generalSettings;
        private SearchSettings searchSettings;
        private ISPOAuthorization spoAuthorization;
        private ClientContext clientContext;
        private IUsersDetails userDetails;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private ISPList spList;
        private CamlQueries camlQueries;
        private ListNames listNames;
        private SharedSettings sharedSettings;
        private ErrorSettings errorSettings;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spoAuthorization"></param>
        /// <param name="generalSettings"></param>
        /// <param name="searchSettings"></param>
        public Search(ISPOAuthorization spoAuthorization,             
            ICustomLogger customLogger,            
            IUsersDetails userDetails,
            ISPList spList,
            IOptionsMonitor<GeneralSettings> generalSettings,
            IOptionsMonitor<SharedSettings> sharedSettings,
            IOptionsMonitor<LogTables> logTables,
            IOptionsMonitor<SearchSettings> searchSettings,
            IOptionsMonitor<CamlQueries> camlQueries, 
            IOptionsMonitor<ListNames> listNames,
            IOptionsMonitor<ErrorSettings> errorSettings)
        {
            this.spoAuthorization = spoAuthorization;
            this.generalSettings = generalSettings.CurrentValue;
            this.searchSettings = searchSettings.CurrentValue;
            this.userDetails = userDetails;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
            this.spList = spList;
            this.camlQueries = camlQueries.CurrentValue;
            this.listNames = listNames.CurrentValue;
            this.sharedSettings = sharedSettings.CurrentValue;
            this.errorSettings = errorSettings.CurrentValue;
        }

        #region Public Methods

        /// <summary>
        /// Gets the matters based on search criteria.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public SearchResponseVM GetMatters(SearchRequestVM searchRequestVM)
        {
            SearchResponseVM searchResponseVM = null;
            var client = searchRequestVM.Client;
            var searchObject = searchRequestVM.SearchObject;
            try
            {
                clientContext = null;
                clientContext = spoAuthorization.GetClientContext(client.Url);                
                KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                if (string.IsNullOrWhiteSpace(searchObject.SearchTerm))
                {
                    searchObject.SearchTerm = ServiceConstants.ASTERISK;
                }

                if (searchObject.Filters != null)
                {
                    if (searchObject.Filters.FilterByMe == 1)
                    {
                            
                        Users currentUserDetail = userDetails.GetLoggedInUserDetails(clientContext);
                        string userTitle = currentUserDetail.Name;
                        searchObject.SearchTerm = string.Concat(searchObject.SearchTerm, ServiceConstants.SPACE, 
                            ServiceConstants.OPERATOR_AND, ServiceConstants.SPACE, 
                            ServiceConstants.OPENING_BRACKET, searchSettings.ManagedPropertyResponsibleAttorney, 
                            ServiceConstants.COLON, ServiceConstants.SPACE, ServiceConstants.DOUBLE_QUOTE, userTitle, 
                            ServiceConstants.DOUBLE_QUOTE, ServiceConstants.SPACE, ServiceConstants.OPERATOR_AND, ServiceConstants.SPACE, 
                            searchSettings.ManagedPropertyTeamMembers, ServiceConstants.COLON, ServiceConstants.SPACE, 
                            ServiceConstants.DOUBLE_QUOTE, userTitle,
                            ServiceConstants.DOUBLE_QUOTE, ServiceConstants.SPACE, ServiceConstants.CLOSING_BRACKET);
                    }

                    keywordQuery = FilterMatters(searchObject, keywordQuery);

                    keywordQuery = KeywordQueryMetrics(client, searchObject, keywordQuery, 
                        ServiceConstants.DOCUMENT_LIBRARY_FILTER_CONDITION, 
                        searchSettings.ManagedPropertyIsMatter, true);

                    // Create a list of managed properties which are required to be present in search results
                    List<string> managedProperties = new List<string>();
                    managedProperties.Add(searchSettings.ManagedPropertyTitle);
                    managedProperties.Add(searchSettings.ManagedPropertyName);
                    managedProperties.Add(searchSettings.ManagedPropertyDescription);
                    managedProperties.Add(searchSettings.ManagedPropertySiteName);
                    managedProperties.Add(searchSettings.ManagedPropertyLastModifiedTime);
                    managedProperties.Add(searchSettings.ManagedPropertyPracticeGroup);
                    managedProperties.Add(searchSettings.ManagedPropertyAreaOfLaw);
                    managedProperties.Add(searchSettings.ManagedPropertySubAreaOfLaw);
                    managedProperties.Add(searchSettings.ManagedPropertyMatterId);
                    managedProperties.Add(searchSettings.ManagedPropertyCustomTitle);
                    managedProperties.Add(searchSettings.ManagedPropertyPath);
                    managedProperties.Add(searchSettings.ManagedPropertyMatterName);
                    managedProperties.Add(searchSettings.ManagedPropertyOpenDate);
                    managedProperties.Add(searchSettings.ManagedPropertyClientName);
                    managedProperties.Add(searchSettings.ManagedPropertyBlockedUploadUsers);
                    managedProperties.Add(searchSettings.ManagedPropertyResponsibleAttorney);
                    managedProperties.Add(searchSettings.ManagedPropertyClientID);
                    managedProperties.Add(searchSettings.ManagedPropertyMatterGuid);
                    //Filter on Result source to fetch only Matter Center specific results
                    keywordQuery.SourceId = new Guid(searchSettings.SearchResultSourceID);
                    keywordQuery = AssignKeywordQueryValues(keywordQuery, managedProperties);
                    keywordQuery.BypassResultTypes = true;
                    searchResponseVM = FillResultData(clientContext, keywordQuery, searchRequestVM, true, managedProperties);
                }
                clientContext.Dispose();
                return searchResponseVM;
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            
        }        

        /// <summary>
        /// Gets the matters based on search criteria.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public SearchResponseVM GetDocuments(SearchRequestVM searchRequestVM)
        {
            SearchResponseVM searchResponseVM = null;
            try
            {                
                var client = searchRequestVM.Client;
                var searchObject = searchRequestVM.SearchObject;
                clientContext = null;
                clientContext = spoAuthorization.GetClientContext(client.Url);                
                KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                if (string.IsNullOrWhiteSpace(searchObject.SearchTerm))
                {
                    searchObject.SearchTerm = ServiceConstants.ASTERISK;
                }

                if (searchObject.Filters != null)
                {
                    if (searchObject.Filters.FilterByMe == 1)
                    {
                        ////Get logged in user alias
                        Users currentUserDetail = userDetails.GetLoggedInUserDetails(clientContext);
                        string userTitle = currentUserDetail.Name;
                        searchObject.SearchTerm = string.Concat(searchObject.SearchTerm, ServiceConstants.SPACE, ServiceConstants.OPERATOR_AND,
                            ServiceConstants.SPACE, ServiceConstants.OPENING_BRACKET, searchSettings.ManagedPropertyResponsibleAttorney,
                            ServiceConstants.COLON, ServiceConstants.SPACE, ServiceConstants.DOUBLE_QUOTE, userTitle, ServiceConstants.DOUBLE_QUOTE,
                            ServiceConstants.SPACE, ServiceConstants.OPERATOR_OR, ServiceConstants.SPACE, searchSettings.ManagedPropertyTeamMembers,
                            ServiceConstants.COLON, ServiceConstants.SPACE, ServiceConstants.DOUBLE_QUOTE, userTitle, ServiceConstants.DOUBLE_QUOTE,
                            ServiceConstants.SPACE, ServiceConstants.CLOSING_BRACKET);
                    }

                    keywordQuery = FilterDocuments(searchObject, keywordQuery);

                    keywordQuery = KeywordQueryMetrics(client, searchObject, keywordQuery, ServiceConstants.DOCUMENT_ITEM_FILTER_CONDITION, 
                        searchSettings.ManagedPropertyIsDocument, false);

                    // Create a list of managed properties which are required to be present in search results
                    List<string> managedProperties = new List<string>();
                    managedProperties.Add(searchSettings.ManagedPropertyFileName);
                    managedProperties.Add(searchSettings.ManagedPropertyTitle);
                    managedProperties.Add(searchSettings.ManagedPropertyCreated);
                    managedProperties.Add(searchSettings.ManagedPropertyUIVersionStringOWSTEXT);
                    managedProperties.Add(searchSettings.ManagedPropertyServerRelativeUrl);
                    managedProperties.Add(searchSettings.ManagedPropertyFileExtension);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentMatterId);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentLastModifiedTime);
                    managedProperties.Add(searchSettings.ManagedPropertySiteTitle);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentClientId);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentClientName);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentMatterName);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentId);
                    managedProperties.Add(searchSettings.ManagedPropertyCheckOutByUser);
                    managedProperties.Add(searchSettings.ManagedPropertySiteName);
                    managedProperties.Add(searchSettings.ManagedPropertySPWebUrl);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentVersion);
                    managedProperties.Add(searchSettings.ManagedPropertyDocumentCheckOutUser);
                    managedProperties.Add(searchSettings.ManagedPropertySPWebUrl);
                    managedProperties.Add(searchSettings.ManagedPropertyAuthor);
                    //Filter on Result source to fetch only Matter Center specific results
                    keywordQuery.SourceId = new Guid(searchSettings.SearchResultSourceID);
                    keywordQuery = AssignKeywordQueryValues(keywordQuery, managedProperties);
                        
                    searchResponseVM = FillResultData(clientContext, keywordQuery, searchRequestVM, false, managedProperties);
                    clientContext.Dispose();
                }               
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return searchResponseVM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="listName"></param>
        /// <param name="listColumnName"></param>
        /// <param name="isShowDocument"></param>
        /// <returns></returns>
        public SearchResponseVM GetPinnedData(Client client, string listName, string listColumnName, bool isShowDocument)
        {
            ////Holds logged-in user alias
            string userAlias = string.Empty;
            ////Object to store all the list items retrieved from SharePoint list
            ListItemCollection listItems;
            ////Stores the JSON structure with the meta-data of pinned matter/document
            string userPinnedDetails = string.Empty;

            SearchResponseVM searchResponse = new SearchResponseVM();
            using (clientContext = spoAuthorization.GetClientContext(client.Url))
            {
                try
                {
                    ////Get logged in user alias
                    Users currentUserDetail = userDetails.GetLoggedInUserDetails(clientContext);
                    userAlias = currentUserDetail.LogOnName;
                    listItems = spList.GetData(clientContext, listName, string.Format(CultureInfo.InvariantCulture,
                        camlQueries.UserPinnedDetailsQuery, searchSettings.PinnedListColumnUserAlias, userAlias, listColumnName));                    
                    if (listItems!=null && listItems.Count > 0)
                    {
                        ////Since we are maintaining only single list item per user, listItems collection will have only one object; hence accessing first object
                        ////Check if column holds null or empty string. If non empty, pinned matter/document exists
                        if (!string.IsNullOrEmpty(Convert.ToString(listItems[0][listColumnName], CultureInfo.InvariantCulture)))
                        {
                            string userPinnedMatter = Convert.ToString(listItems[0][listColumnName], CultureInfo.InvariantCulture);
                            if (isShowDocument)
                            {
                                Dictionary<string, DocumentData> userpinnedDocumentCollection = 
                                    JsonConvert.DeserializeObject<Dictionary<string, DocumentData>>(userPinnedMatter);
                                searchResponse.TotalRows = userpinnedDocumentCollection.Count;
                                searchResponse.DocumentDataList = userpinnedDocumentCollection.Values.Reverse();                                
                            }
                            else
                            {
                                Dictionary<string, MatterData> userpinnedMatterCollection = 
                                    JsonConvert.DeserializeObject<Dictionary<string, MatterData>>(userPinnedMatter);
                                searchResponse.TotalRows = userpinnedMatterCollection.Count;
                                searchResponse.MatterDataList = userpinnedMatterCollection.Values.Reverse();                                
                            }                            
                        }                        
                    }
                    else
                    {
                        searchResponse.TotalRows = 0;
                        searchResponse.NoPinnedMessage = ServiceConstants.NO_PINNED_MESSAGE;
                    }
                    return searchResponse;
                }
                catch (Exception ex)
                {
                    customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes pinned item from user pinned details.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matterData">Matter object containing Matter data</param>
        /// 
        /// <param name="documentData">Document object containing Document data</param>
        /// <returns>Status of update</returns>
        public bool UnPinMatter(PinRequestMatterVM pinRequestMatterVM)
        {
            try
            {
                clientContext = spoAuthorization.GetClientContext(pinRequestMatterVM.Client.Url);
                return UnPinThisRecord(clientContext, pinRequestMatterVM.Client, pinRequestMatterVM.MatterData, true);                
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Add a new pin item to the user pinned matter list
        /// </summary>
        /// <param name="pinRequestMatterVM"></param>
        /// <returns></returns>
        public bool PinMatter(PinRequestMatterVM pinRequestMatterVM)
        {            
            try
            {
                using (clientContext = spoAuthorization.GetClientContext(pinRequestMatterVM.Client.Url))
                {
                    return PinThisRecord(clientContext, pinRequestMatterVM.Client, pinRequestMatterVM.MatterData, true);
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            
        }

        /// <summary>
        /// Removes pinned item from user pinned document list.
        /// </summary>
        /// <param name="pinRequestDocumentVM"></param>
        /// <returns></returns>
        public bool UnPinDocument(PinRequestDocumentVM pinRequestDocumentVM)
        {
            try
            {
                using (clientContext = spoAuthorization.GetClientContext(pinRequestDocumentVM.Client.Url))
                {
                    return UnPinThisRecord(clientContext, pinRequestDocumentVM.Client, pinRequestDocumentVM.DocumentData, false);
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Add a new pin document to the user pinned document list
        /// </summary>
        /// <param name="pinRequestDocumentVM"></param>
        /// <returns></returns>
        public bool PinDocument(PinRequestDocumentVM pinRequestDocumentVM)
        {
            try
            {
                using (clientContext = spoAuthorization.GetClientContext(pinRequestDocumentVM.Client.Url))
                {
                    return PinThisRecord(clientContext, pinRequestDocumentVM.Client, pinRequestDocumentVM.DocumentData, false);
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="selectedPage"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public List<ContextHelpData> GetMatterHelp(Client client, string selectedPage, string listName)
        {
            try
            {
                List<ContextHelpData> contextHelpData = null;
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    //Object to store all the list items retrieved from SharePoint list
                    ListItemCollection contextualHelpSectionListItems;

                    // Get MatterCenterHelpSection list data
                    contextualHelpSectionListItems = spList.GetData(clientContext, listName,
                        String.Format(CultureInfo.InvariantCulture, camlQueries.RetrieveContextualHelpSectionsQuery, selectedPage));
                    //If these exists any content for contextual help flyout
                    if (null != contextualHelpSectionListItems && 0 < contextualHelpSectionListItems.Count)
                    {
                        contextHelpData = FetchContextualHelpContentUtility(clientContext, contextualHelpSectionListItems);
                    }
                }
                return contextHelpData;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
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
        public List<ContextHelpData> FetchContextualHelpContentUtility(ClientContext clientContext, ListItemCollection contextualHelpSectionListItems)
        {
            try
            {
                string[] contextualHelpLinksQueryParts = camlQueries.ContextualHelpQueryIncludeOrCondition.Split(';');
                IList<string> sectionID = new List<string>();
                string selectedSectionIDs = string.Empty;
                ListItemCollection contextualHelpLinksListItems;
                List<ContextHelpData> contextHelpCollection = new List<ContextHelpData>();
                foreach (ListItem oListItem in contextualHelpSectionListItems)
                {
                    // Retrieve and save content from MatterCenterHelpSectionList
                    sectionID.Add(Convert.ToString(oListItem[sharedSettings.ContextualHelpSectionColumnSectionID], CultureInfo.InvariantCulture));
                }

                // Using section ids, create caml query which will retrieve links from MatterCenterHelpLinksList
                for (int index = 0; index < sectionID.Count; index++)
                {
                    if (index < 2)
                    {
                        selectedSectionIDs = string.Concat(selectedSectionIDs, String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[0],
                            sharedSettings.ContextualHelpSectionColumnSectionID, sectionID[index]));
                    }
                    else
                    {
                        selectedSectionIDs = String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[1], selectedSectionIDs);
                        selectedSectionIDs = string.Concat(selectedSectionIDs, String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[0],
                            sharedSettings.ContextualHelpSectionColumnSectionID, sectionID[index]));
                    }
                }
                if (sectionID.Count > 1)
                {
                    selectedSectionIDs = String.Format(CultureInfo.InvariantCulture, contextualHelpLinksQueryParts[1], selectedSectionIDs);
                }

                // get Contextual Help links form MatterCenterHelpLinksList
                contextualHelpLinksListItems = spList.GetData(clientContext, listNames.MatterCenterHelpLinksListName,
                    String.Format(CultureInfo.InvariantCulture, camlQueries.RetrieveContextualHelpLinksQuery, selectedSectionIDs));
                //If these exists any links for contextual help flyout
                if (null != contextualHelpLinksListItems && 0 < contextualHelpLinksListItems.Count)
                {
                    foreach (ListItem oListItem in contextualHelpLinksListItems)
                    {
                        foreach (ListItem oListItemHelpSection in contextualHelpSectionListItems)
                        {
                            if (Convert.ToString(oListItemHelpSection[sharedSettings.ContextualHelpSectionColumnSectionID], CultureInfo.InvariantCulture) ==
                                ((Microsoft.SharePoint.Client.FieldLookupValue)(oListItem[sharedSettings.ContextualHelpLinksColumnSectionID])).LookupValue)
                            {
                                string currentLinkOrder = Convert.ToString(oListItem[sharedSettings.ContextualHelpLinksColumnLinkOrder], CultureInfo.InvariantCulture);
                                string currentLinkTitle = Convert.ToString(oListItem[sharedSettings.ContextualHelpLinksColumnLinkTitle], CultureInfo.InvariantCulture);
                                string currentLinkUrl = ((Microsoft.SharePoint.Client.FieldUrlValue)oListItem[sharedSettings.ContextualHelpLinksColumnLinkURL]).Url;
                                string currentPageName = Convert.ToString(oListItemHelpSection[sharedSettings.ContextualHelpSectionColumnPageName], CultureInfo.InvariantCulture);
                                string numberOfColumns = Convert.ToString(oListItemHelpSection[sharedSettings.ContextualHelpSectionColumnNumberOfColumns], CultureInfo.InvariantCulture);

                                ContextHelpData contextData = new ContextHelpData
                                {
                                    ContextSection = new ContextHelpSection
                                    {
                                        SectionID = Convert.ToString(oListItemHelpSection[sharedSettings.ContextualHelpSectionColumnSectionID], CultureInfo.InvariantCulture),
                                        SectionTitle = Convert.ToString(oListItemHelpSection[sharedSettings.ContextualHelpSectionColumnSectionTitle], CultureInfo.InvariantCulture),
                                        SectionOrder = Convert.ToString(oListItemHelpSection[sharedSettings.ContextualHelpSectionColumnSectionOrder], CultureInfo.InvariantCulture),
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
                return contextHelpCollection;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        public List<RoleDefinition> GetWebRoleDefinitions(Client client)
        {
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web.RoleDefinitions, roledefinitions => roledefinitions.Include(thisRole => thisRole.Name, thisRole => thisRole.Id));
                    clientContext.ExecuteQuery();
                    string userAllowedPermissions = searchSettings.UserPermissions;
                
                        List<RoleDefinition> roleDefinition = new List<RoleDefinition>();
                        if (!String.IsNullOrWhiteSpace(userAllowedPermissions))
                        {
                            //// Get the user permissions from the Resource file
                            List<string> userPermissions = userAllowedPermissions.ToUpperInvariant().Trim().Split(new string[] { ServiceConstants.COMMA }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //// Filter only the allowed roles using LINQ query
                            roleDefinition = (from webRole in web.RoleDefinitions.ToList()
                                                                   where userPermissions.Contains(webRole.Name.ToUpperInvariant())
                                                                   select webRole).ToList();
                        }
                        return roleDefinition;
                
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public IList<PeoplePickerUser> SearchUsers(SearchRequestVM searchRequestVM)
        {
            var client = searchRequestVM.Client;
            var searchObject = searchRequestVM.SearchObject;
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    
                    ClientPeoplePickerQueryParameters queryParams = new ClientPeoplePickerQueryParameters();
                    queryParams.AllowMultipleEntities = searchSettings.PeoplePickerAllowMultipleEntities;
                    queryParams.MaximumEntitySuggestions = searchSettings.PeoplePickerMaximumEntitySuggestions;
                    queryParams.PrincipalSource = PrincipalSource.All;
                    queryParams.PrincipalType = PrincipalType.User | PrincipalType.SecurityGroup;
                    queryParams.QueryString = searchObject.SearchTerm;
                    int peoplePickerMaxRecords = searchSettings.PeoplePickerMaxRecords;

                    ClientResult<string> clientResult = ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(clientContext, queryParams);
                    clientContext.ExecuteQuery();
                    string results = clientResult.Value;
                    IList<PeoplePickerUser> foundUsers = JsonConvert.DeserializeObject<List<PeoplePickerUser>>(results).Where(result => (string.Equals(result.EntityType, ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER,
                        StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.Description)) || (!string.Equals(result.EntityType,
                        ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.EntityData.Email))).Take(peoplePickerMaxRecords).ToList();
                    return foundUsers;
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public GenericResponseVM GetConfigurations(string siteCollectionUrl, string listName)
        {
            try
            {
                GenericResponseVM genericResponse = null;
                ListItem settingsItem = null;
                using (ClientContext clientContext = spoAuthorization.GetClientContext(siteCollectionUrl))
                {
                    if (spList.CheckPermissionOnList(clientContext, listName, PermissionKind.EditListItems))
                    {
                        string listQuery = string.Format(CultureInfo.InvariantCulture, camlQueries.MatterConfigurationsListQuery,
                            searchSettings.ManagedPropertyTitle, searchSettings.MatterConfigurationTitleValue);
                        settingsItem = spList.GetData(clientContext, listNames.MatterConfigurationsList, listQuery).FirstOrDefault();
                        if (settingsItem != null)
                        {
                            genericResponse = new GenericResponseVM();
                            genericResponse.Code = WebUtility.HtmlDecode(Convert.ToString(settingsItem[searchSettings.MatterConfigurationColumn]));
                            genericResponse.Value = Convert.ToString(settingsItem[searchSettings.ColumnNameModifiedDate], CultureInfo.InvariantCulture);
                            return genericResponse;
                        }
                        else
                        {
                            genericResponse = new GenericResponseVM();
                            genericResponse.Code = "0";
                            genericResponse.Value = string.Empty;
                            return genericResponse;
                        }
                    }
                    else
                    {
                        genericResponse = new GenericResponseVM();
                        genericResponse.Code = errorSettings.UserNotSiteOwnerCode;
                        genericResponse.Value = errorSettings.UserNotSiteOwnerMessage;
                        genericResponse.IsError = true;
                        return genericResponse;
                    }
                    
                }
               
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Pins the record and associate to logged-in user.
        /// </summary>
        /// <param name="clientContext">The client context object</param>
        /// <param name="getUserPinnedDetails">This is an object that contains the details of the specific pinned matter/document.</param>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <returns>It returns a string object, that contains the execution status of the PinThisRecord function.</returns>
        internal bool PinThisRecord(ClientContext clientContext, Client client, object getUserPinnedDetails, bool isMatterView)
        {
            bool status = false;
            if (clientContext!=null)
            {               
                string userAlias = string.Empty;
                string pinnedDetailsJson = string.Empty;
                ListItemCollection listItems;
                PinUnpinDetails userPinnedDetails = GetCurrentUserPinnedDetails(isMatterView, getUserPinnedDetails);
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(userPinnedDetails.ListName);
                    Users currentUserDetail = userDetails.GetLoggedInUserDetails(clientContext);
                    userAlias = currentUserDetail.LogOnName;
                    listItems = spList.GetData(clientContext, userPinnedDetails.ListName, string.Format(CultureInfo.InvariantCulture, 
                        camlQueries.UserPinnedDetailsQuery,
                        searchSettings.PinnedListColumnUserAlias, userAlias, userPinnedDetails.PinnedListColumnDetails));
                    ////Pinned matter/document(s) exists for users
                    if (null != listItems && 0 < listItems.Count)
                    {
                        ////Logic to create pinned matter/document
                        if (isMatterView)
                        {
                            string userPinnedMatter = !string.IsNullOrEmpty(Convert.ToString(listItems[0][searchSettings.PinnedListColumnMatterDetails], 
                                CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][searchSettings.PinnedListColumnMatterDetails], 
                                CultureInfo.InvariantCulture) : string.Empty;
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
                            string userPinnedDocument = !string.IsNullOrEmpty(Convert.ToString(listItems[0][searchSettings.PinnedListColumnDocumentDetails], 
                                CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][searchSettings.PinnedListColumnDocumentDetails], CultureInfo.InvariantCulture) : string.Empty;
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
                        listItem[searchSettings.PinnedListColumnUserAlias] = userAlias;
                        listItem[userPinnedDetails.PinnedListColumnDetails] = pinnedDetailsJson;
                        listItem.Update();
                        clientContext.ExecuteQuery();
                        listItem.BreakRoleInheritance(false, true);     // Remove inheriting permissions on item
                        clientContext.ExecuteQuery();
                        status = true;
                    }
                    return status;
                }
                catch (Exception exception)
                {
                    customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }            
            return status;
        }

        /// <summary>
        /// Gets the first pinned document serialized JSON object
        /// </summary>        
        /// <param name="userPinnedDetails">User document pin details object</param>
        /// <returns></returns>
        private string GetFirstPinnedDocument(PinUnpinDetails userPinnedDetails)
        {
            try
            {
                Dictionary<string, DocumentData> userFirstPinnedDocument = new Dictionary<string, DocumentData>();
                userFirstPinnedDocument.Add(userPinnedDetails.URL, userPinnedDetails.UserPinnedDocumentData);
                string pinnedDetailsJson = JsonConvert.SerializeObject(userFirstPinnedDocument, Newtonsoft.Json.Formatting.Indented);
                return pinnedDetailsJson;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Gets the first pinned matter serialized JSON object
        /// </summary>
        /// <param name="userPinnedDetails">User matter pin details object</param>
        /// <returns>JSON format of pin data</returns>
        private string GetFirstPinnedMatter(PinUnpinDetails userPinnedDetails)
        {
            try
            {
                Dictionary<string, MatterData> userFirstPinnedMatter = new Dictionary<string, MatterData>();
                userFirstPinnedMatter.Add(userPinnedDetails.UserPinnedMatterData.MatterUrl, userPinnedDetails.UserPinnedMatterData);
                string pinnedDetailsJson = JsonConvert.SerializeObject(userFirstPinnedMatter, Newtonsoft.Json.Formatting.Indented);
                return pinnedDetailsJson;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Removes the record and dissociate from logged-in user.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="getUserPinnedDetails">This is an object that contains the details of the specific pinned matter/document.</param>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <returns>It returns a string object, that contains the execution status of the function.</returns>
        internal bool UnPinThisRecord(ClientContext clientContext, Client client, object getUserPinnedDetails, bool isMatterView)
        {
            bool status = false;
            if (null != clientContext)
            {                
                string userAlias = string.Empty;
                ListItemCollection listItems;
                PinUnpinDetails userPinnedDetails = GetCurrentUserPinnedDetails(isMatterView, getUserPinnedDetails);
                try
                {
                    Users currentUserDetail = userDetails.GetLoggedInUserDetails(clientContext);
                    userAlias = currentUserDetail.LogOnName;
                    listItems = spList.GetData(clientContext, userPinnedDetails.ListName, string.Format(CultureInfo.InvariantCulture, 
                        camlQueries.UserPinnedDetailsQuery, searchSettings.PinnedListColumnUserAlias, 
                        userAlias, userPinnedDetails.PinnedListColumnDetails));

                    ////Pinned matter/document(s) exists for users
                    if (null != listItems && 0 < listItems.Count)
                    {
                        ////Logic to create pinned matter/document
                        if (isMatterView)
                        {
                            string userPinnedMatter = 
                                !string.IsNullOrEmpty(Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], CultureInfo.InvariantCulture)) ? 
                                Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], CultureInfo.InvariantCulture) : string.Empty;
                            Dictionary<string, MatterData> userpinnedMatterCollection = JsonConvert.DeserializeObject<Dictionary<string, MatterData>>(userPinnedMatter);

                            if (!string.IsNullOrWhiteSpace(userPinnedDetails.UserPinnedMatterData.MatterName) && 
                                userpinnedMatterCollection.ContainsKey(userPinnedDetails.UserPinnedMatterData.MatterName))
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
                                    string updatedMatter = JsonConvert.SerializeObject(userpinnedMatterCollection, Formatting.Indented);
                                    ////We are maintaining single list item entry for user
                                    listItems[0][searchSettings.PinnedListColumnMatterDetails] = updatedMatter;
                                    listItems[0].Update();
                                }
                            }
                        }
                        else
                        {
                            string userPinnedDocument = !string.IsNullOrEmpty(Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], 
                                CultureInfo.InvariantCulture)) ? Convert.ToString(listItems[0][userPinnedDetails.PinnedListColumnDetails], 
                                CultureInfo.InvariantCulture) : string.Empty;
                            Dictionary<string, DocumentData> userpinnedDocumentCollection = 
                                JsonConvert.DeserializeObject<Dictionary<string, DocumentData>>(userPinnedDocument);
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
                    return status;
                }
                catch (Exception exception)
                {
                    customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
            return status;            
        }

        /// <summary>
        /// Gets the current user pinned details.
        /// </summary>
        /// <param name="isMatterView">If the user is pinning a matter, this will be true, else will be false.</param>
        /// <param name="getUserPinnedDetails">This is an object that contains the details of the specific pinned matter/document.</param>
        /// <returns>This returns an object that contains the details of the specific pinned matter/document.</returns>
        internal PinUnpinDetails GetCurrentUserPinnedDetails(bool isMatterView, object getUserPinnedDetails)
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

                userPinnedDetails.ListName = isMatterView ? listNames.UserPinnedMatterListName : listNames.UserPinnedDocumentListName;
                userPinnedDetails.PinnedListColumnDetails = isMatterView ? searchSettings.PinnedListColumnMatterDetails : searchSettings.PinnedListColumnDocumentDetails;
                userPinnedDetails.URL = isMatterView ? userPinnedDetails.UserPinnedMatterData.MatterUrl : userPinnedDetails.UserPinnedDocumentData.DocumentUrl;
                return userPinnedDetails;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }            
        }


        /// <summary>
        /// Returns the query to filter the matters.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <returns>It returns a keyword query object.</returns>
        private KeywordQuery FilterMatters(SearchObject searchObject, KeywordQuery keywordQuery)
        {
            string filterValues = string.Empty;
            try {
                if (null != searchObject && null != keywordQuery)
                {
                    if (null != searchObject.Filters)
                    {
                        if (null != searchObject.Filters.AOLList && 0 < searchObject.Filters.AOLList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.AOLList[0]))
                        {
                            filterValues = FormFilterQuery(searchSettings.ManagedPropertyAreaOfLaw, searchObject.Filters.AOLList);
                            keywordQuery.RefinementFilters.Add(filterValues);
                        }

                        if (null != searchObject.Filters.PGList && 0 < searchObject.Filters.PGList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.PGList[0]))
                        {
                            filterValues = FormFilterQuery(searchSettings.ManagedPropertyPracticeGroup, searchObject.Filters.PGList);
                            keywordQuery.RefinementFilters.Add(filterValues);
                        }
                        keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.FromDate, searchObject.Filters.ToDate, searchSettings.ManagedPropertyOpenDate);
                        if (null != searchObject.Filters.ClientsList && 0 < searchObject.Filters.ClientsList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.ClientsList[0]))
                        {
                            filterValues = FormFilterQuery(searchSettings.ManagedPropertyClientName, searchObject.Filters.ClientsList);
                            keywordQuery.RefinementFilters.Add(filterValues);
                        }
                    }

                    keywordQuery = FilterMattersUtility(searchObject, keywordQuery);

                    keywordQuery = FilterCommonDetails(searchObject, keywordQuery, true);
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return keywordQuery;
        }

        /// <summary>
        /// Prepares and returns the query to filter the documents.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <returns>It returns a Keyword Query object.</returns>
        internal KeywordQuery FilterDocuments(SearchObject searchObject, KeywordQuery keywordQuery)
        {
            string filterValues = string.Empty;
            try
            {
                if (null != searchObject && null != keywordQuery)
                {
                    if (null != searchObject.Filters)
                    {
                        keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.FromDate, searchObject.Filters.ToDate, searchSettings.ManagedPropertyCreated);
                        if (null != searchObject.Filters.DocumentAuthor && !string.IsNullOrWhiteSpace(searchObject.Filters.DocumentAuthor))
                        {
                            keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyAuthor, ServiceConstants.COLON,
                                ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.DocumentAuthor, ServiceConstants.DOUBLE_QUOTE));
                        }

                        if (0 < searchObject.Filters.ClientsList.Count && !string.IsNullOrWhiteSpace(searchObject.Filters.ClientsList[0]))
                        {
                            filterValues = FormFilterQuery(searchSettings.ManagedPropertyDocumentClientName, searchObject.Filters.ClientsList);
                            keywordQuery.RefinementFilters.Add(filterValues);
                        }

                        /* New refinement filters for list view control */

                        if (!string.IsNullOrWhiteSpace(searchObject.Filters.Name))
                        {
                            keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyFileName, ServiceConstants.COLON,
                                ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.Name, ServiceConstants.DOUBLE_QUOTE));
                        }

                        if (!string.IsNullOrWhiteSpace(searchObject.Filters.ClientName))
                        {
                            keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyDocumentClientName, ServiceConstants.COLON,
                                ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.ClientName, ServiceConstants.DOUBLE_QUOTE));
                        }

                        if (null != searchObject.Filters.DocumentCheckoutUsers && !string.IsNullOrWhiteSpace(searchObject.Filters.DocumentCheckoutUsers))
                        {
                            keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyDocumentCheckOutUser, ServiceConstants.COLON,
                                ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.DocumentCheckoutUsers, ServiceConstants.DOUBLE_QUOTE));
                        }
                    }
                    keywordQuery = FilterCommonDetails(searchObject, keywordQuery, false);
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return keywordQuery;
        }

        /// <summary>
        /// Forms filter query for the specified property and data list.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="dataList">List of values as data</param>
        /// <returns>Filter query</returns>
        private string FormFilterQuery(string propertyName, IList<string> dataList)
        {
            string previousFilterValues = string.Empty;
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(propertyName) && null != dataList)
                {
                    if (1 == dataList.Count)
                    {
                        previousFilterValues = string.Concat(propertyName, ServiceConstants.COLON);
                    }
                    else
                    {
                        previousFilterValues = string.Concat(propertyName, ServiceConstants.COLON, ServiceConstants.SPACE, 
                            ServiceConstants.OPENING_BRACKET, ServiceConstants.OPERATOR_OR, ServiceConstants.OPENING_BRACKET);
                    }
                    for (int counter = 0; counter < dataList.Count; counter++)
                    {
                        if (0 < counter)
                        {
                            previousFilterValues += ServiceConstants.COMMA;
                        }
                        previousFilterValues += string.Concat(ServiceConstants.DOUBLE_QUOTE, dataList[counter], ServiceConstants.DOUBLE_QUOTE);
                    }
                    if (1 != dataList.Count)
                    {
                        previousFilterValues += string.Concat(ServiceConstants.CLOSING_BRACKET, ServiceConstants.CLOSING_BRACKET);
                    }
                }
                result = previousFilterValues;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Adds date refinement filter to the keyword query object
        /// </summary>        
        /// <param name="keywordQuery">The keyword query</param>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <param name="managedProperty">Managed property name</param>
        /// <returns>Returns a keyword query object</returns>
        private KeywordQuery AddDateRefinementFilter(KeywordQuery keywordQuery, string fromDate, string toDate, string managedProperty)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(fromDate) && !string.IsNullOrWhiteSpace(toDate))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ServiceConstants.COLON, ServiceConstants.OPERATOR_RANGE,
                        ServiceConstants.OPENING_BRACKET, fromDate, ServiceConstants.COMMA, toDate, ServiceConstants.CLOSING_BRACKET));
                }
                else if (string.IsNullOrWhiteSpace(fromDate) && !string.IsNullOrWhiteSpace(toDate))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ServiceConstants.COLON, ServiceConstants.OPERATOR_RANGE,
                        ServiceConstants.OPENING_BRACKET, ServiceConstants.MIN_DATE, ServiceConstants.COMMA, toDate, ServiceConstants.CLOSING_BRACKET));
                }
                else if (!string.IsNullOrWhiteSpace(fromDate) && string.IsNullOrWhiteSpace(toDate))
                {
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ServiceConstants.COLON, ServiceConstants.OPERATOR_RANGE,
                        ServiceConstants.OPENING_BRACKET, fromDate, ServiceConstants.COMMA, ServiceConstants.MAX_DATE, ServiceConstants.CLOSING_BRACKET));
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return keywordQuery;
        }

        /// <summary>
        /// Returns the query to filter the matters.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <returns>It returns a keyword query object.</returns>
        private KeywordQuery FilterMattersUtility(SearchObject searchObject, KeywordQuery keywordQuery)
        {
            try
            {
                if (null != searchObject && null != keywordQuery && null != searchObject.Filters)
                {
                    /* New refinement filters for list view control */
                    if (!string.IsNullOrWhiteSpace(searchObject.Filters.Name))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyMatterName, ServiceConstants.COLON,
                            ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.Name, ServiceConstants.DOUBLE_QUOTE));
                    }

                    if (!string.IsNullOrWhiteSpace(searchObject.Filters.ClientName))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyClientName, ServiceConstants.COLON,
                            ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.ClientName, ServiceConstants.DOUBLE_QUOTE));
                    }

                    if (null != searchObject.Filters.ResponsibleAttorneys && !string.IsNullOrWhiteSpace(searchObject.Filters.ResponsibleAttorneys))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertyResponsibleAttorney, ServiceConstants.COLON,
                            ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.ResponsibleAttorneys, ServiceConstants.DOUBLE_QUOTE));
                    }

                    if (!string.IsNullOrWhiteSpace(searchObject.Filters.SubareaOfLaw))
                    {
                        keywordQuery.RefinementFilters.Add(string.Concat(searchSettings.ManagedPropertySubAreaOfLaw, ServiceConstants.COLON,
                            ServiceConstants.DOUBLE_QUOTE, searchObject.Filters.SubareaOfLaw, ServiceConstants.DOUBLE_QUOTE));
                    }
                    if (null != searchObject.Filters.DateFilters)
                    {
                        ////// Add refiner for Open date value
                        keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.DateFilters.OpenDateFrom,
                            searchObject.Filters.DateFilters.OpenDateTo, searchSettings.ManagedPropertyOpenDate);
                    }
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

            return keywordQuery;
        }

        /// <summary>
        /// Returns the query to filter the matters/ documents for common filters.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <param name="isMatterView">Flag to identify matters/documents view.</param>
        /// <returns>It returns a keyword query object.</returns>
        private KeywordQuery FilterCommonDetails(SearchObject searchObject, KeywordQuery keywordQuery, bool isMatterView)
        {
            try
            {
                if (null != searchObject && null != keywordQuery)
                {

                    if (null != searchObject.Filters.DateFilters)
                    {
                        string lastModifiedTime = searchSettings.ManagedPropertyLastModifiedTime;
                        //// Add refiner for Modified date value
                        if (!isMatterView)
                        {
                            lastModifiedTime = searchSettings.ManagedPropertyDocumentLastModifiedTime;
                        }
                        keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.DateFilters.ModifiedFromDate,
                            searchObject.Filters.DateFilters.ModifiedToDate, lastModifiedTime);

                        ////// Add refiner for Created date value
                        keywordQuery = AddDateRefinementFilter(keywordQuery, searchObject.Filters.DateFilters.CreatedFromDate,
                            searchObject.Filters.DateFilters.CreatedToDate, searchSettings.ManagedPropertyCreated);
                    }
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return keywordQuery;
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
        private KeywordQuery KeywordQueryMetrics(Client client, SearchObject searchObject, KeywordQuery keywordQuery, 
            string filterCondition, string managedProperty, bool isMatterView)
        {
            KeywordQuery result = null;
            try
            {
                if (generalSettings.IsTenantDeployment)
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
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ServiceConstants.COLON, 
                        ServiceConstants.DOUBLE_QUOTE, true, ServiceConstants.DOUBLE_QUOTE));
                }
                else
                {

                    string[] invalidExtensions = searchSettings.FindDocumentInvalidExtensions.Split(',');
                    string chunk = string.Empty;

                    foreach (string extension in invalidExtensions)
                    {
                        chunk = chunk + "equals" + ServiceConstants.OPENING_BRACKET + ServiceConstants.DOUBLE_QUOTE + extension + 
                            ServiceConstants.DOUBLE_QUOTE + ServiceConstants.CLOSING_BRACKET + ServiceConstants.COMMA;
                    }
                    chunk = chunk.Remove(chunk.Length - 1);

                    keywordQuery.RefinementFilters.Add(string.Concat("not" + ServiceConstants.OPENING_BRACKET + "FileType", ServiceConstants.COLON, ServiceConstants.OPERATOR_OR + ServiceConstants.OPENING_BRACKET +
                                                                                                            chunk + ServiceConstants.CLOSING_BRACKET + ServiceConstants.CLOSING_BRACKET
                                                                                                            ));
                    keywordQuery.RefinementFilters.Add(string.Concat(managedProperty, ServiceConstants.COLON, "equals", ServiceConstants.OPENING_BRACKET + ServiceConstants.DOUBLE_QUOTE +
                                                                                                            "1" + ServiceConstants.DOUBLE_QUOTE + ServiceConstants.CLOSING_BRACKET
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
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Assigns the keyword query values.
        /// </summary>
        /// <param name="keywordQuery">The keyword query.</param>
        /// <param name="managedProperties">The managed properties.</param>
        /// <returns>It returns a Keyword Query object.</returns>
        private KeywordQuery AssignKeywordQueryValues(KeywordQuery keywordQuery, List<string> managedProperties)
        {
            KeywordQuery result = null;
            try
            {                
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
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
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
        private SearchResponseVM FillResultData(ClientContext clientContext, KeywordQuery keywordQuery, 
            SearchRequestVM searchRequestVM, Boolean isMatterSearch, List<string> managedProperties)
        {
            SearchResponseVM searchResponseVM = new SearchResponseVM() ;
            Boolean isReadOnly;
            try
            {
                var searchObject = searchRequestVM.SearchObject;
                //var client = searchRequestVM.Client;
                if (null != searchObject.Sort)
                {
                    keywordQuery.EnableSorting = true;
                    keywordQuery = GetSortByProperty(keywordQuery, searchObject, isMatterSearch);
                }
                SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                ClientResult<ResultTableCollection> resultsTableCollection = searchExecutor.ExecuteQuery(keywordQuery);
                Users currentLoggedInUser = userDetails.GetLoggedInUserDetails(clientContext);

                if (null != resultsTableCollection && null != resultsTableCollection.Value && 0 < 
                    resultsTableCollection.Value.Count && null != resultsTableCollection.Value[0].ResultRows)
                {
                    if (isMatterSearch && 0 < resultsTableCollection.Value.Count && 
                        null != resultsTableCollection.Value[0].ResultRows && !string.IsNullOrWhiteSpace(currentLoggedInUser.Email))
                    {
                        foreach (IDictionary<string, object> matterMetadata in resultsTableCollection.Value[0].ResultRows)
                        {
                            isReadOnly = false;
                            if (null != matterMetadata)
                            {
                                // Decode matter properties
                                DecodeMatterProperties(matterMetadata);
                                string readOnlyUsers = Convert.ToString(matterMetadata[searchSettings.ManagedPropertyBlockedUploadUsers], CultureInfo.InvariantCulture);
                                if (!string.IsNullOrWhiteSpace(readOnlyUsers))
                                {
                                    isReadOnly = IsUserReadOnlyForMatter(isReadOnly, currentLoggedInUser.Name, 
                                        currentLoggedInUser.Email, readOnlyUsers);
                                }
                                matterMetadata.Add(generalSettings.IsReadOnlyUser, isReadOnly);
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
                                string authorData = Convert.ToString(documentMetadata[searchSettings.ManagedPropertyAuthor], CultureInfo.InvariantCulture);
                                int ltIndex = authorData.IndexOf(ServiceConstants.OPENING_ANGULAR_BRACKET, StringComparison.Ordinal);
                                int gtIndex = authorData.IndexOf(ServiceConstants.CLOSING_ANGULAR_BRACKET, StringComparison.Ordinal);
                                authorData = (0 <= ltIndex && ltIndex < gtIndex) ? authorData.Remove(ltIndex, (gtIndex - ltIndex) + 1) : authorData;
                                authorData = authorData.Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES, string.Empty);
                                documentMetadata[searchSettings.ManagedPropertyAuthor] = authorData.Trim();
                            }
                        }
                    }
                    if (resultsTableCollection.Value.Count>1)
                    {                        
                        searchResponseVM.TotalRows = resultsTableCollection.Value[0].TotalRows;
                        searchResponseVM.SearchResults = resultsTableCollection.Value[0].ResultRows;
                    }
                    else
                    {
                        if (resultsTableCollection.Value[0].TotalRows==0)
                        {
                            searchResponseVM = NoDataRow(managedProperties);
                        }
                        else
                        {                            
                            searchResponseVM.TotalRows = resultsTableCollection.Value[0].TotalRows;
                            searchResponseVM.SearchResults = resultsTableCollection.Value[0].ResultRows;
                        }
                    }
                }
                else
                {
                    searchResponseVM = NoDataRow(managedProperties);
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return searchResponseVM;
        }

        /// <summary>
        /// Defines the sorting property and direction for querying SharePoint Search.
        /// </summary>
        /// <param name="keywordQuery">Keyword object</param>
        /// <param name="searchObject">Search object</param>
        /// <param name="isMatterSearch">Boolean flag which determines current search is for matters or documents</param>
        /// <returns></returns>
        private KeywordQuery GetSortByProperty(KeywordQuery keywordQuery, SearchObject searchObject, Boolean isMatterSearch)
        {
            string matterIDRefiner = string.Empty;
            try
            {
                ////Sorting by specified property  0 --> Ascending order and 1 --> Descending order
                if (!string.IsNullOrWhiteSpace(searchObject.Sort.ByProperty))
                {
                    keywordQuery = AddSortingRefiner(keywordQuery, searchObject.Sort.ByProperty, searchObject.Sort.Direction);
                    //// Add Matter ID property as second level sort for Client.MatterID column based on Search Matter or Search Document
                    if (searchSettings.ManagedPropertyClientID == searchObject.Sort.ByProperty || 
                        searchSettings.ManagedPropertyDocumentClientId == searchObject.Sort.ByProperty)
                    {
                        if (isMatterSearch)
                        {
                            matterIDRefiner = searchSettings.ManagedPropertyMatterId;
                        }
                        else
                        {
                            matterIDRefiner = searchSettings.ManagedPropertyDocumentMatterId;
                        }
                        keywordQuery = AddSortingRefiner(keywordQuery, matterIDRefiner, searchObject.Sort.Direction);
                    }
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return keywordQuery;
        }

        /// <summary>
        /// Decodes matter properties before sending them to UI
        /// </summary>
        /// <param name="matterMetadata">Dictionary object contains matter meta data</param>
        private void DecodeMatterProperties(IDictionary<string, object> matterMetadata)
        {

            // Decode matter properties
            matterMetadata[searchSettings.ManagedPropertyTitle] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyTitle]);
            matterMetadata[searchSettings.ManagedPropertySiteName] = DecodeValues(matterMetadata[searchSettings.ManagedPropertySiteName]);
            matterMetadata[searchSettings.ManagedPropertyDescription] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyDescription]);
            matterMetadata[searchSettings.ManagedPropertyPracticeGroup] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyPracticeGroup]);
            matterMetadata[searchSettings.ManagedPropertyAreaOfLaw] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyAreaOfLaw]);
            matterMetadata[searchSettings.ManagedPropertySubAreaOfLaw] = DecodeValues(matterMetadata[searchSettings.ManagedPropertySubAreaOfLaw]);
            matterMetadata[searchSettings.ManagedPropertyCustomTitle] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyCustomTitle]);
            matterMetadata[searchSettings.ManagedPropertyPath] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyPath]);
            matterMetadata[searchSettings.ManagedPropertyMatterName] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyMatterName]);
            matterMetadata[searchSettings.ManagedPropertyOpenDate] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyOpenDate]);
            matterMetadata[searchSettings.ManagedPropertyClientName] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyClientName]);
            matterMetadata[searchSettings.ManagedPropertyBlockedUploadUsers] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyBlockedUploadUsers]);
            matterMetadata[searchSettings.ManagedPropertyResponsibleAttorney] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyResponsibleAttorney]);
            matterMetadata[searchSettings.ManagedPropertyClientID] = DecodeValues(matterMetadata[searchSettings.ManagedPropertyClientID]);
        }

        /// <summary>
        /// Provides the required matter properties
        /// </summary>
        /// <param name="value">Matter Properties object</param>
        /// <returns>Decoded String</returns>
        private static string DecodeValues(object value) =>  null != value ? WebUtility.HtmlDecode(Convert.ToString(value, CultureInfo.InvariantCulture)) : string.Empty;


        /// <summary>
        /// Checks if logged-in user has read permission on matter.
        /// </summary>
        /// <param name="isReadOnly">Flag indicating if user has read permission on matter</param>
        /// <param name="currentLoggedInUser">Current logged-in user name</param>
        /// <param name="readOnlyUsers">List of read only user for matter</param>
        /// <returns>Flag indicating if user has read permission on matter</returns>
        private bool IsUserReadOnlyForMatter(Boolean isReadOnly, string currentLoggedInUser, string currentLoggedInUserEmail, string readOnlyUsers)
        {
            try {
                List<string> readOnlyUsersList = readOnlyUsers.Trim().Split(new string[] { ServiceConstants.SEMICOLON }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> currentReadOnlyUser = (from readOnlyUser in readOnlyUsersList
                                                    where string.Equals(readOnlyUser.Trim(), currentLoggedInUser.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                                    string.Equals(readOnlyUser.Trim(), currentLoggedInUserEmail.Trim(), StringComparison.OrdinalIgnoreCase)
                                                    select readOnlyUser).ToList();
                if (null != currentReadOnlyUser && 0 < currentReadOnlyUser.Count)
                {
                    isReadOnly = true;
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return isReadOnly;
        }

        /// <summary>
        /// Returns the Keyword query object with refiners added for sorting
        /// </summary>
        /// <param name="keywordQuery">Keyword object</param>
        /// <param name="sortByProperty">Property by which sort is applied</param>
        /// <param name="sortDirection">Direction in which sort is applied (0 --> Ascending order and 1 --> Descending order)</param>
        /// <returns>Keyword object with sorting refiners applied</returns>
        private static KeywordQuery AddSortingRefiner(KeywordQuery keywordQuery, string sortByProperty, int sortDirection)
        {
            if (0 == sortDirection)
            {
                keywordQuery.SortList.Add(sortByProperty, SortDirection.Ascending);
            }
            else if (1 == sortDirection)
            {
                keywordQuery.SortList.Add(sortByProperty, SortDirection.Descending);
            }
            return keywordQuery;
        }

        /// <summary>
        /// Function to return no data row
        /// </summary>
        /// <param name="managedProperties">Managed properties information</param>
        /// <returns>No data row</returns>
        private SearchResponseVM NoDataRow(List<string> managedProperties)
        {
            SearchResponseVM searchResponseVM = new SearchResponseVM();
            try {
                List<Dictionary<string, object>> noDataList = new List<Dictionary<string, object>>();               
                Dictionary<string, object> noDataObject = new Dictionary<string, object>();
                managedProperties.Add(ServiceConstants.PATH_FIELD_NAME);
                foreach (string managedProperty in managedProperties)
                {
                    if (!noDataObject.ContainsKey(managedProperty))
                    {
                        noDataObject.Add(managedProperty, string.Empty);
                    }
                }

                noDataList.Add(noDataObject);
                searchResponseVM.TotalRows = 0;
                searchResponseVM.SearchResults = noDataList;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            //string result = string.Concat(ServiceConstants.OPEN_SQUARE_BRACE, 
            //    JsonConvert.SerializeObject(noDataObject), ServiceConstants.CLOSE_SQUARE_BRACE,
            //    ServiceConstants.DOLLAR, ServiceConstants.PIPE, ServiceConstants.DOLLAR, 0);
            return searchResponseVM;
        }

        
        #endregion
    }
}
