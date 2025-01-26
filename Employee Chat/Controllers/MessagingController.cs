using Employee_Chat.Data;
using Employee_Chat.Hubs;
using Employee_Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Chat.Controllers
{
    [Authorize]
    public class MessagingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagingController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users); // Display list of users to chat with
        }

        public async Task<IActionResult> Chat(string receiverId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (string.IsNullOrWhiteSpace(receiverId))
            {
                return BadRequest("Receiver ID is missing.");
            }

            var receiver = await _userManager.FindByIdAsync(receiverId);
            if (receiver == null)
            {
                return NotFound("Receiver not found.");
            }

            var messages = _context.Messages
                .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == currentUser.Id))
                .OrderBy(m => m.Timestamp)
                .ToList();

            ViewBag.CurrentUserId = currentUser.Id;
            ViewBag.CurrentUserEmail = currentUser.Email;
            ViewBag.ReceiverId = receiverId;
            ViewBag.ReceiverEmail = receiver.Email;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Message content cannot be empty.");
            }

            var message = new Message
            {
                SenderId = currentUser.Id,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            // Save the message to the database
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Notify the receiver in real-time
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<ChatHub>>();
            await hubContext.Clients.User(receiverId).SendAsync("ReceiveMessage", currentUser.Email, content, message.Timestamp.ToString("g"));

            // Also notify the sender (to update their UI)
            await hubContext.Clients.User(currentUser.Id).SendAsync("ReceiveMessage", currentUser.Email, content, message.Timestamp.ToString("g"));

            return Ok();
        }
    }
}
