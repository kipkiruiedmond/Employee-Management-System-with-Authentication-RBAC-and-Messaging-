using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Employee_Chat.Models
{
    public class ManageUsersModel
    {
        // List of all users in the system
        public List<IdentityUser> Users { get; set; }

        // Mapping of user IDs to their assigned roles
        public Dictionary<string, List<string>> UserRoles { get; set; }
    }
}
