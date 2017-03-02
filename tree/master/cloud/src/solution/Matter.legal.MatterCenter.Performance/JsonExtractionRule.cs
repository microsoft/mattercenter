using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Globalization;
using System.ComponentModel;
using Newtonsoft.Json.Linq;


namespace Matter.legal.MatterCenter.Performance
{
 
    public class JsonExtractionRule : ExtractionRule
    {
        public string Name { get; set; }

        public override void Extract(object sender, ExtractionEventArgs e)
        {
            if (e.Request.Body != null)
            {
                var json = e.Request.Body;
                StringHttpBody httpBody = e.Request.Body as StringHttpBody;

                if (httpBody == null) { return; }

                string body = httpBody.BodyString;
                var data = JObject.Parse(body);

                if (data != null)
                {
                    string tokenName = "Matter." + Name;
                    var value = (data.SelectToken(tokenName) ?? JValue.CreateNull()).ToObject<string>();
                    e.WebTest.Context.Add(this.ContextParameterName, value);
                    e.Success = true;
                    return;
                }
            }
            e.Success = false;
            e.Message = String.Format(CultureInfo.CurrentCulture, "Not Found: {0}", Name);
        }
    }
}

