// ***********************************************************************
// <copyright file="TestReportGenerator.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to call all other class</summary>
// ***********************************************************************
// Assembly         : TestReportGenerator
// Author           : MAQ Software
// Created          : 18-10-2016
// ***********************************************************************

namespace TestReportGenerator
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// This class is used to create execution report from XML file
    /// </summary>
    class ReportGenerator
    {
        /// <summary>
        /// Main method of class
        /// </summary>
        /// <param name="args">Command line arguement</param>
        public static void Main(string[] args)
        {
            ReportGenerator report = new ReportGenerator();
            report.ReportData();
        }

        /// <summary>
        /// This method is used to Manipulate joint string
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Seperated string with first letter with capitalization</returns>
        public string ToSentenceCase(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }

        /// <summary>
        /// Email body chunk
        /// </summary>
        public static string dataTestMailBody = null;

        /// <summary>
        /// Email data
        /// </summary>
        /// <param name="sNo">Row id</param>
        /// <param name="title">Email title</param>
        /// <param name="testResult">Test result</param>
        /// <returns></returns>
        public string EmailDataRow(int sNo, string title, string testResult)
        {
            string resString = testResult == "Failed" ? "Failed" : "Passed";
            string mbody = "";
            string color = testResult == "Failed" ? "yellow" : "lightGreen";
            mbody =
                          "    <tr>"
                          + "        <td style=\"font-size:10.0pt;font-family:'Calibri',sans-serif;color:black; padding-left:5px\">" + sNo + "</td>"
                          + "        <td style=\"font-size:10.0pt;font-family:'Calibri',sans-serif;color:black; padding-left:5px\">" + title + "</td>"
                          + "        <td style=\"color: rgba(0, 0, 0, 1); background-color: " + color + "; font-size:10.0pt;font-family:'Calibri',sans-serif;color:black;padding-left:5px \">" + resString + "</td>"
                          + "    </tr>";
            return mbody;
        }

        /// <summary>
        /// Email configuration details
        /// </summary>
        /// <returns>Confirmation flag</returns>
        public string ConfigureMail()
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    using (SmtpClient smtpc = new SmtpClient(ConfigurationManager.AppSettings["smtpClient"]))
                    {
                        mail.From = new MailAddress(ConfigurationManager.AppSettings["fromEmail"]);
                        mail.To.Add(ConfigurationManager.AppSettings["toEmail"]);
                        mail.CC.Add(ConfigurationManager.AppSettings["ccEmail"]);
                        mail.IsBodyHtml = true;

                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(ConfigurationManager.AppSettings["logFileName"]);
                        mail.Attachments.Add(attachment);

                        DateTime Dt = DateTime.Today;
                        string emaildate = Dt.ToString(ConfigurationManager.AppSettings["Date"]);
                        mail.Subject = ConfigurationManager.AppSettings["Subject"] + emaildate;
                        mail.Body = "<h2 style=\"font-family:'Calibri'\">" + ConfigurationManager.AppSettings["Subject"] + emaildate + "</h1>"
                                      + "<table style='width: 80%;' border=1 cellspacing=0 cellpadding=0 ;margin-left:22.5pt; padding:5px >"
                                      + "<tbody>"
                                      + "    <tr style='color: white; background-color: #4476B6;height:15px'>"
                                      + "        <td style=\"font-size:10.0pt;font-family:'Calibri',sans-serif;color:white;width:3%; text-align:center\"> # </td>"
                                      + "        <td style=\"font-size:10.0pt;font-family:'Calibri',sans-serif;color:white;width:70%; text-align:center\">Description</td>"
                                      + "        <td style=\"font-size:10.0pt;font-family:'Calibri',sans-serif;color:white;width:3.7%; text-align:center \">Status</td>"
                                      + "    </tr>"
                                      + dataTestMailBody
                                      + "</tbody> </table>";

                        smtpc.Port = 587;
                        smtpc.UseDefaultCredentials = false;
                        smtpc.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["fromEmail"], ConfigurationManager.AppSettings["emailPassword"]);
                        smtpc.EnableSsl = true;
                        smtpc.Send(mail);
                        Console.WriteLine("Email sent successfully");
                        return "true";
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetBaseException());
                return "false";
            }
        }

        /// <summary>
        /// To retrieve test report data
        /// </summary>
        public void ReportData()
        {
            int count = 1;
            using (XmlReader reader = XmlReader.Create(ConfigurationManager.AppSettings["logFileName"]))
            {
                string pageName = string.Empty;
                string data = string.Empty;
                string outcome = string.Empty;
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "UnitTestResult":
                                string myString = Regex.Replace(reader["testName"], "_*_", string.Empty);
                                data = reader["testName"];
                                int startLength = data.LastIndexOf('_');
                                int length = data.Length;
                                data = data.Substring((startLength + 1), (length - startLength - 1));
                                outcome = reader["outcome"];
                                Console.WriteLine("TestcaseId: " + count);
                                Console.WriteLine("testName:" + reader["testName"]);
                                Console.WriteLine("duration:" + reader["duration"]);
                                Console.WriteLine("outcome:" + reader["outcome"]);
                                Console.WriteLine("\n");
                                string dataChunk = reader.ReadString();
                                string sentenceChanger = ToSentenceCase(data);
                                if (sentenceChanger.ToLower().Contains("ecb"))
                                    sentenceChanger = sentenceChanger.Replace("ecb", "ECB");
                                if (sentenceChanger.ToLower().Contains("matter"))
                                    sentenceChanger = sentenceChanger.Replace("matter", "Matter");
                                if (sentenceChanger.ToLower().Contains("done:"))
                                    sentenceChanger = sentenceChanger.Substring(sentenceChanger.IndexOf(':') + 1, sentenceChanger.Length - (sentenceChanger.IndexOf(':') + 1));
                                dataTestMailBody += EmailDataRow(count, sentenceChanger, outcome);
                                count++;
                                break;
                        }
                    }
                }
            }
            ConfigureMail();
            if (File.Exists(ConfigurationManager.AppSettings["logFileName"]))
            {
                File.Delete(ConfigurationManager.AppSettings["logFileName"]);
            }
        }
    }
}
