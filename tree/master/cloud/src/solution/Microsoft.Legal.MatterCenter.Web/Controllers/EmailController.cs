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

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Legal.MatterCenter.Models;
using Swashbuckle.SwaggerGen.Annotations;
using System.Threading.Tasks;

using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Extensions.OptionsModel;
using System.Net;
using Microsoft.Legal.MatterCenter.Web.Common;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;

namespace Microsoft.Legal.MatterCenter.Web
{
    /// <summary>
    /// Document Controller class deals with finding document, pinning document, unpinning the document etc.
    /// </summary>
    [Authorize]
    [Route("api/v1/email")]
    public class EmailController : Controller
    {
        private ISPOAuthorization spoAuthorization;
        private ErrorSettings errorSettings;
        IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private ICustomLogger customLogger;
        private LogTables logTables;
        IDocumentProvision documentProvision;
        public EmailController(IOptions<ErrorSettings> errorSettings,
            ICustomLogger customLogger, 
            ISPOAuthorization spoAuthorization, 
            IMatterCenterServiceFunctions matterCenterServiceFunctions, 
            IOptions<LogTables> logTables, IDocumentProvision documentProvision)
        {
            this.spoAuthorization = spoAuthorization;
            this.errorSettings = errorSettings.Value;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.documentProvision = documentProvision;
        }

        /// <summary>
        /// Gets the documents based on search criteria.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        [HttpPost("DownloadAttachments")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK)]
        public IActionResult DownloadAttachments(MailAttachmentDetails mailAttachmentDetails)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
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
    }
}
