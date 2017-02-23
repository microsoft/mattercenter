using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Matter.legal.MatterCenter.Performance
{

    public class FileContentExtractorData : ExtractionRule
    {
        private const string Folder = @"c:\"; 
    public string UserName { get; set; }
        private const int Len = 9;

        public override void Extract(object sender,
        ExtractionEventArgs e)
        {
            if (e.Response.HtmlDocument != null)
            {
                //USER NAME WAS PART OF THE FILE NAME 
                string file_path = Folder + FullName(UserName) + ".txt";
                e.WebTest.Context.Add(this.ContextParameterName,
                File.OpenText(file_path).ReadToEnd());
            }
        }
        //WELL KNOWN FEATURE BY DESIGN, VS REMOVES 
        //LOOK FOR ‘Leading zeroes dropped from datasrouces 
        //values bound to a CSV file’ 
        //IN Performance Testing Guidance How-To’s 
        //LEADING ZEROS BUT WE NEEDED THEN ALL 
        public string FullName(string name)
        {
            if (name.Length < Len)
            {
                StringBuilder new_name = new StringBuilder(Len);
                int delta = 0;
                delta = Len - name.Length;
                for (int i = 0; i < delta; i++)
                    new_name.Append("0");
                new_name.Append(name);
                return new_name.ToString();
            }
            else
                return name;
        }
    }
}