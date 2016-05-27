using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;


namespace Microsoft.Legal.MatterCenter.Service.HelperClasses
{
    /// <summary>
    /// This class will be used to validate the client input values
    /// </summary>
    internal static class ValidationHelperFunctions
    {
        internal static ErrorSettings ErrorSettings
        {
            get;
            set;
        }

        /// <summary>
        /// This method will validate the client object and if there are any errors in the client inout object, the method will
        /// return ErrorResponse object or it will return null
        /// </summary>
        /// <param name="client">Contains the information such as ClientId, ClientUrl etc</param>
        /// <returns>ErrorResponse</returns>
        internal static ErrorResponse TaxonomyValidation(Client client)
        {
            if(client!=null)
            {
                return ValidateClientInformation(client, 0);
            }
            else
            {
                ErrorResponse errorResponse = new ErrorResponse();
                errorResponse.ErrorCode = "";
                errorResponse.Message = ErrorSettings.MessageNoInputs;
                return errorResponse;
            }
            return null;
        }

        /// <summary>
        /// Function to validate client information
        /// </summary>
        /// <param name="client">Client object</param>
        /// <param name="methodNumber">Number indicating which method needs to be validated</param>
        /// <returns>ErrorResponse that contains error message with error code</returns>
        internal static ErrorResponse ValidateClientInformation(Client client, int methodNumber)
        {
            ErrorResponse errorResponse = new ErrorResponse();
            if (string.IsNullOrWhiteSpace(client.Url))
            {                
                errorResponse.ErrorCode = ErrorSettings.IncorrectInputClientUrlCode;
                errorResponse.Message = ErrorSettings.IncorrectInputClientUrlMessage;                
            }
            //else if (int.Parse(ConstantStrings.ProvisionMatterCreateMatter, CultureInfo.InvariantCulture) == methodNumber || 
            //    int.Parse(ConstantStrings.ProvisionMatterAssignContentType, CultureInfo.InvariantCulture) == methodNumber || 
            //    int.Parse(ConstantStrings.ProvisionMatterUpdateMetadataForList, CultureInfo.InvariantCulture) == methodNumber)
            //{
            //    if (string.IsNullOrWhiteSpace(client.Id))
            //    {
            //        result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, 
            //            TextConstants.IncorrectInputClientIdCode, TextConstants.IncorrectInputClientIdMessage);
            //    }
            //    else if (string.IsNullOrWhiteSpace(client.Name))
            //    {
            //        result = string.Format(CultureInfo.InvariantCulture, 
            //            ConstantStrings.ServiceResponse, TextConstants.IncorrectInputClientNameCode, 
            //            TextConstants.IncorrectInputClientNameMessage);
            //    }
            //}
            return errorResponse;
        }
    }
}
