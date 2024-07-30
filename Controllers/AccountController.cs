using InventoryManagementSystem.Models;
using InventoryManagementSystem.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;

namespace InventoryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly InventoryManagementContext _context;
        private readonly IDataProtector _protector;

        public AccountController(InventoryManagementContext context, DataSecurityProvider p, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector(p.key);
        }

        // Login 
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserListEdit uEdit)
        {
            var users = _context.UserLists.ToList();
            if (users != null)
            {

                var u = users.Where(x => x.EmailAddress.ToUpper().Equals(uEdit.EmailAddress.ToUpper()) && _protector.Unprotect(x.UserPassword).Equals(uEdit.UserPassword)).FirstOrDefault();
                if (u != null)
                {
                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.Name,u.UserId.ToString()),
                        new Claim(ClaimTypes.Role,u.UserRole),
                        new Claim("FullName",u.FullName),
                        new Claim("image",u.UsePhoto),
                        new Claim("email",u.EmailAddress),
                        new Claim("address",u.CurrentAddress),
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));

                    return RedirectToAction("Dashboard");

                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid User");

            }
            return View(uEdit);
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index","Admin");
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        [Authorize]
        // logout
        public async Task <IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Static");
        }


        // change password after login
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword c)
        {
            /*return Json(c);*/
           
                var u = _context.UserLists.Where(e => e.UserId == Convert.ToInt16(User.Identity!.Name)).First();
                if (_protector.Unprotect(u.UserPassword) != c.CurrentPassword)
                {
                    ModelState.AddModelError("", "Check your current password");
                }
                else
                {
                    if (c.NewPassword == c.ConfirmPassword)
                    {
                        u.UserPassword = _protector.Protect(c.NewPassword);
                        _context.Update(u);
                        _context.SaveChanges();
                    return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Confirm Password doesnot match");
                        return View(c);
                    }

                }
            ModelState.AddModelError("", "Password Changed Failed. Please Try Again!");
            return View();
        }

        // reset password before login
        [HttpGet]

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(UserListEdit edit)
        {

            if (edit.EmailAddress != null)
            {
                Random r = new Random();
                HttpContext.Session.SetString("token", r.Next(9999).ToString());
                var token = HttpContext.Session.GetString("token");
                var user = _context.UserLists.Where(u => u.EmailAddress == edit.EmailAddress).FirstOrDefault();
                if (user != null)
                {
                    SmtpClient s = new()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        Credentials = new NetworkCredential("ghastlybarely2356@gmail.com", "pfkz vkkg jrpg xlpn"),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };

                    MailMessage m = new()
                    {
                        From = new MailAddress("ghastlybarely2356@gmail.com"),
                        Subject = "Forgot Password Token",
                        Body = $@"<p class='text-red-800' style='background-color:red;'>Forgot Password</p>
                        <a href='https://localhost:44321/Account/ResetPassword?UserId=0&EmailAddress={user.EmailAddress}&EmailToken={_protector.Protect(token)}' style='background-color:blue;' >ResetPassword</a>:{token}",
                        IsBodyHtml = true,

                    };


                    m.To.Add(user.EmailAddress);
                    s.Send(m);
                    // return Json("success");
                    return RedirectToAction("VerifyToken", new { email = user.EmailAddress });

                }
                else
                {
                    ModelState.AddModelError("", "This Email is not registered Email.");
                    return View(edit);
                }
            }
            return Json("Failed");
        }

        // token verification 
        [HttpGet]
        public IActionResult VerifyToken(string email)
        {
            return View(new UserListEdit { EmailAddress = email });
        }

        [HttpPost]
        public IActionResult VerifyToken(UserListEdit e)
        {
            var token = HttpContext.Session.GetString("token");

            if (token == e.EmailToken)
            {
                var et = _protector.Protect(e.EmailToken!);
                return RedirectToAction("ResetPassword", new UserListEdit { EmailAddress = e.EmailAddress, EmailToken = et });
            }
            else
            {
                return Json("Failed");
            }
        }

        // reset password
        [HttpGet]
        public IActionResult ResetPassword(UserListEdit e)
        {
            try
            {
                // return Json(e);
                var token = HttpContext.Session.GetString("token");
                var eToken = _protector.Unprotect(e.EmailToken);
                if (token == eToken)
                {
                    return View(new ChangePassword { EmailAddress = e.EmailAddress });
                }
                else
                {
                    return RedirectToAction("ForgotPassword");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ForgotPassword");
            }
        }


        [HttpPost]
        public IActionResult ResetPassword(ChangePassword model)
        {


            if (model.NewPassword == model.ConfirmPassword)
            {
                var user = _context.UserLists.FirstOrDefault(u => u.EmailAddress == model.EmailAddress);
                if (user != null)
                {
                    user.UserPassword = _protector.Protect(model.NewPassword);
                    _context.Update(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            else
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(model);
            }


            // return RedirectToAction("ForgotPassword");
            return Json("error");
        }

    }
}
