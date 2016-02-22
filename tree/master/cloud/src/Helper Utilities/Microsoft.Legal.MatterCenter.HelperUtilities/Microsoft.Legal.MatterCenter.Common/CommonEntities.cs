// ***********************************************************************
// <copyright file="CommonEntities.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines structure for Clients Term Set from Term Store.</summary>
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Common
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Common
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A Client refer to term store client termSet
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets Client ID of a client present is term store
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets Name of the Client present in term store
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets Site Collection URL
        /// </summary>
        public string ClientUrl { get; set; }
    }

    /// <summary>
    /// Class represents collections of Clients
    /// </summary>
    public class ClientTermSets
    {
        /// <summary>
        /// Gets or sets List of clients
        /// </summary>
        public IList<Client> ClientTerms { get; set; }
    }
}
