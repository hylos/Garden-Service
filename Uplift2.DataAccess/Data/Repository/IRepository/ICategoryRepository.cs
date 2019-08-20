using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Uplift2.Models;

namespace Uplift2.DataAccess.Data.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        //Select list item for dropdown
        IEnumerable<SelectListItem> GetCategoryListForDropDown();

        //Update method
        void Update(Category category);
    }
}
