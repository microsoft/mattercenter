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
using System.Threading.Tasks;

using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.Legal.MatterCenter.Web.Common;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.Net.Http;
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
        public EmailController(IOptionsMonitor<ErrorSettings> errorSettings,
            ICustomLogger customLogger,             
            IMatterCenterServiceFunctions matterCenterServiceFunctions, 
            IOptionsMonitor<LogTables> logTables, IDocumentProvision documentProvision, 
            IOptionsMonitor<DocumentSettings> documentSettings)
        {            
            this.errorSettings = errorSettings.CurrentValue;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
            this.documentProvision = documentProvision;
            this.documentSettings = documentSettings.CurrentValue;
        }

        /// <summary>
        /// Gets the documents based on search criteria.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        [HttpPost("downloadattachmentsasstream")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK)]
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

                return matterCenterServiceFunctions.ServiceResponse(downloadAttachments, (int)HttpStatusCode.OK);
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
        [HttpPost("downloadattachments")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK)]
        public IActionResult DownloadAttachments([FromBody]MailAttachmentDetails mailAttachmentDetails)
        {
            try
            {
                
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                if (mailAttachmentDetails == null && mailAttachmentDetails.AttachmentContent == null)
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
                string emailText = WebUtility.UrlDecode(mailAttachmentDetails.AttachmentContent);
                MemoryStream emailStream = GenerateStreamFromString(emailText);
                string emailName = documentSettings.TempEmailName + new Guid().ToString() + ServiceConstants.EMAIL_FILE_EXTENSION;
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);                
                result.Content = new StreamContent(emailStream);
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.Add("content-disposition", "inline; filename=" + emailName);

                return matterCenterServiceFunctions.ServiceResponse(result, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        ///// <summary>
        ///// Generates the stream from mail content.
        ///// </summary>
        ///// <param name="streamValue">The stream value.</param>
        ///// <returns>Memory Stream.</returns>
        private MemoryStream GenerateStreamFromString(string streamValue)
        {
            MemoryStream result = null;
            try
            {
                using (MemoryStream mailStream = new MemoryStream())
                {
                    StreamWriter mailStreamWriter = new StreamWriter(mailStream);
                    mailStreamWriter.Write(streamValue);
                    mailStreamWriter.Flush();
                    mailStream.Position = 0;
                    result = mailStream;
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                result = new MemoryStream();
            }
            return result;
        }
    }
}
