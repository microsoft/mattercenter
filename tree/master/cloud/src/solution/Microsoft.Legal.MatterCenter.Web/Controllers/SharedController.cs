// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-lapedd
// Created          : 04-09-2016
//
// ***********************************************************************
// <copyright file="SharedController.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines service for Taxonomy</summary>
// ***********************************************************************


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using System.Reflection;
using Microsoft.Legal.MatterCenter.Web.Common;
#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
#endregion
namespace Microsoft.Legal.MatterCenter.Web
{
    [Authorize]
    [Route("api/v1/shared")]
    public class SharedController : Controller
    {
        private ErrorSettings errorSettings;
        private ISPOAuthorization spoAuthorization;
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private SharedSettings sharedSettings;
        private ISharedRepository sharedRepository;
        private ICustomLogger customLogger;
        private LogTables logTables;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="matterSettings"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        public SharedController(IOptionsMonitor<ErrorSettings> errorSettings,
            IOptionsMonitor<SharedSettings> sharedSettings,
            ISPOAuthorization spoAuthorization,
            IMatterCenterServiceFunctions matterCenterServiceFunctions,            
            ICustomLogger customLogger, IOptionsMonitor<LogTables> logTables,
            ISharedRepository sharedRepository
            )
        {
            this.errorSettings = errorSettings.CurrentValue;            
            this.spoAuthorization = spoAuthorization;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.sharedRepository = sharedRepository;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
            this.sharedSettings = sharedSettings.CurrentValue;
        }

        /// <summary>
        /// Returns true or false based on the existence of the matter landing page and OneNote file at the URLs provided.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="requestedUrl">String object containing the OneNote file path</param>
        /// <param name="requestedPageUrl">String object containing the Matter Landing Page file path</param>
        /// <returns>$|$ Separated string indicating that the OneNote and the Matter Landing Page exist or not</returns>        
        [HttpPost("urlexists")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        public async Task<IActionResult> UrlExists(Client client, string oneNoteUrl, string matterLandingPageUrl)
        {
            string result = string.Empty;
            
            try
            {

                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;
                //if the token is not valid, immediately return no authorization error to the user
                if (errorResponse != null && !errorResponse.IsTokenValid)
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
                }

                if (client == null && string.IsNullOrEmpty(oneNoteUrl) && string.IsNullOrEmpty(matterLandingPageUrl))
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
                var oneNoteUrlExists = await sharedRepository.UrlExistsAsync(client, oneNoteUrl);
                var matterLandingUrlExists = await sharedRepository.UrlExistsAsync(client, matterLandingPageUrl);
                var urlExists = new
                {
                    OneNoteExists = oneNoteUrlExists,
                    MatterLandingExists = matterLandingUrlExists
                };
                return matterCenterServiceFunctions.ServiceResponse(urlExists, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Returns contextual help content in JSON format.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="selectedPage">String object containing the page number where user is on</param>
        /// <returns>IActionResult which return List of ContextHelpData in JSON format</returns>        
        [HttpPost("help")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        public async Task<IActionResult> Help([FromBody]HelpRequestModel helpRequestModel)
        {
            string selectedPage = helpRequestModel.SelectedPage;
            Client client = helpRequestModel.Client;
            string result = string.Empty;
            try
            {

                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;
                //if the token is not valid, immediately return no authorization error to the user
                if (errorResponse != null && !errorResponse.IsTokenValid)
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
                }

                if (client == null && string.IsNullOrWhiteSpace(selectedPage))
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
                List<ContextHelpData> contextHelpCollection = new List<ContextHelpData>();
                string[] pageNames = sharedSettings.MatterCenterPages.Split(';');
                switch (selectedPage)
                {
                    case "1":
                        selectedPage = pageNames[1];
                        break;
                    case "2":
                        selectedPage = pageNames[2];
                        break;
                    case "4":
                        selectedPage = pageNames[4];
                        break;
                    default:
                        selectedPage = pageNames[0];
                        break;
                }
                string cacheKey = string.Concat(selectedPage, ServiceConstants.LINKS_STATIC_STRING);
                result = ServiceUtility.GetDataFromAzureRedisCache(cacheKey);
                if(string.IsNullOrEmpty(result))
                {
                    contextHelpCollection = await sharedRepository.GetMatterHelpAsync(client, selectedPage);
                    ServiceUtility.SetDataIntoAzureRedisCache<List<ContextHelpData>>(cacheKey, contextHelpCollection);
                }
                else
                {
                    contextHelpCollection = JsonConvert.DeserializeObject<List<ContextHelpData>>(result);
                }
                return matterCenterServiceFunctions.ServiceResponse(contextHelpCollection, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }
}
