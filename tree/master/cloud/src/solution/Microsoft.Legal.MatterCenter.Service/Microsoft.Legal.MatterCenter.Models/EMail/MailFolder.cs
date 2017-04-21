
using Newtonsoft.Json;
namespace Microsoft.Legal.MatterCenter.Models
{
    //Define an email folder
    public class MailFolder
    {
        /// <summary>
        /// display name of email folder
        /// </summary>
        [JsonProperty("displayName")]
        public string Name { get; set; }
        /// <summary>
        /// display name of email folder
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// total number of items
        /// </summary>
        [JsonProperty("totalItemCount")]
        public int TotalItemCount { get; set; }
        /// <summary>
        /// Number of unread items
        /// </summary>
        [JsonProperty("unReadItemCount")]
        public int UnreadItemCount { get; set; }
    }
}
