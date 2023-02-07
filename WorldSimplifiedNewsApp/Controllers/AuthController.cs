using Microsoft.AspNetCore.Mvc;

namespace WorldSimplifiedNewsApp.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Resgister()
        {
            return Ok(DotNetEnv.Env.GetString("TEST_STRING"));
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }

        [HttpPost]
        [Route("NewAccessToken")]
        public async Task<IActionResult> NewAccessToken()
        {
            return Ok();
        }
    }
}
