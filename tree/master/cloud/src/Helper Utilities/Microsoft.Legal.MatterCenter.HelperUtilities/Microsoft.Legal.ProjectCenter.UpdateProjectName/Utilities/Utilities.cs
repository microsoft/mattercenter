using Microsoft.Extensions.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client.WebParts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    public class Utilities
    {
        /// <summary>
        /// This method will return the secure password for authentication to SharePoint Online
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
        public static SecureString GetEncryptedPassword(string plainTextPassword)
        {
            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            foreach (char c in plainTextPassword)
            {
                securePassword.AppendChar(c);
            }            
            return securePassword;
        }


        /// <summary>
        /// Checking if the project name is valid
        /// </summary>
        /// <param name="projectNameToValidate"></param>
        /// <returns></returns>
        public static bool IsProjectNameValid(string projectNameToValidate)
        {
            bool projectNameValidation = true;
            var projectName = Regex.Match(projectNameToValidate, Constants.SpecialCharacterExpressionProjectTitle, RegexOptions.IgnoreCase);
            if (string.IsNullOrWhiteSpace(projectNameToValidate) || Constants.ProjectNameLength < projectNameToValidate.Length || projectNameToValidate.Length != projectName.Length)
            {
                projectNameValidation = false;
            }
            return projectNameValidation;
        }



        /// <summary>
        /// Get all the lists associated with the matter name
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="matterName"></param>
        /// <returns></returns>
        public static List<string> MatterAssociatedLists(ClientContext clientContext, string matterName)
        {
            return SpList.MatterAssociatedLists(clientContext, matterName);
        }


        /// <summary>
        /// Function to update list name as per new project name
        /// </summary>
        /// <param name="siteContext">Client context</param>
        /// <param name="oldName">Old project name</param>
        /// <param name="newName">New project name</param>
        public static void UpdateName(ClientContext siteContext, string oldName, string newName)
        {
            try
            {
                List list = siteContext.Web.Lists.GetByTitle(oldName);
                list.Title = newName;
                list.Update();
                siteContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                int lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                //Utility.LogErrorInAzure(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, lineNumber);
                throw;
            }
        }


        /// <summary>
        /// Function to update project OneNote name
        /// </summary>
        /// <param name="siteContext">Client context</param>
        /// <param name="OneNoteURL">Project OneNote URL</param>
        /// <param name="newName">New project name</param>
        public static void UpdateOneNoteFolder(ClientContext siteContext, string GUID, string oldName, string newName)
        {
            try
            {
                string OneNoteURL = GUID + "_OneNote" + Constants.FRONTSLASH + oldName;
                Folder folder = siteContext.Web.GetFolderByServerRelativeUrl(OneNoteURL);
                siteContext.Load(folder);
                siteContext.ExecuteQuery();
                ListItem folderItem = folder.ListItemAllFields;
                folderItem["Title"] = newName;
                folderItem["FileLeafRef"] = newName;
                folderItem.Update();
                siteContext.Load(folderItem);
                siteContext.ExecuteQuery();                
            }
            catch (ServerException exception)
            {
                if (exception.ServerErrorTypeName == Constants.FILENOTFOUNDEXCEPTION)
                {
                    try
                    {
                        string OneNoteURL = GUID + "_OneNote" + Constants.FRONTSLASH + GUID;
                        Folder folder = siteContext.Web.GetFolderByServerRelativeUrl(OneNoteURL);
                        siteContext.Load(folder);
                        siteContext.ExecuteQuery();
                        ListItem folderItem = folder.ListItemAllFields;
                        folderItem["Title"] = newName;
                        folderItem["FileLeafRef"] = newName;
                        folderItem.Update();
                        siteContext.Load(folderItem);
                        siteContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Function to update project file properties as per updated project name
        /// </summary>
        /// <param name="siteContext">SharePoint client context</param>
        /// <param name="oldName">Old project name</param>
        /// <param name="newName">New project name</param>
        public static void UpdateProjectFolders(ClientContext siteContext, string oldName, string newName, string documentBatchSize, IConfigurationRoot configuration)
        {
            try
            {
                List docList = siteContext.Web.Lists.GetByTitle(oldName);
                siteContext.Load(docList);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View Scope='RecursiveAll'></View>";
                ListItemCollection listItemCol = docList.GetItems(camlQuery);

                siteContext.Load(listItemCol);
                siteContext.ExecuteQuery();

                int batchCount = 0, batchSize = Convert.ToInt32(documentBatchSize);

                batchSize = (0 < batchSize) ? batchSize : 1;

                foreach (ListItem item in listItemCol)
                {
                    if (Convert.ToString(item.FileSystemObjectType).Equals("File", StringComparison.OrdinalIgnoreCase))
                    {
                        item[configuration.GetSection("General")["DocumentMatterNameProperty"]] = newName;
                        item.Update();
                    }

                    // Execute operation is performed in batch with specified batch size
                    batchCount++;
                    if (0 == (batchCount % batchSize))
                    {
                        siteContext.Load(listItemCol);
                        siteContext.ExecuteQuery();
                        
                    }
                }

                // Update pending document properties
                siteContext.Load(listItemCol);
                siteContext.ExecuteQuery();
                
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function to update project landing page as per new project name
        /// </summary>
        /// <param name="siteContext">Client context</param>
        /// <param name="projectToUpdate">Details of project to be updated</param>
        /// <param name="guid">Project GUID</param>
        /// <param name="calendarStatus">If Calendar is created</param>
        /// <param name="taskStatus">If task is created</param>
        /// <param name="rssFeedStatus">If RSS feed is created</param>
        public static void UpdateProjectLandingPage(ClientContext siteContext, Project projectToUpdate, string guid, 
            bool calendarStatus, bool taskStatus, bool rssFeedStatus, IConfigurationRoot configuration)
        {
            int listID = RetrieveItemId(siteContext, Constants.SITE, guid);
            try
            {
                if (-1 != listID)
                {
                    List list = siteContext.Web.Lists.GetByTitle(Constants.SITE);
                    ListItem listItem = list.GetItemById(listID);
                    siteContext.Load(listItem);
                    Microsoft.SharePoint.Client.File file = listItem.File;
                    listItem["Title"] = projectToUpdate.NewProjectName;
                    listItem.Update();

                    /// Update stamped project and project GUID on project landing page
                    LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                    siteContext.Load(wpm.WebParts, wps => wps.Include(wp => wp.WebPart.Properties, wp => wp.WebPart.Title));
                    siteContext.ExecuteQuery();

                    WebPartDefinitionCollection oWebPartDefinition = wpm.WebParts;
                    siteContext.Load(oWebPartDefinition, item => item.Include(li => li.ZoneId, li => li.WebPart, li => li.WebPart.Properties));
                    siteContext.ExecuteQuery();

                    foreach (WebPartDefinition zone in oWebPartDefinition)
                    {
                        if (zone.WebPart.Title.ToLower()=="rss viewer")
                        {
                            rssFeedStatus = true;
                            break;
                        }
                    }

                    foreach (WebPartDefinition zone in oWebPartDefinition)
                    {
                        if (("HeaderZone").Equals(zone.ZoneId))
                        {
                            zone.DeleteWebPart();
                            break;
                        }
                    }
                    string catalogAssetsPathStyles = configuration.GetSection("General")["CatalogAssetsStyles"];
                    string sharePointDLLVersion = configuration.GetSection("General")["SharePointDLLVersion"];
                    string matterLandingcss = configuration.GetSection("General")["ProjectLandingCss"];
                    string projectLandingAssets = configuration.GetSection("General")["ProjectLandingAssets"];
                    string projectCenterAssets = configuration.GetSection("General")["ProjectCenterAssets"];

                    string projectLandingSectionContent = "<div id=\"{0}\"></div>";
                    string styleTag = "<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />";
                    string cssLink = string.Format(CultureInfo.InvariantCulture, catalogAssetsPathStyles + matterLandingcss, projectCenterAssets + Constants.FRONTSLASH + projectLandingAssets);
                    string commonCssLink = string.Format(CultureInfo.InvariantCulture, catalogAssetsPathStyles + Constants.COMMONCSS, projectCenterAssets + Constants.FRONTSLASH + Constants.COMMONASSETS);
                    string headerWebPartSection = string.Format(CultureInfo.InvariantCulture, projectLandingSectionContent, Constants.MATTERCENTERHEADER);
                    string projectLandingStampProperties = "var documentLibraryName = \"{0}\", isNewProjectLandingPage = true, documentLibraryGUID=\"{1}\";";
                    string scriptTagWithContents = "<script type=\"text/javascript\">{0}</script>";
                    headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, styleTag, cssLink), headerWebPartSection);
                    headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, styleTag, commonCssLink), headerWebPartSection);
                    headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, scriptTagWithContents, string.Format(CultureInfo.InvariantCulture, projectLandingStampProperties, projectToUpdate.NewProjectName, guid)), headerWebPartSection);

                    string headerWebPart = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, headerWebPartSection, sharePointDLLVersion);
                    WebPartDefinition webPartDefinition = null;
                    if (!string.IsNullOrWhiteSpace(headerWebPart))
                    {
                        LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                        webPartDefinition = limitedWebPartManager.ImportWebPart(headerWebPart);
                        limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, "HeaderZone", 1);
                        siteContext.ExecuteQuery();
                    }

                    // Remove existing web-parts from page and recreate
                    List projectList = siteContext.Web.Lists.GetByTitle(projectToUpdate.NewProjectName);
                    siteContext.Load(projectList, lst => lst.Title, lst => lst.EntityTypeName);
                    siteContext.ExecuteQuery();
                    DeleteProjectWebparts(siteContext, projectList);

                    CreateOneNote(siteContext, projectList, projectToUpdate.NewProjectName, configuration);
                    if (taskStatus)
                    {
                        CreateWebPart(siteContext, projectList, projectToUpdate.NewProjectName, Constants.TASK, Constants.TASKPANE, 2, configuration);
                    }
                    if (calendarStatus)
                    {
                        CreateWebPart(siteContext, projectList, projectToUpdate.NewProjectName, Constants.CALENDAR, Constants.CALENDARPANE, 3, configuration);
                    }
                    if (rssFeedStatus)
                    {
                        CreateWebPart(siteContext, projectList, projectToUpdate.NewProjectName, Constants.RSSTITLE, Constants.RSSPANE, 4, configuration);
                        CreateWebPart(siteContext, projectList, projectToUpdate.NewProjectName, Constants.RSSFEED, Constants.RSSPANE, 5, configuration);
                    }
                }
                else
                {
                    //frmMain.status.UpdateStatus("Project landing page not found with GUID: " + projectToUpdate.ProjectGUID, MatterCenter.Common.Message.MessageType.Error);
                }
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        /// <summary>
        /// This Method delete right zone from project landing page
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="projectList">Project properties</param>
        private static void DeleteProjectWebparts(ClientContext clientContext, List projectList)
        {
            string folderGuid = projectList.EntityTypeName;
            int pageId = RetrieveItemId(clientContext, Constants.SITEPAGES, folderGuid);
            if (-1 != pageId)
            {
                List list = clientContext.Web.Lists.GetByTitle(Constants.SITE);
                ListItem listItem = list.GetItemById(pageId);
                clientContext.Load(list);
                clientContext.Load(listItem, item => item.File);

                File file = listItem.File;
                clientContext.Load(file);

                LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                clientContext.Load(limitedWebPartManager.WebParts, wps => wps.Include(wp => wp.WebPart.Properties, wp => wp.WebPart.Title));

                WebPartDefinitionCollection webPartDefinition = limitedWebPartManager.WebParts;
                clientContext.Load(webPartDefinition, item => item.Include(li => li.ZoneId));
                clientContext.ExecuteQuery();

                var rightZoneWebParts = webPartDefinition.Where(webpart => webpart.ZoneId.Equals(Constants.RIGHTZONE));
                foreach (WebPartDefinition zone in rightZoneWebParts)
                {
                    zone.DeleteWebPart();
                }
                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Retrieve the page id
        /// </summary>
        /// <param name="clientContext">client context</param>
        /// <param name="libraryName">list name</param>
        /// <param name="pageName">GUID of list</param>
        /// <returns>Retrieved page id</returns>
        private static int RetrieveItemId(ClientContext clientContext, string libraryName, string pageName)
        {
            int listItemId = -1;
            if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName) && !string.IsNullOrWhiteSpace(pageName))
            {
                ListItemCollection listItemCollection = GetData(clientContext, libraryName);
                clientContext.Load(listItemCollection, listItemCollectionProperties => listItemCollectionProperties.Include(listItemProperties => listItemProperties.Id, listItemProperties => listItemProperties.DisplayName));
                clientContext.ExecuteQuery();

                ListItem listItem = listItemCollection.Cast<ListItem>().FirstOrDefault(listItemProperties => listItemProperties.DisplayName.ToUpper(CultureInfo.InvariantCulture).Equals(pageName.ToUpper(CultureInfo.InvariantCulture)));

                if (null != listItem)
                {
                    listItemId = listItem.Id;
                }
            }

            return listItemId;
        }

        /// <summary>
        /// Retrieve the data
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="listName">list name</param>
        /// <param name="camlQuery">CAML query</param>
        /// <returns>List of items</returns>
        private static ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null)
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

        /// <summary>
        /// This will set create the web part
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="proj">Project properties</param>
        /// <param name="projectName">Project name</param>
        /// <param name="listName">List name</param>
        /// <param name="listPaneName">ListPane Name</param>
        /// <param name="zoneIndex">Zone Index</param>
        private static void CreateWebPart(ClientContext clientContext, List proj, string projectName, string listname, 
            string listPaneName, int zoneIndex, IConfigurationRoot configuration)
        {
            string folderGuid = proj.EntityTypeName, ContentEditorWebPart, contentEditorWebPartTasks, spDLLVersion;
            spDLLVersion = configuration.GetSection("General")["SharePointDLLVersion"];

            int pageId = RetrieveItemId(clientContext, Constants.SITEPAGES, folderGuid);
            if (-1 != pageId)
            {
                if (listname.Equals(Constants.RSSFEED))
                    ContentEditorWebPart = Constants.RssFeedWebpart;
                else
                    ContentEditorWebPart = Constants.ContentEditorWebPart;

                List list = clientContext.Web.Lists.GetByTitle(Constants.SITE);
                ListItem listItem = list.GetItemById(pageId);
                clientContext.Load(list);
                clientContext.Load(listItem, i => i.File);

                File file = listItem.File;
                clientContext.Load(file);
                string ProjectLandingSectionContent = Constants.SECTIONCONTENT;

                if (listname.Equals(Constants.RSSFEED))
                    contentEditorWebPartTasks = string.Format(CultureInfo.InvariantCulture, ContentEditorWebPart, 
                        HttpUtility.UrlPathEncode(projectName,false), spDLLVersion);
                else
                    contentEditorWebPartTasks = string.Format(CultureInfo.InvariantCulture, ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, ProjectLandingSectionContent, listPaneName), spDLLVersion);

                WebPartDefinition webPartDefinition = null;
                LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                webPartDefinition = limitedWebPartManager.ImportWebPart(contentEditorWebPartTasks);
                limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, Constants.RIGHTZONE, zoneIndex);
                clientContext.ExecuteQuery();                
            }
        }

        /// <summary>
        /// This Method create the OneNote in the project landing page
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="proj">Project properties</param>
        /// <param name="projectName">Project name</param>
        /// <param name="zoneIndex">Zone index</param>
        private static void CreateOneNote(ClientContext clientContext, List proj, string projectName, 
            IConfigurationRoot configuration, int zoneIndex = 1)
        {
            CreateWebPart(clientContext, proj, projectName, Constants.ONENOTE, Constants.ONENOTEPANE, zoneIndex, configuration);
        }


        /// <summary>
        /// Function to update pinned documents information
        /// </summary>
        /// <param name="siteContext">Client context</param>
        /// <param name="projectId">Project Id</param>
        /// <param name="newName">New project name</param>
        public static void UpdatePinnedDocument(ClientContext siteContext, string projectId, string newName, IConfigurationRoot configuration)
        {           

            string pinListTitle = configuration.GetSection("General")["DocumentPinList"];
            string pinListColumn = configuration.GetSection("General")["DocumentPinListColumn"];
            try
            {
                List list = siteContext.Web.Lists.GetByTitle(pinListTitle);
                bool isValueMatched = false;
                CamlQuery camlQuery = new CamlQuery();
                ListItemCollection itemCollection = list.GetItems(CamlQuery.CreateAllItemsQuery());
                siteContext.Load(itemCollection);
                siteContext.ExecuteQuery();
                foreach (ListItem item in itemCollection)
                {
                    string rawData = Convert.ToString(item[pinListColumn]);
                    Dictionary<string, DocumentData> pinnedDocumentCollection = JsonConvert.DeserializeObject<Dictionary<string, DocumentData>>(rawData);
                    foreach (DocumentData pinnedDocument in pinnedDocumentCollection.Values)
                    {
                        if (null != pinnedDocument.DocumentMatterId && pinnedDocument.DocumentMatterId.Equals(projectId, StringComparison.OrdinalIgnoreCase))
                        {
                            pinnedDocument.DocumentMatterName = HttpUtility.HtmlEncode(newName);
                            isValueMatched = true;
                        }
                    }
                    if (isValueMatched)
                    {
                        item[pinListColumn] = JsonConvert.SerializeObject(pinnedDocumentCollection, Newtonsoft.Json.Formatting.Indented);
                        item.Update();
                    }
                }
                siteContext.ExecuteQuery();
                
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function to update pinned project information
        /// </summary>
        /// <param name="siteContext">Client context</param>
        /// <param name="GUID">Project GUID</param>
        /// <param name="newName">New project name</param>
        public static void UpdatePinnedProject(ClientContext siteContext, string GUID, string newName, IConfigurationRoot configuration)
        {
            string pinListTitle = configuration.GetSection("General")["ProjectPinList"];
            string pinListColumn = configuration.GetSection("General")["ProjectPinListColumn"];

            try
            {
                List list = siteContext.Web.Lists.GetByTitle(pinListTitle);
                bool isValueMatched = false;
                CamlQuery camlQuery = new CamlQuery();
                ListItemCollection itemCollection = list.GetItems(CamlQuery.CreateAllItemsQuery());
                siteContext.Load(itemCollection);
                siteContext.ExecuteQuery();
                foreach (ListItem item in itemCollection)
                {
                    string rawData = Convert.ToString(item[pinListColumn]);
                    Dictionary<string, ProjectData> pinnedProjectCollection = JsonConvert.DeserializeObject<Dictionary<string, ProjectData>>(rawData);

                    if (null != pinnedProjectCollection)
                    {
                        foreach (ProjectData pinnedProject in pinnedProjectCollection.Values)
                        {
                            if (null != pinnedProject.MatterGuid && pinnedProject.MatterGuid.Equals(GUID, StringComparison.OrdinalIgnoreCase))
                            {
                                pinnedProject.MatterName = HttpUtility.HtmlEncode(newName);
                                isValueMatched = true;
                                break;
                            }
                        }
                        if (isValueMatched)
                        {
                            item[pinListColumn] = JsonConvert.SerializeObject(pinnedProjectCollection, Newtonsoft.Json.Formatting.Indented);
                            item.Update();
                        }
                    }
                }
                siteContext.ExecuteQuery();                
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
