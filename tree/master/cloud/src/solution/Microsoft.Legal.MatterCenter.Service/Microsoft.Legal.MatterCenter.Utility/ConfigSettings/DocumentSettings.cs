// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="DocumentSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the document configuration information from the appSettings.json file
    /// These properties will subsequently used when getting the document information from SPO
    /// </summary>
    public class DocumentSettings
    {
        public string FolderStructureModified { get; set; }
        public string TempEmailName { get; set; }
        public string MailCartMailSubject { get; set; }
        public string MailCartMailBody { get; set; }
        public string SendAsEmailFormat { get; set; }
        public string TimeStampFormat { get; set; }
    }
}
