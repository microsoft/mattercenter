using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterExtraProperties
    {


        public string ContentTypeName { get; set; }

        public IList<Field> Fields = new List<Field>();
    }

    public class Field
    {
        public string FieldName { get; set; }
        public string Type { get; set; }
        public string FieldValue { get; set; }
    }

}
