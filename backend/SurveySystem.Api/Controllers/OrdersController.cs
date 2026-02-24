using Microsoft.AspNetCore.Mvc;

namespace SurveySystem.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("import")]
        public void ImportOrders(IFormFile file)
        {
        }
    }
}
