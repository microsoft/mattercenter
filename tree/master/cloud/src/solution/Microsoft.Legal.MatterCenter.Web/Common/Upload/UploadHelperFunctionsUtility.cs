// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-akdigh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="UploadHelperFunctionsUtility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to perform document transfer functionalities.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Web.Common
{

    #region using

    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using Models;
    using Repository;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Extensions.Options;
    #endregion

    /// <summary>
    /// Provide methods to perform document transfer functionalities.
    /// </summary>
    public class UploadHelperFunctionsUtility: IUploadHelperFunctionsUtility
    {
        MailSettings mailSettings;
        private ErrorSettings errorSettings;
        private IDocumentRepository documentRepository;
        private GeneralSettings generalSettings;

        /// <summary>
        /// Handle the upload functionality.
        /// </summary>
        /// <param name="mailSettings"></param>
        /// <param name="errorSettings"></param>
        /// <param name="documentRepository"></param>
        /// <param name="generalSettings"></param>
        public UploadHelperFunctionsUtility(IOptions<MailSettings> mailSettings, IOptions<ErrorSettings> errorSettings, IDocumentRepository documentRepository, IOptions<GeneralSettings> generalSettings)
        {
            this.mailSettings = mailSettings.Value;
            this.errorSettings = errorSettings.Value;
            this.documentRepository = documentRepository;
            this.generalSettings = generalSettings.Value;
        }
        /// <summary>
        /// Check if duplicate document exists 
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="isMailUpload">Mail upload check</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="contentCheck">Content check object</param>
        /// <param name="uploadFileName">Name of the file.</param>
        /// <returns>message as per duplicate exists or not</returns>
        public GenericResponseVM CheckDuplicateDocument(ClientContext clientContext, string documentLibraryName, bool isMailUpload, string folderPath, ContentCheckDetails contentCheck, string uploadFileName, bool allowContentCheck, ref string message)
        {
            GenericResponseVM genericResponse = null;
            DuplicateDocument duplicateDocument = documentRepository.DocumentExists(clientContext, contentCheck, documentLibraryName, folderPath, isMailUpload);
            if (duplicateDocument.DocumentExists)
            {
                string documentPath = string.Concat(generalSettings.SiteURL, folderPath, ServiceConstants.FORWARD_SLASH, uploadFileName);
                string duplicateMessage = (allowContentCheck && duplicateDocument.HasPotentialDuplicate) ? errorSettings.FilePotentialDuplicateMessage : errorSettings.FileAlreadyExistMessage;
                duplicateMessage = $"{duplicateMessage}|{duplicateDocument.HasPotentialDuplicate}";
                genericResponse = new GenericResponseVM()
                {
                    IsError = true,
                    Code = UploadEnums.DuplicateDocument.ToString(),
                    Value = string.Format(CultureInfo.InvariantCulture, duplicateMessage, uploadFileName, documentPath)
                };
                return genericResponse;
            }
            return genericResponse;
        }

        /// <summary>
        /// Performs content check
        /// </summary>
        /// <param name="isMailUpload">Mail upload check</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="isMsg">Is .msg file</param>
        /// <param name="xmlDocument">XML document object having information for the attachment</param>
        /// <param name="nsmgr">XML Namespace object</param>
        /// <param name="extension">File extension</param>
        /// <param name="uploadFileName">Name of the file.</param>
        /// <param name="clientContext">SP client context</param>
        /// <returns>result message as per document matches or not</returns>
        public GenericResponseVM PerformContentCheckUtility(bool isMailUpload, string folderPath, bool isMsg, XmlDocument xmlDocument, 
            XmlNamespaceManager nsmgr, string extension, string uploadFileName, ClientContext clientContext)
        {
            dynamic bytes = GetStream(xmlDocument, nsmgr, isMailUpload, extension, isMsg);
            string message = string.Empty;
            GenericResponseVM genericResponse = null;
            using (MemoryStream targetStream = new MemoryStream(bytes, 0, bytes.Length, false, true))
            {
                try
                {
                    string serverFileUrl = folderPath + ServiceConstants.FORWARD_SLASH + uploadFileName;
                    if (documentRepository.PerformContentCheck(clientContext, targetStream, serverFileUrl))
                    {
                        //message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", errorSettings.FoundIdenticalContent, 
                        //    ServiceConstants.PIPE, ServiceConstants.TRUE);
                        genericResponse = new GenericResponseVM()
                        {
                            IsError = true,
                            Code = UploadEnums.IdenticalContent.ToString(),
                            Value = errorSettings.FoundIdenticalContent
                        };
                        return genericResponse;
                    }
                    else
                    {
                        //message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", errorSettings.FoundNonIdenticalContent, 
                        //    ServiceConstants.PIPE, ServiceConstants.FALSE);

                        genericResponse = new GenericResponseVM()
                        {
                            IsError = true,
                            Code = UploadEnums.NonIdenticalContent.ToString(),
                            Value = errorSettings.FoundNonIdenticalContent
                        };
                        return genericResponse;
                    }
                }
                catch (Exception exception)
                {
                    //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    //message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", errorSettings.ContentCheckFailed, 
                    //    ServiceConstants.PIPE, ServiceConstants.TRUE);
                    genericResponse = new GenericResponseVM()
                    {
                        IsError = true,
                        Code = UploadEnums.ContentCheckFailed.ToString(),
                        Value = errorSettings.ContentCheckFailed
                    };
                    return genericResponse;
                }
            }
        }

        /// <summary>
        /// returns stream from exchange
        /// </summary>
        /// <param name="xmlDocument">XML document object having information for the attachment</param>
        /// <param name="nsmgr">XML Namespace object</param>
        /// <param name="isMailUpload">Mail upload check</param>
        /// <param name="extension">File extension</param>
        /// <param name="isMsg">Is .msg file</param>
        /// <returns>stream in byte array</returns>
        public byte[] GetStream(XmlDocument xmlDocument, XmlNamespaceManager nsmgr, bool isMailUpload, string extension, bool isMsg)
        {
            string xPath = string.Empty;
            string mailMessage = string.Empty;
            var bytes = (dynamic)null;
            if (isMailUpload)
            {
                xPath = "/s:Envelope/s:Body/m:GetItemResponse/m:ResponseMessages/m:GetItemResponseMessage/m:Items/t:Message/t:MimeContent";
            }
            else if (string.IsNullOrWhiteSpace(extension) && isMsg)
            {
                xPath = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:ItemAttachment/t:Message/t:MimeContent";
            }
            else
            {
                xPath = "/s:Envelope/s:Body/m:GetAttachmentResponse/m:ResponseMessages/m:GetAttachmentResponseMessage/m:Attachments/t:FileAttachment/t:Content";
            }
            mailMessage = xmlDocument.SelectSingleNode(xPath, nsmgr).InnerXml;
            bytes = Convert.FromBase64String(mailMessage);
            return bytes;
        }

        /// <summary>
        /// Make the request to the Exchange server and get the response.
        /// </summary>
        /// <param name="ewsUrl">Exchange Web Service URL.</param>
        /// <param name="attachmentToken">The attachment token.</param>
        /// <param name="soapRequest">The SOAP request.</param>
        /// <param name="attachmentOrMailID">The attachment or mail identifier.</param>
        /// <returns>HTTP-specific response from Exchange server</returns>
        public HttpWebResponse GetWebResponse(Uri ewsUrl, string attachmentToken, string soapRequest, string attachmentOrMailID)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(ewsUrl);
            webRequest.Headers.Add(ServiceConstants.REQUEST_HEADER_NAME, string.Format(CultureInfo.InvariantCulture, ServiceConstants.REQUEST_HEADER_VALUE, attachmentToken));
            webRequest.PreAuthenticate = true;
            webRequest.KeepAlive = false;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.AllowAutoRedirect = false;
            webRequest.Method = ServiceConstants.REQUEST_METHOD;
            webRequest.ContentType = ServiceConstants.REQUEST_CONTENT_TYPE;

            //// Construct the SOAP message for the GetMailSoap operation.              
            byte[] bodyBytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, soapRequest, attachmentOrMailID));
            webRequest.ContentLength = bodyBytes.Length;
            Stream requestStream = webRequest.GetRequestStream();
            requestStream.Write(bodyBytes, 0, bodyBytes.Length);
            requestStream.Close();

            //// Make the request to the Exchange server and get the response.
            return (HttpWebResponse)webRequest.GetResponse();
        }

        /// <summary>
        /// Get the Attachment ID from the web response
        /// </summary>
        /// <param name="xmlDocument">XML document object having information for mail contents</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The attachment identifier retrieved from the web response.</returns>
        public string GetAttachmentID(XmlDocument xmlDocument, string fileName)
        {
            XNamespace exchangeServiceTypes = ServiceConstants.EXCHANGE_SERVICE_TYPES;
            XName attachmentsTag = exchangeServiceTypes + ServiceConstants.ATTACHMENTS_TAG;
            XName fileAttachmentTag = exchangeServiceTypes + ServiceConstants.FILE_ATTACHMENT_TAG;
            XName itemAttachmentTag = exchangeServiceTypes + ServiceConstants.ITEM_ATTACHMENT_TAG;
            XName fileNameTag = exchangeServiceTypes + ServiceConstants.FILE_NAME_TAG;
            XName attachmentIdTag = exchangeServiceTypes + ServiceConstants.ATTACHMENT_ID_TAG;
            XDocument xDocument = XDocument.Parse(xmlDocument.InnerXml);
            //// Get the attachmentID from the web response received for the itemID. Initial lookup is in the File Attachment
            string attachmentID = xDocument.Descendants(attachmentsTag)
                                            .Descendants(fileAttachmentTag)
                                            .Where((element => RemoveSpecialChar(element.Element(fileNameTag).Value) == fileName))
                                            .Select(element => element.Element(attachmentIdTag)
                                                                      .Attribute(ServiceConstants.ID_ATTRIBUTE).Value)
                                            .SingleOrDefault();
            if (string.IsNullOrWhiteSpace(attachmentID))
            {
                //// If attachmentID is null, check if the file is an Item Attachment and get the attachmentID from the web response received for the itemID
                attachmentID = xDocument.Descendants(attachmentsTag)
                                        .Descendants(itemAttachmentTag)
                                        .Where((element => RemoveSpecialChar(element.Element(fileNameTag).Value) == fileName))
                                        .Select(element => element.Element(attachmentIdTag)
                                                                  .Attribute(ServiceConstants.ID_ATTRIBUTE).Value)
                                        .SingleOrDefault();
            }
            return attachmentID;
        }

        /// <summary>
        /// Removes not allowed characters from SharePoint file name.
        /// </summary>
        /// <param name="filename">file name to be updated</param>
        /// <returns>Updated file name</returns>
        public string RemoveSpecialChar(string filename)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    Regex invalidCharsRegex = new Regex(ServiceConstants.INVALID_CHARREGEX, RegexOptions.Compiled);
                    Regex startEndRegex = new Regex(ServiceConstants.START_END_REGEX, RegexOptions.Compiled);
                    Regex invalidRulesRegex = new Regex(ServiceConstants.INVALID_RULE_REGEX, RegexOptions.Compiled);
                    Regex extraSpacesRegex = new Regex(ServiceConstants.EXTRA_SPACE_REGEX, RegexOptions.Compiled);
                    Regex invalidFileName = new Regex(ServiceConstants.INVALID_FILENAME_REGEX, RegexOptions.Compiled & RegexOptions.IgnoreCase);
                    result = invalidFileName.Replace(
                                extraSpacesRegex.Replace(
                                    invalidRulesRegex.Replace(
                                        startEndRegex.Replace(
                                            invalidCharsRegex.Replace(filename, string.Empty).Trim()
                                        , string.Empty)
                                    , string.Empty)
                                , string.Empty)
                            , string.Empty);
                }
                else
                {
                    result = mailSettings.NoMailSubject;
                }
            }
            catch (Exception exception)
            {
                //result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }
    }
}