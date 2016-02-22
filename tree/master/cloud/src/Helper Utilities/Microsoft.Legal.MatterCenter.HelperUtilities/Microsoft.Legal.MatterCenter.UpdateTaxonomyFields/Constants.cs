// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
// Author           : v-prd
// Created          : 04-06-2014
//
// ***********************************************************************
// <copyright file="Constants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains constants used in project.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
{
    /// <summary>
    /// Class For Storing all Constants and key Values used in the solution
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Meta data default value
        /// </summary>
        internal const string MetadataDefaultValue = "{0};#{1}|{2}";

        /// <summary>
        /// To check is matter key
        /// </summary>
        internal const string MatterKey = "IsMatter";

        /// <summary>
        /// Backslash symbol
        /// </summary>
        internal const string BackSlash = "\\";

        /// <summary>
        /// Check for checked out user
        /// </summary>
        internal const string CheckOutFieldKey = "CheckoutUser";

        /// <summary>
        /// Client URL
        /// </summary>
        internal const string URLDictKey = "ClientURL";

        /// <summary>
        /// Content type key
        /// </summary>
        internal const string ContentTypeDictKey = "ContentTypeName";

        /// <summary>
        /// Processing matters and documents message
        /// </summary>
        internal const string ProcessingMessage = "Processing matters and documents for client: {0}";

        /// <summary>
        /// Processing documents message
        /// </summary>
        internal const string ProcessingMessageDocuments = "Processing Documents for matter: {0}";

        /// <summary>
        /// Failure document message
        /// </summary>
        internal const string FailureMessageDocument = "The default values for old documents are not Updated For Client : {0} , Matter Name : {1} , due to {2}";

        /// <summary>
        /// Failure message for documents which are checked out
        /// </summary>
        internal const string FailureMessageUpdationDocument = "Skipping Document : {0} ,Because Document is Check Out to user {1}";

        /// <summary>
        /// Success Document message
        /// </summary>
        internal const string SucessMessageUpdationDocument = "Processed Document : {0} ";

        /// <summary>
        /// Success Matter message
        /// </summary>
        internal const string SuccessMessageMatter = "Processed matter: {0}";

        /// <summary>
        /// Skipped matter message
        /// </summary>
        internal const string SkippedMatterMessageMatter = "Skipped matter: {0}";

        /// <summary>
        /// Processing documents for matter message
        /// </summary>
        internal const string ProcessingMatterdocumentsMessage = "Processing documents for matter : {0}";

        /// <summary>
        /// Failure message
        /// </summary>
        internal const string FailureMessage = "The values are not updated For Client : {0} , Matter Name : {1}, due To : {2}";

        /// <summary>
        /// Client failure message
        /// </summary>
        internal const string ClientFailureMessage = "The default values are not updated For Client : {0} , due To : {1}";

        /// <summary>
        /// Failure of wssid retrieval
        /// </summary>
        internal const string WssIdRetrievalFailureMessage = "The Values of wssId are not retrieved due To : {0}";

        /// <summary>
        /// Matter center default content type
        /// </summary>
        internal const string MatterContentTypeKey = "MatterCenterDefaultContentType";

        /// <summary>
        /// Document name
        /// </summary>
        internal const string DocNameFieldKey = "FileLeafRef";

        /// <summary>
        /// Custom property not updated properly error
        /// </summary>
        internal const string StampPropertyNotUpdated = "Custom Properties are not updated properly: {0}";

        /// <summary>
        /// Retrieve all documents
        /// </summary>
        internal const string CAMLQueryRetrieveAllDocuments = @"<View Scope='RecursiveAll'>  
                <Query> 
                   <Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>0</Value></Eq></Where> 
                </Query> 
                 <ViewFields><FieldRef Name={0} /><FieldRef Name={1} /><FieldRef Name={2} /><FieldRef Name='CheckoutUser' /> <FieldRef Name='title'/>
       </ViewFields> 
            </View>";
    }
}
