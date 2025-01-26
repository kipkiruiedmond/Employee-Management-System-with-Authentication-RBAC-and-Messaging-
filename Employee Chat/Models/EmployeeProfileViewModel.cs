namespace Employee_Chat.Models
{
    public class EmployeeProfileViewModel
    {
        public string Email { get; set; }
        public string Username { get; set; } // Holds the username derived from email
        public string Roles { get; set; }    // Holds roles assigned to the user
    }

}
