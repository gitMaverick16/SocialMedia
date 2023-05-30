using Autenticacion.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Autenticacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public UserController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ResponseRegister>> Register(RegisterRequest form)
        {
            var user = new IdentityUser { UserName = form.Email, Email = form.Email };
            var result = await userManager.CreateAsync(user, form.Password);

            if (result.Succeeded)
                return BuiltToken(form);
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseRegister>> Login(RegisterRequest form)
        {
            var result = await signInManager.PasswordSignInAsync(form.Email, 
                form.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return BuiltToken(form);
            else
                return BadRequest("Incorrect login");
        }

        private ResponseRegister BuiltToken(RegisterRequest form)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", form.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["KeyJWT"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            return new ResponseRegister
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }
    }
}
