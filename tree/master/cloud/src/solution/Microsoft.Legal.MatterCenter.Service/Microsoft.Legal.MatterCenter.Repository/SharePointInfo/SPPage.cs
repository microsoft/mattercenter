

using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
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
        #endregion

        /// <summary>
        /// All the dependencies are injected 
        /// </summary>
        /// <param name="spoAuthorization"></param>
        /// <param name="generalSettings"></param>
        public SPPage(ISPOAuthorization spoAuthorization, IOptions<GeneralSettings> generalSettings, 
            IOptions<LogTables> logTables, ICustomLogger customLogger)
        {
            this.generalSettings = generalSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.logTables = logTables.Value;
            this.customLogger = customLogger;
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
                File clientFile = clientContext.Web.GetFileByServerRelativeUrl(pageUrl);
                if (IsFileExists(clientContext, pageUrl))
                {
                    clientFile.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }
    }
}
