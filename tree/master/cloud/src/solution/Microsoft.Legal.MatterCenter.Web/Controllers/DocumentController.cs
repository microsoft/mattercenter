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
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Web.Common;
using System.Reflection;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
#endregion


namespace Microsoft.Legal.MatterCenter.Web
{
    /// <summary>
    /// Document Controller class deals with finding document, pinning document, unpinning the document etc.
    /// </summary>
    [Authorize]
    [Route("api/v1/document")]
    public class DocumentController : Controller
    {
        private ErrorSettings errorSettings;
        private ISPOAuthorization spoAuthorization;
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private DocumentSettings documentSettings;
        private IDocumentRepository documentRepositoy;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private IDocumentProvision documentProvision;
        private GeneralSettings generalSettings;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="documentSettings"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        /// <param name="documentRepositoy"></param>
        public DocumentController(IOptions<ErrorSettings> errorSettings,
            IOptions<DocumentSettings> documentSettings,
            ISPOAuthorization spoAuthorization,
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IDocumentRepository documentRepositoy,
            ICustomLogger customLogger, IOptions<LogTables> logTables, IDocumentProvision documentProvision,
            IOptions<GeneralSettings> generalSettings

            )
        {
            this.errorSettings = errorSettings.Value;
            this.documentSettings = documentSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.documentRepositoy = documentRepositoy;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.documentProvision = documentProvision;
            this.generalSettings = generalSettings.Value;
        }

        /// <summary>
        /// Get all counts for all documentCounts, my documentCounts and pinned documentCounts
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("getdocumentcounts")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> GetDocumentCounts([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {
                //Get the authorization token from the Request header
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                #region Error Checking                
                if (searchRequestVM == null && searchRequestVM.Client == null && searchRequestVM.SearchObject == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion                
                int allDocumentCounts = await documentProvision.GetAllCounts(searchRequestVM);
                int myDocumentCounts = await documentProvision.GetMyCounts(searchRequestVM);
                int pinnedDocumentCounts = await documentProvision.GetPinnedCounts(searchRequestVM.Client);
                var documentCounts = new
                {
                    AllDocumentCounts = allDocumentCounts,
                    MyDocumentCounts = myDocumentCounts,
                    PinnedDocumentCounts = pinnedDocumentCounts,
                };
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
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        [HttpPost("getdocuments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                GenericResponseVM genericResponse = null;
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
                var searchResultsVM = await documentProvision.GetDocumentsAsync(searchRequestVM);
                return matterCenterServiceFunctions.ServiceResponse(searchResultsVM.DocumentDataList, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Get all the documents which are pinned
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("getpinneddocuments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> GetPin([FromBody]Client client)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                //if the token is not valid, immediately return no authorization error to the user                
                if (client == null)
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
                var pinResponseVM = await documentRepositoy.GetPinnedRecordsAsync(client);
                //if (pinResponseVM != null && pinResponseVM.TotalRows == 0)
                //{
                //    genericResponse = new GenericResponseVM()
                //    {
                //        Value = pinResponseVM.NoPinnedMessage,
                //        Code = ((int)HttpStatusCode.NotFound).ToString(),
                //        IsError = true
                //    };
                //    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                //}
                return matterCenterServiceFunctions.ServiceResponse(pinResponseVM.DocumentDataList, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Pin a new document
        /// </summary>
        /// <param name="pinRequestDocumentVM"></param>
        /// <returns></returns>
        [HttpPost("pindocument")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> Pin([FromBody]PinRequestDocumentVM pinRequestDocumentVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
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
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var isDocumentPinned = await documentRepositoy.PinRecordAsync<PinRequestDocumentVM>(pinRequestDocumentVM);
                var documentPinned = new
                {
                    IsDocumentPinned = isDocumentPinned
                };
                return matterCenterServiceFunctions.ServiceResponse(documentPinned, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Unpin the document which is already pinned
        /// </summary>
        /// <param name="pinRequestMatterVM"></param>
        /// <returns></returns>
        [HttpPost("unpindocument")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> UnPin([FromBody]PinRequestDocumentVM pinRequestDocumentVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
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
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var isDocumentUnPinned = await documentRepositoy.UnPinRecordAsync<PinRequestDocumentVM>(pinRequestDocumentVM);
                var documentUnPinned = new
                {
                    IsDocumentUnPinned = isDocumentUnPinned
                };
                return matterCenterServiceFunctions.ServiceResponse(documentUnPinned, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Returns document and list GUID
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing list data</param>        
        /// <returns>Document and list GUID</returns>
        [HttpPost("getassets")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> GetDocumentAssets(Client client)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
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
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var documentAsset = await documentRepositoy.GetDocumentAndClientGUIDAsync(client);
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
        /// <param name="attachmentRequestVM"></param>
        /// <returns></returns>
        [HttpPost("uploadattachments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IActionResult UploadAttachments([FromBody] AttachmentRequestVM attachmentRequestVM)
        {            
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                var client = attachmentRequestVM.Client;
                var serviceRequest = attachmentRequestVM.ServiceRequest;
                GenericResponseVM genericResponse = null;
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (client == null && serviceRequest==null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                if (serviceRequest.FolderPath.Count != serviceRequest.Attachments.Count)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = "Folder path count and attachment count are not same",
                        ErrorCode = HttpStatusCode.BadRequest.ToString()                        
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                
                genericResponse = documentProvision.UploadAttachments(attachmentRequestVM);
                if(genericResponse!=null && genericResponse.IsError==true)
                {
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                genericResponse = new GenericResponseVM()
                {
                    Code = HttpStatusCode.OK.ToString(),
                    Value = "Attachment upload success"
                };
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
        /// <param name="attachmentRequestVM"></param>
        /// <returns></returns>
        [HttpPost("uploadfiles")]
        [SwaggerResponse(HttpStatusCode.OK)]
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
                bool isDeployedOnAzure = Convert.ToBoolean(generalSettings.IsTenantDeployment, CultureInfo.InvariantCulture);
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                string originalName = string.Empty;
                bool allowContentCheck = Convert.ToBoolean(Request.Form["AllowContentCheck"], CultureInfo.InvariantCulture);
                Int16 isOverwrite = 3;                
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
                    ContentCheckDetails contentCheckDetails = new ContentCheckDetails(fileMetadata.FileName, uploadedFile.Length);
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

                        if (2 == isOverwrite)   //If User presses "Perform content check" option in overwrite Popup
                        {
                            continueUpload = false;
                            //response = PerformContentCheck(folderName, listResponse, response, upload, clientContext);
                        }
                        else if (3 == isOverwrite)  //If user presses "Cancel upload" option in overwrite popup or file is being uploaded for the first time
                        {
                            genericResponse = documentProvision.CheckDuplicateDocument(clientUrl, folderName, documentLibraryName, listResponse, fileName, contentCheckDetails, allowContentCheck);
                        }
                        else if (1 == isOverwrite)  //If User presses "Append date to file name and save" option in overwrite Popup
                        {
                            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                            string timeStampSuffix = DateTime.Now.ToString(documentSettings.TimeStampFormat, CultureInfo.InvariantCulture).Replace(":", "_");
                            fileName = fileNameWithoutExt + "_" + timeStampSuffix + fileExtension;
                        }
                        if(genericResponse==null)
                        {
                            genericResponse = documentProvision.UploadFiles(uploadedFile, fileExtension, originalName, folderUrl, fileName,
                                clientUrl, folder, documentLibraryName);
                        }
                        if (genericResponse == null)
                        {
                            var successFile = new
                            {
                                IsError = false,
                                Code = HttpStatusCode.OK.ToString(),
                                Value = UploadEnums.UploadSuccess.ToString(),
                                FileName = fileName,
                                DropFolder = folderName
                            };
                            listResponse.Add(successFile);
                        }
                        else
                        {
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
                return matterCenterServiceFunctions.ServiceResponse(listResponse, (int)HttpStatusCode.OK);
                #endregion
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Uploads mail to SharePoint library.
        /// </summary>
        /// <param name="attachmentRequestVM"></param>
        /// <returns></returns>
        [HttpPost("uploadmail")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IActionResult UploadMail([FromBody] AttachmentRequestVM attachmentRequestVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                var client = attachmentRequestVM.Client;
                var serviceRequest = attachmentRequestVM.ServiceRequest;
                GenericResponseVM genericResponse = null;

                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (client == null && serviceRequest==null && string.IsNullOrWhiteSpace(serviceRequest.MailId))
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                genericResponse = documentProvision.UploadEmails(attachmentRequestVM);
                if (genericResponse != null && genericResponse.IsError == true)
                {                                
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                genericResponse = new GenericResponseVM()
                {
                    Code = HttpStatusCode.OK.ToString(),
                    Value = "Attachment upload success"
                };
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
