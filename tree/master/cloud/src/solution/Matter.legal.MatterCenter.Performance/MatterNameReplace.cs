using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matter.legal.MatterCenter.Performance
{
    public class MatterNameReplace: WebTestRequestPlugin
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
            e.Request.Body = newBody;
        }


        private string  UpdateBody(string body)
        {
            
            body.Replace("New Matter microsoft Test46", RandomString(15));
            return body;
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

    
}
