
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-lapedd
// Created          : 04-11-2016
//
// ***********************************************************************
// <copyright file="PinRequestVM.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used for pinning, un pinning and get pinned data</summary>
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Models
{
    public class PinRequestMatterVM
    {
        public Client Client { get; set; }
        public MatterData MatterData { get; set; }        
    }
}
