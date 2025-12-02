using ERP_API.DataAccess.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Entities.Warehouse
{
    public class StockTransferLog
    {
        public int Id { get; set; }
        public DateTime TransferDate { get; set; }

        // Source
        public int FromWarehouseId { get; set; }
        public Warehouse FromWarehouse { get; set; }

        // Destination
        public int ToWarehouseId { get; set; }
        public Warehouse ToWarehouse { get; set; }

        // Item
        public int ProductPackageId { get; set; }
        public ProductPackage ProductPackage { get; set; }

        public decimal Quantity { get; set; }
    }
}
