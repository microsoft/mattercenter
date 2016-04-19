// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="GeneralSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the general configuration information from the appSettings.json file
    /// These properties will subsequently used for authorizing to spo and storing data into redis cache
    /// </summary>
    public class GeneralSettings
    {
        public string ClientId { get; set; }
        public string AppKey { get; set; }
        public string Tenant { get; set; }
        public string AADInstance { get; set; }
        public string Resource { get; set; }        
        public string RedisCacheHostName { get; set; }
        public bool IsTenantDeployment { get; set; }
        public string IsReadOnlyUser { get; set; }        
    }
}
