using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Legal.MatterCenter.Web
{
    public class MatterConfigurationProvider:ConfigurationProvider
    {
        public MatterConfigurationProvider(Action<HttpContext> httpContextOption)
        {
            HttpContextAction = httpContextOption;
        }

        Action<HttpContext> HttpContextAction { get; }

        public override void Load()
        {
            int x = 0;
        }
    }

    public static class MatterFrameworkExtensions
    {
        public static IConfigurationBuilder AddMatterFrameworConfiguration(this IConfigurationBuilder builder,  Action<HttpContext> setUp)
        {
            return builder.Add(new MatterConfigurationProvider(setUp));
        }
    }
}
