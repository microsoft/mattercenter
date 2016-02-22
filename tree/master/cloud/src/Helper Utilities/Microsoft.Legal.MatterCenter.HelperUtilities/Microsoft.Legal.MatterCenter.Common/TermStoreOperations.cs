// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Common
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="TermStoreOperations.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides Term Store related operations.</summary>
// ***********************************************************************
using System;

[assembly: CLSCompliant(false)]

namespace Microsoft.Legal.MatterCenter.Common
{
    #region using
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System.Collections.Generic;
    using System.Linq;
    #endregion

    /// <summary>
    /// A Class provides Term Store related operations
    /// </summary>
    public static class TermStoreOperations
    {
        /// <summary>
        /// Function to return client id and client URL from term store 
        /// </summary>
        /// <param name="clientContext">Tenant client context</param>
        /// <param name="groupName">TermStore Practice Group Name</param>
        /// <param name="termSetName">TermSet name</param>
        /// <param name="clientIdProperty">Name of Client ID property</param>
        /// <param name="clientPropertyName">Name of Client URL property</param>
        /// <returns>ClientId and ClientUrl</returns>
        public static ClientTermSets GetClientDetails(ClientContext clientContext, string groupName, string termSetName, string clientIdProperty, string clientPropertyName)
        {
            ClientTermSets clientDetails = new ClientTermSets();
            clientDetails.ClientTerms = new List<Client>();
            try
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

                    // 5. Get required term from term from extracted term set
                    TermCollection fillteredTerms = termGroup.TermSets.Where(item => item.Name.Equals(termSetName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Terms;
                    clientDetails = GetClientTermSets(fillteredTerms, clientIdProperty, clientPropertyName);
                }
                else
                {
                    clientDetails = null;
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Concat("Exception occurred during getting client details from TermStore.\n", exception.Message, exception.StackTrace));
            }
            return clientDetails;
        }

        /// <summary>
        /// Method to get client term sets
        /// </summary>
        /// <param name="fillteredTerms">term collection</param>
        /// <param name="clientIdProperty">client id property</param>
        /// <param name="clientPropertyName">client property name</param>
        /// <returns>returns client term sets</returns>
        private static ClientTermSets GetClientTermSets(TermCollection fillteredTerms, string clientIdProperty, string clientPropertyName)
        {
            ClientTermSets clientDetails = new ClientTermSets();
            clientDetails.ClientTerms = new List<Client>();

            foreach (Term term in fillteredTerms)
            {
                if (term.CustomProperties.ContainsKey(clientIdProperty) && term.CustomProperties.ContainsKey(clientPropertyName))
                {
                    Client client = new Client();
                    client.ClientName = term.Name;
                    client.ClientId = term.CustomProperties[clientIdProperty];
                    client.ClientUrl = term.CustomProperties[clientPropertyName];
                    clientDetails.ClientTerms.Add(client);
                }
            }
            return clientDetails;
        }
    }
}
