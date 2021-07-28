using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Models
{
    public class Product
    {
        public Product()
        {
            InvoiceDetailses = new HashSet<InvoiceDetails>();
        }
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public bool Status { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string PhotoPath { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<InvoiceDetails> InvoiceDetailses { get; set; }

    }
}
