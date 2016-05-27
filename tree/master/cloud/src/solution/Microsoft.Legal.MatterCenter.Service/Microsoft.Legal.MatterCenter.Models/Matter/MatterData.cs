using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Represents a legal matter. Provides the structure required to hold pinned matter meta-data. It includes name, description, created date, URL, practice group, area of law, sub area of law, client ID, and client name.
    /// </summary>
    public class MatterData
    {
        /// <summary>
        /// Gets or sets the name of the matter. Represents the name of matter library.
        /// </summary>
        /// <value>The name of the matter.</value>
        public string MatterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter description. Represents the description of matter library.
        /// </summary>
        /// <value>The matter description.</value>
        public string MatterDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter created date. Represents the matter library creation date.
        /// </summary>
        /// <value>The matter created date.</value>
        public string MatterCreatedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter URL. Represents the matter library URL present under the client.
        /// </summary>
        /// <value>The matter URL.</value>
        public string MatterUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter practice group. Represents the practice group associated with the matter.
        /// </summary>
        /// <value>The matter practice group.</value>
        public string MatterPracticeGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter area of law. Represents the area of law associated with the matter.
        /// </summary>
        /// <value>The matter area of law.</value>
        public string MatterAreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter sub area of law. Represents the sub area of law associated with the matter.
        /// </summary>
        /// <value>The matter sub area of law.</value>
        public string MatterSubAreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter client URL. Represents the URL of client for the matter.
        /// </summary>
        /// <value>The matter client URL.</value>
        public string MatterClientUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter client. Represents the name of client for the matter.
        /// </summary>
        /// <value>The matter client.</value>
        public string MatterClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter client identifier. Represents the client ID for the matter.
        /// </summary>
        /// <value>The matter client identifier.</value>
        public string MatterClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HideUpload flag. Represents if the upload icon should be hidden from the user.
        /// </summary>
        /// <value>The hide upload.</value>
        public string HideUpload
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Matter ID. Represents the matter ID under the client.
        /// </summary>
        /// <value>The matter ID.</value>
        public string MatterID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the responsible attorney. Represents the attorney responsible for the matter.
        /// </summary>
        /// <value>The responsible attorney.</value>
        public string MatterResponsibleAttorney
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Modified date. Represents the modified date for the matter.
        /// </summary>
        /// <value>The Modified Date.</value>
        public string MatterModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Matter GUID. Represents the GUID for the matter.
        /// </summary>
        /// <value>The Matter GUID.</value>
        public string MatterGuid
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Matter GUID. Represents the GUID for the matter.
        /// </summary>
        /// <value>The Matter GUID.</value>
        public string PinType
        {
            get;
            set;
        }
    }
}
