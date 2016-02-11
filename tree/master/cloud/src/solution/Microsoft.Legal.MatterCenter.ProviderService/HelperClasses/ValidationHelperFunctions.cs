// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-nikhid
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="ValidationHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file validates the inputs for matter provision app and returns the validation status (success/failure).</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.HelperClasses
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel.Web;
    using System.Text.RegularExpressions;
    #endregion

    /// <summary>
    /// Provides methods to perform validations.
    /// </summary>
    internal static class ValidationHelperFunctions
    {
        /// <summary>
        /// Validates the inputs for matter provision app and returns the validation status (success/failure).
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterDetails">Matter details object which has data of properties to be stamped</param>
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal static string ProvisionMatterValidation(RequestObject requestObject, Client client, ClientContext clientContext, Matter matter, MatterDetails matterDetails, int methodNumber, MatterConfigurations matterConfigurations)
        {
            if (int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) <= methodNumber && int.Parse(ConstantStrings.EditMatterPermission, CultureInfo.InvariantCulture) >= methodNumber && !Lists.CheckPermissionOnList(ServiceUtility.GetClientContext(null, new Uri(ConstantStrings.ProvisionMatterAppURL), requestObject.RefreshToken), ConstantStrings.SendMailListName, PermissionKind.EditListItems))
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectInputUserAccessCode, ServiceConstantStrings.IncorrectInputUserAccessMessage);
            }
            else
            {
                if (null != requestObject)
                {
                    if (string.IsNullOrWhiteSpace(requestObject.RefreshToken) && string.IsNullOrWhiteSpace(requestObject.SPAppToken))
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputRequestObjectCode, TextConstants.IncorrectInputRequestObjectMessage);
                    }
                }
                if (null != client)
                {
                    string result = ValidateClientInformation(client, methodNumber);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
                if (null != matter)
                {
                    string MatterMetadataValidationResponse = MatterMetadataValidation(matter, clientContext, methodNumber, matterConfigurations);
                    if (!string.IsNullOrEmpty(MatterMetadataValidationResponse))
                    {
                        return MatterMetadataValidationResponse;
                    }
                    if (int.Parse(ConstantStrings.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber)
                    {
                        string roleCheck = ValidationHelperFunctions.RoleCheck(requestObject, matter, client);
                        if (!string.IsNullOrEmpty(roleCheck))
                        {
                            return roleCheck;
                        }
                    }
                    if (null != matter.Permissions)
                    {
                        bool isFullControlPresent = EditMatterHelperFunctions.ValidateFullControlPermission(matter);
                        if (!isFullControlPresent)
                        {
                            return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectInputUserAccessCode, ServiceConstantStrings.ErrorEditMatterMandatoryPermission);
                        }
                    }
                }
                if (null != matterDetails && !(int.Parse(ConstantStrings.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber))
                {
                    if (string.IsNullOrWhiteSpace(matterDetails.PracticeGroup))
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputPracticeGroupCode, TextConstants.IncorrectInputPracticeGroupMessage);
                    }
                    if (string.IsNullOrWhiteSpace(matterDetails.AreaOfLaw))
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputAreaOfLawCode, TextConstants.IncorrectInputAreaOfLawMessage);
                    }
                    if (string.IsNullOrWhiteSpace(matterDetails.SubareaOfLaw))
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputSubareaOfLawCode, TextConstants.IncorrectInputSubareaOfLawMessage);
                    }
                    try
                    {
                        if (string.IsNullOrWhiteSpace(matterDetails.ResponsibleAttorney))
                        {
                            return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputResponsibleAttorneyCode, TextConstants.IncorrectInputResponsibleAttorneyMessage);
                        }
                        else
                        {
                            IList<string> userNames = matterDetails.ResponsibleAttorney.Split(';').ToList<string>();
                            SharePointHelper.ResolveUserNames(clientContext, userNames).FirstOrDefault();
                        }
                    }
                    catch (Exception)
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputResponsibleAttorneyCode, TextConstants.IncorrectInputResponsibleAttorneyMessage);
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Validate email address with REGEX pattern
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>Boolean result indicating whether the email address is of a valid format</returns>
        internal static bool ValidateExternalUserInput(string email)
        {
            return Regex.IsMatch(email.Trim(), TextConstants.EmailValidationRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// Validates meta-data of a matter and returns the validation status (success/failure).
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">Client context object for SharePoint</param>  
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>     
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal static string MatterMetadataValidation(Matter matter, ClientContext clientContext, int methodNumber, MatterConfigurations matterConfigurations)
        {
            string matterNameValidation = MatterNameValidation(matter);
            if (!string.IsNullOrWhiteSpace(matterNameValidation))
            {
                return matterNameValidation;
            }
            if (int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture) == methodNumber)
            {
                if (string.IsNullOrWhiteSpace(matter.Id))
                {
                    return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputMatterIdCode, TextConstants.IncorrectInputMatterIdMessage);
                }
                else
                {
                    var matterId = Regex.Match(matter.Id, ConstantStrings.SpecialCharacterExpressionMatterId, RegexOptions.IgnoreCase);
                    if (int.Parse(ServiceConstantStrings.MatterIdLength, CultureInfo.InvariantCulture) < matter.Id.Length || !matterId.Success)
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputMatterIdCode, TextConstants.IncorrectInputMatterIdMessage);
                    }
                }
            }
            if (int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterShareMatter, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterMatterLandingPage, CultureInfo.InvariantCulture) == methodNumber)
            {
                string matterDetailsValidationResponse = MatterDetailsValidation(matter, clientContext, methodNumber, matterConfigurations);
                if (!string.IsNullOrEmpty(matterDetailsValidationResponse))
                {
                    return matterDetailsValidationResponse;
                }
            }
            try
            {
                if (!(int.Parse(ConstantStrings.ProvisionMatterCheckMatterExists, CultureInfo.InvariantCulture) == methodNumber) && !(int.Parse(ConstantStrings.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber))
                {
                    if (0 >= matter.AssignUserEmails.Count())
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserNamesCode, TextConstants.IncorrectInputUserNamesMessage);
                    }
                    else
                    {
                        IList<string> userList = matter.AssignUserEmails.SelectMany(x => x).Distinct().ToList();
                        SharePointHelper.ResolveUserNames(clientContext, userList).FirstOrDefault();
                    }
                }
            }
            catch (Exception)
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserNamesCode, TextConstants.IncorrectInputUserNamesMessage);
            }

            if (int.Parse(ConstantStrings.ProvisionMatterAssignUserPermissions, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterMatterLandingPage, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber)
            {
                string CheckUserPermissionResponse = CheckUserPermission(matter);
                if (!string.IsNullOrEmpty(CheckUserPermissionResponse))
                {
                    return CheckUserPermissionResponse;
                }
            }
            if (int.Parse(ConstantStrings.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterShareMatter, CultureInfo.InvariantCulture) == methodNumber)
            {
                string validateContentTypeResponse = ValidateContentType(matter);
                if (!string.IsNullOrEmpty(validateContentTypeResponse))
                {
                    return validateContentTypeResponse;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Validates the matter name.
        /// </summary>
        /// <param name="matter">Matter details</param>
        /// <returns>Matter details validation result</returns>
        private static string MatterNameValidation(Matter matter)
        {
            string matterNameValidation = string.Empty;
            if (string.IsNullOrWhiteSpace(matter.Name))
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputMatterNameCode, TextConstants.IncorrectInputMatterNameMessage);
            }
            var matterName = Regex.Match(matter.Name, ConstantStrings.SpecialCharacterExpressionMatterTitle, RegexOptions.IgnoreCase);
            if (int.Parse(ServiceConstantStrings.MatterNameLength, CultureInfo.InvariantCulture) < matter.Name.Length || matter.Name.Length != matterName.Length)
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputMatterNameCode, TextConstants.IncorrectInputMatterNameMessage);
            }
            return matterNameValidation;
        }

        /// <summary>
        /// Validates the permissions assigned to the users.
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        private static string CheckUserPermission(Matter matter)
        {
            if (0 >= matter.Permissions.Count())
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserPermissionsCode, TextConstants.IncorrectInputUserPermissionsMessage);
            }
            else
            {
                string userAllowedPermissions = ServiceConstantStrings.UserPermissions;
                if (!string.IsNullOrEmpty(userAllowedPermissions))
                {
                    List<string> userPermissions = userAllowedPermissions.ToUpperInvariant().Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (string Permissions in matter.Permissions)
                        if (!userPermissions.Contains(Permissions.Trim().ToUpperInvariant()))
                        {
                            return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserPermissionsCode, TextConstants.IncorrectInputUserPermissionsMessage);
                        }

                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Validates content type for the matter.
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        private static string ValidateContentType(Matter matter)
        {
            if ((0 >= matter.ContentTypes.Count()) || string.IsNullOrWhiteSpace(matter.DefaultContentType))
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputContentTypeCode, TextConstants.IncorrectInputContentTypeMessage);
            }
            else
            {
                foreach (string contentType in matter.ContentTypes)
                {
                    var contentTypeCheck = Regex.Match(contentType, ConstantStrings.SpecialCharacterExpressionContentType, RegexOptions.IgnoreCase);
                    if (contentTypeCheck.Success || int.Parse(ServiceConstantStrings.ContentTypeLength, CultureInfo.InvariantCulture) < contentType.Length)
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputContentTypeCode, TextConstants.IncorrectInputContentTypeMessage);
                    }
                }
                var defaultContentTypeCheck = Regex.Match(matter.DefaultContentType, ConstantStrings.SpecialCharacterExpressionContentType, RegexOptions.IgnoreCase);
                if (defaultContentTypeCheck.Success || int.Parse(ServiceConstantStrings.ContentTypeLength, CultureInfo.InvariantCulture) < matter.DefaultContentType.Length)
                {
                    return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputContentTypeCode, TextConstants.IncorrectInputContentTypeMessage);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Validates details of a matter and returns the validation status.
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">Client context object for SharePoint</param>  
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>        
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal static string MatterDetailsValidation(Matter matter, ClientContext clientContext, int methodNumber, MatterConfigurations matterConfigurations)
        {
            if (matterConfigurations.IsMatterDescriptionMandatory)
            {
                if (string.IsNullOrWhiteSpace(matter.Description))
                {
                    return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputMatterDescriptionCode, TextConstants.IncorrectInputMatterDescriptionMessage);
                }
                else
                {
                    var matterDescription = Regex.Match(matter.Description, ConstantStrings.SpecialCharacterExpressionMatterDescription, RegexOptions.IgnoreCase);
                    if (int.Parse(ServiceConstantStrings.MatterDescriptionLength, CultureInfo.InvariantCulture) < matter.Description.Length || !matterDescription.Success)
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputMatterDescriptionCode, TextConstants.IncorrectInputMatterDescriptionMessage);
                    }
                }
            }
            if (matterConfigurations.IsConflictCheck)
            {
                DateTime conflictCheckedOnDate;
                bool isValidDate = DateTime.TryParse(matter.Conflict.CheckOn, out conflictCheckedOnDate);
                if (!isValidDate || 0 > DateTime.Compare(DateTime.Now, conflictCheckedOnDate))
                {
                    return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictDateCode, TextConstants.IncorrectInputConflictDateMessage);
                }
                if (string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                {
                    return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictIdentifiedCode, TextConstants.IncorrectInputConflictIdentifiedMessage);
                }
                else
                {
                    try
                    {
                        if (0 > string.Compare(ConstantStrings.FALSE, matter.Conflict.Identified, StringComparison.OrdinalIgnoreCase))
                        {
                            if (0 >= matter.BlockUserNames.Count())
                            {
                                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputBlockUserNamesCode, TextConstants.IncorrectInputBlockUserNamesMessage);
                            }
                            else
                            {
                                SharePointHelper.ResolveUserNames(clientContext, matter.BlockUserNames).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputBlockUserNamesCode, TextConstants.IncorrectInputBlockUserNamesMessage);
                    }

                }
                if (string.IsNullOrWhiteSpace(matter.Conflict.CheckBy))
                {
                    return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictCheckByCode, TextConstants.IncorrectInputConflictCheckByMessage);
                }
                else
                {
                    try
                    {
                        SharePointHelper.ResolveUserNames(clientContext, new List<string>() { matter.Conflict.CheckBy }).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictCheckByCode, TextConstants.IncorrectInputConflictCheckByMessage);
                    }
                }
            }
            if (int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber && 0 >= matter.Roles.Count())
            {
                return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserRolesCode, TextConstants.IncorrectInputUserRolesMessage);
            }
            return string.Empty;
        }

        /// <summary>
        /// Validates the roles for the matter and returns the validation status.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="client">Client Object</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal static string RoleCheck(RequestObject requestObject, Matter matter, Client client)
        {
            string returnValue = string.Empty;
            try
            {
                using (ClientContext context = ServiceUtility.GetClientContext(requestObject.SPAppToken, new Uri(ServiceConstantStrings.CentralRepositoryUrl), requestObject.RefreshToken))
                {
                    if (0 >= matter.Roles.Count())
                    {
                        return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserRolesCode, TextConstants.IncorrectInputUserRolesMessage);
                    }
                    ListItemCollection collListItem = Lists.GetData(context, ServiceConstantStrings.DMSRoleListName, ServiceConstantStrings.DMSRoleQuery);
                    IList<string> roles = new List<string>();
                    roles = collListItem.AsEnumerable().Select(roleList => Convert.ToString(roleList[ServiceConstantStrings.RoleListColumnRoleName], CultureInfo.InvariantCulture)).ToList();
                    if (matter.Roles.Except(roles).Count() > 0)
                    {
                        returnValue = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputUserRolesCode, TextConstants.IncorrectInputUserRolesMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }

            return returnValue;
        }

        /// <summary>
        /// Validates request token in headers.
        /// </summary>
        /// <returns> Boolean value returns whether validations passed or fail </returns>
        internal static bool CheckRequestValidatorToken()
        {
            string requestValidationTokens = WebOperationContext.Current.IncomingRequest.Headers["RequestValidationToken"];
            return ServiceUtility.ValidateRequestToken(requestValidationTokens);
        }

        /// <summary>
        /// Function to validate client information
        /// </summary>
        /// <param name="client">Client object</param>
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>
        /// <returns>String that contains error message</returns>
        internal static string ValidateClientInformation(Client client, int methodNumber)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(client.Url))
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputClientUrlCode, TextConstants.IncorrectInputClientUrlMessage);
            }
            else if (int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber || int.Parse(ConstantStrings.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture) == methodNumber)
            {
                if (string.IsNullOrWhiteSpace(client.Id))
                {
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputClientIdCode, TextConstants.IncorrectInputClientIdMessage);
                }
                else if (string.IsNullOrWhiteSpace(client.Name))
                {
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputClientNameCode, TextConstants.IncorrectInputClientNameMessage);
                }
            }
            return result;
        }
    }
}