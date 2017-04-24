// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Web
// Author           : v-lapedd
// Created          : 05-31-2016
//
// ***********************************************************************
// <copyright file="EmailController.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines service for Email functionality such as download email as attachment or download attachmemt as a links</summary>
// ***********************************************************************

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Legal.MatterCenter.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.Legal.MatterCenter.Web.Common;
using System;
using System.Reflection;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Legal.MatterCenter.Repository;

namespace Microsoft.Legal.MatterCenter.Web
{
    /// <summary>
    /// Document Controller class deals with finding document, pinning document, unpinning the document etc.
    /// </summary>
    [Authorize]
    [Route("api/v1/email")]
    public class EmailController : Controller
    {
        
        
        private ErrorSettings errorSettings;
        IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private ICustomLogger customLogger;
        private LogTables logTables;
        IDocumentProvision documentProvision;
        DocumentSettings documentSettings;
        private IMailMessageRepository mailMessageRepository;
        private IEmailProvision emailProvision;

        /// <summary>
        /// Controlls the functionality for email related.
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="customLogger"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        /// <param name="logTables"></param>
        /// <param name="documentProvision"></param>
        /// <param name="documentSettings"></param>
        public EmailController(IOptions<ErrorSettings> errorSettings,
            ICustomLogger customLogger,             
            IMatterCenterServiceFunctions matterCenterServiceFunctions, 
            IOptions<LogTables> logTables, IDocumentProvision documentProvision,            
            IOptions<DocumentSettings> documentSettings, IMailMessageRepository mailMessageRepository,
            IEmailProvision emailProvision)
        {            
            this.errorSettings = errorSettings.Value;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.documentProvision = documentProvision;
            this.documentSettings = documentSettings.Value;
            this.mailMessageRepository = mailMessageRepository;
            this.emailProvision = emailProvision;
        }

        /// <summary>
        ///  Gets the documents based on search criteria.
        /// </summary>
        /// <param name="mailAttachmentDetails"></param>
        /// <returns></returns>
        [HttpPost("downloadattachmentsasstream")]
        [Produces(typeof(Stream))]
        [SwaggerOperation("downloadAttachmentsAsStream")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Returns IActionResult which contains the file streams which needs to be downloaded to the client", Type = typeof(Stream))]
         
        public IActionResult DownloadAttachmentsAsStream([FromBody]MailAttachmentDetails mailAttachmentDetails)
        {
            try
            {
                
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                if (mailAttachmentDetails == null && mailAttachmentDetails.FullUrl == null)
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
                Stream downloadAttachments = documentProvision.DownloadAttachments(mailAttachmentDetails);

                var fileContentResponse = new HttpResponseMessage(HttpStatusCode.OK);
                fileContentResponse.Headers.Clear();

                fileContentResponse.Content = new StreamContent(downloadAttachments);

                fileContentResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(DocumentProvision.ReturnExtension(string.Empty));
                //fileContentResponse.Headers.Add("Content-Type", ReturnExtension(string.Empty));
                fileContentResponse.Content.Headers.Add("Content-Length", downloadAttachments.Length.ToString());
                fileContentResponse.Content.Headers.Add("Content-Description", "File Transfer");

                //application/octet-stream
                fileContentResponse.Content.Headers.Add("Content-Disposition", "attachment; filename=" + documentSettings.TempEmailName + new Guid().ToString() + ServiceConstants.EMAIL_FILE_EXTENSION);
                fileContentResponse.Content.Headers.Add("Content-Transfer-Encoding", "binary");
                fileContentResponse.Content.Headers.Expires = DateTimeOffset.Now.AddDays(-1); ;
                fileContentResponse.Headers.Add("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                fileContentResponse.Headers.Add("Pragma", "public");
                var fileAttachmentContent = fileContentResponse.Content.ReadAsStringAsync();
                var response = new
                {
                    fileAttachment = fileAttachmentContent,
                    fileName = documentSettings.TempEmailName + Guid.NewGuid().ToString() + ServiceConstants.EMAIL_FILE_EXTENSION
                };
                return new ObjectResult(response);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }        
        /// <summary>
        /// Get login user inbox emails
        /// </summary>
        /// <remarks>This api will get login user inbox emails</remarks>        
        /// <returns>IActionResult with user inbox emails</returns>
        [HttpPost("getuserinboxemails")]
        [Produces(typeof(IActionResult))]
        [SwaggerOperation("getuserinboxemails")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "This api will get login user inbox emails",
            Type = typeof(IActionResult))]
        
        public IActionResult GetUserInboxEmails([FromBody]MailRequest mailRequest)
        {
            try
            {
                var userInboxEmails = this.mailMessageRepository.GetUserInboxEmails(mailRequest);
                return matterCenterServiceFunctions.ServiceResponse(userInboxEmails, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
///code for multiple attachments
        /// <summary>
        /// Uploads attachment which are there in the current mail item to SharePoint library.
        /// </summary>
        /// <remarks>When the user drag an attachment from the outlook add in, this api will be called</remarks>
        /// <param name="attachmentRequestVM">This object contains information about the attachment that is getting uploaded to sharepoint</param>
        /// <returns>IActionResult with file upload success or failure</returns>
        [HttpPost("uploadattachmentsofemail")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("uploadattachmentsofemail")]
        [SwaggerResponse((int)HttpStatusCode.OK,
             Description = "Uploads attachment which are there in the current mail item to SharePoint library.",
             Type = typeof(GenericResponseVM))]

        public IActionResult UploadAttachmentsOfEmail([FromBody] AttachmentRequestVM[] attachmentRequestsVM)
        {
            try
            {
                IList<object> listResponse = new List<object>();
                GenericResponseVM genericResponse = null;
                if (attachmentRequestsVM != null && attachmentRequestsVM.Length > 0)
                {
                    foreach (var attachmentRequestVM in attachmentRequestsVM)
                    {
                        var serviceRequest = attachmentRequestVM.ServiceRequest;
                        genericResponse = emailProvision.UploadAttachments(attachmentRequestVM);
                        //If there is any error in uploading the email attachment, send that error information to the UI
                        if (genericResponse != null && genericResponse.IsError == true)
                        {
                            var errorFile = new
                            {
                                IsError = true,
                                Code = genericResponse.Code.ToString(),
                                Value = genericResponse.Value.ToString(),
                                FileName = serviceRequest.Subject,
                                DropFolder = serviceRequest.DocumentLibraryName,
                                MailId = serviceRequest.MailId
                            };
                            listResponse.Add(errorFile);
                        }
                        var successFile = new
                        {
                            IsError = false,
                            Code = HttpStatusCode.OK.ToString(),
                            Value = UploadEnums.UploadSuccess.ToString(),
                            FileName = serviceRequest.Subject,
                            DropFolder = serviceRequest.DocumentLibraryName,
                            MailId = serviceRequest.MailId
                        };
                        listResponse.Add(successFile);
                    }
                }
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
        [SwaggerOperation("bulkuploadMail")]
        [SwaggerResponse((int)HttpStatusCode.OK,
            Description = "Uploads user selected email from outlook to SharePoint library with all the attachments",
            Type = typeof(GenericResponseVM))]

        public IActionResult UploadMail([FromBody] AttachmentRequestVM[] attachmentRequestsVM)
        {
            try
            {
                GenericResponseVM genericResponse = null;
                IList<object> listResponse = new List<object>();
                if (attachmentRequestsVM != null && attachmentRequestsVM.Length > 0)
                {
                    foreach (var attachmentRequestVM in attachmentRequestsVM)
                    {
                        var client = attachmentRequestVM.Client;
                        var serviceRequest = attachmentRequestVM.ServiceRequest;
                        genericResponse = emailProvision.UploadEmails(attachmentRequestVM);
                        //If there is any error in uploading the email attachment, send that error information to the UI
                        if (genericResponse != null && genericResponse.IsError == true)
                        {
                            var errorFile = new
                            {
                                IsError = true,
                                Code = genericResponse.Code.ToString(),
                                Value = genericResponse.Value.ToString(),
                                FileName = serviceRequest.Subject,
                                DropFolder = serviceRequest.DocumentLibraryName,
                                MailId = serviceRequest.MailId
                            };
                            listResponse.Add(errorFile);
                        }
                        var successFile = new
                        {
                            IsError = false,
                            Code = HttpStatusCode.OK.ToString(),
                            Value = UploadEnums.UploadSuccess.ToString(),
                            FileName = serviceRequest.Subject,
                            DropFolder = serviceRequest.DocumentLibraryName,
                            MailId = serviceRequest.MailId
                        };
                        listResponse.Add(successFile);
                    }
                    return matterCenterServiceFunctions.ServiceResponse(listResponse, (int)HttpStatusCode.OK);
                }
                else
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }
}
