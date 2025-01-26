using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Employee_Chat.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Employee_Chat.Models;
using Employee_Chat.Hubs;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]!);
var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Manager", "Employee" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
