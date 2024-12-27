using E_Tickets.Models;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CinemaController : Controller
    {
        private readonly IMovieRepository movieRepository;

        private readonly ICinemaRepository cinemaRepository;


        public CinemaController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository)
        {
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;

        }

        public IActionResult Index(string? query = null, int PageNumber = 0)
        {
            //var cinema = _context.Cinemas.ToList();
            var cinema = cinemaRepository.Get();
            if (query != null)
            {
                query = query.Trim();
                cinema = cinema.Where(e => e.Name.Contains(query));
            }
            if (PageNumber > 0)
            {
                cinema = cinema.Skip((PageNumber - 1) * 3).Take(3);
            }

            return View(cinema.ToList());
        }
        public IActionResult AllMoviesCinema(int cinemaId)
        {
            //var movie = _context.Movies.Where(e=>e.CinemaId==cinemaId).Include(e=>e.Category)
            //    .Include(e=>e.Cinema).ToList();
            var movie = movieRepository.Get(filter: c => c.CinemaId == cinemaId,
                includeProps: [c => c.Category, c => c.Cinema]).ToList();

            return View(movie);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cinema cinema)
        {
            ModelState.Remove("Description");
            ModelState.Remove("CinemaLogo");
            ModelState.Remove("Movies");
            if (ModelState.IsValid)
            {
                //_context.Cinemas.Add(cinema);
                //_context.SaveChanges();
                cinemaRepository.Create(cinema);
                TempData["message"] = "Add Cinema successfuly";
                return RedirectToAction("Index");
            }
            return View(cinema);
        }
        public IActionResult Edit(int cinemaId)
        {
            //var cinema = _context.Cinemas.Find(cinemaId);
            var cinema = cinemaRepository.GetOne(c => c.Id == cinemaId);
            if (cinema != null)
            {
                return View(cinema);
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                //_context.Cinemas.Update(cinema);
                //_context.SaveChanges();
                cinemaRepository.Alter(cinema);
                TempData["message"] = "Update Cinema successfuly";
                return RedirectToAction("Index");
            }
            return View(cinema);
        }
        public IActionResult Delete(int id)
        {
            //var cinema = _context.Cinemas.Find(id);
            var cinema = cinemaRepository.GetOne(e => e.Id == id);

            if (cinema != null)
            {
                //_context.Cinemas.Remove(cinema);
                //_context.SaveChanges();
                cinemaRepository.Delete(cinema);
                TempData["message"] = "Delete Cinema successfuly";
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }
    }
}
