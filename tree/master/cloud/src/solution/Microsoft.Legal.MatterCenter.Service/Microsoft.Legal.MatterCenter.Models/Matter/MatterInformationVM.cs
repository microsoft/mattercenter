using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterInformationVM: MatterVM
    {
        public bool EditMode { get; set; }
        public bool IsConflictCheck { get; set; }
        public string MatterLocation { get; set; }
        public string MatterCreator { get; set; }
        public string MatterCreatorEmail { get; set; }
        public string RequestedUrl { get; set; }
        public IList<IList<string>> UsersNamesToRemove { get; set; }
        public bool IsFullControlPresent { get; set; }
        public string MatterAssociatedInfo { get; set; }
    }
}
