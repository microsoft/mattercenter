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

using Swashbuckle.AspNetCore.SwaggerGen;
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
    /// <summary>
    /// SharedController used for some common functionality.
    /// </summary>
    [Authorize]
    [Route("api/v1/shared")]
    public class SharedController : Controller
    {
        private ErrorSettings errorSettings;
        
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private SharedSettings sharedSettings;
        private ISharedRepository sharedRepository;
        private ICustomLogger customLogger;
        private LogTables logTables;
       /// <summary>
       /// Constructor where all dependencies are injected
       /// </summary>
       /// <param name="errorSettings"></param>
       /// <param name="sharedSettings"></param>
       /// <param name="matterCenterServiceFunctions"></param>
       /// <param name="customLogger"></param>
       /// <param name="logTables"></param>
       /// <param name="sharedRepository"></param>
        public SharedController(IOptions<ErrorSettings> errorSettings,
            IOptions<SharedSettings> sharedSettings,            
            IMatterCenterServiceFunctions matterCenterServiceFunctions,            
            ICustomLogger customLogger, IOptions<LogTables> logTables,
            ISharedRepository sharedRepository
            )
        {
            this.errorSettings = errorSettings.Value;            
            
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.sharedRepository = sharedRepository;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.sharedSettings = sharedSettings.Value;
        }

        

       /// <summary>
       /// This api will return help collection information for a particular page
       /// </summary>
       /// <param name="helpRequestModel"></param>
       /// <returns></returns>
        [HttpPost("help")]
        [Produces(typeof(List<ContextHelpData>))]
         [SwaggerResponse((int)HttpStatusCode.OK, Description = "Returns IActionResult which contains help information about a page", Type = typeof(List<ContextHelpData>))]
         
        public async Task<IActionResult> Help([FromBody]HelpRequestModel helpRequestModel)
        {
            string selectedPage = helpRequestModel.SelectedPage;
            Client client = helpRequestModel.Client;
            string result = string.Empty;
            try
            {                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                
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
