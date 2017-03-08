using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Model
{
    public enum BasketOperationStatus
    {
        /// <summary>
        /// Can complete basket operation
        /// </summary>
        Ok,

        /// <summary>
        /// One or more products was not found
        /// </summary>
        ProductNotFound,

        /// <summary>
        /// One or more products did not have sufficient stock to fulfill the request.
        /// </summary>
        InsufficientStock,

        /// <summary>
        /// The identifier(s) supplied for this basket operation were invalid.
        /// </summary>
        InvalidIdentifier,

        /// <summary>
        /// The item does not exist in the basket.
        /// </summary>
        NotInBasket
    }
}
