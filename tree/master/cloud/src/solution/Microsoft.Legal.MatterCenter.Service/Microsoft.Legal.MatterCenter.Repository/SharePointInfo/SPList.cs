// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Repository
// Author           : v-rijadh
// Created          : 07-07-2016
//***************************************************************************
// History
// Modified         : 07-07-2016
// Modified By      : v-lapedd
// ***********************************************************************
// <copyright file="TaxonomyHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to get/update information from/in SP lists</summary>


using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

#region Matter Namespaces
using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Reflection;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
#endregion

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This file provide methods to get/update information from/in SP lists
    /// </summary>
    public class SPList:ISPList
    {
        #region Properties
        private SearchSettings searchSettings;
        private ISPOAuthorization spoAuthorization;
        private CamlQueries camlQueries;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private MailSettings mailSettings;
        private IHostingEnvironment hostingEnvironment;
        private ErrorSettings errorSettings;
        #endregion

        /// <summary>
        /// All the dependencies are injected into the constructor
        /// </summary>
        /// <param name="spoAuthorization"></param>
        /// <param name="generalSettings"></param>
        public SPList(ISPOAuthorization spoAuthorization,
            IOptionsMonitor<CamlQueries> camlQueries, IOptionsMonitor<ErrorSettings> errorSettings,
            IOptionsMonitor<SearchSettings> searchSettings,
            IOptionsMonitor<ContentTypesConfig> contentTypesConfig,
            ICustomLogger customLogger, IOptionsMonitor<LogTables> logTables, IOptionsMonitor<MailSettings> mailSettings, IHostingEnvironment hostingEnvironment)
        {
            this.searchSettings = searchSettings.CurrentValue;
            this.camlQueries = camlQueries.CurrentValue;
            this.spoAuthorization = spoAuthorization;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
            this.mailSettings = mailSettings.CurrentValue;
            this.hostingEnvironment = hostingEnvironment;
            this.errorSettings = errorSettings.CurrentValue;
        }

        public bool CreateList(ClientContext clientContext, ListInformation listInfo)
        {
            bool result = true;
            if (null != clientContext && null != listInfo && !string.IsNullOrWhiteSpace(listInfo.name))
            {
                Web web = clientContext.Web;
                ListTemplateCollection listTemplates = web.ListTemplates;
                ListCreationInformation creationInfo = new ListCreationInformation();
                creationInfo.Title = listInfo.name;
                creationInfo.Description = listInfo.description;
                // To determine changes in URL we specified below condition as this function is common
                if (!string.IsNullOrWhiteSpace(listInfo.Path))
                {
                    creationInfo.Url = listInfo.Path;
                }
                if (!string.IsNullOrWhiteSpace(listInfo.templateType))
                {
                    string templateType = listInfo.templateType;
                    clientContext.Load(listTemplates, item => item.Include(currentTemplate => currentTemplate.Name, currentTemplate => 
                            currentTemplate.ListTemplateTypeKind).Where(selectedTemplate => selectedTemplate.Name == templateType));
                    clientContext.ExecuteQuery();
                    if (null != listTemplates && 0 < listTemplates.Count)
                    {
                        creationInfo.TemplateType = listTemplates.FirstOrDefault().ListTemplateTypeKind;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
                }
                if (result)
                {
                    List list = web.Lists.Add(creationInfo);
                    list.ContentTypesEnabled = listInfo.isContentTypeEnable;
                    if (null != listInfo.folderNames && listInfo.folderNames.Count > 0)
                    {
                        list = AddFolders(clientContext, list, listInfo.folderNames);
                    }
                    if (null != listInfo.versioning)
                    {
                        list.EnableVersioning = listInfo.versioning.EnableVersioning;
                        list.EnableMinorVersions = listInfo.versioning.EnableMinorVersions;
                        list.ForceCheckout = listInfo.versioning.ForceCheckout;
                    }
                    list.Update();
                    clientContext.Load(list, l => l.DefaultViewUrl);
                    clientContext.ExecuteQuery();
                    result = true;
                }
            }
            return result;
        }

        

        public Stream DownloadAttachments(string attachmentUrl)
        {            
            ClientContext clientContext;            
            clientContext = spoAuthorization.GetClientContext(attachmentUrl.Split(Convert.ToChar(ServiceConstants.DOLLAR, CultureInfo.InvariantCulture))[0]);
            SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(attachmentUrl.Split(Convert.ToChar(ServiceConstants.DOLLAR, CultureInfo.InvariantCulture))[1]);
            ClientResult<System.IO.Stream> fileStream = file.OpenBinaryStream();
            ///// Load the Stream data for the file
            clientContext.Load(file);
            clientContext.ExecuteQuery();  
            return fileStream.Value;
        }

        public GenericResponseVM UploadDocument(string folderPath, IFormFile uploadedFile, string fileName, Dictionary<string, string> mailProperties, string clientUrl, string folderName, string documentLibraryName)
        {
            IList<string> listResponse = new List<string>();
            GenericResponseVM genericResponse = null;
            bool isUploadSuccessful = false;
            try
            {
                ClientContext clientContext = spoAuthorization.GetClientContext(clientUrl);
                Web web = clientContext.Web;
                var uploadFile = new FileCreationInformation();
                using (var stream = uploadedFile.OpenReadStream())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    uploadFile.ContentStream = stream;
                    uploadFile.Url = fileName;
                    uploadFile.Overwrite = true;
                    using (clientContext)
                    {
                        genericResponse = DocumentUpload(folderPath, listResponse, clientContext, documentLibraryName, web, folderName, uploadFile);
                    }
                }
                if (genericResponse==null)
                {
                    SetUploadItemProperties(clientContext, documentLibraryName, fileName, folderPath, mailProperties);
                }
                
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                throw;
            }
            return genericResponse;
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
        private GenericResponseVM DocumentUpload(string folderPath, IList<string> listResponse, ClientContext clientContext, string documentLibraryName, Web web, string folderName, FileCreationInformation uploadFile)
        {            
            GenericResponseVM genericResponse = null;
            using (clientContext)
            {
                if (FolderExists(folderPath, clientContext, documentLibraryName))
                {
                    Folder destionationFolder = clientContext.Web.GetFolderByServerRelativeUrl(folderPath);
                    clientContext.Load(destionationFolder);
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.File fileToUpload = destionationFolder.Files.Add(uploadFile);
                    destionationFolder.Update();
                    web.Update();
                    clientContext.Load(fileToUpload);
                    clientContext.ExecuteQuery();                    
                }
                else
                {                   
                    genericResponse = new GenericResponseVM()
                    {
                        Code = errorSettings.FolderStructureModified,
                        Value = folderName,
                        IsError = true
                    };
                }
            }
            return genericResponse;
        }

        /// <summary>
        /// Breaks the permissions of the list.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="libraryName">Name of list</param>        
        /// <param name="isCopyRoleAssignment">Flag to copy permission from parent</param>
        /// <returns>Success flag</returns>
        public bool BreakPermission(ClientContext clientContext, string libraryName, bool isCopyRoleAssignment)
        {
            bool flag = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName))
            {
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(libraryName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();

                    if (!list.HasUniqueRoleAssignments)
                    {
                        list.BreakRoleInheritance(isCopyRoleAssignment, true);
                        list.Update();
                        clientContext.Load(list);
                        clientContext.ExecuteQuery();
                        flag = true;
                    }
                }
                catch (Exception)
                {
                    throw; // This will transfer control to catch block of parent function.
                }
            }
            return flag;
        }

        public string AddOneNote(ClientContext clientContext, string clientAddressPath, string oneNoteLocation, string listName, string oneNoteTitle)
        {
            string returnValue = String.Empty;
            if (null != clientContext && !string.IsNullOrWhiteSpace(clientAddressPath) && !string.IsNullOrWhiteSpace(oneNoteLocation) && 
                !string.IsNullOrWhiteSpace(listName))
            {
                Uri clientUrl = new Uri(clientAddressPath);
                //ToDo: Need to validate the url path
                string oneNotePath = $"{hostingEnvironment.WebRootPath}//{ServiceConstants.ONE_NOTE_RELATIVE_FILE_PATH}";
                byte[] oneNoteFile = System.IO.File.ReadAllBytes(oneNotePath);
                Web web = clientContext.Web;
                Microsoft.SharePoint.Client.File file = web.GetFolderByServerRelativeUrl(oneNoteLocation).Files.Add(new FileCreationInformation()
                {
                    Url = string.Concat(listName, ServiceConstants.EXTENSION_ONENOTE_TABLE_OF_CONTENT),
                    Overwrite = true,
                    ContentStream = new MemoryStream(oneNoteFile)
                });
                web.Update();
                clientContext.Load(file);
                clientContext.ExecuteQuery();
                ListItem oneNote = file.ListItemAllFields;
                oneNote["Title"] = oneNoteTitle;
                oneNote.Update();
                returnValue = string.Concat(clientUrl.Scheme, ServiceConstants.COLON, ServiceConstants.FORWARD_SLASH, ServiceConstants.FORWARD_SLASH, 
                    clientUrl.Authority, file.ServerRelativeUrl, ServiceConstants.WEB_STRING);
            }
            return returnValue;
        }

        /// <summary>
        /// Validates and breaks the item level permission for the specified list item under the list/library. 
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="isCopyRoleAssignment">Flag to copy permission from parent</param>
        /// <returns>String stating success flag</returns>
        public bool BreakItemPermission(ClientContext clientContext, string listName, int listItemId, bool isCopyRoleAssignment)
        {
            bool result = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();

                if (!listItem.HasUniqueRoleAssignments)
                {
                    listItem.BreakRoleInheritance(isCopyRoleAssignment, true);
                    listItem.Update();
                    clientContext.ExecuteQuery();
                    result = true;
                }
            }
            return result;
        }

        public bool Delete(ClientContext clientContext, IList<string> listsNames)
        {
            bool result = false;
            if (null != clientContext && null != listsNames)
            {
                ListCollection allLists = clientContext.Web.Lists;
                clientContext.Load(allLists);
                clientContext.ExecuteQuery();
                foreach (string listName in listsNames)
                {
                    List list = allLists.Cast<List>().FirstOrDefault(item => item.Title.ToUpperInvariant().Equals(listName.ToUpperInvariant()));
                    if (null != list)
                    {
                        result = true;
                        list.DeleteObject();
                    }

                }
                clientContext.ExecuteQuery();
            }
            return result;
        }

        


        public bool AddItem(ClientContext clientContext, List list, IList<string> columns, IList<object> values)
        {
            bool result = false;
            if (null != clientContext && null != list && null != columns && null != values && columns.Count == values.Count)
            {
                // Add the Matter URL in list
                ListItemCreationInformation listItemCreateInfo = new ListItemCreationInformation();
                ListItem newListItem = list.AddItem(listItemCreateInfo);
                int position = 0;
                foreach (string column in columns)
                {
                    newListItem[column] = values[position++];
                }
                ///// Update the list
                newListItem.Update();
                clientContext.ExecuteQuery();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Adds all the folders from Content type in matter library.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="list">List of folders</param>
        /// <param name="folderNames">The folder names.</param>
        /// <returns>Microsoft SharePoint Client List</returns>
        private List AddFolders(ClientContext clientContext, List list, IList<string> folderNames)
        {
            if (null != clientContext && null != list && null != folderNames)
            {
                FolderCollection listFolders = list.RootFolder.Folders;
                Folder listRootFolder = list.RootFolder;
                clientContext.Load(listFolders);
                clientContext.ExecuteQuery();
                if (0 < folderNames.Count)
                {
                    foreach (string folderName in folderNames)
                    {
                        // Check for empty folder names
                        if (!string.IsNullOrEmpty(folderName))
                        {
                            listFolders.Add(folderName);
                            listRootFolder.Update();
                        }
                    }
                    list.Update();
                }
            }
            return list;
        }
        /// <summary>
        ///  Creates a new view for the list
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="matterList">List name</param>
        /// <param name="viewColumnList">Name of the columns in view</param>
        /// <param name="viewName">View name</param>
        /// <param name="strQuery">View query</param>
        /// <returns>String stating success flag</returns>
        public bool AddView(ClientContext clientContext, List matterList, string[] viewColumnList, string viewName, string strQuery)
        {
            bool result = true;
            if (null != clientContext && null != matterList && null != viewColumnList && !string.IsNullOrWhiteSpace(viewName) && !string.IsNullOrWhiteSpace(strQuery))
                try
                {
                    View outlookView = matterList.Views.Add(new ViewCreationInformation
                    {
                        Title = viewName,
                        ViewTypeKind = ViewType.Html,
                        ViewFields = viewColumnList,
                        Paged = true
                    });
                    outlookView.ViewQuery = strQuery;
                    outlookView.Update();
                    clientContext.ExecuteQuery();
                }
                catch (Exception)
                {
                    result = false;
                }
            return result;
        }

        /// <summary>
        /// Method will check the permission of the list that has been provided
        /// </summary>
        /// <param name="url"></param>
        /// <param name="listName"></param>
        /// <param name="permission"></param>
        /// <returns>True or false</returns>
        public bool CheckPermissionOnList(string url, string listName, PermissionKind permission)
        {           
            try
            {
                ClientContext clientContext = spoAuthorization.GetClientContext(url);
                return CheckPermissionOnList(clientContext, listName, permission);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }            
        }

        /// <summary>
        /// Determines whether user has a particular permission on the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name</param>
        /// <param name="permission">Permission to be checked</param>
        /// <returns>Success flag</returns>
        public bool CheckPermissionOnList(Client client, string listName, PermissionKind permission)
        {
            try
            {
                ClientContext clientContext = spoAuthorization.GetClientContext(client.Url);
                return CheckPermissionOnList(clientContext, listName, permission);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Determines whether user has a particular permission on the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name</param>
        /// <param name="permission">Permission to be checked</param>
        /// <returns>Success flag</returns>
        public bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission)
        {
            try
            {
                bool returnValue = false;
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                {
                    ListCollection listCollection = clientContext.Web.Lists;
                    clientContext.Load(listCollection, lists => lists.Include(list => list.Title, list =>
                                                    list.EffectiveBasePermissions).Where(list => list.Title == listName));
                    clientContext.ExecuteQuery();
                    if (0 < listCollection.Count)
                    {
                        returnValue = listCollection[0].EffectiveBasePermissions.Has(permission);
                    }
                }
                return returnValue;
            }

            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Gets the list items of specified list based on CAML query.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="camlQuery">CAML Query that need to be executed on list</param>
        /// <returns>Collection of list items</returns>
        public ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null)
        {
            try
            {
                ListItemCollection listItemCollection = null;
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                {                
                    CamlQuery query = new CamlQuery();
                    if (!string.IsNullOrWhiteSpace(camlQuery))
                    {
                        query.ViewXml = camlQuery;
                        listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(query);
                    }
                    else
                    {
                        listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(CamlQuery.CreateAllItemsQuery());
                    }
                    clientContext.Load(listItemCollection);
                    clientContext.ExecuteQuery();               
                }
                return listItemCollection;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public ListItemCollection GetData(string url, string listName, string camlQuery = null)
        {
            try
            {
                ListItemCollection listItemCollection = null;
                using (ClientContext clientContext = spoAuthorization.GetClientContext(url))
                {
                    if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                    {
                        CamlQuery query = new CamlQuery();
                        if (!string.IsNullOrWhiteSpace(camlQuery))
                        {
                            query.ViewXml = camlQuery;
                            listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(query);
                        }
                        else
                        {
                            listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(CamlQuery.CreateAllItemsQuery());
                        }
                        clientContext.Load(listItemCollection);
                        clientContext.ExecuteQuery();
                    }
                    return listItemCollection;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public  void CreateFileInsideFolder(ClientContext clientContext, string folderPath, FileCreationInformation newFile)
        {
            Folder destinationFolder = clientContext.Web.GetFolderByServerRelativeUrl(folderPath);
            clientContext.Load(destinationFolder);
            clientContext.ExecuteQuery();
            Microsoft.SharePoint.Client.File fileToUpload = destinationFolder.Files.Add(newFile);
            clientContext.Load(fileToUpload);
            clientContext.ExecuteQuery();
        }

        public bool FolderExists(string folderPath, ClientContext clientContext, string documentLibraryName)
        {
            bool folderFound = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(documentLibraryName) && null != clientContext)
                {
                    string folderName = folderPath.Substring(folderPath.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);
                    List docLibList = clientContext.Web.Lists.GetByTitle(documentLibraryName);
                    ListItemCollection folderList = docLibList.GetItems(CamlQuery.CreateAllFoldersQuery());
                    clientContext.Load(clientContext.Web, web => web.ServerRelativeUrl);
                    clientContext.Load(docLibList, list => list.Title);
                    clientContext.Load(folderList, item => item.Include(currentItem => currentItem.Folder.Name, currentItem => currentItem.Folder.ServerRelativeUrl).Where(currentItem => currentItem.Folder.ServerRelativeUrl == folderPath));
                    clientContext.ExecuteQuery();

                    if (null != docLibList)
                    {
                        string rootFolderURL = string.Concat(clientContext.Web.ServerRelativeUrl, ServiceConstants.FORWARD_SLASH + folderName);
                        if (string.Equals(rootFolderURL, folderPath, StringComparison.OrdinalIgnoreCase))
                        {
                            //// Upload is performed on root folder
                            folderFound = null != docLibList && docLibList.Title.ToUpperInvariant().Equals(documentLibraryName.ToUpperInvariant());
                        }
                        else
                        {
                            //// Upload is performed on different folder other than root folder
                            folderFound = 0 < folderList.Count;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return folderFound;
        }

        /// <summary>
        /// Check if Content of local file and server file matches.
        /// </summary>
        /// <param name="context">SP client context</param>
        /// <param name="localMemoryStream">Memory stream of local file</param>
        /// <param name="serverFileURL">Server relative URL of file with filename</param>
        /// <returns>True if content matched else false</returns>
        public bool PerformContentCheck(ClientContext context, MemoryStream localMemoryStream, String serverFileURL)
        {
            bool isMatched = true;
            if (null != context && null != localMemoryStream && !string.IsNullOrWhiteSpace(serverFileURL))
            {
                Microsoft.SharePoint.Client.File serverFile = context.Web.GetFileByServerRelativeUrl(serverFileURL);
                context.Load(serverFile);
                ClientResult<Stream> serverStream = serverFile.OpenBinaryStream();
                context.ExecuteQuery();
                if (null != serverFile)
                {
                    using (MemoryStream serverMemoryStream = new MemoryStream())
                    {
                        byte[] serverBuffer = new byte[serverFile.Length + 1];

                        int readCount = 0;
                        while ((readCount = serverStream.Value.Read(serverBuffer, 0, serverBuffer.Length)) > 0)
                        {
                            serverMemoryStream.Write(serverBuffer, 0, readCount);
                        }
                        serverMemoryStream.Seek(0, SeekOrigin.Begin);
                        localMemoryStream.Seek(0, SeekOrigin.Begin);
                        if (serverMemoryStream.Length == localMemoryStream.Length)
                        {
                            byte[] localBuffer = localMemoryStream.GetBuffer();
                            serverBuffer = serverMemoryStream.GetBuffer();
                            for (long index = 0; index < serverMemoryStream.Length; index++)
                            {
                                if (localBuffer[index] != serverBuffer[index])
                                {
                                    isMatched = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isMatched = false;
                        }
                    }
                }
                else
                {
                    isMatched = false;
                }
            }
            return isMatched;
        }

        /// <summary>
        /// Sets the upload item properties.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="web">The web.</param>
        /// <param name="documentLibraryName">Name of the document library.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="folderPath">Path of the folder.</param>
        /// <param name="mailProperties">The mail properties.</param>
        public void SetUploadItemProperties(ClientContext clientContext, string documentLibraryName, string fileName, string folderPath, Dictionary<string, string> mailProperties)
        {
            ListItemCollection items = null;
            ListItem listItem = null;
            if (null != clientContext && !string.IsNullOrEmpty(documentLibraryName) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(folderPath) && null != mailProperties)
            {
                Web web = clientContext.Web;
                ListCollection lists = web.Lists;
                CamlQuery query = new CamlQuery();
                List selectedList = lists.GetByTitle(documentLibraryName);
                string serverRelativePath = folderPath +  ServiceConstants.FORWARD_SLASH + fileName;
                query.ViewXml = string.Format(CultureInfo.InvariantCulture, camlQueries.GetAllFilesInFolderQuery, serverRelativePath);
                items = selectedList.GetItems(query);
                if (null != items)
                {
                    clientContext.Load(items, item => item.Include(currentItem => currentItem.DisplayName, currentItem => currentItem.File.Name).Where(currentItem => currentItem.File.Name == fileName));
                    clientContext.ExecuteQuery();
                    if (0 < items.Count)
                    {
                        listItem = items[0];
                        if (null != mailProperties)
                        {
                            listItem[mailSettings.SearchEmailFrom] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_SENDER_KEY]) ? mailProperties[ServiceConstants.MAIL_SENDER_KEY].Trim() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_RECEIVED_DATEKEY]))
                            {
                                listItem[mailSettings.SearchEmailReceivedDate] = Convert.ToDateTime(mailProperties[ServiceConstants.MAIL_RECEIVED_DATEKEY].Trim(), CultureInfo.InvariantCulture).ToUniversalTime();
                            }
                            else
                            {
                                listItem[mailSettings.SearchEmailReceivedDate] = null;
                            }
                            listItem[mailSettings.SearchEmailCC] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_CC_ADDRESS_KEY]) ? mailProperties[ServiceConstants.MAIL_CC_ADDRESS_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailAttachments] = (string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY]) || mailProperties[ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY].Equals(ServiceConstants.TRUE, StringComparison.OrdinalIgnoreCase)) ? mailProperties[ServiceConstants.MAIL_ATTACHMENT_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailFromMailbox] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_FROM_MAILBOX_KEY]) ? mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_FROM_MAILBOX_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailSubject] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT]) ? mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailTo] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_RECEIVER_KEY]) ? mailProperties[ServiceConstants.MAIL_RECEIVER_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailImportance] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_IMPORTANCE_KEY]) ? mailProperties[ServiceConstants.MAIL_IMPORTANCE_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailSensitivity] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_SENSITIVITY_KEY]) ? mailProperties[ServiceConstants.MAIL_SENSITIVITY_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailHasAttachments] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY]) ? mailProperties[ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailConversationId] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_CONVERSATIONID_KEY]) ? mailProperties[ServiceConstants.MAIL_CONVERSATIONID_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailConversationTopic] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_CONVERSATION_TOPIC_KEY]) ? mailProperties[ServiceConstants.MAIL_CONVERSATION_TOPIC_KEY].Trim() : string.Empty;
                            listItem[mailSettings.SearchEmailCategories] = GetCategories(mailProperties[ServiceConstants.MAIL_CATEGORIES_KEY].Trim());
                            if (!string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_SENT_DATE_KEY]))
                            {
                                listItem[mailSettings.SearchEmailSentDate] = Convert.ToDateTime(mailProperties[ServiceConstants.MAIL_SENT_DATE_KEY].Trim(), CultureInfo.InvariantCulture).ToUniversalTime();
                            }
                            else
                            {
                                listItem[mailSettings.SearchEmailSentDate] = null;
                            }
                            listItem[mailSettings.SearchEmailOriginalName] = !string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MAIL_ORIGINAL_NAME]) ? mailProperties[ServiceConstants.MAIL_ORIGINAL_NAME] : string.Empty;
                            listItem.Update();
                            clientContext.ExecuteQuery();
                            listItem.RefreshLoad();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process the categories and trims the "category" word
        /// </summary>
        /// <param name="categories">Categories property</param>
        /// <returns>Processed category field</returns>
        internal static string GetCategories(string categories)
        {
            string processedCategories = string.Empty;
            if (!string.IsNullOrWhiteSpace(categories))
            {
                processedCategories = Regex.Replace(categories, ServiceConstants.CATEGORIES, string.Empty, RegexOptions.IgnoreCase).Trim(); // Replace categories with empty strings
                processedCategories = processedCategories.Replace(ServiceConstants.SPACE, string.Empty); // Remove the space generated because of replace operation
                processedCategories = processedCategories.Replace(ServiceConstants.SEMICOLON, string.Concat(ServiceConstants.SEMICOLON, ServiceConstants.SPACE));
            }
            return processedCategories;
        }

        /// <summary>
        /// Gets the list items of specified list based on CAML query. 
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="camlQuery">CAML Query that need to be executed on list</param>
        /// <returns>Collection of list items</returns>
        public ListItemCollection GetData(Client client, string listName, string camlQuery = null)
        {
            try
            { 
                return GetData(client.Url, listName, camlQuery);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the document and List GUID
        /// </summary>
        /// <param name="client">Client object containing list data</param>        
        /// <param name="clientContext">Client Context</param>     
        /// <returns>Returns the document and List GUID</returns>
        public dynamic GetDocumentAndClientGUID(Client client)
        {
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    string listInternalName = string.Empty;
                    string documentGUID = string.Empty;
                    string result = string.Empty;
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists, list => list.Include(listItem => listItem.Id, listItem => listItem.RootFolder.ServerRelativeUrl));
                    SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(client.Id);
                    clientContext.Load(file, files => files.ListItemAllFields);
                    clientContext.ExecuteQuery();
                    if (0 < file.ListItemAllFields.FieldValues.Count)
                    {
                        documentGUID = Convert.ToString(file.ListItemAllFields.FieldValues[ServiceConstants.DOCUMENT_GUID_COLUMN_NAME], CultureInfo.InvariantCulture);
                    }

                    List retrievedList = (from list in lists
                                          where list.RootFolder.ServerRelativeUrl.ToUpperInvariant().Equals(client.Name.ToUpperInvariant())
                                          select list).FirstOrDefault();
                    if (null != retrievedList)
                    {
                        listInternalName = Convert.ToString(retrievedList.Id, CultureInfo.InvariantCulture);
                    }
                    var documentAsset = new
                    {
                        ListInternalName = listInternalName,
                        DocumentGuid = documentGUID
                    };
                    return documentAsset;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        
        /// <summary>
        /// Method which will give matter folder hierarchy
        /// </summary>
        /// <param name="matterData"></param>
        /// <returns></returns>
        public List<FolderData> GetFolderHierarchy(MatterData matterData)
        {
            try
            { 
                using (ClientContext clientContext = spoAuthorization.GetClientContext(matterData.MatterUrl))
                {
                    List list = clientContext.Web.Lists.GetByTitle(matterData.MatterName);
                    clientContext.Load(list.RootFolder);
                    ListItemCollection listItems = GetData(clientContext, matterData.MatterName, 
                        string.Format(CultureInfo.InvariantCulture, camlQueries.AllFoldersQuery, matterData.MatterName));
                    List<FolderData> allFolders = new List<FolderData>();
                    allFolders = GetFolderAssignment(list, listItems, allFolders);

                    return allFolders;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Retrieves folder hierarchy from document library.
        /// </summary>
        /// <param name="list">SharePoint library</param>
        /// <param name="listItems">List items</param>
        /// <param name="allFolders">List of all folders of type folder data</param>
        /// <returns>List of folders of type folder data</returns>
        internal List<FolderData> GetFolderAssignment(List list, ListItemCollection listItems, List<FolderData> allFolders)
        {
            try
            {
                FolderData folderData = new FolderData()
                {
                    Name = list.RootFolder.Name,
                    URL = list.RootFolder.ServerRelativeUrl,
                    ParentURL = null
                };
                allFolders.Add(folderData);
                foreach (var listItem in listItems)
                {
                    folderData = new FolderData()
                    {
                        Name = Convert.ToString(listItem[searchSettings.ColumnNameFileLeafRef], CultureInfo.InvariantCulture),
                        URL = Convert.ToString(listItem[searchSettings.ColumnNameFileRef], CultureInfo.InvariantCulture),
                        ParentURL = Convert.ToString(listItem[searchSettings.ColumnNameFileDirRef], CultureInfo.InvariantCulture)
                    };

                    allFolders.Add(folderData);
                }
                return allFolders;            
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        

        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public List<string> Exists(Client client, ReadOnlyCollection<string> listsNames)
        {
            try
            { 
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    List<string> existingLists = new List<string>();
                    if (null != clientContext && null != listsNames)
                    {
                        //ToDo: Chec
                        ListCollection lists = clientContext.Web.Lists;
                        clientContext.Load(lists);
                        clientContext.ExecuteQuery();
                        existingLists = (from listName in listsNames
                                         join item in lists
                                         on listName.ToUpper(CultureInfo.InvariantCulture) equals item.Title.ToUpper(CultureInfo.InvariantCulture)
                                         select listName).ToList();
                    }
                    return existingLists;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public List<string> MatterAssociatedLists(ClientContext clientContext, ReadOnlyCollection<string> listsNames)
        {            
            List<string> existingLists = new List<string>();
            try
            { 
                if (null != clientContext && null != listsNames)
                {
                    //ToDo: Chec
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    existingLists = (from listName in listsNames
                                        join item in lists
                                        on listName.ToUpper(CultureInfo.InvariantCulture) equals item.Title.ToUpper(CultureInfo.InvariantCulture)
                                        select listName).ToList();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return existingLists;
        }

        public PropertyValues GetListProperties(ClientContext clientContext, string libraryName)
        {
            
            PropertyValues stampedProperties = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName))
                {
                    stampedProperties = clientContext.Web.Lists.GetByTitle(libraryName).RootFolder.Properties;
                    clientContext.Load(stampedProperties);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw; //// This will transfer control to catch block of parent function.
            }

            return stampedProperties;
        }

        /// <summary>
        /// Fetches the values of property for specified matter.
        /// </summary>
        /// <param name="context">Client context</param>
        /// <param name="matterName">Name of matter</param>
        /// <param name="propertyList">List of properties</param>
        /// <returns>Property list stamped to the matter</returns>
        public string GetPropertyValueForList(ClientContext context, string matterName, string propertyList)
        {
            string value = string.Empty;
            
                if (!string.IsNullOrWhiteSpace(matterName) && null != propertyList)
                {
                    ListCollection allLists = context.Web.Lists;
                    context.Load(allLists);
                    context.ExecuteQuery();
                    List list = allLists.Cast<List>().FirstOrDefault(item => item.Title.ToUpperInvariant().Equals(matterName.ToUpperInvariant()));
                    if (null != list)
                    {
                        var props = list.RootFolder.Properties;
                        context.Load(props);
                        context.ExecuteQuery();
                        if (null != props)
                        {
                            if (props.FieldValues.ContainsKey(propertyList))
                            {
                                value = Convert.ToString(props.FieldValues[propertyList], CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
                return value;
            
        }

        public IEnumerable<RoleAssignment> FetchUserPermissionForLibrary(ClientContext clientContext, string libraryname)
        {
            IEnumerable<RoleAssignment> userPermissionCollection = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(libraryname))
                {
                    List list = clientContext.Web.Lists.GetByTitle(libraryname);
                    userPermissionCollection = clientContext.LoadQuery(list.RoleAssignments.Include(listRoleAssignment => 
                        listRoleAssignment.PrincipalId, listRoleAssignment => listRoleAssignment.Member, 
                        listRoleAssignment => listRoleAssignment.Member.Title, 
                        listRoleAssignment => listRoleAssignment.Member.PrincipalType, 
                        listRoleAssignment => listRoleAssignment.RoleDefinitionBindings.Include(userRoles => userRoles.BasePermissions, 
                                                                                                userRoles => userRoles.Name, 
                                                                                                userRoles => userRoles.Id)).Where(listUsers => (PrincipalType.User == listUsers.Member.PrincipalType) || (PrincipalType.SecurityGroup == listUsers.Member.PrincipalType)));
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw; //// This will transfer control to catch block of parent function.
            }
            return userPermissionCollection;
        }

        /// <summary>
        /// Sets permissions for the list.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="assignUserName">Users to give permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <param name="listName">List name</param>
        /// <returns>String stating success flag</returns>
        public bool SetPermission(ClientContext clientContext, IList<IList<string>> assignUserName, IList<string> permissions, string listName)
        {
            bool result = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ClientRuntimeContext clientRuntimeContext = clientContext;
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments && null != permissions && null != assignUserName && permissions.Count == assignUserName.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> userName = assignUserName[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != userName)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in userName)
                                {
                                    if (!string.IsNullOrWhiteSpace(user))
                                    {
                                        /////get the user object
                                        Principal userPrincipal = clientContext.Web.EnsureUser(user.Trim());
                                        /////create the role definition binding collection
                                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                                        /////add the role definition to the collection
                                        roleDefinitionBindingCollection.Add(roleDefinition);
                                        /////create a RoleAssigment with the user and role definition
                                        list.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                                    }
                                }
                                /////execute the query to add everything
                                clientRuntimeContext.ExecuteQuery();
                            }
                            position++;
                        }
                        ///// Success. Return a success code
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Set permission to the specified list item 
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="assignUserName">Users to give permission</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <returns>Status of the unique item level permission assignment operation</returns>
        public bool SetItemPermission(ClientContext clientContext, IList<IList<string>> assignUserName, string listName, int listItemId, IList<string> permissions)
        {
            bool result = false;
            try
            {
                if (null != clientContext)
                {
                    ClientRuntimeContext clientRuntimeContext = clientContext;
                    ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                    clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (listItem.HasUniqueRoleAssignments && null != permissions && null != assignUserName && permissions.Count == assignUserName.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> userName = assignUserName[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != userName)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in userName)
                                {

                                    if (!string.IsNullOrWhiteSpace(user))
                                    {
                                        /////get the user object
                                        Principal userPrincipal = clientContext.Web.EnsureUser(user.Trim());
                                        /////create the role definition binding collection
                                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                                        /////add the role definition to the collection
                                        roleDefinitionBindingCollection.Add(roleDefinition);
                                        /////create a RoleAssigment with the user and role definition
                                        listItem.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                                    }
                                }
                                /////execute the query to add everything
                                clientRuntimeContext.ExecuteQuery();
                            }
                            position++;
                        }
                        ///// Success. Return a success code
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Checks if item is modified after it is loaded on the client side
        /// </summary>        
        /// <param name="collection">List item collection</param>
        /// <param name="cachedItemModifiedDate">Date time when current user loaded the page to see/update configuration values.</param>
        /// <returns>Success flag</returns>
        public bool CheckItemModified(ListItemCollection collection, string cachedItemModifiedDate)
        {
            bool response = false;
            int errorModifiedDate = 0;  // If there is new list item being created then 'cachedItemModifiedDate' will be 0
            if (null != collection && !string.IsNullOrWhiteSpace(cachedItemModifiedDate))
            {
                // Verify if new item flag is true and no list item is present in the Matter Configuration list
                if (String.Equals(Convert.ToString(errorModifiedDate, CultureInfo.InvariantCulture), cachedItemModifiedDate) && collection.Count.Equals(0)) 
                {
                    response = true;
                }
                else if (0 < collection.Count)
                {
                    ListItem settingsListItem = collection.FirstOrDefault();
                    DateTime cachedDate;
                    if (DateTime.TryParse(cachedItemModifiedDate, out cachedDate))
                    {
                        DateTime itemModifiedDate = Convert.ToDateTime(settingsListItem[ServiceConstants.MODIFIED_DATE_COLUMN], CultureInfo.InvariantCulture);
                        if (0 == DateTime.Compare(cachedDate, itemModifiedDate))
                        {
                            response = true;
                        }
                    }

                }
            }
            return response;
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="props">Property Bag</param>
        /// <param name="matterName">Name of matter to which property is to be attached</param>
        /// <param name="propertyList">List of properties</param>
        public void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            try
            { 
                if (null != clientContext && !string.IsNullOrWhiteSpace(matterName) && null != props && null != propertyList)
                {
                    List list = clientContext.Web.Lists.GetByTitle(matterName);

                    foreach (var item in propertyList)
                    {
                        props[item.Key] = item.Value;
                        list.Update();
                    }

                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

    }
}
