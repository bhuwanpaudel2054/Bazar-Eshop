using Bazar_Eshop.Models;
using Bazar_Eshop.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Controllers
{
    public class SliderController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment hostEnvironment;

        public SliderController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            ViewBag.SliderList = context.Slider.ToList();
            return View();
        }
        [HttpGet]
        public IActionResult AddSlider()
        {
            return View("AddSlider");
        }
        [HttpPost]
        public IActionResult AddSlider(SliderViewModel model)
        {
            
            string photoName = UploadProcessModel(model);
            model.PhotoPath = photoName;
            context.Slider.Add(model);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        private string UploadProcessModel(SliderViewModel model)
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
        public IActionResult RemoveSlider(int id)
        {
            var result = context.Slider.Find(id);
            context.Remove(result);
            context.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpGet]
        public IActionResult EditSlider(int id)
        {
            var editSliderViewModel = new EditSliderViewModel();
            editSliderViewModel.Slider = context.Slider.Find(id);
            editSliderViewModel.ExistingPhotoPath = editSliderViewModel.Slider.PhotoPath;
            return View("EditSlider", editSliderViewModel);
        }
        [HttpPost]
        public IActionResult EditSlider(EditSliderViewModel model)
        {

            if (model.Photo != null)
            {
                if (model.ExistingPhotoPath != null)
                {
                    string filepath = Path.Combine(hostEnvironment.WebRootPath, "images",
                        model.ExistingPhotoPath);
                    System.IO.File.Delete(filepath);
                }
                model.Slider.PhotoPath = UploadProcessModel(model);
            }
            else
            {
                model.Slider.PhotoPath = model.ExistingPhotoPath;
            }
            context.Entry(model.Slider).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index", "Slider");

        }
        private string UploadProcessModel(EditSliderViewModel model)
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

    }
}
