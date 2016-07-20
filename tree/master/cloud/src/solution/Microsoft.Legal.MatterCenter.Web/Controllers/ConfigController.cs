using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using System.Reflection;
using Microsoft.Legal.MatterCenter.Web.Common;

using Microsoft.AspNetCore.Hosting;

using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Linq;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text;
#endregion
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.Legal.MatterCenter.Web.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/v1/config")]
    public class ConfigController : Controller
    {
        private ErrorSettings errorSettings;
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private IConfigRepository configRepository;
        private GeneralSettings generalSettings;

        private ISPOAuthorization spoAuthorization;

        private SharedSettings sharedSettings;
        private ISharedRepository sharedRepository;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private IHostingEnvironment hostingEnvironment;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="matterSettings"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        public ConfigController(IOptionsMonitor<ErrorSettings> errorSettings,
            IOptionsMonitor<GeneralSettings> generalSettings,
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IConfigRepository configRepository,
            IHostingEnvironment hostingEnvironment
            )
        {
            this.errorSettings = errorSettings.CurrentValue;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.configRepository = configRepository;
            this.generalSettings = generalSettings.CurrentValue;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Returns true or false based on the existence of the matter landing page and OneNote file at the URLs provided.
        /// </summary>
        /// <param name="requestObject">Request object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="requestedUrl">String object containing the OneNote file path</param>
        /// <param name="requestedPageUrl">String object containing the Matter Landing Page file path</param>
        /// <returns>$|$ Separated string indicating that the OneNote and the Matter Landing Page exist or not</returns>        
        [HttpPost("Get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromBody] DynamicTableEntity configRequest)
        {
            string result = string.Empty;

            try
            {

                #region Error Checking                
                ErrorResponse errorResponse = null;
                //if the token is not valid, immediately return no authorization error to the user

                #endregion
                var configResultsVM = await configRepository.GetConfigurationsAsync(configRequest);

                createConfig(configResultsVM);
                return matterCenterServiceFunctions.ServiceResponse(configResultsVM, (int)HttpStatusCode.OK);

            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        private void createConfig(List<DynamicTableEntity> configs)
        {


            var configPath = Path.Combine(hostingEnvironment.WebRootPath, "app/uiconfig.js");
            if (System.IO.File.Exists(configPath))
                System.IO.File.Delete(configPath);

            var configFile = System.IO.File.Open(configPath, FileMode.Create);
            var configWriter = new StreamWriter(configFile, Encoding.UTF8);

            List<string> screens = new List<string>();
            EntityProperty value;
            foreach (DynamicTableEntity dt in configs)
            {
                    
                    bool hasKey = dt.Properties.TryGetValue("Screen", out value);
                    if (hasKey)
                    {
                        screens.Add(value.StringValue);
                    }
                                
            }

            foreach (string str in screens)
            {

                foreach (DynamicTableEntity dt in configs)
                {
                    bool scr = dt.Properties.TryGetValue("Screen", out value);
                    if (str.Equals(value.StringValue))
                    {
                        configWriter.WriteLine(" \" + str + \": \"" + dt.Properties["Key"].StringValue + dt.Properties["Key"].StringValue + "\",");
                    }
                }

                configWriter.Dispose();
            }
        }
    }
}
