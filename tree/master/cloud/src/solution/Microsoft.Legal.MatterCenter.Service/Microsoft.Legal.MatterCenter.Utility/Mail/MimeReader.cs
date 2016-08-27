// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-swmirj
// Created          : 07-02-2014
//
// ***********************************************************************
// <copyright file="MimeReader.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file reads MIME based on streams and files.</summary>
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
    using System.Text.RegularExpressions;
    #endregion

    /// <summary>
    /// Reads MIME based emails streams and files.
    /// </summary>
    public class MailMimeReader : IDisposable
    {
        /// <summary>
        /// The bracket chars
        /// </summary>
        private static char[] bracketChars = { '(', ')' };

        /// <summary>
        /// The comma chars
        /// </summary>
        private static char[] commaChars = { ',' };

        /// <summary>
        /// The white space chars
        /// </summary>
        private static char[] whiteSpaceChars = { ' ', '\t' };

        /// <summary>
        /// Collects all unknown header lines for all important emails received
        /// </summary>
        private static bool isCollectHiddenHeaderlines = true;

        /// <summary>
        /// list of all unknown header lines received, for all important emails 
        /// </summary>
        private static List<string> allHiddenHeaderLines = new List<string>();

        /// <summary>
        /// buffer used by every ProcessMimeEntity() to store  MIME entity
        /// </summary>
        private StringBuilder mimeEntitySB = new StringBuilder(100000);

        /// <summary>
        /// Reader for Email streams
        /// </summary>
        private StreamReader emailStreamReader;

        /// <summary>
        /// char 'array' for carriage return / line feed
        /// </summary>
        private string crlf = "\r\n";

        /// <summary>
        /// Format culture provider
        /// </summary>
        private IFormatProvider culture = new CultureInfo("en-US", true);

        /// <summary>
        /// Initializes a new instance of the MailMimeReader class
        /// </summary>
        public MailMimeReader()
        {
        }

        /// <summary>
        /// indicates the reason how a MIME entity processing has terminated
        /// </summary>
        private enum MimeEntityReturnCode
        {
            /// <summary>
            /// Message Entity undefined or null
            /// </summary>
            undefined = 0,

            /// <summary>
            /// end of message line found
            /// </summary>
            bodyComplete,

            /// <summary>
            /// parent boundary start found
            /// </summary>
            parentBoundaryStartFound,

            /// <summary>
            /// parent boundary end found
            /// </summary>
            parentBoundaryEndFound,

            /// <summary>
            /// received message doesn't follow MIME specification
            /// </summary>
            problem
        }

        /// <summary>
        /// Gets a value indicating whether this instance is to collect unknown header lines.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collect unknown header lines otherwise, <c>false</c>.
        /// </value>
        public static bool IsCollectHiddenHeaderLines
        {
            get
            {
                return isCollectHiddenHeaderlines;
            }
        }

        /// <summary>
        /// Gets all hidden header lines.
        /// </summary>
        /// <value>
        /// All hidden header lines.
        /// </value>
        public static List<string> AllHiddenHeaderLines
        {
            get
            {
                return allHiddenHeaderLines;
            }
        }

        /// <summary>
        /// Gets or sets the email stream reader.
        /// </summary>
        /// <value>
        /// The email stream reader.
        /// </value>
        protected StreamReader EmailStreamReader
        {
            get
            {
                return this.emailStreamReader;
            }

            set
            {
                this.emailStreamReader = value;
            }
        }

        /// <summary>
        /// Gets the CRLF.
        /// </summary>
        /// <value>
        /// The CRLF.
        /// </value>
        protected string CRLF
        {
            get
            {
                return this.crlf;
            }
        }

        /// <summary>
        /// Converts byte array to string, using decoding as requested
        /// </summary>
        /// <param name="byteArray">Stream byte array.</param>
        /// <param name="byteEncoding">byte encoding to use</param>
        /// <returns>Decoded Byte Array</returns>
        public static string DecodeByteArrayToString(byte[] byteArray, Encoding byteEncoding)
        {
            if (null == byteArray)
            {
                ////no bytes to convert
                return null;
            }

            Decoder byteArryDecoder;
            if (null == byteEncoding)
            {
                byteArryDecoder = Encoding.UTF7.GetDecoder();
            }
            else
            {
                byteArryDecoder = byteEncoding.GetDecoder();
            }

            int charCount = byteArryDecoder.GetCharCount(byteArray, 0, byteArray.Length);
            char[] bodyChars = new char[charCount];
            return new string(bodyChars);
        }

        /// <summary>
        /// Tries to convert a string into an email address
        /// </summary>
        /// <param name="address">string mail address to parse</param>
        /// <returns>Parsed Email Address</returns>
        public static MailAddress ConvertToMailAddress(string address)
        {
            MailAddress result = null;
            if (!string.IsNullOrEmpty(address))
            {
                address = address.Replace(ServiceConstants.DOUBLE_QUOTE, string.Empty);
                address = address.Trim();
                result = new MailAddress(address);
                if (ServiceConstants.OPENING_ANGULAR_BRACKET + ServiceConstants.CLOSING_ANGULAR_BRACKET == address)
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets an email from the supplied Email Stream and processes it.
        /// </summary>
        /// <param name="mailPath">string that designates the path to a .EML file</param>
        /// <returns>MailMessageParser or null if email not properly formatted</returns>
        public MailMessageParser GetEmail(string mailPath)
        {
            MailMessageParser messageParser = null;
            using (Stream emailStream = File.Open(mailPath, FileMode.Open))
            {
                messageParser = this.GetEmail(emailStream);
            }

            return messageParser;
        }

        /// <summary>
        /// Gets an email from the supplied Email Stream and processes it.
        /// </summary>
        /// <param name="emailStream">The email stream object</param>
        /// <returns>
        /// MailMessageParser or null if email not properly formatted
        /// </returns>
        public MailMessageParser GetEmail(Stream emailStream)
        {
            this.EmailStreamReader = new StreamReader(emailStream, Encoding.ASCII);

            ////prepare message, set defaults as specified in RFC 2046
            MailMessageParser message = new MailMessageParser();
            message.ContentTransferEncoding = TransferEncoding.SevenBit;
            MailMessageParser result = null;
            MimeEntityReturnCode messageMimeReturnCode = this.ProcessMimeEntity(message, string.Empty);

            if (messageMimeReturnCode == MimeEntityReturnCode.bodyComplete || messageMimeReturnCode == MimeEntityReturnCode.parentBoundaryEndFound)
            {
                if (0 == message.To.Count)
                {
                    string toField = message.Headers[ServiceConstants.Mail_Message_Receiver_Header];
                    if (!string.IsNullOrEmpty(toField))
                    {
                        message.To.Add(toField);
                    }
                }

                if (null == message.From)
                {
                    string mailFrom = message.Headers[ServiceConstants.Mail_Message_Sender_Header];
                    if (!string.IsNullOrEmpty(mailFrom))
                    {
                        message.From = new MailAddress(mailFrom);
                    }
                }

                result = message;
            }

            return result;
        }

        /// <summary>
        /// Tries to convert string to date
        /// If there is a run time error, the smallest possible date is returned
        /// </summary>
        /// <param name="date">string to be converted to Date object</param>
        /// <returns>Converted Date object</returns>
        public DateTime ConvertToDateTime(string date)
        {
            DateTime returnDateTime = DateTime.MinValue;
            DateTime result = DateTime.MinValue;
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    ////remove day of week
                    string cleanDateTime = date;
                    string[] dateSplit = cleanDateTime.Split(commaChars, 2);
                    if (1 < dateSplit.Length)
                    {
                        cleanDateTime = dateSplit[1];
                    }

                    ////remove time zone (PST)
                    dateSplit = cleanDateTime.Split(bracketChars);
                    if (1 < dateSplit.Length)
                    {
                        cleanDateTime = dateSplit[0];
                    }

                    ////convert to DateTime
                    if (!DateTime.TryParse(
                      cleanDateTime,
                      this.culture,
                      DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces,
                      out returnDateTime))
                    {
                        ////try just to convert the date
                        int dateLength = cleanDateTime.IndexOf(ServiceConstants.COLON, StringComparison.CurrentCulture) - 3;
                        cleanDateTime = cleanDateTime.Substring(0, dateLength);

                        if (DateTime.TryParse(
                          cleanDateTime,
                          this.culture,
                          DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces,
                          out returnDateTime))
                        {
                            result = returnDateTime;
                        }
                        else
                        {
                            result = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        result = returnDateTime;
                    }
                }
            }
            catch (ArgumentNullException)
            {
                result = DateTime.MinValue;
            }
            catch
            {
                result = DateTime.MinValue;
            }

            return result;
        }

        /// <summary>
        /// To process received date from the header field
        /// </summary>
        /// <param name="headerValue">Header value from the headers of the mail</param>
        /// <returns>Received date field</returns>
        public DateTime ProcessReceivedDate(string headerValue)
        {
            DateTime result = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                string datePortion = headerValue.Substring(headerValue.LastIndexOf(ServiceConstants.SEMICOLON, StringComparison.OrdinalIgnoreCase) + 1);
                result = this.ConvertToDateTime(datePortion);
            }
            return result;
        }

        /// <summary>
        /// read one line in multiline mode from the Email stream. 
        /// </summary>
        /// <param name="response">line received</param>
        /// <returns>false: end of message</returns>
        /// <returns></returns>
        protected bool ReadMultipleLine(out string response)
        {
            bool result = true;
            response = null;
            response = this.EmailStreamReader.ReadLine();
            if (null == response)
            {
                result = false;
            }
            else if (0 < response.Length && ServiceConstants.PERIOD == Convert.ToString(response[0], CultureInfo.InvariantCulture))
            {
                if (ServiceConstants.PERIOD == response)
                {
                    result = false;
                }
                ////remove the first '.'
                response = response.Substring(1, response.Length - 1);
            }

            return result;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (null != this.EmailStreamReader)
                {
                    this.EmailStreamReader.Dispose();
                    this.EmailStreamReader = null;
                }
            }
        }

        /// <summary>
        /// Check if the response line received is a parent boundary
        /// </summary>
        /// <param name="response">mail response line</param>
        /// <param name="parentBoundaryStart"> parent boundary start identifier</param>
        /// <param name="parentBoundaryEnd">parent boundary end identifier</param>
        /// <param name="boundaryMimeReturnCode">boundary MIME return code.</param>
        /// <returns>
        /// If parent boundary found then true or false
        /// </returns>
        private static bool ParentBoundaryFound(string response, string parentBoundaryStart, string parentBoundaryEnd, out MimeEntityReturnCode boundaryMimeReturnCode)
        {
            bool result = false;
            boundaryMimeReturnCode = MimeEntityReturnCode.undefined;
            if (null == response || 2 > response.Length || '-' != response[0] || '-' != response[1])
            {
                result = false;
            }

            else if (response == parentBoundaryStart)
            {
                boundaryMimeReturnCode = MimeEntityReturnCode.parentBoundaryStartFound;
                result = true;
            }
            else if (response == parentBoundaryEnd)
            {
                boundaryMimeReturnCode = MimeEntityReturnCode.parentBoundaryEndFound;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// find individual addresses in the string and add it to address collection
        /// </summary>
        /// <param name="addresses">string with possibly several email addresses</param>
        /// <param name="addressCollection">parsed addresses</param>
        private static void AddMailAddresses(string addresses, MailAddressCollection addressCollection)
        {
            MailAddress mailAddressField;

            Regex regexObj = new Regex(ServiceConstants.MAIL_ADDRESS_FIELD_REGEX);
            MatchCollection addressMatch = regexObj.Matches(addresses);
            foreach (Match match in addressMatch)
            {
                string quotedString = match.Value.Replace(ServiceConstants.COMMA[0], (char)3);
                addresses = addresses.Replace(match.Value, quotedString);
            }

            string[] addressSplit = addresses.Split(ServiceConstants.COMMA[0]);
            foreach (string address in addressSplit)
            {
                // be sure to add the comma back if it was replaced
                mailAddressField = ConvertToMailAddress(address.Replace((char)3, ServiceConstants.COMMA[0]));
                if (null != mailAddressField)
                {
                    addressCollection.Add(mailAddressField);
                }
            }
        }

        /// <summary>
        /// converts TransferEncoding as defined in the RFC into a .NET TransferEncoding
        /// .NET doesn't know the type "bit8". It is translated here into "bit7", which
        /// requires the same kind of processing (none).
        /// </summary>
        /// <param name="transferEncodingString">string to be converted into TransferEncoding</param>
        /// <returns>TransferEncoded string</returns>
        private static TransferEncoding ConvertToTransferEncoding(string transferEncodingString)
        {
            TransferEncoding result = TransferEncoding.Unknown;
            switch (transferEncodingString.Trim().ToUpperInvariant())
            {
                case ServiceConstants.MailAttributes.BIT_7:
                case ServiceConstants.MailAttributes.BIT_8:
                    result = TransferEncoding.SevenBit;
                    break;
                case ServiceConstants.MailAttributes.QUOTED_PRINTABLE:
                    result = TransferEncoding.QuotedPrintable;
                    break;
                case ServiceConstants.MailAttributes.BASE64:
                    result = TransferEncoding.Base64;
                    break;
                default:
                    result = TransferEncoding.Unknown;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Copies the content found for the MIME entity to the MailMessageParser body and creates
        /// a stream which can be used to create attachments and alternative views.
        /// </summary>
        /// <param name="message">Parsed mail message</param>
        /// <param name="contentString">mail content string</param>
        private static void SaveMessageBody(MailMessageParser message, string contentString)
        {
            message.Body = contentString;
            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            MemoryStream bodyStream = new MemoryStream(ascii.GetBytes(contentString), 0, contentString.Length);
            message.ContentStream = bodyStream;
        }

        /// <summary>
        /// each attachment is stored in its own MIME entity and read into this entity's
        /// ContentStream. SaveAttachment creates an attachment out of the ContentStream
        /// and attaches it to the parent MIME entity.
        /// </summary>
        /// <param name="message">Parsed mail message</param>
        private static void SaveAttachment(MailMessageParser message)
        {
            if (null != message.Parent)
            {
                Attachment thisAttachment = new Attachment(message.ContentStream, message.ContentType);

                if (null != message.ContentDisposition)
                {
                    ContentDisposition messageContentDisposition = message.ContentDisposition;
                    ContentDisposition attachmentContentDisposition = thisAttachment.ContentDisposition;
                    if (messageContentDisposition.CreationDate > DateTime.MinValue)
                    {
                        attachmentContentDisposition.CreationDate = messageContentDisposition.CreationDate;
                    }

                    attachmentContentDisposition.DispositionType = messageContentDisposition.DispositionType;
                    attachmentContentDisposition.FileName = messageContentDisposition.FileName;
                    attachmentContentDisposition.Inline = messageContentDisposition.Inline;
                    if (messageContentDisposition.ModificationDate > DateTime.MinValue)
                    {
                        attachmentContentDisposition.ModificationDate = messageContentDisposition.ModificationDate;
                    }

                    if (messageContentDisposition.ReadDate > DateTime.MinValue)
                    {
                        attachmentContentDisposition.ReadDate = messageContentDisposition.ReadDate;
                    }

                    if (0 < messageContentDisposition.Size)
                    {
                        attachmentContentDisposition.Size = messageContentDisposition.Size;
                    }
                }

                string contentIdString = message.ContentId;
                if (null != contentIdString)
                {
                    thisAttachment.ContentId = RemoveBrackets(contentIdString);
                }

                thisAttachment.TransferEncoding = message.ContentTransferEncoding;
                message.Parent.Attachments.Add(thisAttachment);
            }
        }

        /// <summary>
        /// removes leading '&lt;' and trailing '&gt;' if both exist
        /// </summary>
        /// <param name="parameterString">String to be validated</param>
        /// <returns>String with leading '&lt;' and trailing '&gt;' characters removed</returns>
        private static string RemoveBrackets(string parameterString)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(parameterString))
            {
                result = null;
            }
            else if ('<' != parameterString[0] || '>' != parameterString[parameterString.Length - 1])
            {
                result = parameterString;
            }
            else
            {
                result = parameterString.Substring(1, parameterString.Length - 2);
            }
            return result;
        }

        /// <summary>
        /// Add all attachments and alternative views from child to the parent
        /// </summary>
        /// <param name="child">child Object</param>
        /// <param name="parent">parent Object</param>
        private static void AddChildPartsToParent(MailMessageParser child, MailMessageParser parent)
        {
            ////add the child itself to the parent
            parent.Entities.Add(child);

            ////add the alternative views of the child to the parent
            if (null != child.AlternateViews)
            {
                foreach (AlternateView childView in child.AlternateViews)
                {
                    parent.AlternateViews.Add(childView);
                }
            }

            ////add the body of the child as alternative view to parent
            ////this should be the last view attached here, because the POP 3 MIME client
            ////is supposed to display the last alternative view
            if (child.MediaMainType == ServiceConstants.TEXT_MEDIA_MAIN_TYPE && null != child.ContentStream && null != child.Parent.ContentType && child.Parent.ContentType.MediaType.ToUpperInvariant() == ServiceConstants.MULTI_PART_MEDIA_TYPE)
            {
                AlternateView thisAlternateView = new AlternateView(child.ContentStream);
                thisAlternateView.ContentId = RemoveBrackets(child.ContentId);
                thisAlternateView.ContentType = child.ContentType;
                thisAlternateView.TransferEncoding = child.ContentTransferEncoding;
                parent.AlternateViews.Add(thisAlternateView);
            }

            ////add the attachments of the child to the parent
            if (null != child.Attachments)
            {
                foreach (Attachment childAttachment in child.Attachments)
                {
                    parent.Attachments.Add(childAttachment);
                }
            }
        }

        /// <summary>
        /// Processes the delimited body of Mail Mime Entity.
        /// </summary>
        /// <param name="message">Parsed Mail Message.</param>
        /// <param name="boundaryStart">boundary start identifier</param>
        /// <param name="parentBoundaryStart">parent boundary start identifier</param>
        /// <param name="parentBoundaryEnd">parent boundary end identifier</param>
        /// <returns>MimeEntity process result code</returns>
        private MimeEntityReturnCode ProcessDelimitedBody(MailMessageParser message, string boundaryStart, string parentBoundaryStart, string parentBoundaryEnd)
        {
            string response;

            if (boundaryStart.Trim() == parentBoundaryStart.Trim())
            {
                while (this.ReadMultipleLine(out response))
                {
                    continue;
                }

                return MimeEntityReturnCode.problem;
            }

            MimeEntityReturnCode returnCode;
            do
            {
                this.mimeEntitySB.Length = 0;
                MailMessageParser childPart = message.CreateChildEntity();

                ////recursively call MIME part processing
                returnCode = this.ProcessMimeEntity(childPart, boundaryStart);

                if (returnCode == MimeEntityReturnCode.problem)
                {
                    return MimeEntityReturnCode.problem;
                }

                ////add the newly found child MIME part to the parent
                AddChildPartsToParent(childPart, message);
            }
            while (returnCode != MimeEntityReturnCode.parentBoundaryEndFound);

            MimeEntityReturnCode boundaryMimeReturnCode;
            bool hasParentBoundary = parentBoundaryStart.Length > 0;
            while (this.ReadMultipleLine(out response))
            {
                if (hasParentBoundary && ParentBoundaryFound(response, parentBoundaryStart, parentBoundaryEnd, out boundaryMimeReturnCode))
                {
                    return boundaryMimeReturnCode;
                }
            }

            return MimeEntityReturnCode.bodyComplete;
        }

        /// <summary>
        /// Process a MIME entity
        /// A MIME entity consists of header and body.
        /// Separator lines in the body might mark children MIME entities
        /// </summary>
        /// <param name="message">Mail Message.</param>
        /// <param name="parentBoundaryStart">The parent boundary start value</param>
        /// <returns>Mime entity return code for parsed mail message</returns>
        private MimeEntityReturnCode ProcessMimeEntity(MailMessageParser message, string parentBoundaryStart)
        {
            bool hasParentBoundary = parentBoundaryStart.Length > 0;
            string parentBoundaryEnd = parentBoundaryStart + ServiceConstants.HYPHEN + ServiceConstants.HYPHEN;
            MimeEntityReturnCode boundaryMimeReturnCode;

            ////some format fields are inherited from parent, only the default for
            ////ContentType needs to be set here, otherwise the boundary parameter would be
            ////inherited as well
            message.SetContentTypeFields(ServiceConstants.MAIL_CONTENT_TYPE);
            string completeHeaderField = null;     ////consists of one start line and possibly several continuation lines
            string response;

            //// read header lines until empty line is found (end of header)
            while (true)
            {
                if (!this.ReadMultipleLine(out response))
                {
                    while (this.ReadMultipleLine(out response))
                    {
                        continue;
                    }

                    return MimeEntityReturnCode.problem;
                }

                if (1 > response.Length)
                {
                    ////empty line found => end of header
                    if (completeHeaderField != null)
                    {
                        this.ProcessHeaderField(message, completeHeaderField);
                    }

                    break;
                }

                if (hasParentBoundary && ParentBoundaryFound(response, parentBoundaryStart, parentBoundaryEnd, out boundaryMimeReturnCode))
                {
                    while (this.ReadMultipleLine(out response))
                    {
                        continue;
                    }

                    return boundaryMimeReturnCode;
                }
                ////read header field
                ////one header field can extend over one start line and multiple continuation lines
                ////a continuation line starts with at least 1 blank (' ') or tab
                if (ServiceConstants.SPACE == Convert.ToString(response[0], CultureInfo.InvariantCulture) || ServiceConstants.HORIZONTAL_TAB == Convert.ToString(response[0], CultureInfo.InvariantCulture))
                {
                    if (completeHeaderField == null)
                    {
                        while (this.ReadMultipleLine(out response))
                        {
                            continue;
                        }

                        return MimeEntityReturnCode.problem;
                    }
                    else
                    {
                        if (ServiceConstants.SPACE != Convert.ToString(completeHeaderField[completeHeaderField.Length - 1], CultureInfo.InvariantCulture))
                        {
                            completeHeaderField += ServiceConstants.SPACE + response.TrimStart(whiteSpaceChars);
                        }
                        else
                        {
                            completeHeaderField += response.TrimStart(whiteSpaceChars);
                        }
                    }
                }
                else
                {
                    if (null == completeHeaderField)
                    {
                        completeHeaderField = response;
                    }
                    else
                    {
                        this.ProcessHeaderField(message, completeHeaderField);
                        completeHeaderField = response;
                    }
                }
            }

            this.mimeEntitySB.Length = 0;
            string boundaryDelimiterLineStart = null;
            bool isBoundaryDefined = false;
            if (null != message.ContentType.Boundary)
            {
                isBoundaryDefined = true;
                boundaryDelimiterLineStart = "--" + message.ContentType.Boundary;
            }
            ////prepare return code for the case there is no boundary in the body
            boundaryMimeReturnCode = MimeEntityReturnCode.bodyComplete;

            ////read body lines
            while (this.ReadMultipleLine(out response))
            {
                ////check if there is a boundary line from this entity itself in the body
                if (isBoundaryDefined && response.TrimEnd() == boundaryDelimiterLineStart)
                {
                    ////boundary line found.
                    ////stop the processing here and start a delimited body processing
                    return this.ProcessDelimitedBody(message, boundaryDelimiterLineStart, parentBoundaryStart, parentBoundaryEnd);
                }

                ////check if there is a parent boundary in the body
                if (hasParentBoundary &&
                  ParentBoundaryFound(response, parentBoundaryStart, parentBoundaryEnd, out boundaryMimeReturnCode))
                {
                    ////a parent boundary is found. Decode the content of the body received so far, then end this MIME entity
                    ////note that boundaryMimeReturnCode is set here, but used in the return statement
                    break;
                }

                ////process next line
                this.mimeEntitySB.Append(response + this.CRLF);
            }

            ////a complete MIME body read
            ////convert received US ASCII characters to .NET string (Unicode)
            string transferEncodedMessage = Convert.ToString(this.mimeEntitySB, CultureInfo.InvariantCulture);
            bool isAttachmentSaved = false;
            switch (message.ContentTransferEncoding)
            {
                case TransferEncoding.SevenBit:
                    SaveMessageBody(message, transferEncodedMessage);
                    break;

                case TransferEncoding.Base64:
                    byte[] bodyBytes = System.Convert.FromBase64String(transferEncodedMessage);
                    message.ContentStream = new MemoryStream(bodyBytes, false);

                    if (message.MediaMainType == ServiceConstants.TEXT_MEDIA_MAIN_TYPE)
                    {
                        message.Body = DecodeByteArrayToString(bodyBytes, message.BodyEncoding);
                    }
                    else if (message.MediaMainType == ServiceConstants.IMAGE_MEDIA_MAIN_TYPE || message.MediaMainType == ServiceConstants.APPLICATION_MEDIA_MAIN_TYPE || message.MediaMainType == ServiceConstants.MESSAGE_MEDIA_MAIN_TYPE)
                    {
                        SaveAttachment(message);
                        isAttachmentSaved = true;
                    }

                    break;

                case TransferEncoding.QuotedPrintable:
                    SaveMessageBody(message, QuotedPrintable.Decode(transferEncodedMessage));
                    break;

                default:
                    SaveMessageBody(message, transferEncodedMessage);
                    break;
            }

            if (null != message.ContentDisposition && message.ContentDisposition.DispositionType.ToUpperInvariant() == ServiceConstants.MailAttributes.ATTACHMENT && !isAttachmentSaved)
            {
                SaveAttachment(message);
                isAttachmentSaved = true;
            }

            return boundaryMimeReturnCode;
        }

        /// <summary>
        /// Convert one MIME header field and update message accordingly
        /// </summary>
        /// <param name="message">Parsed message object</param>
        /// <param name="headerField">header field</param>
        private void ProcessHeaderField(MailMessageParser message, string headerField)
        {
            string headerLineType;
            string headerLineContent;
            int separatorPosition = headerField.IndexOf(ServiceConstants.COLON, StringComparison.CurrentCulture);
            if (0 < separatorPosition)
            {
                ////process header field type
                headerLineType = headerField.Substring(0, separatorPosition).ToUpperInvariant();
                headerLineContent = headerField.Substring(separatorPosition + 1).Trim(whiteSpaceChars);
                if (string.IsNullOrEmpty(headerLineType) || string.IsNullOrEmpty(headerLineContent))
                {
                    ////mail header parts missing, exist function
                    return;
                }
                //// add header line to headers
                message.Headers.Add(headerLineType, headerLineContent);

                switch (headerLineType)
                {
                    case ServiceConstants.MailAttributes.BCC:
                        AddMailAddresses(headerLineContent, message.Bcc);
                        break;
                    case ServiceConstants.MailAttributes.CC:
                        AddMailAddresses(headerLineContent, message.CC);
                        break;
                    case ServiceConstants.MailAttributes.CONTENT_DESCRIPTION:
                        message.ContentDescription = headerLineContent;
                        break;
                    case ServiceConstants.MailAttributes.CONTENT_DISPOSITION:
                        message.SetContentDisposition(headerLineContent);
                        break;
                    case ServiceConstants.MailAttributes.CONTENT_ID:
                        message.ContentId = headerLineContent;
                        break;
                    case ServiceConstants.MailAttributes.CONTENT_TRANSFER_ENCODING:
                        message.ContentTransferEncoding = ConvertToTransferEncoding(headerLineContent);
                        break;
                    case ServiceConstants.MailAttributes.CONTENT_TYPE:
                        message.SetContentTypeFields(headerLineContent);
                        break;
                    case ServiceConstants.MailAttributes.DATE:
                        message.DeliveryDate = this.ConvertToDateTime(headerLineContent);
                        break;
                    case ServiceConstants.MailAttributes.FROM:
                        MailAddress address = ConvertToMailAddress(headerLineContent);
                        if (null != address)
                        {
                            message.From = address;
                        }
                        break;
                    case ServiceConstants.MailAttributes.SENDER:
                        message.Sender = ConvertToMailAddress(headerLineContent);
                        break;
                    case ServiceConstants.MailAttributes.SUBJECT:
                        message.Subject = headerLineContent;
                        break;
                    case ServiceConstants.MailAttributes.TO:
                        AddMailAddresses(headerLineContent, message.To);
                        break;
                    case ServiceConstants.MailAttributes.IMPORTANCE:
                        message.MailImportance = headerLineContent;
                        break;
                    case ServiceConstants.MailAttributes.CATEGORIES:
                        message.MailCategories = headerLineContent;
                        break;
                    case ServiceConstants.MailAttributes.RECEIVED:
                        if (message.ReceivedDate.Year.Equals(1))
                        {
                            message.ReceivedDate = this.ProcessReceivedDate(headerLineContent);
                        }
                        break;
                    default:
                        message.UnknowHeaderlines.Add(headerField);
                        if (IsCollectHiddenHeaderLines)
                        {
                            AllHiddenHeaderLines.Add(headerField);
                        }

                        break;
                }
            }
        }
    }
}
