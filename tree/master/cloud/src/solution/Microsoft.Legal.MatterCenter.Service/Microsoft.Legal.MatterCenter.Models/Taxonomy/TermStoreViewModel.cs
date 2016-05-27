// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="TermStoreViewModel.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// This is a view mode object that will be passed by the client to 
    /// the service for getting the taxonomy hierarchy and this is used in taxonomy controller
    /// </summary>
    public class TermStoreViewModel
    {
        public Client Client { get; set; }
        public TermStoreDetails TermStoreDetails { get; set; }
    }
}
