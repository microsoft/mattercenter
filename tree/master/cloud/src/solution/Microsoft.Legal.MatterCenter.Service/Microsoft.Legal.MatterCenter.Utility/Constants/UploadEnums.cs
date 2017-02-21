
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This enums will have constants which will be used during file upload scenarios
    /// </summary>
    public enum UploadEnums
    {
        UploadSuccess=1,
        DuplicateDocument=2,
        IdenticalContent=3,
        NonIdenticalContent=4,
        ContentCheckFailed=5,
        UploadFailure=6,
        FileAlreadyExists=7,
        UploadToFolder =8
    }

    public enum UpdateMatterOperation
    {
        MatterLibrary,
        MatterPage,
        StampedProperties
    }
}
