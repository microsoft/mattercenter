// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="DocumentRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// This class deals with all the document related functions such as finding document, pin, unpin
// ***********************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private ISearch search;
        private ISPList spList;
        private SearchSettings searchSettings;
        private ListNames listNames;
        
        private CamlQueries camlQueries;
        private DocumentSettings documentSettings;
        private MailSettings mailSettings;
        private ISPOAuthorization spoAuthorization;
        /// <summary>
        /// Constructory which will inject all the related dependencies related to matter
        /// </summary>
        /// <param name="search"></param>
        public DocumentRepository(ISearch search, IOptions<SearchSettings> searchSettings, 
            IOptions<ListNames> listNames, ISPList spList, IOptions<CamlQueries> camlQueries,  IOptions<DocumentSettings> documentSettings, IOptions<MailSettings> mailSettings, ISPOAuthorization spoAuthorization)
        {
            this.search = search;            
            this.searchSettings = searchSettings.Value;
            this.listNames = listNames.Value;
            this.spList = spList;
            this.camlQueries = camlQueries.Value;
            
            this.documentSettings = documentSettings.Value;
            this.mailSettings = mailSettings.Value;
            this.spoAuthorization = spoAuthorization;
        }

        /// <summary>
        /// This method will try to fetch all the matters that are provisioned by the user
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM, ClientContext clientContext)
        {
            return await Task.FromResult(search.GetDocuments(searchRequestVM, clientContext));
        }

        /// <summary>
        /// This method will try to fetch all the matters that are provisioned by the user
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<SearchResponseVM> GetPinnedRecordsAsync(SearchRequestVM searchRequestVM, ClientContext clientContext)
        {
            return await Task.FromResult(search.GetPinnedData(searchRequestVM, listNames.UserPinnedDocumentListName,
                searchSettings.PinnedListColumnDocumentDetails, true, clientContext));
        }

        public GenericResponseVM UploadDocument(string folderName, IFormFile uploadedFile, string fileName, 
            Dictionary<string, string> mailProperties, string clientUrl, string folder, string documentLibraryName, MatterExtraProperties documentExtraProperites)
        {
            return spList.UploadDocument(folderName, uploadedFile, fileName, mailProperties, clientUrl, folder, documentLibraryName, documentExtraProperites);
        }

        /// <summary>
        /// Create a new pin for the information that has been passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pinRequestData"></param>
        /// <returns>bool</returns>
        public async Task<bool> PinRecordAsync<T>(T pinRequestData)
        {
            PinRequestDocumentVM pinRequestDocumentVM = (PinRequestDocumentVM)Convert.ChangeType(pinRequestData, typeof(PinRequestDocumentVM));
            var documentData = pinRequestDocumentVM.DocumentData;
            documentData.DocumentName = ServiceUtility.EncodeValues(documentData.DocumentName);
            documentData.DocumentVersion = ServiceUtility.EncodeValues(documentData.DocumentVersion);
            documentData.DocumentClient = ServiceUtility.EncodeValues(documentData.DocumentClient);
            documentData.DocumentClientId = ServiceUtility.EncodeValues(documentData.DocumentClientId);
            documentData.DocumentClientUrl = ServiceUtility.EncodeValues(documentData.DocumentClientUrl);
            documentData.DocumentMatter = ServiceUtility.EncodeValues(documentData.DocumentMatter);
            documentData.DocumentMatterName = ServiceUtility.EncodeValues(documentData.DocumentMatter);
            documentData.DocumentMatterId = ServiceUtility.EncodeValues(documentData.DocumentMatterId);
            documentData.DocumentOwner = ServiceUtility.EncodeValues(documentData.DocumentOwner);
            documentData.DocumentUrl = ServiceUtility.EncodeValues(documentData.DocumentUrl);
            documentData.DocumentOWAUrl = ServiceUtility.EncodeValues(documentData.DocumentOWAUrl);
            documentData.DocumentExtension = ServiceUtility.EncodeValues(documentData.DocumentExtension);
            documentData.DocumentCreatedDate = ServiceUtility.EncodeValues(documentData.DocumentCreatedDate);
            documentData.DocumentModifiedDate = ServiceUtility.EncodeValues(documentData.DocumentModifiedDate);
            documentData.DocumentCheckoutUser = ServiceUtility.EncodeValues(documentData.DocumentCheckoutUser);
            documentData.DocumentMatterUrl = ServiceUtility.EncodeValues(documentData.DocumentMatterUrl);
            documentData.DocumentParentUrl = ServiceUtility.EncodeValues(documentData.DocumentParentUrl);
            documentData.DocumentID = ServiceUtility.EncodeValues(documentData.DocumentID);
            documentData.DocumentPracticeGroup = ServiceUtility.EncodeValues(documentData.DocumentPracticeGroup);
            pinRequestDocumentVM.DocumentData = documentData;
            return await Task.FromResult(search.PinDocument(pinRequestDocumentVM));
        }


        /// <summary>
        /// UnPin the record for the information that has been passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pinRequestData"></param>
        /// <returns>bool</returns>
        public async Task<bool> UnPinRecordAsync<T>(T pinRequestData)
        {
            PinRequestDocumentVM pinRequestDocumentVM = (PinRequestDocumentVM)Convert.ChangeType(pinRequestData, typeof(PinRequestDocumentVM));
            return await Task.FromResult(search.UnPinDocument(pinRequestDocumentVM));
        }

        public async Task<dynamic> GetDocumentAndClientGUIDAsync(Client client)
        {
            return await Task.FromResult(spList.GetDocumentAndClientGUID(client));
        }

        public void  SetUploadItemProperties(ClientContext clientContext, string documentLibraryName, string fileName, 
            string folderPath, Dictionary<string, string> mailProperties , MatterExtraProperties documentExtraProperties)
        {
            spList.SetUploadItemProperties(clientContext, documentLibraryName, fileName, folderPath, mailProperties, documentExtraProperties);
        }

        

        public void CreateFileInsideFolder(ClientContext clientContext, string folderPath, FileCreationInformation newFile)
        {
            spList.CreateFileInsideFolder(clientContext, folderPath, newFile);
        }
        

        public Stream DownloadAttachments(string attachmentUrl)
        {
           return spList.DownloadAttachments(attachmentUrl);
        }



        public bool FolderExists(string folderPath, ClientContext clientContext, string documentLibraryName)
        {
            return spList.FolderExists(folderPath, clientContext, documentLibraryName);
        }

        public bool PerformContentCheck(ClientContext context, MemoryStream localMemoryStream, string serverFileURL)
        {
            return spList.PerformContentCheck(context, localMemoryStream, serverFileURL);
        }

        public DuplicateDocument DocumentExists(ClientContext clientContext, ContentCheckDetails contentCheck, string documentLibraryName, string folderPath, bool isMail)
        {
            DuplicateDocument duplicateDocument = new DuplicateDocument(false, false);
            if (null != clientContext && null != contentCheck && !string.IsNullOrEmpty(documentLibraryName) && !string.IsNullOrEmpty(folderPath))
            {
                string serverRelativePath = folderPath + ServiceConstants.FORWARD_SLASH + contentCheck.FileName;
                string camlQuery = string.Format(CultureInfo.InvariantCulture, camlQueries.GetAllFilesInFolderQuery, serverRelativePath);
                ListItemCollection listItemCollection = null;
                listItemCollection = spList.GetData(clientContext, documentLibraryName, camlQuery);
                duplicateDocument.DocumentExists = (null != listItemCollection && 0 < listItemCollection.Count) ? true : false;
                // Check file size, from, sent date as well.
                if (duplicateDocument.DocumentExists)
                {
                    // check for other conditions as well.
                    ListItem listItem = listItemCollection.FirstOrDefault();
                    DateTime sentDate, storedFileSentDate;
                    long fileSize = Convert.ToInt64(listItem.FieldValues[mailSettings.SearchEmailFileSize], CultureInfo.InvariantCulture);
                    if (isMail)
                    {
                        // check for subject, from and sent date
                        string subject = Convert.ToString(listItem.FieldValues[mailSettings.SearchEmailSubject], CultureInfo.InvariantCulture);
                        string from = Convert.ToString(listItem.FieldValues[mailSettings.SearchEmailFrom], CultureInfo.InvariantCulture);
                        bool isValidDateFormat;
                        isValidDateFormat = DateTime.TryParse(Convert.ToString(listItem.FieldValues[mailSettings.SearchEmailSentDate], CultureInfo.InvariantCulture), out storedFileSentDate);
                        isValidDateFormat &= DateTime.TryParse(contentCheck.SentDate, out sentDate);
                        if (isValidDateFormat)
                        {
                            TimeSpan diffrence = sentDate - storedFileSentDate;
                            uint tolleranceMin = Convert.ToUInt16(mailSettings.SentDateTolerance, CultureInfo.InvariantCulture);     // up to how much minutes difference between uploaded files is tolerable
                            duplicateDocument.HasPotentialDuplicate = ((fileSize == contentCheck.FileSize) && (subject.Trim() == contentCheck.Subject.Trim()) && (from.Trim() == contentCheck.FromField.Trim()) && (diffrence.Minutes < tolleranceMin));
                        }
                    }
                    else
                    {
                        duplicateDocument.HasPotentialDuplicate = (fileSize == contentCheck.FileSize);
                    }
                }
            }
            return duplicateDocument;
        }
    }
}
