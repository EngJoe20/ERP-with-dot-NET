using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesInvoice
{
    public class SalesInvoiceItemDto
    {
        public int ProductPackageId { get; set; } 
        public int Quantity { get; set; } 
        public int UnitCount { get; set; } 
        public decimal SellingPrice { get; set; }//per unit
        //total= Quantity* UnitCount *SellingPrice -> auto-calculated
    }
}
