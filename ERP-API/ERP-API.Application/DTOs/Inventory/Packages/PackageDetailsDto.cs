using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Packages
{
    public class PackageDetailsDto
    {
        public string PackageTypeName { get; set; }
        public string UnitOfMeasurement { get; set; }

        // A list of products using this package
        public List<string> ProductsUsingThis { get; set; }
    }
}
