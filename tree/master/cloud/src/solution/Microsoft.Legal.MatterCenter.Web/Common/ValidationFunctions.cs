using Microsoft.Extensions.OptionsModel;
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

namespace Microsoft.Legal.MatterCenter
{
    public class ValidationFunctions : IValidationFunctions
    {
        private MatterSettings matterSettings;
        private ErrorSettings errorSettings;
        private ListNames listNames;
        private CamlQueries camlQueries;
        private ISPList spList;
        private IMatterRepository matterRespository;
        public ValidationFunctions(ISPList spList, IOptions<MatterSettings> matterSettings, 
            IOptions<ErrorSettings> errorSettings, IMatterRepository matterRespository,
            IOptions<ListNames> listNames, IOptions<CamlQueries> camlQueries)
        {
            this.matterSettings = matterSettings.Value;
            this.spList = spList;
            this.errorSettings = errorSettings.Value;
            this.matterRespository = matterRespository;
            this.listNames = listNames.Value;
            this.camlQueries = camlQueries.Value;
        }

        /// <summary>
        /// Checks if the lists exist
        /// </summary>
        /// <param name="client"></param>
        /// <param name="matterName"></param>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        private bool CheckListExists(Client client, string matterName, MatterConfigurations matterConfigurations = null)
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
            bool listExists = spList.Exists(client, new ReadOnlyCollection<string>(lists));
            return listExists;
        }

        public GenericResponseVM IsMatterValid(MatterInformationVM matterInformation, int methodNumber, MatterConfigurations matterConfigurations)
        {
            GenericResponseVM genericResponse = null;
            var matterDetails = matterInformation.MatterDetails;
            if (int.Parse(ServiceConstants.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) <= methodNumber && 
                int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) >= methodNumber && 
                !spList.CheckPermissionOnList(matterSettings.ProvisionMatterAppURL, matterSettings.SendMailListName, PermissionKind.EditListItems))
            {
                genericResponse = new GenericResponseVM();
                //return string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectInputUserAccessCode, ServiceConstantStrings.IncorrectInputUserAccessMessage);
                genericResponse.Code = errorSettings.IncorrectInputUserAccessCode;
                genericResponse.Value = errorSettings.IncorrectInputUserAccessMessage;
            }
            else
            {
                
                if (matterInformation.Client!=null)
                {
                    genericResponse = new GenericResponseVM();
                    genericResponse = ValidateClientInformation(matterInformation.Client, methodNumber);
                    if (genericResponse!=null)
                    {
                        return genericResponse;
                    }
                }
                if (matterInformation.Matter!=null)
                {
                    genericResponse = MatterMetadataValidation(matterInformation.Matter, matterInformation.Client, 
                        methodNumber, matterConfigurations);
                    if (genericResponse!=null)
                    {
                        return genericResponse;
                    }
                    if (int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber)
                    {
                        genericResponse = RoleCheck(matterInformation.Matter);
                        if (genericResponse!=null)
                        {
                            return genericResponse;
                        }
                    }
                    if (matterInformation.Matter.Permissions!=null)
                    {
                        bool isFullControlPresent = ValidateFullControlPermission(matterInformation.Matter);
                        if (!isFullControlPresent)
                        {                            
                            return GenericResponse(errorSettings.IncorrectInputUserAccessCode, errorSettings.ErrorEditMatterMandatoryPermission);
                        }
                    }
                }
                if (null != matterDetails && !(int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber))
                {
                    if (string.IsNullOrWhiteSpace(matterDetails.PracticeGroup))
                    {                        
                        return GenericResponse(errorSettings.IncorrectInputPracticeGroupCode, errorSettings.IncorrectInputPracticeGroupMessage);
                    }
                    if (string.IsNullOrWhiteSpace(matterDetails.AreaOfLaw))
                    {
                        
                        return GenericResponse(errorSettings.IncorrectInputAreaOfLawCode, errorSettings.IncorrectInputAreaOfLawMessage);
                    }
                    if (string.IsNullOrWhiteSpace(matterDetails.SubareaOfLaw))
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
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="client">Client Object</param>
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal GenericResponseVM RoleCheck(Matter matter)
        {
            GenericResponseVM genericResponse = null;
            try
            {
                
                if ( matter.Roles.Count()<=0)
                {                    
                    return GenericResponse(errorSettings.IncorrectInputUserRolesCode, errorSettings.IncorrectInputUserRolesMessage);
                }
                IList<string> roles = matterRespository.RoleCheck(matterSettings.CentralRepositoryUrl, listNames.DMSRoleListName,
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
            GenericResponseVM response = new GenericResponseVM();
            if (string.IsNullOrWhiteSpace(client.Url))
            {
                response.Code = errorSettings.IncorrectInputClientUrlCode;
                response.Value = errorSettings.IncorrectInputClientUrlMessage;                
            }
            else if (int.Parse(ServiceConstants.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture) == methodNumber)
            {
                if (string.IsNullOrWhiteSpace(client.Id))
                {
                    
                    response.Code = errorSettings.IncorrectInputClientIdCode;
                    response.Value = errorSettings.IncorrectInputClientIdMessage;
                }
                else if (string.IsNullOrWhiteSpace(client.Name))
                {                   
                    response.Code = errorSettings.IncorrectInputClientNameCode;
                    response.Value = errorSettings.IncorrectInputClientNameMessage;
                }
            }
            return response;
        }

        /// <summary>
        /// Validates meta-data of a matter and returns the validation status (success/failure).
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">Client context object for SharePoint</param>  
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>     
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal GenericResponseVM MatterMetadataValidation(Matter matter, Client client, 
            int methodNumber, MatterConfigurations matterConfigurations)
        {
            GenericResponseVM genericResponseVM = null;
            genericResponseVM = MatterNameValidation(matter);
            if (genericResponseVM!=null)
            {
                return genericResponseVM;
            }
            if (int.Parse(ServiceConstants.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || 
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
                    if (int.Parse(matterSettings.MatterIdLength, CultureInfo.InvariantCulture) < matter.Id.Length || !matterId.Success)
                    {     
                        return GenericResponse(errorSettings.IncorrectInputMatterIdCode, errorSettings.IncorrectInputMatterIdMessage);
                    }
                }
            }
            if (int.Parse(ServiceConstants.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.ProvisionMatterShareMatter, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.ProvisionMatterMatterLandingPage, CultureInfo.InvariantCulture) == methodNumber)
            {
                genericResponseVM = MatterDetailsValidation(matter, client, methodNumber, matterConfigurations);
                if (genericResponseVM!=null)
                {
                    return genericResponseVM;
                }
            }
            try
            {
                if (!(int.Parse(ServiceConstants.ProvisionMatterCheckMatterExists, CultureInfo.InvariantCulture) == methodNumber) && 
                    !(int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber))
                {
                    if (0 >= matter.AssignUserNames.Count())
                    {                                                
                        return GenericResponse(errorSettings.IncorrectInputUserNamesCode, errorSettings.IncorrectInputUserNamesMessage);
                    }
                    else
                    {
                        IList<string> userList = matter.AssignUserNames.SelectMany(x => x).Distinct().ToList();
                        //ToDo: Need to know the use of this method
                        matterRespository.ResolveUserNames(client, userList).FirstOrDefault();
                    }
                }
            }
            catch (Exception)
            {                
                return GenericResponse(errorSettings.IncorrectInputUserNamesCode, errorSettings.IncorrectInputUserNamesMessage);
            }

            if (int.Parse(ServiceConstants.ProvisionMatterAssignUserPermissions, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.ProvisionMatterMatterLandingPage, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.EditMatterPermission, CultureInfo.InvariantCulture) == methodNumber)
            {
                GenericResponseVM genericResponse = CheckUserPermission(matter);
                if (genericResponse!=null)
                {
                    return genericResponse;
                }
            }
            if (int.Parse(ServiceConstants.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber || 
                int.Parse(ServiceConstants.ProvisionMatterShareMatter, CultureInfo.InvariantCulture) == methodNumber)
            {
                GenericResponseVM genericResponse = ValidateContentType(matter);
                if (genericResponse!=null)
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
            if (int.Parse(matterSettings.MatterNameLength, CultureInfo.InvariantCulture) < matter.Name.Length || matter.Name.Length != matterName.Length)
            {
                
                return GenericResponse(errorSettings.IncorrectInputMatterNameCode, errorSettings.IncorrectInputMatterNameMessage);
            }
            return genericResponseVM;
        }

        /// <summary>
        /// Validates details of a matter and returns the validation status.
        /// </summary>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">Client context object for SharePoint</param>  
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>        
        /// <returns>A string value indicating whether validations passed or fail</returns>
        internal GenericResponseVM MatterDetailsValidation(Matter matter, Client client, int methodNumber, 
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
                    var matterDescription = Regex.Match(matter.Description, matterSettings.SpecialCharacterExpressionMatterDescription, RegexOptions.IgnoreCase);
                    if (int.Parse(matterSettings.MatterDescriptionLength, CultureInfo.InvariantCulture) < matter.Description.Length || !matterDescription.Success)
                    {  
                        return GenericResponse(errorSettings.IncorrectInputMatterDescriptionCode, errorSettings.IncorrectInputMatterDescriptionMessage);
                    }
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
            if (int.Parse(ServiceConstants.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber && 0 >= matter.Roles.Count())
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
                    if (contentTypeCheck.Success || int.Parse(matterSettings.ContentTypeLength, CultureInfo.InvariantCulture) < contentType.Length)
                    {                        
                        return GenericResponse(errorSettings.IncorrectInputContentTypeCode, errorSettings.IncorrectInputContentTypeMessage);
                    }
                }
                var defaultContentTypeCheck = Regex.Match(matter.DefaultContentType, matterSettings.SpecialCharacterExpressionContentType, RegexOptions.IgnoreCase);
                if (defaultContentTypeCheck.Success || 
                    int.Parse(matterSettings.ContentTypeLength, CultureInfo.InvariantCulture) < matter.DefaultContentType.Length)
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
