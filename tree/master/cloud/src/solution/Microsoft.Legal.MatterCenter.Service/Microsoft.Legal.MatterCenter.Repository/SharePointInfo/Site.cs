using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class Site:ISite
    {
        ISPOAuthorization spoAuthorization;
        ClientContext clientContext;
        public Site(ISPOAuthorization spoAuthorization)
        {
            this.spoAuthorization = spoAuthorization;
        }

        public string GetCurrentSite(Client client)
        {
            clientContext = spoAuthorization.GetClientContext(client.Url);
            var spWeb = clientContext.Web;
            clientContext.Load(spWeb);
            clientContext.ExecuteQuery();
            return spWeb.Title.ToString();
        }
    }
}
