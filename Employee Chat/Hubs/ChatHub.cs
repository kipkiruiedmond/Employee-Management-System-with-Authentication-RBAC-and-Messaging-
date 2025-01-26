using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Employee_Chat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string content)
        {
            var timestamp = DateTime.Now.ToString("g");
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content, timestamp);
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, content, timestamp);
        }
    }
}
