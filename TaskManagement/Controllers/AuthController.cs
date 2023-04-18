using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TaskManagement.Data;
using TaskManagement.Model;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly TaskApiDbContext dbContext;

        public AuthController(IConfiguration configuration ,TaskApiDbContext dbContext )
        {
            _configuration = configuration;
           this. dbContext = dbContext;
        }
     //   getallthe user

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await dbContext.Users.ToListAsync());

        }

    //    userRegistration 
        [HttpPost("register")]
        public async Task<IActionResult>Register([FromBody] UserDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var usern = new User()
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName=request.FirstName,
                LastName=request.LastName,
            };
            await dbContext.Users.AddAsync(usern);
            await dbContext.SaveChangesAsync();

          return Ok(usern);
        }

       // userlogin
        [HttpPost("login")]
        public async Task <ActionResult> Login([FromBody] UserDto request)
        {

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user==null)
            {
                return BadRequest("user not found");
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Invalid password");
            }
            

            string token = CreateToken(user);
            user.Token= token;
       
            return Ok(user);
        }


        //userUpdate
        [HttpPut,Authorize]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateContact([FromRoute] int id, UserUpdateDto userupdate)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.FirstName = userupdate.FirstName;
                user.LastName = userupdate.LastName;
                await dbContext.SaveChangesAsync();
                return Ok(user);
            }
            return NotFound();
        }








        //create hash password

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }
       // verify hash password

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

      //  Token Genration
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


    }



}

