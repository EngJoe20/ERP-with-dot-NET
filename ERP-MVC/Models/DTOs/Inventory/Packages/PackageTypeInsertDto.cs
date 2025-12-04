using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Packages
{
    public class PackageTypeInsertDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string UnitOfMeasurement { get; set; } // e.g. "KG", "Liter"
    }
}
