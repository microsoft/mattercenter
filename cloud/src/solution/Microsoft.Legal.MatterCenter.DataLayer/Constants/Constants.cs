// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.DataLayer
// Author           : v-swmirj
// Created          : 06-16-2015
//
// ***********************************************************************
// <copyright file="Constants.cs" company="Microsoft.Legal.MatterCenter.DataLayer">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines constants used under current project.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.DataLayer
{
    #region using
    using System.Collections.Generic;
    #endregion

    static class Constants
    {

        /// <summary>
        /// Holds the relative location for OneNote file that is uploaded upon matter creation
        /// </summary>
        internal const string ONENOTERELATIVEFILEPATH = "~/Open Notebook.onetoc2";

        /// <summary>
        /// Constant required for extension for OneNote table of content
        /// </summary>
        internal const string EXTENSIONONENOTETABLEOFCONTENT = ".onetoc2";

        /// <summary>
        /// The true constant
        /// </summary>
        internal const string TRUE = "true";

        /// <summary>
        /// The false constant
        /// </summary>
        internal const string FALSE = "false";

        /// <summary>
        /// The colon constant
        /// </summary>
        internal const string COLON = ":";

        /// <summary>
        /// The forward slash constant
        /// </summary>
        internal const string FORWARDSLASH = "/";
        /// <summary>
        /// Web Query string
        /// </summary>
        internal const string WEBSTRING = "?Web=1";
        /// <summary>
        /// The dollar constant
        /// </summary>
        internal const string DOLLAR = "$";

        /// <summary>
        /// Modified date column
        /// </summary>
        internal const string MODIFIED_DATE_COLUMN = "Modified";

        /// <summary>
        /// The pipe constant
        /// </summary>
        internal const string PIPE = "|";

        /// <summary>
        /// Zone index to be used while creating matter page
        /// </summary>
        internal const int ZONEINDEX = 1;

        /// <summary>
        /// Title column for list item
        /// </summary>
        internal const string TITLE = "Title";
    }

    public class ListInformation
    {
        public string name
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }

        public string templateType
        {
            get;
            set;
        }

        public bool isContentTypeEnable
        {
            get;
            set;
        }

        public IList<string> folderNames
        {
            get;
            set;
        }

        public VersioningInfo versioning
        {
            get;
            set;
        }

		public string Path
		{
			get;
			set;
		}
    }

    public class VersioningInfo
    {
        public bool EnableVersioning
        {
            get;
            set;
        }
        public bool EnableMinorVersions
        {
            get;
            set;
        }
        public bool ForceCheckout
        {
            get;
            set;
        }

    }
}
