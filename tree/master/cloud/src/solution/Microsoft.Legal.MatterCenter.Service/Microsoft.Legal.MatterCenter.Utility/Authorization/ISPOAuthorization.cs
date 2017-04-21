#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
#endregion

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// An interface for SPOAuthorization which is mapped in the StartUp.cs
    /// </summary>
    public interface ISPOAuthorization
    {
        //ErrorResponse ValidateClientToken(string authToken);
        ClientContext GetClientContext(string url);
        string GetGraphAccessToken();
        string GetExchangeAccessToken();
    }
}
