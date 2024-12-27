using E_Tickets.Models;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActorController : Controller
        {
            private readonly IMovieRepository movieRepository;
            private readonly IActorRepository actorRepository;

            public ActorController(IMovieRepository movieRepository, IActorRepository actorRepository)
            {
                this.movieRepository = movieRepository;
                this.actorRepository = actorRepository;

            }

            public IActionResult Index(int actorId)
            {
                //var actor = _context.Actors.Include(e=>e.Movies).Where(e=>e.Id == actorId).FirstOrDefault();
                //var actor = actorRepository.GetOne(filter: e => e.Id == actorId, includeProps: [m => m.Movies]);
                var actor = actorRepository.GetWithIncludes(filter: e => e.Id == actorId,include:
                    e=>e.Include(e=>e.ActorMovies).ThenInclude(e=>e.Movie)).FirstOrDefault();
            

            return View(actor);
            }
            public IActionResult AllActor(int actorId, string? query = null, int PageNumber = 0)
            {
                //var actors = _context.Actors.ToList();
                var actor = actorRepository.Get();
            if (query != null)
            {
                query = query.Trim();
                actor = actor.Where(e => e.FirstName.Contains(query) || e.LastName.Contains(query));
            }
            if (PageNumber > 0)
            {
                actor = actor.Skip((PageNumber - 1) * 3).Take(3);
            }

            return View(actor.ToList());
            }
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create(Actor actor, IFormFile file)
            {
                ModelState.Remove("Movies");
                ModelState.Remove("ProfilePicture");
                ModelState.Remove("News");

                if (ModelState.IsValid)
                {
                    if (file != null && file.Length > 0)
                    {
                        // Genereate name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // Save in wwwroot
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cast", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(stream);
                        }

                        // Save in db
                        actor.ProfilePicture = fileName;
                    }

                    //_context.Actors.Add(actor);
                    //_context.SaveChanges();
                    actorRepository.Create(actor);
                    TempData["message"] = "Add Actor successfuly";
                    return RedirectToAction("AllActor");
                }
                return View(actor);
            }
            public IActionResult Edit(int actorId)
            {
                //var actor = _context.Actors.Find(actorId);
                var actor = actorRepository.GetOne(filter: e => e.Id == actorId);
                if (actor != null)
                {
                    return View(actor);
                }
                return RedirectToAction("NotFoundSearch", "Home");
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(Actor actor, IFormFile? file)
            {
                ModelState.Remove("Movies");
                ModelState.Remove("ProfilePicture");
                ModelState.Remove("News");

                //var oldActor = _context.Actors.Where(e => e.Id == actor.Id).AsNoTracking().FirstOrDefault();
                var oldActor = actorRepository.GetOne(filter: e => e.Id == actor.Id, tracked: false);

                if (ModelState.IsValid)
                {
                    if (file != null && file.Length > 0)
                    {
                        // Generate new file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // Path to save file
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cast", fileName);

                        // Save new file
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(stream);
                        }

                        // Delete old file
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cast", oldActor.ProfilePicture);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);


                        }

                        actor.ProfilePicture = fileName;
                    }
                    else
                    {
                        actor.ProfilePicture = oldActor.ProfilePicture;
                    }

                    //_context.Actors.Update(actor);
                    //_context.SaveChanges();
                    actorRepository.Alter(actor);

                    TempData["message"] = "Update Actor successfully";
                    return RedirectToAction("AllActor");
                }

                actor.ProfilePicture = oldActor.ProfilePicture;
                return View(actor);
            }

            public IActionResult Delete(int id)
            {
                //var actor = _context.Actors.Find(id);
                var actor = actorRepository.GetOne(e => e.Id == id);
                if (actor != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cast", actor.ProfilePicture);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    //_context.Actors.Remove(actor);
                    //_context.SaveChanges();
                    actorRepository.Delete(actor);
                    TempData["message"] = "Delete Actor successfuly";
                    return RedirectToAction("AllActor");
                }
                return RedirectToAction("NotFoundSearch", "Home");
            }

        }
    }

