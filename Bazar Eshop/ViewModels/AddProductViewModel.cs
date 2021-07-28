using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.ViewModels
{
    public class AddProductViewModel : Product
    {
        public IFormFile Photo { get; set; } 
    }
}
