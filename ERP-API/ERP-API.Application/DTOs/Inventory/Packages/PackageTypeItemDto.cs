using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Inventory.Packages
{
    public class PackageTypeItemDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string UnitOfMeasurement { get; set; }
    }
}
