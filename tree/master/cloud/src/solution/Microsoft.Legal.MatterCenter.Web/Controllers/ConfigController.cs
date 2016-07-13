//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;

//using Swashbuckle.SwaggerGen.Annotations;
//using System.Net;
//using System.Reflection;
//using Microsoft.Legal.MatterCenter.Web.Common;
//#region Matter Namespaces
//using Microsoft.Legal.MatterCenter.Utility;
//using Microsoft.Legal.MatterCenter.Repository;
//using Microsoft.Legal.MatterCenter.Models;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Microsoft.AspNetCore.Authorization;
//#endregion
//// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

//namespace Microsoft.Legal.MatterCenter.Web.Controllers
//{
//    public class ConfigController : Controller
//    {
//        private ErrorSettings errorSettings;
//        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
//        private IConfigRepository configRepository;
//        private GeneralSettings generalSettings;
      
//        private ISPOAuthorization spoAuthorization;
   
//        private SharedSettings sharedSettings;
//        private ISharedRepository sharedRepository;
//        private ICustomLogger customLogger;
//        private LogTables logTables;
//        /// <summary>
//        /// Constructor where all the required dependencies are injected
//        /// </summary>
//        /// <param name="errorSettings"></param>
//        /// <param name="matterSettings"></param>
//        /// <param name="spoAuthorization"></param>
//        /// <param name="matterCenterServiceFunctions"></param>
//        public ConfigController(IOptionsMonitor<ErrorSettings> errorSettings,
//            IOptionsMonitor<GeneralSettings> generalSettings,
//            IMatterCenterServiceFunctions matterCenterServiceFunctions,
//            IConfigRepository configRepository
//            )
//        {
//            this.errorSettings = errorSettings.CurrentValue;
//            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
//            this.configRepository = configRepository;
//            this.generalSettings = generalSettings.CurrentValue;
//        }

//         /// <summary>
//        /// Returns true or false based on the existence of the matter landing page and OneNote file at the URLs provided.
//        /// </summary>
//        /// <param name="requestObject">Request object containing SharePoint App Token</param>
//        /// <param name="client">Client object containing Client data</param>
//        /// <param name="requestedUrl">String object containing the OneNote file path</param>
//        /// <param name="requestedPageUrl">String object containing the Matter Landing Page file path</param>
//        /// <returns>$|$ Separated string indicating that the OneNote and the Matter Landing Page exist or not</returns>        
//        [HttpPost("Get")]
//        [SwaggerResponse(HttpStatusCode.OK)]        
//        public async Task<IActionResult> Get(Client client, string oneNoteUrl, string matterLandingPageUrl)
//        {
//            string result = string.Empty;
            
//            try
//            {

//                spoAuthorization.AccessToken = HttpContext.Request.Headers["Authorization"];
//                #region Error Checking                
//                ErrorResponse errorResponse = null;
//                //if the token is not valid, immediately return no authorization error to the user
//                if (errorResponse != null && !errorResponse.IsTokenValid)
//                {
//                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.Unauthorized);
//                }

//                if (client == null && string.IsNullOrEmpty(oneNoteUrl) && string.IsNullOrEmpty(matterLandingPageUrl))
//                {
//                    errorResponse = new ErrorResponse()
//                    {
//                        Message = errorSettings.MessageNoInputs,
//                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
//                        Description = "No input data is passed"
//                    };
//                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.NotFound);
//                }
//                #endregion
//                var obj = await  configRepository.GetTableStorageEntity();
//                var matterLandingUrlExists = await sharedRepository.UrlExistsAsync(client, matterLandingPageUrl);
//                var urlExists = new
//                {
//                    OneNoteExists = oneNoteUrlExists,
//                    MatterLandingExists = matterLandingUrlExists
//                };
//                return matterCenterServiceFunctions.ServiceResponse(urlExists, (int)HttpStatusCode.OK);
//            }
//            catch (Exception exception)
//            {
//                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
//                throw;
//            }
//        }
//    }
//}
