using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface  IConfigRepository
    {
        Task<ConfigEntities> GetConfigurationsAsync(ConfigEntities configRequest);
        ConfigEntities GetConfigEntities();
    }
}
