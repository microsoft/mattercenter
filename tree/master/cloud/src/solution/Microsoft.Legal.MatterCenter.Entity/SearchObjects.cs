// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akdigh
// Created          : 03-06-2014
//
// ***********************************************************************
// <copyright file="SearchObjects.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines classes used by Search object.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Entity
{
    #region using
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Represents a legal matter. Provides the structure required to hold pinned matter meta-data. It includes name, description, created date, URL, practice group, area of law, sub area of law, client ID, and client name.
    /// </summary>
    public class MatterData
    {
        /// <summary>
        /// Gets or sets the name of the matter. Represents the name of matter library.
        /// </summary>
        /// <value>The name of the matter.</value>
        public string MatterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter description. Represents the description of matter library.
        /// </summary>
        /// <value>The matter description.</value>
        public string MatterDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter created date. Represents the matter library creation date.
        /// </summary>
        /// <value>The matter created date.</value>
        public string MatterCreatedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter URL. Represents the matter library URL present under the client.
        /// </summary>
        /// <value>The matter URL.</value>
        public string MatterUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter practice group. Represents the practice group associated with the matter.
        /// </summary>
        /// <value>The matter practice group.</value>
        public string MatterPracticeGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter area of law. Represents the area of law associated with the matter.
        /// </summary>
        /// <value>The matter area of law.</value>
        public string MatterAreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter sub area of law. Represents the sub area of law associated with the matter.
        /// </summary>
        /// <value>The matter sub area of law.</value>
        public string MatterSubAreaOfLaw
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter client URL. Represents the URL of client for the matter.
        /// </summary>
        /// <value>The matter client URL.</value>
        public string MatterClientUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter client. Represents the name of client for the matter.
        /// </summary>
        /// <value>The matter client.</value>
        public string MatterClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the matter client identifier. Represents the client ID for the matter.
        /// </summary>
        /// <value>The matter client identifier.</value>
        public string MatterClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HideUpload flag. Represents if the upload icon should be hidden from the user.
        /// </summary>
        /// <value>The hide upload.</value>
        public string HideUpload
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Matter ID. Represents the matter ID under the client.
        /// </summary>
        /// <value>The matter ID.</value>
        public string MatterID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the responsible attorney. Represents the attorney responsible for the matter.
        /// </summary>
        /// <value>The responsible attorney.</value>
        public string MatterResponsibleAttorney
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Modified date. Represents the modified date for the matter.
        /// </summary>
        /// <value>The Modified Date.</value>
        public string MatterModifiedDate
        {
            get;
            set;
        }

		/// <summary>
        /// Gets or sets the Matter GUID. Represents the GUID for the matter.
		/// </summary>
        /// <value>The Matter GUID.</value>
		public string MatterGuid
		{
			get;
			set;
		}
    }

    /// <summary>
    /// Represents a legal document. Provides the structure required to hold pinned document meta-data. It includes document name, version, created date, modified date, matter details for document, and client details for document.
    /// </summary>
    public class DocumentData
    {
        /// <summary>
        /// Gets or sets the name of the document. Represents the document under the matter library.
        /// </summary>
        /// <value>The name of the document.</value>
        public string DocumentName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document version. Represents the document version (minor, major, check out, etc.).
        /// </summary>
        /// <value>The document version.</value>
        public string DocumentVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document client. Represents the client name under which the document is present.
        /// </summary>
        /// <value>The document client.</value>
        public string DocumentClient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document client identifier. Represents the unique client ID under which the document is present.
        /// </summary>
        /// <value>The document client identifier.</value>
        public string DocumentClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document client URL. Represents the client URL under which the document is present.
        /// </summary>
        /// <value>The document client URL.</value>
        public string DocumentClientUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document matter. Represents the matter library that holds the document.
        /// </summary>
        /// <value>The document matter.</value>
        public string DocumentMatter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document matter identifier. Represents the matter ID that holds the document.
        /// </summary>
        /// <value>The document matter identifier.</value>
        public string DocumentMatterId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document owner who has uploaded the document. 
        /// </summary>
        /// <value>The document owner.</value>
        public string DocumentOwner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document URL.
        /// </summary>
        /// <value>The document URL.</value>
        public string DocumentUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document OWA URL. Represents the documents URL supported by OWA or Office online; otherwise the document path.
        /// </summary>
        public string DocumentOWAUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document extension.
        /// </summary>
        /// <value>The document extension.</value>
        public string DocumentExtension
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document created date. Represents the document uploaded date in matter library.
        /// </summary>
        /// <value>The document created date.</value>
        public string DocumentCreatedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document modified date. Represents the document last modified date in matter library.
        /// </summary>
        /// <value>The document modified date.</value>
        public string DocumentModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document check out user. Represents the check out user of document.
        /// </summary>
        /// <value>The document check out user.</value>
        public string DocumentCheckoutUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document matter URL. Represents the URL of matter library where document is present.
        /// </summary>
        /// <value>The document check out user.</value>
        public string DocumentMatterUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document parent URL. Represents the parent URL of document.
        /// </summary>
        /// <value>The document check out user.</value>
        public string DocumentParentUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document ID. Represents the ID of a document.
        /// </summary>
        /// <value>The document ID.</value>
        public string DocumentID
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for searching matters/documents. It includes page number, search keyword, filter conditions, sort conditions, and numbers of items to be shown on page.
    /// </summary>
    public class SearchObject
    {
        /// <summary>
        /// Gets or sets the page number. Represents the current page number that is displayed on the page.
        /// </summary>
        /// <value>The page number.</value>

        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items per page. Represents number of items to be shown on the page.
        /// </summary>
        /// <value>The items per page.</value>

        public int ItemsPerPage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term. Represents the search keyword/term to retrieve the results.
        /// </summary>
        /// <value>The search term.</value>

        public string SearchTerm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filters. Represents the filtering condition.
        /// </summary>
        /// <value>The filters.</value>

        public FilterObject Filters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sort. Represents the searching condition.
        /// </summary>
        /// <value>The sort condition.</value>

        public SortObject Sort
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for filtering matters/documents. It includes list of clients, practice group, area of law, sub area of law, from date, to date, and document author.
    /// </summary>
    public class FilterObject
    {
        /// <summary>
        /// Gets or sets the clients list. Represents the clients selected for filtering.
        /// </summary>
        /// <value>The clients list.</value>

        public IList<string> ClientsList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of practice group. Represents the practice groups selected for filtering.
        /// </summary>
        /// <value>The PGList.</value>

        public IList<string> PGList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of area of law. Represents the area of law selected for filtering.
        /// </summary>
        /// <value>The AOL list.</value>

        public IList<string> AOLList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FromDate. Represents the start date for filtering.
        /// </summary>
        /// <value>From date.</value>

        public string FromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ToDate. Represents the end date for filtering.
        /// </summary>
        /// <value>To date.</value>

        public string ToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the FilterByMe flag. Represents filtering on items created by logged-in user.
        /// </summary>
        /// <value>The filter by me.</value>

        public int FilterByMe
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document author. Represents authors selected for filtering.
        /// </summary>
        /// <value>The document author.</value>

        public string DocumentAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document/ matter name. Represents document/ matter name selected for filtering.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document's/ matter's client name. Represents document's/ matter's client name selected for filtering.
        /// </summary>
        public string ClientName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document's checked out user name. Represents the list of check out user names selected for filtering.
        /// </summary>
        public string DocumentCheckoutUsers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Date filters. Represents collection of date filters selected for filtering.
        /// </summary>
        public DateFilterObject DateFilters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Responsible attorney. Represents the list of Responsible attorneys selected for filtering.
        /// </summary>
        public string ResponsibleAttorneys
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sub area of Law. Represents Sub area of Law selected for filtering.
        /// </summary>
        public string SubareaOfLaw
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for storing date filter.
    /// </summary>
    public class DateFilterObject
    {
        /// <summary>
        /// Gets or sets the 'From' modified date. Represents 'From' modified date selected for filtering.
        /// </summary>
        public string ModifiedFromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'To' modified date. Represents 'To' modified date selected for filtering.
        /// </summary>
        public string ModifiedToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'From' created date. Represents 'From' created date selected for filtering.
        /// </summary>
        public string CreatedFromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'To' created date. Represents 'To' created date selected for filtering.
        /// </summary>
        public string CreatedToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'From' open date. Represents 'From' open date selected for filtering.
        /// </summary>
        public string OpenDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'To' open date. Represents 'To' open date selected for filtering.
        /// </summary>
        public string OpenDateTo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for sorting matters/documents. It includes sortable property and sorting direction.
    /// </summary>
    public class SortObject
    {
        /// <summary>
        /// Gets or sets the by property. Represents the property selected for sorting.
        /// </summary>
        /// <value>The by property.</value>

        public string ByProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the direction. Represents the order for sorting.
        /// </summary>
        /// <value>The direction.</value>

        public int Direction
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure for folder hierarchy within matter.
    /// </summary>
    public class FolderData
    {
        /// <summary>
        /// Gets or sets the name. Represents the folder under the matter library.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL. Represents the folder URL under the matter library.
        /// </summary>
        /// <value>The URL.</value>
        [JsonProperty(PropertyName = "url")]
        public string URL
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent URL. Represents the parent URL.
        /// </summary>
        /// <value>The parent URL.</value>
        [JsonProperty(PropertyName = "parenturl")]
        public string ParentURL
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure of mail/attachment mainly EWS URL, folder location, document library, and flag to overwrite.
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        /// Gets or sets the attachment token.
        /// </summary>
        /// <value>The attachment token.</value>
        public string AttachmentToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Exchange Web Service URL.
        /// </summary>
        /// <value>The EWS URL.</value>
        public Uri EwsUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachments. Represents the objects selected.
        /// </summary>
        /// <value>The attachments.</value>
        public IList<AttachmentDetails> Attachments
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail identifier.
        /// </summary>
        /// <value>The mail identifier.</value>
        public string MailId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the folder path. Represents the folder path where item needs to be uploaded.
        /// </summary>
        /// <value>The folder path.</value>
        public IList<string> FolderPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ServiceRequest"/> has overwrite option.
        /// </summary>
        /// <value><c>true</c> if overwrite; otherwise, <c>false</c>.</value>
        public bool Overwrite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets document library name.
        /// </summary>
        /// <value>Name of document Library</value>
        public string DocumentLibraryName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to perform content check or not.
        /// </summary>
        /// <value>Name of document Library</value>
        public bool PerformContentCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Gets content check flag whether allowed or not
        /// </summary>
        /// <value>Content check enabled or not</value>
        public bool AllowContentCheck
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Provides the structure for mail attachment details. It includes MIME type, content type, attachment name, and size.
    /// </summary>
    public class AttachmentDetails
    {
        /// <summary>
        /// Gets or sets the type of the attachment.
        /// </summary>
        /// <value>The type of the attachment.</value>
        public string attachmentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string contentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mail identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in-line.
        /// </summary>
        /// <value><c>true</c> if this instance is in-line; otherwise, <c>false</c>.</value>
        public bool isInline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment name.
        /// </summary>
        /// <value>The name.</value>
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment size.
        /// </summary>
        /// <value>The size.</value>
        public int size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original name of attachment
        /// </summary>
        public string originalName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for SavedSearch meta-data and inherits from <see cref="FilterObject"/> class.
    /// </summary>
    public class SavedSearchDetails : FilterObject
    {
        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public string DateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the modified date time.
        /// </summary>
        /// <value>The modified date time.</value>
        public string ModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>The search term.</value>
        public string SearchTerm
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for pin/unpin operation performed on matter/document. Meta-data includes the list name, list column, URL, and matter/document details.
    /// </summary>
    public class PinUnpinDetails
    {
        /// <summary>
        /// Gets or sets the name of the list for operation.
        /// </summary>
        /// <value>The name of the list.</value>
        public string ListName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pinned list column details.
        /// </summary>
        /// <value>The pinned list column details.</value>
        public string PinnedListColumnDetails
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL of matter/document.
        /// </summary>
        /// <value>The URL.</value>
        public string URL
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user pinned matter data.
        /// </summary>
        /// <value>The user pinned matter data.</value>
        public MatterData UserPinnedMatterData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user pinned document data.
        /// </summary>
        /// <value>The user pinned document data.</value>
        public DocumentData UserPinnedDocumentData
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides the structure required for mail XML response
    /// </summary>
    public struct MailXPath
    {
        /// <summary>
        /// Gets or sets the user who receives the mail.
        /// </summary>
        /// <value>Name of the user who receives the mail</value>
        public string mailReceiver { get; set; }

        /// <summary>
        /// Gets or sets the user who are in CC.
        /// </summary>
        /// <value>Name of the user who are in CC</value>
        public string mailCC { get; set; }

        /// <summary>
        /// Gets or sets the mail received date.
        /// </summary>
        /// <value>Mail received date</value>
        public string mailRecieved { get; set; }

        /// <summary>
        /// Gets or sets the user who sends the mail.
        /// </summary>
        /// <value>Name of the user who sends the mail</value>
        public string mailFromName { get; set; }

        /// <summary>
        /// Gets or sets the address of the mail.
        /// </summary>
        /// <value>Address of the mail</value>
        public string mailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the importance of the mail.
        /// </summary>
        /// <value>Importance of the mail</value>
        public string mailImportance { get; set; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        /// <value>Subject of the mail</value>
        public string mailSubject { get; set; }

        /// <summary>
        /// Gets or sets the conversation id of the mail.
        /// </summary>
        /// <value>Conversation id of the mail</value>
        public string mailConversationId { get; set; }

        /// <summary>
        /// Gets or sets the sensitivity of the mail.
        /// </summary>
        /// <value>Sensitivity of the mail</value>
        public string mailSensitivity { get; set; }

        /// <summary>
        /// Gets or sets the conversation topic of the mail.
        /// </summary>
        /// <value>Conversation topic of the mail</value>
        public string mailConversationTopic { get; set; }

        /// <summary>
        /// Gets or sets the sent date of the mail.
        /// </summary>
        /// <value>Sent date of the mail</value>
        public string mailSentDate { get; set; }

        /// <summary>
        /// Gets or sets the 'Has attachments' of the mail.
        /// </summary>
        /// <value>'Has attachments' value of the mail</value>
        public string mailHasAttachments { get; set; }

        /// <summary>
        /// Gets or sets the categories of the mail.
        /// </summary>
        /// <value>Categories of the mail</value>
        public string mailCategories { get; set; }
    }

    /// <summary>
    /// Provides the structure to hold mail meta data
    /// </summary>
    public struct MailMetaData
    {
        /// <summary>
        /// Gets or sets the user who receives the mail.
        /// </summary>
        /// <value>Name of the user who receives the mail</value>
        public string mailReceiver { get; set; }

        /// <summary>
        /// Gets or sets the user who sends the mail.
        /// </summary>
        /// <value>Name of the user who sends the mail</value>
        public string mailSender { get; set; }

        /// <summary>
        /// Gets or sets the mail received date.
        /// </summary>
        /// <value>Mail received date</value>
        public string receivedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who are in CC.
        /// </summary>
        /// <value>Name of the user who are in CC</value>
        public string cc { get; set; }

        /// <summary>
        /// Gets or sets the attachment of the mail.
        /// </summary>
        /// <value>Attachment of the mail</value>
        public string attachment { get; set; }

        /// <summary>
        /// Gets or sets the importance of the mail.
        /// </summary>
        /// <value>Importance of the mail</value>
        public string mailImportance { get; set; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        /// <value>Subject of the mail</value>
        public string mailSubject { get; set; }

        /// <summary>
        /// Gets or sets the categories of the mail
        /// </summary>
        /// <value>Category of the mail</value>
        public string categories { get; set; }

        /// <summary>
        /// Gets or sets the sensitivity of the mail
        /// </summary>
        /// <value>Sensitivity of the mail</value>
        public string sensitivity { get; set; }

        /// <summary>
        /// Gets or sets the conversation id of the mail
        /// </summary>
        /// <value>Conversation id of the mail</value>
        public string conversationId { get; set; }

        /// <summary>
        /// Gets or sets the conversation topic  of the mail
        /// </summary>
        /// <value>Conversation topic of the mail</value>
        public string conversationTopic { get; set; }

        /// <summary>
        /// Gets or sets the sent date of the mail
        /// </summary>
        /// <value>Sent date of the mail</value>
        public string sentDate { get; set; }

        /// <summary>
        /// Gets or sets the 'Has attachments' of the mail
        /// </summary>
        /// <value>'Has attachments' of the mail</value>
        public string hasAttachments { get; set; }

        /// <summary>
        /// Gets or sets the original name of the mail
        /// </summary>
        /// <value>Original name of the mail</value>
        public string originalName { get; set; }
    }

    /// <summary>
    /// Provides the structure required to store content for contextual help functionality.
    /// </summary>
    public class ContextHelpData
    {
        /// <summary>
        /// Gets or sets the structure required to store content for contextual help sections.
        /// </summary>
        /// <value>Provides the structure required to store content for contextual help sections.</value>
        public ContextHelpSection ContextSection { get; set; }
        /// <summary>
        /// Gets or sets the link title for operation.
        /// </summary>
        /// <value>Link title of contextual help fly out</value>
        public string LinkTitle { get; set; }
        /// <summary>
        /// Gets or sets the Link URL for operation.
        /// </summary>
        /// <value>Link URL of contextual help fly out</value>
        public string LinkURL { get; set; }
        /// <summary>
        /// Gets or sets the Link order for operation.
        /// </summary>
        /// <value>Link order of contextual help fly out</value>
        public string LinkOrder { get; set; }
    }

    /// <summary>
    /// Provides the structure required to store content for contextual help sections.
    /// </summary>
    public class ContextHelpSection
    {
        /// <summary>
        /// Gets or sets the section id for operation.
        /// </summary>
        /// <value>Section id of contextual help fly out</value>
        public string SectionID { get; set; }
        /// <summary>
        /// Gets or sets the section title for operation.
        /// </summary>
        /// <value>Section title of contextual help fly out</value>
        public string SectionTitle { get; set; }
        /// <summary>
        /// Gets or sets the section order for operation.
        /// </summary>
        /// <value>Section order of contextual help fly out</value>
        public string SectionOrder { get; set; }
        /// <summary>
        /// Gets or sets the page name for operation.
        /// </summary>
        /// <value>Page name of contextual help fly out</value>
        public string PageName { get; set; }
        /// <summary>
        /// Gets or sets the number of columns for help links.
        /// </summary>
        /// <value>Number of columns to be displayed on the contextual help section</value>
        public string NumberOfColumns { get; set; }
    }

    /// <summary>
    /// Provides the structure required for performing content check.
    /// </summary>
    public class ContentCheckDetails
    {
        /// <summary>
        /// Gets or sets the name of the file for which content check needs to be performed
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the Subject field of the file for which content check needs to be performed. Applicable only for email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the size of the file for which content check needs to be performed
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the From field of the file for which content check needs to be performed. Applicable only for email
        /// </summary>
        public string FromField { get; set; }

        /// <summary>
        /// Gets or sets the Sent date field of the file for which content check needs to be performed. Applicable only for email
        /// </summary>
        public string SentDate { get; set; }

        /// <summary>
        /// Two-parameters constructor to initialize object for checking if document exists with the same file name and size
        /// </summary>
        /// <param name="fileName">Name of the file being uploaded</param>
        /// <param name="fileSize">Size of the file being uploaded</param>
        public ContentCheckDetails(string fileName, long fileSize)
        {
            this.FileName = fileName;
            this.Subject = string.Empty;
            this.FileSize = fileSize;
            this.FromField = string.Empty;
            this.SentDate = string.Empty;
        }

        /// <summary>
        /// Four-parameters constructor to initialize object for checking if mail exists with the same file name, size, from field and sent date
        /// </summary>
        /// <param name="fileName">Name of the mail being uploaded</param>
        /// <param name="fileSize">Size of the mail being uploaded</param>
        /// <param name="fromField">Value in the From field of the mail being uploaded</param>
        /// <param name="sentDate">Sent date of the mail being uploaded</param>
        public ContentCheckDetails(string fileName, string subject, long fileSize, string fromField, string sentDate)
        {
            this.FileName = fileName;
            this.Subject = subject;
            this.FileSize = fileSize;
            this.FromField = fromField;
            this.SentDate = sentDate;
        }
    }

    /// <summary>
    /// Provides the structure required for checking if document already exists.
    /// </summary>
    public class DuplicateDocument
    {
        /// <summary>
        /// Gets or sets a value indicating whether a document with the same name exists or not.
        /// </summary>
        /// <value><c>true</c> if document with the same name found; otherwise, <c>false</c></value>
        public bool DocumentExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a potential duplicate exists, i.e. along with name, other properties also match
        /// </summary>
        /// <value><c>true</c> if potential duplicate found; otherwise, <c>false</c></value>
        public bool HasPotentialDuplicate { get; set; }

        /// <summary>
        /// Constructor to initialize the DuplicateDocument object
        /// </summary>
        /// <param name="documentExists">Default value for DocumentExists property</param>
        /// <param name="hasPotentialDuplicate">Default value for HasPotentialDuplicate property</param>
        public DuplicateDocument(bool documentExists, bool hasPotentialDuplicate)
        {
            this.DocumentExists = documentExists;
            this.HasPotentialDuplicate = hasPotentialDuplicate;
        }
    }
}
