using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Net;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http;
using Microsoft.Exchange.WebServices.Data;

using System.Reflection;
using System.Dynamic;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class DocumentProvision : IDocumentProvision
    {
        private IDocumentRepository docRepository;
        private IUploadHelperFunctions uploadHelperFunctions;
        private IUserRepository userRepository;
        private GeneralSettings generalSettings;
        private DocumentSettings documentSettings;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private ErrorSettings errorSettings;
        private SearchSettings searchSettings;
        private IConfigurationRoot configuration;
        private IHttpContextAccessor httpContextAccessor;
        public DocumentProvision(IDocumentRepository docRepository, 
            IUserRepository userRepository, 
            IUploadHelperFunctions uploadHelperFunctions, 
            IOptions<GeneralSettings> generalSettings, 
            IOptions<DocumentSettings> documentSettings, 
            ICustomLogger customLogger,
            IOptions<SearchSettings> searchSettings,
            IConfigurationRoot configuration,
            IHttpContextAccessor httpContextAccessor,
            IOptions<LogTables> logTables, IOptions<ErrorSettings> errorSettings)
        {
            this.docRepository = docRepository;
            this.uploadHelperFunctions = uploadHelperFunctions;
            this.userRepository = userRepository;
            this.generalSettings = generalSettings.Value;
            this.documentSettings = documentSettings.Value;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.errorSettings = errorSettings.Value;
            this.searchSettings = searchSettings.Value;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetAllCounts(SearchRequestVM searchRequestVM)
        {
            try
            {
                searchRequestVM.SearchObject.Filters.FilterByMe = 0;
                var searchObject = searchRequestVM.SearchObject;
                // Encode all fields which are coming from js
                SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
                // Encode Search Term
                searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                    WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES,
                    ServiceConstants.DOUBLE_QUOTE) : string.Empty;

                var searchResultsVM = await docRepository.GetDocumentsAsync(searchRequestVM);
                return searchResultsVM.TotalRows;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

        }

        public async Task<int> GetMyCounts(SearchRequestVM searchRequestVM)
        {
            try
            {
                searchRequestVM.SearchObject.Filters.FilterByMe = 1;
                var searchObject = searchRequestVM.SearchObject;
                // Encode all fields which are coming from js
                SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
                // Encode Search Term
                searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                    WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES,
                    ServiceConstants.DOUBLE_QUOTE) : string.Empty;

                var searchResultsVM = await docRepository.GetDocumentsAsync(searchRequestVM);
                return searchResultsVM.TotalRows;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public async Task<int> GetPinnedCounts(SearchRequestVM searchRequestVM)
        {
            try
            {
                var pinResponseVM = await docRepository.GetPinnedRecordsAsync(searchRequestVM);
                return pinResponseVM.TotalRows;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public Stream DownloadAttachments(MailAttachmentDetails mailAttachmentDetails)
        {
            
            ///// filename, stream
            Dictionary<string, Stream> collectionOfAttachments = new Dictionary<string, Stream>();
            ///// full URL, relative URL
            string[] allAttachmentUrl = mailAttachmentDetails.FullUrl.Split(';');

            bool attachmentFlag = mailAttachmentDetails.IsAttachmentCall;
            foreach (string attachmentUrl in allAttachmentUrl)
            {
                if (!string.IsNullOrWhiteSpace(attachmentUrl))
                {
                    string uniqueKeyWithDate = attachmentUrl.Split(Convert.ToChar(ServiceConstants.DOLLAR, CultureInfo.InvariantCulture))[1].Substring(attachmentUrl.Split(Convert.ToChar(ServiceConstants.DOLLAR, 
                        CultureInfo.InvariantCulture))[1].LastIndexOf(Convert.ToChar(ServiceConstants.BACKWARD_SLASH, CultureInfo.InvariantCulture)) + 1) + ServiceConstants.DOLLAR + Guid.NewGuid();
                    Stream fileStream = docRepository.DownloadAttachments(attachmentUrl);
                    collectionOfAttachments.Add(uniqueKeyWithDate, fileStream);
                }
            }
            return GenerateEmail(collectionOfAttachments, allAttachmentUrl, attachmentFlag);            
        }       


        private Stream GenerateEmail(Dictionary<string, Stream> collectionOfAttachments, string[] documentUrls, bool attachmentFlag)
        {
            Stream result = null;
            try
            {
                
                


                MemoryStream mailFile = GetMailAsStream(collectionOfAttachments, documentUrls, attachmentFlag);
                mailFile.Position = 0;
                
                result = mailFile;
            }
            catch(Exception ex)
            {

            }
            return result;
        }


        /// <summary>
        /// Forms the memory stream of the mail with attachments.
        /// </summary>
        /// <param name="collectionOfAttachments">Collection of attachments as dictionary</param>
        /// <returns>Memory stream of the created mail object</returns>
        internal MemoryStream GetMailAsStream(Dictionary<string, Stream> collectionOfAttachments, string[] documentUrls, bool attachmentFlag)
        {
            MemoryStream result = null;
            string documentUrl = string.Empty;
            try
            {
                // need to be able to update/configure or get current version of server
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                //// can use on premise exchange server credentials with service.UseDefaultCredentials = true, or explicitly specify the admin account (set default to false)
                service.Credentials = new WebCredentials(generalSettings.AdminUserName, generalSettings.AdminPassword);
                service.Url = new Uri(generalSettings.ExchangeServiceURL);
                Microsoft.Exchange.WebServices.Data.EmailMessage email = new Microsoft.Exchange.WebServices.Data.EmailMessage(service);
                email.Subject = documentSettings.MailCartMailSubject;

                if (attachmentFlag)
                {
                    email.Body = new MessageBody(documentSettings.MailCartMailBody);
                    foreach (KeyValuePair<string, Stream> mailAttachment in collectionOfAttachments)
                    {
                        if (null != mailAttachment.Value)
                        {
                            // Remove the date time string before adding the file as an attachment
                            email.Attachments.AddFileAttachment(mailAttachment.Key.Split('$')[0], mailAttachment.Value);
                        }
                    }
                }
                else
                {
                    int index = 0;
                    foreach (string currentURL in documentUrls)
                    {
                        if (null != currentURL && 0 < currentURL.Length)
                        {
                            string[] currentAssets = currentURL.Split('$');
                            string documentURL = generalSettings.SiteURL + currentAssets[1];
                            string documentName = currentAssets[2];

                            documentUrl = string.Concat(documentUrl, string.Format(CultureInfo.InvariantCulture, "'{0} ) {1} : <a href='{2}'>{2} </a><br/>" , ++index, documentName, documentURL));
                        }
                    }
                    documentUrl = string.Format(CultureInfo.InvariantCulture, "<div style='font-family:Calibri;font-size:12pt'>{0}</div>", documentUrl);
                    email.Body = new MessageBody(documentUrl);
                }
                //// This header allows us to open the .eml in compose mode in outlook
                email.SetExtendedProperty(new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders, "X-Unsent", MapiPropertyType.String), "1");
                email.Save(WellKnownFolderName.Drafts); // must save draft in order to get MimeContent
                email.Load(new PropertySet(EmailMessageSchema.MimeContent));
                MimeContent mimcon = email.MimeContent;
                //// Do not make the StylCop fixes for MemoryStream here
                MemoryStream fileContents = new MemoryStream();
                fileContents.Write(mimcon.Content, 0, mimcon.Content.Length);
                fileContents.Position = 0;
                result = fileContents;
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                MemoryStream fileContents = new MemoryStream();
                result = fileContents;
            }
            return result;
        }

        /// <summary>
        /// Gets the file content type based on specified extensions.
        /// </summary>
        /// <param name="fileExtension">Extension of the file</param>
        /// <returns>File content type</returns>
        public static string ReturnExtension(string fileExtension)
        {
            string result = string.Empty;
            switch (fileExtension)
            {
                case ".txt":
                    result = "text/plain";
                    break;
                case ".doc":
                    result = "application/ms-word";
                    break;
                case ".xls":
                    result = "application/vnd.ms-excel";
                    break;
                case ".gif":
                    result = "image/gif";
                    break;
                case ".jpg":
                case "jpeg":
                    result = "image/jpeg";
                    break;
                case ".bmp":
                    result = "image/bmp";
                    break;
                case ".wav":
                    result = "audio/wav";
                    break;
                case ".ppt":
                    result = "application/mspowerpoint";
                    break;
                case ".dwg":
                    result = "image/vnd.dwg";
                    break;
                default:
                    result = "application/octet-stream";
                    break;
            }
            return result;
        }

        public GenericResponseVM UploadAttachments(AttachmentRequestVM attachmentRequestVM)
        {
            int attachmentCount = 0;
            string message = string.Empty;
            var client = attachmentRequestVM.Client;
            var serviceRequest = attachmentRequestVM.ServiceRequest;            
            GenericResponseVM genericResponse = null;            
            foreach (AttachmentDetails attachment in serviceRequest.Attachments)
            {
                genericResponse = uploadHelperFunctions.Upload(client, serviceRequest, ServiceConstants.ATTACHMENT_SOAP_REQUEST, attachment.id, false,
                    attachment.name, serviceRequest.FolderPath[attachmentCount], false, ref message,
                    attachment.originalName);
                if (genericResponse!=null && genericResponse.IsError==true)
                {
                    //result = false;
                    break;
                }
                attachmentCount++;
            }            
            return genericResponse;
        }

        public GenericResponseVM UploadEmails(AttachmentRequestVM attachmentRequestVM)
        {
            string message = string.Empty;
            var client = attachmentRequestVM.Client;
            var serviceRequest = attachmentRequestVM.ServiceRequest;            
            GenericResponseVM genericResponse = uploadHelperFunctions.Upload(client, serviceRequest, ServiceConstants.MAIL_SOAP_REQUEST, serviceRequest.MailId, true,
                        serviceRequest.Subject, serviceRequest.FolderPath[0], true, ref message, string.Empty);            
            return genericResponse;
        }

        public GenericResponseVM CheckDuplicateDocument(string clientUrl, string folderName, string documentLibraryName, 
            string fileName, ContentCheckDetails contentCheck, bool allowContentCheck)
        {
            GenericResponseVM genericResponse = null;
            DuplicateDocument duplicateDocument = uploadHelperFunctions.DocumentExists(clientUrl, contentCheck, documentLibraryName, folderName, false);
            if (duplicateDocument != null && duplicateDocument.DocumentExists)
            {
                string documentPath = string.Concat(generalSettings.SiteURL, folderName, ServiceConstants.FORWARD_SLASH, fileName);
                string duplicateMessage = (allowContentCheck && duplicateDocument.HasPotentialDuplicate) ? errorSettings.FilePotentialDuplicateMessage : errorSettings.FileAlreadyExistMessage;
                duplicateMessage = $"{duplicateMessage}|{duplicateDocument.HasPotentialDuplicate}";
                genericResponse = new GenericResponseVM()
                {
                    IsError = true,
                    Code = UploadEnums.DuplicateDocument.ToString(),
                    Value = string.Format(CultureInfo.InvariantCulture, duplicateMessage, fileName, documentPath)
                };

            }
            return genericResponse;
        }

        public GenericResponseVM PerformContentCheck(string clientUrl, string folderUrl, IFormFile uploadedFile, string fileName)
        {
            GenericResponseVM genericResponse = null;
            genericResponse = uploadHelperFunctions.PerformContentCheck(clientUrl, folderUrl, uploadedFile, fileName);            
            return genericResponse;
        }

        public async Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM)
        {
            var searchObject = searchRequestVM.SearchObject;
            // Encode all fields which are coming from js
            SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
            // Encode Search Term
            searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES, ServiceConstants.DOUBLE_QUOTE) : string.Empty;

            var searchResultsVM = await docRepository.GetDocumentsAsync(searchRequestVM);

            if (searchResultsVM.TotalRows > 0)
            {                
                dynamic documentDataList = new List<dynamic>();
                IEnumerable<IDictionary<string, object>> searchResults = searchResultsVM.SearchResults;
                foreach (var searchResult in searchResults)
                {
                    
                    dynamic documentData = new ExpandoObject();
                    foreach (var key in searchResult.Keys)
                    {
                        documentData.Checker = false;
                        ServiceUtility.AddProperty(documentData, "Checker", false);
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDocumentClientName.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentClient").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyFileName.ToString().ToLower())
                        {

                            string fileNameWithOutExt = System.IO.Path.GetFileNameWithoutExtension(searchResult[key].ToString());
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentName").Key,
                                fileNameWithOutExt);
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDocumentClientId.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentClientId").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertySiteName.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentClientUrl").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDocumentVersion.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentVersion").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDocumentMatterName.ToString().ToLower())
                        {
                            if(searchResult[key].ToString()!=string.Empty)
                            {
                                ServiceUtility.AddProperty(documentData,
                                    configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentMatterName").Key,
                                    searchResult[key].ToString());
                            }
                            else
                            {
                                ServiceUtility.AddProperty(documentData,
                                    configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentMatterName").Key,
                                    searchResult["Title"].ToString());
                            }
                        }                        
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDocumentMatterId.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentMatterId").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDocumentCheckOutUser.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentCheckoutUser").Key,
                                searchResult[key].ToString());
                        }
                        //-------------------------
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyCreated.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentCreatedDate").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyFileExtension.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentExtension").Key,
                                searchResult[key].ToString());
                            if (searchResult[key].ToString().ToLower() == "csv")
                            {
                                ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentIconUrl").Key,
                                $"{generalSettings.SiteURL}/_layouts/15/images/generaldocument.png");
                            }
                            else if (searchResult[key].ToString().ToLower() != "pdf")
                            {
                                ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentIconUrl").Key,
                                $"{generalSettings.SiteURL}/_layouts/15/images/ic{searchResult[key].ToString().ToLower()}.gif");
                            }
                            else
                            {
                                ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentIconUrl").Key,
                                $"{generalSettings.SiteURL}/_layouts/15/images/ic{searchResult[key].ToString().ToLower()}.png");
                            }
                        }
                        
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyPath.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentOWAUrl").Key,
                                searchResult[key].ToString());
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentUrl").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == "serverredirectedurl")
                        {
                            if (searchResult[key] != null)
                            {
                                ServiceUtility.AddProperty(documentData,
                                    configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentOWAUrl").Key,
                                    searchResult[key].ToString());
                                ServiceUtility.AddProperty(documentData,
                                    configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentUrl").Key,
                                    searchResult[key].ToString());
                            }                                
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyLastModifiedTime.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentModifiedDate").Key,
                                searchResult[key].ToString());                            
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyAuthor.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentOwner").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == "docid")
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("docId").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyPracticeGroup.ToString().ToLower())
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentPracticeGroup").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == "parentlink")
                        {
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentParentUrl").Key,
                                searchResult[key].ToString());
                            string documentUrl = searchResult[key].ToString().Substring(0, searchResult[key].ToString().LastIndexOf("/"));
                            string siteUrl = searchResult["SiteName"].ToString();
                            string matterGuid = searchResult[key].ToString().ToLower().Replace(siteUrl, "").Split('/')[1];
                            string matterUrl = $"{siteUrl}/sitepages/{matterGuid}.aspx";
                            ServiceUtility.AddProperty(documentData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument").GetSection("documentMatterUrl").Key,
                                matterUrl);
                        }  
                        ServiceUtility.AddProperty(documentData, "PinType", "Pin");
                        ServiceUtility.AddProperty(documentData, "DocGuid", Guid.NewGuid().ToString());


                    }
                    documentDataList.Add(documentData);
                }
                searchResultsVM.DocumentDataList = documentDataList;
                
            }
            searchResultsVM.SearchResults = null;
            if (searchRequestVM.SearchObject.IsUnique && searchResultsVM.DocumentDataList != null && !string.IsNullOrWhiteSpace(searchRequestVM.SearchObject.UniqueColumnName))
            {
                searchResultsVM.DocumentDataList = getUniqueResults(searchRequestVM, searchResultsVM);
            }
            return searchResultsVM;
        }

        /// <summary>
        /// getting unique results for this.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <param name="searchResultsVM"></param>
        /// <returns></returns>
        public dynamic getUniqueResults(SearchRequestVM searchRequestVM, dynamic searchResultsVM)
        {
           dynamic documentDataList1 = new List<dynamic>();

           var colList = configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument");

           string UniqueColumnName = getuniqueColumnName(searchRequestVM.SearchObject.UniqueColumnName.ToLower().Trim());
            if (!string.IsNullOrWhiteSpace(UniqueColumnName))
            {
                if (UniqueColumnName.Equals(colList.GetSection("documentMatterName").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.DocumentDataList).Where(d => d.documentMatterName.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.documentMatterName).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic documentData1 = new ExpandoObject();
                        documentData1.documentMatterName = dt;
                        documentDataList1.Add(documentData1);
                    }
                    searchResultsVM.DocumentDataList = documentDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("documentPracticeGroup").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.DocumentDataList).Where(d => d.documentPracticeGroup.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.documentPracticeGroup).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic documentData1 = new ExpandoObject();
                        documentData1.documentPracticeGroup = dt;
                        documentDataList1.Add(documentData1);
                    }
                    searchResultsVM.DocumentDataList = documentDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("documentOwner").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.DocumentDataList).Where(d => d.documentOwner.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.documentOwner).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic documentData1 = new ExpandoObject();
                        documentData1.documentOwner = dt;
                        documentDataList1.Add(documentData1);
                    }
                    searchResultsVM.DocumentDataList = documentDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("documentCheckoutUser").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.DocumentDataList).Where(d => d.documentCheckoutUser.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.documentCheckoutUser).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic documentData1 = new ExpandoObject();
                        documentData1.documentCheckoutUser = dt;
                        documentDataList1.Add(documentData1);
                    }
                    searchResultsVM.DocumentDataList = documentDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("documentClient").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.DocumentDataList).Where(d => d.documentClient.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.documentClient).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic documentData1 = new ExpandoObject();
                        documentData1.documentClient = dt;
                        documentDataList1.Add(documentData1);
                    }
                    searchResultsVM.DocumentDataList = documentDataList1;
                }
            }
        
            return searchResultsVM.DocumentDataList;
        }

        /// <summary>
        /// to get column name 
        /// </summary>
        /// <returns></returns>
        public string getuniqueColumnName(string uniueColumnName)
        {
            var docColumnSesction = configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForDocument");

            if (searchSettings.ManagedPropertyDocumentClientName.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("documentClient").Key;
            }
            else if (searchSettings.ManagedPropertyMatterName.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("documentMatterName").Key;
            }
            else if (searchSettings.ManagedPropertyAuthor.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("documentOwner").Key;
            }
            else if (searchSettings.ManagedPropertyPracticeGroup.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("documentPracticeGroup").Key;
            }
            else if (searchSettings.ManagedPropertyDocumentCheckOutUser.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("documentCheckoutUser").Key;
            }
            else
            {
                uniueColumnName = string.Empty;
            }

            return uniueColumnName;
        }

        /// <summary>
        /// get the documents async
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<SearchResponseVM> GetPinnedDocumentsAsync(SearchRequestVM searchRequestVM)
        {         

            var searchResultsVM = await docRepository.GetPinnedRecordsAsync(searchRequestVM);    
            return searchResultsVM;
        }

        public GenericResponseVM UploadFiles(IFormFile uploadedFile, string fileExtension, string originalName, 
            string folderName, string fileName, string clientUrl, string folder, string documentLibraryName)
        {
            GenericResponseVM genericResponse = null;
            try
            {                
                Dictionary<string, string> mailProperties = ContinueUpload(uploadedFile, fileExtension);
                //setting original name property for attachment
                if (string.IsNullOrWhiteSpace(mailProperties[ServiceConstants.MailOriginalName]))
                {
                    mailProperties[ServiceConstants.MAIL_ORIGINAL_NAME] = originalName;
                }

                genericResponse = docRepository.UploadDocument(folderName, uploadedFile, fileName, mailProperties, clientUrl, folder, documentLibraryName);
            }
            catch(Exception ex)
            {
                genericResponse = new GenericResponseVM()
                {
                    Code = UploadEnums.UploadFailure.ToString(),
                    Value = folderName,
                    IsError = true
                };
            }
            return genericResponse;

        }

        private Dictionary<string, string> ContinueUpload(IFormFile uploadedFile, string fileExtension)
        {
            Dictionary<string, string> mailProperties = new Dictionary<string, string>
                            {
                                { ServiceConstants.MAIL_SENDER_KEY, string.Empty },
                                { ServiceConstants.MAIL_RECEIVER_KEY, string.Empty },
                                { ServiceConstants.MAIL_RECEIVED_DATEKEY, string.Empty },
                                { ServiceConstants.MAIL_CC_ADDRESS_KEY, string.Empty },
                                { ServiceConstants.MAIL_ATTACHMENT_KEY, string.Empty },
                                { ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT, string.Empty },
                                { ServiceConstants.MAIL_SEARCH_EMAIL_FROM_MAILBOX_KEY, string.Empty },
                                { ServiceConstants.MAIL_FILE_EXTENSION_KEY, fileExtension },
                                { ServiceConstants.MAIL_IMPORTANCE_KEY, string.Empty},
                                { ServiceConstants.MAIL_CONVERSATIONID_KEY, string.Empty},
                                { ServiceConstants.MAIL_CONVERSATION_TOPIC_KEY, string.Empty},
                                { ServiceConstants.MAIL_SENT_DATE_KEY, string.Empty},
                                { ServiceConstants.MAIL_HAS_ATTACHMENTS_KEY, string.Empty},
                                { ServiceConstants.MAIL_SENSITIVITY_KEY, string.Empty },
                                { ServiceConstants.MAIL_CATEGORIES_KEY, string.Empty },
                                { ServiceConstants.MailOriginalName, string.Empty}
                            };
            if (string.Equals(fileExtension, ServiceConstants.EMAIL_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                var client = new Client()
                {
                    Url = generalSettings.CentralRepositoryUrl
                };
                
                Users currentUserDetail = userRepository.GetLoggedInUserDetails(client);
                mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_FROM_MAILBOX_KEY] = currentUserDetail.Name;
                Stream fileStream = uploadedFile.OpenReadStream();
                mailProperties = MailMessageParser.GetMailFileProperties(fileStream, mailProperties);       // Reading properties only for .eml file 
               
            }
            return mailProperties;
        }
    }
}
