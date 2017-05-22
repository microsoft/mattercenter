using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;

namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");
                var configuration = builder.Build();

                //Console.WriteLine("Enter login user email:");
                var email = configuration.GetSection("General")["AdminUserName"];
                Console.WriteLine("Enter login user password:");
                var passWord = Console.ReadLine();
                SecureString secureStringPassword = Utilities.GetEncryptedPassword(passWord);
                //Console.WriteLine("Enter the team url:");
                var teamUrl = configuration.GetSection("General")["TeamUrl"];
                string teamName = "";
                //Console.WriteLine("Enter the project name that needs to be modified");
                var projectName = configuration.GetSection("General")["ProjectName"];

                //s Console.WriteLine("Enter the new project name that needs to be updated");
                var newProjectName = configuration.GetSection("General")["NewProjectName"];

                bool isProjectNameValid = Utilities.IsProjectNameValid(newProjectName);

                UserInfo userInfo = new UserInfo();
                userInfo.UserName = email;
                userInfo.Password = secureStringPassword;

                Console.WriteLine($"Checking if the new project name is valid {newProjectName}...");
                if (!isProjectNameValid)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Project name should contain the following characters" + Constants.SpecialCharacterExpressionProjectTitle);
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($"Checking if any project already exists with new project name '{newProjectName}'...");
                if (IsNewProjectAlreadyExists(newProjectName, teamUrl, userInfo, configuration))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"There is already a project with the give name '{newProjectName}'. Please enter a new one");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("New Project name is valid");
                    Project project = new Project(projectName, teamName, teamUrl);
                    project.NewProjectName = newProjectName;
                    Console.WriteLine($"Updating the old project {projectName} to {newProjectName}...");
                    bool isUpdateProjectNameSuccess = UpdateProjectName(project, userInfo, configuration);
                    if (isUpdateProjectNameSuccess)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Project has been renamed from {projectName} to {newProjectName} successfully");
                        Console.WriteLine("Press any key to exit...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Project rename unsuccessfull");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            
            //Set the project information that needs to be updated
            Console.ReadLine();
        }


        private static bool IsNewProjectAlreadyExists(string newProjectName, string teamUrl,
            UserInfo userInfo, 
            IConfigurationRoot configuration)
        {
            ClientContext clientContext = null;
            clientContext = new ClientContext(teamUrl);
            clientContext.Credentials = new SharePointOnlineCredentials(userInfo.UserName, userInfo.Password);
            List<string> associatedLists = Utilities.MatterAssociatedLists(clientContext, newProjectName);
            if (associatedLists!= null && associatedLists.Count > 0)
            {                
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method will update the project name and its corresponding lists
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>s
        private static bool UpdateProjectName(Project project, UserInfo userInfo, IConfigurationRoot configuration)
        {
            try
            {
                ClientContext clientContext = null;
                clientContext = new ClientContext(project.TeamURL);
                clientContext.Credentials = new SharePointOnlineCredentials(userInfo.UserName, userInfo.Password);

                Console.WriteLine($"Checking what lists are associated with {project.ProjectName}...");
                List<string> associatedLists = Utilities.MatterAssociatedLists(clientContext, project.ProjectName);

                if (associatedLists == null && associatedLists.Count <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The given project name '{project.ProjectName}' does not exists in the provided team url");
                    Console.ForegroundColor = ConsoleColor.White;
                    return false;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Getting all the properties of the project '{project.ProjectName}'...");
                //Get the project document library information that needs to be modified
                List proj = clientContext.Web.Lists.GetByTitle(project.ProjectName);
                clientContext.Load(proj, properties => properties.Title, properties => properties.RootFolder.Properties);
                clientContext.ExecuteQuery();

                string guid = Convert.ToString(proj.RootFolder.Properties.FieldValues[configuration.GetSection("General")["DocumentMatterGUIDProperty"]]);
                bool isTaskPresent = associatedLists.Contains(project.ProjectName + Constants.TASK);
                bool isCalendarPresent = associatedLists.Contains(project.ProjectName + Constants.CALENDAR);
                bool isRSSFeedPresent = false;//s Convert.ToBoolean(proj.RootFolder.Properties.FieldValues[Constants.ISRSSFEEDPRESENT]);
                string projectId = Convert.ToString(proj.RootFolder.Properties.FieldValues[configuration.GetSection("General")["DocumentMatterIDProperty"]]);

                Console.WriteLine($"Updating the document library from '{project.ProjectName}' to '{project.NewProjectName}'...");
                Utilities.UpdateName(clientContext, project.ProjectName, project.NewProjectName);
                Console.WriteLine($"Successfully updated the document library name from '{project.ProjectName}' to '{project.NewProjectName}'");
                if (isTaskPresent)
                {
                    Utilities.UpdateName(clientContext, project.ProjectName + "_Task", project.NewProjectName + "_Task");
                    Console.WriteLine($"Successfully updated the task name from '{project.ProjectName}_Task' to '{project.NewProjectName}_Task'");
                }
                if (isCalendarPresent)
                {
                    Utilities.UpdateName(clientContext, project.ProjectName + "_Calendar", project.NewProjectName + "_Calendar");
                    Console.WriteLine($"Successfully updated the calendat name from '{project.ProjectName}_Calendar' to '{project.NewProjectName}_Calendar'");
                }

                Utilities.UpdateName(clientContext, project.ProjectName + "_OneNote", project.NewProjectName + "_OneNote");
                Utilities.UpdateOneNoteFolder(clientContext, guid, project.ProjectName, project.NewProjectName);
                Console.WriteLine($"Successfully updated the onenote name from '{project.ProjectName}_Onenote' to '{project.NewProjectName}_Onenote'");

                string documentBatchSize = configuration.GetSection("General")["DocumentUpdateBatchSize"];
                Utilities.UpdateProjectFolders(clientContext, project.ProjectName, project.NewProjectName, documentBatchSize, configuration);
                Console.WriteLine($"Successfully updated the project name column from each file to '{project.NewProjectName}'");
                Utilities.UpdateProjectLandingPage(clientContext, project, guid, isCalendarPresent, isTaskPresent, isRSSFeedPresent, configuration);
                Console.WriteLine($"Successfully updated the project landing page to '{project.NewProjectName}'");


                proj.RootFolder.Properties[configuration.GetSection("General")["ProjectNameStampedProperty"]] = HttpUtility.HtmlEncode(project.NewProjectName);
                proj.RootFolder.Update();
                proj.Update();
                clientContext.ExecuteQuery();

                Console.WriteLine($"Successfully updated project stamped property from '{project.ProjectName}'  to '{project.NewProjectName}'");

                ClientContext catalogContext = null;
                catalogContext = new ClientContext(configuration.GetSection("General")["SharePointCatalogUrl"]);
                catalogContext.Credentials = new SharePointOnlineCredentials(userInfo.UserName, userInfo.Password);
                Utilities.UpdatePinnedProject(catalogContext, guid, project.NewProjectName, configuration);
                Console.WriteLine($"Successfully updated all pinned projects from '{project.ProjectName}'  to '{project.NewProjectName}'");
                Utilities.UpdatePinnedDocument(catalogContext, projectId, project.NewProjectName, configuration);
                Console.WriteLine($"Successfully updated all pinned documents from '{project.ProjectName}'  to '{project.NewProjectName}'");
                
                return true;
            }
            catch(Exception ex)
            {
                throw;
            }            
        }
    }
}
