﻿using E_Tickets.Models;
using E_Tickets.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
                    Name = UserVM.Name,
                    photo = "~/default-photo.png"

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
                var user = await userManager.FindByEmailAsync(loginVM.Account)?? await userManager.FindByNameAsync(loginVM.Account);
                

                if (user != null )
                {
                     if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                         {
                               ModelState.AddModelError(string.Empty, "Your account is locked.");
                               return View(loginVM);
                         }
                    var result = await userManager.CheckPasswordAsync(user, loginVM.Password);

                    if (result)
                    {
                        await signInManager.SignInAsync(user, loginVM.RemeberMe);
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

        public IActionResult CheckEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckEmail(ForgotPasswordVM forgotPasswordVM)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordVM.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني غير موجود.");
                return View(forgotPasswordVM);
            }
            TempData["Email"] = forgotPasswordVM.Email;
            return RedirectToAction("ForgetPassword", "Account");
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

                var email = TempData["Email"] as string;
                if (email == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                    return RedirectToAction("CheckEmail");
                }

                var user = await userManager.FindByEmailAsync(email);

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

        public IActionResult ChangePassword()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChanagePasswordVM chanagePasswordVM)
        {
            
            if (ModelState.IsValid)
            {

                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                    //return RedirectToAction("CheckEmail");
                }

                //var user = await userManager.FindByEmailAsync(email);

                //if (user == null)
                //{
                //    ModelState.AddModelError(string.Empty, "البريد الإلكتروني غير موجود.");
                //    return View();
                //}
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, chanagePasswordVM.OldPassword);
                if (!checkPasswordResult)
                {
                    ModelState.AddModelError("OldPassword", "كلمة السر القديمة غير صحيحة");
                    return View(chanagePasswordVM);
                }
                var result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, chanagePasswordVM.NewPassword);
                    TempData["message"] = "تم تغيير كلمة المرور بنجاح.";
                    return RedirectToAction("Profile", "Account");

                }

                ModelState.AddModelError(string.Empty, "حدث خطأ أثناء إزالة كلمة المرور القديمة.");
                return View(chanagePasswordVM);

            }
            return View(chanagePasswordVM);

        }



        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            return View(model: new ApplicationUserVM()
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Address = user.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ApplicationUserVM userVM)
        {
            var user = await userManager.GetUserAsync(User);
            user.UserName = userVM.UserName;
            user.Email = userVM.Email;
            user.Name = userVM.Name;
            user.Address = userVM.Address;

            await userManager.UpdateAsync(user);
            TempData["message"] = "تم تحديث البيانات بنجاح";

            return RedirectToAction("Profile", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> updatePhoto(IFormFile photo)
        {
            var user = await userManager.GetUserAsync(User);

            if (photo != null && photo.Length > 0)
            {
                // Genereate name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);

                // Save in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\movies", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    photo.CopyTo(stream);
                }

                // Save in db
                user.photo = fileName;
                await userManager.UpdateAsync(user);
            }

            TempData["message"] = "تم تحديث البيانات بنجاح";

            return RedirectToAction("Profile", "Account");
        }

        public async Task<IActionResult> Users()
        {
            var users = userManager.Users.ToList();

            
            var userRoles = new List<UserWithRolesVM>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRoles.Add(new UserWithRolesVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    Roles = roles.ToList(),
                    IsBlocked = user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow
                });
            }

            return View(userRoles);
        }
        [HttpPost]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                {
                    
                    user.LockoutEnd = null;
                }
                else
                {
                    user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                }

                await userManager.UpdateAsync(user);

                TempData["message"] = $"User {user.UserName} status updated.";
                
            }
           

            return RedirectToAction("Users");
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            
            var user = await userManager.FindByIdAsync(id);

            
            if (user != null)
            {
                
                var result = await userManager.DeleteAsync(user);

                
                if (result.Succeeded)
                {
                    TempData["message"] = $"User {user.UserName} is Deleted Sueccessfully";
                    return RedirectToAction("Users");
                }
                else
                {
                    return BadRequest("Error deleting user.");
                }
            }

            
            return NotFound("User not found.");
        }




    }
}
