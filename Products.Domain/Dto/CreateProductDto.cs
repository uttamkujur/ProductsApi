using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Dto
{
    public class CreateProductDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int AvailableStock { get; set; }
        public string Description { get; set; }
    }
}
