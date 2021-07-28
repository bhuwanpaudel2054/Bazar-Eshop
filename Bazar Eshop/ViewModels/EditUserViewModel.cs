using Bazar_Eshop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.ViewModels
{
    public class EditUserViewModel: RegisterViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<string>();
            Claims = new List<string>();
        }
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string ExistingPhotoPath { get; set; }
        public IList<string> Roles { get; set; }
        public List<string> Claims { get; set; }
       
        
        
    }
}
