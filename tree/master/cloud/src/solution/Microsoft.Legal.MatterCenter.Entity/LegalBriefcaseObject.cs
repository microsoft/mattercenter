// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-swmirj
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="LegalBriefcaseObject.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides the structure required for briefcase operations. It includes the document ID and URL.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Entity
{
    #region using
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Provides the structure required for Matter Center briefcase operations. It includes the document ID and URL.
    /// </summary>
    public class BriefcaseDetails
    {
        /// <summary>
        /// Gets or sets document ID. Represents the unique document in matter library.
        /// </summary>
        /// <value>The document identifier.</value>
        public string DocumentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets document URL. Represents the URL of document in matter library.
        /// </summary>
        /// <value>The document URL.</value>
        public string DocumentUrl
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required to perform briefcase operations (Update Copy, Matter Check In, and Matter Check Out and Detach) for particular briefcase document.
    /// </summary>
    public class SyncDetails
    {
        /// <summary>
        /// Gets or sets List ID of the item.
        /// </summary>
        /// <value>The list identifier.</value>
        public string ListId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets list of item ID. Represents the items selected in briefcase operations.
        /// </summary>
        /// <value>The item identifier.</value>
        public IList<int> ItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Integer value specifying the type of operation i.e. Update Copy, Matter Check In, and Matter Check Out and Detach.
        /// </summary>
        /// <value>The briefcase operation.</value>
        public int Operation
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the response/status of each document in legal briefcase operation.
    /// </summary>
    public class ResponseDetails
    {
        /// <summary>
        /// Gets or sets the status of operation requested. Indicates whether the operation was success/failure.
        /// </summary>
        /// <value>The status of operation.</value>
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of file names returned. Represents the files involved in the operation.
        /// </summary>
        /// <value>The file names.</value>
        public IList<string> FileNames
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the version details, status flag, and success/error message, relative URL for document after briefcase operation.
    /// </summary>
    public class VersionDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionDetails"/> class.
        /// </summary>
        public VersionDetails()
        {
            this.IsMajorVersion = new List<bool>();
            this.IsMinorVersion = new List<bool>();
            this.CurrentMinorVersion = new List<string>();
            this.CurrentMajorVersion = new List<string>();
            this.CheckOutStatus = new List<string>();
            this.RelativeURL = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VersionDetails"/> is having status.
        /// </summary>
        /// <value>The status indicating whether the document is checked out by logged-in user or not.</value>
        public bool Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets boolean indicator for major version.
        /// </summary>
        /// <value>The major version.</value>
        public IList<bool> IsMajorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets boolean indicator for minor version.
        /// </summary>
        /// <value>The minor version.</value>
        public IList<bool> IsMinorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets minor version of the item.
        /// </summary>
        /// <value>The current minor version.</value>
        public IList<string> CurrentMinorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets major version of the item.
        /// </summary>
        /// <value>The current major version.</value>
        public IList<string> CurrentMajorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets check out status for the item.
        /// </summary>
        /// <value>The check out status.</value>
        public IList<string> CheckOutStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets relative URL of the items.
        /// </summary>
        /// <value>The relative URL.</value>
        public IList<string> RelativeURL
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the file names, status, success/error message for briefcase operation.
    /// </summary>
    public class CommonResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonResponse"/> class.
        /// </summary>
        public CommonResponse()
        {
            this.Status = new List<bool>();
            this.FileNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets the status of the request. Indicates whether the operation was success/failure.
        /// </summary>
        /// <value>The status.</value>
        public IList<bool> Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets relative URL of the file. Represents the files involved in the operation.
        /// </summary>
        /// <value>The file names.</value>
        public IList<string> FileNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error message. Represents the error details in case of operation failure.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for mail attachment meta-data. It includes full URL of mail and flag to overwrite existing files, if present.
    /// </summary>

    public class MailAttachmentDetails
    {
        /// <summary>
        /// Gets or sets the full URL of mail attachment.
        /// </summary>
        /// <value>The full URL.</value>

        public string FullUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets overwrite option for mail attachment.
        /// </summary>
        /// <value>The overwrite.</value>

        public int IsOverwrite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets file position where check out failed
        /// </summary>
        public static int CheckoutFailedPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value based on attachment call for mail attachment.
        /// </summary>
        /// <value>The Attachment Flag.</value>
        public bool IsAttachmentCall
        {
            get;
            set;
        }
    }
}
