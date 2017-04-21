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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using System.Net;
#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Web.Common;
#endregion

namespace Microsoft.Legal.MatterCenter.Web
{
    /// <summary>
    /// Taxonomy Controller will read the term store information related to matter center
    /// </summary>
    [Authorize]
    [Route("api/v1/taxonomy")]
    
    public class TaxonomyController:Controller
    {
        private ErrorSettings errorSettings;        
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private TaxonomySettings taxonomySettings;
        private GeneralSettings generalSettings;
        private ITaxonomyRepository taxonomyRepository;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private IValidationFunctions validationFunctions;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="taxonomySettings"></param>
        /// <param name="generalSettings"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        /// <param name="taxonomyRepository"></param>
        /// <param name="customLogger"></param>
        /// <param name="logTables"></param>
        /// <param name="validationFunctions"></param>
        public TaxonomyController(IOptions<ErrorSettings> errorSettings, 
            IOptions<TaxonomySettings> taxonomySettings, 
            IOptions<GeneralSettings> generalSettings,            
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            ITaxonomyRepository taxonomyRepository, 
            ICustomLogger customLogger, 
            IOptions<LogTables> logTables, 
            IValidationFunctions validationFunctions)
        {
            this.errorSettings = errorSettings.Value;
            this.taxonomySettings = taxonomySettings.Value;
            this.generalSettings = generalSettings.Value;            
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;            
            this.taxonomyRepository = taxonomyRepository;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.validationFunctions = validationFunctions;
        }

        /// <summary>
        /// Gets the hierarchy of terms along with the specific custom properties of each term from term store.
        /// </summary>
        /// <param name="termStoreViewModel">request object which contains information from where we need to generate the taxonomy hierarchy</param>
        /// <returns></returns>
        [HttpPost("gettaxonomy")]
        [Produces(typeof(TermSets))]
        [SwaggerOperation("getTaxonomy")]
         [SwaggerResponse((int)HttpStatusCode.OK, Description = "Returns IActionResult which contains either ClientTermSets or  TermSets either ", Type = typeof(TermSets))]
         
        public async Task<IActionResult> GetTaxonomy([FromBody]TermStoreViewModel termStoreViewModel)
        {
            try
            {
                
                #region Error Checking                
               
                var matterInformation = new MatterInformationVM()
                {
                    Client = new Client()
                    {
                        Url = termStoreViewModel.Client.Url
                    }
                };
                var genericResponseVM = validationFunctions.IsMatterValid(matterInformation, 0, null); 
                if (genericResponseVM != null)
                {
                    genericResponseVM.Description = $"Error occurred while getting the taxonomy data";
                    return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.BadRequest);
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

                ServiceUtility.RedisCacheHostName = generalSettings.RedisCacheHostName;
                cacheValue = ServiceUtility.GetDataFromAzureRedisCache(key);
                //cacheValue = "";
                TaxonomyResponseVM taxonomyRepositoryVM = null;
                if (String.IsNullOrEmpty(cacheValue))
                {
                    taxonomyRepositoryVM = await taxonomyRepository.GetTaxonomyHierarchyAsync(termStoreViewModel);
                    if (termStoreViewModel.TermStoreDetails.TermSetName == taxonomySettings.PracticeGroupTermSetName && taxonomyRepositoryVM.TermSets!=null)
                    {                        
                        ServiceUtility.SetDataIntoAzureRedisCache<string>(key, taxonomyRepositoryVM.TermSets);
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
                        var pgTermSets = JsonConvert.DeserializeObject<string>(cacheValue);
                        if (pgTermSets == null)
                        {

                             genericResponseVM = new GenericResponseVM()
                            {
                                Value = errorSettings.MessageNoResult,
                                Code = "404",
                                Description = "No data is present for the given passed input"
                            };
                            return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.NotFound);
                        }
                        return matterCenterServiceFunctions.ServiceResponse(pgTermSets, (int)HttpStatusCode.OK);
                    }
                    if (termStoreViewModel.TermStoreDetails.TermSetName == taxonomySettings.ClientTermSetName)
                    {
                        var clientTermSets = JsonConvert.DeserializeObject<ClientTermSets>(cacheValue);
                        if (clientTermSets == null)
                        {
                            genericResponseVM = new GenericResponseVM()
                            {
                                Value = errorSettings.MessageNoResult,
                                Code = HttpStatusCode.NotFound.ToString(),
                                Description = "No data is present for the given passed input"
                            };
                            return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.NotFound);
                        }
                        return matterCenterServiceFunctions.ServiceResponse(clientTermSets, (int)HttpStatusCode.OK);
                    }
                }
                //If all the above condition fails, return validation error object
                 genericResponseVM = new GenericResponseVM()
                {
                     Value = errorSettings.MessageNoResult,
                     Code = HttpStatusCode.NotFound.ToString(),
                     Description = "No data is present for the given passed input"
                 };
                return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.BadRequest);
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                var errorResponse = customLogger.GenerateErrorResponse(ex);
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.InternalServerError);
            }            
        }        
    }
}
