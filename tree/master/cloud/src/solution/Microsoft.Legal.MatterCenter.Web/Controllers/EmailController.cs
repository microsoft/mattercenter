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
using Swashbuckle.SwaggerGen.Annotations;

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
            IOptions<DocumentSettings> documentSettings)
        {            
            this.errorSettings = errorSettings.Value;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.documentProvision = documentProvision;
            this.documentSettings = documentSettings.Value;       
        }

        /// <summary>
        ///  Gets the documents based on search criteria.
        /// </summary>
        /// <param name="mailAttachmentDetails"></param>
        /// <returns></returns>
        [HttpPost("downloadattachmentsasstream")]
        [Produces(typeof(Stream))]
        [SwaggerOperation("downloadAttachmentsAsStream")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns IActionResult which contains the file streams which needs to be downloaded to the client", Type = typeof(Stream))]
        [SwaggerResponseRemoveDefaults]
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
    }
}
