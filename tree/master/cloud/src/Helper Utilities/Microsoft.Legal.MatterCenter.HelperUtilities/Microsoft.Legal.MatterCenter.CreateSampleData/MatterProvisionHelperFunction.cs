// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 04-27-2015
//
// ***********************************************************************
// <copyright file="MatterProvisionHelperFunction.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    #endregion

    class MatterProvisionHelperFunction
    {
        internal static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + "\\" + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// Creates matter
        /// </summary>
        internal static string CreateMatter(ClientContext clientContext, string url, Matter matterData, string folders)
        {
            string result = string.Empty;
            string oneNoteLibraryName = matterData.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"];
            try
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                clientContext.Load(web.Lists);
                clientContext.ExecuteQuery();
                List matterLibrary = (from list in web.Lists where list.Title.ToString().ToUpper() == matterData.MatterName.ToUpper() select list).FirstOrDefault();
                List oneNoteLibrary = (from list in web.Lists where list.Title.ToString().ToUpper() == oneNoteLibraryName.ToUpper() select list).FirstOrDefault();
                Uri clientUri = new Uri(url);
                string requestedUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", clientUri.AbsolutePath, Constants.Backslash, ConfigurationManager.AppSettings["MatterLandingPageRepository"].Replace(Constants.SPACE, string.Empty), Constants.Backslash, matterData.MatterName, Constants.AspxExtension);
                bool isMatterLandingPageExists = PageExists(requestedUrl, clientContext);
                // Matter exists
                if (null != matterLibrary || null != oneNoteLibrary || isMatterLandingPageExists)
                {
                    if (null != matterLibrary)
                    {
                        result = Constants.MatterLibraryExists; // return if matter library exists
                    }
                    else if (null != oneNoteLibrary)
                    {
                        result = Constants.OneNoteLibraryExists; // return if OneNote library exists
                    }
                    else if (isMatterLandingPageExists)
                    {
                        result = Constants.MatterLandingPageExists; // return if matter landing page exists
                    }
                }
                else
                {
                    //Create Document library
                    CreateDocumentLibrary(matterData.MatterName, clientContext, folders, false, matterData);

                    //Create OneNote library 
                    CreateDocumentLibrary(oneNoteLibraryName, clientContext, folders, true, matterData);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["CalendarCreationEnabled"], CultureInfo.InvariantCulture))
                    {
                        MatterLandingHelperFunction.AddCalendarList(clientContext, matterData);
                    }
                    CreateOneNote(clientContext, new Uri(url), matterData);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["TaskListCreationEnabled"], CultureInfo.InvariantCulture))
                    {
                        CreateTaskList(clientContext, matterData);
                    }
                    result = Constants.MatterProvisionPrerequisitesSuccess;
                }
            }
            catch (Exception exception)
            {
                DeleteMatter(clientContext, matterData);
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "Matter name: " + matterData.MatterName + "\nStacktrace: " + exception.StackTrace);
            }
            return result;
        }


        /// <summary>
        /// Function to create document library for Matter and OneNote
        /// </summary>
        /// <param name="libraryName">Matter library name</param>
        /// <param name="clientContext">client context information</param>
        /// <param name="folders">folders to create in document library</param>
        /// <param name="isOneNoteLibrary">Flag to determine OneNote library or Matter Library</param>
        /// <param name="matter">Matter object containing Matter data</param>
        internal static void CreateDocumentLibrary(string libraryName, ClientContext clientContext, string folders, bool isOneNoteLibrary, Matter matter)
        {
            IList<string> folderNames = new List<string>();
            Microsoft.SharePoint.Client.Web web = clientContext.Web;
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = libraryName;
            creationInfo.Description = matter.MatterDescription;
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            // Added library GUID for URL consolidation changes
            creationInfo.Url = matter.MatterGuid + (isOneNoteLibrary ? ConfigurationManager.AppSettings["OneNoteLibrarySuffix"] : string.Empty);
            List list = web.Lists.Add(creationInfo);
            list.ContentTypesEnabled = true;
            // Version setting for OneNote document library
            if (isOneNoteLibrary)
            {
                list.EnableVersioning = false;
                string oneNoteFolderName = string.Empty;
                oneNoteFolderName = matter.MatterGuid;
                // create folder
                folderNames = new List<string>() { oneNoteFolderName };
            }
            else
            {
                list.EnableVersioning = true;
                list.EnableMinorVersions = false;
                list.ForceCheckout = false;
                //Addition of Email folder
                folderNames = folders.Split(';').Where(folder => !string.IsNullOrWhiteSpace(folder.Trim())).ToList();
            }
            list.Update();
            clientContext.ExecuteQuery();
            AddFolders(clientContext, list, folderNames);
            BreakPermission(clientContext, libraryName, matter.CopyPermissionsFromParent);
        }

        /// <summary>
        /// Breaks permission for user
        /// </summary>
        internal static bool BreakPermission(ClientContext clientContext, string MatterName, bool copyPermissionsFromParent, string calendarName = null)
        {
            bool flag = false;
            try
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                string matterOrCalendar = !string.IsNullOrWhiteSpace(calendarName) ? calendarName : MatterName;
                List list = web.Lists.GetByTitle(matterOrCalendar);
                clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();
                if (!list.HasUniqueRoleAssignments)
                {
                    list.BreakRoleInheritance(copyPermissionsFromParent, true);
                    list.Update();
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, "Message: " + "Matter name: " + exception.Message + MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
            return flag;
        }

        // <summary>
        // Creates the One Note.
        // </summary>
        // <param name="clientContext">The client context.</param>
        // <param name="clientUrl">The client URL.</param>
        // <param name="matter">Matter object containing Matter data</param>
        // <returns>Returns the URL of the One Note</returns>
        internal static string CreateOneNote(ClientContext clientContext, Uri clientUrl, Matter matter)
        {
            string returnValue = String.Empty;
            try
            {
                byte[] oneNoteFile = System.IO.File.ReadAllBytes("./Open Notebook.onetoc2");

                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                Microsoft.SharePoint.Client.File file = web.GetFolderByServerRelativeUrl(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", clientUrl.AbsolutePath, Constants.Backslash, matter.MatterGuid + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], Constants.Backslash, matter.MatterGuid)).Files.Add(new FileCreationInformation()
                {
                    Url = string.Format(CultureInfo.InvariantCulture, "{0}{1}", matter.MatterGuid, ConfigurationManager.AppSettings["ExtensionOneNoteTableOfContent"]),
                    Overwrite = true,
                    ContentStream = new MemoryStream(oneNoteFile)
                });
                web.Update();
                clientContext.Load(file);
                clientContext.ExecuteQuery();
                ListItem oneNote = file.ListItemAllFields;
                oneNote["Title"] = matter.MatterName;
                oneNote.Update();
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}{6}", clientUrl.Scheme, Constants.COLON, Constants.Backslash, Constants.Backslash, clientUrl.Authority, file.ServerRelativeUrl, "?Web=1");
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, "Message: " + "Matter name: " + matter.MatterName + "\n" + exception.Message + "\nStacktrace: " + exception.StackTrace);
                throw;
            }

            return returnValue;
        }

        // <summary>
        // Adds folders inside the document library
        // </summary>
        internal static List AddFolders(ClientContext clientContext, List list, IList<string> folderNames)
        {
            // Addition of Folders
            FolderCollection listFolders = list.RootFolder.Folders;
            Microsoft.SharePoint.Client.Folder listRootFolder = list.RootFolder;
            clientContext.Load(listFolders);
            if (0 < folderNames.Count)
            {
                foreach (string folderName in folderNames)
                {
                    // Check for empty folder names
                    if (!string.IsNullOrWhiteSpace(folderName))
                    {
                        listFolders.Add(folderName);
                        listRootFolder.Update();
                    }
                }
                list.Update();
                clientContext.ExecuteQuery();
            }
            return list;
        }

        /// <summary>
        /// Checks if the requested page exists or not.
        /// </summary>
        /// <param name="requestedUrl">URL of the page, for which check is to be performed</param>
        /// <param name="clientContext">ClientContext for SharePoint</param>
        /// <returns>true or false string based upon the existence of the page, referred in requestedUrl</returns>
        internal static bool PageExists(string requestedUrl, ClientContext clientContext)
        {
            Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(requestedUrl);
            clientContext.Load(clientFile, cf => cf.Exists);
            clientContext.ExecuteQuery();
            return clientFile.Exists;
        }

        //// <summary>
        //// Deletes Matter if exception occur post creation
        //// </summary>
        //// <param name="clientContext">Client context object for SharePoint</param>
        //// <param name="matter">Matter object containing Matter data</param>
        internal static void DeleteMatter(ClientContext clientContext, Matter matter)
        {
            //Delete matter library
            MatterProvisionHelperFunction.DeleteMatterObject(clientContext, matter.MatterName, true);
            //Delete Task List
            MatterProvisionHelperFunction.DeleteMatterObject(clientContext, matter.MatterName + ConfigurationManager.AppSettings["TaskListSuffix"], true);
            //Delete OneNote library
            MatterProvisionHelperFunction.DeleteMatterObject(clientContext, matter.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], true);
            //Delete Calendar list
            MatterProvisionHelperFunction.DeleteMatterObject(clientContext, matter.MatterName + ConfigurationManager.AppSettings["CalendarNameSuffix"], true);
            //Delete Matter landing page
            MatterProvisionHelperFunction.DeleteMatterObject(clientContext, matter.MatterGuid, false);
            Console.WriteLine(ConfigurationManager.AppSettings["dashedLine"]);
        }

        /// <summary>
        /// Gets the encoded value for the search index property
        /// </summary>
        internal static string GetEncodedValueForSearchIndexProperty(List<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (keys != null)
            {
                foreach (string current in keys)
                {
                    stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                    stringBuilder.Append('|');
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the property value for the list
        /// </summary>
        internal static string GetPropertyValueForList(ClientContext context, string matterName, Dictionary<string, string> propertyList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (context != null)
            {
                var props = context.Web.Lists.GetByTitle(matterName).RootFolder.Properties;
                context.Load(props);
                context.ExecuteQuery();
                if (propertyList != null)
                {
                    foreach (var item in propertyList)
                    {
                        if (props.FieldValues.ContainsKey(item.Key))
                        {
                            stringBuilder.Append(props.FieldValues[item.Key].ToString());
                            stringBuilder.Append('|');
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Sets the property value for the list
        /// </summary>
        internal static void SetPropertyValueForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            var oList = clientContext.Web.Lists.GetByTitle(matterName);

            foreach (var item in propertyList)
            {
                props[item.Key] = item.Value;
                oList.RootFolder.Update();
            }
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Gets content type data
        /// </summary>
        internal static IList<ContentType> GetContentTypeData(ClientContext clientContext, IList<string> contentTypeNames)
        {
            ContentTypeCollection ContentTypeCollection = null;
            IList<ContentType> SelectedContentTypeCollection = null;
            if (null != clientContext && null != contentTypeNames)
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                ContentTypeCollection = web.ContentTypes;
                clientContext.Load(ContentTypeCollection);
                clientContext.ExecuteQuery();
                SelectedContentTypeCollection = new List<ContentType>();
                foreach (string contentTypeName in contentTypeNames)
                {
                    foreach (ContentType contentType in ContentTypeCollection)
                    {
                        if (string.Equals(contentTypeName, contentType.Name))
                        {
                            SelectedContentTypeCollection.Add(contentType);
                        }
                    }
                }
            }
            return SelectedContentTypeCollection;
        }

        /// <summary>
        /// Assigns permissions for user
        /// </summary>
        internal static string AssignUserPermissions(ClientContext clientcontext, string MatterName, List<string> Users, string permission, string CalendarName = null)
        {
            {
                string returnvalue = "false";
                try
                {
                    List<string> permissions = new List<string>();
                    permissions.Add(permission);
                    Microsoft.SharePoint.Client.Web web = clientcontext.Web;
                    clientcontext.Load(web.RoleDefinitions);
                    string matterOrCalendar = !string.IsNullOrWhiteSpace(CalendarName) ? CalendarName : MatterName;
                    List list = web.Lists.GetByTitle(matterOrCalendar);
                    clientcontext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientcontext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments)
                    {
                        if (null != permissions && null != Users) //matter.permissions=read/limited access/contribute/ full control/ view only
                        {
                            int position = 0;
                            foreach (string rolename in permissions)
                            {
                                try
                                {
                                    RoleDefinitionCollection roleDefinitions = clientcontext.Web.RoleDefinitions;
                                    RoleDefinition role = (from roleDef in roleDefinitions
                                                           where roleDef.Name == rolename
                                                           select roleDef).First();

                                    foreach (string user in Users)
                                    {
                                        //get the user object
                                        Principal userprincipal = clientcontext.Web.EnsureUser(user);
                                        //create the role definition binding collection
                                        RoleDefinitionBindingCollection roledefinitionbindingcollection = new RoleDefinitionBindingCollection(clientcontext);
                                        //add the role definition to the collection
                                        roledefinitionbindingcollection.Add(role);
                                        //create a role assignment with the user and role definition
                                        list.RoleAssignments.Add(userprincipal, roledefinitionbindingcollection);
                                    }
                                    //execute the query to add everything
                                    clientcontext.ExecuteQuery();
                                }
                                // SharePoint specific exception
                                catch (ClientRequestException clientRequestException)
                                {
                                    DisplayAndLogError(errorFilePath, "Message: " + clientRequestException.Message + "Matter name: " + MatterName + "\nStacktrace: " + clientRequestException.StackTrace);
                                    throw;
                                }
                                // SharePoint specific exception
                                catch (ServerException serverException)
                                {
                                    DisplayAndLogError(errorFilePath, "Message: " + serverException.Message + "Matter name: " + MatterName + "\nStacktrace: " + serverException.StackTrace);
                                    throw;
                                }
                                position++;
                            }
                        }
                        // success. return a success code
                        returnvalue = "true";
                    }

                }
                // web exception
                catch (Exception exception)
                {
                    DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "assigning Permission"));
                    DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "Matter name: " + MatterName + "\nStacktrace: " + exception.StackTrace);
                    throw;
                }
                return returnvalue;
            }
        }

        /// <summary>
        /// Sets the default content type
        /// </summary>
        internal static void SetDefaultContentType(ClientContext clientContext, List list, string defaultContentType)
        {
            ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
            clientContext.Load(currentContentTypeOrder);
            clientContext.ExecuteQuery();

            IList<ContentTypeId> updatedContentTypeOrder = new List<ContentTypeId>();
            int contentCount = 0, contentSwap = 0;
            foreach (ContentType contentType in currentContentTypeOrder)
            {
                if (contentType.Name.Equals(defaultContentType))
                {
                    contentSwap = contentCount;
                }
                if (!contentType.Name.Equals("Folder"))
                {
                    updatedContentTypeOrder.Add(contentType.Id);
                    contentCount++;
                }
            }

            if (updatedContentTypeOrder.Count > contentSwap)
            {
                ContentTypeId documentContentType = updatedContentTypeOrder[0];
                updatedContentTypeOrder[0] = updatedContentTypeOrder[contentSwap];
                updatedContentTypeOrder.RemoveAt(contentSwap);
                updatedContentTypeOrder.Add(documentContentType);
            }

            list.RootFolder.UniqueContentTypeOrder = updatedContentTypeOrder;
            list.RootFolder.Update();
            list.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Assigns content type
        /// </summary>
        /// <param name="clientcontext">SP client context</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        internal static void AssignContentType(ClientContext clientcontext, MatterMetadata matterMetadata)
        {
            // For each value in the list of Content Type Names
            // Add that content Type to the Library
            string defaultContentType = matterMetadata.ContentTypes[0];
            try
            {
                // Returns the selected Content types from the Site Content Types
                IList<ContentType> ContentTypeCollection = GetContentTypeData(clientcontext, matterMetadata.ContentTypes);
                if (null != ContentTypeCollection)
                {
                    Microsoft.SharePoint.Client.Web web = clientcontext.Web;
                    List MatterList = web.Lists.GetByTitle(matterMetadata.Matter.MatterName);
                    FieldCollection fields = GetContentType(clientcontext, ContentTypeCollection, MatterList);
                    matterMetadata = GetWSSId(clientcontext, matterMetadata, fields);
                    SetFieldValues(matterMetadata, fields);
                    clientcontext.ExecuteQuery();
                    SetDefaultContentType(clientcontext, MatterList, defaultContentType);
                    CreateView(clientcontext, MatterList);
                }
            }
            // SharePoint Specific Exception
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "assigning ContentType"));
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "Matter name: " + matterMetadata.Matter.MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }

        }

        /// <summary>
        /// Retrieves the list of content types that are to be associated with the matter
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="contentTypeCollection">Collection of content types</param>
        /// <param name="matterList">List containing matters</param>
        /// <returns>Content types in Field Collection object</returns>
        internal static FieldCollection GetContentType(ClientContext clientContext, IList<ContentType> contentTypeCollection, List matterList)
        {
            foreach (ContentType contenttype in contentTypeCollection)
            {
                matterList.ContentTypesEnabled = true;
                matterList.ContentTypes.AddExistingContentType(contenttype);
            }

            matterList.Update();
            FieldCollection fields = matterList.Fields;
            clientContext.Load(fields);
            clientContext.Load(matterList);
            clientContext.ExecuteQuery();
            return fields;
        }

        /// <summary>
        /// Assign field values for specified content types to the specified matter (document library)
        /// </summary>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <param name="fields">Field Collection object</param>
        internal static void SetFieldValues(MatterMetadata matterMetadata, FieldCollection fields)
        {
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).DefaultValue = matterMetadata.Client.ClientId;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).DefaultValue = matterMetadata.Client.ClientName;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).Update();

            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).DefaultValue = matterMetadata.Matter.MatterId;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).DefaultValue = matterMetadata.Matter.MatterName;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]).DefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, matterMetadata.PracticeGroupTerm.WssId, matterMetadata.PracticeGroupTerm.TermName, matterMetadata.PracticeGroupTerm.Id); ;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]).DefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, matterMetadata.AreaTerm.WssId, matterMetadata.AreaTerm.TermName, matterMetadata.AreaTerm.Id);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]).DefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, matterMetadata.SubareaTerm.WssId, matterMetadata.SubareaTerm.TermName, matterMetadata.SubareaTerm.Id);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]).Update();
        }

        /// <summary>
        /// Function to get the WssID for the Practice group, Area of law and Subarea of law terms
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <param name="fields">Field Collection object</param>
        /// <returns>An Object containing metadata for Matter</returns>
        internal static MatterMetadata GetWSSId(ClientContext clientContext, MatterMetadata matterMetadata, FieldCollection fields)
        {
            ClientResult<TaxonomyFieldValue> practiceGroupResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.PracticeGroupTerm.Id);
            ClientResult<TaxonomyFieldValue> areaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.AreaTerm.Id);
            ClientResult<TaxonomyFieldValue> subareaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.SubareaTerm.Id);
            clientContext.ExecuteQuery();

            matterMetadata.PracticeGroupTerm.WssId = practiceGroupResult.Value.WssId;
            matterMetadata.AreaTerm.WssId = areaOfLawResult.Value.WssId;
            matterMetadata.SubareaTerm.WssId = subareaOfLawResult.Value.WssId;
            return matterMetadata;
        }

        /// <summary>
        /// Creates a new view for the document library (Matter)
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="matterList">Name of the list</param>
        /// <returns>True if success else False</returns>
        internal static void CreateView(ClientContext clientContext, List matterList)
        {
            try
            {
                string viewName = ConfigurationManager.AppSettings["ViewName"];
                string[] viewColumnList = ConfigurationManager.AppSettings["ViewColumnList"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(listEntry => listEntry.Trim()).ToArray();
                View outlookView = matterList.Views.Add(new ViewCreationInformation
                {
                    Title = viewName,
                    ViewTypeKind = ViewType.Html,
                    ViewFields = viewColumnList,
                    Paged = true
                });
                string strQuery = string.Format(CultureInfo.InvariantCulture, Constants.ViewOrderByQuery, ConfigurationManager.AppSettings["ViewOrderByColumn"]);
                outlookView.ViewQuery = strQuery;
                outlookView.Update();
                clientContext.ExecuteQuery();
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Created {0}", viewName));
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "creating Outlook View"));
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Updates meta data for list
        /// </summary>
        internal static string UpdateMetadataForList(ClientContext clientContext, Matter matter, Client client)
        {
            string properties = string.Empty;
            try
            {
                var props = clientContext.Web.Lists.GetByTitle(matter.MatterName).RootFolder.Properties;
                List<string> keys = new List<string>();
                keys.Add("PracticeGroup");
                keys.Add("AreaOfLaw");
                keys.Add("SubAreaOfLaw");
                keys.Add("MatterName");
                keys.Add("MatterID");
                keys.Add("ClientName");
                keys.Add("ClientID");
                keys.Add("ResponsibleAttorney");
                keys.Add("TeamMembers");
                keys.Add("IsMatter");
                keys.Add("OpenDate");
                keys.Add("SecureMatter");
                keys.Add("BlockedUploadUsers");
                keys.Add("Success");
                keys.Add("IsConflictIdentified");
                keys.Add("MatterConflictCheckBy");
                keys.Add("MatterConflictCheckDate");
                keys.Add("MatterCenterPermissions");
                keys.Add("MatterCenterRoles");
                keys.Add("MatterCenterUsers");
                keys.Add("DocumentTemplateCount");
                keys.Add("MatterCenterDefaultContentType");
                keys.Add("MatterDescription");
                keys.Add("BlockedUsers");
                keys.Add("MatterGUID");

                Dictionary<string, string> propertyList = new Dictionary<string, string>();
                propertyList.Add("PracticeGroup", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterType.PracticeGroup));
                propertyList.Add("AreaOfLaw", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterType.AreaofLaw));
                propertyList.Add("SubAreaOfLaw", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterType.SubAreaofLaw));
                propertyList.Add("MatterName", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterName));
                propertyList.Add("MatterID", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterId));
                propertyList.Add("ClientName", Microsoft.Security.Application.Encoder.HtmlEncode(client.ClientName));
                propertyList.Add("ClientID", Microsoft.Security.Application.Encoder.HtmlEncode(client.ClientId));
                propertyList.Add("ResponsibleAttorney", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.ResponsibleAttorneys));
                propertyList.Add("TeamMembers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.Attorneys));
                propertyList.Add("IsMatter", Microsoft.Security.Application.Encoder.HtmlEncode("true"));
                propertyList.Add("OpenDate", Microsoft.Security.Application.Encoder.HtmlEncode(matter.OpenDate));
                propertyList.Add("SecureMatter", Microsoft.Security.Application.Encoder.HtmlEncode(matter.Conflict.SecureMatter));
                propertyList.Add("Success", Microsoft.Security.Application.Encoder.HtmlEncode("true"));
                propertyList.Add("BlockedUsers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.BlockedUsers));
                propertyList.Add("IsConflictIdentified", Microsoft.Security.Application.Encoder.HtmlEncode("true"));
                propertyList.Add("MatterConflictCheckBy", Microsoft.Security.Application.Encoder.HtmlEncode(matter.Conflict.ConflictCheckBy.TrimEnd(';')));
                propertyList.Add("MatterConflictCheckDate", Microsoft.Security.Application.Encoder.HtmlEncode(matter.Conflict.ConflictCheckOn));
                propertyList.Add("MatterCenterRoles", Microsoft.Security.Application.Encoder.HtmlEncode(ConfigurationManager.AppSettings["Roles"]));
                propertyList.Add("MatterCenterUsers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.ResponsibleAttorneys + Constants.SEPARATOR + matter.TeamInfo.Attorneys + Constants.SEPARATOR + matter.TeamInfo.BlockedUploadUsers).TrimEnd(';'));
                propertyList.Add("MatterCenterPermissions", Microsoft.Security.Application.Encoder.HtmlEncode(ConfigurationManager.AppSettings["FullControl"] + Constants.SEPARATOR + ConfigurationManager.AppSettings["Contribute"] + Constants.SEPARATOR + ConfigurationManager.AppSettings["Read"]));
                propertyList.Add("DocumentTemplateCount", Microsoft.Security.Application.Encoder.HtmlEncode(matter.DocumentCount));
                propertyList.Add("MatterCenterDefaultContentType", Microsoft.Security.Application.Encoder.HtmlEncode(matter.DefaultContentType));
                propertyList.Add("MatterDescription", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterDescription));
                propertyList.Add("BlockedUploadUsers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.BlockedUploadUsers.TrimEnd(';')));
                propertyList.Add("MatterGUID", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterGuid));
                propertyList.Add("vti_indexedpropertykeys", Microsoft.Security.Application.Encoder.HtmlEncode(GetEncodedValueForSearchIndexProperty(keys)));

                clientContext.Load(props);
                clientContext.ExecuteQuery();

                SetPropertyValueForList(clientContext, props, matter.MatterName, propertyList);
                properties = GetPropertyValueForList(clientContext, matter.MatterName, propertyList);

            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "while updating Metadata"));
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "Matter name: " + matter.MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
            return properties;
        }

        internal static IList<FieldUserValue> ResolveUserNames(Dictionary<string, string> configVal, string tempUserNames)
        {
            IList<string> userNames = new List<string>();
            if (!string.IsNullOrEmpty(tempUserNames) && string.Empty != tempUserNames.Split(';')[0].Trim())
            {
                foreach (string temp in tempUserNames.Split(';'))
                {
                    userNames.Add(temp);
                }

                List<FieldUserValue> userList = new List<FieldUserValue>();
                if (null != userNames)
                {
                    foreach (string userName in userNames)
                    {
                        if (!string.IsNullOrWhiteSpace(userName))
                        {
                            using (ClientContext clientContext = GetClientContext(configVal["CatalogSiteURL"], configVal))
                            {
                                User user = clientContext.Web.EnsureUser(userName.Trim());
                                // Only Fetch the User ID which is required
                                clientContext.Load(user); //, u => u.Id);
                                clientContext.ExecuteQuery();
                                // Add the user to the first element of the FieldUserValue array.
                                FieldUserValue tempUser = new FieldUserValue();
                                tempUser.LookupId = user.Id;
                                userList.Add(tempUser);
                            }
                        }
                    }
                }
                return userList;
            }
            return null;
        }

        /// <summary>
        /// Inserts details into LegalDMSMatters list
        /// </summary>
        internal static string InsertIntoMatterCenterMatters(Dictionary<string, string> configVal, string title, Matter matter, Client client)
        {
            try
            {
                using (ClientContext clientContext = GetClientContext(configVal["CatalogSiteURL"], configVal))
                {
                    bool isConflict = Convert.ToBoolean(matter.Conflict.ConflictIdentified, CultureInfo.InvariantCulture);
                    List<FieldUserValue> conflictByUsers = ResolveUserNames(configVal, matter.Conflict.ConflictCheckBy).ToList<FieldUserValue>(),
                        blockedUsers = !string.IsNullOrEmpty(matter.TeamInfo.BlockedUsers.Trim()) ? ResolveUserNames(configVal, matter.TeamInfo.BlockedUsers).ToList<FieldUserValue>() : null,
                        managingUsers = ResolveUserNames(configVal, matter.TeamInfo.ResponsibleAttorneys).ToList<FieldUserValue>(),
                        supportUsers = !string.IsNullOrEmpty(matter.TeamInfo.Attorneys.Trim()) ? ResolveUserNames(configVal, matter.TeamInfo.Attorneys).ToList<FieldUserValue>() : null;

                    List oList = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["MatterCenterList"]);
                    ListItemCreationInformation listCreationInformation = new ListItemCreationInformation();
                    ListItem oListItem = oList.AddItem(listCreationInformation);
                    oListItem["Title"] = title;// "MyTitle";
                    oListItem["ClientName"] = client.ClientName;// "https://lcadms.sharepoint.com/sites/catalog/";
                    oListItem["ClientID"] = client.ClientId;// "MyID";
                    oListItem["MatterName"] = matter.MatterName;
                    oListItem["MatterID"] = matter.MatterId;

                    oListItem["ConflictCheckBy"] = conflictByUsers;
                    oListItem["ConflictCheckOn"] = matter.Conflict.ConflictCheckOn;
                    oListItem["ConflictIdentified"] = isConflict;

                    if (isConflict) { oListItem["BlockUsers"] = blockedUsers; }
                    oListItem["ManagingAttorney"] = managingUsers;
                    oListItem["Support"] = supportUsers;

                    oListItem.Update();
                    clientContext.ExecuteQuery();
                }
                return "true";
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "while updating Metadata"));
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "Matter name: " + matter.MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Fetches matter details from excel sheet
        /// </summary>
        internal static List<DataStorage> FetchMatterData(List<List<string>> dataValue)
        {
            List<DataStorage> kvp = new List<DataStorage>();
            int xlRange = dataValue.Count;
            if (0 != xlRange)
            {
                int rCnt;
                for (rCnt = 1; rCnt <= xlRange - 1; rCnt++)
                {
                    if (!string.IsNullOrWhiteSpace(dataValue[rCnt][0]))
                    {
                        DataStorage Tuple = new DataStorage();
                        Tuple.ClientName = Convert.ToString(dataValue[rCnt][0], CultureInfo.InvariantCulture).Trim();
                        Tuple.MatterPrefix = Convert.ToString(dataValue[rCnt][1], CultureInfo.InvariantCulture).Trim();
                        Tuple.MatterDescription = Convert.ToString(dataValue[rCnt][2], CultureInfo.InvariantCulture).Trim();
                        Tuple.MatterIdPrefix = Convert.ToString(dataValue[rCnt][3], CultureInfo.InvariantCulture).Trim();
                        Tuple.PracticeGroup = Convert.ToString(dataValue[rCnt][4], CultureInfo.InvariantCulture).Trim();
                        Tuple.AreaOfLaw = Convert.ToString(dataValue[rCnt][5], CultureInfo.InvariantCulture).Trim();
                        Tuple.SubAreaOfLaw = Convert.ToString(dataValue[rCnt][6], CultureInfo.InvariantCulture).Trim();
                        Tuple.ConflictCheckConductedBy = Convert.ToString(dataValue[rCnt][7], CultureInfo.InvariantCulture).Trim();
                        Tuple.ConflictDate = Convert.ToString(dataValue[rCnt][8], CultureInfo.InvariantCulture).Trim();
                        Tuple.BlockedUser = Convert.ToString(dataValue[rCnt][9], CultureInfo.InvariantCulture).Trim();
                        Tuple.ResponsibleAttorneys = Convert.ToString(dataValue[rCnt][10], CultureInfo.InvariantCulture).Trim();
                        Tuple.Attorneys = Convert.ToString(dataValue[rCnt][11], CultureInfo.InvariantCulture).Trim();
                        Tuple.BlockedUploadUsers = Convert.ToString(dataValue[rCnt][12], CultureInfo.InvariantCulture).Trim();
                        Tuple.ConflictIdentified = Convert.ToString(dataValue[rCnt][14], CultureInfo.InvariantCulture).Trim();
                        Tuple.CopyPermissionsFromParent = Convert.ToBoolean(Tuple.ConflictIdentified, CultureInfo.InvariantCulture) ? Constants.FALSE.ToUpperInvariant() : Convert.ToString(dataValue[rCnt][13], CultureInfo.InvariantCulture).Trim();
                        kvp.Add(Tuple);
                    }
                }
            }
            return kvp;
        }

        /// <summary>
        /// Validates the semicolon separate string
        /// </summary>
        internal static string ProcessString(string receivedString)
        {
            string processedString = string.Empty;
            List<string> listOfValues = receivedString.Trim().Split(';').Where(input => !string.IsNullOrWhiteSpace(input.Trim())).ToList();
            processedString = string.Join(";", listOfValues);
            return processedString.Trim();
        }

        /// <summary>
        /// Validate and change the date format to universal sortable format 
        /// </summary>
        internal static string ValidateDateFormat(string currentDate)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(currentDate))
                {
                    DateTime todaysDate = DateTime.Today;
                    DateTime dt = DateTime.FromOADate(Convert.ToDouble(currentDate, CultureInfo.InvariantCulture));
                    if (DateTime.Compare(dt, todaysDate) < 0)
                    {
                        result = dt.ToString(System.Configuration.ConfigurationManager.AppSettings["DateFormat"], CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        result = Constants.InvalidConflictDate;
                    }
                }
            }
            catch (Exception)
            {
                result = Constants.InvalidConflictDate;
            }
            return result;
        }

        /// <summary>
        /// Converts string to Pascal case
        /// </summary>
        internal static string UpperFirst(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : Char.ToUpper(str[0], CultureInfo.InvariantCulture) + str.Substring(1).ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Extract term store groups
        /// </summary>
        /// <param name="configVal">Configuration values from excel</param>
        /// <returns>Term sets</returns>
        internal static TermSets FetchGroupTerms(Dictionary<string, string> configVal)
        {
            TermStoreDetails termStoreDetails = new TermStoreDetails();
            termStoreDetails.TermSetName = ConfigurationManager.AppSettings["PracticeGroupTermSetName"];
            termStoreDetails.CustomPropertyName = ConfigurationManager.AppSettings["CustomPropertyName"];
            termStoreDetails.TermGroup = ConfigurationManager.AppSettings["PracticeGroupName"];
            TermSets practiceGroupTermSets = null;

            using (ClientContext clientContext = GetClientContext(configVal["TenantAdminURL"], configVal))
            {
                TaxonomySession taxanomySession = TaxonomySession.GetTaxonomySession(clientContext);
                clientContext.Load(taxanomySession,
                                    items => items.TermStores.Include(
                                        item => item.Groups,
                                        item => item.Groups.Include(
                                            group => group.Name)));
                clientContext.ExecuteQuery();
                TermGroup termGroup = taxanomySession.TermStores[0].Groups.GetByName(termStoreDetails.TermGroup);
                clientContext.Load(termGroup,
                                       group => group.Name,
                                       group => group.TermSets.Include(
                                           termSet => termSet.Name,
                                           termSet => termSet.Terms.Include(
                                               term => term.Name,
                                               term => term.Id,
                                               term => term.CustomProperties,
                                               term => term.Terms.Include(
                                                   termArea => termArea.Name,
                                                   termArea => termArea.Id,
                                                   termArea => termArea.CustomProperties,
                                                   termArea => termArea.Terms.Include(
                                                       termSubArea => termSubArea.Name,
                                                       termSubArea => termSubArea.Id,
                                                       termSubArea => termSubArea.CustomProperties)))));
                clientContext.ExecuteQuery();

                foreach (TermSet termSet in termGroup.TermSets)
                {
                    if (termSet.Name.Equals(termStoreDetails.TermSetName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (termStoreDetails.TermSetName.Equals(termStoreDetails.TermSetName, StringComparison.OrdinalIgnoreCase))
                        {
                            practiceGroupTermSets = GetPracticeGroupTermSetHierarchy(termSet, termStoreDetails.CustomPropertyName);
                        }
                    }
                }

                return practiceGroupTermSets;
            }
        }

        /// <summary>
        /// Load practice group term set from term store 
        /// </summary>
        /// <param name="termSet">Term set containing term set data</param>
        /// <param name="customPropertyName">Custom property name</param>
        /// <returns>Term sets</returns>
        internal static TermSets GetPracticeGroupTermSetHierarchy(TermSet termSet, string customPropertyName)
        {
            TermSets tempTermSet = new TermSets();
            string practiceGroupCustomPropertyFolderNames = ConfigurationManager.AppSettings["PracticeGroupCustomPropertyFolderNames"];
            string areaCustomPropertyFolderNames = ConfigurationManager.AppSettings["AreaCustomPropertyFolderNames"];
            string subAreaCustomPropertyFolderNames = ConfigurationManager.AppSettings["SubAreaCustomPropertyFolderNames"];
            string subAreaCustomPropertyisNoFolderStructurePresent = ConfigurationManager.AppSettings["SubAreaCustomPropertyisNoFolderStructurePresent"];
            string subAreaOfLawDocumentTemplates = ConfigurationManager.AppSettings["SubAreaOfLawDocumentTemplates"];

            try
            {
                tempTermSet.Name = termSet.Name;
                ////Retrieve the Terms - level 1
                tempTermSet.PGTerms = new List<PracticeGroupTerm>();
                TermCollection termColl = termSet.Terms;
                foreach (Term term in termColl)
                {
                    PracticeGroupTerm tempTermPG = new PracticeGroupTerm();
                    tempTermPG.TermName = term.Name;
                    tempTermPG.Id = Convert.ToString(term.Id, CultureInfo.InvariantCulture);
                    tempTermPG.ParentTermName = termSet.Name;
                    /////Retrieve the custom property for Terms at level 1
                    foreach (KeyValuePair<string, string> customProperty in term.CustomProperties)
                    {
                        tempTermPG.FolderNames = (customProperty.Key.Equals(practiceGroupCustomPropertyFolderNames, StringComparison.OrdinalIgnoreCase)) ? customProperty.Value : string.Empty;
                    }
                    tempTermSet.PGTerms.Add(tempTermPG);
                    ///// Retrieve the Terms - level 2
                    tempTermPG.AreaTerms = new List<AreaTerm>();
                    TermCollection termCollArea = term.Terms;
                    foreach (Term termArea in termCollArea)
                    {
                        AreaTerm tempTermArea = new AreaTerm();
                        tempTermArea.TermName = termArea.Name;
                        tempTermArea.Id = Convert.ToString(termArea.Id, CultureInfo.InvariantCulture);
                        tempTermArea.ParentTermName = term.Name;
                        /////Retrieve the custom property for Terms at level 2
                        foreach (KeyValuePair<string, string> customProperty in termArea.CustomProperties)
                        {
                            tempTermArea.FolderNames = (customProperty.Key.Equals(areaCustomPropertyFolderNames, StringComparison.OrdinalIgnoreCase)) ? customProperty.Value : string.Empty;
                        }
                        tempTermPG.AreaTerms.Add(tempTermArea);
                        /////Retrieve the Terms - level 3
                        tempTermArea.SubareaTerms = new List<SubareaTerm>();
                        TermCollection termCollSubArea = termArea.Terms;
                        foreach (Term termSubArea in termCollSubArea)
                        {
                            SubareaTerm tempTermSubArea = new SubareaTerm();
                            tempTermSubArea.TermName = termSubArea.Name;
                            tempTermSubArea.Id = Convert.ToString(termSubArea.Id, CultureInfo.InvariantCulture);
                            tempTermSubArea.ParentTermName = termArea.Name;
                            /////Retrieve the custom property for Terms at level 3

                            tempTermSubArea.DocumentTemplates = string.Empty;
                            foreach (KeyValuePair<string, string> customProperty in termSubArea.CustomProperties)
                            {
                                if (customProperty.Key.Equals(customPropertyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.ContentTypeName = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(subAreaCustomPropertyFolderNames, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.FolderNames = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(subAreaCustomPropertyisNoFolderStructurePresent, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.IsNoFolderStructurePresent = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(subAreaOfLawDocumentTemplates, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.DocumentTemplates = customProperty.Value;
                                }
                            }
                            tempTermArea.SubareaTerms.Add(tempTermSubArea);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                tempTermSet = null;
            }
            return tempTermSet;
        }

        /// <summary>
        /// Get a client context
        /// </summary>
        /// <param name="siteURl">Site Url</param>
        /// <param name="configVal">Configuration values from excel</param>
        /// <returns>client context</returns>
        internal static ClientContext GetClientContext(string siteURl, Dictionary<string, string> configVal)
        {
            string login = configVal["Username"];
            string password = configVal["Password"];
            bool isDeployedOnAzure = Convert.ToBoolean(configVal["IsDeployedOnAzure"], CultureInfo.InvariantCulture);
            ClientContext clientContext = null;
            try
            {
                clientContext = new ClientContext(siteURl);
                var securePassword = new SecureString();
                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }
                object onlineCredentials;
                if (isDeployedOnAzure)
                {
                    onlineCredentials = new SharePointOnlineCredentials(login, securePassword);
                    clientContext.Credentials = (SharePointOnlineCredentials)onlineCredentials;// Secure the crdentials and generate the SharePoint Online Credentials                    
                }
                else
                {
                    onlineCredentials = new NetworkCredential(login, securePassword);
                    clientContext.Credentials = (NetworkCredential)onlineCredentials; // Assign On Premise credentials to the Client Context
                }
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return clientContext;
        }

        /// <summary>
        /// Function to return client id and client url from term store 
        /// </summary>
        /// <param name="clientName">Client Name</param>
        /// <param name="configVal">Configuration from excel file</param>
        /// <returns>ClientId and ClientUrl</returns>
        internal static ClientTermSets GetClientDetails(Dictionary<string, string> configVal)
        {
            ClientTermSets clientDetails = new ClientTermSets();
            clientDetails.ClientTerms = new List<Client>();

            string groupName = ConfigurationManager.AppSettings["PracticeGroupName"];
            string termSetName = ConfigurationManager.AppSettings["TermSetName"];
            string clientIdProperty = ConfigurationManager.AppSettings["ClientIDProperty"];
            string clientUrlProperty = ConfigurationManager.AppSettings["ClientUrlProperty"];

            // 1. get client context
            using (ClientContext clientContext = GetClientContext(configVal["TenantAdminURL"], configVal))
            {
                if (null != clientContext)
                {
                    // 2. Create taxonomy session
                    TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                    clientContext.Load(taxonomySession.TermStores);
                    clientContext.ExecuteQuery();

                    // 3. Create term store object and load data
                    TermStore termStore = taxonomySession.TermStores[0];
                    clientContext.Load(
                        termStore,
                        store => store.Name,
                        store => store.Groups.Include(
                            group => group.Name));
                    clientContext.ExecuteQuery();

                    // 4. create a term group object and load data
                    TermGroup termGroup = termStore.Groups.Where(item => item.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    clientContext.Load(
                                  termGroup,
                                   group => group.Name,
                                   group => group.TermSets.Include(
                                       termSet => termSet.Name,
                                       termSet => termSet.Terms.Include(
                                           term => term.Name,
                                           term => term.CustomProperties)));
                    clientContext.ExecuteQuery();

                    // 5. Get required term from term from extracted term set
                    TermCollection fillteredTerms = termGroup.TermSets.Where(item => item.Name.Equals(termSetName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Terms;
                    Client client;
                    foreach (Term term in fillteredTerms)
                    {
                        if (term.CustomProperties.ContainsKey(clientIdProperty) && term.CustomProperties.ContainsKey(clientUrlProperty))
                        {
                            client = new Client();
                            client.ClientName = term.Name;
                            client.ClientId = term.CustomProperties[clientIdProperty];
                            client.ClientUrl = term.CustomProperties[clientUrlProperty];
                            clientDetails.ClientTerms.Add(client);
                        }
                    }
                }
                else
                {
                    clientDetails = null;
                }
            }
            return clientDetails;
        }

        /// <summary>
        /// Deletes the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="objectName">Object name</param>
        /// <param name="isList">Flag to specify is matter object is list</param>
        internal static void DeleteMatterObject(ClientContext clientContext, string objectName, bool isList)
        {
            try
            {
                if (isList)
                {
                    List list = clientContext.Web.Lists.GetByTitle(objectName);
                    list.DeleteObject();
                    clientContext.ExecuteQuery();
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["Deleted"], objectName));
                }
                else
                {
                    clientContext.Load(clientContext.Web, webDetails => webDetails.ServerRelativeUrl);
                    clientContext.ExecuteQuery();
                    string matterLandingPageUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", clientContext.Web.ServerRelativeUrl, Constants.Backslash, ConfigurationManager.AppSettings["MatterLandingPageRepository"].Replace(Constants.SPACE, string.Empty), Constants.Backslash, objectName, Constants.AspxExtension);
                    Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(matterLandingPageUrl);
                    clientFile.DeleteObject();
                    clientContext.ExecuteQuery();
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["DeletedMatter"], objectName));
                }

            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Log and display error
        /// </summary>
        /// <param name="errorFilePath">Error file path</param>
        /// <param name="errorMessage">Error message to display or write into file</param>
        /// <param name="logError">Flag to log error into file or not</param>
        internal static void DisplayAndLogError(string errorFilePath, string errorMessage)
        {
            ConsoleColor currentColor = Console.ForegroundColor;    // Save current foreground color
            Console.ForegroundColor = ConsoleColor.Red;
            ErrorLogger.LogErrorToTextFile(errorFilePath, errorMessage);
            Console.ForegroundColor = currentColor;
        }
        /// <summary>
        /// Creates task list while provisioning matter
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="matter">Matter object containing Matter data</param>
        internal static void CreateTaskList(ClientContext clientContext, Matter matter)
        {
            try
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                ListCreationInformation creationInfo = new ListCreationInformation();
                string listName = matter.MatterName + ConfigurationManager.AppSettings["TaskListSuffix"];
                creationInfo.Title = listName;
                creationInfo.Description = matter.MatterDescription;
                // Added list property for URL consolidation changes 
                creationInfo.Url = Constants.TitleListPath + matter.MatterGuid + ConfigurationManager.AppSettings["TaskListSuffix"];
                creationInfo.TemplateType = (int)ListTemplateType.Tasks;
                List list = web.Lists.Add(creationInfo);
                list.ContentTypesEnabled = false;
                list.Update();
                clientContext.ExecuteQuery();
                BreakPermission(clientContext, listName, matter.CopyPermissionsFromParent);
            }
            catch (Exception exception)
            {
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }
    }
}