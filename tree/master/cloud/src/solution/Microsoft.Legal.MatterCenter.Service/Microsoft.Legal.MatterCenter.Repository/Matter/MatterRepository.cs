// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="MatterRepository.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// This class deals with all the matter related functions such as finding matter, pin, unpin, update matter etc
// ***********************************************************************

using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.Globalization;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class MatterRepository:IMatterRepository
    {
        private ISearch search;
        private ISPList spList;
        private MatterSettings matterSettings;
        private SearchSettings searchSettings;
        private CamlQueries camlQueries;
        private ListNames listNames;
        /// <summary>
        /// Constructory which will inject all the related dependencies related to matter
        /// </summary>
        /// <param name="search"></param>
        public MatterRepository(ISearch search, IOptions<MatterSettings> matterSettings, 
            IOptions<SearchSettings> searchSettings, IOptions<ListNames> listNames, ISPList spList, IOptions<CamlQueries> camlQueries)
        {
            this.search = search;
            this.matterSettings = matterSettings.Value;
            this.searchSettings = searchSettings.Value;
            this.listNames = listNames.Value;
            this.spList = spList;
            this.camlQueries = camlQueries.Value;
        }

        /// <summary>
        /// This method will try to fetch all the matters that are provisioned by the user
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<SearchResponseVM> GetMattersAsync(SearchRequestVM searchRequestVM)
        {
            return await Task.FromResult(search.GetMatters(searchRequestVM));
        }

        /// <summary>
        /// This method will try to fetch all the matters that are provisioned by the user
        /// </summary>
        /// <param name="searchRequestVM"></param>
        /// <returns></returns>
        public async Task<PinResponseVM> GetPinnedRecordsAsync(Client client)
        {
            return await Task.FromResult(search.GetPinnedData(client, listNames.UserPinnedMatterListName,
                searchSettings.PinnedListColumnMatterDetails, false));
        }

        /// <summary>
        /// Create a new pin for the information that has been passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pinData"></param>
        /// <returns></returns>
        public async Task<bool> PinRecordAsync<T>(T pinRequestData)
        {
            PinRequestMatterVM pinRequestMatterVM = (PinRequestMatterVM)Convert.ChangeType(pinRequestData, typeof(PinRequestMatterVM));
            var matterData = pinRequestMatterVM.MatterData;            
            matterData.MatterName = ServiceUtility.EncodeValues(matterData.MatterName);
            matterData.MatterDescription = ServiceUtility.EncodeValues(matterData.MatterDescription);
            matterData.MatterCreatedDate = ServiceUtility.EncodeValues(matterData.MatterCreatedDate);
            matterData.MatterUrl = ServiceUtility.EncodeValues(matterData.MatterUrl);
            matterData.MatterPracticeGroup = ServiceUtility.EncodeValues(matterData.MatterPracticeGroup);
            matterData.MatterAreaOfLaw = ServiceUtility.EncodeValues(matterData.MatterAreaOfLaw);
            matterData.MatterSubAreaOfLaw = ServiceUtility.EncodeValues(matterData.MatterSubAreaOfLaw);
            matterData.MatterClientUrl = ServiceUtility.EncodeValues(matterData.MatterClientUrl);
            matterData.MatterClient = ServiceUtility.EncodeValues(matterData.MatterClient);
            matterData.MatterClientId = ServiceUtility.EncodeValues(matterData.MatterClientId);
            matterData.HideUpload = ServiceUtility.EncodeValues(matterData.HideUpload);
            matterData.MatterID = ServiceUtility.EncodeValues(matterData.MatterID);
            matterData.MatterResponsibleAttorney = ServiceUtility.EncodeValues(matterData.MatterResponsibleAttorney);
            matterData.MatterModifiedDate = ServiceUtility.EncodeValues(matterData.MatterModifiedDate);
            matterData.MatterGuid = ServiceUtility.EncodeValues(matterData.MatterGuid);
            pinRequestMatterVM.MatterData = matterData;
            return await Task.FromResult(search.PinMatter(pinRequestMatterVM));
        }

        /// <summary>
        /// Unpin the matter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pinRequestData"></param>
        /// <returns></returns>
        public async Task<bool> UnPinRecordAsync<T>(T pinRequestData)
        {
            PinRequestMatterVM pinRequestMatterVM = (PinRequestMatterVM)Convert.ChangeType(pinRequestData, typeof(PinRequestMatterVM));
            return await Task.FromResult(search.UnPinMatter(pinRequestMatterVM));
        }

        /// <summary>
        /// Get the folder hierarchy
        /// </summary>
        /// <param name="matterData"></param>
        /// <returns></returns>
        public async Task<List<FolderData>> GetFolderHierarchyAsync(MatterData matterData)
        {
            return await Task.FromResult(spList.GetFolderHierarchy(matterData));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<IList<Role>> GetRolesAsync(Client client)
        {
            IList<Role> roles = new List<Role>();
            ListItemCollection collListItem = await Task.FromResult(spList.GetData(client, listNames.DMSRoleListName, camlQueries.DMSRoleQuery));
            if (null != collListItem && 0 != collListItem.Count)
            {
                foreach (ListItem item in collListItem)
                {
                    Role tempRole = new Role();
                    tempRole.Id = Convert.ToString(item[matterSettings.ColumnNameGuid], CultureInfo.InvariantCulture);
                    tempRole.Name = Convert.ToString(item[matterSettings.RoleListColumnRoleName], CultureInfo.InvariantCulture);
                    tempRole.Mandatory = Convert.ToBoolean(item[matterSettings.RoleListColumnIsRoleMandatory], CultureInfo.InvariantCulture);
                    roles.Add(tempRole);
                }
            }
            return roles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<IList<Role>> GetPermissionLevelsAsync(Client client)
        {
            IList<Role> roles = new List<Role>();
            List<RoleDefinition> roleDefinitions =  await Task.FromResult(search.GetWebRoleDefinitions(client));
            if (roleDefinitions.Count!=0)
            {
                foreach (RoleDefinition role in roleDefinitions)
                {
                    Role tempRole = new Role();
                    tempRole.Name = role.Name;
                    tempRole.Id = Convert.ToString(role.Id, CultureInfo.InvariantCulture);
                    roles.Add(tempRole);
                }
            }
            return roles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<IList<Users>> GetUsersAsync(SearchRequestVM searchRequestVM)
        {
            IList<PeoplePickerUser> foundUsers = await Task.FromResult(search.SearchUsers(searchRequestVM));
            IList<Users> users = new List<Users>();
            if(foundUsers!=null && foundUsers.Count!=0)
            {
                foreach (PeoplePickerUser item in foundUsers)
                {
                    Users tempUser = new Users();
                    tempUser.Name = Convert.ToString(item.DisplayText, CultureInfo.InvariantCulture);
                    tempUser.LogOnName = Convert.ToString(item.Key, CultureInfo.InvariantCulture);
                    tempUser.Email = string.Equals(item.EntityType, ServiceConstants.PeoplePickerEntityTypeUser, StringComparison.OrdinalIgnoreCase) ? 
                        Convert.ToString(item.Description, CultureInfo.InvariantCulture) : Convert.ToString(item.EntityData.Email, CultureInfo.InvariantCulture);
                    tempUser.EntityType = Convert.ToString(item.EntityType, CultureInfo.InvariantCulture);
                    users.Add(tempUser);
                }
                return users;
            }            
            return users;
        }

        public async Task<GenericResponseVM> GetConfigurationsAsync(string siteCollectionUrl)
        {
            return await Task.FromResult(search.GetConfigurations(siteCollectionUrl, listNames.MatterConfigurationsList));
        }
    }
}
