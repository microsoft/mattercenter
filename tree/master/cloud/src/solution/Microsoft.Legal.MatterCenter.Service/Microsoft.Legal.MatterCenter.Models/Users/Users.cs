// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-nikhid
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="TermStoreDetails.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This class contains all the properties related to current logged in user</summary>
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold user meta-data. It includes user ID, name/alias, logged-in user name, and email.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>The name of the user.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logged-in user name.
        /// </summary>
        /// <value>The name of the user log on.</value>

        public string LogOnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        /// <value>The user email.</value>

        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity type (user or security group).
        /// </summary>
        public string EntityType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity type (user or security group).
        /// </summary>
        public string LargePictureUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity type (user or security group).
        /// </summary>
        public string SmallPictureUrl
        {
            get;
            set;
        }
    }
}
