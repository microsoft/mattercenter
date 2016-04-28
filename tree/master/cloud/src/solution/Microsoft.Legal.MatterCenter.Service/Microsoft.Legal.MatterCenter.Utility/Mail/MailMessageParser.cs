// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-prd
// Created          : 07-02-2014
//
// ***********************************************************************
// <copyright file="MailMessageParser.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file parses EML mail messages.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    #endregion

    /// <summary>    
    /// Stores all MIME decoded information of a received email. One email might consist of
    /// several MIME entities, which have a very similar structure to an email. A RxMailMessage
    /// can be a top most level email or a MIME entity the emails contains.    
    /// According to various RFCs, MIME entities can contain other MIME entities 
    /// recursively. However, they usually need to be mapped to alternative views and 
    /// attachments, which are non recursive.
    /// MailMessageParser inherits from System.Net.MailMessage, but provides additional receiving related information.
    /// </summary>
    public class MailMessageParser : MailMessage
    {
        /// <summary>
        /// A MIME entity can contain several MIME entities. A MIME entity has the same structure
        /// like an email. 
        /// </summary>
        private List<MailMessageParser> entities;

        /// <summary>
        /// ContentDisposition contains same information as stored in the ContentType.
        /// </summary>
        private ContentDisposition contentDisposition;

        /// <summary>
        /// similar as TransferType, but .NET supports only "7bit" / "quoted-printable"
        /// / "base64" here, "bit8" is marked as "bit7" (i.e. no transfer encoding needed),         
        /// </summary>
        private TransferEncoding contentTransferEncoding;

        /// <summary>
        /// The Content-Type field is used to specify the nature of the data in the body of a
        /// MIME entity, by giving media type and subtype identifiers, and by providing 
        /// auxiliary information that may be required for certain media types.
        /// </summary>
        private ContentType contentType;

        /// <summary>
        /// MailParser can be used for any MIME entity, as a normal message body, an attachment or an alternative view. ContentStream
        /// provides the actual content of that MIME entity. It's mainly used internally and later mapped to the corresponding 
        /// .NET types.
        /// </summary>
        private Stream contentStream;

        /// <summary>
        /// This entity can be part of a parent entity.
        /// </summary>
        private MailMessageParser parent;

        /// <summary>
        /// The top most MIME entity this MIME entity belongs to parent.
        /// </summary>
        private MailMessageParser topParent;

        /// <summary>
        /// It may be desirable to allow one body to make reference to another. Accordingly, 
        /// bodies may be labeled using the "Content-ID" header field.    
        /// </summary>
        private string contentId;

        /// <summary>
        /// Header lines not interpretable by Pop3ClientEmail.   
        /// </summary>
        private List<string> unknowHeaderlines;

        /// <summary>
        /// When mail is delivered
        /// </summary>
        private DateTime deliveryDate;

        /// <summary>
        /// When mail is received
        /// </summary>
        private DateTime receivedDate;

        /// <summary>
        /// Body of the Email.
        /// </summary>
        private string contentDescription;

        /// <summary>
        /// .NET framework combines MediaType (text) with subtype (plain) in one property, but
        /// often one or the other is needed alone. MediaMainType in this example would be 'text'.
        /// </summary>
        private string mediaMainType;

        /// <summary>
        /// Email importance
        /// </summary>
        private string mailImportance;

        /// <summary>
        /// Email categories
        /// </summary>
        private string mailCategories;

        /// <summary>
        /// Initializes a new instance of the MailMessageParser class
        /// </summary>
        public MailMessageParser()
        {
            ////for the moment, we assume to be at the top
            ////should this entity become a child, TopParent will be overwritten.
            this.TopParent = this;
            this.Entities = new List<MailMessageParser>();
            this.UnknowHeaderlines = new List<string>();
        }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        public List<MailMessageParser> Entities
        {
            get
            {
                return this.entities;
            }

            set
            {
                this.entities = value;
            }
        }

        /// <summary>
        /// Gets or sets the delivery date.
        /// </summary>
        /// <value>
        /// The delivery date.
        /// </value>
        public DateTime DeliveryDate
        {
            get
            {
                return this.deliveryDate;
            }

            set
            {
                this.deliveryDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the received date.
        /// </summary>
        /// <value>
        /// The received date.
        /// </value>
        public DateTime ReceivedDate
        {
            get
            {
                return this.receivedDate;
            }

            set
            {
                this.receivedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the content description.
        /// </summary>
        /// <value>
        /// The content description.
        /// </value>
        public string ContentDescription
        {
            get
            {
                return this.contentDescription;
            }

            set
            {
                this.contentDescription = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the media main.
        /// </summary>
        /// <value>
        /// The type of the media main.
        /// </value>
        public string MediaMainType
        {
            get
            {
                return this.mediaMainType;
            }

            set
            {
                this.mediaMainType = value;
            }
        }

        /// <summary>
        /// Gets or sets the content disposition.
        /// </summary>
        /// <value>
        /// The content disposition.
        /// </value>
        internal ContentDisposition ContentDisposition
        {
            get
            {
                return this.contentDisposition;
            }

            set
            {
                this.contentDisposition = value;
            }
        }

        /// <summary>
        /// Gets or sets the content transfer encoding.
        /// </summary>
        /// <value>
        /// The content transfer encoding.
        /// </value>
        internal TransferEncoding ContentTransferEncoding
        {
            get
            {
                return this.contentTransferEncoding;
            }

            set
            {
                this.contentTransferEncoding = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        internal ContentType ContentType
        {
            get
            {
                return this.contentType;
            }

            set
            {
                this.contentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the media sub.
        /// </summary>
        /// <value>
        /// The type of the media sub.
        /// </value>
        internal string MediaSubType
        {
            get
            {
                return this.mediaMainType;
            }

            set
            {
                this.mediaMainType = value;
            }
        }

        /// <summary>
        /// Gets or sets the content stream.
        /// </summary>
        /// <value>
        /// The content stream.
        /// </value>
        internal Stream ContentStream
        {
            get
            {
                return this.contentStream;
            }

            set
            {
                this.contentStream = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        internal MailMessageParser Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the top parent.
        /// </summary>
        /// <value>
        /// The top parent.
        /// </value>
        internal MailMessageParser TopParent
        {
            get
            {
                return this.topParent;
            }

            set
            {
                this.topParent = value;
            }
        }

        /// <summary>
        /// Gets or sets the content identifier.
        /// </summary>
        /// <value>
        /// The content identifier.
        /// </value>
        internal string ContentId
        {
            get
            {
                return this.contentId;
            }

            set
            {
                this.contentId = value;
            }
        }

        /// <summary>
        /// Gets or sets the unknown header lines.
        /// </summary>
        /// <value>
        /// The unknown header lines.
        /// </value>
        internal List<string> UnknowHeaderlines
        {
            get
            {
                return this.unknowHeaderlines;
            }

            set
            {
                this.unknowHeaderlines = value;
            }
        }

        /// <summary>
        /// Gets or sets the mail importance
        /// </summary>
        public string MailImportance
        {
            get
            {
                return this.mailImportance;
            }
            set
            {
                this.mailImportance = value;
            }
        }

        /// <summary>
        /// Gets or sets the mail categories
        /// </summary>
        public string MailCategories
        {
            get
            {
                return this.mailCategories;
            }
            set
            {
                this.mailCategories = value;
            }
        }

        /// <summary>
        /// Creates Mail message object from file.
        /// </summary>
        /// <param name="mailPath">EML file path.</param>
        /// <returns>Mime decoded email object</returns>
        public static MailMessageParser CreateFromFile(string mailPath)
        {
            using (MailMimeReader mimeDecoder = new MailMimeReader())
            {
                return mimeDecoder.GetEmail(mailPath);
            }
        }

        /// <summary>
        /// Creates Mail message object from file.
        /// </summary>
        /// <param name="mimeDecoder">read MIME object</param>
        /// <param name="mailPath">EML file path.</param>
        /// <returns>Mime decoded email object</returns>
        public static MailMessageParser CreateFromFile(MailMimeReader mimeDecoder, string mailPath)
        {
            MailMessageParser result = null;
            if (null != mimeDecoder)
            {
                result = mimeDecoder.GetEmail(mailPath);
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Creates Mail message object from stream.
        /// </summary>
        /// <param name="emailStream">email stream.</param>
        /// <returns>Mime decoded email object</returns>
        public static MailMessageParser CreateFromStream(Stream emailStream)
        {
            using (MailMimeReader mimeDecoder = new MailMimeReader())
            {
                return mimeDecoder.GetEmail(emailStream);
            }
        }

        /// <summary>
        /// Creates Mail message object from stream.
        /// </summary>
        /// <param name="mimeDecoder">read MIME object</param>
        /// <param name="emailStream">The email stream.</param>
        /// <returns>Mime decoded email object</returns>
        public static MailMessageParser CreateFromStream(MailMimeReader mimeDecoder, Stream emailStream)
        {
            MailMessageParser result = null;
            if (mimeDecoder != null)
            {
                result = mimeDecoder.GetEmail(emailStream);
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Gets the uploaded email file properties....
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="mailProperties">The mail properties.</param>
        /// <returns>Dictionary string key value pair for mail properties</returns>
        public static Dictionary<string, string> GetMailFileProperties(System.IO.Stream fileStream, Dictionary<string, string> mailProperties)
        {
            if (null != mailProperties && mailProperties.ContainsKey(ServiceConstants.MAIL_FILE_EXTENSION_KEY))
            {
                if (string.Equals(mailProperties[ServiceConstants.MAIL_FILE_EXTENSION_KEY], ServiceConstants.EMAIL_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    MailMimeReader mime = new MailMimeReader();
                    MailMessageParser messageParser = mime.GetEmail(fileStream);
                    string fromDisplayName = Convert.ToString(messageParser.From.DisplayName, CultureInfo.InvariantCulture);
                    mailProperties[ServiceConstants.MAIL_SENDER_KEY] = String.Concat(Convert.ToString(messageParser.From.Address, CultureInfo.InvariantCulture), ServiceConstants.SEMICOLON, fromDisplayName.Replace(Convert.ToString(messageParser.From.Address, CultureInfo.InvariantCulture), string.Empty).Replace(ServiceConstants.OPENING_BRACKET + ServiceConstants.CLOSING_BRACKET, string.Empty));
                    mailProperties[ServiceConstants.MAIL_SEARCH_EMAIL_SUBJECT] = messageParser.Subject;
                    mailProperties[ServiceConstants.MAIL_SENT_DATE_KEY] = Convert.ToString(messageParser.DeliveryDate, CultureInfo.InvariantCulture);
                    mailProperties[ServiceConstants.MAIL_RECEIVED_DATEKEY] = Convert.ToString(messageParser.ReceivedDate, CultureInfo.InvariantCulture);
                    mailProperties[ServiceConstants.MAIL_ORIGINAL_NAME] = messageParser.Subject;
                    StringBuilder mailReceiver = new StringBuilder();
                    StringBuilder mailCCAddress = new StringBuilder();
                    StringBuilder attachmentName = new StringBuilder();

                    foreach (MailAddress toItem in messageParser.To)
                    {
                        string toMailAlias = Convert.ToString(toItem.Address, CultureInfo.InvariantCulture);
                        string toMailName = Convert.ToString(toItem.DisplayName, CultureInfo.InvariantCulture);
                        if (toMailName.Contains(toMailAlias))
                        {
                            toMailName = toMailName.Replace(toMailAlias, string.Empty).Replace(ServiceConstants.OPENING_BRACKET + ServiceConstants.CLOSING_BRACKET, string.Empty);
                        }

                        mailReceiver.Append(toMailAlias + ServiceConstants.SEMICOLON + toMailName + ServiceConstants.SEMICOLON);
                    }

                    mailProperties[ServiceConstants.MAIL_RECEIVER_KEY] = Convert.ToString(mailReceiver, CultureInfo.InvariantCulture);

                    foreach (MailAddress itemCC in messageParser.CC)
                    {
                        string mailCCAlias = Convert.ToString(itemCC.Address, CultureInfo.InvariantCulture);
                        string mailCCName = Convert.ToString(itemCC.DisplayName, CultureInfo.InvariantCulture);
                        if (mailCCName.Contains(mailCCAlias))
                        {
                            mailCCName = mailCCName.Replace(mailCCAlias, string.Empty).Replace(ServiceConstants.OPENING_BRACKET + ServiceConstants.CLOSING_BRACKET, string.Empty);
                        }

                        mailCCAddress.Append(mailCCAlias + ServiceConstants.SEMICOLON + mailCCName + ServiceConstants.SEMICOLON);
                    }

                    mailProperties[ServiceConstants.MAIL_CC_ADDRESS_KEY] = Convert.ToString(mailCCAddress, CultureInfo.InvariantCulture);

                    foreach (System.Net.Mail.Attachment itemAttachment in messageParser.Attachments)
                    {
                        if (!string.IsNullOrWhiteSpace(itemAttachment.Name))
                        {
                            attachmentName.Append(itemAttachment.Name + ServiceConstants.SEMICOLON);
                        }
                    }

                    for (int mailEntitiesCount = 0; mailEntitiesCount < messageParser.Entities.Count; mailEntitiesCount++)
                    {
                        if (string.Equals(messageParser.Entities[mailEntitiesCount].MediaMainType, ServiceConstants.MAIL_ATTACHMENT_MEDIA_MAINT_YPE, StringComparison.CurrentCultureIgnoreCase))
                        {
                            attachmentName.Append(messageParser.Entities[mailEntitiesCount].ContentDescription + ServiceConstants.SEMICOLON);
                        }
                    }

                    mailProperties[ServiceConstants.MAIL_ATTACHMENT_KEY] = Convert.ToString(attachmentName, CultureInfo.InvariantCulture);

                    // Setting email importance
                    mailProperties[ServiceConstants.MAIL_IMPORTANCE_KEY] = (!string.IsNullOrWhiteSpace(messageParser.MailImportance) ? messageParser.MailImportance : ServiceConstants.MAIL_DEFAULT_IMPORTANCE);

                    // Setting email categories
                    mailProperties[ServiceConstants.MAIL_CATEGORIES_KEY] = (!string.IsNullOrWhiteSpace(messageParser.MailCategories) ? messageParser.MailCategories.Replace(ServiceConstants.COMMA, ServiceConstants.SEMICOLON) : string.Empty);
                }
            }

            return mailProperties;
        }

        /// <summary>
        /// Set all content disposition related fields.
        /// </summary>
        /// <param name="headerLineContent">Content of the mail header line.</param>
        public void SetContentDisposition(string headerLineContent)
        {
            if (!string.IsNullOrEmpty(headerLineContent))
            {
                string[] mailParameters = headerLineContent.Split(new string[] { ServiceConstants.SEMICOLON }, StringSplitOptions.RemoveEmptyEntries);
                if (0 == mailParameters.Length)
                {
                    this.ContentDisposition = new ContentDisposition("inline");
                    return;
                }

                this.ContentDisposition = new ContentDisposition(mailParameters[0].Trim());

                for (int mailParametersCount = 1; mailParametersCount < mailParameters.Length; mailParametersCount++)
                {
                    string[] nameValue = mailParameters[mailParametersCount].Split(new string[] { ServiceConstants.OPERATOR_EQUAL }, StringSplitOptions.None);
                    if (2 != nameValue.Length)
                    {
                        continue;
                    }

                    string name = nameValue[0].Trim().ToUpperInvariant();
                    string value = nameValue[1].Trim();
                    value = value.Replace(ServiceConstants.DOUBLE_QUOTE, string.Empty);

                    switch (name)
                    {
                        case ServiceConstants.MailAttributes.FILENAME:
                            this.ContentDisposition.FileName = value;
                            break;
                        case ServiceConstants.MailAttributes.SIZE:
                            this.ContentDisposition.Size = long.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case ServiceConstants.MailAttributes.CREATION_DATE:
                            this.ContentDisposition.CreationDate = DateTime.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case ServiceConstants.MailAttributes.MODIFICATION_DATE:
                            this.ContentDisposition.ModificationDate = DateTime.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case ServiceConstants.MailAttributes.READ_DATE:
                            this.ContentDisposition.ReadDate = DateTime.Parse(value, CultureInfo.InvariantCulture);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the content type fields.
        /// </summary>
        /// <param name="mailContentType">content type string.</param>
        public void SetContentTypeFields(string mailContentType)
        {
            if (!string.IsNullOrEmpty(mailContentType))
            {
                mailContentType = mailContentType.Trim();
                if (null == mailContentType || 1 > mailContentType.Length)
                {
                    this.ContentType = new ContentType(ServiceConstants.MAIL_CONTENT_TYPE);
                }
                else
                {
                    this.ContentType = new ContentType(mailContentType);
                }

                if (null == this.ContentType.CharSet)
                {
                    this.BodyEncoding = Encoding.ASCII;
                }
                else
                {
                    try
                    {
                        this.BodyEncoding = Encoding.GetEncoding(this.ContentType.CharSet);
                    }
                    catch (EncoderFallbackException)
                    {
                        this.BodyEncoding = Encoding.ASCII;
                    }
                    catch
                    {
                        this.BodyEncoding = Encoding.ASCII;
                    }
                }

                if (null == this.ContentType.MediaType || 1 > this.ContentType.MediaType.Length)
                {
                    this.ContentType.MediaType = ServiceConstants.MailMediaType;
                }
                else
                {
                    string mediaTypeString = this.ContentType.MediaType.Trim();
                    int slashPosition = this.ContentType.MediaType.IndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.Ordinal);
                    if (1 > slashPosition)
                    {
                        this.MediaMainType = mediaTypeString;
                        if (string.Equals(this.MediaMainType, ServiceConstants.TEXT_MEDIA_MAIN_TYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            this.MediaSubType = ServiceConstants.MAIL_MEDIA_SUBTYPE;
                        }
                        else
                        {
                            this.MediaSubType = string.Empty;
                        }
                    }
                    else
                    {
                        this.MediaMainType = mediaTypeString.Substring(0, slashPosition);
                        if (mediaTypeString.Length > slashPosition)
                        {
                            this.MediaSubType = mediaTypeString.Substring(slashPosition + 1);
                        }
                        else
                        {
                            if (string.Equals(this.MediaMainType, ServiceConstants.TEXT_MEDIA_MAIN_TYPE, StringComparison.OrdinalIgnoreCase))
                            {
                                this.MediaSubType = ServiceConstants.MAIL_MEDIA_SUBTYPE;
                            }
                            else
                            {
                                this.MediaSubType = string.Empty;
                            }
                        }
                    }
                }

                this.IsBodyHtml = this.MediaSubType == ServiceConstants.HtmlMediaMainType;
            }
        }

        /// <summary>
        /// Creates an empty child MIME entity from the parent MIME entity.        
        /// An email can consist of several MIME entities. A entity has the same structure of email.    
        /// </summary>
        /// <returns>Mail message child object</returns>
        public MailMessageParser CreateChildEntity()
        {
            MailMessageParser child = new MailMessageParser();
            child.Parent = this;
            child.TopParent = this.TopParent;
            child.ContentTransferEncoding = this.ContentTransferEncoding;
            return child;
        }
    }
}
