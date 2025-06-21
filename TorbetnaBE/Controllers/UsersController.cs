using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorbetnaBE.Data;
using TorbetnaBE.Models;
using TorbetnaBE.Methods;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TorbetnaBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(UserStatus? status)
        {
            ICollection<User> user = await _context.Users.ToListAsync();
            if (!String.IsNullOrEmpty(nameof(status)))
                user?.Where(u => u.Status == status)?.ToList();

            return Ok(user);
        }

        [HttpPost]
        [Route("/SignIn")]
        public async Task<IActionResult> SignIn([FromBody] Dtos.User.SignInModel request)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user is not null)
            {
                if (user.Password == Hash.HashPassword(request.Password))
                {
                    if (user.Status == UserStatus.Active)
                    {
                        var jwtSecret = _configuration["Jwt:Secret"];
                        var token = Token.GenerateJwtToken(user.Email!, jwtSecret);
                        return Ok(new { Token = token });
                    }
                    else
                    {
                        return BadRequest("Please Activate Your Account");
                    }
                }
            }

            return Unauthorized("Invalid Email Or Password");
        }

        //TODO: send verification email
        [HttpPost]
        [Route("/SignUp")]
        public async Task<ActionResult<User>> Signup([FromBody] Dtos.User.CreateModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            if (isEmailExist(newUser.Email))
            {
                return BadRequest("This Email Is Already Used");
            }

            User user = new User();
            user.Create(newUser);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var jwtSecret = _configuration["Jwt:Secret"];
            var token = Token.GenerateJwtToken(newUser.Email!, jwtSecret);
            return Ok("User Was Created Successfully Please Verify Your Email");
        }

        [HttpPost]
        [Route("/Update")]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dtos.User.UpdateModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var MyUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(userEmail));

            if (MyUser is null)
            {
                return NotFound("User Not Found");
            }

            MyUser.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("/Me")]
        [Authorize]
        public async Task<ActionResult<User>> Me()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var MyUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(userEmail));

            if (MyUser is not null)
            {
                return Ok(MyUser);
            }
            else
            {
                return Unauthorized("Unauthorized");
            }
        }

        [HttpPost]
        [Route("/Delete")]
        [Authorize]
        public async Task<IActionResult> DeactivateUser()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var myUser = _context.Users.FirstOrDefault(u => u.Email!.Equals(userEmail));
            if (myUser is null)
            {
                return Unauthorized("User isn't authorized");
            }

            myUser.Delete();
            await _context.SaveChangesAsync();

            return Ok("User Has Been Deleted");
        }

        public bool isEmailExist(string email)
        {
            return _context.Users.Any(u => u.Email.Equals(email));
        }
    }
}
