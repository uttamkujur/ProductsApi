using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Dto
{
    public class UpdateProductDto
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int AvailableStock { get; set; }
    }

    public class DecrementStockDto
    {
        public int ProductId { get; set; }  
        public int Quantity { get; set; }      
    }

    public class IncrementStockDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
  


