// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-rijadh
// Created          : 03-14-2014
//
// ***********************************************************************
// <copyright file="DownloadEmail.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of DownloadEmail.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.IO;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides functionality to download email.
    /// </summary>
    public partial class DownloadEmail : System.Web.UI.Page
    {
        /// <summary>
        /// Generates the stream from mail content.
        /// </summary>
        /// <param name="streamValue">The stream value.</param>
        /// <returns>Memory Stream.</returns>
        private static MemoryStream GenerateStreamFromString(string streamValue)
        {
            MemoryStream result = null;
            try
            {
                using (MemoryStream mailStream = new MemoryStream())
                {
                    StreamWriter mailStreamWriter = new StreamWriter(mailStream);
                    mailStreamWriter.Write(streamValue);
                    mailStreamWriter.Flush();
                    mailStream.Position = 0;
                    result = mailStream;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                result = new MemoryStream();
            }
            return result;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
				string requestValidationTokens = Request.Form["requestToken"];				
				if (ServiceUtility.ValidateRequestToken(requestValidationTokens))
				{				
                string emailText = Server.UrlDecode(Request.Form["MailContent"]);
                if (null != emailText)
                {
                    string emailName = UIConstantStrings.EmailName + DateTime.Now + ConstantStrings.EmailFileExtension;
                    MemoryStream emailStream = GenerateStreamFromString(emailText);
                    Response.Buffer = true;
                    Response.ClearHeaders();
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("content-disposition", "inline; filename=" + emailName);
                    Response.BinaryWrite(emailStream.ToArray());
                    Response.Flush();
                    Response.End();
                }
				}
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
            }
        }
    }
}