using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Employee_Chat.Controllers
{
    [Authorize] // Ensure only logged-in users can access this controller
    public class DashboardController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
           

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                return RedirectToAction("ManagerDashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "Employee"))
            {
                return RedirectToAction("EmployeeDashboard");
            }

            return Unauthorized(); // If no valid role is found
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ManagerDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}
