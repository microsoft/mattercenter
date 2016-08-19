
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="ErrorSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting all the error messages that are used in matter center from the appSettings.json file
    /// These properties will subsequently used where ever exceptions messages needs to be set
    /// </summary>
    public class ClientCredentialsSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientResource { get; set; }
        public string Authority { get; set; }
    }
}


