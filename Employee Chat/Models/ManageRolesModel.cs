using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Employee_Chat.Models
{
    public class ManageRolesModel
    {
        public List<IdentityUser> Users { get; set; } // List of users
        public List<string> Roles { get; set; } // List of role names
    }
}
