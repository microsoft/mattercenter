// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="IDocumentRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// This interface contains all the document related functionalities
// ***********************************************************************


using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#region Matter Related Namespaces
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.AspNetCore.Http;
#endregion


namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// Interface matter repository contains all document related methods such as finding document, pin, unpin, document etc
    /// </summary>
    public interface IDocumentRepository:ICommonRepository
    {
        Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM, ClientContext clientContext);
        Task<dynamic> GetDocumentAndClientGUIDAsync(Client client);
        void SetUploadItemProperties(ClientContext clientContext, string documentLibraryName, string fileName, string folderPath, Dictionary<string, string> mailProperties);
       
        void CreateFileInsideFolder(ClientContext clientContext, string folderPath, FileCreationInformation newFile);
        bool FolderExists(string folderPath, ClientContext clientContext, string documentLibraryName);
        bool PerformContentCheck(ClientContext context, MemoryStream localMemoryStream, string serverFileURL);
        DuplicateDocument DocumentExists(ClientContext clientContext, ContentCheckDetails contentCheck, string documentLibraryName, string folderPath, bool isMail);
        GenericResponseVM UploadDocument(string folderName, IFormFile uploadedFile, string fileName, Dictionary<string, string> mailProperties, string clientUrl, string folder, string documentLibraryName);
        
        Stream DownloadAttachments(string attachmentUrl);
    }
}
