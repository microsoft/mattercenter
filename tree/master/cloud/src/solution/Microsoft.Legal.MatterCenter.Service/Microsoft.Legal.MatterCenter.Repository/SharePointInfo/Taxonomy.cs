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
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Configuration;
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
        private ContentTypesConfig contentTypeSettings;
        private LogTables logTables;
        private readonly TaxonomySettings taxonomySettings;
        private ISPOAuthorization spoAuthorization;
        private ClientContext clientContext;
        private TaxonomyResponseVM taxonomyResponseVM;
        private ICustomLogger customLogger;
        private IConfigurationRoot configuration;
        //Initialize the minimum number of taxonomy levels. By default minimum levels will be 2
        private int level = 3;
        #endregion

        /// <summary>
        /// Constructor to inject required dependencies
        /// </summary>
        /// <param name="generalSettings"></param>
        /// <param name="taxonomySettings"></param>
        /// <param name="logTables"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="customLogger"></param>
        public Taxonomy(IOptions<GeneralSettings> generalSettings, 
            IOptions<TaxonomySettings> taxonomySettings,
            IOptions<ContentTypesConfig> contentTypeSettings,
            IOptions<LogTables> logTables,
            ISPOAuthorization spoAuthorization, ICustomLogger customLogger, 
            IConfigurationRoot configuration)
        {
            this.generalSettings = generalSettings.Value;
            this.taxonomySettings = taxonomySettings.Value;
            this.contentTypeSettings = contentTypeSettings.Value;
            this.logTables = logTables.Value;
            this.spoAuthorization = spoAuthorization;
            taxonomyResponseVM = new TaxonomyResponseVM();
            this.customLogger = customLogger;
            
            this.configuration = configuration;
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
                            taxonomyResponseVM.TermSets = GetManagedTermSetHierarchy(clientContext, termSet, termStoreDetails);
                        }
                        //This condition is if the UI is requesting client terms and the clients are defined as term set and not terms
                        else if (termStoreDetails.TermSetName == taxonomySettings.ClientTermSetName )
                        {                            
                            taxonomyResponseVM.ClientTermSets = GetClientTermSetHierarchy(clientContext, termSet, termStoreDetails);    
                        }
                    }
                }

                //if (termStoreDetails.TermSetName == taxonomySettings.ClientTermSetName &&
                //            taxonomySettings.ClientTermPath != ServiceConstants.CLIENT_TERM_PATH)
                //{
                //    //This condition is if the UI is requesting client terms and the clients are defined as terms and not term sets
                //    foreach (TermSet termSet in termGroup.TermSets)
                //    {                        
                //        if (termSet.Name == taxonomySettings.ClientTermPath)
                //        {
                //            taxonomyResponseVM.ClientTermSets = GetClientTermSetHierarchy(clientContext, termSet, termStoreDetails);
                //        }
                //    }
                //}
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return taxonomyResponseVM;
        }




        /// <summary>
        /// Gets the managed term set hierarchy with generic code using JSONWriter
        /// </summary>
        /// <param name="termSet">Term set object holding Practice Group terms</param>
        /// <param name="termStoreDetails">Term Store object containing Term store data</param>
        /// <returns>Serialized Object of Term Set</returns>
        private string GetManagedTermSetHierarchy(ClientContext clientContext, TermSet termSet, TermStoreDetails termStoreDetails)
        {
            level = 3;
            StringBuilder sb = new StringBuilder();
            JsonWriter jw = new JsonTextWriter(new StringWriter(sb));
            jw.Formatting = Formatting.Indented;
            jw.WriteStartObject();
            jw.WritePropertyName("name");
            jw.WriteValue(termSet.Name);
            jw.WritePropertyName("levels");
            jw.WriteValue(taxonomySettings.Levels);
            jw.WritePropertyName(taxonomySettings.Level1Name);
            jw.WriteStartArray();
            TermCollection termColl = termSet.Terms;
            foreach (Term term in termColl)
            {                
                jw.WriteStartObject();
                //Create level1 terms - Practice Groups
                jw.WritePropertyName("termName");
                jw.WriteValue(term.Name);
                jw.WritePropertyName("id");
                jw.WriteValue(Convert.ToString(term.Id, CultureInfo.InvariantCulture));
                jw.WritePropertyName("parentTermName");
                jw.WriteValue(termSet.Name);
                jw.WritePropertyName("siteColumnName");
                jw.WriteValue(configuration.GetSection("ContentTypes").GetSection("ManagedColumns")["ColumnName1"]);
                foreach (KeyValuePair<string, string> customProperty in term.CustomProperties)
                {
                    if (customProperty.Key.Equals(taxonomySettings.PracticeGroupCustomPropertyFolderNames, StringComparison.Ordinal))
                    {
                        jw.WritePropertyName("folderNames");
                        jw.WriteValue(customProperty.Value);                        
                    }
                }
                //Create level 2 terms - Area Of Law
                jw.WritePropertyName(taxonomySettings.Level2Name);                
                if (term.TermsCount > 0)
                {
                    jw.WriteStartArray();
                    TermCollection termCollLevel2 = term.LoadTerms(clientContext);
                    foreach (Term termLevel2 in termCollLevel2)
                    {
                        if (termLevel2.Name != taxonomySettings.ClientTermSetName)
                        {
                            jw.WriteStartObject();
                            jw.WritePropertyName("termName");
                            jw.WriteValue(termLevel2.Name);
                            jw.WritePropertyName("id");
                            jw.WriteValue(Convert.ToString(termLevel2.Id, CultureInfo.InvariantCulture));
                            jw.WritePropertyName("parentTermName");
                            jw.WriteValue(term.Name);
                            jw.WritePropertyName("siteColumnName");
                            jw.WriteValue(configuration.GetSection("ContentTypes").GetSection("ManagedColumns")["ColumnName2"]);
                            foreach (KeyValuePair<string, string> customProperty in term.CustomProperties)
                            {
                                if (customProperty.Key.Equals(taxonomySettings.PracticeGroupCustomPropertyFolderNames, StringComparison.Ordinal))
                                {
                                    jw.WritePropertyName("folderNames");
                                    jw.WriteValue(customProperty.Value);
                                }
                            }
                            //If the number of levels that are configured are more than 2, try to get the 
                            //3rd level hierarchy
                            if (taxonomySettings.Levels > 2)
                            {                                
                                //Create level 3 terms - Sub Area Of LAW
                                if (termLevel2.TermsCount > 0)
                                {
                                    jw.WritePropertyName(taxonomySettings.Level3Name);
                                    jw.WriteStartArray();
                                    TermCollection termCollLevel3 = termLevel2.LoadTerms(clientContext);
                                    string siteColumnName = configuration.GetSection("ContentTypes").GetSection("ManagedColumns")["ColumnName3"];
                                    int termLevelHierarchyPosition = 0;
                                    GetChildTermsWithCustomProperties(termCollLevel3, termStoreDetails, termLevel2, jw, 
                                        siteColumnName, termLevelHierarchyPosition);
                                    jw.WriteEndArray();
                                }
                                jw.WriteEndObject();
                            }
                        }
                    }
                    jw.WriteEndArray();
                } 
                jw.WriteEndObject();
            }
            jw.WriteEndArray();
            jw.WriteEndObject();
            return sb.ToString();            
        }

        /// <summary>
        /// This method will update the taxonomy hierarchy object with custom properties that needs to be send to client. This is a recursive function
        /// and it will loop until a term does not have any child terms
        /// </summary>
        /// <param name="termCollection">The Term Collection object to which terms will be added</param>
        /// <param name="termStoreDetails">The term store details which the client has sent</param>
        /// <param name="parentTerm">The parent term from where the custom properties are read and assign to its child terms</param>
        /// <returns></returns>
        private void GetChildTermsWithCustomProperties(TermCollection termCollection, 
            TermStoreDetails termStoreDetails, Term parentTerm, JsonWriter jw, string siteColumnName, int termLevelHierarchyPosition)
        {
            try
            {
                //var subAreaTerms = new List<SubareaTerm>();
                foreach (Term term in termCollection)
                {
                    if (term.Name != taxonomySettings.ClientTermSetName)
                    {
                        jw.WriteStartObject();
                        jw.WritePropertyName("termName");
                        jw.WriteValue(term.Name);
                        jw.WritePropertyName("id");
                        jw.WriteValue(Convert.ToString(term.Id, CultureInfo.InvariantCulture));
                        jw.WritePropertyName("parentTermName");
                        jw.WriteValue(parentTerm.Name);
                        jw.WritePropertyName("siteColumnName");
                        jw.WriteValue(siteColumnName);
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
                                else if (parentProperty.Key.Equals(taxonomySettings.MatterProvisionExtraPropertiesContentType, StringComparison.Ordinal))
                                {
                                    childCustomProperties.Add(parentProperty.Key, parentProperty.Value);
                                }
                                else
                                {
                                    childCustomProperties.Add(parentProperty.Key, parentProperty.Value);
                                }
                            }

                            //If the key is present in both and parent and child and if child value is empty and the parent value is not empty,
                            //update the child value with the parent value for the same key
                            if (childCustomProperties.Keys.Contains(parentProperty.Key) && childCustomProperties[parentProperty.Key] == string.Empty)
                            {
                                childCustomProperties[parentProperty.Key] = parentProperty.Value;
                            }
                        }

                        //Add the custom properties to the subAreaTerms list collection
                        foreach (KeyValuePair<string, string> customProperty in childCustomProperties)
                        {
                            if (customProperty.Key.Equals(taxonomySettings.MatterProvisionExtraPropertiesContentType, StringComparison.Ordinal))
                            {
                                jw.WritePropertyName("MatterProvisionExtraPropertiesContentType");
                                jw.WriteValue(customProperty.Value);
                            }

                            if (customProperty.Key.Equals(termStoreDetails.CustomPropertyName, StringComparison.Ordinal))
                            {
                                jw.WritePropertyName("documentTemplates");
                                jw.WriteValue(customProperty.Value);
                            }
                            else if (customProperty.Key.Equals(taxonomySettings.SubAreaCustomPropertyFolderNames, StringComparison.Ordinal))
                            {

                                jw.WritePropertyName("folderNames");
                                jw.WriteValue(customProperty.Value);
                            }
                            else if (customProperty.Key.Equals(taxonomySettings.SubAreaCustomPropertyisNoFolderStructurePresent, StringComparison.Ordinal))
                            {
                                jw.WritePropertyName("isNoFolderStructurePresent");
                                if (generalSettings.IsBackwardCompatible)
                                {
                                    if(customProperty.Value.ToLower()==ServiceConstants.IS_FOLDER_STRUCTURE_PRESENT_FALSE.ToLower())
                                    {
                                        jw.WriteValue(ServiceConstants.IS_FOLDER_STRUCTURE_PRESENT_TRUE);
                                    }
                                    else if (customProperty.Value.ToLower() == ServiceConstants.IS_FOLDER_STRUCTURE_PRESENT_TRUE.ToLower())
                                    {
                                        jw.WriteValue(ServiceConstants.IS_FOLDER_STRUCTURE_PRESENT_FALSE);
                                    }                                    
                                }
                                else
                                {
                                    jw.WriteValue(customProperty.Value);
                                }
                                
                                
                            }
                            else if (customProperty.Key.Equals(taxonomySettings.SubAreaOfLawDocumentTemplates, StringComparison.Ordinal))
                            {
                                jw.WritePropertyName("documentTemplateNames");
                                jw.WriteValue(customProperty.Value);
                            }
                        }
                        //Increment the level
                        level = level + 1;
                        //Check if we need to get more terms.
                        if (level <= taxonomySettings.Levels)
                        {
                            if (term.TermsCount > 0)
                            {
                                if(level==4)
                                {
                                    jw.WritePropertyName(taxonomySettings.Level4Name);
                                }
                                //The app will support upto five level of hierarchy
                                if (level == 5)
                                {
                                    jw.WritePropertyName(taxonomySettings.Level5Name);
                                }
                                jw.WriteStartArray();
                                TermCollection termCollLevel4 = term.LoadTerms(clientContext);
                                siteColumnName = configuration.GetSection("ContentTypes").GetSection("ManagedColumns")["ColumnName"+ level];
                                //Recursive function which will call it self until a given term does not have any child terms
                                GetChildTermsWithCustomProperties(termCollLevel4, termStoreDetails, term, jw, siteColumnName, termLevelHierarchyPosition);
                                jw.WriteEndArray();
                            }
                        }
                        jw.WriteEndObject();
                    }
                }             
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
                tempClientTermSet.Name = "Clients";
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
                    if(!string.IsNullOrWhiteSpace(tempTermPG.Id) && !string.IsNullOrWhiteSpace(tempTermPG.Url))
                    {
                        clientTermSet.ClientTerms.Add(tempTermPG);
                    }                    
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
