using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    public class ProjectTeam
    {
        public ProjectTeam(string projectName, string teamName, string teamURL)
        {
            this.ProjectName = projectName;
            this.TeamName = teamName;
            this.TeamURL = teamURL;
            
        }

        public string ProjectName { get; set; }
        public string TeamName { get; set; }
        public string TeamURL { get; set; }
        
    }

    public class Project:ProjectTeam
    {
        /// <summary>
        /// Project description
        /// </summary>
        public string ProjectDescription { get; set; }

        /// <summary>
        /// Project practice group
        /// </summary>
        public string PracticeGroup { get; set; }

        /// <summary>
        /// Project type
        /// </summary>
        public string ProjectType { get; set; }

        /// <summary>
        /// Project property specifying if calendar list present
        /// </summary>
        public bool IsCalendarPresent { get; set; }

        /// <summary>
        /// Project property specifying if task list present
        /// </summary>
        public bool IsTaskPresent { get; set; }

        /// <summary>
        /// Project property specifying if RSS feed is enabled on project landing page
        /// </summary>
        public bool IsRSSFeedPresent { get; set; }

        /// <summary>
        /// Project GUID
        /// </summary>
        public string ProjectGUID { get; set; }

        /// <summary>
        /// Gets or sets project name
        /// </summary>
        public string NewProjectName { get; set; }

        /// <summary>
        /// Constructs Project object and initializes it's base class with project name, team name and team URL
        /// </summary>
        /// <param name="projectName">Project name</param>
        /// <param name="teamName">Team name</param>
        /// <param name="teamURL">Team URL</param>
        public Project(string projectName, string teamName, string teamURL)
            : base(projectName, teamName, teamURL)
        {
            this.ProjectDescription = string.Empty;
        }
        
    }
}
