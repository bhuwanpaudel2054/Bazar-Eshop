using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Models
{
    public class InvoiceDetails
    {
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public virtual Invoice Invoice { get;set; }
        public virtual Product Product { get; set; }
    }
}
