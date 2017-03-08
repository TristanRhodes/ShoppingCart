using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Model
{
    public class ProductIdentifier
    {
        public ProductIdentifier(int? productId, string productName)
        {
            ProductId = productId;
            ProductName = productName;
        }

        public ProductIdentifier(int productId)
        {
            ProductId = productId;
            ProductName = null;
        }

        public ProductIdentifier(string productName)
        {
            ProductId = null;
            ProductName = productName;
        }

        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        public bool Valid
        {
            get
            {
                return !((!ProductId.HasValue && string.IsNullOrEmpty(ProductName))
                    || (ProductId.HasValue && !string.IsNullOrEmpty(ProductName)));
            }
        }
    }
}
