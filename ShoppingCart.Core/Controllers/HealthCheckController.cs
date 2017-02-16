using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Controllers
{
    public class HealthCheckController : Controller
    {
        [Route("heartbeat")]
        public IActionResult Heartbeat()
        {
            return Ok();
        }
    }
}
