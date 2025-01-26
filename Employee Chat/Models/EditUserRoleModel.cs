using System.Collections.Generic;

namespace Employee_Chat.Models
{
    public class EditUserRoleModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string SelectedRole { get; set; }
        public List<string> AllRoles { get; set; }
        public List<SelectRole> Roles { get; set; } 
    }

    public class SelectRole
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }


}
