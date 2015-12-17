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
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
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
    internal static class UploadHelperFunctions
    {
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
        internal static string Upload(RequestObject requestObject, Client client, ServiceRequest serviceRequest, string soapRequest, string attachmentOrMailID, bool isMailUpload, string fileName, string folderPath, bool isFirstCall, ref string message, string originalFileName)
        {
            string result = ConstantStrings.UploadFailed;
            try
            {
                if (null != requestObject && null != client && null != serviceRequest && !string.IsNullOrWhiteSpace(soapRequest) && !string.IsNullOrWhiteSpace(attachmentOrMailID) && !string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(folderPath))
                {
                    string documentLibraryName = serviceRequest.DocumentLibraryName;

                    //// Make the request to the Exchange server and get the response.
                    HttpWebResponse webResponse = UploadHelperFunctionsUtility.GetWebResponse(serviceRequest.EwsUrl, serviceRequest.AttachmentToken, soapRequest, attachmentOrMailID);

                    if (!isFirstCall)
                    {
                        XmlDocument xmlDocument = RetrieveXMLDocument(webResponse);
                        string attachmentID = string.Empty;
                        //// Check original file name is empty
                        if (!string.IsNullOrWhiteSpace(originalFileName))
                        {
                            attachmentID = UploadHelperFunctionsUtility.GetAttachmentID(xmlDocument, originalFileName);
                        }
                        else
                        {
                            attachmentID = UploadHelperFunctionsUtility.GetAttachmentID(xmlDocument, fileName);
                        }
                        if (!string.IsNullOrWhiteSpace(attachmentID))
                        {
                            attachmentOrMailID = attachmentID;
                        }

                        //// Make the request to the Exchange server and get the response.
                        webResponse = UploadHelperFunctionsUtility.GetWebResponse(serviceRequest.EwsUrl, serviceRequest.AttachmentToken, ServiceConstantStrings.AttachmentSoapRequest, attachmentOrMailID);
                    }

                    //// If the response is okay, create an XML document from the response and process the request.
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        result = UploadFilesMail(serviceRequest.Overwrite, serviceRequest.PerformContentCheck, serviceRequest.AllowContentCheck, documentLibraryName, webResponse, isMailUpload, requestObject, client, fileName, folderPath, ref message);
                    }
                    if (string.IsNullOrWhiteSpace(message) && result.Equals(ConstantStrings.UploadFailed) && isFirstCall)
                    {
                        result = Upload(requestObject, client, serviceRequest, ServiceConstantStrings.MailSoapRequest, serviceRequest.MailId, isMailUpload, fileName, folderPath, false, ref message, originalFileName);
                    }
                }
                else
                {
                    result = ConstantStrings.UploadFailed;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = ConstantStrings.UploadFailed;
            }
            return result;
        }

        /// <summary>
        /// Reads the XMLDocument and determines whether the request is to upload entire mail/attachment/.eml file/.msg file and calls respective method.
        /// </summary>
        /// <param name="isOverwrite">Overwrite check</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="folderName">Name of the folder</param>
        /// <param name="webResponse">HTTP web response to get the response stream</param>
        /// <param name="isMailUpload">Mail Upload Flag</param>
        /// <param name="requestObject">request object for web</param>
        /// <param name="client">Service Client Object</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="folderPath">upload folder path</param>
        /// <param name="message">Reference object for the message to be returned</param>
        /// <returns>Returns whether File Uploaded successfully or failed</returns>
        internal static string UploadFilesMail(bool isOverwrite, bool isContentCheckRequired, bool allowContentCheck, string documentLibraryName, HttpWebResponse webResponse, bool isMailUpload, RequestObject requestObject, Client client, string fileName, string folderPath, ref string message)
        {
            bool isMsg = true;
            MailMetaData mailMetaData = new MailMetaData();
            var bytes = (dynamic)null;
            string mailMessage = string.Empty;
            string originalName = string.Empty;
            string xmlPath = string.Empty;
            string result = ConstantStrings.UploadSucceeded;
            ContentCheckDetails contentCheck = null;
            try
            {
                XmlDocument xmlDocument = RetrieveXMLDocument(webResponse);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("s", ConstantStrings.SoapEnvelopURI);
                nsmgr.AddNamespace("m", ConstantStrings.ExchangeServiceMessage);
                nsmgr.AddNamespace("t", ConstantStrings.ExchangeServiceTypes);
                string extension = System.IO.Path.GetExtension(fileName).Trim();
                string uploadFileName = UploadHelperFunctionsUtility.RemoveSpecialChar(fileName);
                if (xmlDocument.SelectSingleNode("/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:FileAttachment/t:Content", nsmgr) != null)
                {
                    isMsg = false;
                }
                if (string.IsNullOrEmpty(extension) && isMsg)
                {
                    uploadFileName = uploadFileName + ConstantStrings.EmailFileExtension;
                }
                using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                {
                    if (isMailUpload)
                    {
                        bytes = GetEmailProperties(xmlDocument, ref mailMetaData);
                    }
                    else
                    {
                        bytes = UploadHelperFunctionsUtility.GetStream(xmlDocument, nsmgr, isMailUpload, extension, isMsg);
                    }
                    if (null != bytes)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(bytes))
                        {
                            contentCheck = new ContentCheckDetails(uploadFileName, mailMetaData.mailSubject, memoryStream.Length, mailMetaData.mailSender, mailMetaData.sentDate);
                        }
                    }
                    if (!isOverwrite && !isContentCheckRequired && UploadHelperFunctionsUtility.CheckDuplicateDocument(clientContext, documentLibraryName, isMailUpload, folderPath, contentCheck, uploadFileName, allowContentCheck, ref message))
                    {
                        result = ConstantStrings.UploadFailed;
                    }
                    else if (isContentCheckRequired)
                    {
                        message = UploadHelperFunctionsUtility.PerformContentCheckUtility(isMailUpload, folderPath, isMsg, xmlDocument, nsmgr, extension, uploadFileName, clientContext);
                        result = ConstantStrings.UploadFailed;
                    }
                    else
                    {
                        if (isMailUpload)       //Upload entire Email
                        {
                            UploadMail(requestObject, client, folderPath, fileName, documentLibraryName, xmlDocument, ref message);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(extension) && isMsg)       //Upload .msg file
                            {
                                UploadAttachedMailExtBlank(requestObject, client, folderPath, fileName, documentLibraryName, xmlDocument, ref message);
                            }
                            else
                            {
                                if (string.Equals(extension, ConstantStrings.EmailFileExtension, StringComparison.OrdinalIgnoreCase))
                                {
                                    UploadEMLFile(documentLibraryName, requestObject, client, folderPath, fileName, ref message, xmlDocument, nsmgr, ref mailMetaData, ref bytes, extension);
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
                                        if (!UploadToFolder(requestObject, client, folderPath, fileName, string.Empty, memoryStream, documentLibraryName, mailMetaData, ref message))
                                        {
                                            result = ConstantStrings.UploadFailed;
                                        }
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(message))
                        {
                            result = ConstantStrings.UploadFailed;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = ConstantStrings.UploadFailed;
            }
            return result;
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
                    xmlNodeValue = HttpUtility.HtmlDecode(xmlNodeValue);
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
        private static void UploadEMLFile(string documentLibraryName, RequestObject requestObject, Client client, string folderPath, string fileName, ref string message, XmlDocument xmlDocument, XmlNamespaceManager nsmgr, ref MailMetaData mailMetaData, ref dynamic bytes, string extension)
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
                                        { ConstantStrings.MailSenderKey, mailMetaData.mailSender }, 
                                        { ConstantStrings.MailReceiverKey, mailMetaData.mailReceiver }, 
                                        { ConstantStrings.MailReceivedDateKey, mailMetaData.receivedDate },
                                        { ConstantStrings.MailCCAddressKey, mailMetaData.cc },
                                        { ConstantStrings.MailCategoriesKey, string.Empty },
                                        { ConstantStrings.MailSensitivityKey, string.Empty },
                                        { ConstantStrings.MailAttachmentKey, string.Empty },
                                        { ConstantStrings.MailSearchEmailSubject, string.Empty },
                                        { ConstantStrings.MailFileExtensionKey, extension },
                                        { ConstantStrings.MailImportanceKey, string.Empty},
                                        { ConstantStrings.MailConversationIdKey, string.Empty},
                                        { ConstantStrings.MailConversationTopicKey, string.Empty},
                                        { ConstantStrings.MailSentDateKey, string.Empty},
                                        { ConstantStrings.MailHasAttachmentsKey, string.Empty},
                                        { ConstantStrings.MailOriginalName, string.Empty}
                                    };

                mailProperties = MailMessageParser.GetMailFileProperties(memoryStream, mailProperties);
                mailMetaData.mailImportance = mailProperties[ConstantStrings.MailImportanceKey];
                mailMetaData.mailReceiver = mailProperties[ConstantStrings.MailReceiverKey];
                mailMetaData.receivedDate = mailProperties[ConstantStrings.MailReceivedDateKey];
                mailMetaData.cc = mailProperties[ConstantStrings.MailCCAddressKey];
                mailMetaData.categories = mailProperties[ConstantStrings.MailCategoriesKey];
                mailMetaData.mailSubject = mailProperties[ConstantStrings.MailSearchEmailSubject];
                mailMetaData.attachment = mailProperties[ConstantStrings.MailAttachmentKey];
                mailMetaData.mailSender = mailProperties[ConstantStrings.MailSenderKey];
                mailMetaData.sentDate = mailProperties[ConstantStrings.MailSentDateKey];
                mailMetaData.originalName = mailProperties[ConstantStrings.MailSearchEmailSubject];
                UploadToFolder(requestObject, client, folderPath, fileName, mailProperties[ConstantStrings.MailFileExtensionKey], memoryStream, documentLibraryName, mailMetaData, ref message);
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
        internal static void UploadMail(RequestObject requestObject, Client client, string folderPath, string fileName, string documentLibraryName, XmlDocument xmlDocument, ref string message)
        {
            var bytes = (dynamic)null;

            try
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("s", ConstantStrings.SoapEnvelopURI);
                nsmgr.AddNamespace("m", ConstantStrings.ExchangeServiceMessage);
                nsmgr.AddNamespace("t", ConstantStrings.ExchangeServiceTypes);
                MailMetaData mailMetaData = new MailMetaData();

                bytes = GetEmailProperties(xmlDocument, ref mailMetaData);

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    using (MailMimeReader mime = new MailMimeReader())
                    {
                        MailMessageParser messageParser = mime.GetEmail(memoryStream);
                        mailMetaData.mailImportance = (string.IsNullOrWhiteSpace(messageParser.MailImportance)) ? ConstantStrings.MailDefaultImportance : messageParser.MailImportance;
                        mailMetaData.receivedDate = (string.IsNullOrWhiteSpace(messageParser.ReceivedDate.Date.ToShortDateString())) ? string.Empty : Convert.ToString(messageParser.ReceivedDate, CultureInfo.InvariantCulture);
                        UploadToFolder(requestObject, client, folderPath, fileName, string.Empty, memoryStream, documentLibraryName, mailMetaData, ref message);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
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
            nsmgr.AddNamespace("s", ConstantStrings.SoapEnvelopURI);
            nsmgr.AddNamespace("m", ConstantStrings.ExchangeServiceMessage);
            nsmgr.AddNamespace("t", ConstantStrings.ExchangeServiceTypes);
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
        internal static void UploadAttachedMailExtBlank(RequestObject requestObject, Client client, string folderPath, string fileName, string documentLibraryName, XmlDocument xmlDocument, ref string message)
        {
            var bytes = (dynamic)null;
            MailMetaData mailMetaData = new MailMetaData();

            try
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("s", ConstantStrings.SoapEnvelopURI);
                nsmgr.AddNamespace("m", ConstantStrings.ExchangeServiceMessage);
                nsmgr.AddNamespace("t", ConstantStrings.ExchangeServiceTypes);
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
                mailMetaData.mailImportance = (string.IsNullOrWhiteSpace(mailMetaData.mailImportance)) ? ConstantStrings.MailDefaultImportance : mailMetaData.mailImportance;
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    UploadToFolder(requestObject, client, folderPath, fileName + ConstantStrings.EmailFileExtension, string.Empty, memoryStream, documentLibraryName, mailMetaData, ref message);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
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
            nsmgr.AddNamespace("s", ConstantStrings.SoapEnvelopURI);
            nsmgr.AddNamespace("m", ConstantStrings.ExchangeServiceMessage);
            nsmgr.AddNamespace("t", ConstantStrings.ExchangeServiceTypes);

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
                mailMetaData.mailSender += ConstantStrings.Semicolon + checkMailFromAddressField.InnerXml;
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
                    mailMetaData.categories += node.InnerText + ConstantStrings.Semicolon;
                }
            }

            XmlNodeList xnlist = xmlDocument.GetElementsByTagName("t:Name");
            foreach (XmlNode node in xnlist)
            {
                if (null != node.PreviousSibling && "t:AttachmentId" == node.PreviousSibling.Name)
                {
                    mailMetaData.attachment += node.InnerText + ConstantStrings.Semicolon;
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
                        result += node["t:Name"].InnerXml + ConstantStrings.Semicolon + node["t:EmailAddress"].InnerXml + ConstantStrings.Semicolon;
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
        internal static bool UploadToFolder(RequestObject requestObject, Client client, string folderPath, string filename, string extension, MemoryStream memoryStream, string documentLibraryName, MailMetaData mailMetaData, ref string message)
        {
            bool isUploadSuccessful = false;
            try
            {
                if (null != memoryStream && null != requestObject && null != client && !string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(filename))
                {
                    if (0 == memoryStream.Length)
                    {
                        message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.ErrorEmptyFile, ConstantStrings.DOLLAR, documentLibraryName);
                    }
                    else
                    {
                        using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                        {
                            isUploadSuccessful = UploadFolderUtility(requestObject, client, folderPath, ref filename, extension, memoryStream, mailMetaData, documentLibraryName, ref message, clientContext);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", exception.Message, ConstantStrings.DOLLAR, documentLibraryName);
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
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
        private static bool UploadFolderUtility(RequestObject requestObject, Client client, string folderPath, ref string filename, string extension, MemoryStream memoryStream, MailMetaData mailMetadata, string documentLibraryName, ref string message, ClientContext clientContext)
        {
            bool isUploadSuccessful = true;
            string folderName = folderPath.Substring(folderPath.LastIndexOf(ConstantStrings.ForwardSlash, StringComparison.OrdinalIgnoreCase) + 1);
            try
            {
                if (UIUtility.FolderExists(folderPath, clientContext, documentLibraryName))
                {
                    filename = CreateFileInsideFolder(folderPath, filename, extension, memoryStream, clientContext);
                    SaveFields(requestObject, client, folderPath, filename, mailMetadata, documentLibraryName);
                }
                else
                {
                    message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}",
                                                string.Format(CultureInfo.InvariantCulture, ConstantStrings.FolderStructureModified, folderName),
                                                ConstantStrings.DOLLAR, folderName);
                    isUploadSuccessful = false;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
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
        private static string CreateFileInsideFolder(string folderPath, string filename, string extension, MemoryStream memoryStream, ClientContext clientContext)
        {
            FileCreationInformation newFile = new FileCreationInformation();
            filename = UploadHelperFunctionsUtility.RemoveSpecialChar(filename);
            using (var stream = memoryStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                newFile.ContentStream = stream;

                if (!string.Equals(extension, ConstantStrings.EmailFileExtension, StringComparison.Ordinal))
                {
                    filename += extension;
                }
                newFile.Url = filename;
                newFile.Overwrite = true;

                Folder destinationFolder = clientContext.Web.GetFolderByServerRelativeUrl(folderPath);
                clientContext.Load(destinationFolder);
                clientContext.ExecuteQuery();
                Microsoft.SharePoint.Client.File fileToUpload = destinationFolder.Files.Add(newFile);
                clientContext.Load(fileToUpload);
                clientContext.ExecuteQuery();
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
        public static void SaveFields(RequestObject requestObject, Client client, string folderPath, string fileName, MailMetaData mailMetadata, string documentLibraryName)
        {
            try
            {
                if (null != requestObject && null != client && !string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(fileName))
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(client.Url), requestObject.RefreshToken))
                    {
                        Microsoft.SharePoint.Client.Web web = clientContext.Web;
                        Users currentUserDetail = UIUtility.GetLoggedInUserDetails(clientContext);
                        Dictionary<string, string> mailProperties = new Dictionary<string, string>
                        {
                            { ConstantStrings.MailSenderKey, mailMetadata.mailSender },
                            { ConstantStrings.MailReceiverKey, string.IsNullOrEmpty(mailMetadata.mailReceiver) ? string.Empty : mailMetadata.mailReceiver },
                            { ConstantStrings.MailReceivedDateKey, mailMetadata.receivedDate },
                            { ConstantStrings.MailCCAddressKey, string.IsNullOrEmpty(mailMetadata.cc) ? string.Empty : mailMetadata.cc },
                            { ConstantStrings.MailAttachmentKey, string.IsNullOrEmpty(mailMetadata.attachment) ? string.Empty : mailMetadata.attachment },
                            { ConstantStrings.MailSearchEmailFromMailboxKey, currentUserDetail.Name },
                            { ConstantStrings.MailSearchEmailSubject, string.IsNullOrEmpty(mailMetadata.mailSubject) ? string.Empty : mailMetadata.mailSubject },
                            { ConstantStrings.MailImportanceKey, string.IsNullOrEmpty(mailMetadata.mailImportance) ? string.Empty : mailMetadata.mailImportance },
                            { ConstantStrings.MailSensitivityKey, string.IsNullOrEmpty(mailMetadata.sensitivity) ? string.Empty : mailMetadata.sensitivity },
                            { ConstantStrings.MailHasAttachmentsKey, string.IsNullOrEmpty(mailMetadata.hasAttachments) ? string.Empty : mailMetadata.hasAttachments },
                            { ConstantStrings.MailConversationIdKey, string.IsNullOrEmpty(mailMetadata.conversationId) ? string.Empty : mailMetadata.conversationId },
                            { ConstantStrings.MailConversationTopicKey, string.IsNullOrEmpty(mailMetadata.conversationTopic) ? string.Empty : mailMetadata.conversationTopic },
                            { ConstantStrings.MailCategoriesKey, string.IsNullOrEmpty(mailMetadata.categories) ? string.Empty : mailMetadata.categories } ,                           
                            { ConstantStrings.MailSentDateKey, string.IsNullOrEmpty(mailMetadata.sentDate) ? string.Empty : mailMetadata.sentDate },
                            { ConstantStrings.MailOriginalName, string.IsNullOrEmpty(mailMetadata.originalName) ? string.Empty : mailMetadata.originalName }

                        };
                        ServiceUtility.SetUploadItemProperties(clientContext, web, documentLibraryName, fileName, folderPath, mailProperties);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }
    }
}
