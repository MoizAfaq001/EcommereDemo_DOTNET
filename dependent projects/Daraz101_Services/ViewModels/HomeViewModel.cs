using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daraz101_Services
{
    public class HomeViewModel
    {
        private string searchTerm;

        public required List<ProductDTO> FeaturedProducts { get; set; }
        public required List<ProductDTO> AllProducts { get; set; }
        public string SearchTerm { get => searchTerm; set => searchTerm = value; }
    }
}

