using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Web.Common;
using Newtonsoft.Json;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/v1/taxonomy")]
    public class UserController : Controller
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
        public UserController(IOptions<ErrorSettings> errorSettings,
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
    }
}
