using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Microsoft.Legal.MatterCenter.Repository
{
    public class SharedRepository:ISharedRepository
    {
        #region Private Variables
        private ISPPage spPage;
        private ListNames listNames;
        private ISearch search;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spPage"></param>
        /// <param name="listNames"></param>
        /// <param name="search"></param>
        public SharedRepository(ISPPage spPage, IOptions<ListNames> listNames, ISearch search)
        {
            this.spPage = spPage;
            this.listNames = listNames.Value;
            this.search = search;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public async Task<bool> UrlExistsAsync(Client client, string pageUrl)
        {
            return await Task.FromResult(spPage.UrlExists(client, pageUrl));            
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="selectedPage"></param>
        /// <returns></returns>
        public async Task<List<ContextHelpData>> GetMatterHelpAsync(Client client, string selectedPage)
        {
            return await Task.FromResult(search.GetMatterHelp(client, selectedPage, listNames.MatterCenterHelpSectionListName));
        }
    }
}
