using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    public class SpList
    {
        public static List<string> MatterAssociatedLists(ClientContext clientContext, string matterName)
        {
            List<string> lists = new List<string>();
            lists.Add(matterName);
            lists.Add(matterName + Constants.ONENOTE);
            lists.Add(matterName + Constants.CALENDAR);
            lists.Add(matterName + Constants.TASK);            
            List<string> listExists = MatterAssociatedLists(clientContext, new ReadOnlyCollection<string>(lists));
            return listExists;
        }

        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        private static List<string> MatterAssociatedLists(ClientContext clientContext, ReadOnlyCollection<string> listsNames)
        {
            List<string> existingLists = new List<string>();
            try
            {
                if (null != clientContext && null != listsNames)
                {
                    //ToDo: Chec

                    foreach (string listName in listsNames)
                    {
                        ListCollection listCollection = clientContext.Web.Lists;
                        var lists = clientContext.LoadQuery(listCollection.Where(listTemp => listTemp.Title == listName));
                        clientContext.ExecuteQuery();
                        var list = lists.FirstOrDefault();
                        if (list != null)
                        {
                            existingLists.Add(listName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return existingLists;
        }

    }
}
