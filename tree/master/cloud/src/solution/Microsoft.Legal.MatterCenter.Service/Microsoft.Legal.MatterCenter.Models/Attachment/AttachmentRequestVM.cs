
namespace Microsoft.Legal.MatterCenter.Models
{
    public class AttachmentRequestVM
    {
        public Client Client { get; set; }
        public ServiceRequest ServiceRequest { get; set; }
        public string RequestedUrl { get; set; }
        public string RequestedPageUrl { get; set; }
    }
}
