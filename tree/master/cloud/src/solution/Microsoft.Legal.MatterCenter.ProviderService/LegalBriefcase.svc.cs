// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-prd
// Created          : 24-06-2014
//
// ***********************************************************************
// <copyright file="LegalBriefcase.svc.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides the operation contracts for performing various actions from user's OneDrive.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    #endregion

    /// <summary>
    /// Provides the operation contracts for performing various actions from user's OneDrive.
    /// </summary>
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LegalBriefcase
    {
        #region Operation Contracts
        /// <summary>
        /// Sends attachments to users briefcase (OneDrive).
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="mailAttachmentDetails">Mail attachment details object containing attachment data</param>
        /// <param name="doCheckOut">Boolean value indicating whether check out needs to be performed or not</param>
        /// <returns>JSON string specifying success or failure</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string SendToBriefcase(RequestObject requestObject, MailAttachmentDetails mailAttachmentDetails, bool doCheckOut)
        {
            string status = ConstantStrings.FALSE;
            string result = string.Empty;
            try
            {
                if (null != requestObject && null != mailAttachmentDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    Dictionary<string, Stream> collectionOfAttachments = new Dictionary<string, Stream>();
                    Dictionary<string, string> collectionOfOriginalAttachments = new Dictionary<string, string>();
                    string[] allAttachmentUrl = mailAttachmentDetails.FullUrl.Split(ConstantStrings.Semicolon[0]);
                    ClientContext clientContext = null;
                    string usersMySite = string.Empty;
                    usersMySite = BriefcaseHelperFunction.GetPersonalURL(requestObject);
                    if (!usersMySite.ToUpperInvariant().Contains(ServiceConstantStrings.OneDriveNotSetupUrl.ToUpperInvariant()))
                    {
                        foreach (string attachmentUrl in allAttachmentUrl)
                        {
                            if (!string.IsNullOrWhiteSpace(attachmentUrl))
                            {
                                // Do not use 'using' to generate client context here since it will dispose the object before other functions get executed
                                clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken);
                                Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1]);
                                ClientResult<System.IO.Stream> clientResultStream = file.OpenBinaryStream();
                                ///// Load the Stream data for the file
                                clientContext.Load(file);
                                clientContext.ExecuteQuery();
                                Guid uniqueKey = Guid.NewGuid();
                                collectionOfAttachments.Add(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].Substring(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].LastIndexOf(Convert.ToChar(ConstantStrings.ForwardSlash, CultureInfo.InvariantCulture)) + 1) + ConstantStrings.DOLLAR + uniqueKey, clientResultStream.Value);
                                collectionOfOriginalAttachments.Add(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].Substring(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].LastIndexOf(Convert.ToChar(ConstantStrings.ForwardSlash, CultureInfo.InvariantCulture)) + 1) + ConstantStrings.DOLLAR + uniqueKey, attachmentUrl);
                                MailAttachmentDetails sendDocumentUrl = new MailAttachmentDetails();
                                sendDocumentUrl.FullUrl = attachmentUrl;
                                if (doCheckOut)
                                {
                                    this.CheckOutDocument(requestObject, sendDocumentUrl);
                                }
                            }
                        }
                        status = BriefcaseHelperFunction.UploadtoBriefcase(requestObject, collectionOfAttachments, collectionOfOriginalAttachments, usersMySite, allAttachmentUrl, mailAttachmentDetails.IsOverwrite);
                        //// If error not occurred while sending documents to OneDrive
                        if (!status.Contains(ConstantStrings.Code) && !status.Contains(ConstantStrings.Value))
                        {
                            status = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, 0, status);
                        }
                    }
                    else
                    {
                        status = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.ErrorCodeOneDriveNotConfigured, usersMySite);
                    }
                    result = status;
                }
                else
                {
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, 0, status);
                }
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Sends attachments from users briefcase (OneDrive) to DMS system.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="briefcaseDetails">Briefcase details object containing document data</param>
        /// <param name="versionInfo">The version information of the document</param>
        /// <param name="comments">The comments associated with the upload</param>
        /// <param name="retainCheckout">Retain Check out option</param>
        /// <param name="convenienceCopy">Convenience Copy option</param>
        /// <returns>JSON string specifying success or failure</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string SendToMatter(RequestObject requestObject, BriefcaseDetails[] briefcaseDetails, int versionInfo, string comments, bool retainCheckout, bool convenienceCopy)
        {
            CommonResponse checkInResponse = new CommonResponse();
            string status = string.Empty;
            try
            {
                if (null != requestObject && null != briefcaseDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    ClientContext clientContext = null;
                    string usersMySite = BriefcaseHelperFunction.GetPersonalURL(requestObject);
                    if (!string.IsNullOrEmpty(usersMySite))
                    {
                        for (int iterator = 0; iterator < briefcaseDetails.Length; iterator++)
                        {
                            if (!string.IsNullOrWhiteSpace(briefcaseDetails[iterator].DocumentUrl) && !string.IsNullOrWhiteSpace(briefcaseDetails[iterator].DocumentId))
                            {
                                using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(usersMySite), requestObject.RefreshToken))
                                {
                                    ClientResult<System.IO.Stream> data = Lists.GetStreamFromFile(clientContext, briefcaseDetails[iterator].DocumentUrl);
                                    string briefcaseItemQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.SendToMatterQuery, ServiceConstantStrings.OneDriveSiteColumn, briefcaseDetails[iterator].DocumentId);
                                    ListItemCollection listItems = Lists.GetData(clientContext, ServiceConstantStrings.OneDriveDocumentLibraryTitle, briefcaseItemQuery);
                                    string sourceUrl = string.Empty;
                                    foreach (var listItem in listItems)
                                    {
                                        sourceUrl = Convert.ToString(listItem[ServiceConstantStrings.OneDriveSiteColumn], CultureInfo.InvariantCulture);
                                    }
                                    if (!string.IsNullOrWhiteSpace(sourceUrl))
                                    {
                                        status = BriefcaseHelperFunction.UploadtoMatter(requestObject, sourceUrl, data.Value, versionInfo, comments, retainCheckout);
                                        checkInResponse.Status.Add(Convert.ToBoolean(status.Split(ConstantStrings.Comma[0])[0], CultureInfo.InvariantCulture));
                                        if (checkInResponse.Status[iterator])
                                        {
                                            checkInResponse.FileNames.Add(status.Split(ConstantStrings.Comma[0])[1]);
                                            if (!retainCheckout)
                                            {
                                                if (!convenienceCopy)
                                                {
                                                    foreach (var listitem in listItems)
                                                    {
                                                        listitem.DeleteObject();
                                                        clientContext.ExecuteQuery();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            checkInResponse.ErrorMessage += ConstantStrings.Space + status.Split(ConstantStrings.Comma[0])[1];
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        checkInResponse.ErrorMessage = ServiceConstantStrings.UserMySiteNotPresent;
                    }
                }
                else
                {
                    checkInResponse.Status.Add(false);
                    checkInResponse.ErrorMessage += ConstantStrings.Space + TextConstants.MissingParametersMessage;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                checkInResponse.Status.Add(false);
                checkInResponse.ErrorMessage += ConstantStrings.Space + ServiceUtility.RemoveEscapeCharacter(exception.Message);
            }
            return BriefcaseHelperFunction.GetSerializeResponse(checkInResponse);
        }

        /// <summary>
        /// Check out document from Matter Center.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="mailAttachmentDetails">Mail attachment details object containing attachment data</param>
        /// <returns>JSON string specifying success or failure</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string CheckOutDocument(RequestObject requestObject, MailAttachmentDetails mailAttachmentDetails)
        {
            string returnFlag = ConstantStrings.FALSE;
            if (null != requestObject && null != mailAttachmentDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
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
                        if (file.CheckOutType == CheckOutType.None)
                        {
                            file.CheckOut();
                            result = string.Concat(ConstantStrings.TRUE, ConstantStrings.Comma, ConstantStrings.Space, file.Name);
                        }
                        else
                        {
                            result = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ServiceConstantStrings.AlreadyCheckOut);
                        }
                        clientContext.ExecuteQuery();
                        returnFlag = result;
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    returnFlag = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
                }
            }
            else
            {
                returnFlag = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, TextConstants.InvalidParametersMessage);
            }
            return returnFlag;
        }

        /// <summary>
        /// Returns user who has checked out the document.
        /// </summary>
        /// <param name="requestObject">RequestObject object</param>
        /// <param name="mailAttachmentDetails">mailAttachmentDetails object</param>
        /// <returns>User who has checked out the document</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string[] CheckedOutDocumentByUser(RequestObject requestObject, MailAttachmentDetails mailAttachmentDetails)
        {
            string[] result = null;
            if (null != requestObject && null != mailAttachmentDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
            {
                string fileException;
                User checkedOutByUser = null;
                try
                {
                    List<string> checkedOutByUserTitle = new List<string>();
                    if (null != requestObject && null != mailAttachmentDetails)
                    {
                        // full URL, relative URL
                        string[] allAttachmentUrl = mailAttachmentDetails.FullUrl.Split(ConstantStrings.Semicolon[0]);
                        ClientContext clientContext;
                        foreach (string attachmentUrl in allAttachmentUrl)
                        {
                            try // To continue retrieve the document status for all documents even if one of the document is not present
                            {
                                if (!string.IsNullOrWhiteSpace(attachmentUrl))
                                {
                                    using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken))
                                    {
                                        string relativePath = attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1];
                                        string[] folderPath = relativePath.Split(Convert.ToChar(ConstantStrings.ForwardSlash, CultureInfo.InvariantCulture));
                                        bool checkExists = false;
                                        Folder folder = clientContext.Web.GetFolderByServerRelativeUrl(relativePath.Substring(0, relativePath.Length - folderPath[folderPath.Length - 1].Length - 1));
                                        FileCollection files = folder.Files;
                                        clientContext.Load(folder);
                                        clientContext.Load(files);
                                        clientContext.ExecuteQuery();
                                        foreach (Microsoft.SharePoint.Client.File file in files)
                                        {
                                            if (file.Name.ToUpperInvariant() == folderPath[folderPath.Length - 1].ToUpperInvariant())
                                            {
                                                checkExists = true;
                                                break;
                                            }
                                        }

                                        if (checkExists)
                                        {
                                            Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1]);
                                            clientContext.Load(file);
                                            clientContext.ExecuteQuery();
                                            if (file.CheckOutType == CheckOutType.None)
                                            {
                                                checkedOutByUserTitle.Add(string.Empty);
                                            }
                                            else
                                            {
                                                clientContext.ExecuteQuery();
                                                checkedOutByUser = file.CheckedOutByUser;
                                                clientContext.Load(checkedOutByUser);
                                                clientContext.ExecuteQuery();
                                                checkedOutByUserTitle.Add(checkedOutByUser.Title);
                                            }
                                        }
                                        else
                                        {
                                            checkedOutByUserTitle.Add(TextConstants.FileNotAvailableMessage);
                                        }
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                fileException = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                                checkedOutByUserTitle.Add(fileException);
                            }
                        }
                    }
                    result = checkedOutByUserTitle.ToArray();
                }
                catch (Exception exception)
                {
                    fileException = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    result = new[] { fileException };
                }
            }
            else
            {
                result = new[] { TextConstants.MessageNoInputs };
            }
            return result;
        }

        /// <summary>
        /// Checks whether normal documents are present in the selected document set.
        /// </summary>
        /// <param name="requestObject">Request Object</param>
        /// <param name="syncDetails">Sync Details object</param>
        /// <returns>True if there are no normal documents else false</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public bool CheckNormalDocuments(RequestObject requestObject, SyncDetails syncDetails)
        {
            ClientContext clientContext = null;
            bool response = false;
            try
            {
                if (null != requestObject && null != syncDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    string usersMySite = BriefcaseHelperFunction.GetPersonalURL(requestObject);
                    if (!string.IsNullOrEmpty(usersMySite))
                    {
                        using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(usersMySite), requestObject.RefreshToken))
                        {
                            List<string> sourceURL = BriefcaseHelperFunction.CheckSourceURL(clientContext, syncDetails, 1);
                            if (0 == sourceURL.Count)
                            {
                                response = true;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return response;
        }

        /// <summary>
        /// Checks and returns version information about the files.
        /// </summary>
        /// <param name="requestObject">RequestObject object</param>
        /// <param name="syncDetails">SyncDetails object</param>
        /// <returns>Status of the operation</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetVersion(RequestObject requestObject, SyncDetails syncDetails)
        {
            string response = string.Empty;
            VersionDetails versionDetails = new VersionDetails();
            try
            {
                if (null != requestObject && null != syncDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    versionDetails.Status = false;
                    ClientContext clientContext = null;
                    string usersMySite = BriefcaseHelperFunction.GetPersonalURL(requestObject);
                    if (!string.IsNullOrEmpty(usersMySite))
                    {
                        using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(usersMySite), requestObject.RefreshToken))
                        {
                            List<string> sourceURL = BriefcaseHelperFunction.CheckSourceURL(clientContext, syncDetails, 1);

                            if (sourceURL.Count != 0)
                            {
                                return BriefcaseHelperFunction.GetNotSupportedMessage(sourceURL);
                            }

                            sourceURL = BriefcaseHelperFunction.CheckSourceURL(clientContext, syncDetails, 2);
                            List<string> relativeURLList = BriefcaseHelperFunction.CheckSourceURL(clientContext, syncDetails, 3);
                            versionDetails.RelativeURL = relativeURLList;
                            for (int iterator = 0; iterator < sourceURL.Count; iterator++)
                            {
                                string url = sourceURL[iterator];
                                string relativeURL = Convert.ToString(url.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1], CultureInfo.InvariantCulture);
                                using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(url.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken))
                                {
                                    Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(relativeURL);
                                    string documentLibraryName = BriefcaseHelperFunction.getLibraryName(clientContext, file);
                                    List docLib = clientContext.Web.Lists.GetByTitle(documentLibraryName);
                                    clientContext.Load(docLib);
                                    clientContext.ExecuteQuery();
                                    versionDetails.IsMajorVersion.Add(docLib.EnableVersioning);
                                    versionDetails.IsMinorVersion.Add(docLib.EnableMinorVersions);
                                    versionDetails.Status = true;
                                    if (versionDetails.IsMajorVersion[iterator] || versionDetails.IsMinorVersion[iterator])
                                    {
                                        versionDetails.CurrentMajorVersion.Add(Convert.ToString(file.MajorVersion, CultureInfo.InvariantCulture));

                                        if (file.CheckOutType != CheckOutType.None)
                                        {
                                            if (0 == file.MinorVersion)
                                            {
                                                versionDetails.CurrentMinorVersion.Add(Convert.ToString(file.MinorVersion, CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                int currentMinorVersion = file.MinorVersion - 1;
                                                versionDetails.CurrentMinorVersion.Add(Convert.ToString(currentMinorVersion, CultureInfo.InvariantCulture));
                                            }

                                            clientContext.Load(file.CheckedOutByUser);
                                            clientContext.ExecuteQuery();
                                            User user = clientContext.Web.CurrentUser;
                                            clientContext.Load(user);
                                            clientContext.ExecuteQuery();
                                            if (user.Title == file.CheckedOutByUser.Title)
                                            {
                                                versionDetails.CheckOutStatus.Add(Convert.ToString(file.CheckedOutByUser.Title, CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                versionDetails.Status = false;
                                                versionDetails.ErrorMessage = ServiceConstantStrings.AlreadyCheckOut;
                                            }
                                        }
                                        else
                                        {
                                            versionDetails.CurrentMinorVersion.Add(Convert.ToString(file.MinorVersion, CultureInfo.InvariantCulture));
                                            versionDetails.CheckOutStatus.Add(ConstantStrings.Space);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        versionDetails.ErrorMessage = ServiceConstantStrings.UserMySiteNotPresent;
                    }
                }
                else
                {
                    versionDetails.Status = false;
                    versionDetails.ErrorMessage = TextConstants.MessageNoInputs;
                }
            }
            catch (Exception exception)
            {
                versionDetails.Status = false;
                versionDetails.ErrorMessage += ServiceUtility.RemoveEscapeCharacter(exception.Message);
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            response = BriefcaseHelperFunction.GetSerializeResponse(versionDetails);
            return response;
        }

        /// <summary>
        /// Obtains and returns relative URL of the files.
        /// </summary>
        /// <param name="requestObject">Request object used for creating context</param>
        /// <param name="listId">List Id</param>
        /// <param name="listItemId">Array of list item id</param>
        /// <param name="currentLocation">URL of the site collection</param>
        /// <returns>Relative URL of files or false</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetFileRelativeLocation(RequestObject requestObject, string listId, string[] listItemId, string currentLocation)
        {
            string response = string.Empty;
            bool hasFolder = false;
            try
            {
                if (null != requestObject && null != listId && null != listItemId && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    ClientContext clientContext = null;
                    using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(currentLocation), requestObject.RefreshToken))
                    {
                        List list = clientContext.Web.Lists.GetById(new Guid(listId));
                        int iterator = 0, length = listItemId.Length;
                        for (iterator = 0; iterator < length; iterator++)
                        {
                            ListItem listItem = list.GetItemById(listItemId[iterator]);
                            clientContext.Load(listItem, item => item.File.ServerRelativeUrl, item => item.FileSystemObjectType);
                            clientContext.ExecuteQuery();
                            if (listItem.FileSystemObjectType == FileSystemObjectType.File)
                            {
                                response += currentLocation + ConstantStrings.DOLLAR + listItem.File.ServerRelativeUrl;
                                if (iterator < length - 1)
                                {
                                    response += ConstantStrings.Semicolon;
                                }
                            }
                            else
                            {
                                hasFolder = true;
                                break;
                            }
                        }

                        if (hasFolder)
                        {
                            response = ConstantStrings.FALSE;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ////Generic Exception
                response = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            return response;
        }

        /// <summary>
        /// Performs legal briefcase operations mainly Check Out, Detach and Update document functionality.
        /// </summary>
        /// <param name="requestObject"> RequestObject object</param>
        /// <param name="syncDetails">SyncDetails object</param>
        /// <returns>Status of the operation in JSON format</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string SyncBriefcase(RequestObject requestObject, SyncDetails syncDetails)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (null != requestObject && null != syncDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    string usersMySite = string.Empty;
                    ClientContext clientContext = null;
                    usersMySite = BriefcaseHelperFunction.GetPersonalURL(requestObject);
                    if (!string.IsNullOrEmpty(usersMySite))
                    {
                        using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(usersMySite), requestObject.RefreshToken))
                        {
                            string status = string.Empty;
                            List<string> sourceURL = BriefcaseHelperFunction.CheckSourceURL(clientContext, syncDetails, 1);
                            if (sourceURL.Count != 0)
                            {
                                return BriefcaseHelperFunction.GetNotSupportedMessage(sourceURL);
                            }

                            GetContextInList(requestObject, syncDetails, ref commonResponse, clientContext, ref status);
                        }
                    }
                    else
                    {
                        commonResponse.ErrorMessage = ServiceConstantStrings.UserMySiteNotPresent;
                    }
                }
                else
                {
                    commonResponse.Status.Add(false);
                    commonResponse.ErrorMessage = TextConstants.InvalidParametersMessage;
                }
            }
            catch (Exception exception)
            {
                commonResponse.Status.Add(false);
                commonResponse.ErrorMessage = ServiceUtility.RemoveEscapeCharacter(exception.Message);
            }

            return BriefcaseHelperFunction.GetSerializeResponse(commonResponse);
        }

        /// <summary>
        /// Helper function to perform Check Out, Detach, and Update document functionality.
        /// </summary>
        /// <param name="requestObject">The Web request object.</param>
        /// <param name="syncDetails">SyncDetails object</param>
        /// <param name="commonResponse">Holds response for briefcase detach operation</param>
        /// <param name="clientContext">Client context object for connection between SP & client</param>
        /// <param name="status">Sets status for send to briefcase operation</param>
        private void GetContextInList(RequestObject requestObject, SyncDetails syncDetails, ref CommonResponse commonResponse, ClientContext clientContext, ref string status)
        {
            List list = clientContext.Web.Lists.GetById(new Guid(syncDetails.ListId));
            foreach (int itemid in syncDetails.ItemId)
            {
                ListItemCollection listItems = BriefcaseHelperFunction.GetListItemCollection(clientContext, itemid);
                MailAttachmentDetails mailattachmentDetails = new MailAttachmentDetails();
                ServiceConstantStrings.OperationTypes operationType = (ServiceConstantStrings.OperationTypes)syncDetails.Operation;
                if (operationType == ServiceConstantStrings.OperationTypes.Detach)
                {
                    commonResponse = BriefcaseHelperFunction.DetachOperation(commonResponse, clientContext, list, listItems);
                }
                else
                {
                    foreach (var listItem in listItems)
                    {
                        mailattachmentDetails.FullUrl = Convert.ToString(listItem[ServiceConstantStrings.OneDriveSiteColumn], CultureInfo.InvariantCulture);
                        mailattachmentDetails.IsOverwrite = 1;
                    }

                    switch (operationType)
                    {
                        case ServiceConstantStrings.OperationTypes.Update:
                            status = this.SendToBriefcase(requestObject, mailattachmentDetails, false);
                            commonResponse = BriefcaseHelperFunction.UpdateOperationStatus(commonResponse, status);
                            break;
                        case ServiceConstantStrings.OperationTypes.Checkout:
                            status = this.CheckOutDocument(requestObject, mailattachmentDetails);
                            commonResponse = BriefcaseHelperFunction.CheckoutOperationStatus(commonResponse, status);
                            commonResponse.Status.Add(Convert.ToBoolean(status.Split(',')[0], CultureInfo.InvariantCulture));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the mail object with attachments and send it as a stream.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="mailAttachmentDetails">Mail attachment details object containing attachment data</param>
        /// <returns>Memory stream of the created mail object</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public Stream SaveEmail(RequestObject requestObject, MailAttachmentDetails mailAttachmentDetails)
        {
            Stream result = null;
            try
            {
                if (null != requestObject && null != mailAttachmentDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    ///// filename, stream
                    Dictionary<string, Stream> collectionOfAttachments = new Dictionary<string, Stream>();
                    ///// full URL, relative URL
                    string[] allAttachmentUrl = mailAttachmentDetails.FullUrl.Split(';');
                    ClientContext clientContext;
                    bool attachmentFlag = mailAttachmentDetails.IsAttachmentCall;
                    if (attachmentFlag)
                    {
                        foreach (string attachmentUrl in allAttachmentUrl)
                        {
                            if (!string.IsNullOrWhiteSpace(attachmentUrl))
                            {
                                // Do not use 'using' to generate client context here since it will dispose the object before other functions get executed
                                clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken);
                                Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1]);
                                ClientResult<System.IO.Stream> fileStream = file.OpenBinaryStream();
                                ///// Load the Stream data for the file
                                clientContext.Load(file);
                                clientContext.ExecuteQuery();

                                ///// In order to allow for multiple files with the same name, we provide a GUID tag to ensure unique keys in the dictionary
                                string uniqueKeyWithDate = attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].Substring(attachmentUrl.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[1].LastIndexOf(Convert.ToChar(ConstantStrings.BackwardSlash, CultureInfo.InvariantCulture)) + 1) + ConstantStrings.DOLLAR + Guid.NewGuid();
                                collectionOfAttachments.Add(uniqueKeyWithDate, fileStream.Value);
                            }
                        }
                    }

                    result = MailHelperFunctions.GenerateEmail(collectionOfAttachments, allAttachmentUrl, attachmentFlag);
                }
                else
                {
                    result = BriefcaseHelperFunction.ReturnErrorFile(TextConstants.MessageNoInputs);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = BriefcaseHelperFunction.ReturnErrorFile(string.Concat(ConstantStrings.ServiceResponse, exception.HResult, exception.Message));
            }
            return result;
        }

        /// <summary>
        /// Discards checkout from the documents
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="mailAttachmentDetails">List of attachment URL</param>
        /// <returns>Status of discarding check out</returns>
        [OperationContract]
        [WebInvoke(
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json, Method = "*", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string DiscardCheckOutChanges(RequestObject requestObject, MailAttachmentDetails mailAttachmentDetails)
        {
            string result = null;
            try
            {
                if (null != requestObject && null != mailAttachmentDetails && ValidationHelperFunctions.CheckRequestValidatorToken())
                {
                    ///// full URL, relative URL
                    string[] allAttachmentUrl = mailAttachmentDetails.FullUrl.Split(';');

                    foreach (string attachmentUrl in allAttachmentUrl)
                    {
                        if (!string.IsNullOrWhiteSpace(attachmentUrl))
                        {
                            MailAttachmentDetails sendDocumentUrl = new MailAttachmentDetails();
                            sendDocumentUrl.FullUrl = attachmentUrl;
                            result = BriefcaseHelperFunction.DiscardCheckout(requestObject, sendDocumentUrl);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
            }
            return result;
        }
        #endregion
    }
}
