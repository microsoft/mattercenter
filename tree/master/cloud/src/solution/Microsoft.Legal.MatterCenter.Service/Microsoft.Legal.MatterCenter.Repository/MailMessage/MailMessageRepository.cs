// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Repository
// Author           : v-lapedd
// Created          : 04-04-2017
//
// ***********************************************************************
// <copyright file="DocumentRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// This class deals with all the email  related functions such as finding email, attachments of loggedin user
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    ///  This class deals with all the email  related functions such as finding email, attachments of loggedin user
    /// </summary>
    public class MailMessageRepository : IMailMessageRepository
    {
        private ISPOAuthorization spoAuthorization;
        private GeneralSettings generalSettings;
        private IDocumentRepository documentRepository;

        public MailMessageRepository(ISPOAuthorization spoAuthorization, 
            IOptions<GeneralSettings> generalSettings, IDocumentRepository documentRepository)
        {
            this.spoAuthorization = spoAuthorization;
            this.generalSettings = generalSettings.Value;
            this.documentRepository = documentRepository;
        }

        /// <summary>
        /// gets all the logged in user inbox emails. 
        /// </summary>
        /// <returns></returns>
        public MailMessageList GetUserInboxEmails(MailRequest mailRequest = null)
        {          
            var userEmailMessages = ListMessages(mailRequest);  
            return userEmailMessages;
        }

        /// <summary>
        /// gets attachment content for the passed in mailid and attachmentid. 
        /// </summary>
        /// <param name="mailId"></param>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public byte[] GetAttachments(string mailId, string attachmentId)
        {
            var bytes = (dynamic)null;
            string mailAttachmentGraphUrl = $"{generalSettings.GraphUrl}/v1.0/me/messages/{mailId}/attachments/{attachmentId}";
            string jsonResponse = MakeGetRequestForString(mailAttachmentGraphUrl);
            var attachment = JsonConvert.DeserializeObject<AttachmentDetails>(jsonResponse);
            var content = attachment.ContentBytes;
            bytes = Convert.FromBase64String(content);
            return bytes;
        }
        /// <summary>
        /// gets mail content for the passed in mailid . 
        /// </summary>
        /// <param name="mailId"></param>
        /// <returns></returns>
        public byte[] GetMail(string mailId)
        {
            var bytes = (dynamic)null;
            string mailGraphUrl = $"{generalSettings.GraphUrl}/v1.0/me/messages/{mailId}";
            string jsonResponse = MakeGetRequestForString(mailGraphUrl);
            var mail = JsonConvert.DeserializeObject<MailMessage>(jsonResponse);
            var content = mail.Body.Content;
            string converted = ServiceUtility.EncodeValues(content);
            bytes = Convert.FromBase64String(converted);
            return bytes;
        }
        /// <summary>
        /// gets mail content for the passed in mailid. 
        /// </summary>
        /// <param name="mailId"></param>
        /// <returns></returns>
        public byte[] GetEmailContent(string mailId)
        {
            string mailGraphUrl = $"{generalSettings.GraphUrl}/v1.0/me/messages/{mailId}";
            string jsonResponse = MakeGetRequestForString(mailGraphUrl);
            var mail = JsonConvert.DeserializeObject<MailMessage>(jsonResponse);
            var bytes = (dynamic)null;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            string accessToken = spoAuthorization.GetExchangeAccessToken();
            service.Credentials = new OAuthCredentials(accessToken);            
            service.UseDefaultCredentials = false;
            service.Url = new Uri(generalSettings.ExchangeServiceURL.ToString());            
            service.PreAuthenticate = true;
            service.SendClientLatencies = true;
            service.EnableScpLookup = false;
            mailId = mail.Id.Replace("_", "+");                       
            PropertySet propSet = new PropertySet(BasePropertySet.FirstClassProperties);
            Folder folder = Folder.Bind(service, WellKnownFolderName.Inbox, propSet);            
            SearchFilter filter = new SearchFilter.IsEqualTo(ItemSchema.Id, mailId);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, filter, new ItemView(1));
            foreach (var item in findResults)
            {
                item.Load(new PropertySet(EmailMessageSchema.MimeContent));                
                return item?.MimeContent?.Content;        
            }
            return bytes;
        }
      



        /// <summary>
        /// This methid will return email messages of the current login user and this will return 10 mail messages at a time
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="startIndex"></param>
        /// <param name="includeAttachments"></param>
        /// <returns></returns>
        public MailMessageList ListMessages(MailRequest mailRequest)
        {            
            var folders = ListFolders();
            var inboxFolder = folders != null ? folders.Single(folder => folder.Name == "Inbox") : null;
            string targetUrl = "";
            if(inboxFolder!=null &&  !string.IsNullOrEmpty(inboxFolder.Name))
            {
                targetUrl = $"{generalSettings.GraphUrl}/v1.0/me/mailFolders/{inboxFolder.Id}/messages?$skip={mailRequest.UpToPageNumbersToSkip}&$filter=isDraft eq false";
            }
            else
            {
                targetUrl = $"{generalSettings.GraphUrl}/v1.0/me/messages?$skip={mailRequest.UpToPageNumbersToSkip}&$filter=isDraft eq false";
            }
            if (mailRequest.MessageToSearch != string.Empty)
            {
                targetUrl = $"{generalSettings.GraphUrl}/v1.0/me/mailFolders/{inboxFolder.Id}/messages?$search={mailRequest.MessageToSearch}";
               
            }            
            string jsonResponse = MakeGetRequestForString(targetUrl);

            var messages = JsonConvert.DeserializeObject<MailMessageList>(jsonResponse);
            messages.MailCount = inboxFolder.TotalItemCount;
            if (mailRequest.IncludeAttachments && messages!=null && messages.Messages!=null && messages.Messages.Count>0)
            {
                foreach(var mailMessage in messages.Messages)
                {
                    mailMessage.IsEmail = "true";
                    LoadAttachments(mailMessage);
                }
            }
            return messages;
        }
        /// <summary>
        ///  This methid will return all the  attachments of passedin mailmessage
        /// </summary>
        /// <param name="mailMessage"></param>
        public void LoadAttachments(MailMessage mailMessage)
        {
            if (mailMessage != null && mailMessage.HasAttachments==true)
            {
                
                string mailAttachmentGraphUrl = $"{generalSettings.GraphUrl}/v1.0/me/messages/{mailMessage.Id}/attachments";
                string jsonResponse = MakeGetRequestForString(mailAttachmentGraphUrl);
                var attachments = JsonConvert.DeserializeObject<MailAttachmentList>(jsonResponse);
                mailMessage.Attachments = attachments.AttachmentDetails;
                foreach (var attachment in attachments.AttachmentDetails)
                {
                    attachment.ParentMessageId = mailMessage.Id;
                    attachment.IsEmail = "false";
                }
            }
        }

        /// <summary>
        /// //This methid will all mail folders of the current login user
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<Models.MailFolder> ListFolders(int startIndex=0)
        {
            string folderGraphUrl = $"{generalSettings.GraphUrl}/v1.0/me/mailFolders?$skip={startIndex}";
            string jsonResponse = MakeGetRequestForString(folderGraphUrl);
            var folders = JsonConvert.DeserializeObject<MailFolderList>(jsonResponse);
            return folders.Folders;
        }

        /// <summary>
        /// This meth0d will make an http request call and will return the result as a stream
        /// </summary>
        /// <param name="graphRequestUrl"></param>
        /// <param name="spoAuthorization"></param>
        /// <returns></returns>
        public string MakeGetRequestForString(string graphRequestUrl)
        {
            return MakeHttpRequest("GET", 
                graphRequestUrl, 
                resultPredicate:r=>r.Content.ReadAsStringAsync().Result);
        }
        /// <summary>
        /// this method prepares a http request and get the response for the request url.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="httpMethod"></param>
        /// <param name="requestUrl"></param>
        /// <param name="accept"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="resultPredicate"></param>
        /// <returns></returns>
        private TResult MakeHttpRequest<TResult>(string httpMethod,             
            string requestUrl,
            string accept = null, 
            object content = null, 
            string contentType = null, 
            Func<HttpResponseMessage, TResult> resultPredicate = null)
        {
            //Prepare the variable to hold the resukt if any
            TResult result = default(TResult);
           //Get the access token of the current login user
            string accessToken = spoAuthorization.GetGraphAccessToken();
            if(!string.IsNullOrEmpty(accessToken))
            {
                HttpClient httpClient = new HttpClient();
                //Set the authorization bearer token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //If there is an accept argument, set the corresponding accept header
                if(!string.IsNullOrEmpty(accept))
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                }

                Uri requestUri = new Uri(requestUrl);

                //Prepare the content of the request if any
                HttpContent requestContent = (content != null) ?
                    new StringContent(JsonConvert.SerializeObject(content,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }), Encoding.UTF8, contentType) : null;
                //Prepare http request message with proper http method
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(httpMethod), requestUrl);
                //Set the request content if any
                if(requestContent!=null)
                {
                    request.Content = requestContent;
                }
                //Fire the http request
                HttpResponseMessage response = httpClient.SendAsync(request).Result;
                if(response.IsSuccessStatusCode)
                {
                    if(resultPredicate!=null)
                    {
                        result = resultPredicate(response);
                    }
                }
                else
                {
                    throw new ApplicationException(
                            $"Exception occured while invoking the endpoint {requestUrl}",
                            new HttpRequestException(response.Content.ReadAsStringAsync().Result));
                        
                }
            }
            return result;
        }        
    }
}
