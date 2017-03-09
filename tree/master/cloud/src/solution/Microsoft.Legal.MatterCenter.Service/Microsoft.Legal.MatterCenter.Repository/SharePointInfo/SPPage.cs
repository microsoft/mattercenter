

using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class SPPage : ISPPage
    {
        #region Properties
        private GeneralSettings generalSettings;
        private ISPOAuthorization spoAuthorization;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private ISPList spList;
        private MatterSettings matterSettings;
        #endregion

        /// <summary>
        /// All the dependencies are injected 
        /// </summary>
        /// <param name="spoAuthorization"></param>
        /// <param name="generalSettings"></param>
        public SPPage(ISPOAuthorization spoAuthorization, IOptions<GeneralSettings> generalSettings, 
            IOptions<LogTables> logTables, ICustomLogger customLogger, ISPList spList, IOptions<MatterSettings> matterSettings)
        {
            this.generalSettings = generalSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.logTables = logTables.Value;
            this.customLogger = customLogger;
            this.spList = spList;
            this.matterSettings = matterSettings.Value;
        }


        public int CreateWebPartPage(ClientContext clientContext, string pageName, string layout, string masterpagelistName, string listName, string pageTitle)
        {
            int response = -1;
            if (null != clientContext && !string.IsNullOrWhiteSpace(pageName) && !string.IsNullOrWhiteSpace(layout) && !string.IsNullOrWhiteSpace(masterpagelistName) && !string.IsNullOrWhiteSpace(listName))
            {
                try
                {
                    //// Find Default Layout from Master Page Gallery to create Web Part Page                

                    Web web = clientContext.Web;
                    ListItemCollection collection = spList.GetData(clientContext, masterpagelistName);
                    clientContext.Load(collection, listItemCollectionProperties => listItemCollectionProperties.Include(listItemProperties => listItemProperties.Id, listItemProperties => listItemProperties.DisplayName));
                    clientContext.ExecuteQuery();
                    ListItem fileName = null;
                    foreach (ListItem findLayout in collection)
                    {
                        if (findLayout.DisplayName.Equals(layout, StringComparison.OrdinalIgnoreCase))
                        {
                            fileName = findLayout;
                            break;
                        }
                    }
                    FileCreationInformation objFileInfo = new FileCreationInformation();
                    objFileInfo.Url = pageName;
                    Microsoft.SharePoint.Client.File fileLayout = fileName.File;
                    clientContext.Load(fileLayout);
                    clientContext.ExecuteQuery();
                    ClientResult<Stream> filedata = fileLayout.OpenBinaryStream();
                    List sitePageLib = web.Lists.GetByTitle(listName);
                    clientContext.Load(sitePageLib);
                    clientContext.ExecuteQuery();
                    StreamReader reader = new StreamReader(filedata.Value);
                    objFileInfo.Content = System.Text.Encoding.ASCII.GetBytes(reader.ReadToEnd());
                    Microsoft.SharePoint.Client.File matterLandingPage = sitePageLib.RootFolder.Files.Add(objFileInfo);
                    ListItem matterLandingPageDetails = matterLandingPage.ListItemAllFields;
                    // Update the title of the page
                    matterLandingPageDetails[ServiceConstants.TITLE] = pageTitle;
                    matterLandingPageDetails.Update();
                    clientContext.Load(matterLandingPageDetails, matterLandingPageProperties => matterLandingPageProperties[ServiceConstants.TITLE], matterLandingPageProperties => matterLandingPageProperties.Id);
                    clientContext.ExecuteQuery();
                    response = matterLandingPageDetails.Id;
                }
                catch (Exception)
                {
                    response = -1;
                }
            }
            return response;
        }

        /// <summary>
        /// Adds all web parts on matter landing page.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="limitedWebPartManager">LimitedWebPartManager object to import web parts</param>
        /// <param name="webPartDefinition">WebPartDefinition object to add web parts on page</param>
        /// <param name="webParts">Array of web parts that should be added on Matter Landing Page</param>
        /// <param name="zones">Array of Zone IDs</param>
        /// <returns>Success flag</returns>
        public bool AddWebPart(ClientContext clientContext, LimitedWebPartManager limitedWebPartManager, WebPartDefinition webPartDefinition, 
            string[] webParts, string[] zones)
        {
            bool result = false;
            if (null != clientContext && null != limitedWebPartManager && null != webParts && null != zones)
            {
                int index = 0;
                try
                {
                    for (index = 0; index < webParts.Length; index++)
                    {
                        if (!string.IsNullOrWhiteSpace(webParts[index]))
                        {
                            webPartDefinition = limitedWebPartManager.ImportWebPart(webParts[index]);
                            limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, zones[index], ServiceConstants.ZONE_INDEX);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Configures XML of web parts.
        /// </summary>
        /// <param name="requestObject">Request Object</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="uri">To get URL segments</param>
        /// <param name="web">Web object of the current context</param>
        /// <returns>List of Web Parts</returns>
        public string[] ConfigureXMLCodeOfWebParts(Client client, Matter matter, ClientContext clientContext, string pageName, Uri uri, 
            Web web, MatterConfigurations matterConfigurations)
        {
            //injecting matterextraproperties as string while creating matter landing page.
            string matterExtraPropertiesValues = string.Empty;
            if (matterConfigurations != null && matterConfigurations.AdditionalFieldValues != null)
            {
                matterExtraPropertiesValues = DisplayAllExtraMatterProperties(matterConfigurations.AdditionalFieldValues);
            }
            string[] result = null;
            try
            {
                List sitePageLib = web.Lists.GetByTitle(matter.Name);
                clientContext.Load(sitePageLib);
                clientContext.ExecuteQuery();

                ////Configure list View Web Part XML
                string listViewWebPart = ConfigureListViewWebPart(sitePageLib, clientContext, pageName, client, matter, 
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, ServiceConstants.FORWARD_SLASH, matter.Name, 
                    ServiceConstants.FORWARD_SLASH, pageName));
                string[] contentEditorSectionIds = matterSettings.MatterLandingPageSections.Split(Convert.ToChar(ServiceConstants.COMMA, CultureInfo.InvariantCulture));

                ////Configure content Editor Web Part of user information XML
                string contentEditorWebPartTasks = string.Empty;
                if (matterConfigurations.IsTaskSelected)
                {
                    contentEditorWebPartTasks = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, 
                        string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, 
                        contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.TaskPanel, CultureInfo.InvariantCulture)]));
                }

                string calendarWebpart = string.Empty, rssFeedWebPart = string.Empty, rssTitleWebPart = string.Empty;
                if (matterConfigurations.IsRSSSelected)
                {
                    rssFeedWebPart = string.Format(CultureInfo.InvariantCulture, ServiceConstants.RSS_FEED_WEB_PART, WebUtility.UrlEncode(matter.Name));
                    rssTitleWebPart = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, 
                        string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, 
                        contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.RSSTitlePanel, CultureInfo.InvariantCulture)]));
                }

                ////Configure calendar Web Part XML
                if (matterConfigurations.IsCalendarSelected)
                {
                    ////If create calendar is enabled configure calendar Web Part XML; else dont configure
                    calendarWebpart = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.CalendarPanel, CultureInfo.InvariantCulture)]));
                }

                string matterInformationSection = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.InformationPanel, CultureInfo.InvariantCulture)]));
                string cssLink = string.Format(CultureInfo.InvariantCulture, matterSettings.MatterLandingCSSFileName, matterSettings.MatterLandingFolderName);
                string commonCssLink = string.Format(CultureInfo.InvariantCulture, matterSettings.CommonCSSFileLink, matterSettings.CommonFolderName);
                string jsLinkMatterLandingPage = string.Format(CultureInfo.InvariantCulture, matterSettings.MatterLandingJSFileName, matterSettings.MatterLandingFolderName);
                string jsLinkJQuery = string.Format(CultureInfo.InvariantCulture, matterSettings.JQueryFileName, matterSettings.CommonFolderName);
                string jsLinkCommon = string.Format(CultureInfo.InvariantCulture, matterSettings.CommonJSFileLink, matterSettings.CommonFolderName);
                string headerWebPartSection = string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.HeaderPanel, CultureInfo.InvariantCulture)]);
                string footerWebPartSection = string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.FooterPanel, CultureInfo.InvariantCulture)]);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, ServiceConstants.STYLE_TAG, cssLink), headerWebPartSection);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, ServiceConstants.STYLE_TAG, commonCssLink), headerWebPartSection);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, ServiceConstants.SCRIPT_TAG_WITH_CONTENTS, string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_STAMP_PROPERTIES, matter.Name, matter.MatterGuid, matterExtraPropertiesValues)), headerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, ServiceConstants.SCRIPT_TAG, jsLinkMatterLandingPage), footerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, ServiceConstants.SCRIPT_TAG, jsLinkCommon), footerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, ServiceConstants.SCRIPT_TAG, jsLinkJQuery), footerWebPartSection);
                string headerWebPart = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, headerWebPartSection);
                string footerWebPart = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, footerWebPartSection);
                string oneNoteWebPart = string.Format(CultureInfo.InvariantCulture, ServiceConstants.CONTENT_EDITOR_WEB_PART, string.Format(CultureInfo.InvariantCulture, ServiceConstants.MATTER_LANDING_SECTION_CONTENT, contentEditorSectionIds[Convert.ToInt32(MatterLandingSection.OneNotePanel, CultureInfo.InvariantCulture)]));
                string[] webParts = { headerWebPart, matterInformationSection, oneNoteWebPart, listViewWebPart, rssFeedWebPart, rssTitleWebPart, footerWebPart, calendarWebpart, contentEditorWebPartTasks };
                result = webParts;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Configures XML code of List View web part.
        /// </summary>
        /// <param name="requestObject">Request Object</param>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="titleUrl">Segment of URL</param>
        /// <returns>Configured ListView Web Part</returns>
        internal string ConfigureListViewWebPart(List sitePageLib, ClientContext clientContext, string pageName, Client client, Matter matter, string titleUrl)
        {
            string viewName = string.Empty;
            string result = string.Empty;
            try
            {
                Uri uri = new Uri(client.Url);
                ViewCollection viewColl = sitePageLib.Views;
                clientContext.Load(
                    viewColl,
                    views => views.Include(
                        view => view.Title,
                        view => view.Id));
                clientContext.ExecuteQuery();
                foreach (View view in viewColl)
                {
                    viewName = Convert.ToString(view.Id, CultureInfo.InvariantCulture);
                    break;
                }
                viewName = string.Concat(ServiceConstants.OPENING_CURLY_BRACE, viewName, ServiceConstants.CLOSING_CURLY_BRACE);
                string listViewWebPart = ServiceConstants.LIST_VIEW_WEBPART;
                listViewWebPart = string.Format(CultureInfo.InvariantCulture, listViewWebPart,
                    Convert.ToString(sitePageLib.Id, CultureInfo.InvariantCulture), titleUrl,
                    string.Concat(ServiceConstants.OPENING_CURLY_BRACE, Convert.ToString(sitePageLib.Id, CultureInfo.InvariantCulture),
                    ServiceConstants.CLOSING_CURLY_BRACE), viewName, string.Concat(uri.AbsolutePath, ServiceConstants.FORWARD_SLASH,
                    matter.Name, ServiceConstants.FORWARD_SLASH, pageName));
                result = listViewWebPart;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Will check whether a url exists in the current site collection or not
        /// </summary>
        /// <param name="client">Contains the url in which we need to check whether a page exists or not</param>
        /// <param name="pageUrl">The page</param>
        /// <returns></returns>
        public bool UrlExists(Client client, string pageUrl)
        {
            bool pageExists = false;
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    string[] requestedUrls = pageUrl.Split(new string[] { ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    if (1 < requestedUrls.Length)
                    {
                        foreach (string url in requestedUrls)
                        {
                            if (IsFileExists(clientContext, url))
                            {
                                pageExists = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        pageExists = IsFileExists(clientContext, pageUrl) ? true : false;
                    }
                }
                return pageExists;
            }            
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Checks the file at the specified location and return the file existence status.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="pageUrl">File URL</param>
        /// <returns>Success flag</returns>
        public bool IsFileExists(ClientContext clientContext, string pageUrl)
        {
            bool success = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(pageUrl))
            {
                Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(pageUrl);
                clientContext.Load(clientFile, cf => cf.Exists);
                clientContext.ExecuteQuery();
                success = clientFile.Exists;
            }
            return success;
        }

        /// <summary>
        /// Checks if the requested page exists or not.
        /// </summary>
        /// <param name="requestedUrl">URL of the page, for which check is to be performed</param>
        /// <param name="clientContext">ClientContext for SharePoint</param>
        /// <returns>true or false string based upon the existence of the page, referred in requestedUrl</returns>
        public bool PageExists(string requestedUrl, ClientContext clientContext)
        {
            bool pageExists = false;
            try
            {
                string[] requestedUrls = requestedUrl.Split(new string[] { ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR }, StringSplitOptions.RemoveEmptyEntries);
                if (1 < requestedUrls.Length)
                {
                    foreach (string url in requestedUrls)
                    {
                        if (IsFileExists(clientContext, url))
                        {
                            pageExists = true;
                            break;
                        }
                    }
                }
                else
                {
                    pageExists = IsFileExists(clientContext, requestedUrl) ? true : false;
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return pageExists;
        }

        /// <summary>
        /// Deletes the page
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="pageUrl">Page URL</param>
        public void Delete(ClientContext clientContext, string pageUrl)
        {
            if (null != clientContext && !string.IsNullOrWhiteSpace(pageUrl))
            {
                SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(pageUrl);
                if (IsFileExists(clientContext, pageUrl))
                {
                    clientFile.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// This method return string collection of matter extra properties which is used in matter landing page.
        /// </summary>
        /// <param name="objValues"></param>
        /// <returns></returns>
        private string DisplayAllExtraMatterProperties(IList<MatterExtraFields> objValues)
        {
            try
            {
                StringBuilder sbr = new StringBuilder(string.Empty);
                var totalCount = objValues.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    objValues[i].FieldValue = objValues[i].FieldValue == null ? "" : objValues[i].FieldValue;

                    if (objValues[i].IsDisplayInUI == "true")
                    {
                        string fieldVal = objValues[i].FieldValue.Trim();

                        if (objValues[i].Type.ToString().Trim().Equals("DateTime"))
                        {
                            DateTime date;
                            bool isDateTime = DateTime.TryParse(fieldVal, out date);
                            if (isDateTime)
                            {
                                fieldVal = String.Format("{0:MMMM yyyy, d}", date);
                            }
                            else
                            {
                                fieldVal = String.Format("{0:MMMM yyyy, d}", DateTime.Now);
                            }
                        }
                        string concatedString = i == totalCount - 1 ? objValues[i].FieldDisplayName + ":" + fieldVal : objValues[i].FieldDisplayName + ":" + fieldVal + "|";
                        sbr.AppendLine(concatedString.Trim());
                    }
                }
                string strReturnValue = sbr.ToString();
                strReturnValue = strReturnValue.Replace("\r\n", "");
                return strReturnValue;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }
}
