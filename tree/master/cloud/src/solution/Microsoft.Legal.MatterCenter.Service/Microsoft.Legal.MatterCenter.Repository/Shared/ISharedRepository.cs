using Microsoft.Legal.MatterCenter.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISharedRepository
    {
        Task<bool> UrlExistsAsync(Client client, string pageUrl);
        Task<List<ContextHelpData>> GetMatterHelpAsync(Client client, string selectedPage);
    }
}
