using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Uplift2.DataAccess.Data.Repository.IRepository;
using Uplift2.Models.ViewModels;

namespace Uplift2.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public ServiceVM ServVM { get; set; }

        public ServiceController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Get/Upsert
        public IActionResult Upsert(int? id)
        {
             ServVM = new ServiceVM()
            {
                Service = new Models.Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropDown(),
            };

            if(id != null)
            {
                ServVM.Service = _unitOfWork.Service.Get(id.GetValueOrDefault()); 
            }

            return View(ServVM);
        }

        //Post/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (ServVM.Service.Id == 0)
                {
                    //New service, *we are not checking if the file has been uploaded because its already checked in the frontend with javascript.*
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\services");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    ServVM.Service.ImageUrl = @"\images\services\" + fileName + extension;

                    _unitOfWork.Service.Add(ServVM.Service);
                }
                else
                {
                    //Edit service
                    var serviceObjFromDb = _unitOfWork.Service.Get(ServVM.Service.Id);
                    if (files.Count > 0)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(webRootPath, @"images\services");
                        var extension_new = Path.GetExtension(files[0].FileName);

                        var imagePath = Path.Combine(webRootPath, serviceObjFromDb.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension_new), FileMode.Create))
                        {
                            files[0].CopyTo(fileStreams);
                        }

                        ServVM.Service.ImageUrl = @"\images\services\" + fileName + extension_new;

                    }
                    else
                    {
                        //if the image does not exists,keep the same image
                        ServVM.Service.ImageUrl = serviceObjFromDb.ImageUrl;
                    }

                    _unitOfWork.Service.Update(ServVM.Service);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ServVM.CategoryList = _unitOfWork.Category.GetCategoryListForDropDown();
                ServVM.FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropDown();
                return View(ServVM);
            }
        }

        //API
        #region API CALLS
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Service.GetAll(includeProperties: "Category,Frequency") });
        }

        //API/Delete
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var serviceFromDb = _unitOfWork.Service.Get(id);
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if(serviceFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            _unitOfWork.Service.Remove(serviceFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully." });

        }

        #endregion

    }
}