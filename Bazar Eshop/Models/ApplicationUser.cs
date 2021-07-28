using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Invoices = new HashSet<Invoice>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string City { get; set; }
        public string PhotoPath { get; set; }
        public Gend Gender { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }

    }
}
