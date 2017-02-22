using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System.Globalization;

using System;

using System.ComponentModel;

using Newtonsoft.Json.Linq;

namespace Matter.legal.MatterCenter.Performance
{
    //-------------------------------------------------------------------------  
    // This class creates a custom extraction rule named "Custom Extract Input"  
    // The user of the rule specifies the name of an input field, and the  
    // rule attempts to extract the value of that input field.  
    //-------------------------------------------------------------------------  
    public class CustomExtractInput : ExtractionRule
    {
        /// Specify a name for use in the user interface.  
        /// The user sees this name in the Add Extraction dialog box.  
        //---------------------------------------------------------------------  
        public override string RuleName
        {
            get { return "Custom Extract Input"; }
        }

        /// Specify a description for use in the user interface.  
        /// The user sees this description in the Add Extraction dialog box.  
        //---------------------------------------------------------------------  
        public override string RuleDescription
        {
            get { return "Extracts the value from a specified input field"; }
        }

        // The name of the desired input field  
        private string NameValue;
        public string Name
        {
            get { return NameValue; }
            set { NameValue = value; }
        }

        // The Extract method.  The parameter e contains the web performance test context.  
        //---------------------------------------------------------------------  
        public override void Extract(object sender, ExtractionEventArgs e)
        {


            if (e.Response.HtmlDocument != null)
            {
                foreach (HtmlTag tag in e.Response.HtmlDocument.GetFilteredHtmlTags(new string[] { "h2" }))
                {
                    if (String.Equals(tag.GetAttributeValueAsString("name"), Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        string formFieldValue = tag.GetAttributeValueAsString("value");
                        if (formFieldValue == null)
                        {
                            formFieldValue = String.Empty;
                        }

                        // add the extracted value to the web performance test context  
                        e.WebTest.Context.Add(this.ContextParameterName, formFieldValue);
                        e.Success = true;
                        return;
                    }
                }
            }
            // If the extraction fails, set the error text that the user sees  
            e.Success = false;
            e.Message = String.Format(CultureInfo.CurrentCulture, "Not Found: {0}", Name);
        }
    }


}
[DisplayName("JSON Extraction Rule")]

[Description("Extracts the specified JSON value from an object.")]

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

