
//using System;
//using Microsoft.Extensions.Configuration;


//namespace Microsoft.Legal.Matter.WebTestAPI
//{

//    public class CompositionRootFixture
//    {

//        public IConfigurationRoot Configuratuion { get; }


//        public CompositionRootFixture()
//        {
//            string path = Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.IndexOf("\\bin\\Debug\\net46\\win7-x64", 0), 25);

//            var builder = new ConfigurationBuilder()
//                .SetBasePath(path)
//                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                .AddInMemoryCollection()
//                .AddEnvironmentVariables();
//            ;
//            Configuratuion = builder.Build();

//            KeyVaultHelper keyVaultHelper = new KeyVaultHelper(Configuratuion);
//            KeyVaultHelper.GetCert(Configuratuion);
//            keyVaultHelper.GetKeyVaultSecretsCerticate();

//        }

//    }

//}