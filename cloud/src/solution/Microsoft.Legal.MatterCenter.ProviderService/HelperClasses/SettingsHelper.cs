// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-prd
// Created          : 26-02-2015
//
// ***********************************************************************
// <copyright file="SettingsHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file holds all the functions that are called by settings page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.HelperClasses
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.Security.Application;
    using Microsoft.SharePoint.Client;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Holds all the functions that are called by settings page
    /// </summary>
    internal static class SettingsHelper
    {
        /// <summary>
        /// Gets the user list from the data sent in Matter Configurations
        /// </summary>
        /// <param name="matterUsers">Matter Users list</param>
        /// <returns>Users list</returns>
        internal static IList<IList<string>> GetUserList(string matterUsers)
        {
            IList<IList<string>> result = new List<IList<string>>();
            IList<string> temp = null;
            string[] resultTemp = matterUsers.Split(new string[] { "$|$" }, StringSplitOptions.None);
            foreach (var userItem in resultTemp)
            {
                temp = new List<string>();
                string[] tempItems = userItem.Split(Convert.ToChar(ConstantStrings.Semicolon, CultureInfo.InvariantCulture));
                foreach (var item in tempItems)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        temp.Add(item.Trim());
                    }
                }
                if (0 < temp.Count)
                {
                    result.Add(temp);
                }
                temp = null;
            }
            return result;
        }

        /// <summary>
        /// Save configurations value to SharePoint list
        /// </summary>
        /// <param name="matterConfigurations">Matter configurations</param>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="cachedItemModifiedDate">Date and time when user loaded the client settings page to configure default values</param>
        /// <returns>true or error</returns>
        internal static string SaveConfigurationToList(MatterConfigurations matterConfigurations, ClientContext clientContext, string cachedItemModifiedDate)
        {
            string result = string.Empty;
            try
            {
                string listQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MatterConfigurationsListQuery, SearchConstants.ManagedPropertyTitle, ServiceConstantStrings.MatterConfigurationTitleValue);
                ListItemCollection collection = Lists.GetData(clientContext, ServiceConstantStrings.MatterConfigurationsList, listQuery);
                // Set the default value for conflict check flag
                matterConfigurations.IsContentCheck = ServiceConstantStrings.IsContentCheck;
                if (0 == collection.Count)
                {
                    List<string> columnNames = new List<string>() { ServiceConstantStrings.MatterConfigurationColumn, SearchConstants.ManagedPropertyTitle };
                    List<object> columnValues = new List<object>() { Encoder.HtmlEncode(JsonConvert.SerializeObject(matterConfigurations)), ServiceConstantStrings.MatterConfigurationTitleValue };
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(ServiceConstantStrings.MatterConfigurationsList);
                    Lists.AddItem(clientContext, list, columnNames, columnValues);
                }
                else
                {
                    bool response = Lists.CheckItemModified(collection, cachedItemModifiedDate);
                    if (response)
                    {
                        foreach (ListItem item in collection)
                        {
                            item[ServiceConstantStrings.MatterConfigurationColumn] = Encoder.HtmlEncode(JsonConvert.SerializeObject(matterConfigurations));
                            item.Update();
                            break;
                        }
                    }
                    else
                    {
                        result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectTeamMembersCode, ServiceConstantStrings.IncorrectTeamMembersMessage + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR);
                    }
                }
                if (string.IsNullOrWhiteSpace(result))
                {
                    clientContext.ExecuteQuery();
                    result = ConstantStrings.TRUE;
                }
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }
    }
}