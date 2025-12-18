using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Server.Data;
using ChatApp.Server.Models;
namespace ChatApp.Server.Hubs
{
    
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        public ChatHub(AppDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string user, string message)
        {
            var NewMessage = new Message
            {
                Sender = user,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            _context.Messages.Add(NewMessage);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", user, message, NewMessage.Timestamp.ToShortTimeString());
        }
    }
}