using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Server.Data;
using ChatApp.Server.Models;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MessagesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
           var messages = await _context.Messages
                .OrderBy(m => m.Timestamp)
                .Take(50)
                .ToListAsync();

            return Ok(messages);
        }

        
    }
}