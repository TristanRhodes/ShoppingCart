using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Model
{
    public enum AvailabilityCheckStatus
    {
        Ok,
        ProductsNotFound,
        InsufficientStock
    }
}
