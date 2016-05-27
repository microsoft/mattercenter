

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to store content for contextual help functionality.
    /// </summary>
    public class ContextHelpData
    {
        /// <summary>
        /// Gets or sets the structure required to store content for contextual help sections.
        /// </summary>
        /// <value>Provides the structure required to store content for contextual help sections.</value>
        public ContextHelpSection ContextSection { get; set; }
        /// <summary>
        /// Gets or sets the link title for operation.
        /// </summary>
        /// <value>Link title of contextual help fly out</value>
        public string LinkTitle { get; set; }
        /// <summary>
        /// Gets or sets the Link URL for operation.
        /// </summary>
        /// <value>Link URL of contextual help fly out</value>
        public string LinkURL { get; set; }
        /// <summary>
        /// Gets or sets the Link order for operation.
        /// </summary>
        /// <value>Link order of contextual help fly out</value>
        public string LinkOrder { get; set; }
    }
}
