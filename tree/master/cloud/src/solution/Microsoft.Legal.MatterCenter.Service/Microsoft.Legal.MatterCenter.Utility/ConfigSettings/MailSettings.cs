// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="MailSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the mail configuration information from the appSettings.json file
    /// These properties will subsequently used when uploading email and email attachments
    /// </summary>
    public class MailSettings
    {
        //public string ManagedPropertyTitle { get; set; }
        //public string ManagedPropertyName { get; set; }
        //public string ManagedPropertyDescription { get; set; }
        //public string ManagedPropertySiteName { get; set; }
        //public string ManagedPropertyLastModifiedTime { get; set; }
        //public string ManagedPropertyPracticeGroup { get; set; }
        //public string ManagedPropertyAreaOfLaw { get; set; }
        //public string ManagedPropertySubAreaOfLaw { get; set; }
        //public string ManagedPropertyMatterId { get; set; }
        //public string ManagedPropertyCustomTitle { get; set; }
        //public string ManagedPropertyPath { get; set; }
        //public string ManagedPropertyMatterName { get; set; }
        //public string ManagedPropertyOpenDate { get; set; }
        //public string ManagedPropertyClientName { get; set; }
        //public string ManagedPropertyBlockedUploadUsers { get; set; }
        //public string ManagedPropertyResponsibleAttorney { get; set; }
        //public string ManagedPropertyClientID { get; set; }
        //public string ManagedPropertyMatterGuidLogging { get; set; }
        //public string ManagedPropertyTeamMembers { get; set; }
        //public string ManagedPropertyFileName { get; set; }
        //public string ManagedPropertyDocumentCheckOutUser { get; set; }
        //public string ManagedPropertyCreated { get; set; }

        public string MatterMailBodyMatterInformation { get; set; }
        public string MatterMailDefaultContentTypeHtmlChunk { get; set; }
        public string MatterMailSubject { get; set; }
        public string ShareListColumnMailBody { get; set; }
        public string ShareListColumnMailSubject { get; set; }
        public string MatterMailBodyConflictCheck { get; set; }
        public string MatterMailBodyTeamMembers { get; set; }

        public string SearchEmailTo { get; set; }
        public string SearchEmailSubject { get; set; }
        public string SearchEmailSentDate { get; set; }
        public string SearchEmailSensitivity { get; set; }
        public string SearchEmailReceivedDate { get; set; }
        public string SearchEmailImportance { get; set; }
        public string SearchEmailHasAttachments { get; set; }
        public string SearchEmailFromMailbox { get; set; }
        public string SearchEmailFrom { get; set; }
        public string SearchEmailConversationTopic { get; set; }
        public string SearchEmailCC { get; set; }
        public string SearchEmailCategories { get; set; }
        public string SearchEmailAttachments { get; set; }
        public string SearchEmailConversationId { get; set; }
        public string SearchEmailOriginalName { get; set; }
        public string NoMailSubject { get; set; }
        public string SearchEmailFileSize { get; set; }
        public int SentDateTolerance { get; set; }
    }
}
