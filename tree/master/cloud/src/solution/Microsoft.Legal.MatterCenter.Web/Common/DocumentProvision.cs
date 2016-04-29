using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Net;
using Microsoft.Legal.MatterCenter.Repository;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class DocumentProvision : IDocumentProvision
    {
        private IDocumentRepository docRepository;
        public DocumentProvision(IDocumentRepository docRepository)
        {
            this.docRepository = docRepository;
        }


        public bool UploadAttachments(ServiceRequest serviceRequest, Client client)
        {
            return true;
        }

        public bool UploadEmails(ServiceRequest serviceRequest, Client client)
        {
            return true;
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
    }
}
