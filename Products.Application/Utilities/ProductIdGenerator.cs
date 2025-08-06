using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Utilities
{
    public class ProductIdGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        // This method generates a unique 6-digit ID
        public static string GenerateProductId()
        {
            lock (_lock)
            {
                // Generate the base ID from a timestamp (to ensure uniqueness over time)
                var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");

                // Combine the timestamp with a random 3-digit number for variability
                var randomPart = _random.Next(100, 999).ToString();

                // Ensure the product ID is always 6 digits
                string productId = timestamp.Substring(0, 3) + randomPart;

                return productId;
            }
        }
    }
}

