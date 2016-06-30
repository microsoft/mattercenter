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
using Microsoft.Extensions.Options;
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
        public Taxonomy(IOptionsMonitor<GeneralSettings> generalSettings, IOptionsMonitor<TaxonomySettings> taxonomySettings, IOptionsMonitor<LogTables> logTables,
            ISPOAuthorization spoAuthorization, ICustomLogger customLogger)
        {
            this.generalSettings = generalSettings.CurrentValue;
            this.taxonomySettings = taxonomySettings.CurrentValue;
            this.spoAuthorization = spoAuthorization;
            taxonomyResponseVM = new TaxonomyResponseVM();
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
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
                                           term => term.TermsCount,
                                           term => term.CustomProperties
                                           )));

                clientContext.ExecuteQuery();


                foreach (TermSet termSet in termGroup.TermSets)
                {
                    if (termSet.Name == termStoreDetails.TermSetName)
                    {
                        if (termStoreDetails.TermSetName == taxonomySettings.PracticeGroupTermSetName)
                        {
                            taxonomyResponseVM.TermSets = GetPracticeGroupTermSetHierarchy(clientContext, termSet, termStoreDetails);
                        }
                        else if (termStoreDetails.TermSetName == taxonomySettings.ClientTermSetName)
                        {
                            taxonomyResponseVM.ClientTermSets = GetClientTermSetHierarchy(clientContext, termSet, termStoreDetails);
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
        private TermSets GetPracticeGroupTermSetHierarchy(ClientContext clientContext, TermSet termSet, TermStoreDetails termStoreDetails)
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
                    //Add Level 1 to the term collection
                    tempTermSet.PGTerms.Add(tempTermPG);
                    if(term.TermsCount>0)
                    {
                        TermCollection termCollLevel2 = LoadTerms(term, clientContext);
                        tempTermPG.AreaTerms = new List<AreaTerm>();
                        foreach (Term termLevel2 in termCollLevel2)
                        {
                            AreaTerm tempTermArea = new AreaTerm();
                            tempTermArea.TermName = termLevel2.Name;
                            tempTermArea.Id = Convert.ToString(termLevel2.Id, CultureInfo.InvariantCulture);
                            tempTermArea.ParentTermName = term.Name;
                            /////Retrieve the custom property for Terms at level 2
                            foreach (KeyValuePair<string, string> customProperty in termLevel2.CustomProperties)
                            {
                                if (customProperty.Key.Equals(taxonomySettings.AreaCustomPropertyFolderNames, StringComparison.Ordinal))
                                {
                                    tempTermArea.FolderNames = customProperty.Value;
                                }
                            }
                            //Add Level 2 to the term collection
                            tempTermPG.AreaTerms.Add(tempTermArea);
                            if(termLevel2.TermsCount>0)
                            {   
                                TermCollection termCollLevel3 = LoadTerms(termLevel2, clientContext);
                                //Add Level 3 to the term collection
                                tempTermArea.SubareaTerms = UpdateLevelTerm(termCollLevel3, termStoreDetails);

                                int termCount = 0;
                                foreach (Term termLevel3 in termCollLevel3)
                                {
                                    if (termLevel3.TermsCount > 0)
                                    {
                                        TermCollection termCollLevel4 = LoadTerms(termLevel3, clientContext);
                                        //Add Level 4 to the term collection
                                        tempTermArea.SubareaTerms[termCount].SubareaTerms = UpdateLevelTerm(termCollLevel4, termStoreDetails);
                                        int termCount1 = 0;
                                        foreach (Term termLevel4 in termCollLevel4)
                                        {
                                            if (termLevel4.TermsCount > 0)
                                            {
                                                TermCollection termCollLevel5 = LoadTerms(termLevel4, clientContext);
                                                //Add Level 5 to the term collection
                                                tempTermArea.SubareaTerms[termCount].SubareaTerms[termCount1].SubareaTerms = 
                                                    UpdateLevelTerm(termCollLevel5, termStoreDetails);
                                            }
                                            termCount1 = termCount1 + 1;
                                        }
                                    }
                                    termCount = termCount + 1;
                                }
                            }
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
        /// This method will update the taxonomy hierarchy object with custom properties that needs to be send to client
        /// </summary>
        /// <param name="termCollection">The Term Collection object to which terms will be added</param>
        /// <param name="termStoreDetails">The term store details which the client has sent</param>
        /// <returns></returns>
        private List<SubareaTerm> UpdateLevelTerm(TermCollection termCollection, TermStoreDetails termStoreDetails)
        {
            var subAreaTerms = new List<SubareaTerm>();
            foreach (Term termLevel3 in termCollection)
            {
                SubareaTerm tempTermSubArea = new SubareaTerm();
                tempTermSubArea.TermName = termLevel3.Name;
                tempTermSubArea.Id = Convert.ToString(termLevel3.Id, CultureInfo.InvariantCulture);
                tempTermSubArea.ParentTermName = termLevel3.Name;
                /////Retrieve the custom property for Terms at level 3

                tempTermSubArea.DocumentTemplates = string.Empty;
                foreach (KeyValuePair<string, string> customProperty in termLevel3.CustomProperties)
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
                subAreaTerms.Add(tempTermSubArea);
            }
            return subAreaTerms;
        }

        
        /// <summary>
        /// This method will load the child terms for a particular parent term
        /// </summary>
        /// <param name="term">Parent Term</param>
        /// <param name="context">SharePoint Context</param>
        /// <returns></returns>
        private TermCollection LoadTerms(Term term, ClientContext context)
        {            
            TermCollection termCollection = term.Terms;
            context.Load(termCollection,
                tc => tc.Include(
                    t => t.TermsCount,
                    t=>t.Id,
                    t => t.Name,
                    t => t.TermsCount,
                    t => t.CustomProperties
                )
            );
            context.ExecuteQuery();
            return termCollection;              
        }

        /// <summary>
        /// Gets the taxonomy hierarchy for client term set.
        /// </summary>
        /// <param name="termSet">Term set object holding Client terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Client Term Set</returns>
        private ClientTermSets GetClientTermSetHierarchy(ClientContext clientContext, TermSet termSet, TermStoreDetails termStoreDetails)
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
