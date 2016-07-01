using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IMatterCenterServiceFunctions
    {
        //The response from the api that will be send to the client
        ObjectResult ServiceResponse(object value, int statusCode);
    }
}
