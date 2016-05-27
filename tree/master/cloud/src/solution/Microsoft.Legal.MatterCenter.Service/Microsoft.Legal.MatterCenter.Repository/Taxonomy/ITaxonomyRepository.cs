using Microsoft.Legal.MatterCenter.Models;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ITaxonomyRepository
    {
        Task<TaxonomyResponseVM> GetTaxonomyHierarchyAsync(TermStoreViewModel termStoreViewModel);
        string GetCurrentSiteName(Client client);
    }
}
