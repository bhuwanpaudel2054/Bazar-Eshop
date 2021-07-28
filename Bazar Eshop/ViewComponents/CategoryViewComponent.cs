using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.ViewComponents
{
    public class CategoryViewComponent :ViewComponent
    {
        private readonly AppDbContext context;

        public CategoryViewComponent(AppDbContext context )
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Category> categories = context.Categories.Where(c => c.Status).ToList();
            return View("Index" ,categories);
        }
    }
}
