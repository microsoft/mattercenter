using System;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Globalization;
using Newtonsoft.Json.Linq;


namespace Matter.legal.MatterCenter.Performance
{
    public class JsonExtractionRule : ExtractionRule
    {
        public string Name { get; set; }
        public string MatterGuid { get; set; }


        public override void Extract(object sender, ExtractionEventArgs e)
        {
            string tokenName = "";
            object contextValue;

            if (e.Request.Body != null)
            {
                var json = e.Request.Body;
                StringHttpBody httpBody = e.Request.Body as StringHttpBody;

                if (httpBody == null) { return; }

                string body = httpBody.BodyString;
                var data = JObject.Parse(body);              
                var value = "";

                if (data != null)
                {
        
                    if (e.WebTest.Context.TryGetValue(this.ContextParameterName, out contextValue))
                    {
                        tokenName = "Matter." + this.ContextParameterName;
                        value = (data.SelectToken(tokenName) ?? JValue.CreateNull()).ToObject<string>();
                        e.WebTest.Context.Add(this.ContextParameterName, value);
                    }
                    e.Success = true;
                    return;
                }
            }
            e.Success = false;
            e.Message = String.Format(CultureInfo.CurrentCulture, "Not Found: {0}", tokenName);
        }
    }
}

