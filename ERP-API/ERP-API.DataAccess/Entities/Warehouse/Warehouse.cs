using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Warehouse
{
    public class Warehouse
    {
        public int Id { get; set; }

        public string Name { get; set; } // e.g., "Main Virtual Warehouse", "Cairo Branch"
        public string? Location { get; set; } // e.g., "Nasr City, Zone 6"

        // Important: This flag helps us find the "Default" warehouse via code easily
        public bool IsMainWarehouse { get; set; }

        // Relationship: One Warehouse has many Stock items
        public ICollection<WarehouseStock> StockItems { get; set; }
    }
}
