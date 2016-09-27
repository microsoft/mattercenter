// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-lapedd
// Created          : 04-09-2016
//
// ***********************************************************************
// <copyright file="MatterController.cs" company="Microsoft">
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
using System.Globalization;
#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Legal.MatterCenter.Web.Common;
using System.Collections.Generic;
#endregion
namespace Microsoft.Legal.MatterCenter.Service
{
    /// <summary>
    /// Matter Controller class deals with matter provisioning, finding matter, pinning matter, unpinning the matterm, updating the matter
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/v1/matter")]
    public class MatterController : Controller
    {
        private ErrorSettings errorSettings;
        
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private MatterSettings matterSettings;
        private IMatterRepository matterRepositoy;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private IValidationFunctions validationFunctions;
        private IEditFunctions editFunctions;
        private IMatterProvision matterProvision;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="matterSettings"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        /// <param name="matterRepositoy"></param>
        /// <param name="customLogger"></param>
        /// <param name="logTables"></param>
        /// <param name="validationFunctions"></param>
        /// <param name="editFunctions"></param>
        /// <param name="matterProvision"></param>
        public MatterController(IOptions<ErrorSettings> errorSettings,
            IOptions<MatterSettings> matterSettings,
            
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IMatterRepository matterRepositoy,
            ICustomLogger customLogger, IOptions<LogTables> logTables,
            IValidationFunctions validationFunctions,
            IEditFunctions editFunctions,
            IMatterProvision matterProvision
            )
        {
            this.errorSettings = errorSettings.Value;
            this.matterSettings = matterSettings.Value;            
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.matterRepositoy = matterRepositoy;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.validationFunctions = validationFunctions;
            this.editFunctions = editFunctions;
            this.matterProvision = matterProvision;
        }

        #region Pin and UnPin
        /// <summary>
        /// Get all pinned matters which are pinned by the user
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("getpinned")]
        [Produces(typeof(SearchResponseVM))]
        [SwaggerOperation("getpinned")]
        [SwaggerResponse(HttpStatusCode.OK, 
            Description = "Returns Asynchronouns IActionResult which contains list pinned matters which are pinned by the user", 
            Type = typeof(SearchResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public async Task<IActionResult> GetPin([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                
                if (searchRequestVM == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed to fetch the pinned matters"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var pinResponseVM = await matterRepositoy.GetPinnedRecordsAsync(searchRequestVM);                
                return matterCenterServiceFunctions.ServiceResponse(pinResponseVM.MatterDataList, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Get all counts for all matters, my matters and pinned matters
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        [HttpPost("getmattercounts")]        
        [SwaggerOperation("getmattercounts")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns Asynchronouns IActionResult anonymous object  which contains count of all matters, pinned matters and my matters")]
        [SwaggerResponseRemoveDefaults]
        public async Task<IActionResult> GetMatterCounts([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {
                //Get the authorization token from the Request header                
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
                int allMatterCounts = await matterProvision.GetAllCounts(searchRequestVM);
                int myMatterCounts = await matterProvision.GetMyCounts(searchRequestVM);
                int pinnedMatterCounts = await matterProvision.GetPinnedCounts(searchRequestVM);
                var matterCounts = new
                {
                    AllMatterCounts = allMatterCounts,
                    MyMatterCounts = myMatterCounts,
                    PinnedMatterCounts = pinnedMatterCounts,
                };
                return matterCenterServiceFunctions.ServiceResponse(matterCounts, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// pin the matter
        /// </summary>
        /// <param name="pinRequestMatterVM"></param>
        /// <returns></returns>
        [HttpPost("pin")]
        [SwaggerOperation("pin")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns Asynchronouns IActionResult anonymous object  whether the matter is pinned or not")]
        [SwaggerResponseRemoveDefaults]
        public async Task<IActionResult> Pin([FromBody]PinRequestMatterVM pinRequestMatterVM)
        {
            try
            {
                
                #region Error Checking                
                ErrorResponse errorResponse = null;                
                if (pinRequestMatterVM == null && pinRequestMatterVM.Client == null && pinRequestMatterVM.MatterData == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed to pin a matter"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion
                var isMatterPinned = await matterRepositoy.PinRecordAsync<PinRequestMatterVM>(pinRequestMatterVM);
                var matterPinned = new
                {
                    IsMatterPinned = isMatterPinned
                };
                return matterCenterServiceFunctions.ServiceResponse(matterPinned, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Unpin the matter
        /// </summary>
        /// <param name="pinRequestMatterVM"></param>
        /// <returns></returns>
        [HttpPost("unpin")]
        [SwaggerOperation("unpin")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns Asynchronouns IActionResult anonymous object  whether the matter is unpinned or not")]
        [SwaggerResponseRemoveDefaults]
        public async Task<IActionResult> UnPin([FromBody]PinRequestMatterVM pinRequestMatterVM)
        {
            try
            {
                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                //if the token is not valid, immediately return no authorization error to the user
                if (errorResponse != null && !errorResponse.IsTokenValid)
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
                }
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
                var isMatterUnPinned = await matterRepositoy.UnPinRecordAsync<PinRequestMatterVM>(pinRequestMatterVM);
                var matterUnPinned = new
                {
                    IsMatterUnPinned = isMatterUnPinned
                };
                return matterCenterServiceFunctions.ServiceResponse(matterUnPinned, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
        #endregion

        #region Search Methods

        /// <summary>
        /// Gets the matters based on search criteria.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns>searchResponseVM</returns>
        [HttpPost("get")]
        [Produces(typeof(SearchResponseVM))]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns Asynchronouns IActionResult of all matters where the user has got permissions", Type = typeof(SearchResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public async Task<IActionResult> Get([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {
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
                var searchResultsVM = await matterProvision.GetMatters(searchRequestVM);
                return matterCenterServiceFunctions.ServiceResponse(searchResultsVM.MatterDataList, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                var errorResponse = customLogger.GenerateErrorResponse(ex);
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                //throw;
            }
        }


        /// <summary>
        /// Get all folders that are there in a particular matter document library
        /// </summary>
        /// <param name="matterData"></param>
        /// <returns></returns>
        [HttpPost("getfolderhierarchy")]
        [Produces(typeof(List<FolderData>))]
        [SwaggerOperation("getFolderHierarchy")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns Asynchronouns IActionResult of all folders that are there in a particular document library", Type = typeof(List<FolderData>))]
        [SwaggerResponseRemoveDefaults]        
        public async Task<IActionResult> GetFolderHierachy([FromBody]MatterData matterData)
        {
            try
            {
                
                #region Error Checking                
                ErrorResponse errorResponse = null;                
                if (matterData == null && string.IsNullOrWhiteSpace(matterData.MatterUrl) && string.IsNullOrWhiteSpace(matterData.MatterName))
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
                var folderHierarchy = await matterRepositoy.GetFolderHierarchyAsync(matterData);
                var genericResponse = new {
                    foldersList = folderHierarchy
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
        /// Get all the stamped properties associated to a particular matter
        /// </summary>
        /// <param name="matterVM"></param>
        /// <returns></returns>
        [HttpPost("getstampedproperties")]
        [Produces(typeof(MatterStampedDetails))]
        [SwaggerOperation("getStampedProperties")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns IActionResult of all stamped properties that are associated to a matter", 
            Type = typeof(MatterStampedDetails))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult GetStampedProperties([FromBody]MatterVM matterVM)
        {
            try
            {
                

                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (matterVM == null && matterVM.Client == null && matterVM.Matter != null && string.IsNullOrWhiteSpace(matterVM.Matter.Name))
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                #endregion
                //ToDo: Need to concert this method to async
                var matterStampedProperties = matterProvision.GetStampedProperties(matterVM);
                //ToDo: Need to do nulk check on  matterStampedProperties
                return matterCenterServiceFunctions.ServiceResponse(matterStampedProperties, (int)HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
        #endregion

        #region Configurations       

        /// <summary>
        /// get configurations for a selected client
        /// </summary>
        /// <param name="siteCollectionPath"></param>
        /// <returns></returns>
        [HttpPost("getconfigurations")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("getConfigurations")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns IActionResult of generic reposne which contains configurations for a selected client",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]        
        public async Task<IActionResult> GetConfigurations([FromBody]string siteCollectionPath)
        {
            try
            {                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (string.IsNullOrWhiteSpace(siteCollectionPath))
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                #endregion
                GenericResponseVM genericResponseVM = await matterRepositoy.GetConfigurationsAsync(siteCollectionPath);
                return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// Method saves default matter configurations from the settings page. When the user select a client and these
        /// default configurations will be loaded by default for that client
        /// </summary>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>        
        [HttpPost("saveconfigurations")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("saveConfigurations")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns IActionResult of generic reposne which contains whether configuration for the matter is saved or not",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult SaveConfigurations([FromBody]MatterConfigurations matterConfigurations)
        {
            try
            {
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                if (matterConfigurations==null &&  string.IsNullOrWhiteSpace(matterConfigurations.ClientUrl))
                {
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.BadRequest);
                }
                #endregion
                GenericResponseVM genericResponseVM = matterProvision.SaveConfigurations(matterConfigurations);
                return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        #endregion

        #region Matter Provision

        /// <summary>
        /// This method will check whether the current login user can create a matter or not
        /// This method will check whether the user is present in the sharepoint group called "Provision Matter User"
        /// If the user is not present in the group, then "Create Matter" link should not be visible to the user
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("cancreate")]        
        [SwaggerOperation("canCreate")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns IActionResult of type of bool checks whether the login user can create a matter or not")]
        [SwaggerResponseRemoveDefaults]
        public IActionResult CanCreateMatter([FromBody]Client client)
        {
            GenericResponseVM genericResponse = null;
            if (null == client && null != client.Url)
            {
                genericResponse = new GenericResponseVM()
                {
                    Value = errorSettings.MessageNoInputs,
                    Code = "",
                    IsError = true
                };
                
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
            }
            try
            {
                var canCreateMatter = matterRepositoy.CanCreateMatter(client);

                var canLoginUserCreateMatter = new
                {
                    CanCreateMatter = true
                };

                return matterCenterServiceFunctions.ServiceResponse(canLoginUserCreateMatter, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This method will check whether a matter already exists with a given name
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        [HttpPost("checkmatterexists")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("checkMatterExists")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Returns IActionResult of generic resposne which checks whether a matter with the given name already exists or not",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult CheckMatterExists([FromBody]MatterMetdataVM matterMetadataVM)
        {
            
            GenericResponseVM genericResponse = ServiceUtility.GenericResponse(matterSettings.DeleteMatterCode, ServiceConstants.TRUE);
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            var matterConfiguration = matterMetadataVM.MatterConfigurations;
            ErrorResponse errorResponse = null;
            if (null == client && null == matter && string.IsNullOrWhiteSpace(client.Url))
            {
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoInputs,
                    ErrorCode = "",
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }
            var matterInformation = new MatterInformationVM()
            {
                Client = client,
                Matter = matter
            };
            genericResponse = validationFunctions.IsMatterValid(matterInformation, int.Parse(ServiceConstants.PROVISION_MATTER_CHECK_MATTER_EXISTS, 
                CultureInfo.InvariantCulture), null);
            if(genericResponse!=null)
            {
                errorResponse = new ErrorResponse()
                {
                    Message = genericResponse.Value,
                    ErrorCode = genericResponse.Code,                                                
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }
                
            try
            {
                if (!matterMetadataVM.HasErrorOccurred)
                {
                    genericResponse = matterProvision.CheckMatterExists(matterMetadataVM);
                    if (genericResponse != null)
                    {
                        errorResponse = new ErrorResponse()
                        {
                            Message = genericResponse.Value,
                            ErrorCode = genericResponse.Code,
                        };
                        return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                    }
                    else
                    {
                        genericResponse = ServiceUtility.GenericResponse(ServiceConstants.SUCCESS, ServiceConstants.TRUE);
                        return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                    }
                }
                else
                {
                    genericResponse = matterProvision.DeleteMatter(matterMetadataVM as MatterVM);
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponse.Value,
                        ErrorCode = genericResponse.Code,
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }                
        }

        /// <summary>
        /// This method will check whether a given security group already exists or not
        /// </summary>
        /// <param name="matterInformationVM"></param>
        /// <returns></returns>
        [HttpPost("checksecuritygroupexists")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("checksecuritygroupexists")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "This method will check whether a given security group already exists or not",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult CheckSecurityGroupExists([FromBody]MatterInformationVM matterInformationVM)
        {
            
            GenericResponseVM genericResponse = null;
            var client = matterInformationVM.Client;
            var matter = matterInformationVM.Matter;
            
            ErrorResponse errorResponse = null;
            if (null == client && null == matter && null != client.Url)
            {
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoInputs,
                    ErrorCode = "",
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }            

            try
            {
                if (0 == matter.AssignUserEmails.Count)
                {                    
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.IncorrectInputUserNamesMessage,
                        ErrorCode = errorSettings.IncorrectInputUserNamesCode,
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }

                genericResponse = matterProvision.CheckSecurityGroupExists(matterInformationVM);
                if(genericResponse != null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponse.Value,
                        ErrorCode = genericResponse.Code,
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                genericResponse = ServiceUtility.GenericResponse(ServiceConstants.SUCCESS, ServiceConstants.TRUE);
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This method will update a given matter information/configuration such as user matter roles, new users to the matter etc
        /// </summary>
        /// <param name="matterInformation"></param>
        /// <returns></returns>
        [HttpPost("update")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "This method will update a given matter information/configuration such as user matter roles, new users to the matter etc",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult Update([FromBody]MatterInformationVM matterInformation)
        {
            string editMatterValidation = string.Empty;
            var matter = matterInformation.Matter;
            var client = matterInformation.Client;
            var userid = matterInformation.UserIds;
            try
            {
                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (matterInformation.Client == null && matterInformation.Matter == null && matterInformation.MatterDetails == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                #endregion

                #region Validations
                GenericResponseVM validationResponse = validationFunctions.IsMatterValid(matterInformation, int.Parse(ServiceConstants.EditMatterPermission), null);
                if (validationResponse != null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = validationResponse.Value,
                        ErrorCode = validationResponse.Code,
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }

                if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                {
                    if (matter.AssignUserNames.Count == 0)
                    {
                        errorResponse = new ErrorResponse()
                        {
                            Message = errorSettings.IncorrectInputUserNamesMessage,
                            ErrorCode = errorSettings.IncorrectInputUserNamesCode,
                        };
                        return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        if (Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture))
                        {
                            validationResponse = editFunctions.CheckSecurityGroupInTeamMembers(client, matter, userid);
                            if (validationResponse != null)
                            {
                                errorResponse = new ErrorResponse()
                                {
                                    Message = validationResponse.Value,
                                    ErrorCode = validationResponse.Code,
                                };
                                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                            }
                        }
                    }
                }
                else
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.IncorrectInputConflictIdentifiedMessage,
                        ErrorCode = errorSettings.IncorrectInputConflictIdentifiedCode,
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                #endregion

                #region Upadte Matter
                GenericResponseVM genericResponse = matterProvision.UpdateMatter(matterInformation);
                if (genericResponse == null)
                {
                    var result = new GenericResponseVM()
                    {
                        Code = "200",
                        Value = "Update Success"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(result, (int)HttpStatusCode.OK);
                }
                else
                {
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.NotModified);
                }

                #endregion

            }
            catch (Exception ex)
            {
                MatterRevertList matterRevertListObject = new MatterRevertList()
                {
                    MatterLibrary = matterInformation.Matter.Name,
                    MatterOneNoteLibrary = matterInformation.Matter.Name + matterSettings.OneNoteLibrarySuffix,
                    MatterCalendar = matterInformation.Matter.Name + matterSettings.CalendarNameSuffix,
                    MatterTask = matterInformation.Matter.Name + matterSettings.TaskNameSuffix,
                    MatterSitePages = matterSettings.MatterLandingPageRepositoryName
                };
                //editFunctions.RevertMatterUpdates(client, matter, matterRevertListObject, loggedInUserName, userPermissionOnLibrary, listItemId, isEditMode);
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            finally
            {

            }
        }

        /// <summary>
        /// This method will delete a given matter and all its associated assets
        /// </summary>
        /// <param name="matterVM"></param>
        /// <returns></returns>
        [HttpPost("deletematter")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("deleteMatter")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "This method will delete a given matter and all its associated assets",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult Delete([FromBody] MatterVM matterVM)
        {            
            ErrorResponse errorResponse = null;
            if (null == matterVM && null == matterVM.Client && null == matterVM.Matter && string.IsNullOrWhiteSpace(matterVM.Client.Url) && string.IsNullOrWhiteSpace(matterVM.Matter.Name))
            {
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoInputs,
                    ErrorCode = HttpStatusCode.BadRequest.ToString(),
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }

            GenericResponseVM genericResponse = matterProvision.DeleteMatter(matterVM);
            return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Create a new matter
        /// </summary>
        /// <param name="matterMetdataVM"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("createMatter")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "This method will create a new matter",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult Create([FromBody] MatterMetdataVM matterMetdataVM)
        {
            ErrorResponse errorResponse = null;
            GenericResponseVM genericResponseVM = null;
            
            if (null == matterMetdataVM && null == matterMetdataVM.Client && null == matterMetdataVM.Matter && string.IsNullOrWhiteSpace(matterMetdataVM.Client.Url))
            {
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoInputs,
                    ErrorCode = HttpStatusCode.BadRequest.ToString(),
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }
            try
            {
                genericResponseVM = matterProvision.CreateMatter(matterMetdataVM);
                if (genericResponseVM != null && genericResponseVM.IsError == true)
                {
                    //Matter not created successfully
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponseVM.Value,
                        ErrorCode = genericResponseVM.Code,
                        Description = "Matter page not created successfully"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                //Matter page created successfully
                genericResponseVM = new GenericResponseVM
                {
                    Code = HttpStatusCode.OK.ToString(),
                    Value = "Matter page created successfully"
                };
                return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                matterProvision.DeleteMatter(matterMetdataVM as MatterVM);
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Assigns specified content types to the specified matter (document library).
        /// </summary>
        /// <param name="matterMetadata"></param>
        /// <returns></returns>
        [HttpPost("assigncontenttype")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("assignContenttype")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Assigns specified content types to the specified matter (document library).",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult AssignContentType([FromBody] MatterMetadata matterMetadata)
        {

            GenericResponseVM genericResponse = null;
            if (null == matterMetadata && null == matterMetadata.Client && null == matterMetadata.Matter && 
                matterMetadata.ManagedColumnTerms==null)
            {
                genericResponse = new GenericResponseVM()
                {
                    Value = errorSettings.MessageNoInputs,
                    Code = HttpStatusCode.BadRequest.ToString(),
                    IsError = true
                };
                
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);               
            }

            // For each value in the list of Content Type Names
            // Add that content Type to the Library
            Matter matter = matterMetadata.Matter;
            Client client = matterMetadata.Client;            

            var matterInformationVM = new MatterInformationVM()
            {
                Client = client,
                Matter = matter,
            };
            try
            {                
                genericResponse = validationFunctions.IsMatterValid(matterInformationVM, int.Parse(ServiceConstants.ProvisionMatterAssignContentType, 
                    CultureInfo.InvariantCulture), null);
                if (genericResponse != null)
                { 
                    matterProvision.DeleteMatter(matterInformationVM as MatterVM);
                    
                    genericResponse = new GenericResponseVM()
                    {
                        Value = genericResponse.Value,
                        Code = genericResponse.Code.ToString(),
                        IsError = true
                    };

                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                genericResponse = matterProvision.AssignContentType(matterMetadata);
                if (genericResponse != null && genericResponse.IsError==true)
                {
                    matterProvision.DeleteMatter(matterInformationVM as MatterVM);
                    genericResponse = new GenericResponseVM()
                    {
                        Value = genericResponse.Value,
                        Code = genericResponse.Code.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                ///// SharePoint Specific Exception
                matterProvision.DeleteMatter(matterInformationVM as MatterVM);
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                var errorResponse = customLogger.GenerateErrorResponse(exception);
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                //throw;
            }
        }

        /// <summary>
        /// This method will assign user permission to the matter
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        [HttpPost("assignuserpermissions")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("assignUserPermissions")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "This method will assign user permission to the matter",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult AssignUserPermissions([FromBody] MatterMetdataVM matterMetadataVM)
        {
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            try
            {
                
                var matterConfigurations = matterMetadataVM.MatterConfigurations;
                ErrorResponse errorResponse = null;
                if (null == client && null == matter && null == client.Url && null == matterConfigurations)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }

                var genericResponseVM = matterProvision.AssignUserPermissions(matterMetadataVM);
                if (genericResponseVM != null && genericResponseVM.IsError == true)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponseVM.Value,
                        ErrorCode = genericResponseVM.Code,
                        Description = ""
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                var assignPermissions = new
                {
                    ReturnValue = true
                };
                return matterCenterServiceFunctions.ServiceResponse(assignPermissions, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                matterProvision.DeleteMatter(matterMetadataVM as MatterVM);
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// Creates matter landing page. If there is any error in creating the landing page, the whole matter will get deleted along with document libraries
        /// </summary>
        /// <param name="matterMetdataVM"></param>
        /// <returns></returns>
        [HttpPost("createlandingpage")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("createLandingPage")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Creates matter landing page. If there is any error in creating the landing page, the whole matter will get deleted along with document libraries",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult CreateLandingPage([FromBody] MatterMetdataVM matterMetdataVM)
        {
            
            ErrorResponse errorResponse = null;
            GenericResponseVM genericResponseVM = null;
            //No valid input
            if (null == matterMetdataVM && null == matterMetdataVM.Client && null == matterMetdataVM.Matter && 
                string.IsNullOrWhiteSpace(matterMetdataVM.Client.Url))
            {
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoInputs,
                    ErrorCode = HttpStatusCode.BadRequest.ToString(),
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
            }           
            try
            {
                genericResponseVM = matterProvision.CreateMatterLandingPage(matterMetdataVM);
                if(genericResponseVM!=null)
                {
                    matterProvision.DeleteMatter(matterMetdataVM as MatterVM);
                    //Matter landing page not created successfully
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponseVM.Value,
                        ErrorCode = genericResponseVM.Code,
                        Description = "Matter landing page not created successfully"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                //Matter landing page created successfully
                genericResponseVM = new GenericResponseVM {
                    Code= HttpStatusCode.OK.ToString(),
                    Value="Matter landing page created successfully" 
                };
                return matterCenterServiceFunctions.ServiceResponse(genericResponseVM, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                //If there is error in creating matter landing page, delete all the information related to this matter
                matterProvision.DeleteMatter(matterMetdataVM as MatterVM);
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Updates matter metadata - Stamps properties to the created matter.
        /// </summary>
        /// <param name="matterMetdata"></param>
        /// <returns></returns>
        [HttpPost("updatemetadata")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("updateMetaData")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Updates matter metadata - Stamps properties to the created matter.",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult UpdateMetadata([FromBody]MatterMetdataVM matterMetdata)
        {
            string editMatterValidation = string.Empty;
            var matter = matterMetdata.Matter;
            var client = matterMetdata.Client;

            try
            {
                #region Error Checking                
                GenericResponseVM genericResponse = null;
                if (matterMetdata.Client == null && matterMetdata.Matter == null &&
                    matterMetdata.MatterDetails == null && matterMetdata.MatterProvisionFlags == null && 
                    matterMetdata.MatterDetails.ManagedColumnTerms==null)
                {
                    matterProvision.DeleteMatter(matterMetdata as MatterVM);
                    genericResponse = new GenericResponseVM()
                    {
                        Value = errorSettings.MessageNoInputs,
                        Code = HttpStatusCode.BadRequest.ToString(),
                        IsError = true
                    };
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion

                #region Validations
                MatterInformationVM matterInfo = new MatterInformationVM()
                {
                    Client = matterMetdata.Client,
                    Matter = matterMetdata.Matter,
                    MatterDetails = matterMetdata.MatterDetails
                };
                genericResponse = validationFunctions.IsMatterValid(matterInfo,
                    int.Parse(ServiceConstants.ProvisionMatterUpdateMetadataForList),
                    matterMetdata.MatterConfigurations);
                if (genericResponse != null)
                {
                    matterProvision.DeleteMatter(matterMetdata as MatterVM);
                    genericResponse = new GenericResponseVM()
                    {
                        Value = genericResponse.Value,
                        Code = genericResponse.Code,
                        IsError = true
                    };                    
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                #endregion   

                try
                {                   

                    genericResponse = matterProvision.UpdateMatterMetadata(matterMetdata);
                    if (genericResponse == null)
                    {
                        genericResponse = new GenericResponseVM()
                        {
                            Code = "200",
                            Value = "Update Success"
                        };
                        
                    }
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    matterProvision.DeleteMatter(matterMetdata as MatterVM);
                    customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
            catch (Exception ex)
            {
                matterProvision.DeleteMatter(matterMetdata as MatterVM);
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

        }

        /// <summary>
        /// This method will allow the matter to be shared with external user
        /// </summary>
        /// <param name="matterInformation"></param>
        /// <returns></returns>
        [HttpPost("sharematter")]
        [Produces(typeof(GenericResponseVM))]
        [SwaggerOperation("shareMatter")]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "This method will allow the matter to be shared with external user",
            Type = typeof(GenericResponseVM))]
        [SwaggerResponseRemoveDefaults]
        public IActionResult ShareMatter([FromBody] MatterInformationVM matterInformation)
        {
            var client = matterInformation.Client;            
            try
            {
                
                
                ErrorResponse errorResponse = null;
                if (matterInformation == null && matterInformation.Client==null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No input data is passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }

                var genericResponseVM = matterProvision.ShareMatterToExternalUser(matterInformation);
                if (genericResponseVM != null && genericResponseVM.IsError == true)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponseVM.Value,
                        ErrorCode = genericResponseVM.Code,
                        Description = ""
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                var assignPermissions = new
                {
                    ReturnValue = true
                };
                return matterCenterServiceFunctions.ServiceResponse(assignPermissions, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        #endregion
    }
}
