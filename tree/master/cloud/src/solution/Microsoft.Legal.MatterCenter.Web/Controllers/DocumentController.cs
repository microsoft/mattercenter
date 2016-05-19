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
            ICustomLogger customLogger, IOptions<LogTables> logTables, IDocumentProvision documentProvision
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
                ErrorResponse errorResponse = null;
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
                ErrorResponse errorResponse = null;
                //if the token is not valid, immediately return no authorization error to the user                
                if (client == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
                }
                #endregion
                var pinResponseVM = await documentRepositoy.GetPinnedRecordsAsync(client);
                if (pinResponseVM != null && pinResponseVM.TotalRows == 0)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = pinResponseVM.NoPinnedMessage,
                        ErrorCode = ((int)HttpStatusCode.NotFound).ToString(),
                        Description = "No resource found for your search criteria"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                return matterCenterServiceFunctions.ServiceResponse(pinResponseVM, (int)HttpStatusCode.OK);
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
                ErrorResponse errorResponse = null;
                if (pinRequestDocumentVM == null && pinRequestDocumentVM.Client == null && pinRequestDocumentVM.DocumentData == null)
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
        public async Task<IActionResult> UnPin([FromBody]PinRequestMatterVM pinRequestMatterVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (pinRequestMatterVM == null && pinRequestMatterVM.Client == null && pinRequestMatterVM.MatterData == null)
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
                var isDocumentUnPinned = await documentRepositoy.UnPinRecordAsync<PinRequestMatterVM>(pinRequestMatterVM);
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
                ErrorResponse errorResponse = null;
                if (client == null)
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
        /// Uploads attachment to SharePoint library.
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
