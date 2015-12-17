// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-swmirj
// Created          : 04-06-2015
//
// ***********************************************************************
// <copyright file="BriefcaseUtilityHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides utility methods involved in briefcase operations.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.CommonHelper
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provide utility methods involved in briefcase operations. 
    /// </summary>
    internal static class BriefcaseUtilityHelperFunctions
    {

        /// <summary>
        /// Overwrite document in OneDrive, if already exists (Users MySite Document library).
        /// </summary>
        /// <param name="collectionOfAttachments">Collection of documents with data for each document</param>
        /// <param name="collectionOfOriginalAttachments">Collection of documents with path of source document</param>
        /// <param name="usersMySite">User's My site URL</param>
        /// <param name="status">Sets status for Overwrite check</param>
        /// <param name="clientContext">Client context object for connection between SP & client</param>
        /// <param name="web">Object of site</param>
        /// <param name="listItems">Object for ListItems Collection</param>
        /// <param name="defaultContentTypeId">Id of content type</param>
        ///<returns>Returns status of operation to calling function</returns>
        internal static string OverWriteDocument(Dictionary<string, Stream> collectionOfAttachments, Dictionary<string, string> collectionOfOriginalAttachments, string usersMySite, string status, ClientContext clientContext, Microsoft.SharePoint.Client.Web web, ListItemCollection listItems, string defaultContentTypeId)
        {
            string fileNameKey = string.Empty;
            int documentCount = 0;
            try
            {
                foreach (string key in collectionOfAttachments.Keys)
                {
                    fileNameKey = key.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0];
                    foreach (ListItem item in listItems)
                    {
                        clientContext.Load(item);
                        clientContext.ExecuteQuery();
                        Folder folder = clientContext.Web.GetFolderByServerRelativeUrl(Convert.ToString(item.FieldValues[ServiceConstantStrings.ColumnNameFileRef], CultureInfo.InvariantCulture));
                        FileCollection files = folder.Files;
                        clientContext.Load(folder);
                        clientContext.Load(files);
                        clientContext.ExecuteQuery();
                        foreach (Microsoft.SharePoint.Client.File file in files)
                        {
                            if (file.Name == fileNameKey)
                            {
                                file.DeleteObject();
                                clientContext.ExecuteQuery();
                                SendDocumentToOneDrive(web, usersMySite, fileNameKey, collectionOfAttachments[key], collectionOfOriginalAttachments[key], defaultContentTypeId);
                                break;
                            }
                        }
                    }
                    documentCount++;
                }
                clientContext.ExecuteQuery();
                status = string.Concat(usersMySite, ServiceConstantStrings.OneDriveDocumentLibraryTitle, ConstantStrings.Semicolon, documentCount, ConstantStrings.Semicolon, collectionOfAttachments.Count);
            }
            catch (Exception exception)
            {
                status = ConstantStrings.TRUE + ConstantStrings.DOLLAR + Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return status;
        }

        /// <summary>
        /// Sends document to user's OneDrive.
        /// </summary>
        /// <param name="web">Object of site</param>
        /// <param name="usersMySite">User's My site URL</param>
        /// <param name="key">Key parameter</param>
        /// <param name="collectionOfAttachments">Attachments Collection</param>
        /// <param name="collectionOfOriginalAttachments">Original attachments collection</param>
        /// <param name="contentTypeId">Content type id of default content type</param>
        internal static void SendDocumentToOneDrive(Microsoft.SharePoint.Client.Web web, string usersMySite, string key, Stream collectionOfAttachments, string collectionOfOriginalAttachments, string contentTypeId)
        {
            try
            {
                Microsoft.SharePoint.Client.File file = web.GetFolderByServerRelativeUrl(string.Concat(usersMySite, ServiceConstantStrings.OneDriveDocumentLibraryTitle, ConstantStrings.ForwardSlash, ServiceConstantStrings.LegalBriefcaseFolder)).Files.Add(new FileCreationInformation()
                {
                    Url = key,
                    Overwrite = true,
                    ContentStream = collectionOfAttachments
                });
                file.ListItemAllFields.ParseAndSetFieldValue(ServiceConstantStrings.OneDriveSiteColumn, collectionOfOriginalAttachments);
                file.ListItemAllFields[ConstantStrings.OneDriveContentTypeProperty] = contentTypeId;
                file.ListItemAllFields.Update();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Sends new documents to user's OneDrive.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="collectionOfAttachments">Dictionary object containing attachment URLs and their stream data</param>
        /// <param name="collectionOfOriginalAttachments">Dictionary object containing attachment URLs</param>
        /// <param name="allAttachmentUrl">A string array containing all the attachment URLs</param>
        /// <param name="web">Object of site</param>
        /// <param name="usersMySite">My Site URL of the user</param>
        /// <param name="defaultContentTypeId">Default content type Id</param>
        /// <returns>JSON string specifying success or failure for new documents and URL of the files that already exist in the user's OneDrive</returns>
        internal static string NewDocumentToOneDrive(ClientContext clientContext, Dictionary<string, Stream> collectionOfAttachments, Dictionary<string, string> collectionOfOriginalAttachments, string[] allAttachmentUrl, Microsoft.SharePoint.Client.Web web, string usersMySite, string defaultContentTypeId)
        {
            string status = ConstantStrings.FALSE,
legalBriefcaseFolderQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.BriefcaseFolderContentsQuery, ServiceConstantStrings.LegalBriefcaseFolder);
            try
            {
                ListItemCollection listItemsColl = Lists.GetData(clientContext, ServiceConstantStrings.OneDriveDocumentLibraryTitle, legalBriefcaseFolderQuery);
                clientContext.Load(
                                    listItemsColl,
                                    items => items.Include(
                                        item => item.DisplayName,
                                        item => item.FileSystemObjectType,
                                        item => item.Folder.Files.Include(
                                            files => files.Name)));
                clientContext.ExecuteQuery();
                status = SendNewDocumentToOneDrive(clientContext, listItemsColl, collectionOfAttachments, collectionOfOriginalAttachments, allAttachmentUrl, web, usersMySite, defaultContentTypeId);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return status;
        }

        /// <summary>
        /// Send new documents to user's OneDrive.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="listItemsColl">List item collection of Legal Briefcase folder</param>
        /// <param name="collectionOfAttachments">Dictionary object containing attachment URLs and their stream data</param>
        /// <param name="collectionOfOriginalAttachments">Dictionary object containing attachment URLs</param>
        /// <param name="allAttachmentUrl">A string array containing all the attachment URLs</param>
        /// <param name="web">Object of site</param>
        /// <param name="usersMySite">My Site URL of the user</param>
        /// <param name="defaultContentTypeId">Default content type Id</param>
        /// <returns>JSON string specifying success or failure for new documents and URL of the files that already exist in the user's OneDrive</returns>
        internal static string SendNewDocumentToOneDrive(ClientContext clientContext, ListItemCollection listItemsColl, Dictionary<string, Stream> collectionOfAttachments, Dictionary<string, string> collectionOfOriginalAttachments, string[] allAttachmentUrl, Microsoft.SharePoint.Client.Web web, string usersMySite, string defaultContentTypeId)
        {
            string status = ConstantStrings.FALSE;
            string result = string.Empty;
            try
            {
                status = SendIndividualDocument(clientContext, collectionOfAttachments, listItemsColl, allAttachmentUrl, web, usersMySite, collectionOfOriginalAttachments, defaultContentTypeId, status);
                result = status;
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Sends individual document to OneDrive
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="collectionOfAttachments">Dictionary object containing attachment URLs and their stream data</param>
        /// <param name="listItemsColl">List item collection of Legal Briefcase folder</param>
        /// <param name="allAttachmentUrl">A string array containing all the attachment URLs</param>
        /// <param name="web">Object of site</param>
        /// <param name="usersMySite">My Site URL of the user</param>
        /// <param name="collectionOfOriginalAttachments">Dictionary object containing attachment URLs</param>
        /// <param name="defaultContentTypeId">Default content type Id</param>
        /// <param name="status">Status of documents sent to OneDrive</param>
        /// <returns>Status of documents sent to OneDrive</returns>
        internal static string SendIndividualDocument(ClientContext clientContext, Dictionary<string, Stream> collectionOfAttachments, ListItemCollection listItemsColl, string[] allAttachmentUrl, Microsoft.SharePoint.Client.Web web, string usersMySite, Dictionary<string, string> collectionOfOriginalAttachments, string defaultContentTypeId, string status)
        {
            int documentCount = 0, count = 0;
            string fileNameKey = string.Empty;
            string overwriteDocumentURLs = string.Empty;
            foreach (string key in collectionOfAttachments.Keys)
            {
                fileNameKey = key.Split(new string[] { ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries)[0];
                var selectedItems = from li in listItemsColl.Cast<ListItem>()
                                    from files in li.Folder.Files
                                    where files.Name.ToUpperInvariant() == fileNameKey.ToUpperInvariant()
                                    select files;
                if (selectedItems.FirstOrDefault() != null)
                {
                    overwriteDocumentURLs += allAttachmentUrl[count] + ConstantStrings.Semicolon;
                }
                else
                {
                    SendDocumentToOneDrive(web, usersMySite, fileNameKey, collectionOfAttachments[key], collectionOfOriginalAttachments[key], defaultContentTypeId);
                    documentCount++;
                }
                count++;
                web.Update();
                clientContext.ExecuteQuery();
                MailAttachmentDetails.CheckoutFailedPosition++;
                status = string.Concat(usersMySite, ServiceConstantStrings.OneDriveDocumentLibraryTitle, ConstantStrings.Semicolon, documentCount, ConstantStrings.Semicolon, collectionOfAttachments.Count, ConstantStrings.Semicolon, overwriteDocumentURLs);
            }
            return status;
        }
    }
}