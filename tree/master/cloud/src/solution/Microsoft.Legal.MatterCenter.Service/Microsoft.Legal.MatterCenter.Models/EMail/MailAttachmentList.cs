using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Microsoft.Legal.MatterCenter.Models
{
    
    public class MailAttachmentList
    {
        /// <summary>
        /// Get or sets the attachements associated with the current mail message
        /// </summary>
        [JsonProperty("value")]
        public List<AttachmentDetails> AttachmentDetails { get; set; }
    }
}
