using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IMatterCenterServiceFunctions
    {
        ObjectResult ServiceResponse(object value, int statusCode);
    }
}
