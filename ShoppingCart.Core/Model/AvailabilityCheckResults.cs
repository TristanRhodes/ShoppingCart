using System.Collections.Generic;

namespace ShoppingCart.Core.Model
{
    public class AvailabilityCheckResults
    {
        public AvailabilityCheckResults()
        {
            ProductsNotFound = new List<int>();
            ProductsNotAvailable = new List<string>();
            Result = AvailabilityCheckStatus.Ok;
        }

        public AvailabilityCheckStatus Result { get; set; }

        public List<int> ProductsNotFound { get; set; }

        public List<string> ProductsNotAvailable { get; set; }
    }
}
