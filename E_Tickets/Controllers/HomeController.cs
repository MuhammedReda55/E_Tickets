using System.Diagnostics;
using System.Linq.Expressions;
using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //ApplicationDbContext _context = new ApplicationDbContext();

        private readonly IMovieRepository movieRepository;
        private readonly IActorRepository actorRepository;
        private readonly ICinemaRepository cinemaRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IActorMovieRepository actorMovieRepository;

        public HomeController(IMovieRepository movieRepository, IActorRepository actorRepository, ICinemaRepository cinemaRepository
            , ICategoryRepository categoryRepository, IActorMovieRepository actorMovieRepository)
        {
            this.movieRepository = movieRepository;
            this.actorRepository = actorRepository;
            this.cinemaRepository = cinemaRepository;
            this.categoryRepository = categoryRepository;
            this.actorMovieRepository = actorMovieRepository;
        }





        [AllowAnonymous]
        public IActionResult Index( int PageNumber = 0)
        {


            var movies = movieRepository.Get(includeProps: [c => c.Category, c => c.Cinema]);
            //var movies = movieRepository.Get(includeProps:e=>e.Include(e=>e.Category).Include(c=>c.Cinema) ).ToList();
            if (PageNumber > 0)
            {
                movies = movies.Skip((PageNumber - 1) * 3).Take(5);
            }
            return View(movies.ToList());
        }
        [AllowAnonymous]
        public IActionResult Details(int movieId)
        {

            var movies = movieRepository.GetWithIncludes(
          filter: e => e.Id == movieId,
          include: query => query
              .Include(e => e.Category)
              .Include(e => e.Cinema)
              .Include(e => e.ActorMovies)
                  .ThenInclude(am => am.Actor)
      ).FirstOrDefault();

            
                movies.Views += 1;
                movieRepository.Alter(movies);
            

            return View(movies);
        }
        [AllowAnonymous]
        public IActionResult Search(string name)
        {
            // using Contains
            //    var movies = _context.Movies.Where(e => e.Name.StartsWith(name)).Include(e => e.Category).Include(e => e.Cinema).ToList();
            // using StartsWith   الاتنين صح


            var movies = movieRepository.Get(filter: n => n.Name.Contains(name), includeProps: [c => c.Category, c => c.Cinema]).ToList();
            if (movies != null && movies.Count > 0)
            {
                return View(movies);
            }
            return RedirectToAction("NotFoundSearch");
        }

        [AllowAnonymous]
        public IActionResult AllFilms()
        {

            //var movies = movieRepository.Get(includeProps: [c => c.Category, c => c.Cinema]).ToList();
            var movies = movieRepository.GetWithIncludes(include: e => e.Include(e => e.Category).Include(e => e.Cinema)
            .Include(e => e.ActorMovies).ThenInclude(e => e.Actor)).ToList();
            //var actors = movieRepository.GetWithIncludes(include: e => e.Include(e => e.ActorMovies).ThenInclude(e => e.Actor));
            //ViewData["actors"] = actors;
            return View(movies);
        }


        public IActionResult Create()
        {

            var cinema = cinemaRepository.Get().ToList();
            ViewData["cinema"] = cinema;

            var category = categoryRepository.Get().ToList();
            ViewData["category"] = category;

            var actor = actorRepository.Get().ToList();
            ViewData["actor"] = actor;

            return View();



        }
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile file, List<int> ActorIds)
        {
            ModelState.Remove("ImgUrl");
            ModelState.Remove("MovieStatus");
            ModelState.Remove("Category");
            ModelState.Remove("Cinema");
            ModelState.Remove("ActorMovies");

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Generate unique file name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save file in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save file name in database
                    movie.ImgUrl = fileName;
                }

                // Add selected actors to the movie
                if (ActorIds != null && ActorIds.Count > 0)
                {
                    movie.ActorMovies = ActorIds.Select(actorId => new ActorMovie
                    {
                        ActorId = actorId,
                        MovieId = movie.Id
                    }).ToList();
                }

                movieRepository.Create(movie);

                TempData["message"] = "Movie added successfully!";
                return RedirectToAction("AllFilms");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int movieId)
        {


            var cinema = cinemaRepository.Get().ToList();
            ViewData["cinema"] = cinema;

            var category = categoryRepository.Get().ToList();
            ViewData["category"] = category;

            var actor = actorRepository.Get().ToList();
            ViewData["actor"] = actor;



            var movie = movieRepository.GetOne(e => e.Id == movieId);
            if (movie != null)
            {
                return View(movie);
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile? file, List<int> ActorIds)
        {
            ModelState.Remove("ImgUrl");
            ModelState.Remove("MovieStatus");
            ModelState.Remove("Category");
            ModelState.Remove("Cinema");
            ModelState.Remove("ActorMovies");


            var oldmovie = movieRepository.GetOne(filter: e => e.Id == movie.Id, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Generate new file name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Path to save file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", fileName);

                    // Save new file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Delete old file
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", oldmovie.ImgUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);


                    }

                    movie.ImgUrl = fileName;
                }
                else
                {
                    movie.ImgUrl = oldmovie.ImgUrl;
                }
                if (ActorIds != null && ActorIds.Count > 0)
                {
                    movie.ActorMovies = ActorIds.Select(actorId => new ActorMovie
                    {
                        ActorId = actorId,
                        MovieId = movie.Id
                    }).ToList();
                }


                movieRepository.Alter(movie);

                TempData["message"] = "Update Movie successfully";
                return RedirectToAction("AllFilms");
            }

            movie.ImgUrl = oldmovie.ImgUrl;
            return View(movie);
        }

        public IActionResult Delete(int movieId)
        {

            var movie = movieRepository.GetOne(e => e.Id == movieId);
            if (movie != null)
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\movies", movie.ImgUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    
                    System.IO.File.Delete(oldPath);
                }


                movieRepository.Delete(movie);
                TempData["message"] = "Delete Movie successfuly";
                return RedirectToAction("AllFilms");
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }


        public IActionResult NotFoundSearch()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
