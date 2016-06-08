// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="IMatterRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// This interface contains all the matter related functionalities
// ***********************************************************************

using Microsoft.Legal.MatterCenter.Models;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// Interface matter repository contains all matter related methods such as finding matter, creating matter, pin, inpin, update matter etc
    /// </summary>
    public interface IDocumentRepository:ICommonRepository
    {
        Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM);
        Task<dynamic> GetDocumentAndClientGUIDAsync(Client client);
    }
}
