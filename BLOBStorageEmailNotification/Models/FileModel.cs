using System.ComponentModel.DataAnnotations;

namespace ReenbitTestTask.Models
{
    public class EmailModel
    {
        [Required, EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAdress { get; set; }
        public string? FileLink { get; set; }

    }
}
