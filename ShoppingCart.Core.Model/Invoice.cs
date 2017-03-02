using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Model
{
    public class Invoice
    {
        public string User { get; set; }

        public decimal Total { get; set; }

        public List<InvoiceItem> Items { get; set; }
    }
}
