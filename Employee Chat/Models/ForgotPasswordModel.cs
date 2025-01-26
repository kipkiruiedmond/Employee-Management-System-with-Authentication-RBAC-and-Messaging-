using System.ComponentModel.DataAnnotations;

namespace Employee_Chat.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
