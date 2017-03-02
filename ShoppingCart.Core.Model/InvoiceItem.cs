using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Model
{
    public class InvoiceItem
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Cost { get; set; }
    }
}
