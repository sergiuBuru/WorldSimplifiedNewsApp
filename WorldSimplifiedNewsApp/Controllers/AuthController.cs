using Microsoft.AspNetCore.Mvc;

namespace WorldSimplifiedNewsApp.Controllers
{
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Resgister()
        {
            return Ok();
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
