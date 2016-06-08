// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Repository
// Author           : v-rijadh
// Created          : 06-19-2014
//***************************************************************************
// History
// Modified         : 07-07-2016
// Modified By      : v-lapedd
// ***********************************************************************
// <copyright file="TaxonomyHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to perform SharePoint term store functionalities.</summary>
// ***********************************************************************


using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

#region Matter Namepspaces
using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
#endregion

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This class contains all the methods which will read the SPO term store and returns taxonomy object
    /// </summary>
    public class Taxonomy:ITaxonomy
    {

        #region Properties
        private GeneralSettings generalSettings;
        private LogTables logTables;
        private TaxonomySettings taxonomySettings;
        private ISPOAuthorization spoAuthorization;
        private ClientContext clientContext;
        private TaxonomyResponseVM taxonomyResponseVM;
        private ICustomLogger customLogger;
        #endregion

        /// <summary>
        /// Constructor to inject required dependencies
        /// </summary>
        /// <param name="generalSettings"></param>
        /// <param name="taxonomySettings"></param>
        /// <param name="logTables"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="customLogger"></param>
        public Taxonomy(IOptions<GeneralSettings> generalSettings, IOptions<TaxonomySettings> taxonomySettings, IOptions<LogTables> logTables,
            ISPOAuthorization spoAuthorization, ICustomLogger customLogger)
        {
            this.generalSettings = generalSettings.Value;
            this.taxonomySettings = taxonomySettings.Value;
            this.spoAuthorization = spoAuthorization;
            taxonomyResponseVM = new TaxonomyResponseVM();
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
        }       

        /// <summary>
        /// This method will return the taxonomy hierarchy either for the practice group term set or for client term sets
        /// </summary>
        /// <typeparam name="T">The return type from this class</typeparam>
        /// <param name="clientContext">The sharepoint client context</param>
        /// <param name="termStoreDetails">The term store deatils that client has passed to we apiu</param>
        /// <param name="generalSettings">The general settings config values</param>
        /// <param name="taxonomySettings">The taxonomy settings config values</param>
        /// <returns></returns>
        public TaxonomyResponseVM GetTaxonomyHierarchy(Client client,  TermStoreDetails termStoreDetails)
        {            
            try
            {
                using (clientContext = spoAuthorization.GetClientContext(client.Url))
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
                    taxonomyResponseVM = GetReturnFlag(termStore, termStoreDetails);
                }
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return taxonomyResponseVM;
        }

        /// <summary>
        /// Iterates through the taxonomy hierarchy for the specified term set group.
        /// </summary>
        /// <param name="clientContext">Client content for specified site</param>
        /// <param name="termStore">Term Store object</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Fetch Group Terms Status</returns>
        private TaxonomyResponseVM GetReturnFlag (TermStore termStore, TermStoreDetails termStoreDetails)
        {
            try {
                if (generalSettings.IsTenantDeployment)
                {
                    foreach (TermGroup termGroup in termStore.Groups)
                    {
                        if (termGroup.Name == termStoreDetails.TermGroup)
                        {
                            taxonomyResponseVM = FetchGroupTerms(termGroup, termStoreDetails);
                            break;
                        }
                    }
                }
                else
                {
                    TermGroup termGroup = termStore.GetSiteCollectionGroup(clientContext.Site, false);
                    taxonomyResponseVM = FetchGroupTerms(termGroup, termStoreDetails);
                }
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }           
            return taxonomyResponseVM;
        }

        /// <summary>
        /// Fetches the term store data for the term group specified
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="termGroup">Term Group from the Term store</param>
        /// <param name="termStoreDetails">Details of the Term Group</param>
        /// <param name="returnFlagStatus">hierarchy of the Term Group</param>
        /// <returns>Hierarchy of the Term Group</returns>
        private TaxonomyResponseVM FetchGroupTerms(TermGroup termGroup,
            TermStoreDetails termStoreDetails)
        {            
            try
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
                        if (termStoreDetails.TermSetName == taxonomySettings.PracticeGroupTermSetName)
                        {
                            taxonomyResponseVM.TermSets = GetPracticeGroupTermSetHierarchy(termSet, termStoreDetails);
                        }
                        else if (termStoreDetails.TermSetName == taxonomySettings.ClientTermSetName)
                        {
                            taxonomyResponseVM.ClientTermSets = GetClientTermSetHierarchy(termSet, termStoreDetails);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return taxonomyResponseVM;
        }

        /// <summary>
        /// Gets the practice group term set hierarchy.
        /// </summary>
        /// <param name="termSet">Term set object holding Practice Group terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Term Set</returns>
        private TermSets GetPracticeGroupTermSetHierarchy(TermSet termSet, TermStoreDetails termStoreDetails)
        {
            TermSets tempTermSet = new TermSets();
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
                        if (customProperty.Key.Equals(taxonomySettings.PracticeGroupCustomPropertyFolderNames, StringComparison.Ordinal))
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
                            if (customProperty.Key.Equals(taxonomySettings.AreaCustomPropertyFolderNames, StringComparison.Ordinal))
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
                                else if (customProperty.Key.Equals(taxonomySettings.SubAreaCustomPropertyFolderNames, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.FolderNames = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(taxonomySettings.SubAreaCustomPropertyisNoFolderStructurePresent, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.IsNoFolderStructurePresent = customProperty.Value;
                                }
                                else if (customProperty.Key.Equals(taxonomySettings.SubAreaOfLawDocumentTemplates, StringComparison.Ordinal))
                                {
                                    tempTermSubArea.DocumentTemplateNames = customProperty.Value;
                                }
                            }

                            tempTermArea.SubareaTerms.Add(tempTermSubArea);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            
            return tempTermSet;
        }

        /// <summary>
        /// Gets the taxonomy hierarchy for client term set.
        /// </summary>
        /// <param name="termSet">Term set object holding Client terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Client Term Set</returns>
        private ClientTermSets GetClientTermSetHierarchy(TermSet termSet, TermStoreDetails termStoreDetails)
        {
            ClientTermSets tempClientTermSet = new ClientTermSets();
            try
            {
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

                            if (customProperty.Key.Equals(taxonomySettings.ClientCustomPropertiesId, StringComparison.Ordinal))
                            {
                                tempTermPG.Id = customProperty.Value;
                            }
                        }
                    }
                    tempClientTermSet.ClientTerms.Add(tempTermPG);
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }            
            return tempClientTermSet;
        }       
    }
}
