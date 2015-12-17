// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-rijadh
// Created          : 09-23-2015
//
// ***********************************************************************
// <copyright file="ProvisionHelperFunctionsUtility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides additional methods involved in matter provisioning.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.HelperClasses
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    #endregion

    /// <summary>
    /// Provide additional methods involved in matter provisioning.
    /// </summary>
    internal static class ProvisionHelperFunctionsUtility
    {
        /// <summary>
        /// Function to get the WssID for the Practice group, Area of law and Sub area of law terms
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="matterMetadata">Object containing meta data for Matter</param>
        /// <param name="fields">Field Collection object</param>
        /// <returns>An Object containing meta data for Matter</returns>
        internal static MatterMetadata GetWSSId(ClientContext clientContext, MatterMetadata matterMetadata, FieldCollection fields)
        {
            ClientResult<TaxonomyFieldValue> practiceGroupResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnPracticeGroup))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.PracticeGroupTerm.Id);
            ClientResult<TaxonomyFieldValue> areaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnAreaOfLaw))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.AreaTerm.Id);
            ClientResult<TaxonomyFieldValue> subareaOfLawResult = clientContext.CastTo<TaxonomyField>
                (fields.GetByInternalNameOrTitle(ServiceConstantStrings.ContentTypeColumnSubareaOfLaw))
                .GetFieldValueAsTaxonomyFieldValue(matterMetadata.SubareaTerm.Id);
            clientContext.ExecuteQuery();

            matterMetadata.PracticeGroupTerm.WssId = practiceGroupResult.Value.WssId;
            matterMetadata.AreaTerm.WssId = areaOfLawResult.Value.WssId;
            matterMetadata.SubareaTerm.WssId = subareaOfLawResult.Value.WssId;
            return matterMetadata;
        }
    }
}