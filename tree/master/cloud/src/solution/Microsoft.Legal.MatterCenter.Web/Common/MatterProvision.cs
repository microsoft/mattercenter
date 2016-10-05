using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    /// <summary>
    /// Matter Provision is a helper class which contains all methods related to matter such as matter search, matter provision, document upload to matter
    /// functionality
    /// </summary>
    public class MatterProvision : IMatterProvision
    {
        private MatterSettings matterSettings;
        private IMatterRepository matterRepositoy;
        private ISPOAuthorization spoAuthorization;
        private IEditFunctions editFunctions;
        private ErrorSettings errorSettings;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private MailSettings mailSettings;
        private IValidationFunctions validationFunctions;
        private CamlQueries camlQueries;
        private ListNames listNames;
        private SearchSettings searchSettings;
        private IUserRepository userRepositoy;
        private IExternalSharing externalSharing;
        private IConfigurationRoot configuration;
        private IUsersDetails userDetails;
        private GeneralSettings generalSettings;

        /// <summary>
        /// Constructor where all the dependencies are injected
        /// </summary>
        /// <param name="matterRepositoy"></param>
        /// <param name="matterSettings"></param>
        /// <param name="errorSettings"></param>
        /// <param name="spoAuthorization"></param>
        /// <param name="editFunctions"></param>
        /// <param name="validationFunctions"></param>
        /// <param name="customLogger"></param>
        /// <param name="logTables"></param>
        /// <param name="mailSettings"></param>
        /// <param name="camlQueries"></param>
        /// <param name="listNames"></param>
        /// <param name="generalSettings"></param>
        /// <param name="searchSettings"></param>
        /// <param name="userRepositoy"></param>
        /// <param name="externalSharing"></param>
        /// <param name="configuration"></param>
        /// <param name="userDetails"></param>
        public MatterProvision(IMatterRepository matterRepositoy, IOptions<MatterSettings> matterSettings,
            IOptions<ErrorSettings> errorSettings,
            ISPOAuthorization spoAuthorization, IEditFunctions editFunctions, IValidationFunctions validationFunctions,
            ICustomLogger customLogger, IOptions<LogTables> logTables,
            IOptions<MailSettings> mailSettings,
            IOptions<CamlQueries> camlQueries,
            IOptions<ListNames> listNames,
            IOptions<GeneralSettings> generalSettings,
            IOptions<SearchSettings> searchSettings, IUserRepository userRepositoy, 
            IExternalSharing externalSharing, IConfigurationRoot configuration, IUsersDetails userDetails
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
            this.externalSharing = externalSharing;
            this.configuration = configuration;
            this.userDetails = userDetails;
            this.generalSettings = generalSettings.Value;
        }


        /// <summary>
        /// This method will save matter configurations in sharepoint list
        /// </summary>
        /// <param name="matterConfigurations"></param>
        /// <returns></returns>
        public GenericResponseVM SaveConfigurations(MatterConfigurations matterConfigurations)
        {
            try
            {
                Matter matter = new Matter();
                matter.AssignUserNames = GetUserList(matterConfigurations.MatterUsers);
                matter.AssignUserEmails = GetUserList(matterConfigurations.MatterUserEmails);
                ClientContext clientContext = null;
                clientContext = spoAuthorization.GetClientContext(matterConfigurations.ClientUrl);
                GenericResponseVM genericResponseVM = null;
                if (0 < matter.AssignUserNames.Count)
                {
                    genericResponseVM = matterRepositoy.ValidateTeamMembers(clientContext, matter, matterConfigurations.UserId);
                }

                if (genericResponseVM != null)
                {
                    return genericResponseVM;
                }
                genericResponseVM = matterRepositoy.SaveConfigurations(clientContext, matterConfigurations);
                return genericResponseVM;
            }            
            catch (Exception ex)
            {                
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// This method will check whether login user can create matter or not
        /// </summary>
        /// <param name="client">The sharepoint site collection in which we need to check whether the login user is present in the sharepoint group or not</param>
        /// <returns></returns>
        public bool CanCreateMatter(Client client)
        {
            return matterRepositoy.CanCreateMatter(client);
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

        public async Task<int> GetPinnedCounts(SearchRequestVM searchRequestVM)
        {
            var pinResponseVM = await matterRepositoy.GetPinnedRecordsAsync(searchRequestVM);
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

                dynamic matterDataList = new List<dynamic>();
                IEnumerable<IDictionary<string, object>> searchResults = searchResultsVM.SearchResults;
                var searchSchema = configuration.GetSection("Search");
                string key1 = configuration.GetSection("Search").GetSection("ManagedPropertyMatterName").Value.ToString();
                foreach (var searchResult in searchResults)
                {
                    dynamic matterData = new ExpandoObject();
                    matterData.matterSubAreaOfLaw = string.Empty;
                    string subAreaOfLaw = "";
                    string subAreaOfLaw1 = "";
                    string subAreaOfLaw2 = "";
                    foreach (var key in searchResult.Keys)
                    {                        
                        if(key.ToString().ToLower() == searchSettings.ManagedPropertyMatterName.ToString().ToLower())
                        {                            
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterName").Key,
                                searchResult[key].ToString());                            
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyDescription.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterDescription").Key,
                                searchResult[key].ToString());
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyOpenDate.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterCreatedDate").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyPath.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterUrl").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertySiteName.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterClientUrl").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyPracticeGroup.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterPracticeGroup").Key,
                                searchResult[key].ToString().Trim().TrimEnd(';'));
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyAreaOfLaw.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterAreaOfLaw").Key,
                                searchResult[key].ToString().Trim().TrimEnd(';'));
                        }

                        #region Subarea of law login
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertySubAreaOfLaw2.ToLower())
                        {
                            subAreaOfLaw2 = searchResult[key].ToString().Trim().TrimEnd(';');
                        }
                        if (key.ToString().ToLower() == searchSettings.ManagedPropertySubAreaOfLaw1.ToLower())
                        {
                            subAreaOfLaw1 = searchResult[key].ToString().Trim().TrimEnd(';');
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertySubAreaOfLaw.ToLower())
                        {
                            subAreaOfLaw = searchResult[key].ToString().Trim().TrimEnd(';');
                        }

                        if (subAreaOfLaw2 != string.Empty && subAreaOfLaw1 != string.Empty && subAreaOfLaw != string.Empty)
                        {
                            ServiceUtility.AddProperty(matterData, "matterSubAreaOfLaw", subAreaOfLaw2);
                        }

                        if (subAreaOfLaw2 == string.Empty && subAreaOfLaw1 != string.Empty && subAreaOfLaw != string.Empty)
                        {
                            ServiceUtility.AddProperty(matterData, "matterSubAreaOfLaw", subAreaOfLaw1);
                        }

                        if (subAreaOfLaw2 == string.Empty && subAreaOfLaw1 == string.Empty && subAreaOfLaw != string.Empty)
                        {
                            ServiceUtility.AddProperty(matterData, "matterSubAreaOfLaw", subAreaOfLaw);
                        }
                        #endregion

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyClientName.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterClient").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyClientID.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterClientId").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyBlockedUploadUsers.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("hideUpload").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyResponsibleAttorney.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterResponsibleAttorney").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyMatterId.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterID").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyMatterGuid.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterGuid").Key,
                                searchResult[key].ToString());
                        }

                        if (key.ToString().ToLower() == searchSettings.ManagedPropertyLastModifiedTime.ToLower())
                        {
                            ServiceUtility.AddProperty(matterData,
                                configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter").GetSection("matterModifiedDate").Key,
                                searchResult[key].ToString());
                        }
                        ServiceUtility.AddProperty(matterData,"PinType","Pin");
                    }
                    matterDataList.Add(matterData);
                }
                searchResultsVM.MatterDataList = matterDataList;
            }
            searchResultsVM.SearchResults = null;
            if (searchRequestVM.SearchObject.IsUnique && searchResultsVM.MatterDataList != null && !string.IsNullOrWhiteSpace(searchRequestVM.SearchObject.UniqueColumnName))
            {
                searchResultsVM.MatterDataList = getUniqueResults(searchRequestVM, searchResultsVM);
            }
            return searchResultsVM;
        }


        /// <summary>
        /// getting unique results for this.
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <param name="searchResultsVM"></param>
        /// <returns></returns>
        public dynamic getUniqueResults(SearchRequestVM searchRequestVM, dynamic searchResultsVM)
        {
            dynamic matterDataList1 = new List<dynamic>();
            var colList = configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter");
            string UniqueColumnName = getuniqueColumnName(searchRequestVM.SearchObject.UniqueColumnName.ToLower().Trim());

            if (!string.IsNullOrWhiteSpace(UniqueColumnName)) 
            {
                if (UniqueColumnName.Equals(colList.GetSection("matterName").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.MatterDataList).Where(d => d.matterName.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.matterName).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic matterData1 = new ExpandoObject();
                        matterData1.matterName = dt;
                        matterDataList1.Add(matterData1);
                    }
                    searchResultsVM.MatterDataList = matterDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("matterClient").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.MatterDataList).Where(d => d.matterClient.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.matterClient).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic matterData1 = new ExpandoObject();
                        matterData1.matterClient = dt;
                        matterDataList1.Add(matterData1);
                    }
                    searchResultsVM.MatterDataList = matterDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("matterPracticeGroup").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.MatterDataList).Where(d => d.matterPracticeGroup.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.matterPracticeGroup).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic matterData1 = new ExpandoObject();
                        matterData1.matterPracticeGroup = dt;
                        matterDataList1.Add(matterData1);
                    }
                    searchResultsVM.MatterDataList = matterDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("matterAreaOfLaw").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.MatterDataList).Where(d => d.matterAreaOfLaw.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.matterAreaOfLaw).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic matterData1 = new ExpandoObject();
                        matterData1.matterAreaOfLaw = dt;
                        matterDataList1.Add(matterData1);
                    }
                    searchResultsVM.MatterDataList = matterDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("matterSubAreaOfLaw").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.MatterDataList).Where(d => d.matterSubAreaOfLaw.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.matterSubAreaOfLaw).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic matterData1 = new ExpandoObject();
                        matterData1.matterSubAreaOfLaw = dt;
                        matterDataList1.Add(matterData1);
                    }
                    searchResultsVM.MatterDataList = matterDataList1;
                }
                else if (UniqueColumnName.Equals(colList.GetSection("matterResponsibleAttorney").Key))
                {
                    var data = ((IEnumerable<dynamic>)searchResultsVM.MatterDataList).Where(d => d.matterResponsibleAttorney.Contains(searchRequestVM.SearchObject.FilterValue));
                    data = data.Select(o => o.matterResponsibleAttorney).Distinct();
                    foreach (var dt in data)
                    {
                        dynamic matterData1 = new ExpandoObject();
                        matterData1.matterResponsibleAttorney = dt;
                        matterDataList1.Add(matterData1);
                    }
                    searchResultsVM.MatterDataList = matterDataList1;
                }
            }

            return searchResultsVM.MatterDataList;
        }

        /// <summary>
        /// to get column name 
        /// </summary>
        /// <returns></returns>
        public string getuniqueColumnName(string uniueColumnName)
        {
            var docColumnSesction = configuration.GetSection("Search").GetSection("SearchColumnsUIPickerForMatter");
            
            if (searchSettings.ManagedPropertyMatterName.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("matterName").Key;
            }
            else if (searchSettings.ManagedPropertyClientName.ToString().ToLower().Equals(uniueColumnName) && !generalSettings.IsBackwardCompatible )
            {
                uniueColumnName = docColumnSesction.GetSection("matterClient").Key;
            }
            else if (searchSettings.ManagedPropertyPracticeGroup.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("matterPracticeGroup").Key;
            }
            else if (searchSettings.ManagedPropertyResponsibleAttorney.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("matterResponsibleAttorney").Key;
            }
            else if (searchSettings.ManagedPropertyAreaOfLaw.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("matterAreaOfLaw").Key;
            }
            else if (searchSettings.ManagedPropertySubAreaOfLaw.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("matterSubAreaOfLaw").Key;
            }
            else if (searchSettings.ManagedPropertyMatterId.ToString().ToLower().Equals(uniueColumnName))
            {
                uniueColumnName = docColumnSesction.GetSection("matterID").Key;
            }    
            else
            {
                uniueColumnName = string.Empty;
            }

            return uniueColumnName;
        }

        public GenericResponseVM ShareMatterToExternalUser(MatterInformationVM matterInformation)
        {
            return matterRepositoy.ShareMatterToExternalUser(matterInformation);
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

                bool isFullControlPresent = editFunctions.ValidateFullControlPermission(matter);

                if (!isFullControlPresent)
                {
                    return ServiceUtility.GenericResponse(errorSettings.IncorrectInputSelfPermissionRemoval,
                        errorSettings.ErrorEditMatterMandatoryPermission);
                }

                genericResponse = matterRepositoy.UpdateMatter(matterInformation);

                //Need to loop each matter information, update the table storage with that matter information, before sending
                //notification to external user
                if (matterInformation.Matter != null && matterInformation.Matter.Roles != null && matterInformation.Matter.Roles.Count > 0)
                {                    
                    ShareMatterToExtUser(matterInformation);                   
                }
            }
            catch (Exception ex)
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
            return null;
        }




        public GenericResponseVM UpdateMatterMetadata(MatterMetdataVM matterMetadata)
        {
            var matter = matterMetadata.Matter;
            var matterDetails = matterMetadata.MatterDetails;
            var client = matterMetadata.Client;
            ClientContext clientContext = null;
            GenericResponseVM genericResponse = null;
            try
            {
                MatterInformationVM matterInfo = new MatterInformationVM()
                {
                    Client = matterMetadata.Client,
                    Matter = matterMetadata.Matter,
                    MatterDetails = matterMetadata.MatterDetails
                };              
                clientContext = spoAuthorization.GetClientContext(matterMetadata.Client.Url);
                PropertyValues matterStampedProperties = matterRepositoy.GetStampedProperties(clientContext, matter.Name);
                Dictionary<string, string> propertyList = SetStampProperty(matterMetadata);
                matterRepositoy.SetPropertBagValuesForList(clientContext, matterStampedProperties, matter.Name, propertyList);
                //As part of final step in matter creation, check whether any assigned users are external to the 
                //organization and if yes, send notification to that user to accepct the invitation so that he can access matter center

                //Need to loop each matter information, update the table storage with that matter information, before sending
                //notification to external user
                if (matterInfo.Matter != null && matterInfo.Matter.Roles != null && matterInfo.Matter.Roles.Count > 0)
                {
                    ShareMatterToExtUser(matterInfo);
                }
                genericResponse = ServiceUtility.GenericResponse("", "Matter Update Success");                
            }
            catch (Exception ex)
            {
                DeleteMatter(matterMetadata as MatterVM);
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return genericResponse;
        }

        public GenericResponseVM DeleteMatter(MatterVM matterVM)
        {
            var client = matterVM.Client;
            var matter = matterVM.Matter;
            GenericResponseVM genericResponse = matterRepositoy.DeleteMatter(client, matter);
            return genericResponse;
        }

        public GenericResponseVM SavConfigurations(MatterConfigurations matterConfiguration)
        {
            ClientContext clientContext = null;
            GenericResponseVM returnFlag = null;
            try
            {
                clientContext = spoAuthorization.GetClientContext(matterConfiguration.ClientUrl);
                Matter matter = new Matter();
                matter.AssignUserNames = GetUserList(matterConfiguration.MatterUsers);
                matter.AssignUserEmails = GetUserList(matterConfiguration.MatterUserEmails);
                if (0 < matter.AssignUserNames.Count)
                {
                    returnFlag = matterRepositoy.ValidateTeamMembers(clientContext, matter, matterConfiguration.UserId);
                }
                if (returnFlag != null)
                {
                    returnFlag = matterRepositoy.SaveConfigurationToList(matterConfiguration, clientContext, matterConfiguration.CachedItemModifiedDate);
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
                                matterConfiguration.CachedItemModifiedDate = Convert.ToString(settingsItem[matterSettings.ColumnNameModifiedDate], CultureInfo.InvariantCulture);
                            }
                            returnFlag.Value = string.Concat(returnFlag.Value, ServiceConstants.PIPE, ServiceConstants.DOLLAR, ServiceConstants.PIPE, matterConfiguration.CachedItemModifiedDate);

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

        public MatterStampedDetails GetStampedProperties(MatterVM matterVM)
        {
            var matter = matterVM.Matter;
            var client = matterVM.Client;
            ClientContext clientContext = null;
            MatterStampedDetails matterStampedDetails = null;
            PropertyValues matterStampedProperties = null;

            try
            {
                clientContext = spoAuthorization.GetClientContext(matterVM.Client.Url);
                matterStampedProperties = matterRepositoy.GetStampedProperties(clientContext, matter.Name);
                Dictionary<string, object> stampedPropertyValues = matterStampedProperties.FieldValues;

                if (stampedPropertyValues.Count > 0)
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

                    matterStampedDetails = new MatterStampedDetails()
                    {
                        IsNewMatter = stampedPropertyValues.ContainsKey(matterSettings.StampedPropertyIsConflictIdentified) ? ServiceConstants.TRUE : ServiceConstants.FALSE,
                        MatterObject = new Matter()
                        {
                            Id = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterID),
                            MatterGuid = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyMatterGUID),
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
            return matterStampedDetails;
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

                if (genericResponseVM != null)
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
                                new GenericResponseVM()
                                {
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
                if (listExists.Count > 0)
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
                if (genericResponseVM != null)
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
            catch (Exception ex)
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
                Uri centralListURL = new Uri(string.Concat(generalSettings.CentralRepositoryUrl, ServiceConstants.FORWARD_SLASH,
                    ServiceConstants.LISTS, ServiceConstants.FORWARD_SLASH, listNames.DMSMatterListName)); // Central Repository List URL  
                IList<string> documentLibraryFolders = new List<string>();
                Dictionary<string, bool> documentLibraryVersioning = new Dictionary<string, bool>();
                Uri clientUrl = new Uri(client.Url);
                string matterOneNoteTitle = TrimMatterNameForOneNoteTitle(matter.Name);
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
                documentLibraryFolders.Add(matterOneNoteTitle);
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
                    matter.MatterGuid, matterSettings.OneNoteLibrarySuffix, ServiceConstants.FORWARD_SLASH, matterOneNoteTitle);
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
                if (genericResponseVM == null)
                {
                    string centralList = Convert.ToString(centralListURL, CultureInfo.InvariantCulture);
                    string matterSiteURL = centralList.Substring(0, centralList.LastIndexOf(string.Concat(ServiceConstants.FORWARD_SLASH,
                        ServiceConstants.LISTS, ServiceConstants.FORWARD_SLASH), StringComparison.OrdinalIgnoreCase));
                    string matterListName = centralList.Substring(centralList.LastIndexOf(ServiceConstants.FORWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1);


                    bool isMatterSaved = matterRepositoy.SaveMatter(client, matter, matterListName, matterConfiguration, matterSiteURL);
                    if (isMatterSaved == false)
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
        /// Function to prepare OneNote URL based on threshold set for OneNote title
        /// </summary>
        /// <param name="matterName">Project name</param>
        /// <returns>OneNote title for provided Project name</returns>
        internal static string TrimMatterNameForOneNoteTitle(string matterName)
        {
            if (matterName.Length > ServiceConstants.Matter_ONE_NOTE_LENGTH)
            {
                matterName = matterName.Substring(0, ServiceConstants.Matter_ONE_NOTE_LENGTH);
            }
            return matterName;
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
            MatterDetails matterDetails = new MatterDetails();

            matterDetails.ResponsibleAttorney = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyResponsibleAttorney);
            matterDetails.TeamMembers = GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyTeamMembers);
            matterDetails.UploadBlockedUsers =
                GetStampPropertyValue(stampedPropertyValues, matterSettings.StampedPropertyBlockedUploadUsers).Split(new string[] { ServiceConstants.SEMICOLON },
                StringSplitOptions.RemoveEmptyEntries).ToList();


            /*
             * All the managed columns need to be read from the appsettings.json file. In old implementation
             * all the managed columns are hardcoded and that hardcoding has been removed, by reading the
             * column names from appsettings.json file
             */
            //Get the number of levels from Taxonomy Settings
            int levels = int.Parse(configuration.GetSection("Taxonomy")["Levels"].ToString());
            IDictionary<string, ManagedColumn> managedColumns = new Dictionary<string, ManagedColumn>();
            for (int i = 1; i <= levels; i++)
            {
                //Get all the managed columns from "ContentType" settings from appsettings.json file
                string columnName = configuration.GetSection("ContentTypes").GetSection("ManagedColumns")["ColumnName" + i];
                string managedColumnValue = GetStampPropertyValue(stampedPropertyValues, columnName);
                ManagedColumn managedColumn = new ManagedColumn();
                managedColumn.TermName = managedColumnValue;
                managedColumns.Add(columnName, managedColumn);

            }
            matterDetails.ManagedColumnTerms = managedColumns;
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
                    Uri mailListURL = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", generalSettings.CentralRepositoryUrl,
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
        internal GenericResponseVM ShareMatterUtility(Client client, Matter matter, MatterDetails matterDetails, string mailSiteURL,
            string centralMailListURL, string matterLandingFlag, MatterConfigurations matterConfigurations)
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
                    if (genericResponse != null)
                    {
                        return genericResponse;
                    }

                    MatterInformationVM matterInformation = new MatterInformationVM()
                    {
                        Client = client,
                        Matter = matter,
                        MatterDetails = matterDetails
                    };

                    // Get the current logged in User
                    clientContext.Load(clientContext.Web.CurrentUser);
                    clientContext.ExecuteQuery(); 
                    matterInformation.MatterCreator = clientContext.Web.CurrentUser.Title;
                   
                    if (ServiceConstants.TRUE == matterLandingFlag)
                    {
                        matterLocation = string.Concat(client.Url, ServiceConstants.FORWARD_SLASH,
                            matterSettings.MatterLandingPageRepositoryName.Replace(ServiceConstants.SPACE, string.Empty),
                            ServiceConstants.FORWARD_SLASH, matter.MatterGuid, ServiceConstants.ASPX_EXTENSION);
                        matterInformation.MatterLocation = matterLocation;
                    }
                    

                    // Step 4: Create Conflict check Information based on the conflict check flag and create mail body
                    if (matterConfigurations.IsConflictCheck)
                    {
                        matterInformation.IsConflictCheck = true;
                        
                    }
                    else
                    {
                        matterInformation.IsConflictCheck = false;
                    }                    
                    //List<FieldUserValue> userList = new List<FieldUserValue>();
                    //List<FieldUserValue> userEmailList = GenerateMailList(matter, new Client { Url = mailSiteURL }, ref userList);
                    /////// Add the Matter URL in list
                    //FieldUrlValue matterPath = new FieldUrlValue()
                    //{
                    //    Url = string.Concat(client.Url.Replace(String.Concat(ServiceConstants.HTTPS, ServiceConstants.COLON,
                    //    ServiceConstants.FORWARD_SLASH, ServiceConstants.FORWARD_SLASH), String.Concat(ServiceConstants.HTTP, ServiceConstants.COLON,
                    //    ServiceConstants.FORWARD_SLASH, ServiceConstants.FORWARD_SLASH)), ServiceConstants.FORWARD_SLASH, matter.Name,
                    //    ServiceConstants.FORWARD_SLASH, matter.Name),
                    //    Description = matter.Name
                    //};
                    //matterInformation.MatterUrlPath = matterPath.Url;                    
                    SerializeMatterToTableStorage(matterInformation);
                }
            }
            return genericResponse;
        }


        /// <summary>
        /// This method will store external requests information in Azure Table Storage
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private void SerializeMatterToTableStorage(MatterInformationVM matterInformation)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(generalSettings.CloudStorageConnectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions = new TableRequestOptions
                {
                    PayloadFormat = TablePayloadFormat.JsonNoMetadata
                };
                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference(logTables.MatterRequests);
                // Create the table if it doesn't exist.
                table.CreateIfNotExists();
                //Insert the entity into Table Storage              
                matterInformation.PartitionKey = matterInformation.Matter.Name;
                matterInformation.RowKey = $"{Guid.NewGuid().ToString()}${matterInformation.Matter.Id}";
                matterInformation.Status = "Pending";
                string matterInformationObject = Newtonsoft.Json.JsonConvert.SerializeObject(matterInformation);
                matterInformation.SerializeMatter = matterInformationObject;
                TableOperation insertOperation = TableOperation.Insert(matterInformation);
                table.Execute(insertOperation);
            }
            catch (Exception ex)
            {
                throw;
            }
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
        internal Dictionary<string, string> SetStampProperty(MatterMetdataVM matterMetadata)
        {
            try
            {

                Client client = matterMetadata.Client;
                Matter matter = matterMetadata.Matter;
                MatterDetails matterDetails = matterMetadata.MatterDetails;

                string documentTemplateCount = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.DocumentTemplateCount);
                string matterCenterPermission = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.Permissions);
                string matterCenterRoles = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.Roles);
                string finalTeamMembers = matterDetails.TeamMembers;
                string finalResponsibleAttorneysUsers = matterDetails.ResponsibleAttorney;
                string finalResponsibleAttorneysEmail = matterDetails.ResponsibleAttorneyEmail;
                string matterCenterUsers = string.Empty;
                string matterCenterUserEmails = string.Empty;
                string separator = string.Empty;
                foreach (IList<string> userNames in matter.AssignUserNames)
                {
                    if (userDetails.CheckUserPresentInMatterCenter(client.Url, userNames.Where(user => !string.IsNullOrWhiteSpace(user)).SingleOrDefault()) == true)
                    {
                        matterCenterUsers += separator + string.Join(ServiceConstants.SEMICOLON, userNames.Where(user => !string.IsNullOrWhiteSpace(user)));
                        separator = ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR;
                    }
                }

                foreach (IList<string> userEmails in matter.AssignUserEmails)
                {
                    if (userDetails.CheckUserPresentInMatterCenter(client.Url, userEmails.Where(user => !string.IsNullOrWhiteSpace(user)).SingleOrDefault()) == true)
                    {
                        matterCenterUserEmails += string.Join(ServiceConstants.SEMICOLON, userEmails.Where(user => !string.IsNullOrWhiteSpace(user))) + separator;
                    }
                }
                var finalMatterPermissionsList = matterCenterPermission.Replace("$|$", "$").Split('$').ToList();
                var finalMatterRolesList = matterCenterRoles.Replace("$|$", "$").Split('$').ToList();
                var finalTeamMembersList = matterDetails.TeamMembers.Replace(";;", "$").Split('$').ToList();
                var finalResponsibleAttorneysEmailList = matterDetails.ResponsibleAttorneyEmail.Split(';').ToList();
                var finalResponsibleAttorneysUsersList = matterDetails.ResponsibleAttorney.Split(';').ToList();
                var userEmailsList = matter.AssignUserEmails;
                var userNamesList = matter.AssignUserNames;

                List<int> itemsToRemove = new List<int>();
                List<int> itemsToRemoveAttorneys = new List<int>();
                int l = 0;
                foreach (string userName in finalResponsibleAttorneysUsersList)
                {
                    if (!string.IsNullOrWhiteSpace(userName) && userDetails.CheckUserPresentInMatterCenter(client.Url, userName) == false)
                    {
                        itemsToRemoveAttorneys.Add(l);
                    }
                    l = l + 1;
                }

                if (itemsToRemoveAttorneys.Count > 0)
                {
                    for (int k = 0; k < itemsToRemoveAttorneys.Count; k++)
                    {
                        if (finalResponsibleAttorneysEmailList.Count > itemsToRemoveAttorneys[k] && finalResponsibleAttorneysEmailList[itemsToRemoveAttorneys[k]] != null)
                        {
                            finalResponsibleAttorneysEmailList[k] = string.Empty;
                            finalResponsibleAttorneysUsersList[k] = string.Empty;
                        }
                    }
                    finalResponsibleAttorneysEmailList = finalResponsibleAttorneysEmailList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    finalResponsibleAttorneysUsersList = finalResponsibleAttorneysUsersList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                }

                l = 0;

                //Check if any of the assigned team member is an external user?
                foreach (IList<string> userNames in matter.AssignUserNames)
                {
                    if (userDetails.CheckUserPresentInMatterCenter(client.Url, userNames.Where(user => !string.IsNullOrWhiteSpace(user)).SingleOrDefault()) == false)
                    {
                        itemsToRemove.Add(l);
                    }
                    l = l + 1;
                }

                //If any of the team members are external users, do not add his role, his permission into matter proeprty bag
                //Once the user accepts the invitation, then  update the property bag with role and permissions
                if (itemsToRemove.Count > 0)
                {
                    for (int k = 0; k < itemsToRemove.Count; k++)
                    {
                        finalMatterPermissionsList[itemsToRemove[k]] = string.Empty;
                        finalMatterRolesList[itemsToRemove[k]] = string.Empty; ;
                        finalTeamMembersList[itemsToRemove[k]] = string.Empty; ;
                    }

                    finalMatterPermissionsList = finalMatterPermissionsList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    finalMatterRolesList = finalMatterRolesList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    finalTeamMembersList = finalTeamMembersList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    var finalTeamMembersArray = finalTeamMembersList.ToArray();
                    var finalMatterPermissionsArray = finalMatterPermissionsList.ToArray();
                    var finalMatterRolesArray = finalMatterRolesList.ToArray();
                    var finalResponsibleAttorneysEmailsArray = finalResponsibleAttorneysEmailList.ToArray();
                    var finalResponsibleAttorneysUsersArray = finalResponsibleAttorneysUsersList.ToArray();

                    matterCenterUsers = "";
                    matterCenterUserEmails = "";
                    separator = "";
                    foreach (IList<string> userNames in userNamesList)
                    {
                        if (userDetails.CheckUserPresentInMatterCenter(client.Url, userNames.Where(user => !string.IsNullOrWhiteSpace(user)).SingleOrDefault()) == true)
                        {
                            matterCenterUsers += separator + string.Join(ServiceConstants.SEMICOLON, userNames.Where(user => !string.IsNullOrWhiteSpace(user)));
                            separator = ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR;
                        }

                    }
                    separator = "";
                    foreach (IList<string> userEmails in userEmailsList)
                    {
                        if (userDetails.CheckUserPresentInMatterCenter(client.Url, userEmails.Where(user => !string.IsNullOrWhiteSpace(user)).SingleOrDefault()) == true)
                        {
                            matterCenterUserEmails += separator + string.Join(ServiceConstants.SEMICOLON, userEmails.Where(user => !string.IsNullOrWhiteSpace(user)));
                            separator = ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR;
                        }
                    }

                    finalTeamMembers = string.Join(";;", finalTeamMembersArray);
                    matterCenterPermission = string.Join("$|$", finalMatterPermissionsArray);
                    matterCenterRoles = string.Join("$|$", finalMatterRolesArray);
                    finalResponsibleAttorneysEmail = string.Join(";", finalResponsibleAttorneysEmailsArray);
                    finalResponsibleAttorneysUsers = string.Join(";", finalResponsibleAttorneysUsersArray);
                }
                List<string> keys = new List<string>();
                Dictionary<string, string> propertyList = new Dictionary<string, string>();
                //Get all the matter stamped properties from the appsettings.json file
                var matterStampedProperties = configuration.GetSection("Matter").GetChildren();
                keys.Add(matterSettings.StampedPropertyMatterName);
                keys.Add(matterSettings.StampedPropertyMatterID);
                keys.Add(matterSettings.StampedPropertyClientName);
                keys.Add(matterSettings.StampedPropertyClientID);
                keys.Add(matterSettings.StampedPropertyResponsibleAttorney);
                keys.Add(matterSettings.StampedPropertyResponsibleAttorneyEmail);
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
                keys.Add(matterSettings.StampedPropertyMatterCenterUserEmails);
                keys.Add(matterSettings.StampedPropertyDefaultContentType);
                keys.Add(matterSettings.StampedPropertyIsConflictIdentified);
                keys.Add(matterSettings.StampedPropertyDocumentTemplateCount);
                keys.Add(matterSettings.StampedPropertyBlockedUsers);
                keys.Add(matterSettings.StampedPropertyMatterGUID);
                /*
                 * All the managed columns need to be read from the appsettings.json file. In old implementation
                 * all the managed columns are hardcoded and that hardcoding has been removed, by reading the
                 * column names from appsettings.json file
                 */
                //Get the number of levels from Taxonomy Settings
                int levels = int.Parse(configuration.GetSection("Taxonomy")["Levels"].ToString());
                for (int i = 1; i <= levels; i++)
                {
                    //Get all the managed columns from "ContentType" settings from appsettings.json file
                    string columnName = configuration.GetSection("ContentTypes").GetSection("ManagedColumns")["ColumnName" + i];
                    ManagedColumn managedColumn = matterDetails.ManagedColumnTerms[columnName];                    
                    //Add all the managed columns values to the property list of the matter document library             
                    propertyList.Add(columnName, WebUtility.HtmlEncode(managedColumn.TermName));
                    //Add all the managed columns to the Indexed Property keys of the matter document library
                    keys.Add(columnName);
                }
                propertyList.Add(matterSettings.StampedPropertyMatterName, WebUtility.HtmlEncode(matter.Name));
                propertyList.Add(matterSettings.StampedPropertyMatterID, WebUtility.HtmlEncode(matter.Id));
                if(!propertyList.ContainsKey(matterSettings.StampedPropertyClientName))
                {
                    propertyList.Add(matterSettings.StampedPropertyClientName, WebUtility.HtmlEncode(client.Name));
                }                
                propertyList.Add(matterSettings.StampedPropertyClientID, WebUtility.HtmlEncode(client.Id));
                propertyList.Add(matterSettings.StampedPropertyResponsibleAttorney, WebUtility.HtmlEncode(finalResponsibleAttorneysUsers));
                propertyList.Add(matterSettings.StampedPropertyTeamMembers, WebUtility.HtmlEncode(finalTeamMembers));
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
                propertyList.Add(matterSettings.StampedPropertyMatterCenterUserEmails, WebUtility.HtmlEncode(matterCenterUserEmails));
                propertyList.Add(matterSettings.StampedPropertyResponsibleAttorneyEmail, WebUtility.HtmlEncode(finalResponsibleAttorneysEmail));

                if (matterMetadata.MatterProvisionFlags.SendEmailFlag)
                {
                    ShareMatter(matterMetadata, matterMetadata.MatterProvisionFlags.MatterLandingFlag);
                }

                l = 0;
                itemsToRemoveAttorneys.Clear();
                finalResponsibleAttorneysEmailList = matterDetails.ResponsibleAttorneyEmail.Split(';').ToList();
                finalResponsibleAttorneysUsersList = matterDetails.ResponsibleAttorney.Split(';').ToList();
                foreach (string userName in finalResponsibleAttorneysUsersList)
                {
                    if (!string.IsNullOrWhiteSpace(userName) && userDetails.CheckUserPresentInMatterCenter(client.Url, userName) == true)
                    {
                        itemsToRemoveAttorneys.Add(l);
                    }
                    l = l + 1;
                }
                if (itemsToRemoveAttorneys.Count > 0)
                {
                    for (int k = 0; k < itemsToRemoveAttorneys.Count; k++)
                    {
                        finalResponsibleAttorneysUsersList[itemsToRemoveAttorneys[k]] = string.Empty;
                        finalResponsibleAttorneysEmailList[itemsToRemoveAttorneys[k]] = string.Empty;
                    }
                    finalResponsibleAttorneysUsersList = finalResponsibleAttorneysUsersList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    finalResponsibleAttorneysEmailList = finalResponsibleAttorneysEmailList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                }
                itemsToRemove.Clear();
                l = 0;
                //Check if any of the assigned team member is an external user?
                foreach (IList<string> userNames in matter.AssignUserNames)
                {
                    if (userDetails.CheckUserPresentInMatterCenter(client.Url, userNames.Where(user => !string.IsNullOrWhiteSpace(user)).SingleOrDefault()) == true)
                    {
                        itemsToRemove.Add(l);
                    }
                    l = l + 1;
                }
                finalTeamMembersList = matterDetails.TeamMembers.Replace(";;", "$").Split('$').ToList();
                if (itemsToRemove.Count > 0)
                {
                    for (int k = 0; k < itemsToRemove.Count; k++)
                    {
                        matter.Permissions[itemsToRemove[k]] = string.Empty;
                        matter.Roles[itemsToRemove[k]] = string.Empty;
                        matter.AssignUserEmails[itemsToRemove[k]] = null;
                        matter.AssignUserNames[itemsToRemove[k]] = null;
                        finalTeamMembersList[itemsToRemove[k]] = string.Empty;
                    }
                }

                matter.Permissions = matter.Permissions.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                matter.Roles = matter.Roles.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                finalTeamMembersList = finalTeamMembersList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                matter.AssignUserEmails = matter.AssignUserEmails.Where(s => s != null).ToList();
                matter.AssignUserNames = matter.AssignUserNames.Where(s => s != null).ToList();


                matterDetails.ResponsibleAttorneyEmail = string.Empty;
                matterDetails.ResponsibleAttorney = string.Empty;
                matterDetails.TeamMembers = string.Join(";;", finalTeamMembersList.ToArray());
                matterDetails.ResponsibleAttorneyEmail = string.Join(";", finalResponsibleAttorneysEmailList.ToArray());
                matterDetails.ResponsibleAttorney = string.Join(";", finalResponsibleAttorneysUsersList.ToArray());
                return propertyList;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        #endregion


        #region Private Methods

        

        /// <summary>
        /// This method will loop for all external users in the matterinformation object and
        /// will send notification to that external user
        /// </summary>
        /// <param name="matterInformation">Contains information about all external users, his roles and permissions</param>
        private void ShareMatterToExtUser(MatterInformationVM matterInformation)
        {            
            for (int i = 0; i < matterInformation.Matter.Roles.Count; i++)
            {
                //Need to construct new MatterInformationVM object for each external user
                MatterInformationVM matterInfoNew = new MatterInformationVM();

                List<string> userIds = new List<string>();
                if (matterInformation.UserIds != null)
                {
                    foreach (var userid in matterInformation.UserIds)
                    {
                        userIds.Add(userid);
                    }
                }

                matterInfoNew.UserIds = userIds;
                Conflict conflictNew = new Conflict();
                conflictNew.Identified = matterInformation.Matter.Conflict.Identified;
                Matter matterNew = new Matter()
                {
                    Id = matterInformation.Matter.Id,
                    Name = matterInformation.Matter.Name,
                    Description = matterInformation.Matter.Description,
                    MatterGuid = matterInformation.Matter.MatterGuid,
                    Conflict = conflictNew,


                };
                matterInfoNew.EditMode = matterInformation.EditMode;
                matterInfoNew.Client = matterInformation.Client;
                var roles = new List<String>();
                string role = matterInformation.Matter.Roles[i];
                roles.Add(role);
                matterNew.Roles = roles;
                var permissions = new List<String>();
                string permission = matterInformation.Matter.Permissions[i];
                permissions.Add(permission);
                matterNew.Permissions = permissions;

                var assignUserEmails = new List<IList<string>>();
                var userEmails = new List<string>();
                foreach (var assignUserEmail in matterInformation.Matter.AssignUserEmails[i])
                {
                    userEmails.Add(assignUserEmail);
                }
                assignUserEmails.Add(userEmails);
                matterNew.AssignUserEmails = assignUserEmails;

                var assignUserNames = new List<IList<string>>();
                var userNames = new List<string>();
                foreach (var assignUserName in matterInformation.Matter.AssignUserNames[i])
                {
                    userNames.Add(assignUserName);
                }
                assignUserNames.Add(userNames);
                matterNew.AssignUserNames = assignUserNames;

                matterInfoNew.Matter = matterNew;
                MatterDetails matterDetailsNew = new MatterDetails();
                if (!string.IsNullOrEmpty(matterInformation.MatterDetails.ResponsibleAttorney))
                {
                    matterDetailsNew.ResponsibleAttorney = matterInformation.MatterDetails.ResponsibleAttorney.Split(';')[i];
                }
                if (!string.IsNullOrEmpty(matterInformation.MatterDetails.ResponsibleAttorneyEmail))
                {
                    matterDetailsNew.ResponsibleAttorneyEmail = matterInformation.MatterDetails.ResponsibleAttorneyEmail.Split(';')[i];
                }
                matterDetailsNew.TeamMembers = matterNew.AssignUserNames[0][0];
                matterDetailsNew.UploadBlockedUsers = matterInformation.MatterDetails.UploadBlockedUsers;
                matterInfoNew.MatterDetails = matterDetailsNew;
                //Share the matter to external user by sending the notification
                externalSharing.ShareMatter(matterInfoNew);
            }            
        }
        #endregion
    }
}
