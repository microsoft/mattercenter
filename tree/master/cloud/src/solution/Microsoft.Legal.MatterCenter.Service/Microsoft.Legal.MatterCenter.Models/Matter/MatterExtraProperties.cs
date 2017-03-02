using System;
using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterExtraProperties
    {
        //Represent name of content type.
        public string ContentTypeName { get; set; }

        public IList<MatterExtraFields> Fields = new List<MatterExtraFields>();
    }

    /// <summary>
    /// This class represent the metadata for each matter extra properties site column.
    /// </summary>
    public class MatterExtraFields
    {

        public string FieldName { get; set; }
        public string Type { get; set; }
        public string FieldValue { get; set; }
        public string FieldDisplayName { get; set; }
        public string IsDisplayInUI { get; set; }
        public string IsMandatory { get; set; }

    }
}
