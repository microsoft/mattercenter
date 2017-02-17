using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class ValidationFunctions : IValidationFunctions
    {
        private MatterSettings matterSettings;
        private ErrorSettings errorSettings;
        private ListNames listNames;
        private CamlQueries camlQueries;
        private ISPList spList;
        private IMatterRepository matterRespository;
        private GeneralSettings generalSettings;
        private TaxonomySettings taxonomySettings;
        /// <summary>
        /// Do validation stuff
        /// </summary>
        /// <param name="spList"></param>
        /// <param name="matterSettings"></param>
        /// <param name="errorSettings"></param>
        /// <param name="matterRespository"></param>
        /// <param name="listNames"></param>
        /// <param name="camlQueries"></param>
        public ValidationFunctions(ISPList spList, IOptions<MatterSettings> matterSettings,
            IOptions<ErrorSettings> errorSettings, IMatterRepository matterRespository,
            IOptions<ListNames> listNames, IOptions<CamlQueries> camlQueries, IOptions<GeneralSettings> generalSettings, IOptions<TaxonomySettings> taxonomySettings)
        {
            this.matterSettings = matterSettings.Value;
            this.spList = spList;
            this.errorSettings = errorSettings.Value;
            this.matterRespository = matterRespository;
            this.listNames = listNames.Value;
            this.camlQueries = camlQueries.Value;
            this.generalSettings = generalSettings.Value;
            this.taxonomySettings = taxonomySettings.Value;
        }

        /// <summary>
        /// Checks if the lists exist
        /// </summary>
        /// <param name="client"></param>
        /// <param name="matterName"></param>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        public List<string> CheckListExists(Client client, string matterName, MatterConfigurations matterConfigurations = null)
        {
            
            List<string> lists = new List<string>();
            lists.Add(matterName);
            lists.Add(matterName + matterSettings.OneNoteLibrarySuffix);
            if (null == matterConfigurations || matterConfigurations.IsCalendarSelected)
            {
                lists.Add(matterName + matterSettings.CalendarNameSuffix);
            }
            if (null == matterConfigurations || matterConfigurations.IsTaskSelected)
            {
                lists.Add(matterName + matterSettings.TaskNameSuffix);
            }
            List<string> listExists = matterRespository.Exists(client, new ReadOnlyCollection<string>(lists));
            return listExists;
        }

        /// <summary>
        /// Check the matter is valid or not
        /// </summary>
        /// <param name="matterInformation"></param>
        /// <param name="methodNumber"></param>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        public GenericResponseVM IsMatterValid(MatterInformationVM matterInformation, int methodNumber, MatterConfigurations matterConfigurations)
        {
            GenericResponseVM genericResponse = null;
            var matterDetails = matterInformation.MatterDetails;
            if (int.Parse(ServiceConstants.PROVISION_MATTER_CREATEMATTER, CultureInfo.InvariantCulture) <= methodNumber &&
                int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) >= methodNumber &&
                !spList.CheckPermissionOnList(generalSettings.CentralRepositoryUrl, matterSettings.SendMailListName, PermissionKind.EditListItems))
            {
                genericResponse = new GenericResponseVM();
                //return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectInputUserAccessCode, ServiceConstantStrings.IncorrectInputUserAccessMessage);
                genericResponse.Code = errorSettings.IncorrectInputUserAccessCode;
                genericResponse.Value = errorSettings.IncorrectInputUserAccessMessage;
            }
            else
            {

                if (matterInformation.Client != null)
                {
                    
                    genericResponse = ValidateClientInformation(matterInformation.Client, methodNumber);
                    if (genericResponse != null)
                    {
                        return genericResponse;
                    }
                }
                if (matterInformation.Matter != null)
                {
                    genericResponse = MatterMetadataValidation(matterInformation.Matter, matterInformation.Client,
                        methodNumber, matterConfigurations);
                    if (genericResponse != null)
                    {
                        return genericResponse;
                    }
                    if (int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber)
                    {
                        genericResponse = RoleCheck(matterInformation.Matter);
                        if (genericResponse != null)
                        {
                            return genericResponse;
                        }
                    }
                    if (matterInformation.Matter.Permissions != null)
                    {
                        bool isFullControlPresent = matterInformation.IsFullControlPresent? matterInformation.IsFullControlPresent: ValidateFullControlPermission(matterInformation.Matter);
                            //ValidateFullControlPermission(matterInformation.Matter);
                        if (!isFullControlPresent)
                        {
                            return GenericResponse(errorSettings.IncorrectInputUserAccessCode, errorSettings.ErrorEditMatterMandatoryPermission);
                        }
                    }
                }
                if (null != matterDetails && !(int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber))
                {
                    //if (string.IsNullOrWhiteSpace(matterDetails.PracticeGroup))
                    //{
                    //    return GenericResponse(errorSettings.IncorrectInputPracticeGroupCode, errorSettings.IncorrectInputPracticeGroupMessage);
                    //}
                    //if (string.IsNullOrWhiteSpace(matterDetails.AreaOfLaw))
                    //{

                    //    return GenericResponse(errorSettings.IncorrectInputAreaOfLawCode, errorSettings.IncorrectInputAreaOfLawMessage);
                    //}
                    //if (string.IsNullOrWhiteSpace(matterDetails.SubareaOfLaw))
                    //{
                    //    return GenericResponse(errorSettings.IncorrectInputSubareaOfLawCode, errorSettings.IncorrectInputSubareaOfLawMessage);
                    //}


                    if (matterDetails.ManagedColumnTerms == null && matterDetails.ManagedColumnTerms.Count != taxonomySettings.Levels)
                    {
                        return GenericResponse(errorSettings.IncorrectInputSubareaOfLawCode, errorSettings.IncorrectInputSubareaOfLawMessage);
                    }
                    try
                    {
                        if (string.IsNullOrWhiteSpace(matterDetails.ResponsibleAttorney))
                        {
                            return GenericResponse(errorSettings.IncorrectInputResponsibleAttorneyCode, errorSettings.IncorrectInputResponsibleAttorneyMessage);
                        }
                        else
                        {
                            IList<string> userNames = matterDetails.ResponsibleAttorney.Split(';').ToList<string>();
                            matterRespository.ResolveUserNames(matterInformation.Client, userNames).FirstOrDefault();
                        }
                    }
                    catch (Exception)
                    {
                        return GenericResponse(errorSettings.IncorrectInputResponsibleAttorneyCode, errorSettings.IncorrectInputResponsibleAttorneyMessage);
                    }
                }
            }
            return genericResponse;
        }


        /// <summary>
        /// Validates the roles for the matter and returns the validation status.
        /// </summary>
        /// <param name="matter"></param>
        /// <returns></returns>
        public GenericResponseVM RoleCheck(Matter matter)
        {
            GenericResponseVM genericResponse = null;
            try
            {

                if (matter.Roles.Count() <= 0)
                {
                    return GenericResponse(errorSettings.IncorrectInputUserRolesCode, errorSettings.IncorrectInputUserRolesMessage);
                }
                IList<string> roles = matterRespository.RoleCheck(generalSettings.CentralRepositoryUrl, listNames.DMSRoleListName,
                camlQueries.DMSRoleQuery);
                if (matter.Roles.Except(roles).Count() > 0)
                {
                    return GenericResponse(errorSettings.IncorrectInputUserRolesCode, errorSettings.IncorrectInputUserRolesMessage);
                }
                return genericResponse;
            }
            catch (Exception exception)
            {
                //ToDo: Why in role check function, we are deleting the matter
                //ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                //returnValue = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                throw;
            }


        }

        /// <summary>
        /// Function to validate client information
        /// </summary>
        /// <param name="client">Client object</param>
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>
        /// <returns>String that contains error message</returns>
        internal GenericResponseVM ValidateClientInformation(Client client, int methodNumber)
        {
            GenericResponseVM response = null;
            if (string.IsNullOrWhiteSpace(client.Url))
            {
                response = new GenericResponseVM();
                response.Code = errorSettings.IncorrectInputClientUrlCode;
                response.Value = errorSettings.IncorrectInputClientUrlMessage;
                return response;
            }
            else if (int.Parse(ServiceConstants.PROVISION_MATTER_CREATEMATTER, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture) == methodNumber)
            {
                if (string.IsNullOrWhiteSpace(client.Id))
                {
                    response = new GenericResponseVM();
                    response.Code = errorSettings.IncorrectInputClientIdCode;
                    response.Value = errorSettings.IncorrectInputClientIdMessage;
                    return response;
                }
                else if (string.IsNullOrWhiteSpace(client.Name))
                {
                    response = new GenericResponseVM();
                    response.Code = errorSettings.IncorrectInputClientNameCode;
                    response.Value = errorSettings.IncorrectInputClientNameMessage;
                    return response;
                }
            }
            return response;
        }
        /// <summary>
        /// Validates meta-data of a matter and returns the validation status (success/failure).
        /// </summary>
        /// <param name="matter"></param>
        /// <param name="client"></param>
        /// <param name="methodNumber"></param>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        public GenericResponseVM MatterMetadataValidation(Matter matter, Client client,
            int methodNumber, MatterConfigurations matterConfigurations)
        {
            GenericResponseVM genericResponseVM = null;
            genericResponseVM = MatterNameValidation(matter);
            if (genericResponseVM != null)
            {
                return genericResponseVM;
            }
            if (int.Parse(ServiceConstants.PROVISION_MATTER_CREATEMATTER, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture) == methodNumber)
            {
                if (string.IsNullOrWhiteSpace(matter.Id))
                {
                    return GenericResponse(errorSettings.IncorrectInputMatterIdCode, errorSettings.IncorrectInputMatterIdMessage);
                }
                else
                {
                    var matterId = Regex.Match(matter.Id, matterSettings.SpecialCharacterExpressionMatterId, RegexOptions.IgnoreCase);
                    if (matterSettings.MatterIdLength < matter.Id.Length || !matterId.Success)
                    {
                        return GenericResponse(errorSettings.IncorrectInputMatterIdCode, errorSettings.IncorrectInputMatterIdMessage);
                    }
                }
            }
            if (int.Parse(ServiceConstants.PROVISION_MATTER_CREATEMATTER, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.ProvisionMatterShareMatter, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.PROVISIONMATTER_MATTER_LANDING_PAGE, CultureInfo.InvariantCulture) == methodNumber)
            {
                genericResponseVM = MatterDetailsValidation(matter, client, methodNumber, matterConfigurations);
                if (genericResponseVM != null)
                {
                    return genericResponseVM;
                }
            }
            try
            {
                if (!(int.Parse(ServiceConstants.PROVISION_MATTER_CHECK_MATTER_EXISTS, CultureInfo.InvariantCulture) == methodNumber) &&
                    !(int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber))
                {
                    if (0 >= matter.AssignUserNames.Count())
                    {
                        return GenericResponse(errorSettings.IncorrectInputUserNamesCode, errorSettings.IncorrectInputUserNamesMessage);
                    }
                    else
                    {
                        IList<string> userList = matter.AssignUserEmails.SelectMany(x => x).Distinct().ToList();
                        //ToDo: Need to know the use of this method
                        matterRespository.ResolveUserNames(client, userList).FirstOrDefault();
                    }
                }
            }
            catch (Exception)
            {
                return GenericResponse(errorSettings.IncorrectInputUserNamesCode, errorSettings.IncorrectInputUserNamesMessage);
            }

            if (int.Parse(ServiceConstants.PROVISION_MATTER_ASSIGN_USER_PERMISSIONS, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.PROVISIONMATTER_MATTER_LANDING_PAGE, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber)
            {
                GenericResponseVM genericResponse = CheckUserPermission(matter);
                if (genericResponse != null)
                {
                    return genericResponse;
                }
            }
            if (int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber ||
                int.Parse(ServiceConstants.ProvisionMatterShareMatter, CultureInfo.InvariantCulture) == methodNumber)
            {
                GenericResponseVM genericResponse = ValidateContentType(matter);
                if (genericResponse != null)
                {
                    return genericResponse;
                }
            }
            return genericResponseVM;
        }

        /// <summary>
        /// Validates the matter name.
        /// </summary>
        /// <param name="matter">Matter details</param>
        /// <returns>Matter details validation result</returns>
        private GenericResponseVM MatterNameValidation(Matter matter)
        {
            GenericResponseVM genericResponseVM = null;
            string matterNameValidation = string.Empty;
            if (string.IsNullOrWhiteSpace(matter.Name))
            {
                return GenericResponse(errorSettings.IncorrectInputMatterNameCode, errorSettings.IncorrectInputMatterNameMessage);
            }
            var matterName = Regex.Match(matter.Name, matterSettings.SpecialCharacterExpressionMatterTitle, RegexOptions.IgnoreCase);
            if (matterSettings.MatterNameLength < matter.Name.Length || matter.Name.Length != matterName.Length)
            {
                return GenericResponse(errorSettings.IncorrectInputMatterNameCode, errorSettings.IncorrectInputMatterNameMessage);
            }
            return genericResponseVM;
        }

        /// <summary>
        ///  Validates details of a matter and returns the validation status.
        /// </summary>
        /// <param name="matter"></param>
        /// <param name="client"></param>
        /// <param name="methodNumber"></param>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        public GenericResponseVM MatterDetailsValidation(Matter matter, Client client, int methodNumber,
            MatterConfigurations matterConfigurations)
        {
            GenericResponseVM genericResponseVM = null;
            if (matterConfigurations.IsMatterDescriptionMandatory)
            {
                if (string.IsNullOrWhiteSpace(matter.Description))
                {
                    return GenericResponse(errorSettings.IncorrectInputMatterDescriptionCode, errorSettings.IncorrectInputMatterDescriptionMessage);
                }
                else
                {
                    //var matterDescription = Regex.Match(matter.Description, matterSettings.SpecialCharacterExpressionMatterDescription, RegexOptions.IgnoreCase);
                    //if (matter.Description.Length > matterSettings.MatterDescriptionLength  || !matterDescription.Success)
                    //{
                    //    return GenericResponse(errorSettings.IncorrectInputMatterDescriptionCode, errorSettings.IncorrectInputMatterDescriptionMessage);
                    //}
                }
            }
            if (matterConfigurations.IsConflictCheck)
            {
                DateTime conflictCheckedOnDate;
                bool isValidDate = DateTime.TryParse(matter.Conflict.CheckOn, out conflictCheckedOnDate);
                if (!isValidDate || 0 > DateTime.Compare(DateTime.Now, conflictCheckedOnDate))
                {
                    return GenericResponse(errorSettings.IncorrectInputConflictDateCode, errorSettings.IncorrectInputConflictDateMessage);
                }
                if (string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                {
                    return GenericResponse(errorSettings.IncorrectInputConflictIdentifiedCode, errorSettings.IncorrectInputConflictIdentifiedMessage);
                }
                else
                {
                    try
                    {
                        if (0 > string.Compare(ServiceConstants.FALSE, matter.Conflict.Identified, StringComparison.OrdinalIgnoreCase))
                        {
                            if (0 >= matter.BlockUserNames.Count())
                            {
                                return GenericResponse(errorSettings.IncorrectInputBlockUserNamesCode, errorSettings.IncorrectInputBlockUserNamesMessage);
                            }
                            else
                            {
                                //ToDo: Need to understand the need of this method
                                matterRespository.ResolveUserNames(client, matter.BlockUserNames).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return GenericResponse(errorSettings.IncorrectInputBlockUserNamesCode, errorSettings.IncorrectInputBlockUserNamesMessage);
                    }

                }
                if (string.IsNullOrWhiteSpace(matter.Conflict.CheckBy))
                {
                    return GenericResponse(errorSettings.IncorrectInputConflictCheckByCode, errorSettings.IncorrectInputConflictCheckByMessage);
                }
                else
                {
                    try
                    {
                        //ToDo: Need to understand the need of this method                   
                        matterRespository.ResolveUserNames(client, new List<string>() { matter.Conflict.CheckBy }).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                        return GenericResponse(errorSettings.IncorrectInputConflictCheckByCode, errorSettings.IncorrectInputConflictCheckByMessage);
                    }
                }
            }
            if (int.Parse(ServiceConstants.PROVISION_MATTER_CREATEMATTER, CultureInfo.InvariantCulture) == methodNumber && 0 >= matter.Roles.Count())
            {
                return GenericResponse(errorSettings.IncorrectInputUserRolesCode, errorSettings.IncorrectInputUserRolesMessage);
            }
            return genericResponseVM;
        }


        public GenericResponseVM GenericResponse(string code, string value)
        {
            GenericResponseVM genericResponseVM = new GenericResponseVM();
            genericResponseVM.Code = errorSettings.IncorrectInputUserRolesCode;
            genericResponseVM.Value = errorSettings.IncorrectInputUserRolesMessage;
            return genericResponseVM;
        }

        /// <summary>
        /// Validates the permissions assigned to the users.
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        private GenericResponseVM CheckUserPermission(Matter matter)
        {
            if (0 >= matter.Permissions.Count())
            {
                return GenericResponse(errorSettings.IncorrectInputUserPermissionsCode, errorSettings.IncorrectInputUserPermissionsMessage);
            }
            else
            {
                string userAllowedPermissions = matterSettings.UserPermissions;
                if (!string.IsNullOrEmpty(userAllowedPermissions))
                {
                    List<string> userPermissions = userAllowedPermissions.ToUpperInvariant().Trim().Split(new string[] { "," },
                        StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (string Permissions in matter.Permissions)
                    {
                        if (!userPermissions.Contains(Permissions.Trim().ToUpperInvariant()))
                        {
                            return GenericResponse(errorSettings.IncorrectInputUserPermissionsCode, errorSettings.IncorrectInputUserPermissionsMessage);
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Validates content type for the matter.
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        private GenericResponseVM ValidateContentType(Matter matter)
        {
            if ((0 >= matter.ContentTypes.Count()) || string.IsNullOrWhiteSpace(matter.DefaultContentType))
            {
                return GenericResponse(errorSettings.IncorrectInputContentTypeCode, errorSettings.IncorrectInputContentTypeMessage);
            }
            else
            {
                foreach (string contentType in matter.ContentTypes)
                {
                    var contentTypeCheck = Regex.Match(contentType, matterSettings.SpecialCharacterExpressionContentType, RegexOptions.IgnoreCase);
                    if (contentTypeCheck.Success || matterSettings.ContentTypeLength < contentType.Length)
                    {
                        return GenericResponse(errorSettings.IncorrectInputContentTypeCode, errorSettings.IncorrectInputContentTypeMessage);
                    }
                }
                var defaultContentTypeCheck = Regex.Match(matter.DefaultContentType, matterSettings.SpecialCharacterExpressionContentType, RegexOptions.IgnoreCase);
                if (defaultContentTypeCheck.Success ||
                    matterSettings.ContentTypeLength < matter.DefaultContentType.Length)
                {
                    return GenericResponse(errorSettings.IncorrectInputContentTypeCode, errorSettings.IncorrectInputContentTypeMessage);
                }
            }
            return null;
        }

        /// <summary>
        /// Validates if there is at-least one user with full control in assign list.
        /// </summary>
        /// <param name="matter">Matter object</param>
        /// <returns>Status of Full Control permission</returns>
        private bool ValidateFullControlPermission(Matter matter)
        {

            bool hasFullConrol = false;
            if (null != matter && null != matter.Permissions && 0 != matter.Permissions.Count)
            {
                hasFullConrol = matter.Permissions.Contains(matterSettings.EditMatterAllowedPermissionLevel);
            }
            return hasFullConrol;
        }
    }
}
