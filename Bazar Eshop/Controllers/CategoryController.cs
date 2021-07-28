
using Bazar_Eshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Controllers
{
   /* [Authorize(Roles = ("Admin"))]*/
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly AppDbContext context;

        public CategoryController(ICategoryRepository categoryRepository,AppDbContext context)
        {
            this.categoryRepository = categoryRepository;
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [Route("Administration/Category/CategoryList")]
        public IActionResult CategoryList()
        {
            var categoryList = categoryRepository.GetAllCategory();
            return View(categoryList);
        }
        [HttpGet]
        [Route("Administration/Category/EditCategory")]
        public IActionResult UpdateCategory(int id)
        {
            var result = categoryRepository.GetCategory(id);
            return View(result);
        }

        [HttpPost]
        [Route("Administration/Category/EditCategory")]
        public IActionResult UpdateCategory(int id ,Category category)
        {

            var resultCategory = categoryRepository.GetCategory(id);
            resultCategory.Name = category.Name;
            resultCategory.Status = category.Status;
            categoryRepository.UpdateCategory(resultCategory);
            return RedirectToAction("CategoryList");
            
            
        }
        [HttpGet]
        [Route("administration/category/addcategory")]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        [Route("administration/category/addcategory")]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                /*var categoryName = new Category
                {
                    Name = category.Name
                };*/
                category.Parent = null;
                categoryRepository.AddCategory(category);
                return RedirectToAction("CategoryList");
            }
            return View();


        }
        [Route("Administration/Category/DeleteCategory")]
        public IActionResult DeleteCategory(int id)
        {

            categoryRepository.DeleteCategory(id);
            return RedirectToAction("CategoryList");
        }

        
           [HttpGet]
        [Route("Administration/Category/AddSubCategory")]
        public IActionResult AddSubCategory(int id)
        {
            var subCategory = new Category()
            {
                ParentId = id
            };
            return View("AddSubCategory", subCategory);
        }
        [HttpPost]
        [Route("Administration/Category/AddSubCategory")]
        public IActionResult AddSubCategory(int CategoryId,Category subCategory)
        {
            categoryRepository.AddSubCategory(subCategory);
            return RedirectToAction("CategoryList");

        }
        public IActionResult SubCategoryList(int id)
        {
            var categories = categoryRepository.GetAllCategory().ToList();
            ViewBag.Id = id;
            ViewBag.SelecteCategory = context.Categories.Find(id);
            ViewBag.ProductList = context.Products;
            ViewBag.SubCategoryList = context.Categories.Where(c=>c.ParentId == id);
            ViewBag.LatestProducts = context.Products.OrderByDescending(p => p.Id).Where(p => p.Status).Take(2).ToList();
            return View(categories);
        }
    }
}
