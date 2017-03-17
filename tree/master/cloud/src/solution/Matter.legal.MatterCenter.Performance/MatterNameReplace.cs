using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Matter.legal.MatterCenter.Performance
{
    public class MatterNameReplace : WebTestRequestPlugin
    {

        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            if (e.Request.Body == null) { return; }
            StringHttpBody httpBody = e.Request.Body as StringHttpBody;
            if (httpBody == null) { return; }
            string body = httpBody.BodyString;

            string updatedBody = UpdateBody(body);

            StringHttpBody newBody = new StringHttpBody();
            newBody.BodyString = updatedBody;
            newBody.ContentType = "application/json";
            e.Request.Body = newBody;
        }

        private string UpdateBody(string body)
        {
        
            Guid id = Guid.NewGuid();
            
            string newName = RandomString(15);
          
            var data = JObject.Parse(body);

            var tokenName = "Matter." + "Name";
            var value = (data.SelectToken(tokenName) ?? JValue.CreateNull()).ToObject<string>();
            var updatedBodyName = UpdateBodyName(body, value, newName);

            tokenName = "Matter." + "MatterGuid";
            value = (data.SelectToken(tokenName) ?? JValue.CreateNull()).ToObject<string>();
            var updatedBodyGUID = UpdateBodyGUID(updatedBodyName, value, id.ToString().Replace("-",""));

            return updatedBodyGUID;
        }

        private string UpdateBodyName(string body, string value, string newName)
        {

            string name = "";
            name = body.Replace(value, newName);

            return name;
        }

        private string UpdateBodyGUID(string body, string value, string newGuid)
        {
            string guid = "";
            guid = body.Replace(value, newGuid);

            return guid;
        }


        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


    }

    public class UseMatterName : WebTestRequestPlugin
    {


        public override void PreRequest(object sender, PreRequestEventArgs e)
        {

            object mattername;
            if (e.Request.Body == null) { return; }
            StringHttpBody httpBody = e.Request.Body as StringHttpBody;
            if (httpBody == null) { return; }
            string body = httpBody.BodyString;
            string updatedBodyName = "";
            string updatedBody = "";
            object matterguid;
            var value = "";


            var data = JObject.Parse(body);

            if (data != null)
            {
                string tokenName = "";

                if (e.WebTest.Context.TryGetValue("Name", out mattername))
                {
                    tokenName = "Matter." + "Name";
                    value = (data.SelectToken(tokenName) ?? JValue.CreateNull()).ToObject<string>();
                    updatedBodyName = UpdateBodyName(body, value, mattername.ToString());
                }

                if (e.WebTest.Context.TryGetValue("MatterGuid", out matterguid))
                {
                    tokenName = "Matter." + "MatterGuid";
                    value = (data.SelectToken(tokenName) ?? JValue.CreateNull()).ToObject<string>();
                    updatedBody = UpdateBodyGUID(updatedBodyName, value, matterguid.ToString());
                }

                StringHttpBody newBody = new StringHttpBody();
                newBody.ContentType = "application/json";
                newBody.BodyString = updatedBody;

                e.Request.Body = newBody;
            }
        }


        private string UpdateBodyName(string body, string value, string newName)
        {

            string name = "";
            name = body.Replace(value, newName);

            return name;
        }

        private string UpdateBodyGUID(string body, string value, string newGuid)
        {
            string guid = "";
            guid = body.Replace(value, newGuid);

            return guid;
        }


        private static Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

    }
}
