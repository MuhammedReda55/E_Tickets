using E_Tickets.Models;
using E_Tickets.Models.ViewModels;
using E_Tickets.Repository.IRepository;
using E_Tickets.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace E_Tickets.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IEmailSender emailSender;
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;
       

        public CheckoutController(IEmailSender emailSender,ICartRepository cartRepository,UserManager<ApplicationUser> userManager)
        {
            this.emailSender = emailSender;
            this.cartRepository = cartRepository;
            this.userManager = userManager;
            
        }

        //public async Task<IActionResult>  Success()
        //{
        //    var user = await userManager.GetUserAsync(User);
        //    if (user != null)
        //    {
        //        await emailSender.SendEmailAsync(await userManager.GetEmailAsync(user), "success pay", $"<html><body> <h1>{cartRepository.Get(includeProps: [e=>e.Movie.Name])}</h1> </body></html>");
        //        return View();

        //    }
        //    TempData["message"] = "هناك خطا في الدفع";
        //    return RedirectToAction("Index", "Home");
        //}
        public async Task<IActionResult> Success()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                
                var cart = cartRepository.Get(
                    includeProps: [e=>e.Movie.Cinema],
                    filter: e => e.ApplicationUserId == user.Id
                );

                if (cart != null && cart.Any())
                {
                    var ticketDetails = new TicketDetailsVM
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        MovieName = cart.First().Movie.Name,
                        CinemaName = cart.First().Movie.Cinema.Name,
                        Price = (double)cart.First().Movie.Price,
                        Count = cart.Sum(e => e.Count),
                        TotalPrice = (double)cart.Sum(e => e.Movie.Price * e.Count),
                        Date = DateTime.Now
                    };

                    // تحويل البيانات إلى JSON
                    var jsonTicketDetails = JsonConvert.SerializeObject(ticketDetails);

                    
                    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/TicketDetailsTemplate.html");
                    string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                    var emailBody = emailTemplate
                        .Replace("{{UserName}}", ticketDetails.UserName)
                        .Replace("{{Email}}", ticketDetails.Email)
                        .Replace("{{MovieName}}", ticketDetails.MovieName)
                        .Replace("{{CinemaName}}", ticketDetails.CinemaName)
                        .Replace("{{Price}}", ticketDetails.Price.ToString("F2"))
                        .Replace("{{Count}}", ticketDetails.Count.ToString())
                        .Replace("{{TotalPrice}}", ticketDetails.TotalPrice.ToString("F2"))
                        .Replace("{{Date}}", ticketDetails.Date.ToString("yyyy-MM-dd HH:mm"));

                    // إرسال البريد الإلكتروني
                    await emailSender.SendEmailAsync(user.Email, "Success", emailBody);
                }

                return View();
            }

            TempData["message"] = "هناك خطأ في الدفع";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Cancel()
        {
            return View();  
        }
        public IActionResult ticket()
        {
            var cart = cartRepository.Get(includeProps: [e => e.Movie.Cinema], filter: e => e.ApplicationUserId == userManager.GetUserId(User));

            //ViewBag.Total = cart.Sum(e => e.Product.Price * e.Count);
            var username = userManager.GetUserName(User);
            CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            {
                Carts = cart.ToList(),
                UserName = username,
                TotalPrice = (double)cart.Sum(e => e.Movie.Price * e.Count)
            };
           

            return View(cartWithTotal);
        }
    }
}
