using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class MatterCenterServiceFunctions: IMatterCenterServiceFunctions
    {
        public ObjectResult ServiceResponse(object value, int statusCode)
        {
            return new ObjectResult(value)
            {
                StatusCode = statusCode                
            };
        }
    }
}
