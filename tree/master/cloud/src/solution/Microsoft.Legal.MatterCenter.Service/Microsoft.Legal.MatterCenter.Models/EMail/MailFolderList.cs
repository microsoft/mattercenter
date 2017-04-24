using System.Collections.Generic;
using Newtonsoft.Json;
namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Define a list of email message folders
    /// </summary>
    public class MailFolderList
    {
        /// <summary>
        /// The list of email message folders
        /// </summary>
        [JsonProperty("value")]
        public List<MailFolder> Folders { get; set; }
    }
}
