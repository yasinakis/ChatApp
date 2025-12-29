using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Server.Models;
using ChatApp.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Repositories
{
    public class MessageRepository : IMessageRepository
    { 
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context) => _context = context;
        public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int count)
        {
            return await _context.Messages
                                 .OrderByDescending(m => m.Timestamp)
                                 .Take(count)
                                 .OrderBy(m => m.Timestamp)
                 
                                 .ToListAsync();
        }
        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }
        public async Task <bool> SaveChangesAsync() => await _context.SaveChangesAsync() >= 0;
    }
}