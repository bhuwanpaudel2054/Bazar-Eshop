using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazar_Eshop.Models
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategory();
       
        Category AddCategory(Category category);
        Category GetCategory(int id);
        Category UpdateCategory(Category category);
        Category DeleteCategory(int id);
        Category AddSubCategory(Category category);
    }
}
