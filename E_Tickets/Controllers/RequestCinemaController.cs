using E_Tickets.Models;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Tickets.Controllers
{

    public class RequestCinemaController : Controller
    {
        private readonly IRequestCinemaRepository requestCinemaRepository;

        public RequestCinemaController(IRequestCinemaRepository requestCinemaRepository)
        {
            this.requestCinemaRepository = requestCinemaRepository;
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
           var requestcinema =  requestCinemaRepository.Get().ToList();
            return View(requestcinema);
        }

        public IActionResult CreateNewRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateNewRequest(CinemaRequest cinemaRequest)
        {
            
            ModelState.Remove("CinemaLogo");
            ModelState.Remove("Movies");
            
           
            if (ModelState.IsValid)
            {
                requestCinemaRepository.Create(cinemaRequest);
                TempData["message"] = "تم اضافة الطلب بنجاح الرجاء انتظار ";
                return RedirectToAction("Index", "Home");

            }
           
            return View();
        }

    }
}
