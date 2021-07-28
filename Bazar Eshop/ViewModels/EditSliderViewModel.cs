using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.ViewModels
{
    public class EditSliderViewModel
    {
        public Slider Slider { get; set; }
        public string ExistingPhotoPath { get; set; }
        public IFormFile Photo { get; set; }
    }
}
