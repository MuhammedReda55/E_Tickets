using E_Tickets.Models;
using E_Tickets.Repository;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Tickets.Controllers
{

    public class RequestCinemaController : Controller
    {
        private readonly IRequestCinemaRepository requestCinemaRepository;
        private readonly ICinemaRepository cinemaRepository;

        public RequestCinemaController(IRequestCinemaRepository requestCinemaRepository,ICinemaRepository cinemaRepository)
        {
            this.requestCinemaRepository = requestCinemaRepository;
            this.cinemaRepository = cinemaRepository;
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
                cinemaRequest.status = "pending";
                requestCinemaRepository.Create(cinemaRequest);
                TempData["message"] = "تم اضافة الطلب بنجاح الرجاء انتظار ";
                return RedirectToAction("Index", "Home");

            }
           
            return View();
        }
        public IActionResult AcceptRequest(int cinemaId)
        {
            var cinemaRequest = requestCinemaRepository.GetOne(e => e.Id == cinemaId);
            if (cinemaRequest != null)
            {
                var cinema = new Cinema()
                {
                    Name = cinemaRequest.Name,
                    Description = cinemaRequest.Description,
                    Address = cinemaRequest.Address,
                    
                };
                cinemaRequest.status = "accepted"; 
                cinemaRepository.Create(cinema); 
                requestCinemaRepository.Delete(cinemaRequest);
                

            }

            return RedirectToAction("Index");
        }        
        public IActionResult RejectRequest(int cinemaId)

        {
            var cinema = requestCinemaRepository.GetOne(e => e.Id == cinemaId);
            if (cinema != null)
            {
                cinema.status = "rejected"; 
                requestCinemaRepository.Alter(cinema); 
            }

            return RedirectToAction("Index");
        }

    }
}
