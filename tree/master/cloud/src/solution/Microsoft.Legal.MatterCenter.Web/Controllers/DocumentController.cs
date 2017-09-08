// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-lapedd
// Created          : 04-09-2016
//
// ***********************************************************************
// <copyright file="DocumentController.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines service for Taxonomy</summary>
// ***********************************************************************

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using Newtonsoft.Json;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Web.Common;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
#endregion


namespace Microsoft.Legal.MatterCenter.Web
{
    /// <summary>
    /// Document Controller class deals with finding document, 
    /// pinning document, unpinning the document etc.
    /// 
    /// </summary>
    [Authorize]
    [Route("api/v1/document")]
    public class DocumentController : Controller
    {
        private ErrorSettings errorSettings;        
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private DocumentSettings documentSettings;
        private IDocumentRepository documentRepositoy;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private IDocumentProvision documentProvision;
        private GeneralSettings generalSettings;
        private ISPOAuthorization spoAuthorization;

        /// <summary>
        /// DcouemtsController Constructor where all the required dependencies are injected
        /// </summary>
        /// <remarks></remarks>        /// 
        /// <param name="errorSettings"></param>
        /// <param name="documentSettings"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        /// <param name="documentRepositoy"></param>
        /// <param name="customLogger"></param>
        /// <param name="logTables"></param>
        /// <param name="documentProvision"></param>
        /// <param name="generalSettings"></param>
        public DocumentController(IOptions<ErrorSettings> errorSettings,
            IOptions<DocumentSettings> documentSettings,            
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IDocumentRepository documentRepositoy,
            ICustomLogger customLogger, IOptions<LogTables> logTables, IDocumentProvision documentProvision,
            IOptions<GeneralSettings> generalSettings,
            ISPOAuthorization spoAuthorization

            )
        {
            this.errorSettings = errorSettings.Value;
            this.documentSettings = documentSettings.Value;            
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.documentRepositoy = documentRepositoy;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.documentProvision = documentProvision;
            this.generalSettings = generalSettings.Value;
            this.spoAuthorization = spoAuthorization;
        }

        /// <summary>
        /// Get all counts for all documentCounts, my documentCounts and pinned documentCounts
        /// </summary>
        /// <remarks>Pass in the search request object which contains the information that will be passed to sharepoint search
        /// to get all counts for all documents, my documents and pinned documents</remarks>
        /// <param name="searchRequestVM">The search request object that has been find by the user to get results back for search criteria</param>
        /// <returns>IActionResult with proper http status code and the search results in JSON format</returns>
        [HttpPost("getdocumentcounts")]
        [Produces(typeof(int))]
        [SwaggerOperation("getDocumentCounts")]
        [SwaggerResponse((int)HttpStatusCode.OK, 
            Description = "Get all counts for all documentCounts, my documentCounts and pinned documentCounts", 
            Type = typeof(int))]
         
        public async Task<IActionResult> GetDocumentCounts([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {     
                GenericResponseVM genericResponse = null;
                #region Error Checking    
                //Input validation
                if (searchRequestVM == null && searchRequestVM.Client == null && searchRequestVM.SearchObject == null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion                
                //For a given search request entered by the user, this api will get all documents that has been 
                //uploaded by him, all documents that are assigned to him and all the documents which are pinned by him
                int allDocumentCounts = await documentProvision.GetAllCounts(searchRequestVM);
                int myDocumentCounts = await documentProvision.GetMyCounts(searchRequestVM);
                int pinnedDocumentCounts = await documentProvision.GetPinnedCounts(searchRequestVM);
                //The object count information that will be sent to the user
                var documentCounts = new
                {
                    AllDocumentCounts = allDocumentCounts,
                    MyDocumentCounts = myDocumentCounts,
                    PinnedDocumentCounts = pinnedDocumentCounts,
                };
                //If the input validation is failed, send GenericResponseVM which contains the error information
                return matterCenterServiceFunctions.ServiceResponse(documentCounts, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Gets the documents based on search criteria.
        /// </summary>
        ///<remarks>Gets all the documents from sharepoint that has been uploaded by the user. 
        ///The user will get only the dcouments for which he or she has got the permissions</remarks>
        ///<example></example>
        /// <param name="searchRequestVM">The search request object that has been find by the user to get results back for search criteria</param>
        /// <returns>IActionResult with proper http status code and the search results in JSON format</returns>
        [HttpPost("getdocuments")]
        [Produces(typeof(SearchResponseVM))]
        [SwaggerOperation("getDocuments")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "Gets the documents based on search criteria.",
            Type = typeof(SearchResponseVM))]
         
        public async Task<IActionResult> Get([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {        
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                //Input validation
                if (searchRequestVM == null && searchRequestVM.Client == null && searchRequestVM.SearchObject == null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion 
                ClientContext clientContext = null;
                clientContext = spoAuthorization.GetClientContext(searchRequestVM.Client.Url);
                var searchResultsVM = await documentProvision.GetDocumentsAsync(searchRequestVM, clientContext);
                return matterCenterServiceFunctions.ServiceResponse(searchResultsVM.DocumentDataList, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                var errorResponse = customLogger.GenerateErrorResponse(ex);
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Get all the documents which are pinned by the logged in user
        /// </summary>
        /// <remarks>Gets all the documents which are pinned by the user</remarks>
        /// <param name="client">The SPO client url from which to retrieve all the documents which are pinnned by the requested user</param>
        /// <returns>IActionResult with proper http status code and the search results in JSON format</returns>
        [HttpPost("getpinneddocuments")]
        [Produces(typeof(SearchResponseVM))]
        [SwaggerOperation("getGinnedDocuments")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "Get all the documents which are pinned by the logged in user",
            Type = typeof(SearchResponseVM))]
         
        public async Task<IActionResult> GetPin([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {   
                #region Error Checking    
                //Input validation            
                GenericResponseVM genericResponse = null;                                
                if (searchRequestVM == null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                ClientContext clientContext = null;
                clientContext = spoAuthorization.GetClientContext(searchRequestVM.Client.Url);
                //Get the documents which are pinned by the user
                var pinResponseVM = await documentProvision.GetPinnedDocumentsAsync(searchRequestVM, clientContext);  
                //Return the response with proper http status code              
                return matterCenterServiceFunctions.ServiceResponse(pinResponseVM.DocumentDataList, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                var errorResponse = customLogger.GenerateErrorResponse(ex);
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// This api will store the metadata of the document in a sharepoint list as a JSON object which is getting pinned
        /// </summary>
        /// <remarks>This api will pin a document along with its metadata</remarks>
        /// <param name="pinRequestDocumentVM">All the metadata associated with the doccument that is getting pinned</param>
        /// <returns>IActionResult which will return whether a document is pinned or not</returns>
        [HttpPost("pindocument")]
        [Produces(typeof(bool))]
        [SwaggerOperation("pinDocument")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "This api will store the metadata of the document in a sharepoint list as a JSON object which is getting pinned",
            Type = typeof(bool))]
         
        public async Task<IActionResult> Pin([FromBody]PinRequestDocumentVM pinRequestDocumentVM)
        {
            try
            {       
                #region Error Checking       
                //Input validation         
                GenericResponseVM genericResponse = null;
                if (pinRequestDocumentVM == null && pinRequestDocumentVM.Client == null && pinRequestDocumentVM.DocumentData == null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var isDocumentPinned = await documentRepositoy.PinRecordAsync<PinRequestDocumentVM>(pinRequestDocumentVM);
                var documentPinned = new
                {
                    IsDocumentPinned = isDocumentPinned
                };
                //Return the response with proper http status code and proper response object
                return matterCenterServiceFunctions.ServiceResponse(documentPinned, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This api will unpin the document which is already pinned and the unpinned document will be removed from the sharepoint 
        /// list object
        /// </summary>
        /// <remarks>This api will help the user to unpin a document which is already pinned</remarks>
        /// <param name="pinRequestDocumentVM">Information about the document which needs to be unpinned</param>
        /// <returns></returns>
        [HttpPost("unpindocument")]
        [Produces(typeof(bool))]
        [SwaggerOperation("unpinDocument")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "This api will unpin the document which is already pinned and the unpinned document will be removed from the sharepoint lists",
            Type = typeof(bool))]
         
        public async Task<IActionResult> UnPin([FromBody]PinRequestDocumentVM pinRequestDocumentVM)
        {
            try
            {   
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                if (pinRequestDocumentVM == null && pinRequestDocumentVM.Client == null && pinRequestDocumentVM.DocumentData == null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var isDocumentUnPinned = await documentRepositoy.UnPinRecordAsync<PinRequestDocumentVM>(pinRequestDocumentVM);
                var documentUnPinned = new
                {
                    IsDocumentUnPinned = isDocumentUnPinned
                };
                //Return the response with proper http status code and proper response object
                return matterCenterServiceFunctions.ServiceResponse(documentUnPinned, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This api will return document guid and list guid for the matter that has been selected
        /// </summary> 
        /// <remarks>From the document fly out menu, if the user clicks on "Open Document", this information will be used internally</remarks>
        /// <param name="client">Client object containing list data</param>        
        /// <returns>Document and list GUID</returns>
        [HttpPost("getassets")]
        
        [SwaggerOperation("getAssets")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "This api will return document guid and list guid for the matter that has been selected")]
         
        public async Task<IActionResult> GetDocumentAssets([FromBody]Client client)
        {
            try
            {      
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                if (client == null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var documentAsset = await documentRepositoy.GetDocumentAndClientGUIDAsync(client);
                //Return the response with proper http status code and proper response object
                return matterCenterServiceFunctions.ServiceResponse(documentAsset, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Uploads attachment which are there in the current mail item to SharePoint library.
        /// </summary>
        /// <remarks>When the user drag an attachment from the outlook add in, this api will be called</remarks>
        /// <param name="attachmentRequestVM">This object contains information about the attachment that is getting uploaded to sharepoint</param>
        /// <returns>IActionResult with file upload success or failure</returns>
        [HttpPost("uploadattachments")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("uploadAttachments")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "Uploads attachment which are there in the current mail item to SharePoint library.",
            Type = typeof(GenericResponseVM))]
         
        public IActionResult UploadAttachments([FromBody] AttachmentRequestVM attachmentRequestVM)
        {            
            try
            {        
                var client = attachmentRequestVM.Client;
                var serviceRequest = attachmentRequestVM.ServiceRequest;
                GenericResponseVM genericResponse = null;
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (client == null && serviceRequest==null)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                if (serviceRequest.FolderPath.Count != serviceRequest.Attachments.Count)
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = "Folder path count and attachment count are not same",
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    //If the input validation is failed, send GenericResponseVM which contains the error information
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                //Upload attachments to the sharepoint document library the user has choosen
                genericResponse = documentProvision.UploadAttachments(attachmentRequestVM);
                //If there is any error in uploading the attachment, send that error information to the UI
                if(genericResponse!=null && genericResponse.IsError==true)
                {
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                //
                genericResponse = new GenericResponseVM()
                {
                    Code = HttpStatusCode.OK.ToString(),
                    Value = "Attachment upload success"
                };
                //Return the response with proper http status code and proper response object
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }



        /// <summary>
        /// Uploads attachments from the user desktop to sharepoint library
        /// </summary>
        /// <remarks>This api will allow the user to upload from his desktop/laptop</remarks>
        /// <returns>IActionResult with the file upload success or failure</returns>
        [HttpPost("uploadfiles")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("uploadFiles")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "Uploads attachments from the user desktop to sharepoint library",
            Type = typeof(GenericResponseVM))]
         
        public IActionResult UploadFiles()
        {
            try
            {
                IFormFileCollection fileCollection = Request.Form.Files;
                Regex regEx = new Regex("[*?|\\\t/:\"\"'<>#{}%~&]");
                string clientUrl = Request.Form["clientUrl"];
                string folderUrl = Request.Form["folderUrl"];
                string folderName = folderUrl.Substring(folderUrl.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);
                string documentLibraryName = Request.Form["documentLibraryName"];
                MatterExtraProperties documentExtraProperites=null;
                if (!string.IsNullOrWhiteSpace(Request.Form["DocumentExtraProperties"]))
                {
                     documentExtraProperites = JsonConvert.DeserializeObject<MatterExtraProperties>(Request.Form["DocumentExtraProperties"].ToString());
                }
                bool isDeployedOnAzure = Convert.ToBoolean(generalSettings.IsTenantDeployment, CultureInfo.InvariantCulture);
                
                string originalName = string.Empty;
                bool allowContentCheck = false;
                bool.TryParse(Request.Form["AllowContentCheck"], out allowContentCheck);
                Int16 isOverwrite = 3;     
                //Input validation           
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                IList<object> listResponse = new List<object>();
                bool continueUpload = true;
                if (isDeployedOnAzure == false && string.IsNullOrWhiteSpace(clientUrl) && string.IsNullOrWhiteSpace(folderUrl))
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                //Get all the files which are uploaded by the user
                for (int fileCounter = 0; fileCounter < fileCollection.Count; fileCounter++)
                {
                    IFormFile uploadedFile = fileCollection[fileCounter];
                    if (!Int16.TryParse(Request.Form["Overwrite" + fileCounter], out isOverwrite))
                    {
                        isOverwrite = 3;
                    }
                    continueUpload = true;
                    ContentDispositionHeaderValue fileMetadata = ContentDispositionHeaderValue.Parse(uploadedFile.ContentDisposition);
                    string fileName = originalName = fileMetadata.FileName.Trim('"');
                    fileName = System.IO.Path.GetFileName(fileName);
                    ContentCheckDetails contentCheckDetails = new ContentCheckDetails(fileName, uploadedFile.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName).Trim();
                    if (-1 < fileName.IndexOf('\\'))
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                    }
                    else if (-1 < fileName.IndexOf('/'))
                    {
                        fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                    }
                    if (null != uploadedFile.OpenReadStream() && 0 == uploadedFile.OpenReadStream().Length)
                    {
                        listResponse.Add(new GenericResponseVM() { Code = fileName, Value = errorSettings.ErrorEmptyFile, IsError = true });
                    }
                    else if (regEx.IsMatch(fileName))
                    {
                        listResponse.Add(new GenericResponseVM() { Code = fileName, Value = errorSettings.ErrorInvalidCharacter, IsError = true });
                    }
                    else
                    {
                        string folder = folderUrl.Substring(folderUrl.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);
                        //If User presses "Perform content check" option in overwrite Popup
                        if (2 == isOverwrite)   
                        {                            
                            genericResponse = documentProvision.PerformContentCheck(clientUrl, folderUrl, uploadedFile, fileName);
                        }
                        //If user presses "Cancel upload" option in overwrite popup or file is being uploaded for the first time
                        else if (3 == isOverwrite)  
                        {
                            genericResponse = documentProvision.CheckDuplicateDocument(clientUrl, folderUrl, documentLibraryName, fileName, contentCheckDetails, allowContentCheck);
                        }
                        //If User presses "Append date to file name and save" option in overwrite Popup
                        else if (1 == isOverwrite)  
                        {
                            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                            string timeStampSuffix = DateTime.Now.ToString(documentSettings.TimeStampFormat, CultureInfo.InvariantCulture).Replace(":", "_");
                            fileName = fileNameWithoutExt + "_" + timeStampSuffix + fileExtension;
                        }
                        if(genericResponse==null)
                        {
                            genericResponse = documentProvision.UploadFiles(uploadedFile, fileExtension, originalName, folderUrl, fileName,
                                clientUrl, folder, documentLibraryName, documentExtraProperites);
                        }
                        if (genericResponse == null)
                        {
                            string documentIconUrl = string.Empty;
                            fileExtension = fileExtension.Replace(".", "");
                            if (fileExtension.ToLower() != "pdf")
                            {
                                documentIconUrl = $"{generalSettings.SiteURL}/_layouts/15/images/ic{fileExtension}.gif";
                            }
                            else
                            {
                                documentIconUrl = $"{generalSettings.SiteURL}/_layouts/15/images/ic{fileExtension}.png";
                            }
                            //Create a json object with file upload success
                            var successFile = new
                            {
                                IsError = false,
                                Code = HttpStatusCode.OK.ToString(),
                                Value = UploadEnums.UploadSuccess.ToString(),
                                FileName = fileName,
                                DropFolder = folderName,
                                DocumentIconUrl = documentIconUrl
                            };
                            listResponse.Add(successFile);
                        }
                        else
                        {
                            //Create a json object with file upload failure
                            var errorFile = new
                            {
                                IsError = true,
                                Code = genericResponse.Code.ToString(),
                                Value = genericResponse.Value.ToString(),
                                FileName = fileName,
                                DropFolder = folderName
                            };
                            listResponse.Add(errorFile);                           
                        }                   
                    }
                }
                //Return the response with proper http status code and proper response object     
                return matterCenterServiceFunctions.ServiceResponse(listResponse, (int)HttpStatusCode.OK);
                
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Uploads user selected email from outlook to SharePoint library with all the attachments
        /// </summary>
        /// <remarks>This api will allow the user to upload mail with attachments to the sharepoint document libarary associated to the matter</remarks>
        /// <param name="attachmentRequestVM"></param>
        /// <returns>IActionResult with mail upload failure or success</returns>
        [HttpPost("uploadmail")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("uploadMail")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "Uploads user selected email from outlook to SharePoint library with all the attachments",
            Type = typeof(GenericResponseVM))]
         
        public IActionResult UploadMail([FromBody] AttachmentRequestVM attachmentRequestVM)
        {
            try
            {   
                var client = attachmentRequestVM.Client;
                var serviceRequest = attachmentRequestVM.ServiceRequest;
                GenericResponseVM genericResponse = null;
                #region Error Checking   
                //Input validation             
                ErrorResponse errorResponse = null;
                if (client == null && serviceRequest==null && string.IsNullOrWhiteSpace(serviceRequest.MailId))
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                //Upload email to the share point library
                genericResponse = documentProvision.UploadEmails(attachmentRequestVM);
                //If there is any error in uploading the email attachment, send that error information to the UI
                if (genericResponse != null && genericResponse.IsError == true)
                {                                
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                //If the email attachment is success, send the success response to the user
                genericResponse = new GenericResponseVM()
                {
                    Code = HttpStatusCode.OK.ToString(),
                    Value = "Attachment upload success"
                };
                //Return the response with proper http status code and proper response object
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }
}
