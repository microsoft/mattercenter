// ***********************************************************************
// <copyright file="TermStoreOperations.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 04-27-2015
//
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    #endregion

    /// <summary>
    /// This class provides meta data related information for matter provision.
    /// </summary>
    public static class TermStoreOperations
    {
        /// <summary>
        /// Extract term store groups
        /// </summary>
        /// <param name="configVal">Configuration values from excel</param>
        /// <returns>Term sets</returns>
        internal static TermSets FetchGroupTerms(Dictionary<string, string> configVal)
        {
            TermStoreDetails termStoreDetails = new TermStoreDetails();
            termStoreDetails.TermSetName = ConfigurationManager.AppSettings["PracticeGroupTermSetName"];
            termStoreDetails.CustomPropertyName = ConfigurationManager.AppSettings["CustomPropertyName"];
            termStoreDetails.TermGroup = ConfigurationManager.AppSettings["PracticeGroupName"];
            TermSets practiceGroupTermSets = null;

            using (ClientContext clientContext = MatterProvisionHelperUtility.GetClientContext(configVal["TenantAdminURL"], configVal))
            {
                TaxonomySession taxanomySession = TaxonomySession.GetTaxonomySession(clientContext);
                clientContext.Load(taxanomySession,
                                    items => items.TermStores.Include(
                                        item => item.Groups,
                                        item => item.Groups.Include(
                                            group => group.Name)));
                clientContext.ExecuteQuery();
                TermGroup termGroup = taxanomySession.TermStores[0].Groups.GetByName(termStoreDetails.TermGroup);
                clientContext.Load(termGroup,
                                       group => group.Name,
                                       group => group.TermSets.Include(
                                           termSet => termSet.Name,
                                           termSet => termSet.Terms.Include(
                                               term => term.Name,
                                               term => term.Id,
                                               term => term.CustomProperties,
                                               term => term.Terms.Include(
                                                   termArea => termArea.Name,
                                                   termArea => termArea.Id,
                                                   termArea => termArea.CustomProperties,
                                                   termArea => termArea.Terms.Include(
                                                       termSubArea => termSubArea.Name,
                                                       termSubArea => termSubArea.Id,
                                                       termSubArea => termSubArea.CustomProperties)))));
                clientContext.ExecuteQuery();

                foreach (TermSet termSet in termGroup.TermSets)
                {
                    if (termSet.Name.Equals(termStoreDetails.TermSetName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (termStoreDetails.TermSetName.Equals(termStoreDetails.TermSetName, StringComparison.OrdinalIgnoreCase))
                        {
                            practiceGroupTermSets = TermStoreOperations.GetPracticeGroupTermSetHierarchy(termSet, termStoreDetails.CustomPropertyName);
                        }
                    }
                }

                return practiceGroupTermSets;
            }
        }

        /// <summary>
        /// Function to return client id and client url from term store 
        /// </summary>
        /// <param name="configVal">Configuration from excel file</param>
        /// <returns>ClientId and ClientUrl</returns>
        internal static ClientTermSets GetClientDetails(Dictionary<string, string> configVal)
        {
            ClientTermSets clientDetails = new ClientTermSets();
            clientDetails.ClientTerms = new List<Client>();

            string groupName = ConfigurationManager.AppSettings["PracticeGroupName"];
            string termSetName = ConfigurationManager.AppSettings["TermSetName"];
            string clientIdProperty = ConfigurationManager.AppSettings["ClientIDProperty"];
            string clientUrlProperty = ConfigurationManager.AppSettings["ClientUrlProperty"];

            // 1. get client context
            using (ClientContext clientContext = MatterProvisionHelperUtility.GetClientContext(configVal["TenantAdminURL"], configVal))
            {
                if (null != clientContext)
                {
                    // 2. Create taxonomy session
                    TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                    clientContext.Load(taxonomySession.TermStores);
                    clientContext.ExecuteQuery();

                    // 3. Create term store object and load data
                    TermStore termStore = taxonomySession.TermStores[0];
                    clientContext.Load(
                        termStore,
                        store => store.Name,
                        store => store.Groups.Include(
                            group => group.Name));
                    clientContext.ExecuteQuery();

                    // 4. create a term group object and load data
                    TermGroup termGroup = GetTermGroup(groupName, clientContext, termStore);

                    // 5. Get required term from term from extracted term set
                    TermCollection fillteredTerms = termGroup.TermSets.Where(item => item.Name.Equals(termSetName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Terms;
                    GetClientTerms(clientDetails, clientIdProperty, clientUrlProperty, fillteredTerms);
                }
                else
                {
                    clientDetails = null;
                }
            }
            return clientDetails;
        }

        /// <summary>
        /// Method to get termgroup information
        /// </summary>
        /// <param name="groupName">Termstore practice group name</param>
        /// <param name="clientContext">Tenant client context</param>
        /// <param name="termStore">Termstore object</param>
        /// <returns>Termgroup object</returns>
        private static TermGroup GetTermGroup(string groupName, ClientContext clientContext, TermStore termStore)
        {
            TermGroup termGroup = termStore.Groups.Where(item => item.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            clientContext.Load(
                          termGroup,
                           group => group.Name,
                           group => group.TermSets.Include(
                               termSet => termSet.Name,
                               termSet => termSet.Terms.Include(
                                   term => term.Name,
                                   term => term.CustomProperties)));
            clientContext.ExecuteQuery();
            return termGroup;
        }

        /// <summary>
        /// Method to get client terms
        /// </summary>
        /// <param name="clientDetails">client term sets</param>
        /// <param name="clientIdProperty">client id property</param>
        /// <param name="clientUrlProperty">client url property</param>
        /// <param name="fillteredTerms">term collection</param>
        private static void GetClientTerms(ClientTermSets clientDetails, string clientIdProperty, string clientUrlProperty, TermCollection fillteredTerms)
        {
            Client client;
            foreach (Term term in fillteredTerms)
            {
                if (term.CustomProperties.ContainsKey(clientIdProperty) && term.CustomProperties.ContainsKey(clientUrlProperty))
                {
                    client = new Client();
                    client.ClientName = term.Name;
                    client.ClientId = term.CustomProperties[clientIdProperty];
                    client.ClientUrl = term.CustomProperties[clientUrlProperty];
                    clientDetails.ClientTerms.Add(client);
                }
            }
        }

        /// <summary>
        /// Load practice group term set from term store 
        /// </summary>
        /// <param name="termSet">Term set containing term set data</param>
        /// <param name="customPropertyName">Custom property name</param>
        /// <returns>Term sets</returns>
        internal static TermSets GetPracticeGroupTermSetHierarchy(TermSet termSet, string customPropertyName)
        {
            TermSets tempTermSet = new TermSets();
            string practiceGroupCustomPropertyFolderNames = ConfigurationManager.AppSettings["PracticeGroupCustomPropertyFolderNames"];
            string areaCustomPropertyFolderNames = ConfigurationManager.AppSettings["AreaCustomPropertyFolderNames"];
            string subAreaCustomPropertyFolderNames = ConfigurationManager.AppSettings["SubAreaCustomPropertyFolderNames"];
            string subAreaCustomPropertyisNoFolderStructurePresent = ConfigurationManager.AppSettings["SubAreaCustomPropertyisNoFolderStructurePresent"];
            string subAreaOfLawDocumentTemplates = ConfigurationManager.AppSettings["SubAreaOfLawDocumentTemplates"];

            try
            {
                tempTermSet.Name = termSet.Name;
                ////Retrieve the Terms - level 1
                tempTermSet.PGTerms = new List<PracticeGroupTerm>();
                TermCollection termColl = termSet.Terms;
                foreach (Term term in termColl)
                {
                    PracticeGroupTerm tempTermPG = new PracticeGroupTerm();
                    tempTermPG.TermName = term.Name;
                    tempTermPG.Id = Convert.ToString(term.Id, CultureInfo.InvariantCulture);
                    tempTermPG.ParentTermName = termSet.Name;
                    /////Retrieve the custom property for Terms at level 1
                    foreach (KeyValuePair<string, string> customProperty in term.CustomProperties)
                    {
                        tempTermPG.FolderNames = (customProperty.Key.Equals(practiceGroupCustomPropertyFolderNames, StringComparison.OrdinalIgnoreCase)) ? customProperty.Value : string.Empty;
                    }
                    tempTermSet.PGTerms.Add(tempTermPG);
                    ///// Retrieve the Terms - level 2
                    tempTermPG.AreaTerms = new List<AreaTerm>();
                    TermCollection termCollArea = term.Terms;
                    foreach (Term termArea in termCollArea)
                    {
                        AreaTerm tempTermArea = new AreaTerm();
                        tempTermArea.TermName = termArea.Name;
                        tempTermArea.Id = Convert.ToString(termArea.Id, CultureInfo.InvariantCulture);
                        tempTermArea.ParentTermName = term.Name;
                        /////Retrieve the custom property for Terms at level 2
                        foreach (KeyValuePair<string, string> customProperty in termArea.CustomProperties)
                        {
                            tempTermArea.FolderNames = customProperty.Key.Equals(areaCustomPropertyFolderNames, StringComparison.OrdinalIgnoreCase) ? customProperty.Value : string.Empty;
                        }
                        tempTermPG.AreaTerms.Add(tempTermArea);
                        /////Retrieve the Terms - level 3
                        tempTermArea.SubareaTerms = new List<SubareaTerm>();
                        TermCollection termCollSubArea = termArea.Terms;
                        foreach (Term termSubArea in termCollSubArea)
                        {
                            SubareaTerm tempTermSubArea = new SubareaTerm();
                            tempTermSubArea.TermName = termSubArea.Name;
                            tempTermSubArea.Id = Convert.ToString(termSubArea.Id, CultureInfo.InvariantCulture);
                            tempTermSubArea.ParentTermName = termArea.Name;
                            /////Retrieve the custom property for Terms at level 3

                            tempTermSubArea.DocumentTemplates = string.Empty;
                            foreach (KeyValuePair<string, string> customProperty in termSubArea.CustomProperties)
                            {
                                if (customProperty.Key.Equals(customPropertyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.ContentTypeName = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(subAreaCustomPropertyFolderNames, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.FolderNames = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(subAreaCustomPropertyisNoFolderStructurePresent, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.IsNoFolderStructurePresent = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(subAreaOfLawDocumentTemplates, StringComparison.OrdinalIgnoreCase))
                                {
                                    tempTermSubArea.DocumentTemplates = customProperty.Value;
                                }
                            }
                            tempTermArea.SubareaTerms.Add(tempTermSubArea);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                tempTermSet = null;
            }
            return tempTermSet;
        }
    }
}
