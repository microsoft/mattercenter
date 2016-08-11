using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Legal.MatterCenter.Jobs
{
    public class KeyVaultHelper
    {

        IConfigurationRoot configuration;

        public KeyVaultHelper(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        public void GetKeyVaultSecrets()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues = retrieveSecret();

            foreach (var ky in keyValues)
            {
                //config = Configuration.GetSection(ky.Key);
                configuration["General:" + ky.Key] = ky.Value;
            }
        }

        private Dictionary<String, String> retrieveSecret()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            // I put my GetToken method in a Utils class. Change for wherever you placed your method.
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));

            List<string> secrets = new List<string>();

            var values = kv.GetSecretsAsync("https://matterkeyvault.vault.azure.net:443").GetAwaiter().GetResult();


            if (values != null && values.Value != null)
            {

                foreach (var m in values.Value)
                    secrets.Add(m.Identifier.Name);
            }

            while (values != null && !string.IsNullOrWhiteSpace(values.NextLink))
            {
                values = kv.GetSecretsNextAsync(values.NextLink).GetAwaiter().GetResult();
                if (values != null && values.Value != null)
                {

                    foreach (var m in values.Value)
                        secrets.Add(m.Identifier.Name);
                }
            }
            foreach (var value in secrets)
            {
                var secret = kv.GetSecretAsync("https://matterkeyvault.vault.azure.net:443", value).GetAwaiter().GetResult();
                keyValues.Add(value, secret.Value);
            }


            return keyValues;
        }

        public async Task<string> GetToken(string authority, string resource, string scope)
        {

            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(configuration.GetSection("General").GetSection("ClientId").Value.ToString(),
                       configuration.GetSection("General").GetSection("AppKey").Value.ToString());
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
