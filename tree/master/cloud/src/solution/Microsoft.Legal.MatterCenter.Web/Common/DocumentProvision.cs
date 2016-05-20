using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Net;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.AspNet.Http;
using System.IO;
using Microsoft.Extensions.OptionsModel;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class DocumentProvision : IDocumentProvision
    {
        private IDocumentRepository docRepository;
        private IUploadHelperFunctions uploadHelperFunctions;
        private IUserRepository userRepository;
        private GeneralSettings generalSettings;
        public DocumentProvision(IDocumentRepository docRepository, IUserRepository userRepository, IUploadHelperFunctions uploadHelperFunctions, IOptions<GeneralSettings> generalSettings)
        {
            this.docRepository = docRepository;
            this.uploadHelperFunctions = uploadHelperFunctions;
            this.userRepository = userRepository;
            this.generalSettings = generalSettings.Value;
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
            bool result = true;
            GenericResponseVM genericResponse = null;
            if (uploadHelperFunctions.Upload(client, serviceRequest, ServiceConstants.MAIL_SOAP_REQUEST, serviceRequest.MailId, true,
                        serviceRequest.Subject, serviceRequest.FolderPath[0], true, ref message,
                        string.Empty).Equals(ServiceConstants.UPLOAD_FAILED))
            {
                result = false;
            }

            if (!result)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Code = HttpStatusCode.BadRequest.ToString(),
                        Value = message,
                        IsError = true
                    };
                    return genericResponse;
                }
                else
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Code = HttpStatusCode.BadRequest.ToString(),
                        Value = "Attachment not uploaded",
                        IsError = true
                    };
                    return genericResponse;
                }
            }
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
                IList<DocumentData> documentDataList = new List<DocumentData>();
                IEnumerable<IDictionary<string, object>> searchResults = searchResultsVM.SearchResults;
                foreach (var searchResult in searchResults)
                {
                    DocumentData documentData = new DocumentData();
                    foreach (var key in searchResult.Keys)
                    {
                        documentData.Checker = false;
                        switch (key.ToLower())
                        {
                            
                            case "mcdocumentclientname":
                                documentData.DocumentClient = searchResult[key].ToString();
                                break;
                            case "mcdocumentclientid":
                                documentData.DocumentClientId = searchResult[key].ToString();
                                break;
                            case "sitename":
                                documentData.DocumentClientUrl = searchResult[key].ToString();
                                break;
                            case "mcversionnumber":
                                documentData.DocumentVersion = searchResult[key].ToString();
                                break;
                            case "refinablestring13":
                                documentData.DocumentMatter = searchResult[key].ToString();
                                break;
                            case "refinablestring12":
                                documentData.DocumentMatterId = searchResult[key].ToString();
                                break;
                            case "filename":
                                documentData.DocumentName = searchResult[key].ToString();
                                break;
                            case "mccheckoutuser":
                                documentData.DocumentCheckoutUser = searchResult[key].ToString();
                                break;
                            case "created":
                                documentData.DocumentCreatedDate = searchResult[key].ToString();
                                break;
                            case "fileextension":
                                documentData.DocumentExtension = searchResult[key].ToString();
                                break;
                            case "docid":
                                documentData.DocumentID = searchResult[key].ToString();
                                break;
                            case "path":
                                documentData.DocumentOWAUrl = searchResult[key].ToString();
                                break;
                            case "lastmodifiedtime":
                                documentData.DocumentModifiedDate= searchResult[key].ToString();
                                break;
                            
                            case "msitofficeauthor":
                                documentData.DocumentOwner = searchResult[key].ToString();
                                break;
                            case "parentlink":
                                documentData.DocumentParentUrl = searchResult[key].ToString();
                                documentData.DocumentMatterUrl = documentData.DocumentParentUrl.Substring(0, documentData.DocumentParentUrl.LastIndexOf("/"));
                                break;

                        }
                    }
                    documentDataList.Add(documentData);
                }
                searchResultsVM.DocumentDataList = documentDataList;
                
            }
            searchResultsVM.SearchResults = null;
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
