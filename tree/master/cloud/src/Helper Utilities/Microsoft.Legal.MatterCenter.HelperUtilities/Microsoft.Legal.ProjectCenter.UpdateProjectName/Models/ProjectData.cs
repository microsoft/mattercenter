using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    /// <summary>
    /// Represents a legal project. Provides the structure required to hold pinned project meta-data. It includes name, description, created date, Project URL, team name, project type, project team URL, project ID, project team ID, Hide Upload, Responsible Attorney, Project GUID and Project Modified time.
    /// </summary>
    public class ProjectData
    {
        /// <summary>
        /// Gets or sets the name of the project. Represents the name of project library.
        /// </summary>
        /// <value>The name of the project.</value>
        public string MatterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project description. Represents the description of project library.
        /// </summary>
        /// <value>The project description.</value>
        public string MatterDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project created date. Represents the project library creation date.
        /// </summary>
        /// <value>The project created date.</value>
        public string MatterCreatedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project URL. Represents the project library URL present under the teams.
        /// </summary>
        /// <value>The project URL.</value>
        public string MatterUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project practice group. Represents the practice group associated with the project.
        /// </summary>
        /// <value>The project practice group.</value>
        public string MatterPracticeGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project team name. Represents the team name associated with the project.
        /// </summary>
        /// <value>The team name.</value>
        public string MatterAreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project project type. Represents the project type associated with the project.
        /// </summary>
        /// <value>The project type.</value>
        public string MatterSubAreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project team URL. Represents the URL of team for the project.
        /// </summary>
        /// <value>The project team URL.</value>
        public string MatterClientUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project Team id. Represents the Team ID for the project.
        /// </summary>
        /// <value>The project Team ID.</value>
        public string MatterClientId
        {
            get;
            set;
        }

        

        /// <summary>
        /// Gets or sets the Project Id. Represents the project Id under the teams.
        /// </summary>
        /// <value>The Project ID.</value>
        public string MatterID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Modified date. Represents the modified date for the project.
        /// </summary>
        /// <value>The Modified Date.</value>
        public string MatterModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Project GUID. Represents the GUID for the project.
        /// </summary>
        /// <value>The Project GUID.</value>
        public string MatterGuid
        {
            get;
            set;
        }

        private string matterResponsibleAttorney = "";
        public string MatterResponsibleAttorney
        {
            get
            {
                return "";
            }
            set
            {
                matterResponsibleAttorney = "";
            }
        }

        private string hideUpload = "false";
        public string HideUpload
        {
            get
            {
                return hideUpload;
            }
            set
            {
                hideUpload = "false";
            }
        }

    }
}
