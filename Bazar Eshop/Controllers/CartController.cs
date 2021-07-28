using Bazar_Eshop.Helpers;
using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bazar_Eshop.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            List<Item> cart = SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.Cart = cart;
            if (cart == null)
            {
                ViewBag.CountItems = 0;
            }
            else
            {
                ViewBag.CountItems = cart.Count;
                ViewBag.Total = cart.Sum(it => it.Price * it.Quantity);
            }
           
            return View();
        }
        [HttpGet]
        [Route("buy/{id}")]
        public IActionResult Buy(int id)
        {
            var product = context.Products.Find(id);
            if (SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
               var cart = new List<Item>();
                cart.Add(new Item
                {
                   Id = product.Id,
                   Name = product.Name,
                   Price=product.Price,
                   PhotoPath = product.PhotoPath,
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJason(HttpContext.Session, "cart", cart);

            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart");
                int index = exists(id, cart);
                if (index == -1)
                {
                    cart.Add(new Item
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        PhotoPath = product.PhotoPath,
                        Quantity = 1
                    });
                }
                else
                {
                    cart[index].Quantity++;
                  
                }
                SessionHelper.SetObjectAsJason(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        [Route("buy/{id}")]
        public IActionResult Buy(int id ,int quantity)
        {
            var product = context.Products.Find(id);
            if (SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                var cart = new List<Item>();
                cart.Add(new Item
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    PhotoPath = product.PhotoPath,
                    Quantity = quantity
                });
                SessionHelper.SetObjectAsJason(HttpContext.Session, "cart", cart);

            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart");
                int index = exists(id, cart);
                if (index == -1)
                {
                    cart.Add(new Item
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        PhotoPath = product.PhotoPath,
                        Quantity = quantity
                    });
                }
                else
                {
                    cart[index].Quantity += quantity;

                }
                SessionHelper.SetObjectAsJason(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index", "Cart");


        }
            private int exists(int id, List<Item> cart)
        {
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].Id == id)
                {
                    return i;
                }

            }
            return -1;
        }
        public IActionResult RemoveCart(int id)
        {
            List<Item> cart = SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart");
            int index = exists(id, cart);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJason(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index", "Cart");

        }
        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            /*var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);*/

            ApplicationUser applicationUser = await userManager.GetUserAsync(User);
            string userEmail = applicationUser.Email;
            // created new  invoices
            var invoice = new Invoice()
            {
                Name = applicationUser.FirstName +' ' + applicationUser.LastName,
                Created = DateTime.Now,
                Status = 1,
                ApplicationUserId = applicationUser.Id
            };
            context.Invoices.Add(invoice);
            context.SaveChanges();

            //Created invoices Details
            List<Item> cart = SessionHelper.GetObjectfromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    var invoiceDetails = new InvoiceDetails
                    {
                        InvoiceId = invoice.Id,
                        ProductId = item.Id,
                        Price = item.Price,
                        Quantity = item.Quantity

                    };
                    context.InvoiceDetailses.Add(invoiceDetails);
                    context.SaveChanges();
                }
                //Remove cart 
                HttpContext.Session.Remove("cart");


                TempData["Success"] = "Thanks for buying product check your mail for delivery progress!!";
            }
            else
            {
                TempData["Success"] = "You Don't have any items on cart.Add items on cart before CheckOut";
            }
            return RedirectToAction("Index", "Cart");
        }
        [Authorize]
        public async Task<IActionResult> History()
        {
            ApplicationUser applicationUser = await userManager.GetUserAsync(User);
            ViewBag.invoices = context.Invoices.OrderByDescending(i=>i.Id).Where(c=>c.ApplicationUserId == applicationUser.Id);
            return View("History");
        }
        [Authorize]
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            ViewBag.invoiceDetails = context.InvoiceDetailses.Include(i => i.Product).Where(i => i.InvoiceId == id); 
            
            return View("InvoiceDetails");
        }
    }
}
