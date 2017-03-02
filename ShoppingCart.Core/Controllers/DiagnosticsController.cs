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
        [Route("diagnostics/heartbeat")]
        public IActionResult Heartbeat()
        {
            return Ok();
        }

        [Route("diagnostics/recycle")]
        public IActionResult Recycle()
        {
            //TODO: Add recycle logic



            return Ok();
        }
    }
}
