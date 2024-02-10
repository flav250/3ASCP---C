using System.Net;
using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IBayApi2.Controller

{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        
        private readonly ApiContext _context;

        public UserController(ApiContext context)
        {
            _context = context;
        }
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var users =await _context.Users.Select(u => new User
                { Id = u.Id, pseudo = u.pseudo, email = u.email, role = u.role }).ToListAsync();
            return users;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<User>> GetUserById(int Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        

        [HttpPut("{Id}")]
        public async Task<ActionResult<User>> PutUser(int Id, [FromBody]User payload)
        {
            var user = await _context.Users.FirstOrDefaultAsync(U => U.Id == Id);

            if (user == null)
            {
                return NotFound();
            }

            user.email = payload.email;
            user.pseudo = payload.pseudo;
            user.password = payload.password;
            user.role = payload.role;

            await _context.SaveChangesAsync();

            return user;
        }
        
        [HttpDelete("{Id}")]
        public async Task<ActionResult<User>> DeleteUser(int Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}