using Microsoft.SharePoint.Client.Taxonomy;
using System;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace Microsoft.Legal.MatterCenter.Repository.Extensions
{
    /// <summary>
    /// Extension class for sharepoint content type.
    /// </summary>
    public static class ContentTypeExtensions
    {

        /// <summary>
        /// This extension method will return all fields of the passed content type.
        /// </summary>
        /// <param name="conentTypeName"></param>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        public static FieldCollection GetFieldsInContentType(this string conentTypeName, ClientContext clientContext)
        {
            if (!string.IsNullOrWhiteSpace(conentTypeName))
            {
                ContentType ct = clientContext.Web.ContentTypes.GetByName(conentTypeName);
                //added content type null check

                if(ct!=null)
                {
                    // Gets a value that specifies the collection of fields for the content type
                    FieldCollection fieldColl = ct.Fields;
                    clientContext.Load(fieldColl);
                    clientContext.ExecuteQuery();
                    return fieldColl;
                }                
            }
            return null;
        }

        /// <summary>
        /// This method will return the content type for the string content type name passed.
        /// </summary>
        /// <param name="cts"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ContentType GetByName(this ContentTypeCollection cts, string name)
        {
            var ctx = cts.Context;
            ctx.Load(cts);
            ctx.ExecuteQuery();
            return Enumerable.FirstOrDefault(cts, ct => ct.Name == name);
        }

    }
}