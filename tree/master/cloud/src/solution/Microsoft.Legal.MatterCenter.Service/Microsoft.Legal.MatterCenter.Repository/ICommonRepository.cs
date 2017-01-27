using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ICommonRepository
    {
        Task<SearchResponseVM> GetPinnedRecordsAsync(SearchRequestVM searchRequestVM, ClientContext clientContext);
        Task<bool> PinRecordAsync<T>(T pinData);
        Task<bool> UnPinRecordAsync<T>(T unpinData);
    }
}
