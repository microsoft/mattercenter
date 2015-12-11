// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="Search.svc.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines service for Search(Find matter/Find document) App.</summary>
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
    using Microsoft.SharePoint.Client.Search.Query;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    #endregion

    /// <summary>
    /// Holds the operation contracts used for searching matters and documents.
    /// </summary>
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Search
    {
        #region Operation Contracts
        /// <summary>
        /// Gets the matters based on search criteria.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="searchObject">Search object containing Search, Filter and Sort data</param>
        /// <returns>Serialized string of Matter JSON object</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string FindMatter(RequestObject requestObject, Client client, SearchObject searchObject)
        {
            string result = string.Empty;
            if (null != requestObject && null != client & null != searchObject && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    // Encode all fields which are coming from js
                    SearchHelperFunctions.EncodeSearchDetails(searchObject.Filters, true);
                    // Encode Search Term
                    searchObject.SearchTerm = (null != searchObject.SearchTerm) ? Encoder.HtmlEncode(searchObject.SearchTerm).Replace(ConstantStrings.ENCODEDDOUBLEQUOTES, ConstantStrings.DoubleQuote) : string.Empty;

                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                        if (string.IsNullOrWhiteSpace(searchObject.SearchTerm))
                        {
                            searchObject.SearchTerm = ConstantStrings.Asterisk;
                        }

                        if (null != searchObject.Filters)
                        {
                            if (1 == searchObject.Filters.FilterByMe)
                            {
                                ////Get logged in user alias
                                Users currentUserDetail = UIUtility.GetLoggedInUserDetails(clientContext);
                                string userTitle = currentUserDetail.Name;
                                searchObject.SearchTerm = string.Concat(searchObject.SearchTerm, ConstantStrings.Space, ConstantStrings.OperatorAnd, ConstantStrings.Space, ConstantStrings.OpeningBracket, SearchConstants.ManagedPropertyResponsibleAttorney, ConstantStrings.COLON, ConstantStrings.Space, ConstantStrings.DoubleQuote, userTitle, ConstantStrings.DoubleQuote, ConstantStrings.Space, ConstantStrings.OperatorOR, ConstantStrings.Space, SearchConstants.ManagedPropertyTeamMembers, ConstantStrings.COLON, ConstantStrings.Space, ConstantStrings.DoubleQuote, userTitle, ConstantStrings.DoubleQuote, ConstantStrings.Space, ConstantStrings.ClosingBracket);
                            }

                            keywordQuery = SearchHelperFunctions.FilterMatters(searchObject, keywordQuery);
                        }

                        keywordQuery = SearchHelperFunctions.KeywordQueryMetrics(client, searchObject, keywordQuery, ConstantStrings.DocumentLibraryFilterCondition, SearchConstants.ManagedPropertyIsMatter, true);

                        // Create a list of managed properties which are required to be present in search results
                        List<string> managedProperties = new List<string>();
                        managedProperties.Add(SearchConstants.ManagedPropertyTitle);
                        managedProperties.Add(SearchConstants.ManagedPropertyName);
                        managedProperties.Add(SearchConstants.ManagedPropertyDescription);
                        managedProperties.Add(SearchConstants.ManagedPropertySiteName);
                        managedProperties.Add(SearchConstants.ManagedPropertyLastModifiedTime);
                        managedProperties.Add(SearchConstants.ManagedPropertyPracticeGroup);
                        managedProperties.Add(SearchConstants.ManagedPropertyAreaOfLaw);
                        managedProperties.Add(SearchConstants.ManagedPropertySubAreaOfLaw);
                        managedProperties.Add(SearchConstants.ManagedPropertyMatterId);
                        managedProperties.Add(SearchConstants.ManagedPropertyCustomTitle);
                        managedProperties.Add(SearchConstants.ManagedPropertyPath);
                        managedProperties.Add(SearchConstants.ManagedPropertyMatterName);
                        managedProperties.Add(SearchConstants.ManagedPropertyOpenDate);
                        managedProperties.Add(SearchConstants.ManagedPropertyClientName);
                        managedProperties.Add(SearchConstants.ManagedPropertyBlockedUploadUsers);
                        managedProperties.Add(SearchConstants.ManagedPropertyResponsibleAttorney);
                        managedProperties.Add(SearchConstants.ManagedPropertyClientID);
                        managedProperties.Add(SearchConstants.ManagedPropertyMatterGuid);

                        //Filter on Result source to fetch only Matter Center specific results
                        keywordQuery.SourceId = new Guid(SearchConstants.SearchResultSourceID);

                        keywordQuery = SearchHelperFunctions.AssignKeywordQueryValues(keywordQuery, managedProperties);

                        keywordQuery.BypassResultTypes = true;

                        result = SearchHelperFunctions.FillResultData(clientContext, keywordQuery, searchObject, true, managedProperties);
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
        /// Gets the folder hierarchy of the specified matter.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="matterData">Matter object containing Matter data</param>
        /// <returns>Serialized string of folder hierarchy JSON object</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetFolderHierarchy(RequestObject requestObject, MatterData matterData)
        {
            string result = string.Empty;
            if (null != requestObject && null != matterData && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(matterData.MatterName) && !string.IsNullOrWhiteSpace(matterData.MatterUrl))
                    {
                        using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(matterData.MatterUrl), requestObject.RefreshToken))
                        {
                            List list = clientContext.Web.Lists.GetByTitle(matterData.MatterName);
                            clientContext.Load(list.RootFolder);
                            ListItemCollection listItems = Lists.GetData(clientContext, matterData.MatterName, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.AllFoldersQuery, matterData.MatterName));
                            List<FolderData> allFolders = new List<FolderData>();
                            allFolders = SearchHelperFunctions.GetFolderAssignment(list, listItems, allFolders);

                            result = JsonConvert.SerializeObject(allFolders);
                        }
                    }
                    else
                    {
                        result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, 0, TextConstants.MessageNoInputs);
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
        /// Gets user specific pinned matters.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <returns>JSON structure with the meta-data of pinned matter for requested user</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string FindUserPinnedMatter(RequestObject requestObject, Client client)
        {
            string result = string.Empty;
            if (null != requestObject && null != client && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                result = SearchHelperFunctions.ShowPinData(requestObject, client, ServiceConstantStrings.UserPinnedMatterListName, ServiceConstantStrings.PinnedListColumnMatterDetails, false);
                // Decode pinned documents
                result = HttpUtility.HtmlDecode(result);
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, string.Empty, TextConstants.MessageNoInputs);
            }

            return result;
        }

        /// <summary>
        /// Adds matter in user pinned details.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matterData">Matter object containing Matter data</param>
        /// <returns>Status of update</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string PinMatterForUser(RequestObject requestObject, Client client, MatterData matterData)
        {
            string status = string.Empty;
            if (null != requestObject && null != client && null != matterData && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                ////Create matter object and populate meta-data
                MatterData userPinnedMatterData = new MatterData()
                {
                    MatterName = SearchHelperFunctions.EncodeValues(matterData.MatterName),
                    MatterDescription = SearchHelperFunctions.EncodeValues(matterData.MatterDescription),
                    MatterCreatedDate = SearchHelperFunctions.EncodeValues(matterData.MatterCreatedDate),
                    MatterUrl = SearchHelperFunctions.EncodeValues(matterData.MatterUrl),
                    MatterPracticeGroup = SearchHelperFunctions.EncodeValues(matterData.MatterPracticeGroup),
                    MatterAreaOfLaw = SearchHelperFunctions.EncodeValues(matterData.MatterAreaOfLaw),
                    MatterSubAreaOfLaw = SearchHelperFunctions.EncodeValues(matterData.MatterSubAreaOfLaw),
                    MatterClientUrl = SearchHelperFunctions.EncodeValues(matterData.MatterClientUrl),
                    MatterClient = SearchHelperFunctions.EncodeValues(matterData.MatterClient),
                    MatterClientId = SearchHelperFunctions.EncodeValues(matterData.MatterClientId),
                    HideUpload = SearchHelperFunctions.EncodeValues(matterData.HideUpload),
                    MatterID = SearchHelperFunctions.EncodeValues(matterData.MatterID),
                    MatterResponsibleAttorney = SearchHelperFunctions.EncodeValues(matterData.MatterResponsibleAttorney),
                    MatterModifiedDate = SearchHelperFunctions.EncodeValues(matterData.MatterModifiedDate),
                    MatterGuid = SearchHelperFunctions.EncodeValues(matterData.MatterGuid)
                };
                status = SearchHelperFunctions.PopulateMetadeta(requestObject, client, userPinnedMatterData, null);
            }
            else
            {
                status = TextConstants.MessageNoInputs;
            }
            return status;
        }

        /// <summary>
        /// Removes pinned matter from user pinned details.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matterData">Matter object containing Matter data</param>
        /// <returns>Status of update</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string RemovePinnedMatter(RequestObject requestObject, Client client, MatterData matterData)
        {

            string status = string.Empty;
            if (null != requestObject && null != client && null != matterData && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                status = SearchHelperFunctions.UnpinItem(requestObject, client, matterData, null);
            }
            else
            {
                status = TextConstants.MessageNoInputs;
            }
            return status;
        }

        /// <summary>
        /// Gets the documents based on search criteria.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="searchObject">Search object containing Search, Filter and Sort data</param>
        /// <returns>Serialized string of Document JSON object</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string FindDocument(RequestObject requestObject, Client client, SearchObject searchObject)
        {
            string result = string.Empty;
            if (null != requestObject && null != client & null != searchObject && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    // Encode all fields which are coming from js
                    SearchHelperFunctions.EncodeSearchDetails(searchObject.Filters, false);
                    // Encode Search Term
                    searchObject.SearchTerm = (null != searchObject.SearchTerm) ? Encoder.HtmlEncode(searchObject.SearchTerm).Replace(ConstantStrings.ENCODEDDOUBLEQUOTES, ConstantStrings.DoubleQuote) : string.Empty;

                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                        if (string.IsNullOrWhiteSpace(searchObject.SearchTerm))
                        {
                            searchObject.SearchTerm = ConstantStrings.Asterisk;
                        }

                        if (null != searchObject.Filters)
                        {
                            if (1 == searchObject.Filters.FilterByMe)
                            {
                                // Get the current logged in User
                                clientContext.Load(clientContext.Web.CurrentUser);
                                clientContext.ExecuteQuery();
                                string currentLoggedInUser = clientContext.Web.CurrentUser.Title;
                                searchObject.SearchTerm = String.Concat(searchObject.SearchTerm, ConstantStrings.Space, ConstantStrings.OperatorAnd, ConstantStrings.Space, SearchConstants.ManagedPropertyAuthor, ConstantStrings.COLON, currentLoggedInUser);
                            }
                            keywordQuery = SearchHelperFunctions.FilterDocuments(searchObject, keywordQuery);
                        }

                        keywordQuery = SearchHelperFunctions.KeywordQueryMetrics(client, searchObject, keywordQuery, ConstantStrings.DocumentItemFilterCondition, SearchConstants.ManagedPropertyIsDocument, false);

                        //// Create a list of managed properties which are required to be present in search results
                        List<string> managedProperties = new List<string>();
                        managedProperties.Add(SearchConstants.ManagedPropertyFileName);
                        managedProperties.Add(SearchConstants.ManagedPropertyTitle);
                        managedProperties.Add(SearchConstants.ManagedPropertyCreated);
                        managedProperties.Add(SearchConstants.ManagedPropertyUIVersionStringOWSTEXT);
                        managedProperties.Add(SearchConstants.ManagedPropertyServerRelativeUrl);
                        managedProperties.Add(SearchConstants.ManagedPropertyFileExtension);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentMatterId);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentLastModifiedTime);
                        managedProperties.Add(SearchConstants.ManagedPropertySiteTitle);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentClientId);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentClientName);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentMatterName);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentId);
                        managedProperties.Add(SearchConstants.ManagedPropertyCheckOutByUser);
                        managedProperties.Add(SearchConstants.ManagedPropertySiteName);
                        managedProperties.Add(SearchConstants.ManagedPropertySPWebUrl);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentVersion);
                        managedProperties.Add(SearchConstants.ManagedPropertyDocumentCheckOutUser);
                        managedProperties.Add(SearchConstants.ManagedPropertySPWebUrl);
                        managedProperties.Add(SearchConstants.ManagedPropertyAuthor);

                        //Filter on Result source to fetch only Matter Center specific results
                        keywordQuery.SourceId = new Guid(SearchConstants.SearchResultSourceID);

                        keywordQuery = SearchHelperFunctions.AssignKeywordQueryValues(keywordQuery, managedProperties);

                        result = SearchHelperFunctions.FillResultData(clientContext, keywordQuery, searchObject, false, managedProperties);
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
        /// Adds document in user pinned details.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="documentData">Document object containing Document data</param>
        /// <returns>Status of update</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string PinDocumentForUser(RequestObject requestObject, Client client, DocumentData documentData)
        {
            string status = string.Empty;
            if (null != requestObject && null != client && null != documentData && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                ////Create document object and populate meta-data
                DocumentData userPinnedDocumentData = new DocumentData()
                {
                    DocumentName = SearchHelperFunctions.EncodeValues(documentData.DocumentName),
                    DocumentVersion = SearchHelperFunctions.EncodeValues(documentData.DocumentVersion),
                    DocumentClient = SearchHelperFunctions.EncodeValues(documentData.DocumentClient),
                    DocumentClientId = SearchHelperFunctions.EncodeValues(documentData.DocumentClientId),
                    DocumentClientUrl = SearchHelperFunctions.EncodeValues(documentData.DocumentClientUrl),
                    DocumentMatter = SearchHelperFunctions.EncodeValues(documentData.DocumentMatter),
                    DocumentMatterId = SearchHelperFunctions.EncodeValues(documentData.DocumentMatterId),
                    DocumentOwner = SearchHelperFunctions.EncodeValues(documentData.DocumentOwner),
                    DocumentUrl = SearchHelperFunctions.EncodeValues(documentData.DocumentUrl),
                    DocumentOWAUrl = SearchHelperFunctions.EncodeValues(documentData.DocumentOWAUrl),
                    DocumentExtension = SearchHelperFunctions.EncodeValues(documentData.DocumentExtension),
                    DocumentCreatedDate = SearchHelperFunctions.EncodeValues(documentData.DocumentCreatedDate),
                    DocumentModifiedDate = SearchHelperFunctions.EncodeValues(documentData.DocumentModifiedDate),
                    DocumentCheckoutUser = SearchHelperFunctions.EncodeValues(documentData.DocumentCheckoutUser),
                    DocumentMatterUrl = SearchHelperFunctions.EncodeValues(documentData.DocumentMatterUrl),
                    DocumentParentUrl = SearchHelperFunctions.EncodeValues(documentData.DocumentParentUrl),
                    DocumentID = SearchHelperFunctions.EncodeValues(documentData.DocumentID)
                };
                status = SearchHelperFunctions.PopulateMetadeta(requestObject, client, null, userPinnedDocumentData);
            }
            else
            {
                status = TextConstants.MessageNoInputs;
            }
            return status;
        }

        /// <summary>
        /// Removes pinned document from user pinned details.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="documentData">Document object containing Document data</param>
        /// <returns>Status of update</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string RemovePinnedDocument(RequestObject requestObject, Client client, DocumentData documentData)
        {
            string status = string.Empty;
            if (null != requestObject && null != client && null != documentData && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                status = SearchHelperFunctions.UnpinItem(requestObject, client, null, documentData);
            }
            else
            {
                status = TextConstants.MessageNoInputs;
            }
            return status;
        }

        /// <summary>
        /// Gets user specific pinned documents.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <returns>JSON structure with the meta-data of pinned document for requested user</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string FindUserPinnedDocument(RequestObject requestObject, Client client)
        {
            string result = string.Empty;
            if (null != requestObject && null != client && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                result = SearchHelperFunctions.ShowPinData(requestObject, client, ServiceConstantStrings.UserPinnedDocumentListName, ServiceConstantStrings.PinnedListColumnDocumentDetails, true);
                // Decode pinned documents
                result = HttpUtility.HtmlDecode(result);
            }
            else
            {
                result = TextConstants.MessageNoInputs;
            }

            return result;
        }

        /// <summary>
        /// Uploads mail to SharePoint library.
        /// </summary>
        /// <param name="requestObject"> Request object containing SharePoint App Token </param>
        /// <param name="client"> Client object containing Client data </param>
        /// <param name="serviceRequest"> Service request object containing mail data </param>
        /// <returns> Status of update</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string UploadMail(RequestObject requestObject, Client client, ServiceRequest serviceRequest)
        {
            string message = string.Empty;
            string result = "True";
            if (null != requestObject && null != client && null != serviceRequest && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                if (UploadHelperFunctions.Upload(requestObject, client, serviceRequest, ServiceConstantStrings.MailSoapRequest, serviceRequest.MailId, true, serviceRequest.Subject, serviceRequest.FolderPath[0], true, ref message, string.Empty).Equals(ConstantStrings.UploadFailed))
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = message;
                    }
                    else
                    {
                        result = "False";
                    }
                }
            }
            else
            {
                result = TextConstants.MessageNoInputs;
            }
            return result;
        }

        /// <summary>
        /// Uploads attachment to SharePoint library.
        /// </summary>
        /// <param name="requestObject"> Request object containing SharePoint App Token </param>
        /// <param name="client"> Client object containing Client data </param>
        /// <param name="serviceRequest"> Service request object containing mail data</param>
        /// <returns>  Status of update </returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string UploadAttachment(RequestObject requestObject, Client client, ServiceRequest serviceRequest)
        {
            string resultFlag = string.Empty;
            if (null != requestObject && null != client && null != serviceRequest && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                ///// check if folder path and attachment length are same
                if (serviceRequest.FolderPath.Count == serviceRequest.Attachments.Count)
                {
                    bool result = true;
                    ///// StringBuilder builder = new StringBuilder();
                    int attachmentCount = 0;
                    string message = string.Empty;
                    foreach (AttachmentDetails attachment in serviceRequest.Attachments)
                    {
                        if (UploadHelperFunctions.Upload(requestObject, client, serviceRequest, ServiceConstantStrings.AttachmentSoapRequest, attachment.id, false, attachment.name, serviceRequest.FolderPath[attachmentCount], true, ref message, attachment.originalName).Equals(ConstantStrings.UploadFailed))
                        {
                            result = false;
                            break;
                        }

                        attachmentCount++;
                    }

                    if (result)
                    {
                        resultFlag = "True";
                    }
                    else if (!string.IsNullOrEmpty(message))
                    {
                        resultFlag = message;
                    }
                    else
                    {
                        resultFlag = "False";
                    }
                }
                else
                {
                    resultFlag = "False";
                }
            }
            else
            {
                resultFlag = TextConstants.MessageNoInputs;
            }
            return resultFlag;
        }

        /// <summary>
        /// Returns true or false based on the existence of the matter landing page and OneNote file at the URLs provided.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="requestedUrl">String object containing the OneNote file path</param>
        /// <param name="requestedPageUrl">String object containing the Matter Landing Page file path</param>
        /// <returns>$|$ Separated string indicating that the OneNote and the Matter Landing Page exist or not</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string UrlExists(RequestObject requestObject, Client client, string requestedUrl, string requestedPageUrl)
        {
            string result = string.Empty;
            if (null != requestObject && null != client && null != requestedUrl && null != requestedPageUrl && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        // Check if the OneNote exists
                        string oneNoteExists = SearchHelperFunctions.PageExists(requestedUrl, clientContext);
                        // Check if the Matter Landing Page exists
                        string matterLandingExists = SearchHelperFunctions.PageExists(requestedPageUrl, clientContext);
                        // Finally return the result of the two operations
                        result = oneNoteExists + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + matterLandingExists;
                    }
                }
                catch (Exception exception)
                {
                    // In case of a general exception, return false values for both cases
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    result = ConstantStrings.FALSE + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + ConstantStrings.FALSE;
                }
            }
            else
            {
                result = TextConstants.MessageNoInputs;
            }
            return result;
        }

        /// <summary>
        /// Returns contextual help content in JSON format.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="selectedPage">String object containing the page number where user is on</param>
        /// <returns>$|$ contextual help content in JSON format</returns>
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string FetchContextualHelpContent(RequestObject requestObject, Client client, string selectedPage)
        {
            // Basic initialization
            string result = string.Empty, cacheKey = string.Empty;
            string selectedSectionIDs = string.Empty;
            IList<string> sectionID = new List<string>();
            if (null != requestObject && null != client && null != selectedPage && !string.IsNullOrWhiteSpace(selectedPage) && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                List<ContextHelpData> contextHelpCollection = new List<ContextHelpData>();
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        string[] pageNames = ServiceConstantStrings.MatterCenterPages.Split(';');
                        switch (selectedPage)
                        {
                            case "1": selectedPage = pageNames[1];
                                break;
                            case "2": selectedPage = pageNames[2];
                                break;
                            case "4": selectedPage = pageNames[4];
                                break;
                            default: selectedPage = pageNames[0];
                                break;
                        }

                        cacheKey = string.Concat(selectedPage, ConstantStrings.LINKS_STATIC_STRING);
                        result = ServiceUtility.GetOrSetCachedValue(cacheKey);
                        if (result.Equals(ConstantStrings.FALSE))
                        {

                            //Object to store all the list items retrieved from SharePoint list
                            ListItemCollection contextualHelpSectionListItems;

                            // Get MatterCenterHelpSection list data
                            contextualHelpSectionListItems = Lists.GetData(clientContext, ServiceConstantStrings.MatterCenterHelpSectionListName, String.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.RetrieveContextualHelpSectionsQuery, selectedPage));

                            //If these exists any content for contextual help flyout
                            if (null != contextualHelpSectionListItems && 0 < contextualHelpSectionListItems.Count)
                            {
                                SearchHelperFunctions.FetchContextualHelpContentUtility(ref result, ref selectedSectionIDs, sectionID, ref contextHelpCollection, clientContext, contextualHelpSectionListItems);
                                if (!ServiceUtility.CheckValueHasErrors(result))
                                {
                                    ServiceUtility.GetOrSetCachedValue(cacheKey, result);
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
            return result;
        }

        /// <summary>
        /// Returns document and list GUID
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing list data</param>        
        /// <returns>Document and list GUID</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetDocumentAssets(RequestObject requestObject, Client client)
        {
            string result = string.Empty;
            if (null != requestObject && null != client && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        result = SearchHelperFunctionsUtility.GetDocumentAndClientGUID(client, clientContext);
                    }
                }
                catch (Exception exception)
                {
                    result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                }
            }
            return result;
        }
    }
        #endregion
}
