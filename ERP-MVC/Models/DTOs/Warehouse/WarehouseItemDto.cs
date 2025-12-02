using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_MVC.Models.DTOs.Warehouse
{
    public class WarehouseItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public bool IsMainWarehouse { get; set; }
    }
}
