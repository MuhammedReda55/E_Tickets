using E_Tickets.Models;
using E_Tickets.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace E_Tickets.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser>
            signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Register()
        {
            //if (!roleManager.Roles.Any())
            //{
            //    await roleManager.CreateAsync(new("Admin"));
            //    await roleManager.CreateAsync(new("Company"));
            //    await roleManager.CreateAsync(new("Customer"));
            //}

            //await userManager.AddToRoleAsync(user, "Admin");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUserVM UserVM)
        {
            if (ModelState.IsValid) {
                ApplicationUser user = new()
                {
                    UserName = UserVM.UserName,
                    Email = UserVM.Email,
                    Address = UserVM.Address,
                    Name = UserVM.Name

                };
             var result =  await  userManager.CreateAsync(user, UserVM.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "يرجى ادخال كلمه سر تحتوي على احرف كبيره وصغيره وارقام وحروف مميزه");
                }

            }
            return View();
        }
        public  IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var appUserWithEmail = await userManager.FindByEmailAsync(loginVM.Account);
                var appUserWithUserName = await userManager.FindByNameAsync(loginVM.Account);

                if (appUserWithEmail != null || appUserWithUserName != null)
                {
                    var result = await userManager.CheckPasswordAsync(appUserWithEmail ?? appUserWithUserName, loginVM.Password);

                    if (result)
                    {
                        await signInManager.SignInAsync(appUserWithEmail ?? appUserWithUserName, loginVM.RemeberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "هناك خطأ في الحساب او في كلمه السر");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "هناك خطأ في الحساب او في كلمه السر");
                }
            }

            return View(loginVM);
        }
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
       
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(forgotPasswordVM.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                    return View();
                }
                
                var result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, forgotPasswordVM.NewPassword);
                    TempData["message"] = "تم تغيير كلمة المرور بنجاح.";
                    return RedirectToAction("Login", "Account");

                }
               
                    ModelState.AddModelError(string.Empty, "حدث خطأ أثناء إزالة كلمة المرور القديمة.");
                    return View(forgotPasswordVM);
               
            }
            return View(forgotPasswordVM);
       
        }
        public IActionResult Profile(ApplicationUserVM? UserVM)
        {
            var user = userManager.FindByIdAsync("UserVM.Id");
            
           

                return View(user);
        }


    }
}
