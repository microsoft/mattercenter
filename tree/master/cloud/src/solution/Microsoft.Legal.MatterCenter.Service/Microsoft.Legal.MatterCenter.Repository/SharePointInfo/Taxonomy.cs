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
using Microsoft.Legal.MatterCenter.Repository.Extensions;
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
        private readonly TaxonomySettings taxonomySettings;
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
        public Taxonomy(IOptionsMonitor<GeneralSettings> generalSettings, 
            IOptionsMonitor<TaxonomySettings> taxonomySettings, 
            IOptionsMonitor<LogTables> logTables,
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
                    taxonomyResponseVM = GetTaxonomyHierarchy(termStore, termStoreDetails);
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
        private TaxonomyResponseVM GetTaxonomyHierarchy (TermStore termStore, TermStoreDetails termStoreDetails)
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
                //As a first step, load Practice Group Term set and anf its child terms only and not its child child terms                
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

                //This loop of the term set will get all the client termset and practice group term sets
                foreach (TermSet termSet in termGroup.TermSets)
                {
                    if (termSet.Name == termStoreDetails.TermSetName)
                    {
                        //This condition is if the UI is requesting practice group terms
                        if (termStoreDetails.TermSetName == taxonomySettings.PracticeGroupTermSetName)
                        {
                            taxonomyResponseVM.TermSets = GetPracticeGroupTermSetHierarchy(clientContext, termSet, termStoreDetails);
                        }
                        //This condition is if the UI is requesting client terms and the clients are defined as term set and not terms
                        else if (termStoreDetails.TermSetName == taxonomySettings.ClientTermSetName && 
                            taxonomySettings.ClientTermPath==ServiceConstants.CLIENT_TERM_PATH)
                        {                            
                            taxonomyResponseVM.ClientTermSets = GetClientTermSetHierarchy(clientContext, termSet, termStoreDetails);    
                        }                       
                        
                    }
                }

                if (termStoreDetails.TermSetName == taxonomySettings.ClientTermSetName &&
                            taxonomySettings.ClientTermPath != ServiceConstants.CLIENT_TERM_PATH)
                {
                    //This condition is if the UI is requesting client terms and the clients are defined as terms and not term sets
                    foreach (TermSet termSet in termGroup.TermSets)
                    {                        
                        Term term = null;
                        if (termSet.TermExists(clientContext, termStoreDetails.TermSetName, ref term))
                        {
                            taxonomyResponseVM.ClientTermSets = GetClientTerms(clientContext, term, termStoreDetails);
                            break;
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
                    if (term.TermsCount > 0)
                    {
                        TermCollection termCollLevel2 = term.LoadTerms(clientContext);
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
                            if (termLevel2.TermsCount > 0)
                            {
                                TermCollection termCollLevel3 = termLevel2.LoadTerms(clientContext);
                                //Add Level 3 to the term collection
                                tempTermArea.SubareaTerms = UpdateTermWithCustomProperties(termCollLevel3, termStoreDetails, termLevel2);

                                int termCount = 0;
                                foreach (Term termLevel3 in termCollLevel3)
                                {
                                    if (termLevel3.TermsCount > 0)
                                    {
                                        TermCollection termCollLevel4 = termLevel3.LoadTerms(clientContext);
                                        //Add Level 4 to the term collection
                                        tempTermArea.SubareaTerms[termCount].SubareaTerms = 
                                            UpdateTermWithCustomProperties(termCollLevel4, termStoreDetails, termLevel3);
                                        int termCount1 = 0;
                                        foreach (Term termLevel4 in termCollLevel4)
                                        {
                                            if (termLevel4.TermsCount > 0)
                                            {
                                                TermCollection termCollLevel5 = termLevel4.LoadTerms(clientContext);
                                                //Add Level 5 to the term collection
                                                tempTermArea.SubareaTerms[termCount].SubareaTerms[termCount1].SubareaTerms =
                                                    UpdateTermWithCustomProperties(termCollLevel5, termStoreDetails, termLevel4);
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
        /// <param name="parentTerm">The parent term from where the custom properties are read and assign to its child terms</param>
        /// <returns></returns>
        private List<SubareaTerm> UpdateTermWithCustomProperties(TermCollection termCollection, TermStoreDetails termStoreDetails, Term parentTerm)
        {
            try
            {
                var subAreaTerms = new List<SubareaTerm>();
                foreach (Term term in termCollection)
                {
                    SubareaTerm tempTermSubArea = new SubareaTerm();
                    tempTermSubArea.TermName = term.Name;
                    tempTermSubArea.Id = Convert.ToString(term.Id, CultureInfo.InvariantCulture);
                    tempTermSubArea.ParentTermName = parentTerm.Name;
                    IDictionary<string, string> childCustomProperties = term.CustomProperties;
                    IDictionary<string, string> parentCustomProperties = parentTerm.CustomProperties;

                    foreach (KeyValuePair<string, string> parentProperty in parentTerm.CustomProperties)
                    {
                        //if the key is present in the parent and not in the child, add that key value to the child custom properties
                        if (!childCustomProperties.Keys.Contains(parentProperty.Key))
                        {
                            if (parentProperty.Key.Equals(termStoreDetails.CustomPropertyName, StringComparison.Ordinal))
                            {
                                childCustomProperties.Add(parentProperty.Key, parentProperty.Value);
                            }
                            else if (parentProperty.Key.Equals(taxonomySettings.SubAreaOfLawDocumentTemplates, StringComparison.Ordinal))
                            {
                                childCustomProperties.Add(parentProperty.Key, parentProperty.Value);
                            }
                            else
                            {
                                childCustomProperties.Add(parentProperty.Key, parentProperty.Value);
                            }
                        }

                        //If the key is present in both and parent and, child value is empty and the parent value is not empty,
                        //update the child value with the parent value for the same key
                        if (childCustomProperties.Keys.Contains(parentProperty.Key) && childCustomProperties[parentProperty.Key] == string.Empty)
                        {
                            childCustomProperties[parentProperty.Key] = parentProperty.Value;
                        }
                    }

                    //Add the custom properties to the subAreaTerms list collection
                    foreach (KeyValuePair<string, string> customProperty in childCustomProperties)
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
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This method will be called if the clients are defined as terms and not as term sets
        /// </summary>
        /// <param name="clientContext">The sharepoint client context</param>
        /// <param name="term"></param>
        /// <param name="termStoreDetails"></param>
        /// <returns></returns>
        private ClientTermSets GetClientTerms(ClientContext clientContext, Term term, TermStoreDetails termStoreDetails)
        {
            ClientTermSets tempClientTermSet = new ClientTermSets();
            try
            {
                tempClientTermSet.Name = taxonomySettings.ClientTermSetName;
                /////Retrieve the Terms - level 1
                tempClientTermSet.ClientTerms = new List<Client>();
                TermCollection termColl = null;
                termColl = term.LoadTerms(clientContext);

                return GetClientTermProperties(termColl, termStoreDetails, tempClientTermSet);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

        }
    
        /// <summary>
        /// This method will be called if the clients are defined as term set and not as terms
        /// </summary>
        /// <param name="termSet">Term set object holding Client terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Client Term Set</returns>
        private ClientTermSets GetClientTermSetHierarchy(ClientContext clientContext, TermSet termSet, TermStoreDetails termStoreDetails)
        {
            ClientTermSets tempClientTermSet = new ClientTermSets();
            try
            {
                tempClientTermSet.Name = taxonomySettings.ClientTermSetName;
                /////Retrieve the Terms - level 1
                tempClientTermSet.ClientTerms = new List<Client>();
                TermCollection termColl = null;  
                termColl = termSet.Terms;
                return GetClientTermProperties(termColl, termStoreDetails, tempClientTermSet);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, 
                    MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }            
            
        }

        /// <summary>
        /// This method will get all client term properties such as ID and URL
        /// </summary>
        /// <param name="termColl">The taxonomy client terms</param>
        /// <param name="termStoreDetails">The term store details which the UI has sent to the clients</param>
        /// <param name="clientTermSet">The ClientTermSets object to which all the client terms are added</param>
        /// <returns></returns>
        private ClientTermSets GetClientTermProperties(TermCollection termColl, TermStoreDetails termStoreDetails, ClientTermSets clientTermSet)
        {
            try
            {
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
                    clientTermSet.ClientTerms.Add(tempTermPG);
                }
                return clientTermSet;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }  
}
