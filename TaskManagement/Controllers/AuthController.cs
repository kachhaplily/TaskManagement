using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using TaskManagement.Data;
using TaskManagement.Model;
using TaskManagement.Helper;
using Org.BouncyCastle.Asn1.Ocsp;
using TaskManagement.UtlityServices;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {

        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly TaskApiDbContext dbContext;
        private readonly IEmailService _emailservice;
        


        public AuthController(IConfiguration configuration, TaskApiDbContext dbContext, IEmailService emailService)
        {
            _configuration = configuration;
            this.dbContext = dbContext;
            _emailservice = emailService;
        }
        //   getallthe user

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await dbContext.Users.ToListAsync());

        }

        //    userRegistration 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto request)
        {

            // Check if the email already exists in the database
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                // Return a response indicating that the email is already registered
                return Conflict("Email is already registered.");
            }

            
            var usern = new User()
            {
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            await dbContext.Users.AddAsync(usern);
            await dbContext.SaveChangesAsync();



            return Ok("registration Sucessfully");
        }

        // userlogin
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] userloginDto request)
        {

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                return BadRequest("user not found");
            };
            if (!PasswordHasher.Verifypassword(request.Password, user.Password))
            {
                return BadRequest("Invalid password");
            };


            string token = CreateToken(user);
            user.Token = token;

            return Ok(new
            {
                user.Id,
                user.Token
            }) ;
        }


        //userUpdate
        [HttpPut, Authorize]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateContact([FromRoute] int id, UserUpdateDto userupdate)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.FirstName = userupdate.FirstName;
                user.LastName = userupdate.LastName;
                await dbContext.SaveChangesAsync();
                return Ok(new
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Update sucessfully",
                    user.Id,
                    user.FirstName,
                    user.LastName
                }); ;
            }
            return NotFound();
        }

        //forgotpassword
        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email Dosen't Exist"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            Console.WriteLine(tokenBytes);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!", EmailBody.EmailStringBody(email, emailToken));
            _emailservice.SendEmail(emailModel);
            dbContext.Entry(user).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!"
            });
        }

        //reset password
        [HttpPost("reset-password")]
        public async Task <IActionResult> ResetPassword(ResetPasswordDto resetpassword)
        {
            var newToken = resetpassword.EmailToken.Replace(" ", "+");
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == resetpassword.Email);
            if(user == null) { return NotFound("user Not found"); }
            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            if (tokenCode != resetpassword.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest("invalid reset link");
            };
            user.Password = PasswordHasher.HashPassword(resetpassword.NewPassword);
            dbContext.Entry(user).State=EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return Ok("Password reset sucessfully");

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
                new Claim(ClaimTypes.Email,user.Email),

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

