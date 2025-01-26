using Employee_Chat.Data;
using Employee_Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Chat.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Allow Managers and Admins to view all employees
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }

        // Allow Managers and Admins to view employee details
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Details(string id)
        {
            // Find user by their ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Extract username and fetch roles
            var username = user.Email.Split('@')[0]; // Derive username from email
            var roles = await _userManager.GetRolesAsync(user);

            // Map details to the ViewModel
            var model = new EmployeeProfileViewModel
            {
                Email = user.Email,
                Username = username,
                Roles = string.Join(", ", roles) // Join multiple roles into a single string
            };

            return View(model);
        }

        // Allow employees to view their own profile
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Extract username and fetch roles
            var username = currentUser.Email.Split('@')[0]; // Derive username from email
            var roles = await _userManager.GetRolesAsync(currentUser);

            // Map details to the ViewModel
            var model = new EmployeeProfileViewModel
            {
                Email = currentUser.Email,
                Username = username,
                Roles = string.Join(", ", roles) // Join multiple roles into a single string
            };

            return View(model);
        }

        // Allow only Admins to create employees
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // Allow only Admins to edit employees
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // Allow only Admins to delete employees
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
