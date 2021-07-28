using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Models
{
    public class Invoice
    {
        public Invoice()
        {
            InvoiceDetailses = new HashSet<InvoiceDetails>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public int Status { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<InvoiceDetails> InvoiceDetailses { get; set; }
    }
}
