using E_Tickets.Models;
using E_Tickets.Models.ViewModels;
using E_Tickets.Repository;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Stripe.Checkout;

namespace E_Tickets.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            this.cartRepository = cartRepository;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var cart = cartRepository.Get(includeProps: [e => e.Movie], filter: e => e.ApplicationUserId == userManager.GetUserId(User));

            //ViewBag.Total = cart.Sum(e => e.Product.Price * e.Count);

            CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            {
                Carts = cart.ToList(),
                TotalPrice = (double)cart.Sum(e => e.Movie.Price * e.Count)
            };

            return View(cartWithTotal);
        }
        public IActionResult AddToCart(int count,int MovieId)
        {
            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                var cartInBD = cartRepository.GetOne(filter: e => e.ApplicationUserId == userId && e.MovieId == MovieId);

                if (cartInBD != null)
                {
                    cartInBD.Count += count;
                    cartRepository.Commit();
                }

                else
                {
                    cartRepository.Create(new()
                    {
                        ApplicationUserId = userId,
                        Count = count,
                        MovieId = MovieId,
                        Time = DateTime.Now
                    });

                }

                TempData["message"] = "تم اضافة تذكرة الفليم الي السلة ";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Account");
        }
        public IActionResult Increment(int MovieId)
        {
            var cart = cartRepository.GetOne(filter: e => e.ApplicationUserId == userManager.GetUserId(User) && e.MovieId == MovieId);

            cart.Count++;
            cartRepository.Commit();

            return RedirectToAction("Index");
        }
        public IActionResult Decreament(int MovieId)
        {
            
            var cart = cartRepository.GetOne(filter: e => e.ApplicationUserId == userManager.GetUserId(User) && e.MovieId == MovieId);

            var count = cart.Count--;
            if (count >= 1)
            {
                
                cartRepository.Commit();
            }
            
           
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var cart = cartRepository.GetOne(e => e.MovieId== id);

            if (cart != null)
            {

                cartRepository.Delete(cart);
                TempData["message"] = "Delete Ticket successfuly";
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }


        public IActionResult Pay()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/Cancel",
            };


            var cart = cartRepository.Get(includeProps: [e => e.Movie], filter: e => e.ApplicationUserId == userManager.GetUserId(User)).ToList();

            foreach (var item in cart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                        },
                        UnitAmount = (long)item.Movie.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
