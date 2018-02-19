using System.Threading.Tasks;
using auth0api.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

//new
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth0api.Users;
using auth0api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace auth0api.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        //public AccountController(IMediator mediator, ApplicationDbContext dbContext) : base(mediator) {
        //    _dbContext = dbContext;
        //}
        public AccountController(ApplicationDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        private readonly ApplicationDbContext _dbContext;
        private IConfiguration _config;

        //public class AccountController : ControllerBase
        //    public class AccountController : Controller
        //{
        //    //public AccountController(IMediator mediator, ApplicationDbContext dbContext) : base(mediator) {
        //    //    _dbContext = dbContext;
        //    //}
        //    public AccountController(ApplicationDbContext dbContext, IConfiguration config)
        //    {
        //        _dbContext = dbContext;
        //        _config = config;
        //    }
        //    private readonly ApplicationDbContext _dbContext;
        //    private IConfiguration _config;



        //private readonly ApplicationDbContext _dbContext;



        //required for the 2.0 core jwt bearer version
        //public AccountController(IConfiguration config)
        //{
        //    _config = config;
        //}

        //[HttpPost]
        //[Route("login")]
        //[Authorize]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    await Mediator.Send(request);

        //    return new NoContentResult();
        //}



        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]ApplicationUser message)
        {
            IActionResult response = Unauthorized();

            var existingUser = _dbContext.Users.SingleOrDefault(u => u.ExternalId == message.ExternalId);

            if ( existingUser == null)
            {
                 _dbContext.Users.Add(new ApplicationUser
                {
                    ExternalId = message.ExternalId,
                    Username = message.Username,
                    Email = message.Email,
                    FirstName = message.FirstName,
                    LastName = message.LastName
                });

            }
            else
            {
                var tokenString = BuildToken(existingUser);
                response = Ok(new { token = tokenString });
                //existingUser.Username = message.Username;
                //existingUser.Email = message.Email;
                //existingUser.FirstName = message.FirstName;
                //existingUser.LastName = message.LastName;
            }
            _dbContext.SaveChanges();
            return Ok(response);
            //if ( _dbContext.Users.SingleOrDefault(u => u.ExternalId == login.ExternalId) != null) { 
            // var user = _dbContext.Users.SingleOrDefault(u => u.ExternalId == login.ExternalId);

            //if (user != null)
            //{
            //    var tokenString = BuildToken(user);
            //    response = Ok(new { token = tokenString });
            //}

            // return response;
            // }
            //else ()
            /////////////////////////////////////////////////////////
            //var existingUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.ExternalId == message.ExternalId);

            //if (existingUser == null)
            //{
            //    await _dbContext.Users.AddAsync(new ApplicationUser
            //    {
            //        ExternalId = message.ExternalId,
            //        Username = message.Username,
            //        Email = message.Email,
            //        FirstName = message.FirstName,
            //        LastName = message.LastName
            //    });

            //}
            //else
            //{
            //    existingUser.Username = message.Username;
            //    existingUser.Email = message.Email;
            //    existingUser.FirstName = message.FirstName;
            //    existingUser.LastName = message.LastName;
            //}

            //await _dbContext.SaveChangesAsync();
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

        //private UserModel Authenticate(LoginModel login)
        //{
        //    public async Task<IActionResult> Authenticationfunction([FromBody] LoginRequest request)
        //{
        



        //    var existingUser = _dbContext.Users.SingleOrDefault(u => u.ExternalId == request.ExternalId);

        //    if (existingUser == null)
        //    {
        //        _dbContext.Users.Add(new ApplicationUser
        //        {
        //            ExternalId = request.ExternalId,
        //            Username = request.Username,
        //            Email = request.Email,
        //            FirstName = request.FirstName,
        //            LastName = request.LastName
        //        });
               
        //    }
        //    else
        //    {
        //        existingUser.Username = request.Username;
        //        existingUser.Email = request.Email;
        //        existingUser.FirstName = request.FirstName;
        //        existingUser.LastName = request.LastName;
                
        //    }
        //    //return existingUser;
        //    //_dbContext.SaveChanges();
        //    //await Mediator.Send(request);

        //    //return new NoContentResult();
        //    //await _dbContext.SaveChangesAsync();
        //}

        //public class LoginModel
        //{
        //    public string Username { get; set; }
        //    public string Password { get; set; }
        //}

        //private class UserModel
        //{
        //    public string Name { get; set; }
        //    public string Email { get; set; }
        //    public DateTime Birthdate { get; set; }
        //}

    }





    

}
