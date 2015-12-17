// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-akvira
// Created          : 04-01-2014
//
// ***********************************************************************
// <copyright file="ServiceUtility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines common function used by service layer.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using Microsoft.IdentityModel.SecurityTokenService;
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.SharePoint.Client;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    #endregion

    /// <summary>
    /// Provides methods used by the service.
    /// </summary>
    [CLSCompliant(false)]
    public static class ServiceUtility
    {
        /// <summary>
        /// Generates Client Context in case of online and onpremise deployments
        /// </summary>
        /// <param name="contextToken">SharePoint Application Token</param>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="refreshToken">The refresh token</param>
        /// <param name="currentIdentity">Current Identity object</param>
        /// <returns>Client Context</returns>
        private static ClientContext GetCommonContext(string contextToken, Uri siteUrl, string refreshToken, WindowsIdentity currentIdentity)
        {
            bool environment = Convert.ToBoolean(ConstantStrings.IsDeployedOnAzure, CultureInfo.InvariantCulture);
            ClientContext result = null;
            if (environment)
            {
                try
                {
                    string decryptedRefreshToken = string.Empty;
                    string key = ConfigurationManager.AppSettings["Encryption_Key"];

                    if (!string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(key))
                    {
                        decryptedRefreshToken = EncryptionDecryption.Decrypt(refreshToken, key);
                        if (!string.IsNullOrEmpty(decryptedRefreshToken))
                        {
                            result = FetchClientContext(contextToken, siteUrl, decryptedRefreshToken);
                        }
                        else
                        {
                            key = ConfigurationManager.AppSettings["Old_Encryption_Key"];
                            if (!string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(key))
                            {
                                decryptedRefreshToken = EncryptionDecryption.Decrypt(refreshToken, key);
                                if (!string.IsNullOrEmpty(decryptedRefreshToken))
                                {
                                    result = FetchClientContext(contextToken, siteUrl, decryptedRefreshToken);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                    throw;
                }
            }
            else
            {
                if (null != siteUrl)
                {
                    result = TokenHelper.GetS2SClientContextWithWindowsIdentity(siteUrl, currentIdentity);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the client context for the specified site using SP app token using current Windows Identity
        /// </summary>
        /// <param name="contextToken">SharePoint Application Token</param>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="refreshToken">The refresh token</param>
        /// <returns>Client Context</returns>
        public static ClientContext GetClientContext(string contextToken, Uri siteUrl, string refreshToken)
        {
            return (null != siteUrl) ? GetCommonContext(contextToken, siteUrl, refreshToken, OperationContext.Current.ServiceSecurityContext.WindowsIdentity) : null;
        }

        /// <summary>
        /// Gets the client context for the specified site using SP app token using HttpRequest Context
        /// </summary>
        /// <param name="contextToken">SharePoint Application Token</param>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="refreshToken">The refresh token</param>
        /// <param name="request">Current HTTP request object</param>
        /// <returns>Client Context</returns>
        public static ClientContext GetClientContext(string contextToken, Uri siteUrl, string refreshToken, HttpRequest request)
        {
            return (null != request && null != siteUrl) ? GetCommonContext(contextToken, siteUrl, refreshToken, request.LogonUserIdentity) : null;
        }

        /// <summary>
        /// Gets the client context for the refresh token.
        /// </summary>
        /// <param name="contextToken">SharePoint Application Token</param>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="result">The client context for the refresh token</param>
        /// <param name="refreshToken">The refresh token</param>
        /// <returns>The client context for the refresh token</returns>

        private static ClientContext FetchClientContext(string contextToken, Uri siteUrl, string refreshToken)
        {
            ClientContext result = null;
            using (ClientContext fetchedClientContext = AccessClientContext(contextToken, siteUrl, refreshToken))
            {
                try
                {
                    if (fetchedClientContext != null)
                    {
                        result = fetchedClientContext;
                    }
                    else if (null != siteUrl && !string.IsNullOrWhiteSpace(refreshToken))
                    {
                        string accessToken = TokenHelper.GetAccessToken(refreshToken, TokenHelper.SharePointPrincipal, siteUrl.Authority, TokenHelper.GetRealmFromTargetUrl(siteUrl)).AccessToken;
                        result = TokenHelper.GetClientContextWithAccessToken(Convert.ToString(siteUrl, CultureInfo.InvariantCulture), accessToken);
                    }
                }
                catch (Exception exception)
                {


                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets encoded value for search index property.
        /// </summary>
        /// <param name="keys">Key value of the property</param>
        /// <returns>Encoded value</returns>
        public static string GetEncodedValueForSearchIndexProperty(List<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (null != keys && 0 < keys.Count)
            {
                foreach (string current in keys)
                {
                    stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                    stringBuilder.Append(ConstantStrings.Pipe);
                }
            }

            return Convert.ToString(stringBuilder, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="props">Property Bag</param>
        /// <param name="matterName">Name of matter to which property is to be attached</param>
        /// <param name="propertyList">List of properties</param>
        public static void SetPropertyValueForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            if (null != clientContext && !string.IsNullOrWhiteSpace(matterName) && null != props && null != propertyList)
            {
                var list = clientContext.Web.Lists.GetByTitle(matterName);

                foreach (var item in propertyList)
                {
                    props[item.Key] = item.Value;
                    list.Update();
                }

                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Fetches the values of property for specified matter.
        /// </summary>
        /// <param name="context">Client context</param>
        /// <param name="matterName">Name of matter</param>
        /// <param name="propertyList">List of properties</param>
        /// <returns>
        /// Property list stamped to the matter
        /// </returns>
        public static string GetPropertyValueForList(ClientContext context, string matterName, Dictionary<string, string> propertyList)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (context != null && !string.IsNullOrWhiteSpace(matterName))
            {
                var props = context.Web.Lists.GetByTitle(matterName).RootFolder.Properties;
                context.Load(props);
                context.ExecuteQuery();
                if (null != propertyList && null != props)
                {
                    foreach (var item in propertyList)
                    {
                        if (props.FieldValues.ContainsKey(item.Key))
                        {
                            stringBuilder.Append(Convert.ToString(props.FieldValues[item.Key], CultureInfo.InvariantCulture));
                            stringBuilder.Append(ConstantStrings.Pipe);
                        }
                    }
                }
            }

            return Convert.ToString(stringBuilder, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Accesses the client context.
        /// </summary>
        /// <param name="contextToken">The context token for the client context</param>
        /// <param name="siteUrl">The site URL for the client context</param>
        /// <param name="refreshToken">The refresh token for the client context</param>
        /// <returns>Client Context with Access Token</returns>
        public static ClientContext AccessClientContext(string contextToken, Uri siteUrl, string refreshToken)
        {
            ClientContext result = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(contextToken) && null != siteUrl)
                {
                    SharePointContextToken contextTokenData = TokenHelper.ReadAndValidateContextToken(contextToken, null);
                    string accessToken = TokenHelper.GetAccessToken(contextTokenData, siteUrl.Authority).AccessToken;
                    result = TokenHelper.GetClientContextWithAccessToken(Convert.ToString(siteUrl, CultureInfo.InvariantCulture), accessToken);
                }
                else if (null != siteUrl && !string.IsNullOrWhiteSpace(refreshToken))
                {
                    Uri sharePointURL = siteUrl;
                    string accessToken = TokenHelper.GetAccessToken(refreshToken, TokenHelper.SharePointPrincipal, sharePointURL.Authority, TokenHelper.GetRealmFromTargetUrl(sharePointURL)).AccessToken;
                    result = TokenHelper.GetClientContextWithAccessToken(Convert.ToString(sharePointURL, CultureInfo.InvariantCulture), accessToken);
                }
            }
            catch (RequestException requestException)
            {
                if (requestException.Message.ToUpperInvariant().Contains(ConstantStrings.TokenRequestFailedErrorMessage.ToUpperInvariant()))
                {
                    //// Failed with Token Request failure issue hence generate the Matter Center custom exception
                    MatterCenterException tokenRequestException = new MatterCenterException(ConstantStrings.TokenRequestFailedErrorCode, requestException.Message);
                    //// Log the exception with custom error code and default error message
                    Logger.LogError(tokenRequestException, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                    throw tokenRequestException; // Throw will direct to current function's catch block (if present). If not present then it will direct to parent catch block. Parent will be the calling function
                }
                else
                {
                    //// Some other exception occurred
                    Logger.LogError(requestException, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                    throw;
                }
            }
            catch (Exception exception)
            {


                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Sets the upload item properties.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="web">The web.</param>
        /// <param name="documentLibraryName">Name of the document library.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="folderPath">Path of the folder.</param>
        /// <param name="mailProperties">The mail properties.</param>
        public static void SetUploadItemProperties(ClientContext clientContext, Microsoft.SharePoint.Client.Web web, string documentLibraryName, string fileName, string folderPath, Dictionary<string, string> mailProperties)
        {
            ListItemCollection items = null;
            ListItem listItem = null;
            if (null != clientContext && null != web && !string.IsNullOrEmpty(documentLibraryName) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(folderPath) && null != mailProperties)
            {
                ListCollection lists = web.Lists;
                CamlQuery query = new CamlQuery();
                List selectedList = lists.GetByTitle(documentLibraryName);
                string serverRelativePath = folderPath + ConstantStrings.ForwardSlash + fileName;
                query.ViewXml = string.Format(CultureInfo.InvariantCulture, ConstantStrings.GetAllFilesInFolderQuery, serverRelativePath);
                items = selectedList.GetItems(query);
                if (null != items)
                {
                    clientContext.Load(items, item => item.Include(currentItem => currentItem.DisplayName, currentItem => currentItem.File.Name).Where(currentItem => currentItem.File.Name == fileName));
                    clientContext.ExecuteQuery();
                    if (0 < items.Count)
                    {
                        listItem = items[0];
                        if (null != mailProperties)
                        {
                            listItem[ConstantStrings.SearchEmailFrom] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailSenderKey]) ? mailProperties[ConstantStrings.MailSenderKey].Trim() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailReceivedDateKey]))
                            {
                                listItem[ConstantStrings.SearchEmailReceivedDate] = Convert.ToDateTime(mailProperties[ConstantStrings.MailReceivedDateKey].Trim(), CultureInfo.InvariantCulture).ToUniversalTime();
                            }
                            else
                            {
                                listItem[ConstantStrings.SearchEmailReceivedDate] = null;
                            }
                            listItem[ConstantStrings.SearchEmailCC] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailCCAddressKey]) ? mailProperties[ConstantStrings.MailCCAddressKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailAttachments] = (string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailHasAttachmentsKey]) ||mailProperties[ConstantStrings.MailHasAttachmentsKey].Equals(ConstantStrings.TRUE, StringComparison.OrdinalIgnoreCase)) ? mailProperties[ConstantStrings.MailAttachmentKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailFromMailbox] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailSearchEmailFromMailboxKey]) ? mailProperties[ConstantStrings.MailSearchEmailFromMailboxKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailSubject] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailSearchEmailSubject]) ? mailProperties[ConstantStrings.MailSearchEmailSubject].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailTo] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailReceiverKey]) ? mailProperties[ConstantStrings.MailReceiverKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailImportance] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailImportanceKey]) ? mailProperties[ConstantStrings.MailImportanceKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailSensitivity] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailSensitivityKey]) ? mailProperties[ConstantStrings.MailSensitivityKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailHasAttachments] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailHasAttachmentsKey]) ? mailProperties[ConstantStrings.MailHasAttachmentsKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailConversationId] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailConversationIdKey]) ? mailProperties[ConstantStrings.MailConversationIdKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailConversationTopic] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailConversationTopicKey]) ? mailProperties[ConstantStrings.MailConversationTopicKey].Trim() : string.Empty;
                            listItem[ConstantStrings.SearchEmailCategories] = GetCategories(mailProperties[ConstantStrings.MailCategoriesKey].Trim());
                            if (!string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailSentDateKey]))
                            {
                                listItem[ConstantStrings.SearchEmailSentDate] = Convert.ToDateTime(mailProperties[ConstantStrings.MailSentDateKey].Trim(), CultureInfo.InvariantCulture).ToUniversalTime();
                            }
                            else
                            {
                                listItem[ConstantStrings.SearchEmailSentDate] = null;
                            }
                            listItem[ConstantStrings.SearchEmailOriginalName] = !string.IsNullOrWhiteSpace(mailProperties[ConstantStrings.MailOriginalName]) ? mailProperties[ConstantStrings.MailOriginalName] : string.Empty;
                            listItem.Update();
                            clientContext.ExecuteQuery();
                            listItem.RefreshLoad();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if document exists.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="documentLibraryName">Name of the document library.</param>
        /// <param name="folderPath">Path of the folder.</param>
        /// <returns>Returns true in case of Document presence</returns>
        public static DuplicateDocument DocumentExists(ClientContext clientContext, ContentCheckDetails contentCheck, string documentLibraryName, string folderPath, bool isMail)
        {
            DuplicateDocument duplicateDocument = new DuplicateDocument(false, false);
            if (null != clientContext && null != contentCheck && !string.IsNullOrEmpty(documentLibraryName) && !string.IsNullOrEmpty(folderPath))
            {
                string serverRelativePath = folderPath + ConstantStrings.ForwardSlash + contentCheck.FileName;
                ListItemCollection listItemCollection = null;
                string camlQuery = string.Format(CultureInfo.InvariantCulture, ConstantStrings.GetAllFilesInFolderQuery, serverRelativePath);
                listItemCollection = Lists.GetData(clientContext, documentLibraryName, camlQuery);
                duplicateDocument.DocumentExists = (null != listItemCollection && 0 < listItemCollection.Count) ? true : false;

                // Check file size, from, sent date as well.
                if (duplicateDocument.DocumentExists)
                {
                    // check for other conditions as well.
                    ListItem listItem = listItemCollection.FirstOrDefault();
                    DateTime sentDate, storedFileSentDate;
                    long fileSize = Convert.ToInt64(listItem.FieldValues[ConstantStrings.SearchEmailFileSize], CultureInfo.InvariantCulture);
                    if (isMail)
                    {
                        // check for subject, from and sent date
                        string subject = Convert.ToString(listItem.FieldValues[ConstantStrings.SearchEmailSubject], CultureInfo.InvariantCulture);
                        string from = Convert.ToString(listItem.FieldValues[ConstantStrings.SearchEmailFrom], CultureInfo.InvariantCulture);
                        bool isValidDateFormat;
                        isValidDateFormat = DateTime.TryParse(Convert.ToString(listItem.FieldValues[ConstantStrings.SearchEmailSentDate], CultureInfo.InvariantCulture), out storedFileSentDate);
                        isValidDateFormat &= DateTime.TryParse(contentCheck.SentDate, out sentDate);
                        if (isValidDateFormat)
                        {
                            TimeSpan diffrence = sentDate - storedFileSentDate;
                            uint tolleranceMin = Convert.ToUInt16(ConstantStrings.SentDateTolerance, CultureInfo.InvariantCulture);     // up to how much minutes difference between uploaded files is tolerable
                            duplicateDocument.HasPotentialDuplicate = ((fileSize == contentCheck.FileSize) && (subject.Trim() == contentCheck.Subject.Trim()) && (from.Trim() == contentCheck.FromField.Trim()) && (diffrence.Minutes < tolleranceMin));
                        }
                    }
                    else
                    {
                        duplicateDocument.HasPotentialDuplicate = (fileSize == contentCheck.FileSize);
                    }
                }
            }
            return duplicateDocument;
        }

        /// <summary>
        /// Check if Content of local file and server file matches.
        /// </summary>
        /// <param name="context">SP client context</param>
        /// <param name="localMemoryStream">Memory stream of local file</param>
        /// <param name="serverFileURL">Server relative URL of file with filename</param>
        /// <returns>True if content matched else false</returns>
        public static bool PerformContentCheck(ClientContext context, MemoryStream localMemoryStream, String serverFileURL)
        {
            bool isMatched = true;
            if (null != context && null != localMemoryStream && !string.IsNullOrWhiteSpace(serverFileURL))
            {
                Microsoft.SharePoint.Client.File serverFile = context.Web.GetFileByServerRelativeUrl(serverFileURL);
                context.Load(serverFile);
                ClientResult<Stream> serverStream = serverFile.OpenBinaryStream();
                context.ExecuteQuery();
                if (null != serverFile)
                {
                    using (MemoryStream serverMemoryStream = new MemoryStream())
                    {
                        byte[] serverBuffer = new byte[serverFile.Length + 1];

                        int readCount = 0;
                        while ((readCount = serverStream.Value.Read(serverBuffer, 0, serverBuffer.Length)) > 0)
                        {
                            serverMemoryStream.Write(serverBuffer, 0, readCount);
                        }
                        serverMemoryStream.Seek(0, SeekOrigin.Begin);
                        localMemoryStream.Seek(0, SeekOrigin.Begin);
                        if (serverMemoryStream.Length == localMemoryStream.Length)
                        {
                            byte[] localBuffer = localMemoryStream.GetBuffer();
                            serverBuffer = serverMemoryStream.GetBuffer();
                            for (long index = 0; index < serverMemoryStream.Length; index++)
                            {
                                if (localBuffer[index] != serverBuffer[index])
                                {
                                    isMatched = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isMatched = false;
                        }
                    }
                }
                else
                {
                    isMatched = false;
                }
            }
            return isMatched;
        }

        /// <summary>
        /// Checks if request token is valid or not.
        /// </summary>
        /// <param name="requestValidationTokens"> Request Token to be validate</param>
        /// <returns> Returns true if request token is valid</returns>
        public static bool ValidateRequestToken(string requestValidationTokens)
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(requestValidationTokens))
            {
                string[] token = requestValidationTokens.Split(new char[] { Convert.ToChar(ConstantStrings.COLON, CultureInfo.InvariantCulture) }, StringSplitOptions.RemoveEmptyEntries);
                if (null != token && 2 == token.Length && token[0].Equals(token[1]))
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Remove Escape character
        /// </summary>
        /// <param name="message">The input message to remove escape character</param>
        /// <returns> Escape character removed message</returns>
        public static string RemoveEscapeCharacter(string message)
        {
            // Removed escape character which will avoid breaking of JSON Parse on UI layer
            return Regex.Replace(message, ConstantStrings.EscapeCharacterPattern, string.Empty);
        }

       /// <summary>
        /// Process the categories and trims the "category" word
        /// </summary>
        /// <param name="categories">Categories property</param>
        /// <returns>Processed category field</returns>
        internal static string GetCategories(string categories)
        {
            string processedCategories = string.Empty;
            if (!string.IsNullOrWhiteSpace(categories))
            {
                processedCategories = Regex.Replace(categories, ConstantStrings.categories, string.Empty, RegexOptions.IgnoreCase).Trim(); // Replace categories with empty strings
                processedCategories = processedCategories.Replace(ConstantStrings.Space, string.Empty); // Remove the space generated because of replace operation
                processedCategories = processedCategories.Replace(ConstantStrings.Semicolon, string.Concat(ConstantStrings.Semicolon, ConstantStrings.Space));
            }
            return processedCategories;
        }

        /// <summary>
        /// Checks if the user is a Site Admin
        /// </summary>
        /// <param name="refreshToken">The refresh token for Client Context</param>
        /// <param name="clientUrl">The client URL for Client Context</param>
        /// <returns>User present in Site Owner group</returns>
        public static bool GetUserGroup(string refreshToken, Uri clientUrl, HttpRequest request)
        {
            bool returnValue = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(refreshToken) && null != clientUrl)
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(null, clientUrl, refreshToken, request))
                    {
                        UserCollection siteOwners = clientContext.Web.AssociatedOwnerGroup.Users;
                        clientContext.Load(siteOwners, owners => owners.Include(owner => owner.Title));
                        clientContext.Load(clientContext.Web.CurrentUser);
                        clientContext.ExecuteQuery();
                        returnValue = siteOwners.Any(owners => owners.Title.Equals(clientContext.Web.CurrentUser.Title));
                    }
                }
            }
            catch (Exception exception)
            {
                returnValue = false;
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return returnValue;
        }

        /// <summary>
        /// Get/Set data in cache
        /// </summary>
        /// <param name="key">Key to be stored in the cache</param>
        /// <param name="value">Value of the key</param>
        /// <returns>Cached data else returns error</returns>
        public static string GetOrSetCachedValue(string key, string value = null)
        {
            string result = ConstantStrings.FALSE, hostname = ConfigurationManager.AppSettings[ConstantStrings.APP_SETTINGS_CACHE_HOSTNAME_KEY], cacheKey = ConfigurationManager.AppSettings[ConstantStrings.APP_SETTINGS_CACHE_PRIMARY_KEY];
            try
            {
                if (!string.IsNullOrWhiteSpace(hostname) && !string.IsNullOrWhiteSpace(cacheKey) && !string.IsNullOrWhiteSpace(key))
                {
                    using (ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(string.Format(CultureInfo.InvariantCulture, ConstantStrings.RedisCacheConnectionString, hostname, cacheKey)))
                    {
                        IDatabase cacheDatabase = connection.GetDatabase();
                        if (null == value)
                        {
                            result = cacheDatabase.StringGet(key);
                            if (null == result)
                            {
                                result = ConstantStrings.FALSE;
                            }
                        }
                        else
                        {
                            cacheDatabase.StringSet(key, value, TimeSpan.FromDays(1));
                            result = ConstantStrings.TRUE;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                result = ConstantStrings.FALSE;
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Check if retrieved value has errors
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>True or false</returns>
        public static bool CheckValueHasErrors(string value)
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(value) || value.Equals(ConstantStrings.FALSE) || (value.Contains(ConstantStrings.SERVICE_RESPONSE_CODE_PART) && value.Contains(ConstantStrings.SERVICE_RESPONSE_VALUE_PART)))
            {
                result = true;
            }
            return result;
        }
    }
}
