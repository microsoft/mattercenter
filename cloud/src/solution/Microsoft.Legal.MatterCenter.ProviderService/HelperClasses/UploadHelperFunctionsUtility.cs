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
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;
    #endregion

    /// <summary>
    /// Provide methods to perform document transfer functionalities.
    /// </summary>
    internal class UploadHelperFunctionsUtility
    {
        /// <summary>
        /// Check if duplicate document exists 
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <param name="folderName">Name of the folder</param>
        /// <param name="isMailUpload">Mail upload check</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="contentCheck">Content check object</param>
        /// <param name="uploadFileName">Name of the file.</param>
        /// <returns>message as per duplicate exists or not</returns>
        internal static bool CheckDuplicateDocument(ClientContext clientContext, string documentLibraryName, bool isMailUpload, string folderPath, ContentCheckDetails contentCheck, string uploadFileName, bool allowContentCheck, ref string message)
        {
            DuplicateDocument duplicateDocument = ServiceUtility.DocumentExists(clientContext, contentCheck, documentLibraryName, folderPath, isMailUpload);
            if (duplicateDocument.DocumentExists)
            {
                string documentPath = string.Concat(ServiceConstantStrings.SiteURL, folderPath, ConstantStrings.ForwardSlash, uploadFileName);
                string duplicateMessage = (allowContentCheck && duplicateDocument.HasPotentialDuplicate) ? ConstantStrings.FilePotentialDuplicateMessage : ConstantStrings.FileAlreadyExistMessage;
                message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", string.Format(CultureInfo.InvariantCulture, duplicateMessage, uploadFileName, documentPath), ConstantStrings.SymbolAt, duplicateDocument.HasPotentialDuplicate.ToString());
            }
            return duplicateDocument.DocumentExists;
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
        internal static string PerformContentCheckUtility(bool isMailUpload, string folderPath, bool isMsg, XmlDocument xmlDocument, XmlNamespaceManager nsmgr, string extension, string uploadFileName, ClientContext clientContext)
        {
            dynamic bytes = GetStream(xmlDocument, nsmgr, isMailUpload, extension, isMsg);
            string message = string.Empty;
            using (MemoryStream targetStream = new MemoryStream(bytes, 0, bytes.Length, false, true))
            {
                try
                {
                    string serverFileUrl = folderPath + ConstantStrings.ForwardSlash + uploadFileName;
                    if (ServiceUtility.PerformContentCheck(clientContext, targetStream, serverFileUrl))
                    {
                        message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.FoundIdenticalContent, ConstantStrings.Pipe, ConstantStrings.TRUE);
                    }
                    else
                    {
                        message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.FoundNonIdenticalContent, ConstantStrings.Pipe, ConstantStrings.FALSE);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                    message = string.Format(CultureInfo.InvariantCulture, "{0}{1}{1}{1}{2}", ConstantStrings.ContentCheckFailed, ConstantStrings.Pipe, ConstantStrings.TRUE);
                }
            }
            return message;
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
        internal static byte[] GetStream(XmlDocument xmlDocument, XmlNamespaceManager nsmgr, bool isMailUpload, string extension, bool isMsg)
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
        internal static HttpWebResponse GetWebResponse(Uri ewsUrl, string attachmentToken, string soapRequest, string attachmentOrMailID)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(ewsUrl);
            webRequest.Headers.Add(ServiceConstantStrings.RequestHeaderName, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.RequestHeaderValue, attachmentToken));
            webRequest.PreAuthenticate = true;
            webRequest.KeepAlive = false;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.AllowAutoRedirect = false;
            webRequest.Method = ServiceConstantStrings.RequestMethod;
            webRequest.ContentType = ServiceConstantStrings.RequestContentType;

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
        internal static string GetAttachmentID(XmlDocument xmlDocument, string fileName)
        {
            XNamespace exchangeServiceTypes = ConstantStrings.ExchangeServiceTypes;
            XName attachmentsTag = exchangeServiceTypes + ServiceConstantStrings.AttachmentsTag;
            XName fileAttachmentTag = exchangeServiceTypes + ServiceConstantStrings.FileAttachmentTag;
            XName itemAttachmentTag = exchangeServiceTypes + ServiceConstantStrings.ItemAttachmentTag;
            XName fileNameTag = exchangeServiceTypes + ServiceConstantStrings.FileNameTag;
            XName attachmentIdTag = exchangeServiceTypes + ServiceConstantStrings.AttachmentIdTag;
            XDocument xDocument = XDocument.Parse(xmlDocument.InnerXml);
            //// Get the attachmentID from the web response received for the itemID. Initial lookup is in the File Attachment
            string attachmentID = xDocument.Descendants(attachmentsTag)
                                            .Descendants(fileAttachmentTag)
                                            .Where((element => RemoveSpecialChar(element.Element(fileNameTag).Value) == fileName))
                                            .Select(element => element.Element(attachmentIdTag)
                                                                      .Attribute(ServiceConstantStrings.IdAttribute).Value)
                                            .SingleOrDefault();
            if (string.IsNullOrWhiteSpace(attachmentID))
            {
                //// If attachmentID is null, check if the file is an Item Attachment and get the attachmentID from the web response received for the itemID
                attachmentID = xDocument.Descendants(attachmentsTag)
                                        .Descendants(itemAttachmentTag)
                                        .Where((element => RemoveSpecialChar(element.Element(fileNameTag).Value) == fileName))
                                        .Select(element => element.Element(attachmentIdTag)
                                                                  .Attribute(ServiceConstantStrings.IdAttribute).Value)
                                        .SingleOrDefault();
            }
            return attachmentID;
        }

        /// <summary>
        /// Removes not allowed characters from SharePoint file name.
        /// </summary>
        /// <param name="filename">file name to be updated</param>
        /// <returns>Updated file name</returns>
        internal static string RemoveSpecialChar(string filename)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    Regex invalidCharsRegex = new Regex(ServiceConstantStrings.InvalidCharRegex, RegexOptions.Compiled);
                    Regex startEndRegex = new Regex(ServiceConstantStrings.StartEndregex, RegexOptions.Compiled);
                    Regex invalidRulesRegex = new Regex(ServiceConstantStrings.InvalidRuleRegex, RegexOptions.Compiled);
                    Regex extraSpacesRegex = new Regex(ServiceConstantStrings.ExtraSpaceRegex, RegexOptions.Compiled);
                    Regex invalidFileName = new Regex(ServiceConstantStrings.InvalidFileNameRegex, RegexOptions.Compiled & RegexOptions.IgnoreCase);
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
                    result = TextConstants.NoMailSubject;
                }
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }
    }
}