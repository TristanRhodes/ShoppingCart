﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
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
        private IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<StockItem>), StatusCodes.Status200OK)]
        public IActionResult GetStock()
        {
            var stock = _stockRepository.GetStock();

            if (stock.Count == 0)
                return NoContent();

            return Json(stock);
        }
    }
}
