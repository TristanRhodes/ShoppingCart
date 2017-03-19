using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Controllers
{
    public class DiagnosticsController : Controller
    {
        [HttpGet("diagnostics/heartbeat")]
        public IActionResult Heartbeat()
        {
            return Ok();
        }

        [HttpPost("diagnostics/recycle")]
        public IActionResult Recycle()
        {
            //TODO: Add recycle logic



            return Ok();
        }
    }
}
