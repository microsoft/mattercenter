
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    /// <summary>
    /// Represents a legal document. Provides the structure required to hold pinned document meta-data. It includes document name, version, created date, modified date, project details for document, and team details for document.
    /// </summary>
    public class DocumentData
    {
        /// <summary>
        /// Gets or sets the name of the document. Represents the document under the project library.
        /// </summary>
        /// <value>The name of the document.</value>
        public string DocumentName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document version. Represents the document version (minor, major, check out, etc.).
        /// </summary>
        /// <value>The document version.</value>
        public string DocumentVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document Team. Represents the Team name under which the document is present.
        /// </summary>
        /// <value>The document Team.</value>
        public string DocumentClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document Team identifier. Represents the unique Team ID under which the document is present.
        /// </summary>
        /// <value>The document Team ID identifier.</value>
        public string DocumentClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document Team URL. Represents the team URL under which the document is present.
        /// </summary>
        /// <value>The document Team URL.</value>
        public string DocumentClientUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document project. Represents the project library that holds the document.
        /// </summary>
        /// <value>The document project.</value>
        public string DocumentMatterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document project identifier. Represents the project ID that holds the document.
        /// </summary>
        /// <value>The document project identifier.</value>
        public string DocumentMatterId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document owner who has uploaded the document.
        /// </summary>
        /// <value>The document owner.</value>
        public string DocumentOwner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document URL.
        /// </summary>
        /// <value>The document URL.</value>
        public string DocumentUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document OWA URL. Represents the documents URL supported by OWA or Office online; otherwise the document path.
        /// </summary>
        public string DocumentOWAUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document extension.
        /// </summary>
        /// <value>The document extension.</value>
        public string DocumentExtension
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document created date. Represents the document uploaded date in project library.
        /// </summary>
        /// <value>The document created date.</value>
        public string DocumentCreatedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document modified date. Represents the document last modified date in project library.
        /// </summary>
        /// <value>The document modified date.</value>
        public string DocumentModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document check out user. Represents the check out user of document.
        /// </summary>
        /// <value>The document check out user.</value>
        public string DocumentCheckoutUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document project URL. Represents the URL of project library where document is present.
        /// </summary>
        /// <value>The document check out user.</value>
        public string DocumentMatterUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document parent URL. Represents the parent URL of document.
        /// </summary>
        /// <value>The document check out user.</value>
        public string DocumentParentUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document ID. Represents the ID of a document.
        /// </summary>
        /// <value>The document ID.</value>
        public string DocumentID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document practice group. Represents the practice group for a document.
        /// </summary>
        /// <value>The document ID.</value>
        public string DocumentPracticeGroup
        {
            get;
            set;
        }

        public string DocumentIconUrl { get; set; }

        public string PinType { get; set; }
    }
}
