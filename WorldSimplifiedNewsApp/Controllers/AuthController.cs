using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WorldSimplifiedNewsApp.Models;
using WorldSimplifiedNewsApp.Models.DTOs;

namespace WorldSimplifiedNewsApp.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto registerRequest)
        {
            if (ModelState.IsValid)
            {
                // check if the email is already used
                var user_exists = await _userManager.FindByEmailAsync(registerRequest.Email);

                if(user_exists != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() { "email already exists" }
                    });
                }

                // create the new user
                var new_user = new IdentityUser()
                {
                    Email = registerRequest.Email,
                    UserName = registerRequest.UserName,
                };

                var is_created = await _userManager.CreateAsync(new_user, registerRequest.Password);

                if(is_created.Succeeded)
                {
                    // generate token
                    var jwt = await GenerateToken(new_user);

                    return Ok(jwt);
                }

                List<String> Errors = new List<string>();
                foreach(var error in is_created.Errors)
                {
                    Errors.Add(error.Description);
                }

                return BadRequest(new AuthResult()
                {
                    Errors= Errors
                });
            }
            List<string> emailErrors = new List<string>();
            foreach(var error in ModelState["Email"].Errors)
            {
                emailErrors.Add(error.ErrorMessage);
            }
            return BadRequest(new AuthResult()
            {
                Errors = emailErrors
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            // validate the incoming request
            if (ModelState.IsValid)
            {
                // check if the email exists
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);

                if (existing_user == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<String>()
                        {
                            "Invalid Payload"
                        },
                        Success = false
                    });
                }
                // check if the email and password match
                var correctPassword = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);
                
                if(correctPassword == false)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<String>()
                        {
                            "Invalid credentials"
                        },
                        Success = false
                    });
                }

                // generate the access token
                var jwt = await GenerateToken(existing_user);
                return Ok(jwt);

            }
            return BadRequest(new AuthResult()
            {
                Errors = new List<String>()
                {
                    "Invalid payload"
                },
                Success = false
            });
        }

        [HttpPost]
        [Route("NewAccessToken")]
        public async Task<IActionResult> NewAccessToken()
        {
            return Ok();
        }

        private async Task<AuthResult> GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
                Issuer = "https://localhost:7218",
                Audience = "https://localhost:7218",
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)),
                NotBefore = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true
            };
        }
    }
}
