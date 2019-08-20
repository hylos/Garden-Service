﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uplift2.DataAccess.Data.Repository.IRepository;
using Uplift2.Models;

namespace Uplift2.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class FrequencyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FrequencyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        //Upsert Get
        public IActionResult Upsert(int? id)
        {
            Frequency frequency = new Frequency();

            if (id == null)
            {
                return View(frequency);
            }

            frequency = _unitOfWork.Frequency.Get(id.GetValueOrDefault());

            if (frequency == null)
            {
                return NotFound();
            }

            return View(frequency);

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Frequency frequency)
        {
            if (ModelState.IsValid)
            {
                if (frequency.Id == 0)
                {
                    _unitOfWork.Frequency.Add(frequency);
                }
                else
                {
                    _unitOfWork.Frequency.Update(frequency);
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(frequency);
        }

        //API calls
        #region API CALLS

        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Frequency.GetAll() });
        }

        //delete category
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //retrieve object from database.
            var objFromDb = _unitOfWork.Frequency.Get(id);

            //Check if the object exist in Db.
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            //Remove object
            _unitOfWork.Frequency.Remove(objFromDb);

            //Save changes to db.
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successful." });
        }

        #endregion
    }
}