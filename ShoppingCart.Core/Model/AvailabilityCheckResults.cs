using System.Collections.Generic;

namespace ShoppingCart.Core.Model
{
    public class AvailabilityCheckResults
    {
        public AvailabilityCheckResults()
        {
            ProductsNotFound = new List<int>();
            ProductsNotAvailable = new List<string>();
            Available = true;
        }

        public bool Available { get; set; }

        public List<int> ProductsNotFound { get; set; }

        public List<string> ProductsNotAvailable { get; set; }

        public bool HasNotFoundProducts
        {
            get { return !Available && ProductsNotFound.Count > 0; }
        }

        public bool HasUnavailableProducts
        {
            get { return !Available && ProductsNotAvailable.Count > 0; }
        }
    }
}
