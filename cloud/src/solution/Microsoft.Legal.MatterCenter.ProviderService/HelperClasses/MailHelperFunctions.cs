// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-prd
// Created          : 29-01-2015
//
// ***********************************************************************
// <copyright file="MailHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods involved in mail functionalities.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Exchange.WebServices.Data;
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Web;
    #endregion
    /// <summary>
    /// Provide methods involved in mail functionalities.
    /// </summary>
    internal static class MailHelperFunctions
    {
        /// <summary>
        /// Generates the email.
        /// </summary>
        /// <param name="collectionOfAttachments">The collection of attachments.</param>
        /// <returns>Stream Content</returns>
        internal static Stream GenerateEmail(Dictionary<string, Stream> collectionOfAttachments, string[] documentUrls, bool attachmentFlag)
        {
            Stream result = null;
            try
            {
                MemoryStream mailFile = GetMailAsStream(collectionOfAttachments, documentUrls, attachmentFlag);
                mailFile.Position = 0;
                WebOperationContext.Current.OutgoingResponse.Headers.Clear();
                WebOperationContext.Current.OutgoingResponse.ContentType = MailHelperFunctions.ReturnExtension(string.Empty);
                WebOperationContext.Current.OutgoingResponse.ContentLength = mailFile.Length;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Description: File Transfer");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=" + ServiceConstantStrings.TempEmailName + DateTime.Now + ConstantStrings.EmailFileExtension);
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Transfer-Encoding: binary");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Expires: 0");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control: must-revalidate, post-check=0, pre-check=0");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma: public");
                result = mailFile;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                Stream mailFile = null;
                result = mailFile;
            }
            return result;
        }

        /// <summary>
        /// Forms the memory stream of the mail with attachments.
        /// </summary>
        /// <param name="collectionOfAttachments">Collection of attachments as dictionary</param>
        /// <returns>Memory stream of the created mail object</returns>
        internal static MemoryStream GetMailAsStream(Dictionary<string, Stream> collectionOfAttachments, string[] documentUrls, bool attachmentFlag)
        {
            MemoryStream result = null;
            string documentUrl = string.Empty;
            try
            {
                // need to be able to update/configure or get current version of server
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                //// can use on premise exchange server credentials with service.UseDefaultCredentials = true, or explicitly specify the admin account (set default to false)
                service.Credentials = new WebCredentials(ConfigurationManager.AppSettings["Mail_Cart_Mail_User_Name"], ConfigurationManager.AppSettings["Mail_Cart_Mail_Password"]);
                service.Url = new Uri(ServiceConstantStrings.ExchangeServiceURL);
                Microsoft.Exchange.WebServices.Data.EmailMessage email = new Microsoft.Exchange.WebServices.Data.EmailMessage(service);
                email.Subject = TextConstants.MailCartMailSubject;

                if (attachmentFlag)
                {
                    email.Body = new MessageBody(TextConstants.MailCartMailBody);
                    foreach (KeyValuePair<string, Stream> mailAttachment in collectionOfAttachments)
                    {
                        if (null != mailAttachment.Value)
                        {
                            // Remove the date time string before adding the file as an attachment
                            email.Attachments.AddFileAttachment(mailAttachment.Key.Split('$')[0], mailAttachment.Value);
                        }
                    }
                }
                else
                {
                    int index = 0;
                    foreach (string currentURL in documentUrls)
                    {
                        if (null != currentURL && 0 < currentURL.Length)
                        {
                            string[] currentAssets = currentURL.Split('$');
                            string documentURL = ServiceConstantStrings.SiteURL + currentAssets[1];
                            string documentName = currentAssets[2];

                            documentUrl = string.Concat(documentUrl, string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.SendAsEmailFormat, ++index, documentName, documentURL));
                        }
                    }
                    documentUrl = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.SendAsEmailFontStyle, documentUrl);
                    email.Body = new MessageBody(documentUrl);
                }
                //// This header allows us to open the .eml in compose mode in outlook
                email.SetExtendedProperty(new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders, "X-Unsent", MapiPropertyType.String), "1");
                email.Save(WellKnownFolderName.Drafts); // must save draft in order to get MimeContent
                email.Load(new PropertySet(EmailMessageSchema.MimeContent));
                MimeContent mimcon = email.MimeContent;
                //// Do not make the StylCop fixes for MemoryStream here
                MemoryStream fileContents = new MemoryStream();
                fileContents.Write(mimcon.Content, 0, mimcon.Content.Length);
                fileContents.Position = 0;
                result = fileContents;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                MemoryStream fileContents = new MemoryStream();
                result = fileContents;
            }
            return result;
        }
        /// <summary>
        /// Gets the file content type based on specified extensions.
        /// </summary>
        /// <param name="fileExtension">Extension of the file</param>
        /// <returns>File content type</returns>
        internal static string ReturnExtension(string fileExtension)
        {
            string result = string.Empty;
            switch (fileExtension)
            {
                case ".txt":
                    result = "text/plain";
                    break;
                case ".doc":
                    result = "application/ms-word";
                    break;
                case ".xls":
                    result = "application/vnd.ms-excel";
                    break;
                case ".gif":
                    result = "image/gif";
                    break;
                case ".jpg":
                case "jpeg":
                    result = "image/jpeg";
                    break;
                case ".bmp":
                    result = "image/bmp";
                    break;
                case ".wav":
                    result = "audio/wav";
                    break;
                case ".ppt":
                    result = "application/mspowerpoint";
                    break;
                case ".dwg":
                    result = "image/vnd.dwg";
                    break;
                default:
                    result = "application/octet-stream";
                    break;
            }
            return result;
        }
    }
}