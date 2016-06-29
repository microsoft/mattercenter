using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

#region MatterCenter Namespaces
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Web.Common;
#endregion

namespace Microsoft.Legal.MatterCenter.Web
{
    /// <summary>
    /// This controller will try to get all the user related information such as user roles, user permissions etc
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/v1/user")]    
    public class UserController : Controller
    {
        private ErrorSettings errorSettings;
        private ISPOAuthorization spoAuthorization;
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;        
        private IUserRepository userRepositoy;
        private ICustomLogger customLogger;
        private LogTables logTables;      
        private GeneralSettings generalSettings;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="matterSettings"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        public UserController(IOptionsMonitor<ErrorSettings> errorSettings,           
            ISPOAuthorization spoAuthorization,
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IUserRepository userRepositoy,
            ICustomLogger customLogger, 
            IOptionsMonitor<LogTables> logTables,  
            IOptionsMonitor<GeneralSettings> generalSettings
            )
        {
            this.errorSettings = errorSettings.CurrentValue;            
            this.spoAuthorization = spoAuthorization;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;            
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;             
            this.generalSettings = generalSettings.CurrentValue;
            this.userRepositoy = userRepositoy;
        }

        /// <summary>
        /// Get all the users that are configured for a given client
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        [HttpPost("getusers")]
        [SwaggerResponse(HttpStatusCode.OK)]        
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
                if (searchRequestVM.Client == null && string.IsNullOrWhiteSpace(searchRequestVM.Client.Url))
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
                searchRequestVM.SearchObject.SearchTerm = (!string.IsNullOrWhiteSpace(searchRequestVM.SearchObject.SearchTerm)) ? searchRequestVM.SearchObject.SearchTerm : string.Empty;
                IList<Users> users = await userRepositoy.GetUsersAsync(searchRequestVM);
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

        [HttpPost("getuserprofilepicture")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IActionResult UserProfilePicture([FromBody]Client client)
        {           
            spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
            string accountName = $"i:0#.f|membership|{HttpContext.User.Identity.Name}";
            spoAuthorization.AccountName = accountName;
            #region Error Checking                
            ErrorResponse errorResponse = null;

            if (client == null && string.IsNullOrWhiteSpace(client.Url))
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
            var userInfo = userRepositoy.GetUserProfilePicture(client);
            return matterCenterServiceFunctions.ServiceResponse(userInfo, (int)HttpStatusCode.OK);
        }



        /// <summary>
        /// Get all the roles that are configured for a given client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("getroles")]
        [SwaggerResponse(HttpStatusCode.OK)]               
        public async Task<IActionResult> GetRoles([FromBody]Client client)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;
                
                if (client == null && string.IsNullOrWhiteSpace(client.Url))
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
                IList<Role> roles = null;
                ServiceUtility.RedisCacheHostName = generalSettings.RedisCacheHostName;
                string result = ServiceUtility.GetDataFromAzureRedisCache(ServiceConstants.CACHE_ROLES);
                if (string.IsNullOrEmpty(result))
                {
                    roles = await userRepositoy.GetRolesAsync(client);
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

        /// <summary>
        /// Get all the permissions level that are configured for a given client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost("getpermissionlevels")]
        [SwaggerResponse(HttpStatusCode.OK)]         
        public async Task<IActionResult> GetPermissionLevels([FromBody]Client client)
        {
            try
            {
                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
                #region Error Checking                
                ErrorResponse errorResponse = null;

                if (client == null && string.IsNullOrWhiteSpace(client.Url))
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
                IList<Role> roles = null;

                ServiceUtility.RedisCacheHostName = generalSettings.RedisCacheHostName;
                string result = ServiceUtility.GetDataFromAzureRedisCache(ServiceConstants.CACHE_PERMISSIONS);
                if (string.IsNullOrEmpty(result))
                {
                    roles = await userRepositoy.GetPermissionLevelsAsync(client);
                    ServiceUtility.SetDataIntoAzureRedisCache<IList<Role>>(ServiceConstants.CACHE_PERMISSIONS, roles);
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
    }
}
