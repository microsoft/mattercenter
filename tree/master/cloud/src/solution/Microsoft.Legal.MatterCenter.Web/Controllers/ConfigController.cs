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
        /// Returns all the entries for Configuring the UI
        /// </summary>
        /// <param name="configRequest">Request object for POST</param>
    
        [HttpPost("Get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromBody] DynamicTableEntity configRequest)
        {
            string result = string.Empty;

            try
            {

                #region Error Checking                
                ErrorResponse errorResponse = null;
              
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

            List<string> configGroup = new List<string>();
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

            int groupCount = configGroup.Count;
            int count = configs.Count;

            configWriter.WriteLine("var uiconfigs = {");

            int groups = 0;
            foreach (string str in configGroup)
            {
              
                configWriter.WriteLine("\"" + str + "\":  {");
                int entityCount = 0;
                foreach (DynamicTableEntity dt in configs)
                {
                    entityCount++;
                    bool scr = dt.Properties.TryGetValue("ConfigGroup", out value);

                    if (str.ToLower().Equals(value.StringValue.ToLower()))
                    {
                       if (entityCount < count)
                        {
                            configWriter.WriteLine("\"" + dt.Properties["Key"].StringValue + "\" :" + "\"" + dt.Properties["Value"].StringValue + "\",");
                        }
                       else
                        {
                            configWriter.WriteLine("\"" + dt.Properties["Key"].StringValue + "\": " + "\"" + dt.Properties["Value"].StringValue + "\"");
                        }
                    }

                }

                if (groups < count)
                {
                    configWriter.WriteLine("},");
                }
                else
                {
                    configWriter.WriteLine("}");
                }
                
            }
            configWriter.WriteLine("};");
            configWriter.Dispose();
        }
    }

}
