using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth0api.Controllers
{
    [Route("api")]
    public class PingController : Controller
    {
        [HttpGet]
        [Route("ping")]
        public JsonResult Ping()
        {
            return new JsonResult("Pong");
        }

        //[Authorize]
        [HttpGet]
        //[Route("ping/secure")]
        [Route("private-scoped")]
        [Authorize("read:messages")]
        public IActionResult Scoped()
        //public JsonResult PingSecured()
        {
            return Json(new
            {
                Message = "Hello from a private endpoint! You need to be authenticated and have a scope of read:messages to see this."
            });
            //return new JsonResult("All good. You only get this message if you are authenticated.");
        }
    }
}