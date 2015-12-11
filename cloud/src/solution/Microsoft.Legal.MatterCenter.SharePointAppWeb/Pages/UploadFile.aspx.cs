// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-shpate
// Created          : 13-01-2015
//
// ***********************************************************************
// <copyright file="UploadFile.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of UploadFile.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;
    #endregion

    /// <summary>
    /// Provides functionality to upload file.
    /// </summary>
    public partial class UploadFile : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string requestValidationTokens = Request.Headers["RequestValidationToken"];
            if (ServiceUtility.ValidateRequestToken(requestValidationTokens))
            {
                string result = this.UploadFileToSharePoint(this.Request);
                Response.Write(Microsoft.Security.Application.Encoder.HtmlEncode(result));
            }
        }

        /// <summary>
        /// Uploads the file to SharePoint.
        /// </summary>
        /// <param name="request">Web request object</param>
        protected string UploadFileToSharePoint(HttpRequest request)
        {
            HttpFileCollection fileCollection = Request.Files;
            Regex regEx = new Regex("[*?|\\\t/:\"\"'<>#{}%~&]");
            string sharePointAppToken = Request.Form["SPAppToken"];
            string refreshToken = Request.Form["RefreshToken"];
            string clientUrl = Request.Form["ClientUrl"];
            string folderName = Request.Form["FolderName"];
            string documentLibraryName = Request.Form["DocumentLibraryName"];
            string originalName = string.Empty;
            bool allowContentCheck = Convert.ToBoolean(Request.Form["AllowContentCheck"], CultureInfo.InvariantCulture);
            bool continueUpload = true;
            Int16 isOverwrite = 3;
            IList<string> listResponse = new List<string>();
            string response = string.Empty;
            bool environment = Convert.ToBoolean(UIConstantStrings.IsDeployedOnAzure, CultureInfo.InvariantCulture);
            try
            {
                if (!environment || ((!string.IsNullOrWhiteSpace(sharePointAppToken) || !string.IsNullOrWhiteSpace(refreshToken)) && !string.IsNullOrWhiteSpace(clientUrl) && !string.IsNullOrWhiteSpace(folderName)))
                {
                    for (int fileCounter = 0; fileCounter < fileCollection.Count; fileCounter++)
                    {
                        if (!Int16.TryParse(Request.Form["Overwrite" + fileCounter], out isOverwrite))
                        {
                            isOverwrite = 3;
                        }
                        HttpPostedFile upload = fileCollection[fileCounter];
                        //// Added condition to check if upload.FileName returns the complete path or just the file name
                        string fileName = originalName = upload.FileName;
                        ContentCheckDetails contentCheck = new ContentCheckDetails(upload.FileName, upload.InputStream.Length);
                        string fileExtension = System.IO.Path.GetExtension(upload.FileName).Trim();
                        continueUpload = true;
                        if (-1 < fileName.IndexOf('\\'))
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                        }
                        else if (-1 < fileName.IndexOf('/'))
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                        }
                        if (null != upload.InputStream && 0 == upload.InputStream.Length)
                        {
                            listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.ErrorEmptyFile, ConstantStrings.DOLLAR, fileName));
                        }
                        else
                        {
                            if (regEx.IsMatch(fileName))
                            {
                                listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", UIConstantStrings.ErrorInvalidCharacter, ConstantStrings.DOLLAR, fileName));
                            }
                            else
                            {
                                using (ClientContext clientContext = ServiceUtility.GetClientContext(sharePointAppToken, new Uri(clientUrl), refreshToken, Request))
                                {
                                    string folder = folderName.Substring(folderName.LastIndexOf(ConstantStrings.ForwardSlash, StringComparison.OrdinalIgnoreCase) + 1);
                                    if (2 == isOverwrite)   //If User presses "Perform content check" option in overwrite Popup
                                    {
                                        continueUpload = false;
                                        response = PerformContentCheck(folderName, listResponse, response, upload, clientContext);
                                    }
                                    else if (3 == isOverwrite)  //If user presses "Cancel upload" option in overwrite popup or file is being uploaded for the first time
                                    {
                                        continueUpload = CheckDuplicateDocument(clientContext, folderName, documentLibraryName, listResponse, fileName, contentCheck, allowContentCheck);
                                    }
                                    else if (1 == isOverwrite)  //If User presses "Append date to file name and save" option in overwrite Popup
                                    {
                                        string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                                        string timeStampSuffix = DateTime.Now.ToString(UIConstantStrings.TimeStampFormat, CultureInfo.InvariantCulture).Replace(":", "_");
                                        fileName = fileNameWithoutExt + "_" + timeStampSuffix + fileExtension;
                                    }
                                    if (continueUpload)
                                    {
                                        listResponse = SetDocumentProperties(request, refreshToken, upload, fileExtension, originalName, listResponse, folderName, fileName, clientContext, folder, documentLibraryName);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var item in listResponse)
                    {
                        response += string.Concat(item, ConstantStrings.DOLLAR, ConstantStrings.Pipe, ConstantStrings.DOLLAR);
                    }
                }
                else
                {
                    response = UIConstantStrings.MessageNoInputs;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                response = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", exception.Message, ConstantStrings.DOLLAR, documentLibraryName);
            }
            return response;
        }

        /// <summary>
        /// Continue the upload functionality with setting of properties
        /// </summary>
        /// <param name="request">Current Http request object</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="environment">Flag to determine on-line or on-premise deployment</param>
        /// <param name="upload">Object for current file</param>
        /// <param name="fileExtension">Extension of current file</param>
        /// <param name="originalName">Original name of the file</param>
        /// <param name="listResponse">List of responses</param>
        /// <param name="folderName">Path of the folder</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="clientContext">SP client context</param>
        /// <param name="folder">Name of the folder</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <returns>list of all the responses being sent</returns>
        private static IList<string> SetDocumentProperties(HttpRequest request, string refreshToken, HttpPostedFile upload, string fileExtension, string originalName, IList<string> listResponse, string folderName, string fileName, ClientContext clientContext, string folder, string documentLibraryName)
        {
            Dictionary<string, string> mailProperties = ContinueUpload(request, refreshToken, upload, fileExtension);
            //setting original name property for attachment
            if (string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailOriginalName]))
            {
                mailProperties[ConstantStrings.MailOriginalName] = originalName;
            }
            return listResponse = listResponse.Concat(UploadDocument(folderName, upload, fileName, mailProperties, clientContext, folder, documentLibraryName)).ToList();
        }

        /// <summary>
        /// checks for duplicate document to upload
        /// </summary>
        /// <param name="folderName">Path of the folder</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="listResponse">List of responses</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="contentCheck">Content check object</param>
        /// <param name="clientContext">SP client context</param>
        /// <param name="folder">Name of the folder</param>
        /// <returns>true if duplicate document found</returns>
        private static bool CheckDuplicateDocument(ClientContext clientContext, string folderName, string documentLibraryName, IList<string> listResponse, string fileName, ContentCheckDetails contentCheck, bool allowContentCheck)
        {
            bool continueUpload = true;
            DuplicateDocument duplicateDocument = ServiceUtility.DocumentExists(clientContext, contentCheck, documentLibraryName, folderName, false);
            if (duplicateDocument.DocumentExists)
            {
                continueUpload = false;
                string documentPath = string.Concat(UIConstantStrings.SiteURL, folderName, ConstantStrings.ForwardSlash, fileName);
                string duplicateMessage = (allowContentCheck && duplicateDocument.HasPotentialDuplicate) ? ConstantStrings.FilePotentialDuplicateMessage : ConstantStrings.FileAlreadyExistMessage;
                listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", string.Format(CultureInfo.InvariantCulture, duplicateMessage, fileName, documentPath), ConstantStrings.SymbolAt, Convert.ToString(duplicateDocument.HasPotentialDuplicate, CultureInfo.InvariantCulture)));
            }
            return continueUpload;
        }

        /// <summary>
        /// Performs content check on stream level
        /// </summary>
        /// <param name="folderName">Path of the folder</param>
        /// <param name="listResponse">List of responses</param>
        /// <param name="response">Response object</param>
        /// <param name="upload">HttpPostedFile object</param>
        /// <param name="clientContext">SP client context</param>
        /// <returns>result string of content check operation</returns>
        private static string PerformContentCheck(string folderName, IList<string> listResponse, string response, HttpPostedFile upload, ClientContext clientContext)
        {
            using (MemoryStream targetStream = new MemoryStream())
            {
                Stream sourceStream = upload.InputStream;
                try
                {
                    byte[] buffer = new byte[sourceStream.Length + 1];
                    int read = 0;
                    while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        targetStream.Write(buffer, 0, read);
                    }
                    string serverFileUrl = folderName + ConstantStrings.ForwardSlash + upload.FileName;
                    if (ServiceUtility.PerformContentCheck(clientContext, targetStream, serverFileUrl))
                    {
                        listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.FoundIdenticalContent, ConstantStrings.Pipe, ConstantStrings.TRUE));
                    }
                    else
                    {
                        listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.FoundNonIdenticalContent, ConstantStrings.Pipe, ConstantStrings.FALSE));
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                    response = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.ContentCheckFailed, ConstantStrings.Pipe, ConstantStrings.TRUE);
                }
                finally
                {
                    sourceStream.Dispose();
                }
            }
            return response;
        }

        /// <summary>
        /// Method to be called if Overwrite flag is true
        /// </summary>
        /// <param name="request">Web request object</param>
        /// <param name="refreshToken">The refresh token for Client Context</param>
        /// <param name="environment">Flag to determine if deployed on Azure</param>
        /// <param name="upload">File parameters</param>
        /// <param name="fileName">Name of the file to be uploaded</param>
        /// <param name="fileExtension">Extension of the file being uploaded</param>
        /// <returns>Dictionary object having the key-value pair or mail properties</returns>
        private static Dictionary<string, string> ContinueUpload(HttpRequest request, string refreshToken, HttpPostedFile upload, string fileExtension)
        {
            Dictionary<string, string> mailProperties = new Dictionary<string, string>
                            {
                                { ConstantStrings.MailSenderKey, string.Empty },
                                { ConstantStrings.MailReceiverKey, string.Empty }, 
                                { ConstantStrings.MailReceivedDateKey, string.Empty }, 
                                { ConstantStrings.MailCCAddressKey, string.Empty }, 
                                { ConstantStrings.MailAttachmentKey, string.Empty }, 
                                { ConstantStrings.MailSearchEmailSubject, string.Empty },
                                { ConstantStrings.MailSearchEmailFromMailboxKey, string.Empty },
                                { ConstantStrings.MailFileExtensionKey, fileExtension },
                                { ConstantStrings.MailImportanceKey, string.Empty},
                                { ConstantStrings.MailConversationIdKey, string.Empty},
                                { ConstantStrings.MailConversationTopicKey, string.Empty},
                                { ConstantStrings.MailSentDateKey, string.Empty},
                                { ConstantStrings.MailHasAttachmentsKey, string.Empty},
                                { ConstantStrings.MailSensitivityKey, string.Empty },
                                { ConstantStrings.MailCategoriesKey, string.Empty },
                                { ConstantStrings.MailOriginalName, string.Empty}
                            };
            if (string.Equals(fileExtension, ConstantStrings.EmailFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                using (ClientContext clientContext = ServiceUtility.GetClientContext(null, new Uri(UIConstantStrings.CentralRepository), refreshToken, request))
                {
                    Users currentUserDetail = UIUtility.GetLoggedInUserDetails(clientContext);
                    mailProperties[ConstantStrings.MailSearchEmailFromMailboxKey] = currentUserDetail.Name;
                    Stream fileStream = upload.InputStream;
                    mailProperties = MailMessageParser.GetMailFileProperties(fileStream, mailProperties);       // Reading properties only for .eml file 
                }
            }
            return mailProperties;
        }

        /// <summary>
        /// Uploads document to SharePoint Library.
        /// </summary>
        /// <param name="folderPath">Relative path of folder</param>
        /// <param name="upload">File parameters</param>
        /// <param name="fileName">Name of the file being uploaded</param>
        /// <param name="mailProperties">Dictionary object having the key-value pair or mail properties</param>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="folderName">Target folder for the upload operation</param>
        /// <param name="documentLibraryName">Name of the document library no which upload operation is being performed</param>
        /// <returns>
        /// list of all the responses being sent
        /// </returns>
        internal static IList<string> UploadDocument(string folderPath, HttpPostedFile upload, string fileName, Dictionary<string, string> mailProperties, ClientContext clientContext, string folderName, string documentLibraryName)
        {
            IList<string> listResponse = new List<string>();
            bool isUploadSuccessful = false;
            try
            {
                Web web = clientContext.Web;
                var uploadFile = new FileCreationInformation();
                using (var stream = upload.InputStream)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    uploadFile.ContentStream = stream;
                    uploadFile.Url = fileName;
                    uploadFile.Overwrite = true;
                    using (clientContext)
                    {
                        isUploadSuccessful = DocumentUpload(folderPath, listResponse, clientContext, documentLibraryName, web, folderName, uploadFile);
                    }
                }
                if (isUploadSuccessful)
                {
                    ServiceUtility.SetUploadItemProperties(clientContext, web, documentLibraryName, fileName, folderPath, mailProperties);
                }
                listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", fileName, ConstantStrings.COLON, folderName));
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", exception.Message, ConstantStrings.DOLLAR, documentLibraryName));
            }
            return listResponse;
        }

        /// <summary>
        /// Upload helper function for uploading documents to SharePoint library.
        /// </summary>
        /// <param name="folderPath">Folder path of Document Library</param>
        /// <param name="listResponse">SharePoint list response</param>
        /// <param name="clientContext">Client context object for connection between SP & client</param>
        /// <param name="documentLibraryName">Name of document library in which upload is to be done</param>
        /// <param name="web">Object of site</param>
        /// <param name="folderName">Target folder name where file needs to be uploaded.</param>
        /// <param name="uploadFile">Object having file creation information</param>
        /// <returns>It returns true if upload is successful else false</returns>
        private static bool DocumentUpload(string folderPath, IList<string> listResponse, ClientContext clientContext, string documentLibraryName, Web web, string folderName, FileCreationInformation uploadFile)
        {
            bool isUploadSuccessful = false;
            using (clientContext)
            {
                if (UIUtility.FolderExists(folderPath, clientContext, documentLibraryName))
                {
                    Folder destionationFolder = clientContext.Web.GetFolderByServerRelativeUrl(folderPath);
                    clientContext.Load(destionationFolder);
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.File fileToUpload = destionationFolder.Files.Add(uploadFile);
                    destionationFolder.Update();
                    web.Update();
                    clientContext.Load(fileToUpload);
                    clientContext.ExecuteQuery();
                    isUploadSuccessful = true;
                }
                else
                {
                    listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}",
                                                string.Format(CultureInfo.InvariantCulture, ConstantStrings.FolderStructureModified, folderName),
                                                ConstantStrings.DOLLAR, folderName));
                }
            }
            return isUploadSuccessful;
        }
    }
}
