
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="ErrorSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting all the error messages that are used in matter center from the appSettings.json file
    /// These properties will subsequently used where ever exceptions messages needs to be set
    /// </summary>
    public class ErrorSettings
    {
        public string IncorrectInputClientUrlCode { get; set; }
        public string IncorrectInputClientUrlMessage { get; set; }
        public string AuthorizationLengthError { get; set; }
        public string NoBearerStringPresent { get; set; }
        public string MessageNoInputs { get; set; }
        public string MessageNoResult { get; set; }
        public string PeoplePickerNoResults { get; set; }
    }
}
