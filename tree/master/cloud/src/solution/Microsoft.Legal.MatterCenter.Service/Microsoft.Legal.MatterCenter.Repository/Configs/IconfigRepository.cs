using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface  IConfigRepository
    {
        Task<List<DynamicTableEntity>> GetConfigurationsAsync(String filter);
        List<DynamicTableEntity>  GetConfigEntities(string filter);
    }
}
