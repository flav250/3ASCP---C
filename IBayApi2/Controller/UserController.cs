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
        private readonly Hashpassword _hashpassword;

        public UserController(ApiContext context, Hashpassword hashpassword)
        {
            _context = context;
            _hashpassword = hashpassword;
        }
        
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
        
        [Authorize]
        [HttpPut("Id")]
        public async Task<ActionResult<User>> PutUser([FromBody]User payload)
        {
            var userId = User.FindFirst("UserId")?.Value;
            
            var hashpassword = _hashpassword.Hpassword(payload.password);

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId,out int userIdToken))
            {
                return BadRequest();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userIdToken);

            if (user == null)
            {
                return NotFound();
            }

            user.email = payload.email;
            user.pseudo = payload.pseudo;
            user.password = hashpassword;
            user.role = payload.role;

            await _context.SaveChangesAsync();

            return user;
        }
        
        [Authorize]
        [HttpDelete("Id")]
        public async Task<ActionResult<User>> DeleteUser()
        {
            
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId,out int userIdToken))
            {
                return BadRequest();
            }
            
            var user = await _context.Users.FindAsync(userIdToken);

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