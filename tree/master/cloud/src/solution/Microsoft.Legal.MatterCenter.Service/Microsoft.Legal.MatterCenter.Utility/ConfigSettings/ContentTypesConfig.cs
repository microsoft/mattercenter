using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class ContentTypesConfig
    {
        public string ContentTypeColumnClientId
        {
            get; set;
        }
        public string ContentTypeColumnClientName
        {
            get; set;
        }
        public string ContentTypeColumnMatterId
        {
            get; set;
        }
        public string ContentTypeColumnMatterName
        {
            get; set;
        }
        public string ContentTypeColumnPracticeGroup
        {
            get; set;
        }
        public string ContentTypeColumnAreaOfLaw
        {
            get; set;
        }
        public string ContentTypeColumnSubareaOfLaw
        {
            get; set;
        }
        public string OneDriveContentTypeGroup
        {
            get; set;
        }
        public string HiddenContentType { get; set; }
        public string ViewColumnList { get; set; }
        public string ViewOrderByColumn { get; set; }
        public string ViewName { get; set; }
    }
}
