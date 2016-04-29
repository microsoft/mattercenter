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
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.Legal.MatterCenter.Web.Common;
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
        private ISPOAuthorization spoAuthorization;
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
        public MatterController(IOptions<ErrorSettings> errorSettings,
            IOptions<MatterSettings> matterSettings,
            ISPOAuthorization spoAuthorization,
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
            this.spoAuthorization = spoAuthorization;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.matterRepositoy = matterRepositoy;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.validationFunctions = validationFunctions;
            this.editFunctions = editFunctions;
            this.matterProvision = matterProvision;
        }


        

        

        #region Pin and UnPin

        [HttpPost("pin")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// pin the matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> Pin([FromBody]PinRequestMatterVM pinRequestMatterVM)
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
                if (pinRequestMatterVM == null && pinRequestMatterVM.Client == null && pinRequestMatterVM.MatterData == null)
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

        [HttpPost("unpin")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// unpin the matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> UnPin([FromBody]PinRequestMatterVM pinRequestMatterVM)
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
                if (pinRequestMatterVM == null && pinRequestMatterVM.Client == null && pinRequestMatterVM.MatterData == null)
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
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromBody]SearchRequestVM searchRequestVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];

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
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
                }
                #endregion                
                var searchResultsVM = await matterProvision.GetMatters(searchRequestVM);
                return matterCenterServiceFunctions.ServiceResponse(searchResultsVM, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        [HttpPost("getpinned")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// unpin the matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> GetPin([FromBody]Client client)
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
                if (client == null)
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

                var pinResponseVM = await matterRepositoy.GetPinnedRecordsAsync(client);
                if (pinResponseVM != null && pinResponseVM.TotalRows == 0)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = pinResponseVM.NoPinnedMessage,
                        ErrorCode = ((int)HttpStatusCode.NotFound).ToString(),
                        Description = "No resource found for your search criteria"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
                }
                return matterCenterServiceFunctions.ServiceResponse(pinResponseVM, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        [HttpPost("getfolderhierarchy")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
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
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;
                //if the token is not valid, immediately return no authorization error to the user
                if (errorResponse != null && !errorResponse.IsTokenValid)
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
                }
                if (matterData == null && string.IsNullOrWhiteSpace(matterData.MatterUrl) && string.IsNullOrWhiteSpace(matterData.MatterName))
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
                var folderHierarchy = await matterRepositoy.GetFolderHierarchyAsync(matterData);
                return matterCenterServiceFunctions.ServiceResponse(folderHierarchy, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        [HttpPost("getroles")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// unpin the matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> GetRoles([FromBody]Client client)
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
                if (client == null)
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
                IList<Role> roles = new List<Role>();
                string result = ServiceUtility.GetDataFromAzureRedisCache(ServiceConstants.CACHE_ROLES);
                if (string.IsNullOrEmpty(result))
                {
                    roles = await matterRepositoy.GetRolesAsync(client);
                    ServiceUtility.SetDataIntoAzureRedisCache<IList<Role>>(ServiceConstants.CACHE_ROLES, roles);
                }
                else
                {
                    roles = JsonConvert.DeserializeObject<IList<Role>>(result);
                }
                return matterCenterServiceFunctions.ServiceResponse(roles, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        [HttpPost("getpermissionlevels")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// unpin the matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> GetPermissionLevels([FromBody]Client client)
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
                if (client == null)
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
                IList<Role> roles = new List<Role>();
                roles = await matterRepositoy.GetPermissionLevelsAsync(client);
                return matterCenterServiceFunctions.ServiceResponse(roles, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        [HttpPost("getusers")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// get users
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> GetUsers([FromBody]SearchRequestVM searchRequestVM)
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
                if (searchRequestVM.Client == null)
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
                searchRequestVM.SearchObject.SearchTerm = (!string.IsNullOrWhiteSpace(searchRequestVM.SearchObject.SearchTerm)) ? searchRequestVM.SearchObject.SearchTerm : string.Empty;
                IList<Users> users = await matterRepositoy.GetUsersAsync(searchRequestVM);
                if (users != null && users.Count != 0)
                {
                    return matterCenterServiceFunctions.ServiceResponse(users, (int)HttpStatusCode.OK);
                }
                else
                {
                    Users noResult = new Users()
                    {
                        Name = errorSettings.PeoplePickerNoResults,
                        LogOnName = string.Empty,
                        Email = string.Empty,
                        EntityType = string.Empty
                    };
                    users.Add(noResult);
                    return matterCenterServiceFunctions.ServiceResponse(users, (int)HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        
        [HttpPost("getstampedproperties")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
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
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];

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
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// Method saves matter configurations
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public IActionResult SavConfigurations([FromBody]SaveConfigurationsVM saveConfigurationsVM)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];

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
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
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
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
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
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
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

        [HttpPost("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// Updates matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public IActionResult Update([FromBody]MatterInformationVM matterInformation)
        {
            string editMatterValidation = string.Empty;
            var matter = matterInformation.Matter;
            var client = matterInformation.Client;
            var userid = matterInformation.UserIds;
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
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



        [HttpPost("updatemetadata")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// Updates matter metadata - Stamps properties to the created matter.
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public IActionResult UpdateMetadata([FromBody]MatterMetdataVM matterMetdata)
        {
            string editMatterValidation = string.Empty;
            var matter = matterMetdata.Matter;
            var client = matterMetdata.Client;

            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;
                if (matterMetdata.Client == null && matterMetdata.Matter == null &&
                    matterMetdata.MatterDetails == null && matterMetdata.MatterProvisionFlags == null)
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
                    matterProvision.DeleteMatter(client, matter);
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
                        var result = new GenericResponseVM()
                        {
                            Code = "200",
                            Value = "Update Success"
                        };
                        return matterCenterServiceFunctions.ServiceResponse(result, (int)HttpStatusCode.OK);
                    }
                    return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.NotModified);
                }
                catch (Exception ex)
                {
                    matterProvision.DeleteMatter(client, matter);
                    customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

        }

        [HttpPost("assignuserpermissions")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IActionResult AssignUserPermissions([FromBody] MatterMetdataVM matterMetadataVM)
        {
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];                
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
                if(genericResponseVM!=null && genericResponseVM.IsError==true)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = genericResponseVM.Value,
                        ErrorCode = genericResponseVM.Code,
                        Description = ""
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
                }
                var assignPermissions = new {
                    ReturnValue = true
                };
                return matterCenterServiceFunctions.ServiceResponse(assignPermissions, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                matterProvision.DeleteMatter(client, matter);
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
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
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
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
                return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.BadRequest);
               
            }

            // For each value in the list of Content Type Names
            // Add that content Type to the Library
            Matter matter = matterMetadata.Matter;
            Client client = matterMetadata.Client;
            try
            {
                var matterInformationVM = new MatterInformationVM()
                {
                    Client = client,
                    Matter = matter,

                };
                GenericResponseVM genericResponse = validationFunctions.IsMatterValid(matterInformationVM, int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture), null);
                if (genericResponse == null)
                {                    
                    matterProvision.DeleteMatter(client, matter);
                }
                genericResponse = matterProvision.AssignContentType(matterMetadata);
                return matterCenterServiceFunctions.ServiceResponse(genericResponse, (int)HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                ///// SharePoint Specific Exception
                matterProvision.DeleteMatter(client, matter);
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
        #endregion
    }
}
