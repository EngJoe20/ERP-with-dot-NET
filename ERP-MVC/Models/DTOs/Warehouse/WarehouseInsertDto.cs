using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Warehouse
{
    public class WarehouseInsertDto
    {
        public string Name { get; set; } // e.g. "Cairo Branch"
        public string? Location { get; set; } // e.g. "Nasr City"
    }
}
