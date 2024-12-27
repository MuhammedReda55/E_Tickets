using E_Tickets.Models;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Tickets.Controllers
{

    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
        {
            private readonly IMovieRepository movieRepository;
            private readonly ICategoryRepository categoryRepository;

            public CategoryController(IMovieRepository movieRepository, ICategoryRepository categoryRepository)
            {
                this.movieRepository = movieRepository;
                this.categoryRepository = categoryRepository;
            }
        [AllowAnonymous]
            public IActionResult Index(string? query = null, int PageNumber = 0)
            {
                //var category = _context.Categories.ToList();
                var category = categoryRepository.Get();
            if(query != null)
            {
                query = query.Trim();
                 category = category.Where(e=>e.Name.Contains(query));
            }
            if (PageNumber > 0)
            {
                category = category.Skip((PageNumber - 1) * 3).Take(3);
            }
           
                return View(category.ToList());
            }
            public IActionResult AllMoviesCategory(int categoryId)
            {
                //var movies = _context.Movies.Where(m => m.CategoryId == categoryId)
                //    .Include(e=>e.Category).Include(m => m.Cinema).ToList();
                var movies = movieRepository.Get(filter: c => c.CategoryId == categoryId,
                    includeProps: [c => c.Category, c => c.Cinema]).ToList();

                return View(movies);
            }
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create(Category category)
            {
                ModelState.Remove("Movies");
                if (ModelState.IsValid)
                {
                    //_context.Categories.Add(category);
                    //_context.SaveChanges();
                    categoryRepository.Create(category);
                    TempData["message"] = "Add Category successfuly";
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            public IActionResult Edit(int categoryId)
            {
                //var category = _context.Categories.Find(categoryId);
                var category = categoryRepository.GetOne(filter: c => c.Id == categoryId);
                if (category != null)
                {
                    return View(category);
                }
                return RedirectToAction("NotFoundSearch", "Home");
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(Category category)
            {
                if (ModelState.IsValid)
                {
                    //_context.Categories.Update(category);
                    //_context.SaveChanges();
                    categoryRepository.Alter(category);
                    TempData["message"] = "Update Category successfuly";
                    return RedirectToAction("Index");
                }
                return View(category);
            }

            public IActionResult Delete(int id)
            {
                //var category = _context.Categories.Find(id);
                var category = categoryRepository.GetOne(filter: e => e.Id == id);
                if (category != null)
                {
                    //_context.Categories.Remove(category);
                    //_context.SaveChanges();
                    categoryRepository.Delete(category);
                    TempData["message"] = "Delete Category successfuly";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("NotFoundSearch", "Home");
            }



        }
    }



