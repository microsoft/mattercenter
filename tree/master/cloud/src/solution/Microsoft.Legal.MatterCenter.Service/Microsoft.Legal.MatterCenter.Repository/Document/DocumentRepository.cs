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

using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private ISearch search;
        private ISPList spList;
        private SearchSettings searchSettings;
        private ListNames listNames;
        /// <summary>
        /// Constructory which will inject all the related dependencies related to matter
        /// </summary>
        /// <param name="search"></param>
        public DocumentRepository(ISearch search, IOptions<SearchSettings> searchSettings, 
            IOptions<ListNames> listNames, ISPList spList)
        {
            this.search = search;            
            this.searchSettings = searchSettings.Value;
            this.listNames = listNames.Value;
            this.spList = spList;
        }

        /// <summary>
        /// This method will try to fetch all the matters that are provisioned by the user
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM)
        {
            return await Task.FromResult(search.GetDocuments(searchRequestVM));
        }

        /// <summary>
        /// This method will try to fetch all the matters that are provisioned by the user
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<PinResponseVM> GetPinnedRecordsAsync(Client client)
        {
            return await Task.FromResult(search.GetPinnedData(client, listNames.UserPinnedDocumentListName,
                searchSettings.PinnedListColumnDocumentDetails, true));
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
    }
}
