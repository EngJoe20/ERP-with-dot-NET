using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Warehouse
{
    public class WarehouseInsertDto
    {
        public required string Name { get; set; } // e.g. "Cairo Branch"
        public string? Location { get; set; } // e.g. "Nasr City"
    }
}
