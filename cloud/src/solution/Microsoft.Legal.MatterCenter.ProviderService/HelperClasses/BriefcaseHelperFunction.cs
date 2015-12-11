// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-akdigh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="BriefcaseHelperFunction.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods involved in briefcase operations.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.CommonHelper
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Web;
    using System.Web.Script.Serialization;
    #endregion

    /// <summary>
    /// Provide methods involved in briefcase operations.
    /// </summary>
    internal static class BriefcaseHelperFunction
    {
        /// <summary>
        ///  Validates the document during briefcase operations. Returns not supported message if the document in operation is not relevant to Matter Center.
        /// </summary>
        /// <param name="sourceURL">List of File names</param>
        /// <returns>Returns response for not supported files</returns>
        internal static string GetNotSupportedMessage(List<string> sourceURL)
        {
            string response = string.Empty;
            ResponseDetails responseDetails = new ResponseDetails();
            responseDetails.Status = ConstantStrings.FALSE;
            responseDetails.FileNames = sourceURL;
            response = new JavaScriptSerializer().Serialize(responseDetails);
            return response;
        }

        /// <summary>
        /// Fetches the collection of list items satisfying the search criteria.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="itemid">Item Id of the document</param>
        /// <returns>ListItemCollection object</returns>
        internal static ListItemCollection GetListItemCollection(ClientContext clientContext, int itemid)
        {
            ListItemCollection listItems = null;
            try
            {
                string briefcaseItemQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.BriefCaseItemsQuery, itemid);
                listItems = Lists.GetData(clientContext, ServiceConstantStrings.OneDriveDocumentLibraryTitle, briefcaseItemQuery);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return listItems;
        }

        /// <summary>
        /// Returns serialized response of the object.
        /// </summary>
        /// <param name="commonResponse">CommonResponse object</param>
        /// <returns>Serialized response of the object</returns>
        internal static string GetSerializeResponse(object commonResponse)
        {
            string result = string.Empty;
            try
            {
                result = new JavaScriptSerializer().Serialize(commonResponse);
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Obtains and returns MySite URL of logged-in user.
        /// </summary>
        /// <param name="requestObject">Request Object</param>
        /// <returns>My Site URL of the user</returns>
        internal static string GetPersonalURL(RequestObject requestObject)
        {
            string usersMySite = string.Empty;
            string result = string.Empty;
            try
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(ServiceConstantStrings.CentralRepositoryUrl), requestObject.RefreshToken))
                {
                    PeopleManager peopleManager = new PeopleManager(clientContext);
                    PersonProperties personProperties = peopleManager.GetMyProperties();
                    ///// Load users my site URL
                    clientContext.Load(personProperties, p => p.PersonalUrl);
                    clientContext.ExecuteQuery();
                    usersMySite = personProperties.PersonalUrl;
                    result = usersMySite;
                }
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Returns a list of source URL based on type of briefcase operation.
        /// </summary>
        /// <param name="clientContext">Client Context object</param>
        /// <param name="syncDetails">SyncDetails object</param>
        /// <param name="operationType">Indicates the type of operation to be performed</param>
        /// <returns>List of Source URLs</returns>
        internal static List<string> CheckSourceURL(ClientContext clientContext, SyncDetails syncDetails, int operationType)
        {
            List<string> sourceURL = new List<string>();
            try
            {
                foreach (int itemid in syncDetails.ItemId)
                {
                    string briefcaseItemQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.BriefCaseItemsQuery, itemid);
                    ListItemCollection listItems = Lists.GetData(clientContext, ServiceConstantStrings.OneDriveDocumentLibraryTitle, briefcaseItemQuery);
                    clientContext.Load(listItems);
                    clientContext.ExecuteQuery();
                    foreach (var listitem in listItems)
                    {
                        switch (operationType)
                        {
                            case 1:
                                if (string.IsNullOrEmpty(Convert.ToString(listitem[ServiceConstantStrings.OneDriveSiteColumn], CultureInfo.InvariantCulture)))
                                {
                                    sourceURL.Add(Convert.ToString(listitem[ServiceConstantStrings.ColumnNameFileLeafRef], CultureInfo.InvariantCulture));
                                }
                                break;
                            case 2:
                                sourceURL.Add(Convert.ToString(listitem[ServiceConstantStrings.OneDriveSiteColumn], CultureInfo.InvariantCulture));
                                break;
                            case 3:
                                sourceURL.Add(Convert.ToString(listitem[ServiceConstantStrings.ColumnNameFileRef], CultureInfo.InvariantCulture));
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                sourceURL.Add(ServiceUtility.RemoveEscapeCharacter(exception.Message));
            }
            return sourceURL;
        }

        /// <summary>
        /// Uploads the document to OneDrive (Users MySite document library).
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="collectionOfAttachments">Collection of documents with data for each document</param>
        /// <param name="collectionOfOriginalAttachments">Collection of documents with path of source document</param>
        /// <param name="usersMySite">User's My site URL</param>
        /// <param name="allAttachmentUrl">Attachment URL.</param>
        /// <param name="isOverwrite">Overwrite check</param>
        /// <returns>JSON string specifying path of OneDrive as success and false as failure</returns>
        internal static string UploadtoBriefcase(RequestObject requestObject, Dictionary<string, Stream> collectionOfAttachments, Dictionary<string, string> collectionOfOriginalAttachments, string usersMySite, string[] allAttachmentUrl, int isOverwrite)
        {
            string status = ConstantStrings.FALSE;
            try
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(usersMySite), requestObject.RefreshToken))
                {
                    Microsoft.SharePoint.Client.Web web = clientContext.Web;
                    string contentType = ServiceConstantStrings.OneDriveContentTypeName; // Get Content Type
                    string contentTypegroup = ServiceConstantStrings.OneDriveContentTypeGroup; // Get Group of Content Type
                    List<string> siteColumns = new List<string>(new string[] { ServiceConstantStrings.OneDriveSiteColumn });
                    bool isSiteColumnCreated = BriefcaseContentTypeHelperFunctions.CreateSiteColumn(clientContext, web, siteColumns);
                    if (isSiteColumnCreated)
                    {
                        bool isContentTypeCreated = BriefcaseContentTypeHelperFunctions.IsContentTypePresentCheck(clientContext, web, contentType, contentTypegroup, siteColumns);
                        if (isContentTypeCreated)
                        {
                            List list = web.Lists.GetByTitle(ServiceConstantStrings.OneDriveDocumentLibraryTitle);
                            string briefcaseFolderQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.BriefcaseFolderQuery, ServiceConstantStrings.LegalBriefcaseFolder);
                            ListItemCollection listItems = Lists.GetData(clientContext, ServiceConstantStrings.OneDriveDocumentLibraryTitle, briefcaseFolderQuery);
                            BriefcaseContentTypeHelperFunctions.SetContentType(clientContext, list, usersMySite);
                            string defaultContentTypeId = BriefcaseContentTypeHelperFunctions.SetOneDriveDefaultContentType(clientContext, list, contentType);
                            if (ConstantStrings.FALSE != defaultContentTypeId)
                            {
                                web.Update();
                                if (0 == isOverwrite)
                                {
                                    MailAttachmentDetails.CheckoutFailedPosition = 0;
                                    status = BriefcaseUtilityHelperFunctions.NewDocumentToOneDrive(clientContext, collectionOfAttachments, collectionOfOriginalAttachments, allAttachmentUrl, web, usersMySite, defaultContentTypeId);
                                    //// Undo check out in case of failure
                                    //// undo checkout all documents from position MailAttachmentDetails.checkOutFailedPosition
                                    BriefcaseHelperFunction.DiscardDocumentCheckout(requestObject, allAttachmentUrl);
                                }
                                else
                                {
                                    MailAttachmentDetails.CheckoutFailedPosition = 0;
                                    status = BriefcaseUtilityHelperFunctions.OverWriteDocument(collectionOfAttachments, collectionOfOriginalAttachments, usersMySite, status, clientContext, web, listItems, defaultContentTypeId);
                                    string[] returnedStatus = status.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture));
                                    if (ConstantStrings.TRUE == returnedStatus[0])
                                    {
                                        BriefcaseHelperFunction.DiscardDocumentCheckout(requestObject, allAttachmentUrl);
                                    }
                                    status = returnedStatus[1];
                                }
                            }
                            else
                            {
                                MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeAssignDefaultContentType, TextConstants.ErrorMessageAssignDefaultContentType);
                                throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                            }
                        }
                        else
                        {
                            MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeCreateSiteContentType, TextConstants.ErrorMessageCreateSiteContentType);
                            throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                        }
                    }
                    else
                    {
                        MatterCenterException customException = new MatterCenterException(TextConstants.ErrorCodeCreateSiteColumn, TextConstants.ErrorMessageCreateSiteColumn);
                        throw customException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                status += ConstantStrings.Semicolon + usersMySite;
            }
            return status;
        }

        /// <summary>
        /// Uploads the document to matter library.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="sourceUrl">URL of the source document</param>
        /// <param name="documentStream">Content stream of the document</param>
        /// <param name="versionInfo">The version information.</param>
        /// <param name="comments">The comments.</param>
        /// <param name="retainCheckOut">retain check out option</param>
        /// <returns>Content Type List for the Request Object containing SharePoint App Token</returns>
        internal static string UploadtoMatter(RequestObject requestObject, string sourceUrl, Stream documentStream, int versionInfo, string comments, bool retainCheckOut)
        {
            string status = ConstantStrings.FALSE;
            string result = ConstantStrings.FALSE;
            try
            {
                if (null != requestObject)
                {
                    ClientContext clientContext;
                    if (!string.IsNullOrWhiteSpace(sourceUrl) && null != documentStream)
                    {
                        string[] sourceUrlParts = sourceUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture));
                        if (2 == sourceUrlParts.Length)
                        {
                            using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(sourceUrlParts[0]), requestObject.RefreshToken))
                            {
                                Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(sourceUrlParts[1]);
                                string documentLibraryName = BriefcaseHelperFunction.getLibraryName(clientContext, file);
                                string contentType = string.Empty;
                                contentType = BriefcaseContentTypeHelperFunctions.ContentTypeByName(requestObject, clientContext, file.Name, sourceUrl, 1, contentType, documentLibraryName);
                                string listContentType = BriefcaseContentTypeHelperFunctions.GetContentTypeList(requestObject, clientContext, sourceUrl, contentType, 1, documentLibraryName);
                                status = BriefcaseContentTypeHelperFunctions.GetContentTypeList(requestObject, clientContext, sourceUrl, contentType, 2, documentLibraryName);
                                FileSaveBinaryInformation fileSaveBinaryInformation = new FileSaveBinaryInformation();
                                fileSaveBinaryInformation.ContentStream = documentStream;
                                file.SaveBinary(fileSaveBinaryInformation);
                                // Check if file is already checked out
                                if (file.CheckOutType == CheckOutType.None)
                                {
                                    file.CheckOut();
                                }
                                // Check the type of Check in to be performed
                                switch (versionInfo)
                                {
                                    case 0:
                                        file.CheckIn(comments, CheckinType.MinorCheckIn);
                                        break;
                                    case 1:
                                        file.CheckIn(comments, CheckinType.MajorCheckIn);
                                        break;
                                    case 2:
                                        file.CheckIn(comments, CheckinType.OverwriteCheckIn);
                                        break;
                                }
                                // Load the Stream data for the file                                                            
                                clientContext.ExecuteQuery();
                                status = BriefcaseContentTypeHelperFunctions.GetContentTypeList(requestObject, clientContext, sourceUrl, listContentType, 2, documentLibraryName);
                                // Check whether we need to retain checkout
                                if (retainCheckOut)
                                {
                                    file.CheckOut();
                                    clientContext.ExecuteQuery();
                                }
                                status = string.Concat(ConstantStrings.TRUE, ConstantStrings.Comma, ConstantStrings.Space, file.ServerRelativeUrl);
                            }
                        }
                        result = status;
                    }
                    else
                    {
                        status = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, TextConstants.MissingParametersMessage);
                        result = status;
                    }
                }
            }
            catch (Exception exception)
            {
                status = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
                result = status;
            }
            return result;
        }

        /// <summary>
        /// Returns the checkout operation status.
        /// </summary>
        /// <param name="commonResponse">Common response object</param>
        /// <param name="status">Status of the operation</param>
        /// <returns>Returns Common response object</returns>
        internal static CommonResponse CheckoutOperationStatus(CommonResponse commonResponse, string status)
        {
            CommonResponse result = null;
            try
            {
                if (ConstantStrings.TRUE == status.Split(ConstantStrings.Comma[0])[0])
                {
                    commonResponse.FileNames.Add(status.Split(ConstantStrings.Comma[0])[1]);
                }
                else
                {
                    if (ServiceConstantStrings.AlreadyCheckOut == status.Split(ConstantStrings.Comma[0])[1])
                    {
                        commonResponse.FileNames.Add(status.Split(ConstantStrings.Comma[0])[1]);
                    }
                    else
                    {
                        commonResponse.ErrorMessage += status.Split(ConstantStrings.Comma[0])[1];
                    }
                }
                result = commonResponse;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                CommonResponse CheckoutOperationStatusError = new CommonResponse();
                result = CheckoutOperationStatusError;
            }
            return result;
        }

        /// <summary>
        /// Updates the operation status.
        /// </summary>
        /// <param name="commonResponse">Common response object</param>
        /// <param name="status">Status of the operation</param>
        /// <returns>Returns Common response object</returns>
        internal static CommonResponse UpdateOperationStatus(CommonResponse commonResponse, string status)
        {
            CommonResponse result = null;
            try
            {
                Dictionary<string, object> responseJson = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(status);
                int code = Convert.ToInt32(responseJson[ConstantStrings.ResponseCode], CultureInfo.InvariantCulture);
                if (code == 0)
                {
                    commonResponse.Status.Add(true);
                }
                else
                {
                    commonResponse.Status.Add(false);
                    commonResponse.ErrorMessage += Convert.ToString(responseJson[ConstantStrings.ResponseValue], CultureInfo.InvariantCulture);
                }
                result = commonResponse;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                CommonResponse UpdateOperationStatusError = new CommonResponse();
                result = UpdateOperationStatusError;
            }
            return result;
        }

        /// <summary>
        /// Detaches the operation.
        /// </summary>
        /// <param name="commonResponse">Common response object</param>
        /// <param name="clientContext">Client context object</param>
        /// <param name="list">List object</param>
        /// <param name="listItems">List item collection object</param>
        /// <returns>Returns Common response object</returns>
        internal static CommonResponse DetachOperation(CommonResponse commonResponse, ClientContext clientContext, List list, ListItemCollection listItems)
        {
            CommonResponse detachOperationResponse = new CommonResponse();
            try
            {
                ContentType targetDocumentSetContentType = BriefcaseContentTypeHelperFunctions.GetContentType(list, clientContext);
                if (null != targetDocumentSetContentType)
                {
                    foreach (var listItem in listItems)
                    {
                        listItem[ServiceConstantStrings.OneDriveSiteColumn] = string.Empty;
                        listItem[ConstantStrings.OneDriveContentTypeProperty] = Convert.ToString(targetDocumentSetContentType.Id, CultureInfo.InvariantCulture);
                        listItem.Update();
                        clientContext.ExecuteQuery();
                        commonResponse.Status.Add(true);
                    }
                    detachOperationResponse = commonResponse;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return detachOperationResponse;
        }

        /// <summary>
        /// Gets the memory stream with specified content.
        /// </summary>
        /// <param name="fileContent">Error message to be displayed in file</param>
        /// <returns>Error file stream with error content</returns>
        internal static MemoryStream ReturnErrorFile(string fileContent)
        {
            MemoryStream result = null;
            try
            {
                byte[] errorMessage = System.Text.Encoding.UTF8.GetBytes(fileContent);
                MemoryStream mailFile = new MemoryStream(errorMessage);
                mailFile.Position = 0;
                WebOperationContext.Current.OutgoingResponse.ContentType = ConstantStrings.MailMediaType;
                WebOperationContext.Current.OutgoingResponse.Headers.Add(ConstantStrings.MailContentDispositionHeader);
                result = mailFile;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                MemoryStream ReturnErrorFileError = new MemoryStream();
                result = ReturnErrorFileError;
            }
            return result;
        }

        /// <summary>
        /// Discards checkout from the documents
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="allAttachmentUrl">List of attachment URL</param>
        /// <returns>Status of discarding check out</returns>
        internal static string DiscardDocumentCheckout(RequestObject requestObject, string[] allAttachmentUrl)
        {
            int currentCount = 0;
            string result = ConstantStrings.FALSE;

            try
            {
                foreach (string attachmentUrl in allAttachmentUrl)
                {
                    if (!string.IsNullOrWhiteSpace(attachmentUrl))
                    {
                        MailAttachmentDetails sendDocumentUrl = new MailAttachmentDetails();
                        sendDocumentUrl.FullUrl = attachmentUrl;
                        if (currentCount >= MailAttachmentDetails.CheckoutFailedPosition)
                        {
                            result = DiscardCheckout(requestObject, sendDocumentUrl);
                        }
                    }
                    currentCount++;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
            }
            return result;
        }

        /// <summary>
        /// Discard individual document from Matter Center
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="mailAttachmentDetails">Attachment object</param>
        /// <returns>Status of discarding individual document</returns>
        internal static string DiscardCheckout(RequestObject requestObject, MailAttachmentDetails mailAttachmentDetails)
        {
            string result = ConstantStrings.FALSE;
            ClientContext clientContext;
            try
            {
                using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(mailAttachmentDetails.FullUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken))
                {
                    Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(mailAttachmentDetails.FullUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].TrimEnd(ConstantStrings.Semicolon[0]));
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    if (file.CheckOutType != CheckOutType.None)
                    {
                        file.UndoCheckOut();
                        result = string.Concat(ConstantStrings.TRUE, ConstantStrings.Comma, ConstantStrings.Space, file.Name);
                    }
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
            }
            return result;
        }

        /// <summary>
        /// Get library title using parent list 
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="file">file object of Microsoft.Sharepoint.Client.File</param>
        /// <returns>Title of the document library</returns>
        internal static string getLibraryName(ClientContext clientContext, Microsoft.SharePoint.Client.File file)
        {
            string result = String.Empty;
            try
            {
                clientContext.Load(file);
                List documentLibrary = file.ListItemAllFields.ParentList;
                clientContext.Load(documentLibrary, libraryTitle => libraryTitle.Title);
                clientContext.ExecuteQuery();
                result = documentLibrary.Title;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
            }
            return result;
        }
    }
}