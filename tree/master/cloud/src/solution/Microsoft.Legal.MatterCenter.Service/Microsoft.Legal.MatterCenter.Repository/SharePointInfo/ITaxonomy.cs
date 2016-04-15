using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ITaxonomy
    {
        TaxonomyResponseVM GetTaxonomyHierarchy (Client client, TermStoreDetails termStoreDetails);
    }
}
