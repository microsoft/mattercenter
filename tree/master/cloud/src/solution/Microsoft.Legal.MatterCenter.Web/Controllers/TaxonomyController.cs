// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="TaxonomyController.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines service for Taxonomy</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Swashbuckle.SwaggerGen.Annotations;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNet.Authorization;
using System.Net;
#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Service.HelperClasses;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Web.Common;
#endregion

namespace Microsoft.Legal.MatterCenter.Service.Controllers
{
    /// <summary>
    /// Taxonomy Controller will read the term store information related to matter center
    /// </summary>
    //[Authorize]
    [Route("api/v1/taxonomy")]
    
    public class TaxonomyController:Controller
    {
        private ErrorSettings errorSettings;
        private ISPOAuthorization spoAuthorization;
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private TaxonomySettings taxonomySettings;
        private GeneralSettings generalSettings;
        private ITaxonomyRepository taxonomyRepository;
        private ICustomLogger customLogger;
        private LogTables logTables;

        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="taxonomySettings"></param>
        /// <param name="generalSettings"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        /// <param name="taxonomyRepository"></param>
        public TaxonomyController(IOptions<ErrorSettings> errorSettings, 
            IOptions<TaxonomySettings> taxonomySettings, 
            IOptions<GeneralSettings> generalSettings,
            ISPOAuthorization spoAuthorization, 
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            ITaxonomyRepository taxonomyRepository, ICustomLogger customLogger, IOptions<LogTables> logTables)
        {
            this.errorSettings = errorSettings.Value;
            this.taxonomySettings = taxonomySettings.Value;
            this.generalSettings = generalSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;            
            this.taxonomyRepository = taxonomyRepository;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
        }
         
        [HttpPost("gettaxonomy")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// Gets the hierarchy of terms along with the specific custom properties of each term from term store.
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>
        ///
        public async Task<IActionResult> GetTaxonomy([FromBody]TermStoreViewModel termStoreViewModel)
        {
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

                ValidationHelperFunctions.ErrorSettings = errorSettings;
                errorResponse = ValidationHelperFunctions.TaxonomyValidation(termStoreViewModel.Client);
                if (errorResponse != null && !String.IsNullOrWhiteSpace(errorResponse.Message))
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                #endregion

                string cacheValue = string.Empty;
                string key = string.Empty;
                var details = termStoreViewModel.TermStoreDetails;
                if (details.TermSetName == taxonomySettings.PracticeGroupTermSetName)
                {
                    key = ServiceConstants.CACHE_MATTER_TYPE;
                }
                else if (details.TermSetName == taxonomySettings.ClientTermSetName)
                {
                    key = ServiceConstants.CACHE_CLIENTS;
                }

                ServiceUtility.GeneralSettings = generalSettings;
                //cacheValue = ServiceUtility.GetDataFromAzureRedisCache(key);
                TaxonomyResponseVM taxonomyRepositoryVM = null;
                if (String.IsNullOrEmpty(cacheValue))
                {
                    taxonomyRepositoryVM = await taxonomyRepository.GetTaxonomyHierarchyAsync(termStoreViewModel);
                    if (termStoreViewModel.TermStoreDetails.TermSetName == taxonomySettings.PracticeGroupTermSetName && taxonomyRepositoryVM.TermSets!=null)
                    {                        
                        ServiceUtility.SetDataIntoAzureRedisCache<TermSets>(key, taxonomyRepositoryVM.TermSets);
                        return matterCenterServiceFunctions.ServiceResponse(taxonomyRepositoryVM.TermSets, (int)HttpStatusCode.OK);
                    }
                    if (termStoreViewModel.TermStoreDetails.TermSetName == taxonomySettings.ClientTermSetName && taxonomyRepositoryVM.ClientTermSets != null)
                    {                        
                        ServiceUtility.SetDataIntoAzureRedisCache<ClientTermSets>(key, taxonomyRepositoryVM.ClientTermSets);
                        return matterCenterServiceFunctions.ServiceResponse(taxonomyRepositoryVM.ClientTermSets, (int)HttpStatusCode.OK);
                    }
                }
                else
                {
                    if (termStoreViewModel.TermStoreDetails.TermSetName == taxonomySettings.PracticeGroupTermSetName)
                    {
                        var pgTermSets = JsonConvert.DeserializeObject<TermSets>(cacheValue);
                        if (pgTermSets == null)
                        {
                            errorResponse = new ErrorResponse()
                            {
                                Message = errorSettings.MessageNoResult,
                                ErrorCode = "404",
                                Description = "No data is present for the given passed input"
                            };
                            return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
                        }
                        return matterCenterServiceFunctions.ServiceResponse(pgTermSets, (int)HttpStatusCode.OK);
                    }
                    if (termStoreViewModel.TermStoreDetails.TermSetName == taxonomySettings.ClientTermSetName)
                    {
                        var clientTermSets = JsonConvert.DeserializeObject<ClientTermSets>(cacheValue);
                        if (clientTermSets == null)
                        {
                            errorResponse = new ErrorResponse()
                            {
                                Message = errorSettings.MessageNoResult,
                                ErrorCode = "404",
                                Description = "No data is present for the given passed input"
                            };
                            return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
                        }
                        return matterCenterServiceFunctions.ServiceResponse(clientTermSets, (int)HttpStatusCode.OK);
                    }
                }
                //If all the above condition fails, return validation error object
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoResult,
                    ErrorCode = "404",
                    Description = "No data is present for the given passed input"                    
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            
        }
        /// <summary>
        /// This is test method for testing the contrroller
        /// </summary>
        /// <returns></returns>
        
        [HttpGet("getcurrentsitetitle")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        public IActionResult GetCurrentSiteTitle()
        {
            var termStoreViewModel1 = new TermStoreViewModel()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Disney",
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "Site Collection - microsoft.sharepoint.com-teams-mcuisite",
                    TermSetName = "Clients",
                    CustomPropertyName = "ClientURL"
                }
            };
            
            string siteName = string.Empty;
            spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
           
            siteName = taxonomyRepository.GetCurrentSiteName(termStoreViewModel1.Client);
            var success = new 
            {
                Title = siteName
            };
            return matterCenterServiceFunctions.ServiceResponse(success, (int)HttpStatusCode.OK);
        }


        /// <summary>
        /// This is test method for testing the contrroller
        /// </summary>
        /// <returns></returns>
        [HttpGet("getcurrentsitetitlev1")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        public IActionResult TestWebApi()
        {
            
            var success = new
            {
                Title = "New Title"
            };
            return matterCenterServiceFunctions.ServiceResponse(success, (int)HttpStatusCode.OK);
        }
    }
}
