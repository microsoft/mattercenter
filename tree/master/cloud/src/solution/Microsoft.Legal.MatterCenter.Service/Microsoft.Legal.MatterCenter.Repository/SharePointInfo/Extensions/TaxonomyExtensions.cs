using Microsoft.SharePoint.Client.Taxonomy;
using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Legal.MatterCenter.Repository.Extensions
{
    public static class TaxonomyExtensions
    {
        /// <summary>
        /// This method will load the child terms for a particular parent term with required term properties
        /// </summary>
        /// <param name="term">Parent Term</param>
        /// <param name="context">SharePoint Context</param>
        /// <returns></returns>
        public static TermCollection LoadTerms(this Term term, ClientContext context)
        {
            TermCollection termCollection = term.Terms;
            context.Load(termCollection,
                tc => tc.Include(
                    t => t.TermsCount,
                    t => t.Id,
                    t => t.Name,
                    t => t.TermsCount,
                    t => t.CustomProperties
                )
            );
            context.ExecuteQuery();
            return termCollection;
        }

        /// <summary>
        /// This method will check if the term exists and if it exists, it will return the reference to that term
        /// </summary>
        /// <param name="termSet">The term set underwhich, the term exists check will happen</param>
        /// <param name="clientContext">The sharepoint client context object</param>
        /// <param name="path">The term path under which we need to check for a term existence</param>
        /// <param name="termNameToRetrieve">The term that needs to be retrieved from a given term</param>
        /// <param name="term">The term reference that will be returned back to the caller</param>
        /// <returns></returns>
        public static bool TermExists(this TermSet termSet, ClientContext clientContext, string termNameToRetrieve, ref Term term)
        {
            TermCollection termCollection = termSet.GetAllTerms();            
            clientContext.Load(termCollection,
                tc => tc.Include(
                    t => t.Name,
                    t => t.PathOfTerm,
                    t => t.TermsCount,
                    t=>t.Parent.Name
                    
                    ));
            clientContext.ExecuteQuery();
            foreach (Term currentTerm in termCollection)
            {
                string pathOfTerm = currentTerm.PathOfTerm;
                //ClientResult<string> pathOfTerm = currentTerm.GetPath(1033);
                if (currentTerm.Name.ToLower() == termNameToRetrieve.ToLower())
                {
                    term = currentTerm;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This extension method will give all the terms for a give path mentioned
        /// </summary>
        /// <param name="termSet">From which term set, we need to find a given term path</param>
        /// <param name="path">The path name to search</param>
        /// <param name="clientContext">The sharepoint client context object</param>
        /// <returns></returns>
        public static TermCollection GetTermsByPath(this TermSet termSet, String path, ClientContext clientContext)
        {            
            TermCollection termCollection = termSet.GetAllTerms();
            TermCollection termCollectionToReturn = null;
            clientContext.Load(termCollection, 
                tc=>tc.Include(
                    t=>t.Name,
                    t=>t.PathOfTerm,
                    t=>t.TermsCount
                    ));
            clientContext.ExecuteQuery();    
            foreach(Term term in termCollection)
            {
                if(term.PathOfTerm == path)
                {
                    //Get all the terms under that path
                    if (term.TermsCount > 0)
                    {
                        termCollectionToReturn = term.LoadTerms(clientContext);
                    }
                }
            }        
            return termCollectionToReturn;
        }
    }
}
