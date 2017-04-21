// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="UploadHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to perform document upload functionalities.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Web.Common
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    #region using

    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using Models;
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Xml;
    #endregion

    /// <summary>
    /// Provide methods to perform document transfer functionalities.
    /// </summary>
    public class UploadHelperFunctions:IUploadHelperFunctions
    {
        private ISPOAuthorization spoAuthorization;
        private ErrorSettings errorSettings;
        private IDocumentRepository documentRepository;
        private DocumentSettings documentSettings;
        private IUploadHelperFunctionsUtility uploadHelperFunctionsUtility;
        private IUserRepository userRepositoy;
        private IMailMessageRepository mailMessageRepository;

        public UploadHelperFunctions(ISPOAuthorization spoAuthorization, IOptions<ErrorSettings> errorSettings, IUserRepository userRepositoy,

            IDocumentRepository documentRepository, IOptions<DocumentSettings> documentSettings,
            IUploadHelperFunctionsUtility uploadHelperFunctionsUtility, IMailMessageRepository mailMessageRepository)
        {
            this.spoAuthorization = spoAuthorization;
            this.errorSettings = errorSettings.Value;
            this.documentRepository = documentRepository;
            this.documentSettings = documentSettings.Value;
            this.uploadHelperFunctionsUtility = uploadHelperFunctionsUtility;
            this.userRepositoy = userRepositoy;
            this.mailMessageRepository = mailMessageRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DuplicateDocument DocumentExists(string clientUrl, ContentCheckDetails contentCheck, string documentLibraryName, string folderName, bool isMail)
        {            
            ClientContext clientContext = spoAuthorization.GetClientContext(clientUrl);
            DuplicateDocument duplicateDocument = documentRepository.DocumentExists(clientContext, contentCheck, documentLibraryName, folderName, isMail);
            if (duplicateDocument.DocumentExists)
            {
                return duplicateDocument;
            }
            return null;
        }

        public GenericResponseVM PerformContentCheck(string clientUrl, string folderUrl, IFormFile uploadedFile, string fileName)
        {
            GenericResponseVM genericResponse = null;
            ClientContext clientContext = spoAuthorization.GetClientContext(clientUrl);
            using (MemoryStream targetStream = new MemoryStream())
            {
                Stream sourceStream = uploadedFile.OpenReadStream();
                try
                {
                    byte[] buffer = new byte[sourceStream.Length + 1];
                    int read = 0;
                    while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        targetStream.Write(buffer, 0, read);
                    }
                    string serverFileUrl = folderUrl + ServiceConstants.FORWARD_SLASH + fileName;
                    bool isMatched = documentRepository.PerformContentCheck(clientContext, targetStream, serverFileUrl);
                    if (isMatched)
                    {
                        //listResponse.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.FoundIdenticalContent, ConstantStrings.Pipe, ConstantStrings.TRUE));
                        genericResponse = new GenericResponseVM()
                        {
                            IsError = true,
                            Code = UploadEnums.IdenticalContent.ToString(),
                            Value = string.Format(CultureInfo.InvariantCulture, errorSettings.FoundIdenticalContent)
                        };
                    }
                    else
                    {
                        genericResponse = new GenericResponseVM()
                        {
                            IsError = true,
                            Code = UploadEnums.NonIdenticalContent.ToString(),
                            Value = string.Format(CultureInfo.InvariantCulture, errorSettings.FoundNonIdenticalContent)
                        };                        
                    }
                }
                catch (Exception exception)
                {
                    //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                    //response = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.ContentCheckFailed, ConstantStrings.Pipe, ConstantStrings.TRUE);
                }
                finally
                {
                    sourceStream.Dispose();
                }
            }
            return genericResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GenericResponseVM PerformContentCheck()
        {
            return null;
        }


        /// <summary>
        /// Acts as entry point from service to place the request to upload email/attachment. Reads the web request headers and requests applicable methods based on headers.
        /// </summary>
        /// <param name="requestObject">The request object.</param>
        /// <param name="client">The client object</param>
        /// <param name="serviceRequest">The Service request object</param>
        /// <param name="soapRequest">The SOAP request</param>
        /// <param name="attachmentOrMailID">The attachment or mail identifier.</param>
        /// <param name="isMailUpload">Mail upload check</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="isFirstCall">Determines if it is first call or not</param>
        /// <param name="message">Reference object for the message to be returned</param>
        /// <param name="originalFileName">Original file name of the attachment</param>
        /// <returns>It returns a string object, that contains the execution status of the function.</returns>
        public GenericResponseVM Upload(Client client, ServiceRequest serviceRequest, string soapRequest, string attachmentOrMailID, 
            bool isMailUpload, string fileName, string folderPath, bool isFirstCall, ref string message, string originalFileName)
        {
            GenericResponseVM genericResponse = null;
            bool result = ServiceConstants.UPLOAD_FAILED;
            try
            {
                if (null != client && null != serviceRequest && !string.IsNullOrWhiteSpace(soapRequest) && 
                    !string.IsNullOrWhiteSpace(attachmentOrMailID) && !string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(folderPath))
                {
                    string documentLibraryName = serviceRequest.DocumentLibraryName;

                    //// Make the request to the Exchange server and get the response.
                    HttpWebResponse webResponse = uploadHelperFunctionsUtility.GetWebResponse(serviceRequest.EwsUrl, serviceRequest.AttachmentToken, 
                        soapRequest, attachmentOrMailID);

                    if (!isFirstCall)
                    {
                        XmlDocument xmlDocument = RetrieveXMLDocument(webResponse);
                        string attachmentID = string.Empty;
                        //// Check original file name is empty
                        if (!string.IsNullOrWhiteSpace(originalFileName))
                        {
                            attachmentID = uploadHelperFunctionsUtility.GetAttachmentID(xmlDocument, originalFileName);
                        }
                        else
                        {
                            attachmentID = uploadHelperFunctionsUtility.GetAttachmentID(xmlDocument, fileName);
                        }
                        if (!string.IsNullOrWhiteSpace(attachmentID))
                        {
                            attachmentOrMailID = attachmentID;
                        }

                        //// Make the request to the Exchange server and get the response.
                        webResponse = uploadHelperFunctionsUtility.GetWebResponse(serviceRequest.EwsUrl, serviceRequest.AttachmentToken, 
                            ServiceConstants.ATTACHMENT_SOAP_REQUEST, attachmentOrMailID);
                    }

                    //// If the response is okay, create an XML document from the response and process the request.
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        genericResponse = UploadFilesMail(serviceRequest, documentLibraryName, webResponse, isMailUpload,
                            client, fileName, folderPath, ref message);
                    }
                    if (genericResponse!=null && genericResponse.IsError == true && genericResponse.Code==UploadEnums.UploadFailure.ToString() && isFirstCall)
                    {
                        genericResponse = Upload(client, serviceRequest, ServiceConstants.MAIL_SOAP_REQUEST, serviceRequest.MailId, 
                            isMailUpload, fileName, folderPath, false, ref message, originalFileName);
                    }
                }
                else
                {
                    result = ServiceConstants.UPLOAD_FAILED;
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                throw;
            }
            return genericResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <param name="documentLibraryName"></param>
        /// <param name="webResponse"></param>
        /// <param name="isMailUpload"></param>
        /// <param name="client"></param>
        /// <param name="fileName"></param>
        /// <param name="folderPath"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        internal GenericResponseVM UploadFilesMail(ServiceRequest serviceRequest,
            string documentLibraryName, HttpWebResponse webResponse, bool isMailUpload,
            Client client, string fileName, string folderPath, ref string message)
        {
            bool isMsg = true;
            MailMetaData mailMetaData = new MailMetaData();
            var bytes = (dynamic)null;
            string mailMessage = string.Empty;
            string originalName = string.Empty;
            string xmlPath = string.Empty;
            GenericResponseVM genericResponse = null;
            ContentCheckDetails contentCheck = null;
            try
            {
                XmlDocument xmlDocument = RetrieveXMLDocument(webResponse);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("s", ServiceConstants.SOAP_ENVELOP_URI);
                nsmgr.AddNamespace("m", ServiceConstants.EXCHANGE_SERVICE_MESSAGE);
                nsmgr.AddNamespace("t", ServiceConstants.EXCHANGE_SERVICE_TYPES);
                string extension = System.IO.Path.GetExtension(fileName).Trim();
                string uploadFileName = uploadHelperFunctionsUtility.RemoveSpecialChar(fileName);
                if (xmlDocument.SelectSingleNode("/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:FileAttachment/t:Content", nsmgr) != null)
                {
                    isMsg = false;
                }
                if (string.IsNullOrEmpty(extension) && isMsg)
                {
                    uploadFileName = uploadFileName + ServiceConstants.EMAIL_FILE_EXTENSION;
                }
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    if (isMailUpload)
                    {
                        bytes = GetEmailProperties(xmlDocument, ref mailMetaData);
                    }
                    else
                    {
                        bytes = uploadHelperFunctionsUtility.GetStream(xmlDocument, nsmgr, isMailUpload, extension, isMsg);
                    }
                    if (null != bytes)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(bytes))
                        {
                            contentCheck = new ContentCheckDetails(uploadFileName, mailMetaData.mailSubject, memoryStream.Length, mailMetaData.mailSender, mailMetaData.sentDate);
                        }
                    }

                    genericResponse = uploadHelperFunctionsUtility.CheckDuplicateDocument(clientContext, documentLibraryName, isMailUpload, folderPath, contentCheck, uploadFileName, serviceRequest.AllowContentCheck, ref message);

                    if (!serviceRequest.Overwrite && !serviceRequest.PerformContentCheck && (genericResponse != null && genericResponse.IsError == true))
                    {
                        return genericResponse;
                    }
                    else if (serviceRequest.PerformContentCheck)
                    {
                        genericResponse = uploadHelperFunctionsUtility.PerformContentCheckUtility(isMailUpload, folderPath, isMsg, xmlDocument, nsmgr, extension, uploadFileName, clientContext);
                        return genericResponse;
                    }
                    else
                    {
                        genericResponse = null;
                        if (isMailUpload)       //Upload entire Email
                        {
                            UploadMail(client, folderPath, fileName, documentLibraryName, xmlDocument, ref message, serviceRequest);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(extension) && isMsg)       //Upload .msg file
                            {
                                UploadAttachedMailExtBlank(client, folderPath, fileName, documentLibraryName, xmlDocument, ref message, serviceRequest);
                            }
                            else
                            {
                                if (string.Equals(extension, ServiceConstants.EMAIL_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                                {
                                    UploadEMLFile(documentLibraryName, client, folderPath, fileName, ref message, xmlDocument, nsmgr,
                                        ref mailMetaData, ref bytes, extension, serviceRequest);
                                }
                                else
                                {
                                    //get original name
                                    xmlPath = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:FileAttachment/t:Name";
                                    originalName = RetrieveNodeValue(xmlDocument, nsmgr, xmlPath, true);
                                    //get attachment content
                                    xmlPath = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:FileAttachment/t:Content";
                                    mailMessage = RetrieveNodeValue(xmlDocument, nsmgr, xmlPath, false);

                                    bytes = Convert.FromBase64String(mailMessage);
                                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                                    {
                                        mailMetaData.mailImportance = string.Empty;
                                        mailMetaData.mailSubject = string.Empty;
                                        mailMetaData.originalName = originalName;
                                        if (!UploadToFolder(client, folderPath, fileName, string.Empty, memoryStream,
                                            documentLibraryName, mailMetaData, ref message, serviceRequest))
                                        {
                                            //result = ServiceConstants.UPLOAD_FAILED;
                                            genericResponse = new GenericResponseVM()
                                            {
                                                IsError = true,
                                                Code = UploadEnums.UploadToFolder.ToString(),
                                                Value = message
                                            };
                                            return genericResponse;
                                        }
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(message))
                        {
                            //result = ServiceConstants.UPLOAD_FAILED;
                            genericResponse = new GenericResponseVM()
                            {
                                IsError = true,
                                Code = UploadEnums.UploadFailure.ToString(),
                                Value = message
                            };
                            return genericResponse;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                genericResponse = new GenericResponseVM()
                {
                    IsError = true,
                    Code = UploadEnums.UploadFailure.ToString(),
                    Value = ""
                };
                
            }
            return genericResponse;
        }

        /// <summary>
        /// Retrieves the specified node value from XML document
        /// </summary>
        /// <param name="xmlDocument">XML document object having information for the attachment</param>
        /// <param name="nsmgr">XML namespace object</param>
        /// <param name="xmlPath">xPath for getting the specific property of mail
        /// <param name="decodeNodeValue">Flag to check if node value requires HTML decoding</param>
        /// <returns>string containing property value specified in xmlPath</returns>
        private static string RetrieveNodeValue(XmlDocument xmlDocument, XmlNamespaceManager nsmgr, string xmlPath, bool decodeNodeValue)
        {
            string xmlNodeValue = string.Empty;
            XmlNode nodeToFetch = xmlDocument.SelectSingleNode(xmlPath, nsmgr);
            if (null != nodeToFetch)
            {
                xmlNodeValue = nodeToFetch.InnerXml;
                if (decodeNodeValue && !string.IsNullOrWhiteSpace(xmlNodeValue))
                {
                    xmlNodeValue = WebUtility.HtmlDecode(xmlNodeValue);
                }
            }
            return xmlNodeValue;
        }

        /// <summary>
        /// Retrieves stream from web page and loads in XML document.
        /// </summary>
        /// <param name="webResponse">HTTP web response to get the response stream</param>
        /// <returns>XML Document containing read values from Request</returns>
        internal static XmlDocument RetrieveXMLDocument(HttpWebResponse webResponse)
        {
            Stream responseStream = webResponse.GetResponseStream();
            XmlDocument xmlDocument = new XmlDocument();
            XmlReader xmlReader = XmlReader.Create(responseStream);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.DtdProcessing = DtdProcessing.Ignore;
            xmlDocument.Load(xmlReader);
            responseStream.Close();
            webResponse.Close();
            return xmlDocument;
        }

        /// <summary>
        /// Uploads the .eml file to specified folder in matter library.
        /// </summary>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="requestObject">web request object</param>
        /// <param name="client">Provider Service client Object</param>
        /// <param name="folderPath">target folder path for upload</param>
        /// <param name="fileName">File name</param>
        /// <param name="message">Reference object for the message to be returned</param>
        /// <param name="xmlDocument">XML document object having information for the attachment</param>
        /// <param name="nsmgr">XML Namespace object</param>
        /// <param name="mailMetaData">Mail metadata object storing property values</param>
        /// <param name="bytes">Array of bytes</param>
        /// <param name="extension">File extension object contains extension of file to be uploaded.</param>
        private void UploadEMLFile(string documentLibraryName, Client client, string folderPath, string fileName,
            ref string message, XmlDocument xmlDocument, XmlNamespaceManager nsmgr, ref MailMetaData mailMetaData,
            ref dynamic bytes, string extension, ServiceRequest serviceRequest)
        {
            string mailMessage = xmlDocument.SelectSingleNode("/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:FileAttachment/t:Content", nsmgr).InnerXml;
            bytes = Convert.FromBase64String(mailMessage);
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                mailMetaData.mailSender = string.Empty;
                mailMetaData.mailReceiver = string.Empty;
                mailMetaData.receivedDate = string.Empty;
                mailMetaData.cc = string.Empty;
                Dictionary<string, string> mailProperties = new Dictionary<string, string>
                                    {
                                        { ServiceConstants.MAIL_SENDER_KEY, mailMetaData.mailSender }, 
                                        { ServiceConstants.MAIL_RECEIVER_KEY, mailMetaData.mailReceiver }, 
                                        { ServiceConstants.MAIL_RECEIVED_DATEKEY, mailMetaData.receivedDate },
                                        { ServiceConstants.MAIL_CC_ADDRESS_KEY, mailMetaData.cc },
                                        { ServiceConstants.MAIL_CATEGORIES_KEY, string.Empty },
                                        { ServiceConstants.MAIL_SENSITIVITY_KEY, string.Empty },
                                        { ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY, string.Empty },
                                        { ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT, string.Empty },
                                        { ServiceConstants.MAIL_FILE_EXTENSION_KEY, extension },
                                        { ServiceConstants.MAIL_IMPORTANCE_KEY, string.Empty},
                                        { ServiceConstants.MAIL_CONVERSATIONID_KEY, string.Empty},
                                        { ServiceConstants.MAIL_CONVERSATION_TOPIC_KEY, string.Empty},
                                        { ServiceConstants.MAIL_SENT_DATE_KEY, string.Empty},
                                        { ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY, string.Empty},
                                        { ServiceConstants.MAIL_ORIGINAL_NAME, string.Empty}
                                    };

                mailProperties = MailMessageParser.GetMailFileProperties(memoryStream, mailProperties);
                mailMetaData.mailImportance = mailProperties[ServiceConstants.MAIL_IMPORTANCE_KEY];
                mailMetaData.mailReceiver = mailProperties[ServiceConstants.MAIL_RECEIVER_KEY];
                mailMetaData.receivedDate = mailProperties[ServiceConstants.MAIL_RECEIVED_DATEKEY];
                mailMetaData.cc = mailProperties[ServiceConstants.MAIL_CC_ADDRESS_KEY];
                mailMetaData.categories = mailProperties[ServiceConstants.MAIL_CATEGORIES_KEY];
                mailMetaData.mailSubject = mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT];
                mailMetaData.attachment = mailProperties[ServiceConstants.MAIL_ATTACHMENT_KEY];
                mailMetaData.mailSender = mailProperties[ServiceConstants.MAIL_SENDER_KEY];
                mailMetaData.sentDate = mailProperties[ServiceConstants.MAIL_SENT_DATE_KEY];
                mailMetaData.originalName = mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT];
                UploadToFolder(client, folderPath, fileName, mailProperties[ServiceConstants.MAIL_FILE_EXTENSION_KEY],
                    memoryStream, documentLibraryName, mailMetaData, ref message, serviceRequest);
            }
        }

        /// <summary>
        /// Uploads the email to specified folder in matter library.
        /// </summary>
        /// <param name="requestObject">web request object</param>
        /// <param name="client">Provider Service client Object</param>
        /// <param name="folderPath">target folder path for upload</param>
        /// <param name="filename">Name of the file</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="xmlDocument">Mail XML Content</param>
        /// <param name="message">Reference object for the message to be returned</param>
        internal void UploadMail(Client client, string folderPath, string fileName, string documentLibraryName,
            XmlDocument xmlDocument, ref string message, ServiceRequest serviceRequest)
        {
            var bytes = (dynamic)null;

            try
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("s", ServiceConstants.SOAP_ENVELOP_URI);
                nsmgr.AddNamespace("m", ServiceConstants.EXCHANGE_SERVICE_MESSAGE);
                nsmgr.AddNamespace("t", ServiceConstants.EXCHANGE_SERVICE_TYPES);
                MailMetaData mailMetaData = new MailMetaData();

                bytes = GetEmailProperties(xmlDocument, ref mailMetaData);

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    using (MailMimeReader mime = new MailMimeReader())
                    {
                        MailMessageParser messageParser = mime.GetEmail(memoryStream);
                        mailMetaData.mailImportance = (string.IsNullOrWhiteSpace(messageParser.MailImportance)) ? ServiceConstants.MAIL_DEFAULT_IMPORTANCE : messageParser.MailImportance;
                        mailMetaData.receivedDate = (string.IsNullOrWhiteSpace(messageParser.ReceivedDate.Date.ToShortDateString())) ? string.Empty : Convert.ToString(messageParser.ReceivedDate, CultureInfo.InvariantCulture);
                        UploadToFolder(client, folderPath, fileName, string.Empty, memoryStream, documentLibraryName, mailMetaData, ref message, serviceRequest);
                    }
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Get properties of the Email being uploaded
        /// </summary>
        /// <param name="xmlDocument">XML Document containing read values from Request</param>
        /// <param name="mailMetaData">Mail metadata object</param>
        /// <returns>Array of bytes</returns>
        private static dynamic GetEmailProperties(XmlDocument xmlDocument, ref MailMetaData mailMetaData)
        {
            var bytes = (dynamic)null;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("s", ServiceConstants.SOAP_ENVELOP_URI);
            nsmgr.AddNamespace("m", ServiceConstants.EXCHANGE_SERVICE_MESSAGE);
            nsmgr.AddNamespace("t", ServiceConstants.EXCHANGE_SERVICE_TYPES);
            MailXPath mailXPath = new MailXPath();
            mailXPath.mailReceiver = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:ToRecipients/t:Mailbox";
            mailXPath.mailCC = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:CcRecipients/t:Mailbox";
            mailXPath.mailSentDate = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:DateTimeSent";
            mailXPath.mailFromName = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:From/t:Mailbox/t:Name";
            mailXPath.mailFromAddress = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:From/t:Mailbox/t:EmailAddress";
            mailXPath.mailSubject = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:Subject";
            mailXPath.mailCategories = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:Categories";
            mailXPath.mailConversationId = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:ConversationId";
            mailXPath.mailConversationTopic = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:ConversationTopic";
            mailXPath.mailSensitivity = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:Sensitivity";
            mailXPath.mailHasAttachments = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:HasAttachments";
            mailXPath.mailImportance = string.Empty;
            bytes = Convert.FromBase64String(xmlDocument.SelectSingleNode("/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:MimeContent", nsmgr).InnerXml);
            GetMailProperties(mailXPath, ref mailMetaData, xmlDocument);

            return bytes;
        }

        /// <summary>
        /// Uploads the .msg file to specified folder in matter library.
        /// </summary>
        /// <param name="requestObject">web request object</param>
        /// <param name="client">service client object</param>
        /// <param name="folderPath">target folder path</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="xmlDocument">XML Document containing read values from Request</param>
        /// <param name="message">Reference object for the message to be returned</param> 
        internal void UploadAttachedMailExtBlank(Client client, string folderPath, string fileName, string documentLibraryName,
            XmlDocument xmlDocument, ref string message, ServiceRequest serviceRequest)
        {
            var bytes = (dynamic)null;
            MailMetaData mailMetaData = new MailMetaData();

            try
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("s", ServiceConstants.SOAP_ENVELOP_URI);
                nsmgr.AddNamespace("m", ServiceConstants.EXCHANGE_SERVICE_MESSAGE);
                nsmgr.AddNamespace("t", ServiceConstants.EXCHANGE_SERVICE_TYPES);
                MailXPath xPath = new MailXPath();
                xPath.mailReceiver = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:ToRecipients/t:Mailbox";
                xPath.mailCC = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:CcRecipients/t:Mailbox";
                xPath.mailRecieved = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:DateTimeReceived";
                xPath.mailFromName = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:From/t:Mailbox/t:Name";
                xPath.mailFromAddress = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:From/t:Mailbox/t:EmailAddress";
                xPath.mailImportance = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:Importance";
                xPath.mailSubject = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:Subject";
                xPath.mailCategories = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:Categories";
                xPath.mailConversationId = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:ConversationId";
                xPath.mailConversationTopic = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:ConversationTopic";
                xPath.mailSensitivity = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:Sensitivity";
                xPath.mailHasAttachments = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:HasAttachments";
                xPath.mailSentDate = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:DateTimeSent";
                bytes = Convert.FromBase64String(xmlDocument.SelectSingleNode("/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:MimeContent", nsmgr).InnerXml);
                GetMailProperties(xPath, ref mailMetaData, xmlDocument);
                mailMetaData.mailImportance = (string.IsNullOrWhiteSpace(mailMetaData.mailImportance)) ? ServiceConstants.MAIL_DEFAULT_IMPORTANCE : mailMetaData.mailImportance;
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    UploadToFolder(client, folderPath, fileName + ServiceConstants.EMAIL_FILE_EXTENSION,
                        string.Empty, memoryStream, documentLibraryName, mailMetaData, ref message, serviceRequest);
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Get properties of the mail file to be uploaded
        /// </summary>
        /// <param name="mailXPath">Mail Xpath object</param>
        /// <param name="mailMetaData">Mail metadata object</param>
        /// <param name="xmlDocument">XML Document containing read values from Request</param>
        private static void GetMailProperties(MailXPath mailXPath, ref MailMetaData mailMetaData, XmlDocument xmlDocument)
        {

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("s", ServiceConstants.SOAP_ENVELOP_URI);
            nsmgr.AddNamespace("m", ServiceConstants.EXCHANGE_SERVICE_MESSAGE);
            nsmgr.AddNamespace("t", ServiceConstants.EXCHANGE_SERVICE_TYPES);

            XmlNode checkMailFromAddressField = xmlDocument.SelectSingleNode(mailXPath.mailFromAddress, nsmgr);
            XmlNode xmlNode = null;
            XmlNodeList xmlNodeList = null;

            mailMetaData.sentDate = GetPropertyValueFromXml(mailXPath.mailSentDate, xmlDocument, nsmgr);
            mailMetaData.mailImportance = GetPropertyValueFromXml(mailXPath.mailImportance, xmlDocument, nsmgr);
            mailMetaData.mailSubject = GetPropertyValueFromXml(mailXPath.mailSubject, xmlDocument, nsmgr);
            mailMetaData.receivedDate = GetPropertyValueFromXml(mailXPath.mailRecieved, xmlDocument, nsmgr);
            mailMetaData.sensitivity = GetPropertyValueFromXml(mailXPath.mailSensitivity, xmlDocument, nsmgr);
            mailMetaData.conversationTopic = GetPropertyValueFromXml(mailXPath.mailConversationTopic, xmlDocument, nsmgr);
            mailMetaData.hasAttachments = GetPropertyValueFromXml(mailXPath.mailHasAttachments, xmlDocument, nsmgr);
            //Subject is kept as the original name of the mail
            mailMetaData.originalName = GetPropertyValueFromXml(mailXPath.mailSubject, xmlDocument, nsmgr);

            xmlNode = xmlDocument.SelectSingleNode(mailXPath.mailFromName, nsmgr);
            if (null != xmlNode && null != xmlNode)
            {
                mailMetaData.mailSender = xmlNode.InnerXml;
                mailMetaData.mailSender += ServiceConstants.SEMICOLON + checkMailFromAddressField.InnerXml;
            }

            xmlNodeList = xmlDocument.SelectNodes(mailXPath.mailReceiver, nsmgr);
            mailMetaData.mailReceiver = GetRecipientsField(xmlNodeList);

            xmlNodeList = xmlDocument.SelectNodes(mailXPath.mailCC, nsmgr);
            mailMetaData.cc = GetRecipientsField(xmlNodeList);

            xmlNode = xmlDocument.SelectSingleNode(mailXPath.mailConversationId, nsmgr);
            if (null != xmlNode)
            {
                mailMetaData.conversationId = Convert.ToString(xmlNode.Attributes["Id"].Value, CultureInfo.InvariantCulture);
            }


            xmlNode = xmlDocument.SelectSingleNode(mailXPath.mailCategories, nsmgr);
            if (null != xmlNode)
            {
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    mailMetaData.categories += node.InnerText + ServiceConstants.SEMICOLON;
                }
            }

            XmlNodeList xnlist = xmlDocument.GetElementsByTagName("t:Name");
            foreach (XmlNode node in xnlist)
            {
                if (null != node.PreviousSibling && "t:AttachmentId" == node.PreviousSibling.Name)
                {
                    mailMetaData.attachment += node.InnerText + ServiceConstants.SEMICOLON;
                }
            }
        }

        /// <summary>
        /// To get recipients field from the Email headers
        /// </summary>
        /// <param name="xmlNodeList">Collection of nodes</param>
        /// <returns>Property value</returns>
        private static string GetRecipientsField(XmlNodeList xmlNodeList)
        {
            string result = string.Empty;
            if (null != xmlNodeList)
            {
                foreach (XmlNode node in xmlNodeList)
                {
                    if (null != node["t:Name"])
                    {
                        result += node["t:Name"].InnerXml + ServiceConstants.SEMICOLON + node["t:EmailAddress"].InnerXml + ServiceConstants.SEMICOLON;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// To get property value from the XML node
        /// </summary>
        /// <param name="xPath">XPath query</param>
        /// <param name="document">XML Document containing read values from Request</param>
        /// <param name="namespaceManager">Namespace manager object</param>
        /// <returns>Property value</returns>
        private static string GetPropertyValueFromXml(string xPath, XmlDocument document, XmlNamespaceManager namespaceManager)
        {
            string propertyValue = string.Empty;
            XmlNode xmlNode = null;
            if (null != xPath && !string.IsNullOrWhiteSpace(xPath))
            {
                xmlNode = document.SelectSingleNode(xPath, namespaceManager);
                if (null != xmlNode)
                {
                    propertyValue = xmlNode.InnerText;
                }
            }
            return propertyValue;
        }

        /// <summary>
        /// To get the short date from the field retrieved from headers
        /// </summary>        
        /// <param name="xmlNode">Representing node from XML document</param>
        /// <returns>Short date format</returns>
        private static string GetShortDateFromField(string value)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime parsedDate = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
                result = parsedDate.ToShortDateString();
            }
            return result;
        }

        /// <summary>
        /// Validates the email/attachment and request call to upload the content to specified folder in matter library.
        /// </summary>
        /// <param name="requestObject">The Web request object.</param>
        /// <param name="client">The Service client.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="filename">The filename</param>
        /// <param name="extension">File extension</param>
        /// <param name="memoryStream">Memory stream object</param>
        /// <param name="mailMetaData">MailMetadata object</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="message">Reference object for the message to be returned</param>
        /// <returns>
        /// Returns True if upload is successful or False if upload fails
        /// </returns>
        internal bool UploadToFolder(Client client, string folderPath, string filename, string extension,
            MemoryStream memoryStream, string documentLibraryName, MailMetaData mailMetaData, ref string message, ServiceRequest serviceRequest)
        {
            bool isUploadSuccessful = false;
            try
            {
                if (null != memoryStream && null != client && !string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(filename))
                {
                    if (0 == memoryStream.Length)
                    {
                        message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", errorSettings.ErrorEmptyFile, ServiceConstants.DOLLAR, documentLibraryName);
                    }
                    else
                    {
                        using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                        {
                            isUploadSuccessful = UploadFolderUtility(client, folderPath, ref filename, extension, memoryStream, mailMetaData, documentLibraryName, ref message, clientContext, serviceRequest);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", exception.Message, ServiceConstants.DOLLAR, documentLibraryName);
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return isUploadSuccessful;
        }

        /// <summary>
        /// Checks for folder existence and uploads the mail/attachment to specified folder in matter library. Returns the operation status (success/failure).
        /// </summary>
        /// <param name="requestObject">The Web request object.</param>
        /// <param name="client">The Service client object.</param>
        /// <param name="folderPath">The folder path object containing path for specified folder.</param>
        /// <param name="filename">The filename object contains name of file to be uploaded.</param>
        /// <param name="extension">File extension object contains extension of file to be uploaded.</param>
        /// <param name="memoryStream">Memory stream object</param>
        /// <param name="mailMetadata">MailMetadata object</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="message">Reference object for the message to be returned</param>
        /// <param name="clientContext">Client context object for connection between SP & client</param>
        /// <returns>Returns True if upload operation is successful or False if operation fails</returns>
        private bool UploadFolderUtility(Client client, string folderPath, ref string filename,
            string extension, MemoryStream memoryStream, MailMetaData mailMetadata, string documentLibraryName,
            ref string message, ClientContext clientContext, ServiceRequest serviceRequest)
        {
            bool isUploadSuccessful = true;
            string folderName = folderPath.Substring(folderPath.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);
            try
            {
                if (documentRepository.FolderExists(folderPath, clientContext, documentLibraryName))
                {
                    filename = CreateFileInsideFolder(folderPath, filename, extension, memoryStream, clientContext);
                    SaveFields(client, folderPath, filename,
                        mailMetadata, documentLibraryName, serviceRequest.DocumentExtraProperties);
                }
                else
                {
                    message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}",
                                                string.Format(CultureInfo.InvariantCulture, documentSettings.FolderStructureModified, folderName),
                                                ServiceConstants.DOLLAR, folderName);
                    isUploadSuccessful = false;
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                isUploadSuccessful = false;
            }
            return isUploadSuccessful;
        }

        /// <summary>
        /// To create file inside SharePoint folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="filename">Filename</param>
        /// <param name="extension">Extension</param>
        /// <param name="memoryStream">Stream of the memory</param>
        /// <param name="clientContext">ClientContext object</param>
        /// <returns>filename</returns>
        private string CreateFileInsideFolder(string folderPath, string filename, string extension, MemoryStream memoryStream, ClientContext clientContext)
        {
            FileCreationInformation newFile = new FileCreationInformation();
            filename = uploadHelperFunctionsUtility.RemoveSpecialChar(filename);
            using (var stream = memoryStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                newFile.ContentStream = stream;

                if (!string.Equals(extension, ServiceConstants.EMAIL_FILE_EXTENSION, StringComparison.Ordinal))
                {
                    filename += extension;
                }
                newFile.Url = filename;
                newFile.Overwrite = true;

                documentRepository.CreateFileInsideFolder(clientContext, folderPath, newFile);
            }
            return filename;
        }

        /// <summary>
        /// Saves the fields for uploaded item in SharePoint library.
        /// </summary>
        /// <param name="requestObject">Web request object</param>
        /// <param name="client">Provider Service client</param>
        /// <param name="folderPath"> folder path</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="mailMetadata">MailMetadata object</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        public void SaveFields(Client client, string folderPath, string fileName, MailMetaData mailMetadata, string documentLibraryName, MatterExtraProperties documentExtraProperties)
        {
            try
            {
                if (null != client && !string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(fileName))
                {
                    using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                    {
                        
                        Users currentUserDetail = userRepositoy.GetLoggedInUserDetails(clientContext);
                        Dictionary<string, string> mailProperties = new Dictionary<string, string>
                        {
                            { ServiceConstants.MAIL_SENDER_KEY, mailMetadata.mailSender },
                            { ServiceConstants.MAIL_RECEIVER_KEY, string.IsNullOrEmpty(mailMetadata.mailReceiver) ? string.Empty : mailMetadata.mailReceiver },
                            { ServiceConstants.MAIL_RECEIVED_DATEKEY, mailMetadata.receivedDate },
                            { ServiceConstants.MAIL_CC_ADDRESS_KEY, string.IsNullOrEmpty(mailMetadata.cc) ? string.Empty : mailMetadata.cc },
                            { ServiceConstants.MAIL_ATTACHMENT_KEY, string.IsNullOrEmpty(mailMetadata.attachment) ? string.Empty : mailMetadata.attachment },
                            { ServiceConstants.MAIL_SEARCH_EMAIL_FROM_MAILBOX_KEY, currentUserDetail.Name },
                            { ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT, string.IsNullOrEmpty(mailMetadata.mailSubject) ? string.Empty : mailMetadata.mailSubject },
                            { ServiceConstants.MAIL_IMPORTANCE_KEY, string.IsNullOrEmpty(mailMetadata.mailImportance) ? string.Empty : mailMetadata.mailImportance },
                            { ServiceConstants.MAIL_SENSITIVITY_KEY, string.IsNullOrEmpty(mailMetadata.sensitivity) ? string.Empty : mailMetadata.sensitivity },
                            { ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY, string.IsNullOrEmpty(mailMetadata.hasAttachments) ? string.Empty : mailMetadata.hasAttachments },
                            { ServiceConstants.MAIL_CONVERSATIONID_KEY, string.IsNullOrEmpty(mailMetadata.conversationId) ? string.Empty : mailMetadata.conversationId },
                            { ServiceConstants.MAIL_CONVERSATION_TOPIC_KEY, string.IsNullOrEmpty(mailMetadata.conversationTopic) ? string.Empty : mailMetadata.conversationTopic },
                            { ServiceConstants.MAIL_CATEGORIES_KEY, string.IsNullOrEmpty(mailMetadata.categories) ? string.Empty : mailMetadata.categories } ,
                            { ServiceConstants.MAIL_SENT_DATE_KEY, string.IsNullOrEmpty(mailMetadata.sentDate) ? string.Empty : mailMetadata.sentDate },
                            { ServiceConstants.MAIL_ORIGINAL_NAME, string.IsNullOrEmpty(mailMetadata.originalName) ? string.Empty : mailMetadata.originalName }

                        };
                        documentRepository.SetUploadItemProperties(clientContext, documentLibraryName, fileName, folderPath, mailProperties, documentExtraProperties);
                    }
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// this method is to upload the emails to the matter
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serviceRequest"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public GenericResponseVM Upload(Client client, ServiceRequest serviceRequest, ref string message)
        {
            GenericResponseVM genericResponse = null;
            try
            {
                if (null != client && null != serviceRequest &&
                    !string.IsNullOrWhiteSpace(serviceRequest.MailId) && !string.IsNullOrWhiteSpace(serviceRequest.Subject) &&
                    !string.IsNullOrWhiteSpace(serviceRequest.FolderPath[0]))
                {
                    string fileName = serviceRequest.Subject;
                    string documentLibraryName = serviceRequest.DocumentLibraryName;
                    byte[] mailAttachment = mailMessageRepository.GetEmailContent(serviceRequest.MailId);
                    MailMetaData mailMetaData = new MailMetaData();
                    using (MemoryStream memoryStream = new MemoryStream(mailAttachment))
                    {
                        mailMetaData.mailImportance = string.Empty;
                        mailMetaData.mailSubject = string.Empty;
                        mailMetaData.originalName = fileName;
                        if (!UploadToFolder(client, serviceRequest.FolderPath[0], fileName, string.Empty, memoryStream,
                            documentLibraryName, mailMetaData, ref message, serviceRequest))
                        {
                            //result = ServiceConstants.UPLOAD_FAILED;
                            genericResponse = new GenericResponseVM()
                            {
                                IsError = true,
                                Code = UploadEnums.UploadToFolder.ToString(),
                                Value = message
                            };
                            return genericResponse;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw;
            }
            return genericResponse;
        }




        /// <summary>
        /// this method is to upload the email attachements to the matter
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serviceRequest"></param>
        /// <param name="attachment"></param>
        /// <param name="folderPath"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public GenericResponseVM UploadAttachmentOfEmail(Client client,
                    ServiceRequest serviceRequest,
                    AttachmentDetails attachment, string folderPath,
                   ref string message)
        {
            GenericResponseVM genericResponse = null;
            try
            {
                if (null != client && null != serviceRequest &&
                    !string.IsNullOrWhiteSpace(attachment.id) && !string.IsNullOrWhiteSpace(attachment.name) &&
                    !string.IsNullOrWhiteSpace(folderPath))
                {
                    string fileName = attachment.name;
                    string documentLibraryName = serviceRequest.DocumentLibraryName;
                    byte[] mailAttachment = mailMessageRepository.GetAttachments(serviceRequest.MailId, serviceRequest.Attachments[0].id);
                    MailMetaData mailMetaData = new MailMetaData();
                    using (MemoryStream memoryStream = new MemoryStream(mailAttachment))
                    {
                        mailMetaData.mailImportance = string.Empty;
                        mailMetaData.mailSubject = string.Empty;
                        mailMetaData.originalName = fileName;
                        if (!UploadToFolder(client, folderPath, fileName, string.Empty, memoryStream,
                            documentLibraryName, mailMetaData, ref message, serviceRequest))
                        {
                            //result = ServiceConstants.UPLOAD_FAILED;
                            genericResponse = new GenericResponseVM()
                            {
                                IsError = true,
                                Code = UploadEnums.UploadToFolder.ToString(),
                                Value = message
                            };
                            return genericResponse;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw;
            }
            return genericResponse;
        }



    }
}
