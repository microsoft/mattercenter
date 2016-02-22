// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
// Author           : v-prd
// Created          : 04-06-2014
//
// ***********************************************************************
// <copyright file="PracticeGroup.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains meta data of term store.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
{
    using System;
    #region using
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Client Class For storing all URLS an Names for object
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the parameters for Clients URL 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the  parameters for Clients Names
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Practice Group Class Inheriting Terms Value and storing All Area of Laws
    /// </summary>
    public class PracticeGroup : Terms
    {
        /// <summary>
        /// Gets or sets the object of an list
        /// </summary>
        public IList<AreaOfLaw> AreaOfLaw { get; set; }
    }

    /// <summary>
    /// Area Of Law Class Inheriting Terms Value and storing All Sub Area of Laws
    /// </summary>
    public class AreaOfLaw : Terms
    {
        /// <summary>
        /// Gets or sets the object of an list
        /// </summary>
        public IList<SubAreaOfLaw> SubareaOfLaw { get; set; }
    }

    /// <summary>
    /// Sub Area of Laws Class Inheriting Terms Value
    /// </summary>
    public class SubAreaOfLaw : Terms
    {
        /// <summary>
        /// Gets or sets the ContentType
        /// </summary>
        public string ContentType { get; set; }
    }

    /// <summary>
    /// Term Class Storing all Terms Value
    /// </summary>
    public class Terms
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the WssId
        /// </summary>
        public int WssId { get; set; }
    }
}
