// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
// Author           : v-prd
// Created          : 04-06-2014
//
// ***********************************************************************
// <copyright file="Utility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains helper functions for updating taxonomy fields.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
{
    #region using
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    #endregion

    /// <summary>
    /// This class contains helper functions for updating taxonomy fields.
    /// </summary>
    internal class Utility
    {
        /// <summary>
        /// declaring the string practiceGroupFieldName
        /// </summary>
        private static string practiceGroupFieldName = ConfigurationManager.AppSettings["practiceGroupFieldName"];

        /// <summary>
        /// declaring the string areaOfLawFieldName
        /// </summary>
        private static string areaOfLawFieldName = ConfigurationManager.AppSettings["areaOfLawFieldName"];

        /// <summary>
        /// declaring the string subAreaOfLawFiedName
        /// </summary>
        private static string subAreaOfLawFiedName = ConfigurationManager.AppSettings["subareaOfLawFieldName"];

        /// <summary>
        /// Loading the Term Group Based on Given Name
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="taxonomySession">Taxonomy session</param>
        /// <param name="termGroupName">Term group name </param>
        /// <returns>Term group </returns>
        public static TermGroup LoadTermGroup(ClientContext clientContext, TaxonomySession taxonomySession, string termGroupName)
        {
            TermStore termStore = taxonomySession.GetDefaultKeywordsTermStore();
            clientContext.Load(termStore,
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name,
                        group => group.TermSets.Include(
                            termSet => termSet.Name,
                            termSet => termSet.Terms.Include(
                                term => term.Name,
                                term => term.CustomProperties
                                )
                        )
                    )
            );
            clientContext.ExecuteQuery();
            TermGroup termGroup = termStore.Groups.GetByName(termGroupName);
            return termGroup;
        }

        /// <summary>
        /// Initializing Client List By Loading TermGroup
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="termGroup">Term Group</param>
        /// <param name="termSetName">Term Store</param>
        /// <returns>Client List</returns>
        public static List<Client> IntializeClientObject(ClientContext clientContext, TermGroup termGroup, string termSetName)
        {
            Client client;
            List<Client> clients = new List<Client>();
            TermSet termSet = termGroup.TermSets.GetByName(termSetName);
            TermCollection termColl = termSet.Terms;
            clientContext.Load(termColl);
            // Execute the query to the server
            clientContext.ExecuteQuery();
            foreach (Term term in termColl)
            {
                client = new Client();
                client.Name = term.Name;
                foreach (KeyValuePair<string, string> termProperty in term.CustomProperties)
                {
                    if (termProperty.Key.Equals(Constants.URLDictKey))
                    {
                        client.Url = termProperty.Value;
                    }
                }
                clients.Add(client);
            }
            return clients;
        }

        /// <summary>
        /// Initializing Practice Group List By Loading TermGroup
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="termGroup">Term Group</param>
        /// <param name="termSetName">Term Set Name</param>
        /// <returns>Practice Group List</returns>
        public static List<PracticeGroup> IntializePracticeGroupObject(ClientContext clientContext, TermGroup termGroup, string termSetName)
        {
            PracticeGroup practiceGroup;
            SubAreaOfLaw subAreaOfLawTerm;
            AreaOfLaw areaOfLawTerm;
            TermSet termSet = termGroup.TermSets.GetByName(termSetName);
            TermCollection termColl = termSet.Terms;
            List<PracticeGroup> practiceGroups = new List<PracticeGroup>();
            LoadExecute(clientContext, termColl);
            foreach (Term prctcGrp in termColl)
            {
                // Initializing Practice Group
                practiceGroup = new PracticeGroup();
                practiceGroup.Name = prctcGrp.Name;
                practiceGroup.Id = Convert.ToString(prctcGrp.Id, CultureInfo.InvariantCulture);
                practiceGroup.AreaOfLaw = new List<AreaOfLaw>();
                practiceGroups.Add(practiceGroup);
                foreach (Term areaLaw in prctcGrp.Terms)
                {
                    // Initializing Area of Law
                    areaOfLawTerm = new AreaOfLaw();
                    areaOfLawTerm.Name = areaLaw.Name;
                    areaOfLawTerm.Id = Convert.ToString(areaLaw.Id, CultureInfo.InvariantCulture);
                    areaOfLawTerm.SubareaOfLaw = new List<SubAreaOfLaw>();
                    practiceGroup.AreaOfLaw.Add(areaOfLawTerm);

                    foreach (Term subAreaOfLaw in areaLaw.Terms)
                    {
                        subAreaOfLawTerm = new SubAreaOfLaw();
                        subAreaOfLawTerm.Name = subAreaOfLaw.Name;
                        subAreaOfLawTerm.Id = Convert.ToString(subAreaOfLaw.Id, CultureInfo.InvariantCulture);
                        foreach (KeyValuePair<string, string> termProperty in subAreaOfLaw.CustomProperties)
                        {
                            if (termProperty.Key.Equals(Constants.ContentTypeDictKey))
                            {
                                subAreaOfLawTerm.ContentType = termProperty.Value;
                            }
                        }
                        areaOfLawTerm.SubareaOfLaw.Add(subAreaOfLawTerm);
                    }
                }
            }
            return practiceGroups;
        }

        /// <summary>
        /// To load and execute the Term collection
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="termColl">Term Collection</param>
        private static void LoadExecute(ClientContext clientContext, TermCollection termColl)
        {
            clientContext.Load(termColl, practicegroup => practicegroup.Include(
                item => item.Name,
                item => item.Id,
                item => item.Terms.Include(
                    areaOfLaw => areaOfLaw.Name,
                    areaOfLaw => areaOfLaw.Id,
                    areaOfLaw => areaOfLaw.Terms.Include(
                        subAreaOfLaw => subAreaOfLaw.Name,
                        subAreaOfLaw => subAreaOfLaw.Id,
                        subAreaOfLaw => subAreaOfLaw.CustomProperties
                        )
                    )
                ));
            // Execute the query to the server
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Updating Previous Documents Recursively using CAML Query
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="list">List of Matters</param>
        /// <param name="practiceGroupMetadataDefaultValue">Default Value to be Set for practice group</param>
        /// <param name="areaOfLawMetadataDefaultValue">Default Value to be Set for Area of Law</param>
        /// <param name="subAreaOfLawMetadataDefaultValue">default Value to be Set for Sub Area of Law</param>
        /// <param name="clientName">Current Client name</param>
        public static void UpdatePreviousDocuments(ClientContext clientContext, List list, string practiceGroupMetadataDefaultValue, string areaOfLawMetadataDefaultValue, string subAreaOfLawMetadataDefaultValue, string clientName)
        {
            try
            {
                Web site = clientContext.Web;
                User currentUser = site.CurrentUser;
                clientContext.Load(currentUser);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.ProcessingMessageDocuments, list.Title));
                CamlQuery CAMLQuery = new CamlQuery();
                CAMLQuery.ViewXml = string.Format(CultureInfo.InvariantCulture, Constants.CAMLQueryRetrieveAllDocuments, practiceGroupFieldName, areaOfLawFieldName, subAreaOfLawFiedName);;
                ListItemCollection listItems = list.GetItems(CAMLQuery);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();
                foreach (ListItem item in listItems)
                {
                    bool checkoutByCurrent = false;
                    if (null != item[Constants.CheckOutFieldKey])
                    {
                        string checkOutUser = ((Microsoft.SharePoint.Client.FieldLookupValue)(item[Constants.CheckOutFieldKey])).LookupValue;

                        if (currentUser.Title == checkOutUser)
                        {
                            checkoutByCurrent = true;
                        }
                        else
                        {
                            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.FailureMessageUpdationDocument, item[Constants.DocNameFieldKey], checkOutUser));
                        }
                    }
                    if (null == item[Constants.CheckOutFieldKey] || checkoutByCurrent)// Checkout Functionality
                    {
                        TaxonomyFieldValueCollection practicegroupTaxanomyColl = item[practiceGroupFieldName] as TaxonomyFieldValueCollection;
                        if (0 >= practicegroupTaxanomyColl.Count)
                        {
                            item[practiceGroupFieldName] = practiceGroupMetadataDefaultValue;
                            item.Update();
                        }
                        TaxonomyFieldValueCollection areaOfLawTaxanomyColl = item[areaOfLawFieldName] as TaxonomyFieldValueCollection;
                        if (0 >= areaOfLawTaxanomyColl.Count)
                        {
                            item[areaOfLawFieldName] = areaOfLawMetadataDefaultValue;
                            item.Update();
                        }
                        TaxonomyFieldValueCollection subAreaOfLawTaxanomyColl = item[subAreaOfLawFiedName] as TaxonomyFieldValueCollection;
                        if (0 >= subAreaOfLawTaxanomyColl.Count)
                        {
                            item[subAreaOfLawFiedName] = subAreaOfLawMetadataDefaultValue;
                            item.Update();
                        }
                        clientContext.ExecuteQuery();
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.SucessMessageUpdationDocument, item[Constants.DocNameFieldKey]));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.FailureMessageDocument, clientName, list.Title, e.Message));
            }
        }

        /// <summary>
        /// Retrieving WssId for given field title
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="list">List For Matters</param>
        /// <param name="title"> Title for whom WssId need to be retrieved</param>
        /// <param name="id">Id for whom WssId need to be retrieved</param>
        /// <returns>WssId for corresponding ID</returns>
        public static int GetWSSID(ClientContext clientContext, List list, string title, string id)
        {
            int wssID = -1;
            try
            {
                FieldCollection fieldCollection = list.Fields;
                clientContext.Load(fieldCollection, c => c.Where(item => item.InternalName == title));
                clientContext.ExecuteQuery();
                TaxonomyField field = fieldCollection.FirstOrDefault() as TaxonomyField;
                ClientResult<TaxonomyFieldValue> clientResult = field.GetFieldValueAsTaxonomyFieldValue(id);
                clientContext.ExecuteQuery();
                wssID = clientResult.Value.WssId;
                return wssID;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.WssIdRetrievalFailureMessage, e.Message));
                return wssID;
            }
        }

        /// <summary>
        /// Updating Matter Based on given Title
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="list">List For All Matters</param>
        /// <param name="titlePracticeGroup"> Title For Practice Group</param>
        /// <param name="practiceGroupDefaultValue">Default Value to be Set for practice group</param>
        /// <param name="titleAreaOfLaw">Title For Area of Law</param>
        /// <param name="areaOfLawDefaultValue">Default Value to be Set for Area of Law</param>
        /// <param name="titleSubAreaOfLaw">Title For Sub Area of Law</param>
        /// <param name="subAreaOfLawDefaultValue">default Value to be Set for Sub Area of Law</param>
        /// <param name="clientName">Current Client Name</param>
        public static void UpdateMatter(ClientContext clientContext, List list, string titlePracticeGroup, string practiceGroupDefaultValue, string titleAreaOfLaw, string areaOfLawDefaultValue, string titleSubAreaOfLaw, string subAreaOfLawDefaultValue, string clientName)
        {
            try
            {
                FieldCollection collectionPracticeGroup = list.Fields;
                clientContext.Load(collectionPracticeGroup, practiceGroup => practiceGroup.Where(practiceGroupItem => practiceGroupItem.InternalName == titlePracticeGroup));
                clientContext.ExecuteQuery();
                TaxonomyField practiceGroupField = collectionPracticeGroup.FirstOrDefault() as TaxonomyField;
                FieldCollection collectionAreaOfLaw = list.Fields;
                clientContext.Load(collectionAreaOfLaw, areaOfLaw => areaOfLaw.Where(areaOfLawItem => areaOfLawItem.InternalName == titleAreaOfLaw));
                clientContext.ExecuteQuery();
                TaxonomyField areaOfLawField = collectionAreaOfLaw.FirstOrDefault() as TaxonomyField;
                FieldCollection collectionSubAreaOfLaw = list.Fields;
                clientContext.Load(collectionSubAreaOfLaw, subareaOfLaw => subareaOfLaw.Where(subareaOfLawItem => subareaOfLawItem.InternalName == titleSubAreaOfLaw));
                clientContext.ExecuteQuery();
                TaxonomyField subAreaOfLawField = collectionSubAreaOfLaw.FirstOrDefault() as TaxonomyField;

                if (string.IsNullOrWhiteSpace(practiceGroupField.DefaultValue))
                {
                    practiceGroupField.DefaultValue = practiceGroupDefaultValue;
                    practiceGroupField.Update();
                }
                if (string.IsNullOrWhiteSpace(areaOfLawField.DefaultValue))
                {
                    areaOfLawField.DefaultValue = areaOfLawDefaultValue;
                    areaOfLawField.Update();
                }
                if (string.IsNullOrWhiteSpace(subAreaOfLawField.DefaultValue))
                {
                    subAreaOfLawField.DefaultValue = subAreaOfLawDefaultValue;
                    subAreaOfLawField.Update();
                }

                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.FailureMessage, clientName, list.Title, e.Message));
            }
        }

        /// <summary>
        /// Update Content based on comparison with content type
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="list"> List For All Matters</param>
        /// <param name="practiceGroups"> Object List For Class Practice Group</param>
        /// <param name="compareToContentTypeValue"> Value To be Compared based on Case by flag</param>
        /// <param name="clientName">Name of the Client for which Matter are processing</param>
        /// <param name="flag">Scenario based on which we need to do update</param>
        /// <returns>Success or Failure</returns>
        public static bool UpdateMatterBasedOnContentType(ClientContext clientContext, List list, List<PracticeGroup> practiceGroups, string compareToContentTypeValue, string clientName, int flag)
        {
            try
            {
                bool isDoUpdate = false;
                foreach (PracticeGroup practiceGroup in practiceGroups)
                {
                    foreach (AreaOfLaw areaOfLaw in practiceGroup.AreaOfLaw)
                    {
                        foreach (SubAreaOfLaw subAreaOfLaw in areaOfLaw.SubareaOfLaw)
                        {
                            if (1 == flag)
                            {
                                if (compareToContentTypeValue.Equals(subAreaOfLaw.ContentType))
                                {
                                    isDoUpdate = true;
                                }
                            }
                            if (2 == flag)
                            {
                                if (compareToContentTypeValue.Equals(subAreaOfLaw.Name))
                                {
                                    string[] areaOfLawString = Convert.ToString(list.RootFolder.Properties.FieldValues[areaOfLawFieldName], CultureInfo.InvariantCulture).Split(';');
                                    string[] practiceGroupString = Convert.ToString(list.RootFolder.Properties.FieldValues[practiceGroupFieldName], CultureInfo.InvariantCulture).Split(';');
                                    string areaOfLawStringTrimed = WebUtility.HtmlDecode(areaOfLawString[0]);
                                    string practiceGroupStringTrimed = WebUtility.HtmlDecode(practiceGroupString[0]);
                                    if (areaOfLawStringTrimed.Equals(areaOfLaw.Name) && practiceGroupStringTrimed.Equals(practiceGroup.Name))
                                    {
                                        isDoUpdate = true;
                                    }
                                }
                            }
                            if (isDoUpdate)
                            {
                                if (0 == practiceGroup.WssId)
                                {
                                    practiceGroup.WssId = GetWSSID(clientContext, list, practiceGroupFieldName, practiceGroup.Id);
                                }
                                if (0 == areaOfLaw.WssId)
                                {
                                    areaOfLaw.WssId = GetWSSID(clientContext, list, areaOfLawFieldName, areaOfLaw.Id);
                                }
                                if (0 == subAreaOfLaw.WssId)
                                {
                                    subAreaOfLaw.WssId = GetWSSID(clientContext, list, subAreaOfLawFiedName, subAreaOfLaw.Id);
                                }
                                string practiceGroupMetadataDefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, practiceGroup.WssId, practiceGroup.Name, practiceGroup.Id);
                                string areaOfLawMetadataDefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, areaOfLaw.WssId, areaOfLaw.Name, areaOfLaw.Id);
                                string subAreaOfLawMetadataDefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, subAreaOfLaw.WssId, subAreaOfLaw.Name, subAreaOfLaw.Id);
                                UpdateMatter(clientContext, list, practiceGroupFieldName, practiceGroupMetadataDefaultValue, areaOfLawFieldName, areaOfLawMetadataDefaultValue, subAreaOfLawFiedName, subAreaOfLawMetadataDefaultValue, clientName);
                                UpdatePreviousDocuments(clientContext, list, practiceGroupMetadataDefaultValue, areaOfLawMetadataDefaultValue, subAreaOfLawMetadataDefaultValue, clientName);
                                return true;
                            }
                        }
                    }
                }
                if (!isDoUpdate)
                {
                    Console.Write(string.Format(CultureInfo.InvariantCulture, Constants.StampPropertyNotUpdated, list.Title));
                }
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.FailureMessage, clientName, list.Title, exception.Message));
                return false;
            }
        }
    }
}
