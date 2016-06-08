using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class MatterProvision:IMatterProvision
    {
        private MatterSettings matterSettings;
        private IMatterRepository matterRepositoy;
        private ISPOAuthorization spoAuthorization;
        private IEditFunctions editFunctions;
        private ErrorSettings errorSettings;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private MailSettings  mailSettings;
        private IValidationFunctions validationFunctions;
        private CamlQueries camlQueries;
        private ListNames listNames;
        private SearchSettings searchSettings;
        private IUserRepository userRepositoy;
        public MatterProvision(IMatterRepository matterRepositoy, IOptions<MatterSettings> matterSettings, IOptions<ErrorSettings> errorSettings,
            ISPOAuthorization spoAuthorization, IEditFunctions editFunctions, IValidationFunctions validationFunctions,
            ICustomLogger customLogger, IOptions<LogTables> logTables, IOptions<MailSettings> mailSettings, IOptions<CamlQueries> camlQueries, IOptions<ListNames> listNames,
            IOptions<SearchSettings> searchSettings, IUserRepository userRepositoy
            )
        {
            this.matterRepositoy = matterRepositoy;
            this.matterSettings = matterSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.editFunctions = editFunctions;
            this.errorSettings = errorSettings.Value;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.validationFunctions = validationFunctions;
            this.mailSettings = mailSettings.Value;
            this.camlQueries = camlQueries.Value;
            this.listNames = listNames.Value;
            this.searchSettings = searchSettings.Value;
            this.userRepositoy = userRepositoy;
        }

        public async Task<int> GetAllCounts(SearchRequestVM searchRequestVM)
        {
            searchRequestVM.SearchObject.Filters.FilterByMe = 0;
            var searchObject = searchRequestVM.SearchObject;
            // Encode all fields which are coming from js
            SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
            // Encode Search Term
            searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES,
                ServiceConstants.DOUBLE_QUOTE) : string.Empty;

            var searchResultsVM = await matterRepositoy.GetMattersAsync(searchRequestVM);
            return searchResultsVM.TotalRows;
            
        }

        public async Task<int> GetMyCounts(SearchRequestVM searchRequestVM)
        {
            searchRequestVM.SearchObject.Filters.FilterByMe = 1;
            var searchObject = searchRequestVM.SearchObject;
            // Encode all fields which are coming from js
            SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
            // Encode Search Term
            searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES,
                ServiceConstants.DOUBLE_QUOTE) : string.Empty;

            var searchResultsVM = await matterRepositoy.GetMattersAsync(searchRequestVM);
            return searchResultsVM.TotalRows;
        }

        public async Task<int> GetPinnedCounts(Client client)
        {
            var pinResponseVM = await matterRepositoy.GetPinnedRecordsAsync(client);
            return pinResponseVM.TotalRows;
        }


        public async Task<SearchResponseVM> GetMatters(SearchRequestVM searchRequestVM)
        {
            var searchObject = searchRequestVM.SearchObject;
            // Encode all fields which are coming from js
            SearchUtility.EncodeSearchDetails(searchObject.Filters, false);
            // Encode Search Term
            searchObject.SearchTerm = (searchObject.SearchTerm != null) ?
                WebUtility.HtmlEncode(searchObject.SearchTerm).Replace(ServiceConstants.ENCODED_DOUBLE_QUOTES, 
                ServiceConstants.DOUBLE_QUOTE) : string.Empty;

            var searchResultsVM = await matterRepositoy.GetMattersAsync(searchRequestVM);
            if (searchResultsVM.TotalRows > 0)
            {
                IList<MatterData> matterDataList = new List<MatterData>();
                IEnumerable<IDictionary<string, object>> searchResults = searchResultsVM.SearchResults;
                foreach (var searchResult in searchResults)
                {
                    MatterData matterData = new MatterData();
                    foreach (var key in searchResult.Keys)
                    {
                        switch (key.ToLower())
                        {
                            case "mcmattername":
                                matterData.MatterName = searchResult[key].ToString();
                                break;
                            case "description":
                                matterData.MatterDescription = searchResult[key].ToString();
                                break;
                            case "mcopendate":
                                matterData.MatterCreatedDate = searchResult[key].ToString();
                                break;
                            case "path":
                                matterData.MatterUrl = searchResult[key].ToString();

                                break;
                            case "sitename":
                                matterData.MatterClientUrl = searchResult[key].ToString();
                                break;
                            case "mcpracticegroup":
                                matterData.MatterPracticeGroup = searchResult[key].ToString();
                                break;
                            case "mcareaoflaw":
                                matterData.MatterAreaOfLaw = searchResult[key].ToString();
                                break;
                            case "mcsubareaoflaw":
                                matterData.MatterSubAreaOfLaw = searchResult[key].ToString();
                                break;
                            case "mcclientname":
                                matterData.MatterClient = searchResult[key].ToString();
                                break;
                            case "mcclientid":
                                matterData.MatterClientId = searchResult[key].ToString();
                                break;
                            case "mcblockeduploaduser":
                                matterData.HideUpload = searchResult[key].ToString();
                                break;
                            case "mcmatterid":
                                matterData.MatterID = searchResult[key].ToString();
                                break;
                            case "mcresponsibleattorney":
                                matterData.MatterResponsibleAttorney = searchResult[key].ToString();
                                break;
                            case "lastmodifiedtime":
                                matterData.MatterModifiedDate = searchResult[key].ToString();
                                break;
                            case "mattercentermatterguid":
                                matterData.MatterGuid = searchResult[key].ToString();
                                break;
                        }
                        matterData.PinType = "Pin";
                    }
                    matterDataList.Add(matterData);
                }
                searchResultsVM.MatterDataList = matterDataList;                
            }
            searchResultsVM.SearchResults = null;
            return searchResultsVM;
        }

        public GenericResponseVM ShareMatterToExternalUser(ExternalSharingRequest externalSharingRequest)
        {
            return matterRepositoy.ShareMatterToExternalUser(externalSharingRequest);
        }

        public GenericResponseVM UpdateMatter(MatterInformationVM matterInformation)
        {
            var matter = matterInformation.Matter;
            var matterDetails = matterInformation.MatterDetails;
            var client = matterInformation.Client;
            int listItemId = -1;
            string loggedInUserName = "";
            bool isEditMode = matterInformation.EditMode;
            ClientContext clientContext = null;
            IEnumerable<RoleAssignment> userPermissionOnLibrary = null;
            GenericResponseVM genericResponse = null;
            try
            {                
                clientContext = spoAuthorization.GetClientContext(matterInformation.Client.Url);
                PropertyValues matterStampedProperties = matterRepositoy.GetStampedProperties(clientContext, matter.Name);
                loggedInUserName = userRepositoy.GetLoggedInUserDetails(clientContext).Name;
                bool isFullControlPresent = editFunctions.ValidateFullControlPermission(matter);
                
                if (!isFullControlPresent)
                {
                    return ServiceUtility.GenericResponse(errorSettings.IncorrectInputSelfPermissionRemoval, errorSettings.ErrorEditMatterMandatoryPermission);
                }               
                
                // Get matter library current permissions
                userPermissionOnLibrary = matterRepositoy.FetchUserPermissionForLibrary(clientContext, matter.Name);
                string originalMatterName = matterRepositoy.GetMatterName(clientContext, matter.Name);
                listItemId = matterRepositoy.RetrieveItemId(clientContext, matterSettings.MatterLandingPageRepositoryName, originalMatterName);
                List<string> usersToRemove = RetrieveMatterUsers(userPermissionOnLibrary);
                bool hasFullPermission = CheckFullPermissionInAssignList(matter.AssignUserNames, matter.Permissions, loggedInUserName);
                List<string> listExists = matterRepositoy.MatterAssociatedLists(clientContext, matter.Name);
                matterRepositoy.AssignRemoveFullControl(clientContext, matter, loggedInUserName, listItemId, listExists, true, hasFullPermission);
                bool result = false;
                if (listExists.Contains(matter.Name))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name, -1, isEditMode);
                }
                if (listExists.Contains(matter.Name + matterSettings.OneNoteLibrarySuffix))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + matterSettings.OneNoteLibrarySuffix, -1, isEditMode);
                }
                if (listExists.Contains(matter.Name + matterSettings.CalendarNameSuffix))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + matterSettings.CalendarNameSuffix, -1, isEditMode);
                }
                if (listExists.Contains(matter.Name + matterSettings.TaskNameSuffix))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + matterSettings.TaskNameSuffix, -1, isEditMode);
                }
                if (0 <= listItemId)
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, true, matterSettings.MatterLandingPageRepositoryName, listItemId, isEditMode);
                }
                // Update matter metadata
                result = matterRepositoy.UpdateMatterStampedProperties(clientContext, matterDetails, matter, matterStampedProperties, isEditMode);
                if(result)
                {
                    return genericResponse;
                }
            }
            catch(Exception ex)
            {
                MatterRevertList matterRevertListObject = new MatterRevertList()
                {
                    MatterLibrary = matter.Name,
                    MatterOneNoteLibrary = matter.Name + matterSettings.OneNoteLibrarySuffix,
                    MatterCalendar = matter.Name + matterSettings.CalendarNameSuffix,
                    MatterTask = matter.Name + matterSettings.TaskNameSuffix,
                    MatterSitePages = matterSettings.MatterLandingPageRepositoryName
                };
                matterRepositoy.RevertMatterUpdates(client, matter, clientContext, matterRevertListObject, loggedInUserName, 
                    userPermissionOnLibrary, listItemId, isEditMode);                
            }
            return ServiceUtility.GenericResponse("9999999", "Error in updating matter information");
        }


        

        public GenericResponseVM UpdateMatterMetadata(MatterMetdataVM matterMetadata)
        {
            var matter = matterMetadata.Matter;
            var matterDetails = matterMetadata.MatterDetails;
            var client = matterMetadata.Client;
            ClientContext clientContext = null;
            GenericResponseVM returnFlag = null;
            try
            {
                clientContext = spoAuthorization.GetClientContext(matterMetadata.Client.Url);
                PropertyValues matterStampedProperties = matterRepositoy.GetStampedProperties(clientContext, matter.Name);
                Dictionary<string, string> propertyList = SetStampProperty(client, matter, matterDetails);
                matterRepositoy.SetPropertBagValuesForList(clientContext, matterStampedProperties, matter.Name, propertyList);
                if (matterMetadata.MatterProvisionFlags.SendEmailFlag)
                {
                    returnFlag = ShareMatter(matterMetadata, matterMetadata.MatterProvisionFlags.MatterLandingFlag);
                }
                else
                {
                    ServiceUtility.GenericResponse("", "Matter Update Success");
                }
            }
            catch(Exception ex)
            {
                DeleteMatter(matterMetadata as MatterVM);
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return returnFlag;
        }

        public GenericResponseVM DeleteMatter(MatterVM matterVM)
        {
            var client = matterVM.Client;
            var matter = matterVM.Matter;
            GenericResponseVM genericResponse =   matterRepositoy.DeleteMatter(client, matter);
            return genericResponse;
        }

        public GenericResponseVM SavConfigurations(SaveConfigurationsVM saveConfigurationsVM)
        {
            ClientContext clientContext = null;
            GenericResponseVM returnFlag = null;
            try
            {
                clientContext = spoAuthorization.GetClientContext(saveConfigurationsVM.SiteCollectionPath);
                Matter matter = new Matter();
                matter.AssignUserNames = GetUserList(saveConfigurationsVM.MatterConfigurations.MatterUsers);
                matter.AssignUserEmails = GetUserList(saveConfigurationsVM.MatterConfigurations.MatterUserEmails);
                if (0 < matter.AssignUserNames.Count)
                {
                    returnFlag = matterRepositoy.ValidateTeamMembers(clientContext, matter, saveConfigurationsVM.UserId);
                }
                if (returnFlag!=null)
                {
                    returnFlag = matterRepositoy.SaveConfigurationToList(saveConfigurationsVM.MatterConfigurations, clientContext, saveConfigurationsVM.CachedItemModifiedDate);
                    bool tempResult = false;
                    if (returnFlag != null)
                    {
                        tempResult = bool.Parse(returnFlag.Value);
                        if (tempResult)
                        {
                            string listQuery = string.Format(CultureInfo.InvariantCulture, camlQueries.MatterConfigurationsListQuery, searchSettings.ManagedPropertyTitle, searchSettings.MatterConfigurationTitleValue);
                            ListItem settingsItem = matterRepositoy.GetItem(clientContext, listNames.MatterConfigurationsList, listQuery);
                            if (null != settingsItem)
                            {
                                saveConfigurationsVM.CachedItemModifiedDate = Convert.ToString(settingsItem[matterSettings.ColumnNameModifiedDate], CultureInfo.InvariantCulture);
                            }
                            returnFlag.Value = string.Concat(returnFlag.Value, ServiceConstants.PIPE, ServiceConstants.DOLLAR, ServiceConstants.PIPE, saveConfigurationsVM.CachedItemModifiedDate);

                        }
                    }
                }
                return returnFlag;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public PropertyValues GetStampedProperties(MatterVM matterVM)
        {
            var matter = matterVM.Matter;
            var client = matterVM.Client;
            ClientContext clientContext = null;
            PropertyValues matterStampedProperties = null;
            try
            {
                clientContext = spoAuthorization.GetClientContext(matterVM.Client.Url);
                matterStampedProperties = matterRepositoy.GetStampedProperties(clientContext, matter.Name);
                Dictionary<string, object> stampedPropertyValues = matterStampedProperties.FieldValues;
                if (stampedPropertyValues.Count>0)
                {
                    string matterCenterUsers = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterCenterUsers);
                    string matterCenterUserEmails = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterCenterUserEmails);
                    List<List<string>> matterCenterUserCollection = new List<List<string>>();
                    List<List<string>> matterCenterUserEmailsCollection = new List<List<string>>();
                    if (!string.IsNullOrWhiteSpace(matterCenterUsers))
                    {
                        matterCenterUserCollection = GetMatterAssignedUsers(matterCenterUsers);
                    }
                    if (!string.IsNullOrWhiteSpace(matterCenterUserEmails))
                    {
                        matterCenterUserEmailsCollection = GetMatterAssignedUsers(matterCenterUserEmails);
                    }

                    MatterStampedDetails matterStampedDetails = new MatterStampedDetails()
                    {
                        IsNewMatter = stampedPropertyValues.ContainsKey(matterSettings.StampedPropertyIsConflictIdentified) ? ServiceConstants.TRUE : ServiceConstants.FALSE,
                        MatterObject = new Matter()
                        {
                            Id = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterID),
                            Name = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterName),
                            Description = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterDescription),
                            DefaultContentType = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyDefaultContentType),
                            DocumentTemplateCount = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyDocumentTemplateCount).Split(new string[] { ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            Roles = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterCenterRoles).Split(new string[] { ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            Permissions = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterCenterPermissions).Split(new string[] { ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            BlockUserNames = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyBlockedUsers).Split(new string[] { ServiceConstants.SEMICOLON }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            AssignUserNames = matterCenterUserCollection.ToList<IList<string>>(),
                            AssignUserEmails = matterCenterUserEmailsCollection.ToList<IList<string>>(),
                            Conflict = new Conflict()
                            {
                                CheckBy = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyConflictCheckBy),
                                CheckOn = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyConflictCheckDate),
                                Identified = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyIsConflictIdentified),
                                SecureMatter = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertySecureMatter),
                            }
                        },
                        MatterDetailsObject = ExtractMatterDetails(stampedPropertyValues),
                        ClientObject = new Client()
                        {
                            Id = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyClientID),
                            Name = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyClientName),
                        }
                    };

                }
            }
            catch (Exception ex)
            {
               
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return matterStampedProperties;
        }

        public GenericResponseVM AssignUserPermissions(MatterMetdataVM matterMetadataVM)
        {
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            var matterConfigurations = matterMetadataVM.MatterConfigurations;
            ClientContext clientContext = null;
            string calendarName = string.Concat(matter.Name, matterSettings.CalendarNameSuffix);
            string oneNoteLibraryName = string.Concat(matter.Name, matterSettings.OneNoteLibrarySuffix);
            string taskLibraryName = string.Concat(matter.Name, matterSettings.TaskNameSuffix);
            GenericResponseVM genericResponseVM = null;
            using (clientContext = spoAuthorization.GetClientContext(client.Url))
            {
                MatterInformationVM matterInfo = new MatterInformationVM()
                {
                    Client = matterMetadataVM.Client,
                    Matter = matterMetadataVM.Matter,
                    MatterDetails = matterMetadataVM.MatterDetails
                };
                genericResponseVM = validationFunctions.IsMatterValid(matterInfo, 
                    int.Parse(ServiceConstants.PROVISION_MATTER_ASSIGN_USER_PERMISSIONS, CultureInfo.InvariantCulture), null);

                if(genericResponseVM!=null)
                {
                    DeleteMatter(matterMetadataVM as MatterVM);
                    return genericResponseVM;
                }
                if (!string.IsNullOrWhiteSpace(matter.Name))
                {
                    //Assign permission for Matter library
                    matterRepositoy.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, matter.Name);
                    //Assign permission for OneNote library 
                    matterRepositoy.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, oneNoteLibraryName);
                    if (matterSettings.IsCreateCalendarEnabled && matterConfigurations.IsCalendarSelected)
                    {
                        bool returnValueCalendar = matterRepositoy.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, calendarName);
                        if (!returnValueCalendar)
                        {
                            genericResponseVM =
                                new GenericResponseVM() {
                                    Code = errorSettings.ErrorCodeCalendarCreation,
                                    Value = errorSettings.ErrorMessageCalendarCreation,
                                    IsError = true
                                };
                            return genericResponseVM; 
                        }
                    }

                    // Assign permission to task list if it is selected
                    if (matterConfigurations.IsTaskSelected)
                    {
                        bool returnValueTask = matterRepositoy.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, taskLibraryName);
                        if (!returnValueTask)
                        {
                            
                            genericResponseVM =
                                new GenericResponseVM()
                                {
                                    Code = errorSettings.ErrorMessageTaskCreation,
                                    Value = errorSettings.ErrorCodeAddTaskList,
                                    IsError = true
                                };
                            return genericResponseVM;
                        }
                    }
                }
            }
            return genericResponseVM;
        }

        public GenericResponseVM AssignContentType(MatterMetadata matterMetadata)
        {
            try
            {
                var client = matterMetadata.Client;
                var matter = matterMetadata.Matter;
                ClientContext clientContext = null;
                GenericResponseVM genericResponseVM = null;
                using (clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    IList<ContentType> contentTypeCollection = matterRepositoy.GetContentTypeData(clientContext, matter.ContentTypes, client, matter);
                    if (null != contentTypeCollection && matter.ContentTypes.Count == contentTypeCollection.Count && !string.IsNullOrWhiteSpace(matter.Name))
                    {
                        genericResponseVM = matterRepositoy.AssignContentTypeHelper(matterMetadata, clientContext, contentTypeCollection, client, matter);
                    }
                    else
                    {
                        genericResponseVM =
                                    new GenericResponseVM()
                                    {
                                        Code = errorSettings.ErrorCodeContentTypes,
                                        Value = errorSettings.ErrorMessageContentTypes,
                                        IsError = true
                                    };
                        
                    }
                    return genericResponseVM;
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

        }

        public GenericResponseVM CheckMatterExists(MatterMetdataVM matterMetadataVM)
        {
            var matter = matterMetadataVM.Matter;
            var client = matterMetadataVM.Client;
            GenericResponseVM genericResponse = null;
            using (ClientContext clientContext = spoAuthorization.GetClientContext(matterMetadataVM.Client.Url))
            {
                List<string> listExists = validationFunctions.CheckListExists(matterMetadataVM.Client, matterMetadataVM.Matter.Name, matterMetadataVM.MatterConfigurations);
                if (listExists.Count>0)
                {
                    string listName = !string.Equals(matter.Name, listExists[0]) ? listExists[0].Contains(ServiceConstants.UNDER_SCORE) ? 
                        listExists[0].Split(ServiceConstants.UNDER_SCORE[0]).Last() : ServiceConstants.MATTER : ServiceConstants.MATTER;
                    return ServiceUtility.GenericResponse(errorSettings.MatterLibraryExistsCode,
                        string.Format(CultureInfo.InvariantCulture, errorSettings.ErrorDuplicateMatter, listName) + ServiceConstants.DOLLAR + 
                        ServiceConstants.PIPE + ServiceConstants.DOLLAR + MatterPrerequisiteCheck.LibraryExists);
                }
                else
                {
                    Uri clientUri = new Uri(client.Url);
                    string requestedUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", clientUri.AbsolutePath, ServiceConstants.FORWARD_SLASH, 
                        matterSettings.MatterLandingPageRepositoryName.Replace(ServiceConstants.SPACE, string.Empty),
                        ServiceConstants.FORWARD_SLASH, matter.Name, ServiceConstants.ASPX_EXTENSION);
                    if (matterRepositoy.IsPageExists(clientContext, requestedUrl))
                    {                        
                        return ServiceUtility.GenericResponse(errorSettings.MatterLibraryExistsCode,
                        errorSettings.ErrorDuplicateMatterLandingPage + ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR + MatterPrerequisiteCheck.MatterLandingPageExists);
                    }
                }
            }
            return genericResponse;
        }

        public GenericResponseVM CheckSecurityGroupExists(MatterInformationVM matterInformationVM)
        {
            GenericResponseVM genericResponseVM = null;
            var matter = matterInformationVM.Matter;
            var client = matterInformationVM.Client;
            var userids = matterInformationVM.UserIds;
            using (ClientContext clientContext = spoAuthorization.GetClientContext(matterInformationVM.Client.Url))
            {
                genericResponseVM = matterRepositoy.ValidateTeamMembers(clientContext, matter, userids);
                if(genericResponseVM!=null)
                {
                    return genericResponseVM;
                }
                else
                {
                    if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                    {
                        if (0 == matter.AssignUserEmails.Count())
                        {                            
                            return ServiceUtility.GenericResponse(errorSettings.IncorrectInputUserNamesCode, errorSettings.IncorrectInputUserNamesMessage);
                        }
                        else
                        {
                            if (Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture))
                            {
                                return editFunctions.CheckSecurityGroupInTeamMembers(client, matter, userids);
                            }
                        }
                    }
                    else
                    {                        
                        return ServiceUtility.GenericResponse(errorSettings.IncorrectInputConflictIdentifiedCode, errorSettings.IncorrectInputConflictIdentifiedMessage);
                    }
                }
            }
            return genericResponseVM;
        }

        public GenericResponseVM CreateMatter(MatterMetdataVM matterMetadataVM)
        {
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            var matterConfiguration = matterMetadataVM.MatterConfigurations;
            GenericResponseVM genericResponseVM = null;
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    if (!matterRepositoy.CheckPermissionOnList(clientContext, listNames.MatterConfigurationsList, PermissionKind.EditListItems))
                    {
                        return ServiceUtility.GenericResponse(errorSettings.UserNotSiteOwnerCode, errorSettings.UserNotSiteOwnerMessage);
                    }
                    var matterInformation = new MatterInformationVM()
                    {
                        Client = client,
                        Matter = matter
                    };

                    genericResponseVM = validationFunctions.IsMatterValid(matterInformation, int.Parse(ServiceConstants.PROVISION_MATTER_CREATEMATTER), matterConfiguration);

                    if (genericResponseVM != null)
                    {
                        return ServiceUtility.GenericResponse(genericResponseVM.Code, genericResponseVM.Value);
                    }

                    genericResponseVM = CheckMatterExists(matterMetadataVM);

                    if (genericResponseVM != null)
                    {
                        return ServiceUtility.GenericResponse(genericResponseVM.Code, genericResponseVM.Value);
                    }

                    if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                    {
                        if (Convert.ToBoolean(matter.Conflict.Identified, CultureInfo.InvariantCulture))
                        {
                            genericResponseVM = editFunctions.CheckSecurityGroupInTeamMembers(client, matter, matterMetadataVM.UserIds);
                            if (genericResponseVM != null)
                            {
                                return ServiceUtility.GenericResponse(errorSettings.IncorrectInputConflictIdentifiedCode, errorSettings.IncorrectInputConflictIdentifiedMessage);
                            }
                        }

                        genericResponseVM = CreateMatter(clientContext, matterMetadataVM);

                    }
                }
                return genericResponseVM;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public GenericResponseVM CreateMatterLandingPage(MatterMetdataVM matterMetadataVM)
        {
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            var matterConfigurations = matterMetadataVM.MatterConfigurations;
            GenericResponseVM genericResponseVM = null;
            int matterLandingPageId;
            var matterInformation = new MatterInformationVM()
            {
                Client = client,
                Matter = matter
            };

            genericResponseVM = validationFunctions.IsMatterValid(matterInformation, int.Parse(ServiceConstants.PROVISIONMATTER_MATTER_LANDING_PAGE), matterConfigurations);

            if (genericResponseVM != null)
            {
                return ServiceUtility.GenericResponse(genericResponseVM.Code, genericResponseVM.Value);
            }
            //// Create Matter Landing Web Part Page
            string pageName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", matter.MatterGuid, ServiceConstants.ASPX_EXTENSION);
            using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
            {
                matterLandingPageId = matterRepositoy.CreateWebPartPage(clientContext, pageName, ServiceConstants.DefaultLayout, 
                    ServiceConstants.MasterPageGallery, matterSettings.MatterLandingPageRepositoryName, matter.Name);
                if (0 <= matterLandingPageId)
                {
                    Uri uri = new Uri(client.Url);
                    SharePoint.Client.Web web = clientContext.Web;

                    bool isCopyRoleAssignment = CopyRoleAssignment(matter.Conflict.Identified, matter.Conflict.SecureMatter);
                    matterRepositoy.BreakItemPermission(clientContext, matterSettings.MatterLandingPageRepositoryName, matterLandingPageId, isCopyRoleAssignment);
                    matterRepositoy.SetItemPermission(clientContext, matter.AssignUserEmails, matterSettings.MatterLandingPageRepositoryName, 
                        matterLandingPageId, matter.Permissions);
                    //// Configure All Web Parts
                    string[] webParts = matterRepositoy.ConfigureXMLCodeOfWebParts(client, matter, clientContext, 
                        pageName, uri, web, matterConfigurations);
                    Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, ServiceConstants.FORWARD_SLASH, matterSettings.MatterLandingPageRepositoryName.Replace(ServiceConstants.SPACE, string.Empty), ServiceConstants.FORWARD_SLASH, pageName));
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                    WebPartDefinition webPartDefinition = null;
                    string[] zones = { ServiceConstants.HEADER_ZONE, ServiceConstants.TOP_ZONE, ServiceConstants.RIGHT_ZONE, ServiceConstants.TOP_ZONE,
                        ServiceConstants.RIGHT_ZONE, ServiceConstants.RIGHT_ZONE, ServiceConstants.FOOTER_ZONE,
                        ServiceConstants.RIGHT_ZONE, ServiceConstants.RIGHT_ZONE };
                    matterRepositoy.AddWebPart(clientContext, limitedWebPartManager, webPartDefinition, webParts, zones);
                    return genericResponseVM;
                }
                else
                {
                    return ServiceUtility.GenericResponse(errorSettings.ErrorCodeMatterLandingPageExists, errorSettings.ErrorCodeMatterLandingPageExists);
                }
            }
        }

        #region private functions

        private GenericResponseVM CreateMatter(ClientContext clientContext, MatterMetdataVM matterMetadataVM)
        {
            var client = matterMetadataVM.Client;
            var matter = matterMetadataVM.Matter;
            GenericResponseVM genericResponseVM = null;
            try
            {                
                var matterConfiguration = matterMetadataVM.MatterConfigurations;                
                Uri centralListURL = new Uri(string.Concat(matterSettings.CentralRepositoryUrl, ServiceConstants.FORWARD_SLASH, 
                    ServiceConstants.LISTS, ServiceConstants.FORWARD_SLASH, listNames.DMSMatterListName)); // Central Repository List URL  
                IList<string> documentLibraryFolders = new List<string>();
                Dictionary<string, bool> documentLibraryVersioning = new Dictionary<string, bool>();
                Uri clientUrl = new Uri(client.Url);

                ListInformation listInformation = new ListInformation();
                listInformation.name = matter.Name;
                listInformation.description = matter.Description;
                listInformation.folderNames = matter.FolderNames;
                listInformation.isContentTypeEnable = true;
                listInformation.versioning = new VersioningInfo();
                listInformation.versioning.EnableVersioning = matterSettings.IsMajorVersionEnable;
                listInformation.versioning.EnableMinorVersions = matterSettings.IsMinorVersionEnable;
                listInformation.versioning.ForceCheckout = matterSettings.IsForceCheckOut;
                listInformation.Path = matter.MatterGuid;

                matterRepositoy.CreateList(clientContext, listInformation);

                documentLibraryVersioning.Add("EnableVersioning", false);
                documentLibraryFolders.Add(matter.MatterGuid);
                listInformation.name = matter.Name + matterSettings.OneNoteLibrarySuffix;
                listInformation.folderNames = documentLibraryFolders;
                listInformation.versioning.EnableVersioning = false;
                listInformation.versioning.EnableMinorVersions = false;
                listInformation.versioning.ForceCheckout = false;
                listInformation.Path = matter.MatterGuid + matterSettings.OneNoteLibrarySuffix;
                matterRepositoy.CreateList(clientContext, listInformation);

                bool isCopyRoleAssignment = CopyRoleAssignment(matter.Conflict.Identified, matter.Conflict.SecureMatter);
                //create calendar list if create calendar flag is enabled and break its permissions
                string calendarName = string.Concat(matter.Name, matterSettings.CalendarNameSuffix);
                string taskListName = string.Concat(matter.Name, matterSettings.TaskNameSuffix);

                if (matterSettings.IsCreateCalendarEnabled && matterConfiguration.IsCalendarSelected)
                {
                    ListInformation calendarInformation = new ListInformation();
                    calendarInformation.name = calendarName;
                    calendarInformation.isContentTypeEnable = false;
                    calendarInformation.templateType = ServiceConstants.CALENDAR_NAME;
                    calendarInformation.Path = matterSettings.TitleListsPath + matter.MatterGuid + matterSettings.CalendarNameSuffix;

                    if (matterRepositoy.CreateList(clientContext, calendarInformation))
                    {
                        matterRepositoy.BreakPermission(clientContext, calendarName, isCopyRoleAssignment);
                    }
                    else
                    {                        
                        return ServiceUtility.GenericResponse(errorSettings.ErrorCodeAddCalendarList, errorSettings.ErrorMessageAddCalendarList);
                    }
                }

                if (matterConfiguration.IsTaskSelected)
                {
                    ListInformation taskListInformation = new ListInformation();
                    taskListInformation.name = taskListName;
                    taskListInformation.isContentTypeEnable = false;
                    taskListInformation.templateType = ServiceConstants.TASK_LIST_TEMPLATE_TYPE;
                    taskListInformation.Path = matterSettings.TitleListsPath + matter.MatterGuid + matterSettings.TaskNameSuffix;
                    if (matterRepositoy.CreateList(clientContext, taskListInformation))
                    {
                        matterRepositoy.BreakPermission(clientContext, taskListName, isCopyRoleAssignment);
                    }
                    else
                    {                        
                        return ServiceUtility.GenericResponse(errorSettings.ErrorCodeAddTaskList, errorSettings.ErrorMessageAddTaskList);
                    }
                }

                string oneNoteUrl = string.Concat(clientUrl.AbsolutePath, ServiceConstants.FORWARD_SLASH, 
                    matter.MatterGuid, matterSettings.OneNoteLibrarySuffix, ServiceConstants.FORWARD_SLASH, matter.MatterGuid);
                matterRepositoy.AddOneNote(clientContext, client.Url, oneNoteUrl, matter.MatterGuid, matter.Name);
                if (null != matter.Conflict)
                {
                    //Break permission for Matter library
                    matterRepositoy.BreakPermission(clientContext, matter.Name, isCopyRoleAssignment);

                    //Break permission for OneNote document library
                    string oneNoteLibraryName = string.Concat(matter.Name, matterSettings.OneNoteLibrarySuffix);
                    matterRepositoy.BreakPermission(clientContext, oneNoteLibraryName, isCopyRoleAssignment);
                }

                genericResponseVM = validationFunctions.RoleCheck(matter);
                if(genericResponseVM==null)
                {
                    string centralList = Convert.ToString(centralListURL, CultureInfo.InvariantCulture);
                    string matterSiteURL = centralList.Substring(0, centralList.LastIndexOf(string.Concat(ServiceConstants.FORWARD_SLASH, 
                        ServiceConstants.LISTS, ServiceConstants.FORWARD_SLASH), StringComparison.OrdinalIgnoreCase));
                    string matterListName = centralList.Substring(centralList.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);

                    
                    bool isMatterSaved = matterRepositoy.SaveMatter(client, matter, matterListName, matterConfiguration, matterSiteURL);
                    if(isMatterSaved==false)
                    {
                        genericResponseVM = ServiceUtility.GenericResponse(errorSettings.ErrorCodeAddTaskList, "Matter Not Saved");
                        genericResponseVM.IsError = true;                        
                        return genericResponseVM;
                    }
                }
                genericResponseVM = new GenericResponseVM()
                {
                    Code = HttpStatusCode.OK.ToString(),
                    Value = client.Url,
                    IsError = false
                };
                return genericResponseVM;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Checks whether to retain previous users while breaking permission
        /// </summary>
        /// <param name="conflictIdentified">Conflict identified information</param>
        /// <param name="matterSecured">Security information</param>
        /// <returns>Flag to indicate whether to retain the previous users</returns>
        internal static bool CopyRoleAssignment(string conflictIdentified, string matterSecured)
        {
            bool isBreakPermission = true;
            if (Convert.ToBoolean(conflictIdentified, CultureInfo.InvariantCulture) || Convert.ToBoolean(matterSecured, CultureInfo.InvariantCulture))
            {
                isBreakPermission = false;
            }
            return isBreakPermission;
        }

        /// <summary>
        /// Extracts matter details from matter library property bag.
        /// </summary>
        /// <param name="stampedPropertyValues">Dictionary object containing matter property bag key/value pairs</param>
        /// <returns>Matter details from matter library property bag</returns>
        internal MatterDetails ExtractMatterDetails(Dictionary<string, object> stampedPropertyValues)
        {
            MatterDetails matterDetails = new MatterDetails()
            {
                PracticeGroup = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyPracticeGroup),
                AreaOfLaw = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyAreaOfLaw),
                SubareaOfLaw = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertySubAreaOfLaw),
                ResponsibleAttorney = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyResponsibleAttorney),
                TeamMembers = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyTeamMembers),
                UploadBlockedUsers = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyBlockedUploadUsers).Split(new string[] { ServiceConstants.SEMICOLON }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };
            return matterDetails;
        }

        /// <summary>
        /// Retrieves the users assigned to matter.
        /// </summary>
        /// <param name="matterCenterUsers">Users tagged with matter in property bag</param>
        /// <returns>Users assigned to matter</returns>
        internal List<List<string>> GetMatterAssignedUsers(string matterCenterUsers)
        {
            List<List<string>> matterCenterUserCollection = new List<List<string>>();

            if (!string.IsNullOrWhiteSpace(matterCenterUsers))
            {
                List<string> userCollection = matterCenterUsers.Split(new string[] { ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string userRow in userCollection)
                {
                    List<string> users = userRow.Split(new string[] { ServiceConstants.SEMICOLON }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    matterCenterUserCollection.Add(users);
                }
            }
            return matterCenterUserCollection;
        }


        /// <summary>
        /// Checks if the property exists in property bag. Returns the value for the property from property bag.
        /// </summary>
        /// <param name="stampedPropertyValues">Dictionary object containing matter property bag key/value pairs</param>
        /// <param name="key">Key to check in dictionary</param>
        /// <returns>Property bag value for </returns>
        internal string GetStampPropertyValue(Dictionary<string, object> stampedPropertyValues, string key)
        {
            string result = string.Empty;
            if (stampedPropertyValues.ContainsKey(key))
            {
                result = WebUtility.HtmlDecode(Convert.ToString(stampedPropertyValues[key], CultureInfo.InvariantCulture));
            }

            // This is just to check for null value in key, if exists
            return (!string.IsNullOrWhiteSpace(result)) ? result : string.Empty;
        }

        /// <summary>
        /// Gets the user list from the data sent in Matter Configurations
        /// </summary>
        /// <param name="matterUsers">Matter Users list</param>
        /// <returns>Users list</returns>
        internal IList<IList<string>> GetUserList(string matterUsers)
        {
            IList<IList<string>> result = new List<IList<string>>();
            IList<string> temp = null;
            string[] resultTemp = matterUsers.Split(new string[] { "$|$" }, StringSplitOptions.None);
            foreach (var userItem in resultTemp)
            {
                temp = new List<string>();
                string[] tempItems = userItem.Split(Convert.ToChar(ServiceConstants.SEMICOLON, CultureInfo.InvariantCulture));
                foreach (var item in tempItems)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        temp.Add(item.Trim());
                    }
                }
                if (0 < temp.Count)
                {
                    result.Add(temp);
                }
                temp = null;
            }
            return result;
        }

        /// <summary>
        /// Creates an item in the specific list with the list of users to whom the matter will be shared.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>true if success else false</returns>
        /// /// <summary>
        internal GenericResponseVM ShareMatter(MatterMetdataVM matterMetadata, string matterLandingFlag)
        {
            GenericResponseVM returnFlag = null;
            var matter = matterMetadata.Matter;
            var matterDetails = matterMetadata.MatterDetails;
            var client = matterMetadata.Client;
            var matterConfigurations = matterMetadata.MatterConfigurations;
            if (null != client && null != matter && null != matterDetails)
            {
                try
                {
                    Uri mailListURL = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", matterSettings.ProvisionMatterAppURL, 
                        ServiceConstants.FORWARD_SLASH, ServiceConstants.LISTS, ServiceConstants.FORWARD_SLASH, matterSettings.SendMailListName));
                    string centralMailListURL = Convert.ToString(mailListURL, CultureInfo.InvariantCulture);
                    string mailSiteURL = centralMailListURL.Substring(0, centralMailListURL.LastIndexOf(string.Concat(ServiceConstants.FORWARD_SLASH,
                        ServiceConstants.LISTS, ServiceConstants.FORWARD_SLASH), StringComparison.OrdinalIgnoreCase));
                    ///// Retrieve the specific site where the Mail List is present along with the required List Name
                    if (null != mailListURL && null != client.Url)
                    {
                        if (!string.IsNullOrWhiteSpace(mailSiteURL))
                        {
                            returnFlag = ShareMatterUtility(client, matter, matterDetails, 
                                mailSiteURL, centralMailListURL, matterLandingFlag, matterConfigurations);
                        }
                    }
                }
                catch (Exception ex)
                {
                    customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
            return returnFlag;
        }

        /// <summary>
        /// Function to share the matter.
        /// </summary>
        /// <param name="requestObject">Request Object containing SharePoint App Token</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="subAreaOfLawList">String contains all sub area of law</param>
        /// <param name="mailListURL">URL contains list of mail recipients</param>
        /// <returns>Result of operation: Matter Shared successfully or not</returns>        
        internal GenericResponseVM  ShareMatterUtility(Client client, Matter matter, MatterDetails matterDetails, string mailSiteURL, string centralMailListURL, string matterLandingFlag, MatterConfigurations matterConfigurations)
        {
            bool shareFlag = false;
            string mailListName = centralMailListURL.Substring(centralMailListURL.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);
            string matterLocation = string.Concat(client.Url, ServiceConstants.FORWARD_SLASH, matter.Name);
            string ProvisionMatterValidation = string.Empty;
            GenericResponseVM genericResponse = null;
            if (!string.IsNullOrWhiteSpace(mailSiteURL))
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(mailSiteURL))
                {
                    
                    genericResponse = validationFunctions.MatterDetailsValidation(matter, client, 
                        int.Parse(ServiceConstants.ProvisionMatterShareMatter, CultureInfo.InvariantCulture), matterConfigurations);
                    if(genericResponse!=null)
                    {
                        return genericResponse;
                    }
                    
                    // Get the current logged in User
                    clientContext.Load(clientContext.Web.CurrentUser);
                    clientContext.ExecuteQuery();
                    string matterMailBody, blockUserNames;
                    // Generate Mail Subject
                    string matterMailSubject = string.Format(CultureInfo.InvariantCulture, mailSettings.MatterMailSubject, 
                        matter.Id, matter.Name, clientContext.Web.CurrentUser.Title);

                    // Logic to Create Mail body
                    // Step 1: Create Matter Information
                    // Step 2: Create Team Information
                    // Step 3: Create Access Information
                    // Step 4: Create Conflict check Information based on the conflict check flag and create mail body

                    // Step 1: Create Matter Information
                    string defaultContentType = string.Format(CultureInfo.InvariantCulture, 
                        mailSettings.MatterMailDefaultContentTypeHtmlChunk, matter.DefaultContentType);
                    string matterType = string.Join(";", matter.ContentTypes.ToArray()).TrimEnd(';').Replace(matter.DefaultContentType, defaultContentType);

                    // Step 2: Create Team Information
                    string secureMatter = ServiceConstants.FALSE.ToUpperInvariant() == matter.Conflict.SecureMatter.ToUpperInvariant() ?
                        ServiceConstants.NO : ServiceConstants.YES;
                    string mailBodyTeamInformation = string.Empty;
                    mailBodyTeamInformation = TeamMembersPermissionInformation(matterDetails, mailBodyTeamInformation);

                    // Step 3: Create Access Information
                    if (ServiceConstants.TRUE == matterLandingFlag)
                    {
                        matterLocation = string.Concat(client.Url, ServiceConstants.FORWARD_SLASH, 
                            matterSettings.MatterLandingPageRepositoryName.Replace(ServiceConstants.SPACE, string.Empty), 
                            ServiceConstants.FORWARD_SLASH, matter.MatterGuid, ServiceConstants.ASPX_EXTENSION);
                    }
                    string oneNotePath = string.Concat(client.Url, ServiceConstants.FORWARD_SLASH, 
                        matter.MatterGuid, matterSettings.OneNoteLibrarySuffix, 
                        ServiceConstants.FORWARD_SLASH, matter.MatterGuid, ServiceConstants.FORWARD_SLASH, matter.MatterGuid);

                    // Step 4: Create Conflict check Information based on the conflict check flag and create mail body
                    if (matterConfigurations.IsConflictCheck)
                    {
                        string conflictIdentified = ServiceConstants.FALSE.ToUpperInvariant() == matter.Conflict.Identified.ToUpperInvariant() ?
                        ServiceConstants.NO : ServiceConstants.YES;
                        blockUserNames = string.Join(";", matter.BlockUserNames.ToArray()).Trim().TrimEnd(';');

                        blockUserNames = !String.IsNullOrEmpty(blockUserNames) ? string.Format(CultureInfo.InvariantCulture, 
                            "<div>{0}: {1}</div>", "Conflicted User", blockUserNames) : string.Empty;
                        matterMailBody = string.Format(CultureInfo.InvariantCulture, 
                            mailSettings.MatterMailBodyMatterInformation, client.Name, client.Id, 
                            matter.Name, matter.Id, matter.Description, matterType) + string.Format(CultureInfo.InvariantCulture, 
                            mailSettings.MatterMailBodyConflictCheck, ServiceConstants.YES, matter.Conflict.CheckBy, 
                            Convert.ToDateTime(matter.Conflict.CheckOn, CultureInfo.InvariantCulture).ToString(matterSettings.MatterCenterDateFormat, CultureInfo.InvariantCulture), 
                            conflictIdentified) + string.Format(CultureInfo.InvariantCulture, 
                            mailSettings.MatterMailBodyTeamMembers, secureMatter, mailBodyTeamInformation, 
                            blockUserNames, client.Url, oneNotePath, matter.Name, matterLocation, matter.Name);
                    }
                    else
                    {
                        blockUserNames = string.Empty;
                        matterMailBody = string.Format(CultureInfo.InvariantCulture, mailSettings.MatterMailBodyMatterInformation, 
                            client.Name, client.Id, matter.Name, matter.Id, 
                            matter.Description, matterType) + string.Format(CultureInfo.InvariantCulture, mailSettings.MatterMailBodyTeamMembers, secureMatter, 
                            mailBodyTeamInformation, blockUserNames, client.Url, oneNotePath, matter.Name, matterLocation, matter.Name);
                    }

                    Microsoft.SharePoint.Client.Web web = clientContext.Web;
                    List mailList = web.Lists.GetByTitle(mailListName);
                    List<FieldUserValue> userList = new List<FieldUserValue>();
                    List<FieldUserValue> userEmailList = GenerateMailList(matter, new Client {Url = mailSiteURL }, ref userList);
                    ///// Add the Matter URL in list
                    FieldUrlValue matterPath = new FieldUrlValue()
                    {
                        Url = string.Concat(client.Url.Replace(String.Concat(ServiceConstants.HTTPS, ServiceConstants.COLON, 
                        ServiceConstants.FORWARD_SLASH, ServiceConstants.FORWARD_SLASH), String.Concat(ServiceConstants.HTTP, ServiceConstants.COLON, 
                        ServiceConstants.FORWARD_SLASH, ServiceConstants.FORWARD_SLASH)), ServiceConstants.FORWARD_SLASH, matter.Name, 
                        ServiceConstants.FORWARD_SLASH, matter.Name),
                        Description = matter.Name
                    };
                    List<string> columnNames = new List<string>() { matterSettings.ShareListColumnMatterPath, matterSettings.ShareListColumnMailList,
                        mailSettings.ShareListColumnMailBody, mailSettings.ShareListColumnMailSubject };
                    List<object> columnValues = new List<object>() { matterPath, userEmailList, matterMailBody, matterMailSubject };
                    // To avoid the invalid symbol error while parsing the JSON, return the response in lower case 
                    matterRepositoy.AddItem(clientContext, mailList, columnNames, columnValues);
                    
                }
            }
            return genericResponse;
        }


        /// <summary>
        /// Generates list of users for sending email.
        /// </summary>
        /// <param name="matter">Matter details</param>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="userList">List of users associated with the matter</param>
        /// <returns>List of users to whom mail is to be sent</returns>
        internal List<FieldUserValue> GenerateMailList(Matter matter, Client client, ref List<FieldUserValue> userList)
        {
            List<FieldUserValue> result = null;
            try
            {
                List<FieldUserValue> userEmailList = new List<FieldUserValue>();
                if (null != matter.AssignUserNames)
                {
                    foreach (IList<string> userNames in matter.AssignUserNames)
                    {
                        userList = matterRepositoy.ResolveUserNames(client, userNames).ToList();
                        foreach (FieldUserValue userEmail in userList)
                        {
                            userEmailList.Add(userEmail);
                        }
                    }
                }
                result = userEmailList;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Provides the team members and their respective permission details.
        /// </summary>
        /// <param name="matterDetails">Matter Details object</param>
        /// <param name="mailBodyTeamInformation">Team members permission information</param>
        /// <returns>Team members permission information</returns>
        private static string TeamMembersPermissionInformation(MatterDetails matterDetails, string mailBodyTeamInformation)
        {
            if (null != matterDetails && !string.IsNullOrWhiteSpace(matterDetails.RoleInformation))
            {
                Dictionary<string, string> roleInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(matterDetails.RoleInformation);

                foreach (KeyValuePair<string, string> entry in roleInformation)
                {
                    mailBodyTeamInformation = string.Format(CultureInfo.InvariantCulture, ServiceConstants.RoleInfoHtmlChunk, entry.Key, entry.Value) + 
                        mailBodyTeamInformation;
                }
            }
            return mailBodyTeamInformation;
        }

        /// <summary>
        /// Function to create dictionary object for stamp property 
        /// </summary>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="matterDetails">Matter details object which has data of properties to be stamped</param>
        /// <returns>returns dictionary object</returns>
        internal Dictionary<string, string> SetStampProperty(Client client, Matter matter, MatterDetails matterDetails)
        {
            string matterCenterPermission = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.Permissions);
            string matterCenterRoles = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.Roles);
            string documentTemplateCount = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.DocumentTemplateCount);
            string matterCenterUsers = string.Empty;
            string separator = string.Empty;
            foreach (IList<string> userNames in matter.AssignUserNames)
            {
                matterCenterUsers += separator + string.Join(ServiceConstants.SEMICOLON, userNames.Where(user => !string.IsNullOrWhiteSpace(user)));
                separator = ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR;
            }
            List<string> keys = new List<string>();
            Dictionary<string, string> propertyList = new Dictionary<string, string>();
            keys.Add(matterSettings.StampedPropertyPracticeGroup);
            keys.Add(matterSettings.StampedPropertyAreaOfLaw);
            keys.Add(matterSettings.StampedPropertySubAreaOfLaw);
            keys.Add(matterSettings.StampedPropertyMatterName);
            keys.Add(matterSettings.StampedPropertyMatterID);
            keys.Add(matterSettings.StampedPropertyClientName);
            keys.Add(matterSettings.StampedPropertyClientID);
            keys.Add(matterSettings.StampedPropertyResponsibleAttorney);
            keys.Add(matterSettings.StampedPropertyTeamMembers);
            keys.Add(matterSettings.StampedPropertyIsMatter);
            keys.Add(matterSettings.StampedPropertyOpenDate);
            keys.Add(matterSettings.StampedPropertySecureMatter);
            keys.Add(matterSettings.StampedPropertyBlockedUploadUsers);
            keys.Add(matterSettings.StampedPropertyMatterDescription);
            keys.Add(matterSettings.StampedPropertyConflictCheckDate);
            keys.Add(matterSettings.StampedPropertyConflictCheckBy);
            keys.Add(matterSettings.StampedPropertyMatterCenterRoles);
            keys.Add(matterSettings.StampedPropertyMatterCenterPermissions);
            keys.Add(matterSettings.StampedPropertyMatterCenterUsers);
            keys.Add(matterSettings.StampedPropertyDefaultContentType);
            keys.Add(matterSettings.StampedPropertyIsConflictIdentified);
            keys.Add(matterSettings.StampedPropertyDocumentTemplateCount);
            keys.Add(matterSettings.StampedPropertyBlockedUsers);
            keys.Add(matterSettings.StampedPropertyMatterGUID);

            propertyList.Add(matterSettings.StampedPropertyPracticeGroup, WebUtility.HtmlEncode(matterDetails.PracticeGroup));
            propertyList.Add(matterSettings.StampedPropertyAreaOfLaw, WebUtility.HtmlEncode(matterDetails.AreaOfLaw));
            propertyList.Add(matterSettings.StampedPropertySubAreaOfLaw, WebUtility.HtmlEncode(matterDetails.SubareaOfLaw));
            propertyList.Add(matterSettings.StampedPropertyMatterName, WebUtility.HtmlEncode(matter.Name));
            propertyList.Add(matterSettings.StampedPropertyMatterID, WebUtility.HtmlEncode(matter.Id));
            propertyList.Add(matterSettings.StampedPropertyClientName, WebUtility.HtmlEncode(client.Name));
            propertyList.Add(matterSettings.StampedPropertyClientID, WebUtility.HtmlEncode(client.Id));
            propertyList.Add(matterSettings.StampedPropertyResponsibleAttorney, WebUtility.HtmlEncode(matterDetails.ResponsibleAttorney));
            propertyList.Add(matterSettings.StampedPropertyTeamMembers, WebUtility.HtmlEncode(matterDetails.TeamMembers));
            propertyList.Add(matterSettings.StampedPropertyIsMatter, ServiceConstants.TRUE);
            propertyList.Add(matterSettings.StampedPropertyOpenDate, WebUtility.HtmlEncode(DateTime.Now.ToString(matterSettings.ValidDateFormat, CultureInfo.InvariantCulture)));
            propertyList.Add(matterSettings.PropertyNameVtiIndexedPropertyKeys, WebUtility.HtmlEncode(ServiceUtility.GetEncodedValueForSearchIndexProperty(keys)));
            propertyList.Add(matterSettings.StampedPropertySecureMatter, (matter.Conflict != null) ? (matter.Conflict.SecureMatter != null) ? WebUtility.HtmlEncode(matter.Conflict.SecureMatter) : "False" : "False");
            propertyList.Add(matterSettings.StampedPropertyBlockedUploadUsers, WebUtility.HtmlEncode(string.Join(";", matterDetails.UploadBlockedUsers)));
            propertyList.Add(matterSettings.StampedPropertyMatterDescription, WebUtility.HtmlEncode(matter.Description));
            propertyList.Add(matterSettings.StampedPropertyConflictCheckDate, (string.IsNullOrEmpty(matter.Conflict.CheckOn)) ? 
                "" : WebUtility.HtmlEncode(Convert.ToDateTime(matter.Conflict.CheckOn, CultureInfo.InvariantCulture).ToString(matterSettings.ValidDateFormat, CultureInfo.InvariantCulture)));
            propertyList.Add(matterSettings.StampedPropertyConflictCheckBy, WebUtility.HtmlEncode(matter.Conflict.CheckBy));
            propertyList.Add(matterSettings.StampedPropertyMatterCenterRoles, WebUtility.HtmlEncode(matterCenterRoles));
            propertyList.Add(matterSettings.StampedPropertyMatterCenterPermissions, WebUtility.HtmlEncode(matterCenterPermission));
            propertyList.Add(matterSettings.StampedPropertyMatterCenterUsers, WebUtility.HtmlEncode(matterCenterUsers));
            propertyList.Add(matterSettings.StampedPropertyDefaultContentType, WebUtility.HtmlEncode(matter.DefaultContentType));
            propertyList.Add(matterSettings.StampedPropertyIsConflictIdentified, WebUtility.HtmlEncode(matter.Conflict.Identified));
            propertyList.Add(matterSettings.StampedPropertyDocumentTemplateCount, WebUtility.HtmlEncode(documentTemplateCount));
            propertyList.Add(matterSettings.StampedPropertyBlockedUsers, WebUtility.HtmlEncode(string.Join(";", matter.BlockUserNames)));
            propertyList.Add(matterSettings.StampedPropertyMatterGUID, WebUtility.HtmlEncode(matter.MatterGuid));
            propertyList.Add(matterSettings.StampedPropertySuccess, ServiceConstants.TRUE);
            return propertyList;
        }

        /// <summary>
        /// Gets the display name of users having permission on library.
        /// </summary>
        /// <param name="userPermissionOnLibrary">Users having permission on library</param>
        /// <returns></returns>
        internal List<string> RetrieveMatterUsers(IEnumerable<RoleAssignment> userPermissionOnLibrary)
        {
            List<string> users = new List<string>();
            try
            {
                if (null != userPermissionOnLibrary && 0 < userPermissionOnLibrary.Count())
                {
                    foreach (RoleAssignment roles in userPermissionOnLibrary)
                    {
                        users.Add(roles.Member.Title);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return users;
        }

        /// <summary>
        /// Check Full Permission for logged in User.
        /// </summary>
        /// <param name="AssignUserNames">List of Assigned UserNames</param>
        /// <param name="Permissions">List of Permission</param>
        /// <param name="loggedInUserName">Name of logged in User</param>
        /// <returns>Status of Full Permission</returns>
        internal bool CheckFullPermissionInAssignList(IList<IList<string>> AssignUserNames, IList<string> Permissions, string loggedInUserName)
        {
            bool result = false;
            if (null != Permissions && null != AssignUserNames && Permissions.Count == AssignUserNames.Count)
            {
                int position = 0;
                foreach (string roleName in Permissions)
                {
                    IList<string> assignUserNames = AssignUserNames[position];
                    if (!string.IsNullOrWhiteSpace(roleName) && null != assignUserNames)
                    {
                        foreach (string user in assignUserNames)
                        {
                            if (!string.IsNullOrWhiteSpace(user) && user.Trim().Equals(loggedInUserName.Trim()))
                            {
                                if (roleName == matterSettings.EditMatterAllowedPermissionLevel)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    position++;
                }
                return result;
            }
            return result;
        }
        #endregion
    }
}
