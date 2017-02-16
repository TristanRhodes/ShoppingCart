using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Controllers
{
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private IStockManager _stockManager;

        public StockController(IStockManager stockManager)
        {
            _stockManager = stockManager;
        }

        [HttpGet]
        public IActionResult GetStock()
        {
            var stock = _stockManager.GetStock();

            if (stock.Count == 0)
                return NoContent();

            return Json(stock);
        }
    }
}
