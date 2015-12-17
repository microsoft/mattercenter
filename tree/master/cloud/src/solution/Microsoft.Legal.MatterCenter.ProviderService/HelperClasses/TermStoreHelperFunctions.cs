// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-rijadh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="TermStoreHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to perform SharePoint term store functionalities.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provide methods to perform SharePoint term store functionalities.
    /// </summary>
    internal static class TermStoreHelperFunctions
    {
        /// <summary>
        /// Gets the taxonomy hierarchy.
        /// </summary>
        /// <param name="clientContext">Client object containing Client data</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Fetch Group Terms Status</returns>
        internal static string GetTaxonomyHierarchy(ClientContext clientContext, TermStoreDetails termStoreDetails)
        {
            string returnFlag = ConstantStrings.FALSE;
            try
            {
                TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                TermStore termStore;
                clientContext.Load(taxonomySession.TermStores);
                clientContext.ExecuteQuery();
                termStore = taxonomySession.TermStores[0];
                clientContext.Load(
                    termStore,
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name));
                clientContext.ExecuteQuery();
                returnFlag = GetReturnFlag(clientContext, termStore, termStoreDetails);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            return returnFlag;
        }

        /// <summary>
        /// Fetches the term store data for the term group specified
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="termGroup">Term Group from the Term store</param>
        /// <param name="termStoreDetails">Details of the Term Group</param>
        /// <param name="returnFlagStatus">hierarchy of the Term Group</param>
        /// <returns>Hierarchy of the Term Group</returns>
        internal static string FetchGroupTerms(ClientContext clientContext, TermGroup termGroup, TermStoreDetails termStoreDetails, string returnFlagStatus)
        {
            clientContext.Load(
                                  termGroup,
                                   group => group.Name,
                                   group => group.TermSets.Include(
                                       termSet => termSet.Name,
                                       termSet => termSet.Id,
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
                if (termSet.Name == termStoreDetails.TermSetName)
                {
                    if (termStoreDetails.TermSetName == ServiceConstantStrings.PracticeGroupTermSetName)
                    {
                        returnFlagStatus = GetPracticeGroupTermSetHierarchy(termSet, termStoreDetails);
                    }
                    else if (termStoreDetails.TermSetName == ServiceConstantStrings.ClientTermSetName)
                    {
                        returnFlagStatus = GetClientTermSetHierarchy(termSet, termStoreDetails);
                    }
                }
            }

            return returnFlagStatus;
        }

        /// <summary>
        /// Iterates through the taxonomy hierarchy for the specified term set group.
        /// </summary>
        /// <param name="clientContext">Client content for specified site</param>
        /// <param name="termStore">Term Store object</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Fetch Group Terms Status</returns>
        internal static string GetReturnFlag(ClientContext clientContext, TermStore termStore, TermStoreDetails termStoreDetails)
        {
            string returnFlagStatus = ConstantStrings.FALSE;

            if (ServiceConstantStrings.IsTenantDeployment)
            {
                foreach (TermGroup termGroup in termStore.Groups)
                {
                    if (termGroup.Name == termStoreDetails.TermGroup)
                    {
                        returnFlagStatus = FetchGroupTerms(clientContext, termGroup, termStoreDetails, returnFlagStatus);
                    }
                }
            }
            else
            {
                TermGroup termGroup = termStore.GetSiteCollectionGroup(clientContext.Site, false);
                returnFlagStatus = FetchGroupTerms(clientContext, termGroup, termStoreDetails, returnFlagStatus);
            }

            // Returns the taxonomy hierarchy for the specified term set group
            return returnFlagStatus;
        }

        /// <summary>
        /// Gets the practice group term set hierarchy.
        /// </summary>
        /// <param name="termSet">Term set object holding Practice Group terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Term Set</returns>
        internal static string GetPracticeGroupTermSetHierarchy(TermSet termSet, TermStoreDetails termStoreDetails)
        {
            string result = string.Empty;
            try
            {
                TermSets tempTermSet = new TermSets();
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
                        if (customProperty.Key.Equals(ServiceConstantStrings.PracticeGroupCustomPropertyFolderNames, StringComparison.Ordinal))
                        {
                            tempTermPG.FolderNames = customProperty.Value;
                        }
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
                            if (customProperty.Key.Equals(ServiceConstantStrings.AreaCustomPropertyFolderNames, StringComparison.Ordinal))
                            {
                                tempTermArea.FolderNames = customProperty.Value;
                            }
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
                                if (customProperty.Key.Equals(termStoreDetails.CustomPropertyName, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.DocumentTemplates = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(ServiceConstantStrings.SubAreaCustomPropertyFolderNames, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.FolderNames = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(ServiceConstantStrings.SubAreaCustomPropertyisNoFolderStructurePresent, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.IsNoFolderStructurePresent = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(ServiceConstantStrings.SubAreaOfLawDocumentTemplates, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.DocumentTemplateNames = customProperty.Value;
                                }
                            }

                            tempTermArea.SubareaTerms.Add(tempTermSubArea);
                        }
                    }
                }
                /////Serialize the Term set (Practice Group) object to get all terms under it and return it
                result = JsonConvert.SerializeObject(tempTermSet);
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Gets the taxonomy hierarchy for client term set.
        /// </summary>
        /// <param name="termSet">Term set object holding Client terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Client Term Set</returns>
        internal static string GetClientTermSetHierarchy(TermSet termSet, TermStoreDetails termStoreDetails)
        {
            string result = string.Empty;
            try
            {
                ClientTermSets tempClientTermSet = new ClientTermSets();
                tempClientTermSet.Name = termSet.Name;
                /////Retrieve the Terms - level 1
                tempClientTermSet.ClientTerms = new List<Client>();
                TermCollection termColl = termSet.Terms;
                foreach (Term term in termColl)
                {
                    Client tempTermPG = new Client();
                    tempTermPG.Name = term.Name;
                    if (term.CustomProperties.Count > 0)
                    {
                        tempTermPG.Url = string.Empty;
                        tempTermPG.Id = string.Empty;
                        foreach (KeyValuePair<string, string> customProperty in term.CustomProperties)
                        {
                            if (customProperty.Key.Equals(termStoreDetails.CustomPropertyName, StringComparison.Ordinal))
                            {
                                tempTermPG.Url = customProperty.Value;
                            }

                            if (customProperty.Key.Equals(ServiceConstantStrings.ClientCustomPropertiesId, StringComparison.Ordinal))
                            {
                                tempTermPG.Id = customProperty.Value;
                            }
                        }
                    }

                    tempClientTermSet.ClientTerms.Add(tempTermPG);
                }
                /////Serialize the Term set (Practice Group) object to get all terms under it
                result = JsonConvert.SerializeObject(tempClientTermSet);
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }
    }
}