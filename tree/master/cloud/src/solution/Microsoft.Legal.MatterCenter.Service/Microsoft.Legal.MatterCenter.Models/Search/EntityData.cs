

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Holds the structure of entity data returned from people picker web service.
    /// </summary>
    public class EntityData
    {
        /// <summary>
        /// Gets or sets the title of user or security group
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the department of user or security group
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the email of user or security group
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the email of user or security group
        /// </summary>
        public string PrincipalType { get; set; }
        public string SPUserID { get; set; }
        public string IsBlocked { get; set; }
        public string SIPAddress { get; set; }
        public string AccountName { get; set; }
    }
}
