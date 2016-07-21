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
        /// <param name="spoAuthorization"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        public MatterController(IOptionsMonitor<ErrorSettings> errorSettings,
            IOptionsMonitor<MatterSettings> matterSettings,
            
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IMatterRepository matterRepositoy,
            ICustomLogger customLogger, IOptionsMonitor<LogTables> logTables,
            IValidationFunctions validationFunctions,
            IEditFunctions editFunctions,
            IMatterProvision matterProvision
            )
        {
            this.errorSettings = errorSettings.CurrentValue;
            this.matterSettings = matterSettings.CurrentValue;            
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.matterRepositoy = matterRepositoy;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
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
        [SwaggerResponse(HttpStatusCode.OK)]             
        public async Task<IActionResult> GetPin([FromBody]Client client)
        {
            try
            {
                //Get the authorization token from the Request header
                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                
                if (client == null)
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
                var pinResponseVM = await matterRepositoy.GetPinnedRecordsAsync(client);

                //if (pinResponseVM != null && pinResponseVM.TotalRows == 0)
                //{
                //    errorResponse = new ErrorResponse()
                //    {
                //        Message = pinResponseVM.NoPinnedMessage,
                //        ErrorCode = ((int)HttpStatusCode.NotFound).ToString(),
                //        Description = "No resource found for your search criteria"
                //    };
                //    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                //}
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
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("getmattercounts")]
        [SwaggerResponse(HttpStatusCode.OK)]
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
                int pinnedMatterCounts = await matterProvision.GetPinnedCounts(searchRequestVM.Client);
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
        [SwaggerResponse(HttpStatusCode.OK)]           
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
        [SwaggerResponse(HttpStatusCode.OK)]         
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
        [SwaggerResponse(HttpStatusCode.OK)]       
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
                throw;
            }
        }

        

        [HttpPost("getfolderhierarchy")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        /// <summary>
        /// unpin the matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
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

        

        
        [HttpPost("getstampedproperties")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        /// <summary>
        /// Method saves matter configurations
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
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

        [HttpPost("savconfigurations")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        /// <summary>
        /// Method saves default matter configurations from the settings page. When the user select a client and these
        /// default configurations will be loaded by default for that client
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public IActionResult SavConfigurations([FromBody]SaveConfigurationsVM saveConfigurationsVM)
        {
            try
            {
                
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (string.IsNullOrWhiteSpace(saveConfigurationsVM.SiteCollectionPath) && saveConfigurationsVM.MatterConfigurations == null)
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
                var response = matterProvision.SavConfigurations(saveConfigurationsVM);
                return matterCenterServiceFunctions.ServiceResponse(response, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        [HttpPost("getconfigurations")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        /// <summary>
        /// get users
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
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
        #endregion

        #region Matter Provision
        /// <summary>
        /// This method will check whether a matter already exists with a given name
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        [HttpPost("checkmatterexists")]
        [SwaggerResponse(HttpStatusCode.OK)]        
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
        [SwaggerResponse(HttpStatusCode.OK)]        
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
        /// This method will update a given matter information/configuration
        /// </summary>
        /// <param name="matterInformation"></param>
        /// <returns></returns>
        [HttpPost("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        
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
        /// Assigns specified content types to the specified matter (document library).
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <returns>true if success else false</returns>
        [HttpPost("deletematter")]
        [SwaggerResponse(HttpStatusCode.OK)]
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
        [SwaggerResponse(HttpStatusCode.OK)]
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
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <returns>true if success else false</returns>
        [HttpPost("assigncontenttype")]
        [SwaggerResponse(HttpStatusCode.OK)]        
        public IActionResult AssignContentType([FromBody] MatterMetadata matterMetadata)
        {
            
            ErrorResponse errorResponse = null;
            if (null == matterMetadata && null == matterMetadata.Client && null == matterMetadata.Matter )
            {                
                errorResponse = new ErrorResponse()
                {
                    Message = errorSettings.MessageNoInputs,
                    ErrorCode = HttpStatusCode.BadRequest.ToString(),
                    Description = "No input data is passed"
                };
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);               
            }

            // For each value in the list of Content Type Names
            // Add that content Type to the Library
            Matter matter = matterMetadata.Matter;
            Client client = matterMetadata.Client;


            //ToDo: This values will come from the client. Once the UI is implemented, 
            //This will be removed
            var managedColumnTerms = new Dictionary<string, ManagedColumn>();
            managedColumnTerms.Add("PracticeGroup", new ManagedColumn() {
                TermName= matterMetadata.PracticeGroupTerm.TermName,
                Id = matterMetadata.PracticeGroupTerm.Id
            });

            managedColumnTerms.Add("AreaOfLaw", new ManagedColumn()
            {
                TermName = matterMetadata.AreaTerm.TermName,
                Id = matterMetadata.AreaTerm.Id
            });

            managedColumnTerms.Add("SubareaOfLaw", new ManagedColumn()
            {
                TermName = matterMetadata.SubareaTerm.TermName,
                Id = matterMetadata.SubareaTerm.Id
            });

            matterMetadata.ManagedColumnTerms = managedColumnTerms;



            var matterInformationVM = new MatterInformationVM()
            {
                Client = client,
                Matter = matter,

            };
            try
            {                
                GenericResponseVM genericResponse = validationFunctions.IsMatterValid(matterInformationVM, int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture), null);
                if (genericResponse != null)
                { 
                    matterProvision.DeleteMatter(matterInformationVM as MatterVM);
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponse.Value,
                        ErrorCode = genericResponse.Code.ToString()                       
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                genericResponse = matterProvision.AssignContentType(matterMetadata);
                if (genericResponse != null && genericResponse.IsError==true)
                {
                    matterProvision.DeleteMatter(matterInformationVM as MatterVM);
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponse.Value,
                        ErrorCode = genericResponse.Code.ToString()
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                ///// SharePoint Specific Exception
                matterProvision.DeleteMatter(matterInformationVM as MatterVM);
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This method will assign user permission to the matter
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        [HttpPost("assignuserpermissions")]
        [SwaggerResponse(HttpStatusCode.OK)]
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
        [SwaggerResponse(HttpStatusCode.OK)]        
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
        [SwaggerResponse(HttpStatusCode.OK)]        
        public IActionResult UpdateMetadata([FromBody]MatterMetdataVM matterMetdata)
        {
            string editMatterValidation = string.Empty;
            var matter = matterMetdata.Matter;
            var client = matterMetdata.Client;

            try
            {
                

                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (matterMetdata.Client == null && matterMetdata.Matter == null &&
                    matterMetdata.MatterDetails == null && matterMetdata.MatterProvisionFlags == null)
                {
                    matterProvision.DeleteMatter(matterMetdata as MatterVM);
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
                MatterInformationVM matterInfo = new MatterInformationVM()
                {
                    Client = matterMetdata.Client,
                    Matter = matterMetdata.Matter,
                    MatterDetails = matterMetdata.MatterDetails
                };
                GenericResponseVM genericResponse = validationFunctions.IsMatterValid(matterInfo,
                    int.Parse(ServiceConstants.ProvisionMatterUpdateMetadataForList),
                    matterMetdata.MatterConfigurations);
                if (genericResponse != null)
                {
                    matterProvision.DeleteMatter(matterMetdata as MatterVM);
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponse.Value,
                        ErrorCode = genericResponse.Code,
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
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
        /// This method will assign user permission to the matter
        /// </summary>
        /// <param name="matterMetadataVM"></param>
        /// <returns></returns>
        [HttpPost("sharematter")]
        [SwaggerResponse(HttpStatusCode.OK)]
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
