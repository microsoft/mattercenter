

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to store content for contextual help sections.
    /// </summary>
    public class ContextHelpSection
    {
        /// <summary>
        /// Gets or sets the section id for operation.
        /// </summary>
        /// <value>Section id of contextual help fly out</value>
        public string SectionID { get; set; }
        /// <summary>
        /// Gets or sets the section title for operation.
        /// </summary>
        /// <value>Section title of contextual help fly out</value>
        public string SectionTitle { get; set; }
        /// <summary>
        /// Gets or sets the section order for operation.
        /// </summary>
        /// <value>Section order of contextual help fly out</value>
        public string SectionOrder { get; set; }
        /// <summary>
        /// Gets or sets the page name for operation.
        /// </summary>
        /// <value>Page name of contextual help fly out</value>
        public string PageName { get; set; }
        /// <summary>
        /// Gets or sets the number of columns for help links.
        /// </summary>
        /// <value>Number of columns to be displayed on the contextual help section</value>
        public string NumberOfColumns { get; set; }
    }
}
