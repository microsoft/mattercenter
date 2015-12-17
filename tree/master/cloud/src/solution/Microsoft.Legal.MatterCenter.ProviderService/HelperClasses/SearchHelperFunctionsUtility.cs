// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-shpate
// Created          : 06-06-2015
//
// ***********************************************************************
// <copyright file="SearchHelperFunctionsUtility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is being used by Search service.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.HelperClasses
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Globalization;
    using System.Linq;
    #endregion

    internal class SearchHelperFunctionsUtility
    {

        /// <summary>
        /// Retrieves the document and List GUID
        /// </summary>
        /// <param name="client">Client object containing list data</param>        
        /// <param name="clientContext">Client Context</param>     
        /// <returns>Returns the document and List GUID</returns>
        internal static string GetDocumentAndClientGUID(Client client, ClientContext clientContext)
        {
            string listInternalName = string.Empty, documentGUID = string.Empty, result = string.Empty;
            ListCollection lists = clientContext.Web.Lists;
            clientContext.Load(lists, list => list.Include(listItem => listItem.Id, listItem => listItem.RootFolder.ServerRelativeUrl));
            Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(client.Id);
            clientContext.Load(file, files => files.ListItemAllFields);
            clientContext.ExecuteQuery();
            if (0 < file.ListItemAllFields.FieldValues.Count)
            {
                documentGUID = Convert.ToString(file.ListItemAllFields.FieldValues[ServiceConstantStrings.DocumentGUIDColumnName], CultureInfo.InvariantCulture);
            }

            List retrievedList = (from list in lists
                                  where list.RootFolder.ServerRelativeUrl.ToUpperInvariant().Equals(client.Name.ToUpperInvariant())
                                  select list).FirstOrDefault();
            if (null != retrievedList)
            {
                listInternalName = Convert.ToString(retrievedList.Id, CultureInfo.InvariantCulture);
            }
            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponseForSearch, listInternalName + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + documentGUID);

            return result;
        }
    }
}