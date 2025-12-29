using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Server.Data;
using ChatApp.Server.Models;
using ChatApp.Server.Repositories;
namespace ChatApp.Server.Hubs
{
    
    public class ChatHub : Hub
    {
        private readonly IMessageRepository _repo;
        public ChatHub(IMessageRepository repo)
        {
            
            _repo = repo;
        }
        public async Task SendMessage(string user, string message)
        {
            var NewMessage = new Message
            {
                Sender = user,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            await _repo.AddMessageAsync(NewMessage);
            await _repo.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", user, message, NewMessage.Timestamp.ToShortTimeString());
        }
    }
}