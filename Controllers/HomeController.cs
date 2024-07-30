using InventoryManagementSystem.Models;
using InventoryManagementSystem.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace InventoryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly InventoryManagementContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IDataProtector _protector;

        public HomeController(DataSecurityProvider key, IDataProtectionProvider provider, InventoryManagementContext contet, IWebHostEnvironment env)
        {
            _context = contet;
            _env = env;
            _protector = provider.CreateProtector(key.key);
        }
        

        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            var p = products.Select(x=> new ProductEdit
            {
                ProId = x.ProId,
                ProName = x.ProName,
                ProImage = x.ProImage,
                OpeningQuantity = x.OpeningQuantity,
                Description = x.Description,
                ProPrice = x.ProPrice,
                CategoryId = x.CategoryId
            }).ToList();
            ViewData["Product"] = products;
            return View(p);
        }

        public IActionResult ProductList()
        {
            ViewData["cat"] = new SelectList(_context.Categories, nameof(Category.CatId),nameof(Category.CatName));
            return View(); 
        }

        public IActionResult GetProductList(int CatId)
        {
            var products = _context.Products.Where(x=>x.CategoryId==CatId).ToList();
            return PartialView("_ProductList", products);
        }


        public IActionResult Details(string id)
        {

            int proid = Convert.ToInt32(id);

            var u = _context.Products.Where(x => x.ProId == proid).First();
            return View(u);
            //return Json(u);

        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // register users
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create(UserListEdit u)
        {
            //return Json(u);
            try
            {
                var users = _context.UserLists.Where(x => x.EmailAddress == u.EmailAddress).FirstOrDefault();
               // return Json(users);
                if (users == null) 
                {
                    short maxid;
                    if (_context.UserLists.Any())
                        maxid = Convert.ToInt16(_context.UserLists.Max(x => x.UserId) + 1);
                    else
                        maxid = 1;
                    u.UserId = maxid;

                    

                    if (u.UserFile !=null)
                    {
                        string fileName = "UserImage" + Guid.NewGuid() + Path.GetExtension(u.UserFile.FileName);
                        string filePath = Path.Combine(_env.WebRootPath, "UserImage", fileName);
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            u.UserFile.CopyTo(stream);
                        }
                        u.UsePhoto = fileName;
                    }

                    //return Json(u);

                    UserList userList = new()
                    {
                        EmailAddress = u.EmailAddress,
                        FullName = u.FullName,
                        CurrentAddress = u.CurrentAddress,
                        UsePhoto = u.UsePhoto,
                        UserId = u.UserId,
                        UserPassword = _protector.Protect(u.UserPassword),
                        UserRole = "User"
                    };

                    //return Json(userList);
                    _context.Add(userList);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Account");

                }
                else
                {
                    ModelState.AddModelError("", "User already exist with this email. Please use another email!");
                    return View(u);
                }
            }
            catch
            {
                ModelState.AddModelError("", "User Registration failed. Please try again");
                return View(u);
            }
        }

        // partial view 
        public IActionResult ProfileImage()
        {
            var p = _context.UserLists.Where(u=>u.UserId.Equals(Convert.ToInt16(User.Identity!.Name))).FirstOrDefault();
            ViewData["img"] = p.UsePhoto;
            return PartialView("_Profile");
        }

        // edit user profile
        [HttpGet]
        public IActionResult Edit(int id)
        {
            
            var user = _context.UserLists.Where(x => x.UserId == Convert.ToInt16(User.Identity!.Name)).FirstOrDefault();
            UserListEdit userEdit = new()
            {
                UserId = user.UserId,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                CurrentAddress = user.CurrentAddress,
                UsePhoto = user.UsePhoto
            };
            return View(userEdit);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(UserListEdit u)
        {
            if (u.UserFile != null)
            {
                string fileName = "UserImage" + Guid.NewGuid() + Path.GetExtension(u.UserFile.FileName);
                string filePath = Path.Combine(_env.WebRootPath, "UserImage", fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    u.UserFile.CopyTo(stream);
                }
                u.UsePhoto = fileName;
            }

            UserList user = new()
            {
                UserId = u.UserId,
                FullName = u.FullName,
                EmailAddress = u.EmailAddress,
                CurrentAddress = u.CurrentAddress,
                UsePhoto = u.UsePhoto
            };

            _context.Update(user);
            _context.SaveChangesAsync();
            return RedirectToAction("ProfileUpdate");

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
