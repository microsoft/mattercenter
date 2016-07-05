﻿using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class SPContentTypes:ISPContentTypes
    {

        private ContentTypesConfig contentTypesConfig;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private CamlQueries camlQueries;
        private ISPList spList;
        public SPContentTypes(IOptionsMonitor<ContentTypesConfig> contentTypesConfig, IOptionsMonitor<CamlQueries> camlQueries, ISPList spList,
            ICustomLogger customLogger, IOptionsMonitor<LogTables> logTables
            )
        {
            this.contentTypesConfig = contentTypesConfig.CurrentValue;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
            this.camlQueries = camlQueries.CurrentValue;
            this.spList = spList;
        }

        /// <summary>
        /// This method will get all content types from the specified content type group and will filter out the content types that user has selected 
        /// when creating the matter
        /// </summary>
        /// <param name="clientContext">The sharepoint context object</param>
        /// <param name="contentTypesNames">Content Type Names that user selected in the create matter screen</param>
        /// <param name="client">The client object which contains information for which client the matter is getting created and the url of the client</param>
        /// <param name="matter">The matter information that is getting created</param>
        /// <returns></returns>
        public IList<ContentType> GetContentTypeData(ClientContext clientContext, IList<string> contentTypesNames, Client client, Matter matter)
        {
            ContentTypeCollection contentTypeCollection = null;
            IList<ContentType> selectedContentTypeCollection = new List<ContentType>();
            try
            {
                if (null != clientContext && null != contentTypesNames)
                {                   

                    Web web = clientContext.Web;
                    string contentTypeName = contentTypesConfig.OneDriveContentTypeGroup.Trim();
                    contentTypeCollection = web.ContentTypes;
                    clientContext.Load(contentTypeCollection, contentType => contentType.Include(thisContentType => thisContentType.Name).Where(currContentType => currContentType.Group == contentTypeName));
                    clientContext.ExecuteQuery();
                    selectedContentTypeCollection = GetContentTypeList(contentTypesNames, contentTypeCollection.ToList());
                }
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }

            return selectedContentTypeCollection;
        }


        /// <summary>
        /// This method will assign content types to the matter that is getting created
        /// </summary>
        /// <param name="matterMetadata"></param>
        /// <param name="clientContext"></param>
        /// <param name="contentTypeCollection"></param>
        /// <param name="client"></param>
        /// <param name="matter"></param>
        /// <returns></returns>
        public GenericResponseVM AssignContentTypeHelper(MatterMetadata matterMetadata, ClientContext clientContext,
            IList<ContentType> contentTypeCollection, Client client, Matter matter)
        {
            try
            {
                Web web = clientContext.Web;
                List matterList = web.Lists.GetByTitle(matter.Name);
                SetFieldValues(clientContext, contentTypeCollection, matterList, matterMetadata);
                clientContext.ExecuteQuery();
                SetDefaultContentType(clientContext, matterList, client, matter);
                string[] viewColumnList = contentTypesConfig.ViewColumnList.Split(new string[] { ServiceConstants.SEMICOLON }, StringSplitOptions.RemoveEmptyEntries).Select(listEntry => listEntry.Trim()).ToArray();
                string strQuery = string.Format(CultureInfo.InvariantCulture, camlQueries.ViewOrderByQuery, contentTypesConfig.ViewOrderByColumn);
                bool isViewCreated = spList.AddView(clientContext, matterList, viewColumnList, contentTypesConfig.ViewName, strQuery);
                return ServiceUtility.GenericResponse(string.Empty, 
                    Convert.ToString(isViewCreated, CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture));
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, 
                    MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Sets the default content type based on user selection for the new matter that is getting created
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="list">Name of the list</param>
        /// <param name="requestObject">Request Object</param>
        /// <param name="client">Client Object</param>
        /// <param name="matter">Matter Object</param>
        private void SetDefaultContentType(ClientContext clientContext, List list, Client client, Matter matter)
        {
            int contentCount = 0, contentSwap = 0;
            try
            {
                ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
                clientContext.Load(currentContentTypeOrder);
                clientContext.ExecuteQuery();
                IList<ContentTypeId> updatedContentTypeOrder = new List<ContentTypeId>();
                foreach (ContentType contentType in currentContentTypeOrder)
                {
                    if (0 == string.Compare(contentType.Name, matter.DefaultContentType, StringComparison.OrdinalIgnoreCase))
                    {
                        contentSwap = contentCount;
                    }

                    if (0 != string.Compare(contentType.Name, contentTypesConfig.HiddenContentType, StringComparison.OrdinalIgnoreCase))
                    {
                        updatedContentTypeOrder.Add(contentType.Id);
                        contentCount++;
                    }
                }
                if (updatedContentTypeOrder.Count > contentSwap)
                {
                    ContentTypeId documentContentType = updatedContentTypeOrder[0];
                    updatedContentTypeOrder[0] = updatedContentTypeOrder[contentSwap];
                    updatedContentTypeOrder.RemoveAt(contentSwap);
                    updatedContentTypeOrder.Add(documentContentType);
                }
                list.RootFolder.UniqueContentTypeOrder = updatedContentTypeOrder;
                list.RootFolder.Update();
                list.Update();
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Assigns field values for specified content types to the specified matter (document library).
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="contentTypeCollection">Collection of content types</param>
        /// <param name="matterList">List containing matters</param>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        private void SetFieldValues(ClientContext clientContext, IList<ContentType> contentTypeCollection, List matterList, 
            MatterMetadata matterMetadata)
        {
            FieldCollection fields = GetContentType(clientContext, contentTypeCollection, matterList);
            if (null != fields)
            {
                matterMetadata = GetWSSId(clientContext, matterMetadata, fields);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientId).DefaultValue = matterMetadata.Client.Id;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientId).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientId).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientId).Update();
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientName).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientName).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientName).DefaultValue = matterMetadata.Client.Name;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnClientName).Update();
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterId).DefaultValue = matterMetadata.Matter.Id;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterId).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterId).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterId).Update();
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterName).DefaultValue = matterMetadata.Matter.Name;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterName).ReadOnlyField = true;
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterName).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnMatterName).Update();
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnPracticeGroup).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnPracticeGroup).DefaultValue = string.Format(CultureInfo.InvariantCulture, ServiceConstants.MetadataDefaultValue, matterMetadata.PracticeGroupTerm.WssId, matterMetadata.PracticeGroupTerm.TermName, matterMetadata.PracticeGroupTerm.Id);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnPracticeGroup).Update();
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnAreaOfLaw).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnAreaOfLaw).DefaultValue = string.Format(CultureInfo.InvariantCulture, ServiceConstants.MetadataDefaultValue, matterMetadata.AreaTerm.WssId, matterMetadata.AreaTerm.TermName, matterMetadata.AreaTerm.Id);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnAreaOfLaw).Update();
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnSubareaOfLaw).SetShowInDisplayForm(true);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnSubareaOfLaw).DefaultValue = string.Format(CultureInfo.InvariantCulture, ServiceConstants.MetadataDefaultValue, matterMetadata.SubareaTerm.WssId, matterMetadata.SubareaTerm.TermName, matterMetadata.SubareaTerm.Id);
                fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnSubareaOfLaw).Update();
            }
        }

        /// <summary>
        /// Function to get the WssID for the Practice group, Area of law and Sub area of law terms
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="matterMetadata">Object containing meta data for Matter</param>
        /// <param name="fields">Field Collection object</param>
        /// <returns>An Object containing meta data for Matter</returns>
        private MatterMetadata GetWSSId(ClientContext clientContext, MatterMetadata matterMetadata, FieldCollection fields)
        {
            ClientResult<TaxonomyFieldValue> practiceGroupResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnPracticeGroup))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.PracticeGroupTerm.Id);
            ClientResult<TaxonomyFieldValue> areaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnAreaOfLaw))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.AreaTerm.Id);
            ClientResult<TaxonomyFieldValue> subareaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(contentTypesConfig.ContentTypeColumnSubareaOfLaw))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.SubareaTerm.Id);
            clientContext.ExecuteQuery();

            matterMetadata.PracticeGroupTerm.WssId = practiceGroupResult.Value.WssId;
            matterMetadata.AreaTerm.WssId = areaOfLawResult.Value.WssId;
            matterMetadata.SubareaTerm.WssId = subareaOfLawResult.Value.WssId;
            return matterMetadata;
        }

        /// <summary>
        /// Retrieves the list of content types that are to be associated with the matter.
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="contentTypeCollection">Collection of content types</param>
        /// <param name="matterList">List containing matters</param>
        /// <returns>Content types in Field Collection object</returns>
        internal static FieldCollection GetContentType(ClientContext clientContext, IList<ContentType> contentTypeCollection, List matterList)
        {
            foreach (ContentType contenttype in contentTypeCollection)
            {
                matterList.ContentTypesEnabled = true;
                matterList.ContentTypes.AddExistingContentType(contenttype);
            }

            matterList.Update();
            FieldCollection fields = matterList.Fields;
            clientContext.Load(fields);
            clientContext.Load(matterList);
            clientContext.ExecuteQuery();
            return fields;
        }

        /// <summary>
        /// Generates the list of Content Types that are associated with matter.
        /// </summary>
        /// <param name="contentTypesNames">List of Content Type names that user has selected</param>
        /// <param name="contentTypeList">Content Types listed in Content Type hub under Matter Center group</param>
        /// <returns>List of Content Types associated with matter</returns>
        private static IList<ContentType> GetContentTypeList(IList<string> contentTypesNames, List<ContentType> contentTypeList)
        {
            IList<ContentType> selectedContentTypeCollection = new List<ContentType>();
            ContentType selectedContentType = null;
            foreach (string contentTypeName in contentTypesNames)
            {
                selectedContentType = (from currContentType in contentTypeList
                                       where currContentType.Name.ToUpperInvariant().Equals(contentTypeName.ToUpperInvariant())
                                       select currContentType).ToList().FirstOrDefault();
                if (null != selectedContentType)
                {
                    selectedContentTypeCollection.Add(selectedContentType);
                }
            }
            return selectedContentTypeCollection;
        }
    }
}
