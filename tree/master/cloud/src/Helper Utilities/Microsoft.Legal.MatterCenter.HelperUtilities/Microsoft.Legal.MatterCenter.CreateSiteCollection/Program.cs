// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSiteCollection
// Author           : v-rijadh
// Created          : 08-27-2015
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains utility to create site collection.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSiteCollection
{
    #region using

    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;

    #endregion using

    /// <summary>
    /// A class is entry point of the application
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// A variable to save file path for logging error
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// This method is the entry point for this application
        /// </summary>
        /// <param name="args">input from console</param>
        private static void Main(string[] args)
        {
            string configSheet = ConfigurationManager.AppSettings["configsheetname"];
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
            Dictionary<string, string> configVal = ExcelOperations.ReadFromExcel(filePath, configSheet);
            if (2 <= args.Length)
            {
                string login = args[1], password = args[2];
                bool createSiteCollections = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture);
                configVal.Add("Username", login);
                if (!ExcelOperations.IsNullOrEmptyCredential(login, password))
                {
                    try
                    {
                        Collection<Collection<string>> clientVal = ExcelOperations.ReadSheet(filePath, ConfigurationManager.AppSettings["clientsheetname"]);
                        string targetSite = configVal["TenantAdminURL"];
                        string tenantSite = configVal["TenantURL"];

                        using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password ))
                        {
                            using (SecureString securePassword = new SecureString())
                            {
                                foreach (char letter in password)
                                {
                                    securePassword.AppendChar(letter);
                                }
                                SharePointOnlineCredentials onlineCredentials = new SharePointOnlineCredentials(login, securePassword);

                                // Activate feature on tenant site collection
                                ActivateFeature(tenantSite, onlineCredentials);
                                for (int count = 1; count < clientVal.Count; count++)
                                {
                                    string clientName = clientVal[count][0];
                                    string clientUrl = clientVal[count][2];
                                    string siteOwners = clientVal[count][3];
                                    string siteVisitors = clientVal[count][4];
                                    if (createSiteCollections)
                                    {
                                        CreateSiteCollections(clientContext, configVal, clientUrl, clientName);
                                        CreateRestrictedGroup(clientUrl, onlineCredentials);

                                        //// Check if the user list for the group is empty
                                        if (!string.IsNullOrEmpty(siteOwners))
                                        {
                                            AssignPermissions(clientUrl, onlineCredentials, ConfigurationManager.AppSettings["Owners Group"], siteOwners);
                                        }

                                        //// Check if the user list for the group is empty
                                        if (!string.IsNullOrEmpty(siteVisitors))
                                        {
                                            AssignPermissions(clientUrl, onlineCredentials, ConfigurationManager.AppSettings["Visitors Group"], siteVisitors);
                                        }

                                        ActivateFeature(clientUrl, onlineCredentials);
                                    }
                                    else
                                    {
                                        DeleteSiteCollection(clientContext, clientUrl);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorLogger.DisplayErrorMessage(exception.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Command-line parameters are missing. Provide {0} Action(true/false) {1} Username and {2} Password");
            }
        }

        /// <summary>
        /// Function is used to create group on each and every site collection with specified permissions
        /// </summary>
        /// <param name="clientUrl">Client url</param>
        /// <param name="credentials">Credentials object</param>
        /// <param name="groupName">Group name</param>
        /// <param name="members">Members object</param>
        /// <returns>Either error or empty</returns>
        internal static string AssignPermissions(string clientUrl, SharePointOnlineCredentials credentials, string groupName, string members)
        {
            string result = string.Empty;
            try
            {
                using (ClientContext clientContext = new ClientContext(clientUrl))
                {
                    clientContext.Credentials = credentials;
                    Web web = clientContext.Web;
                    clientContext.Load(web, item => item.SiteGroups);
                    clientContext.Load(web, item => item.Title);
                    clientContext.ExecuteQuery();
                    string webTitle = web.Title;
                    groupName = string.Format(CultureInfo.InvariantCulture, groupName, webTitle);

                    Console.WriteLine("Adding users to group: {0}", groupName);

                    // Remove group if it is already present
                    Group group = web.SiteGroups.Where(item => item.Title == groupName).FirstOrDefault();
                    if (null != group)
                    {
                        string[] allUserEmail = members.Split(new char[] { ';' });
                        //Adding users to the current group
                        List<User> allUsers = new List<User>();
                        foreach (string userEmail in allUserEmail)
                        {
                            if (!string.IsNullOrEmpty(userEmail))
                            {
                                User user = clientContext.Web.EnsureUser(userEmail.Trim());
                                clientContext.Load(user);
                                clientContext.ExecuteQuery();
                                allUsers.Add(user);
                            }
                        }
                        foreach (User user in allUsers)
                        {
                            group.Users.AddUser(user);
                        }
                        web.Update();
                        clientContext.Load(group);
                        clientContext.ExecuteQuery();
                        Console.WriteLine("Successfully added users to group: " + group.Title);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception occurred while assigning permissions: " + exception.Message);
                result = exception.Message;
            }
            return result;
        }

        /// <summary>
        /// Activates SharePoint feature
        /// </summary>
        /// <param name="clientUrl">Client Url</param>
        /// <param name="credentials">SharePoint Credentials</param>
        /// <returns>Flag if feature activated</returns>
        public static bool ActivateFeature(string clientUrl, SharePointOnlineCredentials credentials)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(clientUrl))
                {
                    clientContext.Credentials = credentials;
                    Guid featureId = new Guid(ConfigurationManager.AppSettings["DocumentFeatureID"]);
                    FeatureCollection features = clientContext.Site.Features;

                    clientContext.Load(features);
                    clientContext.ExecuteQuery();
                    var isFeatureActivated = (from feature in features where feature.DefinitionId == featureId select feature).FirstOrDefault();

                    if (null != isFeatureActivated)
                    {
                        // Feature Activated
                        return false;
                    }
                    else
                    {
                        // Activate the feature
                        features.Add(featureId, false, FeatureDefinitionScope.Farm);
                        clientContext.ExecuteQuery();
                        return true;
                    }
                }
            }
            // SharePoint Specific Exception
            catch (ClientRequestException clientRequestException)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + clientRequestException.Message + "\nStacktrace: " + clientRequestException.StackTrace);
            }
            // SharePoint Specific Exception
            catch (ServerException serverException)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + serverException.Message + "\nStacktrace: " + serverException.StackTrace);
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Method to create site collections
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="configVal">Values in Config sheet</param>
        /// <param name="clientUrl">Url for Site Collection</param>
        /// <param name="clientTitle">Name of Site Collection</param>
        internal static void CreateSiteCollections(ClientContext clientContext, Dictionary<string, string> configVal, string clientUrl, string clientTitle)
        {
            try
            {
                Tenant tenant = new Tenant(clientContext);
                clientContext.Load(tenant);
                clientContext.ExecuteQuery(); //login into SharePoint Online

                SPOSitePropertiesEnumerable spoSiteProperties = tenant.GetSiteProperties(0, true);
                clientContext.Load(spoSiteProperties);
                clientContext.ExecuteQuery();
                SiteProperties siteProperties = (from properties in spoSiteProperties
                                                 where properties.Url.ToString().ToUpper() == clientUrl.ToUpper()
                                                 select properties).FirstOrDefault();

                if (null != siteProperties)
                {
                    // site exists
                    Console.WriteLine(clientUrl + " already exists...");
                    return;
                }
                else
                {
                    // site does not exists
                    SiteCreationProperties newSite = new SiteCreationProperties()
                    {
                        Url = clientUrl,
                        Owner = configVal["Username"],
                        Template = ConfigurationManager.AppSettings["template"], //using the team site template, check the MSDN if you want to use other template
                        StorageMaximumLevel = Convert.ToInt64(ConfigurationManager.AppSettings["storageMaximumLevel"], CultureInfo.InvariantCulture), //1000
                        UserCodeMaximumLevel = Convert.ToDouble(ConfigurationManager.AppSettings["userCodeMaximumLevel"], CultureInfo.InvariantCulture), //300
                        Title = clientTitle,
                        CompatibilityLevel = 15, //15 means SharePoint online 2013, 14 means SharePoint online 2010
                    };
                    SpoOperation spo = tenant.CreateSite(newSite);
                    clientContext.Load(tenant);
                    clientContext.Load(spo, operation => operation.IsComplete);

                    clientContext.ExecuteQuery();

                    Console.WriteLine("Creating site collection at " + clientUrl);
                    Console.WriteLine("Loading.");
                    //Check if provisioning of the SiteCollection is complete.
                    while (!spo.IsComplete)
                    {
                        Console.Write(".");
                        //Wait for 30 seconds and then try again
                        System.Threading.Thread.Sleep(30000);
                        spo.RefreshLoad();
                        clientContext.ExecuteQuery();
                    }
                    Console.WriteLine("Site Collection: " + clientUrl + " is created successfully");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception occurred while creating site collection: " + exception.Message);
            }
        }

        /// <summary>
        /// Method for deleting site collections
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="clientUrl">Url for Site Collection</param>
        internal static void DeleteSiteCollection(ClientContext clientContext, string clientUrl)
        {
            try
            {
                Tenant tenant = new Tenant(clientContext);
                clientContext.Load(tenant);
                clientContext.ExecuteQuery(); //login into SharePoint online

                SPOSitePropertiesEnumerable spSiteProperties = tenant.GetSiteProperties(0, true);
                clientContext.Load(spSiteProperties);
                clientContext.ExecuteQuery();

                SiteProperties siteProperties = (from properties in spSiteProperties
                                                 where properties.Url.ToString().ToUpper() == clientUrl.ToUpper()
                                                 select properties).FirstOrDefault();

                if (null != siteProperties)
                {
                    // site exists
                    // delete the site
                    SpoOperation spoOperation = tenant.RemoveSite(clientUrl);
                    clientContext.Load(spoOperation, operation => operation.IsComplete);
                    clientContext.ExecuteQuery();

                    while (!spoOperation.IsComplete)
                    {
                        Console.Write(".");
                        //Wait for 30 seconds and then try again
                        System.Threading.Thread.Sleep(30000);
                        spoOperation.RefreshLoad();
                        clientContext.ExecuteQuery();
                    }
                    Console.WriteLine("Site Collection: " + clientUrl + " is deleted successfully");
                }
                else
                {
                    // site does not exists
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception occurred while deleting site collection: " + exception.Message);
            }
        }

        /// <summary>
        /// Creates group with restricted access permissions on client site collection to allow functionality in matter landing page
        /// </summary>
        /// <param name="siteUrl">URL of the client site collection to create group at</param>
        /// <param name="onlineCredentials">credentials to access site</param>
        internal static void CreateRestrictedGroup(string siteUrl, SharePointOnlineCredentials onlineCredentials)
        {
            try
            {
                Console.WriteLine("Creating " + ConfigurationManager.AppSettings["restrictedAccessGroupName"] + " group.");

                using (ClientContext clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = onlineCredentials;
                    Site collSite = clientContext.Site;
                    Web site = clientContext.Web;

                    //Create group
                    GroupCollection collGroup = site.SiteGroups;
                    clientContext.Load(collGroup, group => group.Include(properties => properties.Title));
                    clientContext.Load(site.RoleDefinitions);
                    clientContext.ExecuteQuery();

                    Group currentGrp = (from grp in collGroup where grp.Title == ConfigurationManager.AppSettings["restrictedAccessGroupName"] select grp).FirstOrDefault();
                    if (currentGrp != null)
                    {
                        collGroup.Remove(currentGrp);
                    }

                    GroupCreationInformation grpInfo = new GroupCreationInformation();
                    grpInfo.Title = ConfigurationManager.AppSettings["restrictedAccessGroupName"];
                    grpInfo.Description = ConfigurationManager.AppSettings["restrictedAccessGroupDescription"];
                    collGroup.Add(grpInfo);
                    site.Update();
                    clientContext.Load(collGroup);
                    clientContext.ExecuteQuery();

                    currentGrp = (from grp in collGroup where grp.Title == ConfigurationManager.AppSettings["restrictedAccessGroupName"] select grp).FirstOrDefault();

                    AssignPermissionToGroup(clientContext, collSite, site, currentGrp);

                    //Add everyone to group
                    User allUsers = clientContext.Web.EnsureUser(ConfigurationManager.AppSettings["allUsers"]);
                    clientContext.Load(allUsers);
                    clientContext.ExecuteQuery();

                    currentGrp.Users.AddUser(allUsers);
                    site.Update();
                    clientContext.Load(currentGrp);
                    clientContext.ExecuteQuery();

                    Console.WriteLine("Created " + ConfigurationManager.AppSettings["restrictedAccessGroupName"] + " group successfully");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception occurred while creating group: " + exception.Message);
            }
        }

        /// <summary>
        /// Assign permission to the group
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="collSite">Site object</param>
        /// <param name="site">Web object</param>
        /// <param name="currentGrp">Group object</param>
        private static void AssignPermissionToGroup(ClientContext clientContext, Site collSite, Web site, Group currentGrp)
        {
            //Create permission role and assign group to the role
            RoleDefinitionBindingCollection collRDB = new RoleDefinitionBindingCollection(clientContext);
            RoleDefinition roleDef = site.RoleDefinitions.FirstOrDefault(roles => roles.Name == ConfigurationManager.AppSettings["restrictedAccessRoleName"]);
            if (roleDef == null)
            {
                BasePermissions permissions = new BasePermissions();
                permissions.Set(PermissionKind.UseRemoteAPIs);
                permissions.Set(PermissionKind.Open);

                RoleDefinitionCreationInformation rdcInfo = new RoleDefinitionCreationInformation();
                rdcInfo.Name = ConfigurationManager.AppSettings["restrictedAccessRoleName"];
                rdcInfo.Description = ConfigurationManager.AppSettings["restrictedAccessRoleDescription"];
                rdcInfo.BasePermissions = permissions;
                roleDef = collSite.RootWeb.RoleDefinitions.Add(rdcInfo);
            }
            collRDB.Add(roleDef);
            site.RoleAssignments.Add(currentGrp, collRDB);
            clientContext.Load(currentGrp);
            clientContext.Load(collRDB);
            clientContext.ExecuteQuery();
        }
    }
}