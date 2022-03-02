using Microsoft.AspNetCore.Mvc;
using web.auth;

namespace weblib.Controllers
{
    public interface INotImplemented
    {
        void NotImplementedSoAlsoNotRegisteredInDI();
    }

    [CustomApiVersion("1-notimplemented")]
    [ApiController]
    [Route("broken")]
    public class BrokenController : ControllerBase
    {
        private readonly INotImplemented notImplemented;

        public BrokenController(INotImplemented notImplemented)
        {
            this.notImplemented = notImplemented;
        }

        [HttpGet]
        public ActionResult Get()
        {
            this.notImplemented.NotImplementedSoAlsoNotRegisteredInDI();
            return Ok();
        }
    }
}
