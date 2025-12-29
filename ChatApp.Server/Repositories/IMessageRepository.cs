using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Server.Models;
namespace ChatApp.Server.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetRecentMessagesAsync(int count);
        Task AddMessageAsync(Message message);
        Task <bool> SaveChangesAsync();
    }
}