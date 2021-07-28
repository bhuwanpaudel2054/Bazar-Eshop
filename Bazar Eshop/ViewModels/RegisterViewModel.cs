using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.ViewModels
{
    public class RegisterViewModel
    {   
        
        
        [Required]
        [RegularExpression(@"^[a-zA-Z]+", ErrorMessage = "Enter only characters!! ")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+", ErrorMessage = "Enter only characters!!")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public Gend Gender { get; set; }
        [Required]
        [Display(Name ="Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]+", ErrorMessage = "Wrong number enter digit only with 10 digit!!")]
        public string PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password did not match")]
        public string ConfirmPassword { get; set; }
       
        public IFormFile Photo { get; set; }

    }
}
