using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly AppDbContext context;

        public InvoiceController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            ViewBag.invoices = context.Invoices.OrderByDescending(i => i.Id).ToList();
            return View();
        }
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            ViewBag.invoiceDetails = context.Invoices.Include(i => i.ApplicationUser).FirstOrDefault(c=>c.Id==id);
            ViewBag.invoicess = context.Invoices.Find(id);
            ViewBag.invoice = context.InvoiceDetailses.Include(i => i.Product).Where(i => i.InvoiceId == id);
            return View("InvoiceDetails");
        }
        public IActionResult PaymentComplete(int id)
        {
            var result = context.Invoices.Find(id);
            result.Status = 0;
            context.SaveChanges();
            return RedirectToAction("DashBoard","Administration");
        }
    }
}
