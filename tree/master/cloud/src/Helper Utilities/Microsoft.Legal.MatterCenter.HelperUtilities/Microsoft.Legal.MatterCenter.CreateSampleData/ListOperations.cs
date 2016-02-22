// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : MAQ Software
// Created          : 04-27-2015
//
// ***********************************************************************
// <copyright file="ListOperations.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Text;
    #endregion

    /// <summary>
    /// Class containing methods related to lists
    /// </summary>
    public static class ListOperations
    {
        /// <summary>
        /// Sets the property value for a list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="props">Property values</param>
        /// <param name="matterName">Matter name</param>
        /// <param name="propertyList">Property list</param>
        internal static void SetPropertyValueForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            List list = clientContext.Web.Lists.GetByTitle(matterName);

            foreach (var item in propertyList)
            {
                props[item.Key] = item.Value;
                list.RootFolder.Update();
            }
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Updates metadata for list
        /// </summary>
        /// <param name="clientContext">CLient context</param>
        /// <param name="matter">Matter object</param>
        /// <param name="client">Client object</param>
        /// <returns>String value</returns>
        internal static string UpdateMetadataForList(ClientContext clientContext, Matter matter, Client client)
        {
            string properties = string.Empty;
            try
            {
                var props = clientContext.Web.Lists.GetByTitle(matter.MatterName).RootFolder.Properties;
                List<string> keys = new List<string>();
                keys.Add("PracticeGroup");
                keys.Add("AreaOfLaw");
                keys.Add("SubAreaOfLaw");
                keys.Add("MatterName");
                keys.Add("MatterID");
                keys.Add("ClientName");
                keys.Add("ClientID");
                keys.Add("ResponsibleAttorney");
                keys.Add("TeamMembers");
                keys.Add("IsMatter");
                keys.Add("OpenDate");
                keys.Add("SecureMatter");
                keys.Add("BlockedUploadUsers");
                keys.Add("Success");
                keys.Add("IsConflictIdentified");
                keys.Add("MatterConflictCheckBy");
                keys.Add("MatterConflictCheckDate");
                keys.Add("MatterCenterPermissions");
                keys.Add("MatterCenterRoles");
                keys.Add("MatterCenterUsers");
                keys.Add("DocumentTemplateCount");
                keys.Add("MatterCenterDefaultContentType");
                keys.Add("MatterDescription");
                keys.Add("BlockedUsers");
                keys.Add("MatterGUID");

                Dictionary<string, string> propertyList = new Dictionary<string, string>();
                propertyList.Add("PracticeGroup", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterType.PracticeGroup));
                propertyList.Add("AreaOfLaw", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterType.AreaofLaw));
                propertyList.Add("SubAreaOfLaw", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterType.SubAreaofLaw));
                propertyList.Add("MatterName", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterName));
                propertyList.Add("MatterID", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterId));
                propertyList.Add("ClientName", Microsoft.Security.Application.Encoder.HtmlEncode(client.ClientName));
                propertyList.Add("ClientID", Microsoft.Security.Application.Encoder.HtmlEncode(client.ClientId));
                propertyList.Add("ResponsibleAttorney", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.ResponsibleAttorneys));
                propertyList.Add("TeamMembers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.Attorneys));
                propertyList.Add("IsMatter", Microsoft.Security.Application.Encoder.HtmlEncode("true"));
                propertyList.Add("OpenDate", Microsoft.Security.Application.Encoder.HtmlEncode(matter.OpenDate));
                propertyList.Add("SecureMatter", Microsoft.Security.Application.Encoder.HtmlEncode(matter.Conflict.SecureMatter));
                propertyList.Add("Success", Microsoft.Security.Application.Encoder.HtmlEncode("true"));
                propertyList.Add("BlockedUsers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.BlockedUsers));
                propertyList.Add("IsConflictIdentified", Microsoft.Security.Application.Encoder.HtmlEncode("true"));
                propertyList.Add("MatterConflictCheckBy", Microsoft.Security.Application.Encoder.HtmlEncode(matter.Conflict.ConflictCheckBy.TrimEnd(';')));
                propertyList.Add("MatterConflictCheckDate", Microsoft.Security.Application.Encoder.HtmlEncode(matter.Conflict.ConflictCheckOn));
                propertyList.Add("MatterCenterRoles", Microsoft.Security.Application.Encoder.HtmlEncode(ConfigurationManager.AppSettings["Roles"]));
                propertyList.Add("MatterCenterUsers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.ResponsibleAttorneys + Constants.SEPARATOR + matter.TeamInfo.Attorneys + Constants.SEPARATOR + matter.TeamInfo.BlockedUploadUsers).TrimEnd(';'));
                propertyList.Add("MatterCenterPermissions", Microsoft.Security.Application.Encoder.HtmlEncode(ConfigurationManager.AppSettings["FullControl"] + Constants.SEPARATOR + ConfigurationManager.AppSettings["Contribute"] + Constants.SEPARATOR + ConfigurationManager.AppSettings["Read"]));
                propertyList.Add("DocumentTemplateCount", Microsoft.Security.Application.Encoder.HtmlEncode(matter.DocumentCount));
                propertyList.Add("MatterCenterDefaultContentType", Microsoft.Security.Application.Encoder.HtmlEncode(matter.DefaultContentType));
                propertyList.Add("MatterDescription", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterDescription));
                propertyList.Add("BlockedUploadUsers", Microsoft.Security.Application.Encoder.HtmlEncode(matter.TeamInfo.BlockedUploadUsers.TrimEnd(';')));
                propertyList.Add("MatterGUID", Microsoft.Security.Application.Encoder.HtmlEncode(matter.MatterGuid));
                propertyList.Add("vti_indexedpropertykeys", Microsoft.Security.Application.Encoder.HtmlEncode(MatterProvisionHelperUtility.GetEncodedValueForSearchIndexProperty(keys)));

                clientContext.Load(props);
                clientContext.ExecuteQuery();

                SetPropertyValueForList(clientContext, props, matter.MatterName, propertyList);
                properties = ListOperations.GetPropertyValueForList(clientContext, matter.MatterName, propertyList);
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "while updating Metadata"));
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "Matter name: " + matter.MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
            return properties;
        }

        /// <summary>
        /// Gets the property value for the list
        /// </summary>
        /// <param name="context">Client context</param>
        /// <param name="matterName">Matter name</param>
        /// <param name="propertyList">Property list</param>
        /// <returns>String value</returns>
        internal static string GetPropertyValueForList(ClientContext context, string matterName, Dictionary<string, string> propertyList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (context != null)
            {
                var props = context.Web.Lists.GetByTitle(matterName).RootFolder.Properties;
                context.Load(props);
                context.ExecuteQuery();
                if (propertyList != null)
                {
                    foreach (var item in propertyList)
                    {
                        if (props.FieldValues.ContainsKey(item.Key))
                        {
                            stringBuilder.Append(props.FieldValues[item.Key].ToString());
                            stringBuilder.Append('|');
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Creates task list while provisioning matter
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="matter">Matter object containing Matter data</param>
        internal static void CreateTaskList(ClientContext clientContext, Matter matter)
        {
            try
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                ListCreationInformation creationInfo = new ListCreationInformation();
                string listName = matter.MatterName + ConfigurationManager.AppSettings["TaskListSuffix"];
                creationInfo.Title = listName;
                creationInfo.Description = matter.MatterDescription;
                // Added list property for URL consolidation changes 
                creationInfo.Url = Constants.TitleListPath + matter.MatterGuid + ConfigurationManager.AppSettings["TaskListSuffix"];
                creationInfo.TemplateType = (int)ListTemplateType.Tasks;
                List list = web.Lists.Add(creationInfo);
                list.ContentTypesEnabled = false;
                list.Update();
                clientContext.ExecuteQuery();
                MatterProvisionHelperUtility.BreakPermission(clientContext, listName, matter.CopyPermissionsFromParent);
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }
    }
}
