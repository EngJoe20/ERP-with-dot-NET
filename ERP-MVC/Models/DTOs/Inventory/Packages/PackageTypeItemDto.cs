using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Inventory.Packages
{
    public class PackageTypeItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasurement { get; set; }
    }
}
