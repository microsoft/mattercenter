// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-diajme
// Created          : 04-06-2015
//
// ***********************************************************************
// <copyright file="BriefcaseContentTypeHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods involved in briefcase operations related to content type.</summary>
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
    using System.Linq;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provide methods involved in briefcase operations related to content type.
    /// </summary>
    internal static class BriefcaseContentTypeHelperFunctions
    {
        /// <summary>
        /// Returns content type associated with the list.
        /// </summary>
        /// <param name="list">List object</param>
        /// <param name="clientContext">Client Context object</param>
        /// <returns>Content type object</returns>
        internal static ContentType GetContentType(List list, ClientContext clientContext)
        {
            ContentType targetDocumentSetContentType = null;
            ContentTypeCollection listContentTypes = null;
            try
            {
                listContentTypes = list.ContentTypes;
                clientContext.Load(
                                                   listContentTypes,
                                                   types => types.Include(
                                                  type => type.Id,
                                                  type => type.Name,
                                                  type => type.Parent));

                var result = clientContext.LoadQuery(listContentTypes.Where(c => c.Name == ConstantStrings.OneDriveDocumentContentType));
                clientContext.ExecuteQuery();
                targetDocumentSetContentType = result.FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return targetDocumentSetContentType;
        }

        /// <summary>
        /// Updates the content type of the list item and returns the updated content type based on briefcase operation. 
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="documentName">Document Name</param>
        /// <param name="url">URL parameter</param>
        /// <param name="operationType">Specifies the type of operation</param>
        /// <param name="contentTypeId">String form of Content type ID</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <returns>List item Content Type Id</returns>
        internal static string ContentTypeByName(RequestObject requestObject, ClientContext clientContext, string documentName, string url, int operationType, string contentTypeId, string documentLibraryName)
        {
            string response = string.Empty;
            try
            {
                using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(url.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken))
                {
                    string itemQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.GetItemByNameQuery, documentName);
                    ListItemCollection listItems = Lists.GetData(clientContext, documentLibraryName, itemQuery);
                    ServiceConstantStrings.OperationTypes type = (ServiceConstantStrings.OperationTypes)operationType;
                    switch (type)
                    {
                        case ServiceConstantStrings.OperationTypes.Update:
                            foreach (var listitem in listItems)
                            {
                                response = Convert.ToString(listitem[ConstantStrings.OneDriveContentTypeProperty], CultureInfo.InvariantCulture);
                            }
                            break;
                        case ServiceConstantStrings.OperationTypes.Checkout:
                            foreach (var listitem in listItems)
                            {
                                listitem[ConstantStrings.OneDriveContentTypeProperty] = contentTypeId;
                                listitem.Update();
                                clientContext.ExecuteQuery();
                                response = ConstantStrings.TRUE;
                            }
                            break;
                        default:
                            response = string.Concat(ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, TextConstants.InvalidParametersMessage);
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                response = string.Concat(CultureInfo.InvariantCulture, ConstantStrings.FALSE, ConstantStrings.Comma, ConstantStrings.Space, ServiceUtility.RemoveEscapeCharacter(exception.Message));
            }
            return response;
        }

        /// <summary>
        /// Gets and sets default content type of the list.
        /// </summary>
        /// <param name="requestObject">RequestObject object</param>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="url">SourceURL of the OneDrive document</param>
        /// <param name="contentTypeId">Content Type Id</param>
        /// <param name="operationType">Operation type specifies either get or set Content type operation</param>
        /// <param name="documentLibraryName">Name of the document library</param>
        /// <returns>Returns Content Type Id</returns>
        internal static string GetContentTypeList(RequestObject requestObject, ClientContext clientContext, string url, string contentTypeId, int operationType, string documentLibraryName)
        {
            try
            {
                using (clientContext = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(url.Split(Convert.ToChar(ConstantStrings.DOLLAR, CultureInfo.InvariantCulture))[0]), requestObject.RefreshToken))
                {
                    List docLib = clientContext.Web.Lists.GetByTitle(documentLibraryName);
                    clientContext.Load(docLib);
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.Folder folder = docLib.RootFolder;
                    IList<ContentTypeId> list = new List<ContentTypeId>();
                    clientContext.Load(folder, item => item.ContentTypeOrder, item => item.UniqueContentTypeOrder);
                    clientContext.ExecuteQuery();
                    list = folder.ContentTypeOrder;
                    ServiceConstantStrings.OperationTypes type = (ServiceConstantStrings.OperationTypes)operationType;
                    switch (type)
                    {
                        case ServiceConstantStrings.OperationTypes.Update:
                            contentTypeId = list[0].StringValue;
                            break;
                        case ServiceConstantStrings.OperationTypes.Checkout:
                            int iterator = 0, index = 0;
                            for (iterator = 0; iterator < list.Count; iterator++)
                            {
                                if (list[iterator].StringValue == contentTypeId)
                                {
                                    index = iterator;
                                    break;
                                }
                            }
                            ContentTypeId currentContentTypeID = list[index];
                            list.RemoveAt(index);
                            list.Insert(0, currentContentTypeID);
                            folder.UniqueContentTypeOrder = list;
                            folder.Update();
                            clientContext.ExecuteQuery();
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return contentTypeId;
        }

        /// <summary>
        /// Check for the list item named Matter Center Briefcase already exists, if not then only create new folder
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="list">Name of the list</param>
        /// <param name="usersMySite">My Site URL of the user</param>
        internal static void CreateBriefcaseIfNotExists(ClientContext clientContext, List list, string usersMySite)
        {
            CamlQuery briefcaseQuery = new CamlQuery();
            briefcaseQuery.ViewXml = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.BriefcaseFolderQuery, ServiceConstantStrings.LegalBriefcaseFolder);
            ListItemCollection briefcases = list.GetItems(briefcaseQuery);
            clientContext.Load(briefcases, listItems => listItems.Include(item => item.DisplayName));
            clientContext.ExecuteQuery();
            ListItem listItem = briefcases.Where(item => item.DisplayName == ServiceConstantStrings.LegalBriefcaseFolder).FirstOrDefault();
            if (null == listItem)      // Check for Matter Center Briefcase folder exists, if not then create
            {
                ListItemCreationInformation newItem = new ListItemCreationInformation();
                newItem.FolderUrl = string.Concat(usersMySite, ServiceConstantStrings.OneDriveDocumentLibraryTitle);
                newItem.LeafName = ServiceConstantStrings.LegalBriefcaseFolder;
                newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                listItem = list.AddItem(newItem);
                listItem.Update();
                clientContext.Load(listItem, field => field.DisplayName);
                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Sets the content type.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="list">Name of the list</param>
        /// <param name="usersMySite">My Site URL of the user</param>
        internal static void SetContentType(ClientContext clientContext, List list, string usersMySite)
        {
            bool flag = false;
            try
            {
                CreateBriefcaseIfNotExists(clientContext, list, usersMySite);
                // Get Matter Center for OneDrive content type
                ContentType oneDriveContentType = clientContext.Web.ContentTypes.Where(item => item.Name == ServiceConstantStrings.OneDriveContentTypeName).FirstOrDefault();
                if (null != oneDriveContentType)
                {
                    list.ContentTypes.AddExistingContentType(oneDriveContentType); // Associate content type with list (Documents)
                    list.Update();
                    clientContext.ExecuteQuery();
                }
                ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
                clientContext.Load(currentContentTypeOrder);
                clientContext.ExecuteQuery();
                foreach (ContentType contentType in currentContentTypeOrder)
                {
                    if (contentType.Name.Equals(ServiceConstantStrings.OneDriveContentTypeName))
                    {
                        flag = true;
                    }
                }
                List<string> contentTypes = new List<string>() { ServiceConstantStrings.OneDriveContentTypeName };
                if (flag == false)
                {
                    IList<ContentType> contentTypeCollection = SharePointHelper.GetContentTypeData(clientContext, contentTypes);
                    if (null != contentTypeCollection)
                    {
                        ViewCollection views = list.Views;
                        clientContext.Load(views);
                        FieldCollection fields = ProvisionHelperFunctions.GetContentType(clientContext, contentTypeCollection, list);
                        fields.GetByInternalNameOrTitle(ServiceConstantStrings.OneDriveSiteColumn).SetShowInEditForm(false);
                        fields.GetByInternalNameOrTitle(ServiceConstantStrings.OneDriveSiteColumn).SetShowInNewForm(false);
                        fields.GetByInternalNameOrTitle(ServiceConstantStrings.OneDriveSiteColumn).Update();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Sets default content type for user's OneDrive.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="list">Name of the list</param>
        /// <param name="defaultContentType">Default content type to be set</param>
        /// <returns>Content type Id of default content type</returns>
        internal static string SetOneDriveDefaultContentType(ClientContext clientContext, List list, string defaultContentType)
        {
            string defaultContentTypeId = string.Empty;
            try
            {
                ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
                clientContext.Load(currentContentTypeOrder);
                clientContext.ExecuteQuery();
                IList<ContentTypeId> updatedContentTypeOrder = new List<ContentTypeId>();
                int count = 0, swap = 0;
                foreach (ContentType contentType in currentContentTypeOrder)
                {
                    if (contentType.Name.Equals(defaultContentType))
                    {
                        defaultContentTypeId = contentType.StringId;
                        swap = count;
                    }
                    if (!contentType.Name.Equals(ConstantStrings.OneDriveFolderContentType))
                    {
                        updatedContentTypeOrder.Add(contentType.Id);
                        count++;
                    }
                }
                ContentTypeId documentContentType = updatedContentTypeOrder[0];
                updatedContentTypeOrder[0] = updatedContentTypeOrder[swap];
                updatedContentTypeOrder[swap] = documentContentType;
                list.RootFolder.UniqueContentTypeOrder = updatedContentTypeOrder;
                list.RootFolder.Update();
                list.Update();
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                defaultContentTypeId = ConstantStrings.FALSE;
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return defaultContentTypeId;
        }

        /// <summary>
        /// Determines whether the content type exists in the content type group under the specified client context.  
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="web">Object of site</param>
        /// <param name="receivedContentType">Type of the received content</param>
        /// <param name="contentTypeGroup">The content type group</param>
        /// <param name="siteColumns">List of site columns</param>
        internal static bool IsContentTypePresentCheck(ClientContext clientContext, Microsoft.SharePoint.Client.Web web, string receivedContentType, string contentTypeGroup, List<string> siteColumns)
        {
            bool status = true;
            try
            {
                ContentTypeCollection contentTypeCollection = web.ContentTypes;
                clientContext.Load(contentTypeCollection, contentTypes => contentTypes.Include(contentTypeProperties => contentTypeProperties.Group, contentTypeProperties => contentTypeProperties.Name).Where(contentTypeValues => (contentTypeValues.Group == ServiceConstantStrings.OneDriveContentTypeGroup) && (contentTypeValues.Name == ServiceConstantStrings.OneDriveContentTypeName)));
                clientContext.ExecuteQuery();

                if (0 < contentTypeCollection.Count)
                {
                    FieldCollection fields = contentTypeCollection[0].Fields;
                    clientContext.Load(fields, field => field.Include(fieldType => fieldType.Title).Where(column => column.Title == ServiceConstantStrings.OneDriveSiteColumn));
                    clientContext.ExecuteQuery();
                    if (0 == fields.Count)
                    {
                        BriefcaseContentTypeHelperFunctions.AddColumnsToContentType(web, siteColumns, contentTypeCollection[0]);
                        web.Update();
                        clientContext.ExecuteQuery();
                    }
                }
                else
                {
                    status = BriefcaseContentTypeHelperFunctions.CreateContentType(clientContext, web, siteColumns, receivedContentType, contentTypeGroup);
                }
            }
            catch (Exception exception)
            {
                status = false;
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return status;
        }

        /// <summary>
        /// Creates site column.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="web">Object of site</param>
        /// <param name="siteColumns">List of site columns</param>
        /// <returns>Success or Failure</returns>
        internal static bool CreateSiteColumn(ClientContext clientContext, Microsoft.SharePoint.Client.Web web, List<string> siteColumns)
        {
            bool result = false;
            try
            {
                FieldCollection fieldCol = web.Fields;
                clientContext.Load(fieldCol);
                clientContext.ExecuteQuery();
                List<Field> existingFields = fieldCol.ToList<Field>();
                foreach (string columns in siteColumns)
                {
                    Field field = (from fld in existingFields
                                   where fld.Title == columns && fld.TypeAsString == ServiceConstantStrings.OneDriveSiteColumnType
                                   select fld).FirstOrDefault();
                    if (null == field)
                    {
                        web.Fields.AddFieldAsXml(string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.OneDriveSiteColumnSchema, ServiceConstantStrings.OneDriveSiteColumn, ServiceConstantStrings.OneDriveContentTypeGroup), true, AddFieldOptions.DefaultValue);
                        web.Update();
                        clientContext.ExecuteQuery();
                    }
                }
                result = true;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Creates the type of the content.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="web">Object of site</param>
        /// <param name="siteColumns">The site columns.</param>
        /// <param name="contentTypeName">Name of Content Type</param>
        internal static bool CreateContentType(ClientContext clientContext, Microsoft.SharePoint.Client.Web web, List<string> siteColumns, string contentTypeName, string contentTypegroup)
        {
            bool status = true;
            ContentType parentContentType = null;
            try
            {
                ContentTypeCollection contentTypeCollection = web.ContentTypes;
                clientContext.Load(contentTypeCollection);
                clientContext.Load(web.Fields);
                clientContext.ExecuteQuery();
                parentContentType = (from ct in contentTypeCollection
                                     where ct.Name == ServiceConstantStrings.OneDriveParentContentType
                                     select ct).FirstOrDefault();
                ContentTypeCreationInformation contentType = new ContentTypeCreationInformation();
                contentType.Name = contentTypeName;
                contentType.Group = contentTypegroup;
                ///// contentType.Id = "0x010100B1ED198475FB3A4AABC59AAAD89B7EAD";
                if (parentContentType != null)
                {
                    contentType.ParentContentType = parentContentType;
                }
                ContentType finalObj = web.ContentTypes.Add(contentType);
                AddColumnsToContentType(web, siteColumns, finalObj);
                web.Update();
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                status = false;
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return status;
        }

        /// <summary>
        /// Add columns to content type.
        /// </summary>
        /// <param name="web">Object of site</param>
        /// <param name="siteColumns">List of site columns</param>
        /// <param name="finalObj">Content type to which columns are to be added</param>        
        internal static void AddColumnsToContentType(Microsoft.SharePoint.Client.Web web, List<string> siteColumns, ContentType finalObj)
        {
            try
            {
                FieldCollection fieldCol = web.Fields;
                foreach (string columns in siteColumns)
                {
                    Field customDoc = (from fld in fieldCol
                                       where fld.Title == columns && fld.TypeAsString == ServiceConstantStrings.OneDriveSiteColumnType
                                       select fld).FirstOrDefault();
                    FieldLinkCreationInformation fieldLinkCreationInfo = new FieldLinkCreationInformation();
                    fieldLinkCreationInfo.Field = customDoc;
                    finalObj.FieldLinks.Add(fieldLinkCreationInfo);
                }
                finalObj.Update(true);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
        }
    }
}