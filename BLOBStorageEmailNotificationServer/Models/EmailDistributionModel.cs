using System.ComponentModel.DataAnnotations;

namespace BLOBStorageEmailNotificationServer.Models
{
    public class EmailDistributionModel
    {
        [Required, EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAdress { get; set; }
    }
}
