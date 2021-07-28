using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Models
{
    public class SQlCategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext context;

        public SQlCategoryRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Category AddCategory(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return category;
        }

        public Category AddSubCategory(Category subCategory)
        {
            context.Categories.Add(subCategory);
            context.SaveChanges();
            return subCategory;
        }

       

        public IEnumerable<Category> GetAllCategory()
        {
            return context.Categories.ToList();

        }

        public Category GetCategory(int id)
        {
            return context.Categories.Find(id);
        }

        public Category UpdateCategory(Category categoryChanges)
        {
            var category = context.Categories.Attach(categoryChanges);
            category.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return categoryChanges;
        }
        public Category DeleteCategory(int id)
        {
            var category = context.Categories.Find(id);
            context.Categories.Remove(category);
            context.SaveChanges();
            return category;

        }

       
    }
}
