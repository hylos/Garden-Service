using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uplift2.DataAccess.Data.Repository;
using Uplift2.DataAccess.Data.Repository.IRepository;
using Uplift2.Extensions;
using Uplift2.Models;
using Uplift2.Models.ViewModels;
using Uplift2.Utility;

namespace Uplift2.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public CartViewModel CartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            CartVM = new CartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
                ServiceList= new List<Service>()
            };
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetdObject<List<int>>(SD.SessionCart) != null)
            {
                List<int> sessionList = new List<int>();
                sessionList = HttpContext.Session.GetdObject<List<int>>(SD.SessionCart);
                foreach (int serviceId in sessionList)
                {
                    CartVM.ServiceList.Add(_unitOfWork.Service.GetFirstOrDefault(u => u.Id == serviceId, includeProperties: "Frequency,Category"));
                }
            }
            return View(CartVM);
        }


        public IActionResult Summary()
        {
            if (HttpContext.Session.GetdObject<List<int>>(SD.SessionCart) != null)
            {
                List<int> sessionList = new List<int>();
                sessionList = HttpContext.Session.GetdObject<List<int>>(SD.SessionCart);
                foreach (int serviceId in sessionList)
                {
                    CartVM.ServiceList.Add(_unitOfWork.Service.GetFirstOrDefault(u => u.Id == serviceId, includeProperties: "Frequency,Category"));
                }
            }
            return View(CartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            if (HttpContext.Session.GetdObject<List<int>>(SD.SessionCart) != null)
            {
                List<int> sessionList = new List<int>();
                sessionList = HttpContext.Session.GetdObject<List<int>>(SD.SessionCart);
                //Error that had to be fixed
                CartVM.ServiceList = new List<Service>();

                foreach (int serviceId in sessionList)
                {
                    CartVM.ServiceList.Add(_unitOfWork.Service.Get(serviceId));
                }
            }

            if (!ModelState.IsValid)
            {
                return View(CartVM);
            }
            else
            {
                CartVM.OrderHeader.OrderDate = DateTime.Now;
                CartVM.OrderHeader.Status = SD.StatusSubmitted;
                CartVM.OrderHeader.ServiceCount = CartVM.ServiceList.Count;
                _unitOfWork.OrderHeader.Add(CartVM.OrderHeader);
                _unitOfWork.Save();

                foreach (var item in CartVM.ServiceList)
                {
                    OrderDetails orderDetails = new OrderDetails
                    {
                        ServiceId = item.Id,
                        OrderHeaderId = CartVM.OrderHeader.Id,
                        ServiceName = item.Name,
                        Price = item.Price
                    };

                    _unitOfWork.OrderDetails.Add(orderDetails);
                }

                _unitOfWork.Save();

                //Empty session after adding to db
                HttpContext.Session.SetObject(SD.SessionCart, new List<int>());

                return RedirectToAction("OrderConfirmation", "Cart",new { id = CartVM.OrderHeader.Id });
            }

        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

        public IActionResult Remove(int serviceId)
        {
            List<int> sessionList = new List<int>();
            sessionList = HttpContext.Session.GetdObject<List<int>>(SD.SessionCart);
            sessionList.Remove(serviceId);
            HttpContext.Session.SetObject(SD.SessionCart, sessionList);

            return RedirectToAction(nameof(Index));
        }
    }
}