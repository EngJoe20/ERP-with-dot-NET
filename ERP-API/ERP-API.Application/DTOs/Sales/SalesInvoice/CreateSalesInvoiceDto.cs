using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Sales.SalesInvoice
{
    public class CreateSalesInvoiceDto
    {
        public Guid CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; } 
        public decimal? Discount { get; set; } 
        public decimal? AmountReceived { get; set; } // المبلغ المستلم اختياري
        public List<SalesInvoiceItemDto> Items { get; set; } = new();
    }
}