using Microsoft.SharePoint.Client.Taxonomy;
using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Legal.MatterCenter.Repository.Extensions
{
    public static class ContentTypeExtensions
    {
        /// <summary>
        /// This method will load the child terms for a particular parent term with required term properties
        /// </summary>
        /// <param name="term">Parent Term</param>
        /// <param name="context">SharePoint Context</param>
        /// <returns></returns>
        public static FieldCollection GetFieldsInContentType(this string conentTypeName, ClientContext clientContext)
        {
            //// Get the content type using ID: 0x01003D7B5A54BF843D4381F54AB9D229F98A - is the ID of the "Custom" content Type
            ContentType ct = clientContext.Web.ContentTypes.GetByName(conentTypeName);

            //// Gets a value that specifies the collection of fields for the content type
            FieldCollection fieldColl = ct.Fields;

            clientContext.Load(fieldColl);
            clientContext.ExecuteQuery();
            return fieldColl;
        }

    }
}

