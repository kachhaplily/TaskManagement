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


namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {

        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly TaskApiDbContext dbContext;
        private readonly string _googleSmtpEmail = "lilykumarikachhap@gmail.com"; // Replace with your actual Google email address
        private readonly string _googleSmtpPassword = "zbghgycjklabzjcg"; // Replace with your actual Google email password


        public AuthController(IConfiguration configuration, TaskApiDbContext dbContext)
        {
            _configuration = configuration;
            this.dbContext = dbContext;

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

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var usern = new User()
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
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
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Invalid password");
            }


            string token = CreateToken(user);
            user.Token = token;

            return Ok(user);
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
                return Ok(user);
            }
            return NotFound();
        }







        // Forgot password
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                // Return a response indicating that the email is not registered
                return NotFound("Email not found.");
            }

            // Generate a new password
            var newPassword = GenerateRandomPassword();

            // Update user's password in the database
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await dbContext.SaveChangesAsync();

            // Send password reset email
            await SendPasswordResetEmail(request.Email, newPassword);

            return Ok("Password reset successful. Please check your email for the new password.");
        }

        // Helper method to send password reset email
        private async Task SendPasswordResetEmail(string email, string newPassword)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_googleSmtpEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = "Password Reset";
                mailMessage.Body = $"Your new password is: {newPassword}";

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_googleSmtpEmail, _googleSmtpPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }

        // Helper method to generate a random password
        private string GenerateRandomPassword()
        {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString();
            char[] password = new char[10];
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                password[i] = guidString[random.Next(guidString.Length)];
            }

            return new string(password);
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

