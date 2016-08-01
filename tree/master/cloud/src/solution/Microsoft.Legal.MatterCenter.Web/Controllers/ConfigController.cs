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
using Newtonsoft.Json;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Newtonsoft.Json.Linq;


#endregion
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.Legal.MatterCenter.Web.Controllers
{
    /// <summary>
    /// Config settings
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/v1/config")]
    public class ConfigController : Controller
    {
        private ErrorSettings errorSettings;
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private IConfigRepository configRepository;
        private GeneralSettings generalSettings;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private IHostingEnvironment hostingEnvironment;
        /// <summary>
        /// Constructor where all the required dependencies are injected
        /// </summary>
        /// <param name="errorSettings"></param>
        /// <param name="generalSettings"></param>
        /// <param name="configRepository"></param>
        ///    /// <param name="hostingEnvironment"></param>
        /// <param name="matterCenterServiceFunctions"></param>
        public ConfigController(IOptions<ErrorSettings> errorSettings,
            IOptions<GeneralSettings> generalSettings,
            IMatterCenterServiceFunctions matterCenterServiceFunctions,
            IConfigRepository configRepository,
            IHostingEnvironment hostingEnvironment
            )
        {
            this.errorSettings = errorSettings.Value;
            this.matterCenterServiceFunctions = matterCenterServiceFunctions;
            this.configRepository = configRepository;
            this.generalSettings = generalSettings.Value;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Returns all the entries for Configuring the UI
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("Get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromBody] String filter)
        {
            string result = string.Empty;

            try
            {
                #region Error Checking                
                ErrorResponse errorResponse = null;

                if (filter == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No filter was passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion


                var configResultsVM = await configRepository.GetConfigurationsAsync(filter);

                CreateConfig(configResultsVM);
                return matterCenterServiceFunctions.ServiceResponse(configResultsVM, (int)HttpStatusCode.OK);

            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }



        /// <summary>
        /// Returns all the entries for Configuring the UI
        /// </summary>
        /// <param name="configs">Request object for POST</param>   
        [HttpPost("Insertupdate")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> InsertUpdate([FromBody] String configs)
        {
            string result = string.Empty;

            try
            {
                #region Error Checking                
                ErrorResponse errorResponse = null;

                if (configs == null)
                {
                    errorResponse = new ErrorResponse()
                    {
                        Message = errorSettings.MessageNoInputs,
                        ErrorCode = HttpStatusCode.BadRequest.ToString(),
                        Description = "No config Data was passed"
                    };
                    return matterCenterServiceFunctions.ServiceResponse(errorResponse, (int)HttpStatusCode.OK);
                }
                #endregion


                var configResultsVM = await configRepository.InsertUpdateConfigurationsAsync(configs);


                return matterCenterServiceFunctions.ServiceResponse(configResultsVM, (int)HttpStatusCode.OK);

            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        private void CreateConfig(List<DynamicTableEntity> configs)

        {
            StringBuilder sb = new StringBuilder();
            JsonWriter jw = new JsonTextWriter(new StringWriter(sb));
            jw.Formatting = Formatting.Indented;

            var configPath = Path.Combine(hostingEnvironment.WebRootPath, "app/uiconfig.js");
            if (System.IO.File.Exists(configPath))
                System.IO.File.Delete(configPath);

            var configFile = System.IO.File.Open(configPath, FileMode.Create);
            var configWriter = new StreamWriter(configFile, Encoding.UTF8);


            List<string> configGroup = new List<string>();
            EntityProperty key;
            EntityProperty value;

            foreach (DynamicTableEntity dt in configs)
            {
                bool hasKey = dt.Properties.TryGetValue("ConfigGroup", out value);
                if (hasKey)
                {
                    if (!configGroup.Contains(value.StringValue))
                    {
                        configGroup.Add(value.StringValue);
                    }
                }
            }
            configWriter.WriteLine("var uiconfigs =");

            jw.WriteStartObject();
            foreach (string str in configGroup)
            {
                jw.WritePropertyName(str);
                jw.WriteStartObject();

                foreach (DynamicTableEntity dt in configs)
                {
                    bool scr = dt.Properties.TryGetValue("ConfigGroup", out value);

                    if (str.ToLower().Equals(value.StringValue.ToLower()))
                    {
                        bool hasKey = dt.Properties.TryGetValue("Key", out key);
                        bool hasValue = dt.Properties.TryGetValue("Value", out value);

                        if (hasKey && hasValue)
                        {
                            {
                                jw.WritePropertyName(key.StringValue.ToString());
                                jw.WriteValue(value.StringValue.ToString());
                            }
                        }
                    }
                }
                jw.WriteEndObject();
            }
            jw.WriteEndObject();
            configWriter.Write(sb.ToString());
            configWriter.WriteLine(";");
            configWriter.Dispose();
        }
    }
}
