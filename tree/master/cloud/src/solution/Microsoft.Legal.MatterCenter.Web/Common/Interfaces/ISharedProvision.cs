using Microsoft.Legal.MatterCenter.Models;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface ISharedProvision
    {
        Task<int> GetAllCounts(SearchRequestVM searchRequestVM);
        Task<int> GetMyCounts(SearchRequestVM searchRequestVM);
        Task<int> GetPinnedCounts(SearchRequestVM searchRequestVM);
    }
}
