using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Employee_Chat.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Sockets;
using Employee_Chat.Data;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;

    public AccountController(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager,
                             RoleManager<IdentityRole> roleManager,
                             IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
    }

    // GET: Register
    [HttpGet]
    public IActionResult Register()
    {
        // Fetch roles for the dropdown
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        ViewBag.Roles = roles;

        return View();
    }

    // POST: Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { userId = user.Id, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Confirm Your Email",
                $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

            return RedirectToAction("ConfirmEmailPrompt");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return View(model);
    }

    // GET: ConfirmEmail
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return BadRequest("Invalid email confirmation request.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            Console.WriteLine("Redirecting to Login after email confirmation.");
            return RedirectToAction("Login");
        }

        return BadRequest("Email confirmation failed.");
    }

    // GET: ConfirmEmailPrompt
    [HttpGet]
    public IActionResult ConfirmEmailPrompt()
    {
        return View();
    }

    // GET: Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError(string.Empty, "You must confirm your email before logging in.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
        if (result.Succeeded)
            return RedirectToAction("Index", "Dashboard");

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    // POST: Logout

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    // GET: ForgotPassword
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST: ForgotPassword
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

        // Send email
        await _emailSender.SendEmailAsync(user.Email, "Reset Password",
            $"Please reset your password by clicking here: <a href='{resetLink}'>link</a>");

        return RedirectToAction("ForgotPasswordConfirmation");
    }

    // GET: ForgotPasswordConfirmation
    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    // GET: ResetPassword
    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            return BadRequest("Invalid password reset token.");
        }

        return View(new ResetPasswordModel { Token = token, Email = email });
    }

    // POST: ResetPassword
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction("ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // GET: ResetPasswordConfirmation
    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ManageUsers()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new Dictionary<string, List<string>>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.ToList();
        }

        var model = new ManageUsersModel
        {
            Users = users,
            UserRoles = userRoles
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ManageRoles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> EditUserRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        var model = new EditUserRoleModel
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty, // Handle possible null reference
            SelectedRole = userRoles.FirstOrDefault() ?? string.Empty, // Handle possible null reference
            AllRoles = allRoles,
            Roles = allRoles.Select(role => new SelectRole
            {
                RoleName = role,
                IsSelected = userRoles.Contains(role)
            }).ToList()
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> EditUserRole(EditUserRoleModel model)
    {
        if (model.Roles == null)
        {
            ModelState.AddModelError(string.Empty, "Roles data is missing.");
            return RedirectToAction("EditUserRole", new { userId = model.UserId });
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
            return NotFound();

        // Remove current roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Add selected roles
        foreach (var role in model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName))
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction("ManageUsers");
    }





}
