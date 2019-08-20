using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uplift2.DataAccess.Data.Repository.IRepository;
using Uplift2.Models;
using Uplift2.Utility;

namespace Uplift2.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //we not passing any data to controller because we will use data table in the view
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                return View(category);
            }

            category = _unitOfWork.Category.Get(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                //Add new category else updating category
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //call API for data retrieval
        #region API CALLS

        //Get all categories
        [HttpGet]
        public IActionResult GetAll()
        {
            //pass json object to retrieve all data.
            //return Json(new { data = _unitOfWork.Category.GetAll() });
            return Json(new { data = _unitOfWork.SP_Call.ReturnList<Category>(SD.usp_GetAllCategory, null)});
        }

        //delete category
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //retrieve object from database.
            var objFromDb = _unitOfWork.Category.Get(id);

            //Check if the object exist in Db.
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            //Remove object
            _unitOfWork.Category.Remove(objFromDb);

            //Save changes to db.
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successful." });
        }



        #endregion
    }
}