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

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
#endregion
namespace Microsoft.Legal.MatterCenter.Service
{
    /// <summary>
    /// Matter Controller class deals with matter provisioning, finding matter, pinning matter, unpinning the matterm, updating the matter
    /// </summary>
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
            ICustomLogger customLogger, IOptions<LogTables> logTables
            )
        {
            this.errorSettings = errorSettings.Value;
            this.matterSettings = matterSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.matterRepositoy = matterRepositoy;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
        }


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
                #region Error Checking

                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
                //if the token is not valid, immediately return no authorization error to the user
                if (errorResponse != null && !errorResponse.IsTokenValid)
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
                }

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

                var searchObject = searchRequestVM.SearchObject;
                // Encode all fields which are coming from js
                SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
                // Encode Search Term
                searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                    WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES, ServiceConstants.DOUBLE_QUOTE) : string.Empty;

                var searchResultsVM = await matterRepositoy.GetMattersAsync(searchRequestVM);
                return matterCenterServiceFunctions.ServiceResponse(searchResultsVM, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        [HttpPost("create")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// Creates a matter
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> Create([FromBody]TermStoreViewModel termStoreViewModel)
        {
            return null;
        }

        [HttpPost("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        /// <summary>
        /// Update 
        /// </summary>        
        /// <param name="client">Client object containing Client data</param>
        /// <param name="details">Term Store object containing Term store data</param>
        /// <returns>Returns JSON object to the client</returns>        ///
        public async Task<IActionResult> Update(string matterId, [FromBody]TermStoreViewModel termStoreViewModel)
        {
            return null;
        }

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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                if (pinResponseVM != null && pinResponseVM.TotalCount == 0)
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
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
                #region Error Checking
                string authorization = HttpContext.Request.Headers["Authorization"];
                ErrorResponse errorResponse = null;
                errorResponse = spoAuthorization.ValidateClientToken(authorization);
                //if the token is not valid, immediately return no authorization error to the user
                if (errorResponse != null && !errorResponse.IsTokenValid)
                {
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
                }
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
    }
}
