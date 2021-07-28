using Bazar_Eshop.Helpers;
using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.ViewComponents
{
    [ViewComponent(Name = "Cart")]
    public class CartViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Item> cart = SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                ViewBag.CountItems = 0;
            }
            else
            {
                ViewBag.CountItems = cart.Count;
            }
            return View("index");
        }
    }
}
