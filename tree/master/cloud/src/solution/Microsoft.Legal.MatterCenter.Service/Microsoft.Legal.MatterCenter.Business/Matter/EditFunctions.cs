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

namespace Microsoft.Legal.MatterCenter.Business
{
    public class EditFunctions : IEditFunctions
    {
        private MatterSettings matterSettings;
        private ErrorSettings errorSettings;
        private ListNames listNames;
        private CamlQueries camlQueries;
        private ISPList spList;
        private IMatterRepository matterRespository;
        public EditFunctions(ISPList spList, IOptions<MatterSettings> matterSettings, 
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

        

    }
}
