using Microsoft.Legal.MatterCenter.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public static class SearchUtility
    {

        /// <summary>
        /// Encodes search results before saving to the list.
        /// </summary>
        /// <param name="searchDetails">SavedSearchDetails object containing Current search details</param>
        /// <param name="isMatterSearch">Flag for matter or document search (true = matter, false = document)</param>
        public static void EncodeSearchDetails(FilterObject searchDetails, Boolean isMatterSearch)
        {
            // Encode all the values which are coming from the JS file
            searchDetails.FromDate = (null != searchDetails.FromDate) ? WebUtility.HtmlEncode(searchDetails.FromDate) : string.Empty;
            searchDetails.ToDate = (null != searchDetails.ToDate) ? WebUtility.HtmlEncode(searchDetails.ToDate) : string.Empty;

            if (searchDetails.AOLList !=null)
            {
                IList<string> encodedAOLList = new List<string>();
                foreach (string aolList in searchDetails.AOLList)
                {
                    if (!string.IsNullOrWhiteSpace(aolList))
                    {
                        encodedAOLList.Add(WebUtility.HtmlEncode(aolList));
                    }
                }
                searchDetails.AOLList = encodedAOLList;
            }

            if (searchDetails.PGList != null)
            {
                IList<string> encodedPGList = new List<string>();
                foreach (string pgList in searchDetails.PGList)
                {
                    if (!string.IsNullOrWhiteSpace(pgList))
                    {
                        encodedPGList.Add(WebUtility.HtmlEncode(pgList));
                    }
                }
                searchDetails.PGList = encodedPGList;
            }


            if (searchDetails.ClientsList != null)
            {
                IList<string> encodedClientsList = new List<string>();
                foreach (string clientsList in searchDetails.ClientsList)
                {
                    if (!string.IsNullOrWhiteSpace(clientsList))
                    {
                        encodedClientsList.Add(WebUtility.HtmlEncode(clientsList));
                    }
                }
                searchDetails.ClientsList = encodedClientsList;
            }

            if (searchDetails.DocumentAuthor != null)
            {
                searchDetails.DocumentAuthor = WebUtility.HtmlEncode(searchDetails.DocumentAuthor);
            }

            EncodeSearchDetailsUtility(searchDetails, isMatterSearch);
        }

        /// <summary>
        /// Encodes search results before saving to the list.
        /// </summary>
        /// <param name="searchDetails">SavedSearchDetails object containing Current search details</param>
        /// <param name="isMatterSearch">Flag for matter or document search (true = matter, false = document)</param>
        private static void EncodeSearchDetailsUtility(FilterObject searchDetails, Boolean isMatterSearch)
        {
            if (isMatterSearch && null != searchDetails.Name) // Encode name only in case of Matter search
            {
                searchDetails.Name = WebUtility.HtmlEncode(searchDetails.Name);
            }

            if (null != searchDetails.ResponsibleAttorneys)
            {
                searchDetails.ResponsibleAttorneys = WebUtility.HtmlEncode(searchDetails.ResponsibleAttorneys);
            }

            if (null != searchDetails.SubareaOfLaw)
            {
                searchDetails.SubareaOfLaw = WebUtility.HtmlEncode(searchDetails.SubareaOfLaw);
            }
        }
    }
}
