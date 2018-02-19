using auth0api.Data;
using auth0api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace auth0api.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        public TokenController(ApplicationDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        private readonly ApplicationDbContext _dbContext;
        private IConfiguration _config;

        //private IConfiguration _config;

        //public TokenController(IConfiguration config)
        //{
        //    _config = config;
        //}

        //private readonly ApplicationDbContext _dbContext;

        //public LoginRequestHandler(ApplicationDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]ApplicationUser login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string BuildToken(ApplicationUser user)
        {
            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
                claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ApplicationUser Authenticate([FromBody]ApplicationUser message)
        {
            //UserModel user = null;
            IActionResult response = Unauthorized();
            var user = _dbContext.Users.SingleOrDefault(u => u.ExternalId == message.ExternalId);
            //ApplicationUser user = _dbContext.Users.SingleOrDefault(u => u.ExternalId == message.ExternalId);

            if (user == null)
            {
                _dbContext.Users.Add(new ApplicationUser
                {
                    ExternalId = message.ExternalId,
                    Username = message.Username,
                    Email = message.Email,
                    FirstName = message.FirstName,
                    LastName = message.LastName
                });
                return user;
                //UserModel user = null;

                //if (login.Username == "mario" && login.Password == "secret")
                //{
                //    user = new UserModel { Name = "Mario Rossi", Email = "mario.rossi@domain.com" };
                //}
               
            }
            else
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
                
               
            }
            return user;
            //return response;
        }



//        var existingUser = _dbContext.Users.SingleOrDefault(u => u.ExternalId == message.ExternalId);

    //        if (existingUser == null)
    //        {
    //             _dbContext.Users.Add(new ApplicationUser
    //            {
    //                ExternalId = message.ExternalId,
    //                Username = message.Username,
    //                Email = message.Email,
    //                FirstName = message.FirstName,
    //                LastName = message.LastName
    //});

//            }
//            else
//            {
//                var tokenString = BuildToken(existingUser);
//response = Ok(new { token = tokenString });
//                //existingUser.Username = message.Username;
//                //existingUser.Email = message.Email;
//                //existingUser.FirstName = message.FirstName;
//                //existingUser.LastName = message.LastName;
//            }
//            _dbContext.SaveChanges();














        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        //private class UserModel
        //{
        //    public string Name { get; set; }
        //    public string Email { get; set; }
        //    public DateTime Birthdate { get; set; }
        //}
    }
}