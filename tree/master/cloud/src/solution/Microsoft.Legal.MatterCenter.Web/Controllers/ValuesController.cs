

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
#endregion

namespace Microsoft.Legal.MatterCenter.Service
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private GeneralSettings generalSettings;
        private SharedSettings sharedSettings;
        public ValuesController(IOptions<GeneralSettings> generalSettings, IOptions<SharedSettings> sharedSettings)
        {
            this.generalSettings = generalSettings.Value;
            this.sharedSettings = sharedSettings.Value;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
