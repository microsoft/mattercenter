// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="TaxonomyRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
#endregion

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This class will talk to Taxonomy class which will handle all the request and response to SPO from term store perspective
    /// </summary>
    public class TaxonomyRepository : ITaxonomyRepository
    {   
        private ITaxonomy taxonomy;
        private ISite site;
        /// <summary>
        /// All the required dependencies are injected into constructor
        /// </summary>        
        /// <param name="spoAuthorization"></param>
        /// <param name="taxonomy"></param>
        /// <param name="site"></param>
        public TaxonomyRepository(ISPOAuthorization spoAuthorization, ITaxonomy taxonomy, ISite site)
        {
            this.taxonomy = taxonomy;
            this.site = site;
        }

        /// <summary>
        /// This method will get the taxonomy hierarchy object for the given search criterai and return to the service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="termStoreViewModel">The request object for which the taxonomy has to be retrieved</param>
        /// <returns>Client TermSet or SPO TermSet</returns>
        public async Task<TaxonomyResponseVM> GetTaxonomyHierarchyAsync(TermStoreViewModel termStoreViewModel)
        {
            return await Task.FromResult(taxonomy.GetTaxonomyHierarchy(termStoreViewModel.Client, termStoreViewModel.TermStoreDetails));
        }


        /// <summary>
        /// This method will get the current SPO site title for the url that is there in the client object. 
        /// This is a test method and will be removed later
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public string GetCurrentSiteName(Client client)
        {
            return site.GetCurrentSite(client);
        }       
    }
}
