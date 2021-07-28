using Bazar_Eshop.Models;
using Bazar_Eshop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Controllers
{
   /* [Authorize(Roles = ("Admin"))]*/
    public class ProductController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment hostEnvironment;

        public ProductController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }
        [HttpGet]
        [Route("Administration/Product/AddProduct")]
        public IActionResult AddProduct()
        {
            var productViewModel = new ProductViewModel();
            productViewModel.Product = new Product();
            productViewModel.Categories = new List<SelectListItem>();
            string UniqueFileName = UploadProcessModel(productViewModel);
            productViewModel.Product.PhotoPath = UniqueFileName;
            var categories = context.Categories.ToList();
            foreach (var category in categories)
            {
                var group = new SelectListGroup { Name = category.Name };
                if (category.InverseParents != null && category.InverseParents.Count > 0)
                {
                    foreach (var subcategory in category.InverseParents)
                    {
                        var selectListItem = new SelectListItem
                        {
                            Text = subcategory.Name,
                            Value = subcategory.Id.ToString(),
                            Group = group
                        };
                        productViewModel.Categories.Add(selectListItem);
                    }
                }

            }

            return View("AddProduct", productViewModel);
        }
        [HttpPost]
        [Route("Administration/Product/AddProduct")]
        public IActionResult AddProduct(ProductViewModel productViewModel)
        {
            string UniqueFileName = UploadProcessModel(productViewModel);
            productViewModel.Product.PhotoPath = UniqueFileName;
            context.Products.Add(productViewModel.Product);
            context.SaveChanges();
            return RedirectToAction("ProductList", "Product");
        }
        private string UploadProcessModel(ProductViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

            }

            return uniqueFileName;
        }
        [Route("Administration/Product/ProductList")]
        public IActionResult ProductList()
        {
            var productList = context.Products.Include(c => c.Category).ToList();
            return View(productList);
        }

        [HttpGet]
        [Route("Administration/Product/DeleteProduct")]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.Products.Find(id);
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("ProductList");

        }
        [HttpGet]
        [Route("Administration/Product/EditProduct")]
        public IActionResult UpdateProduct(int id)
        {
            Product product = new Product();
            var updateProductViewModel = new UpdateProductViewModel();
            updateProductViewModel.Product = context.Products.Find(id);
            updateProductViewModel.Categories = new List<SelectListItem>();
            updateProductViewModel.ExistingPhotoPath = updateProductViewModel.Product.PhotoPath;
            var categories = context.Categories.ToList();
            foreach (var category in categories)
            {
                var group = new SelectListGroup { Name = category.Name };
                if (category.InverseParents != null && category.InverseParents.Count > 0)
                {
                    foreach (var subcategory in category.InverseParents)
                    {
                        var selectListItem = new SelectListItem
                        {
                            Text = subcategory.Name,
                            Value = subcategory.Id.ToString(),
                            Group = group
                        };
                        updateProductViewModel.Categories.Add(selectListItem);
                    }
                }
            }
            return View("UpdateProduct", updateProductViewModel);
        }
        [HttpPost]
        [Route("Administration/Product/EditProduct")]
        public IActionResult updateProduct(int id, UpdateProductViewModel updateProductViewModel)
        {
           /* Product product = context.Products.Find(id);
            product.Name = updateProductViewModel.Product.Name;
            product.Quantity = updateProductViewModel.Product.Quantity;
            product.Description = updateProductViewModel.Product.Description;
            product.Details = updateProductViewModel.Product.Details;
            product.Price = updateProductViewModel.Product.Price;
            product.Status = updateProductViewModel.Product.Status;
            product.Category = updateProductViewModel.Product.Category;
            product.Category = updateProductViewModel.Categories;
*/

            if (updateProductViewModel.Photo != null)
            {
                if (updateProductViewModel.ExistingPhotoPath != null)
                {
                    string filepath = Path.Combine(hostEnvironment.WebRootPath, "images",
                        updateProductViewModel.ExistingPhotoPath);
                    System.IO.File.Delete(filepath);
                }
                updateProductViewModel.Product.PhotoPath = UploadProcessModelForUpdate(updateProductViewModel);
            }
            else
            {
                updateProductViewModel.Product.PhotoPath = updateProductViewModel.ExistingPhotoPath;
            }
            
            context.Entry(updateProductViewModel.Product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("ProductList", "Product");
        }
        private string UploadProcessModelForUpdate(UpdateProductViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

            }

            return uniqueFileName;
        }

        public IActionResult ProductInSubCategory(int id, int parentId)
        {
            ViewBag.ParentName = context.Categories.Find(parentId);
            ViewBag.categoryName = context.Categories.Find(id);
            var result = context.Products.Where(c => c.CategoryId == id);
            return View(result);
        }
        public IActionResult UserProductList()
        {
            var productListResult = context.Products;
            return View (productListResult);
        }
        public IActionResult ProductDetails(int id)
        {
            var result = context.Products.Find(id);
            return View(result);
        }
        public IActionResult Search(string keyword )
        {
            
            var product = context.Products.Where(p => p.Name.Contains(keyword) && p.Status).ToList();
            ViewBag.Keyword = keyword;
            ViewBag.CountProducts = product.Count;
            return View("Search",product);
        }

    }
}
