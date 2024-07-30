using InventoryManagementSystem.Models;
using InventoryManagementSystem.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InventoryManagementSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly InventoryManagementContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IDataProtector _protector;

        public AdminController(InventoryManagementContext context, DataSecurityProvider p, IDataProtectionProvider provider, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
            _protector = provider.CreateProtector(p.key);
        }

        // GET: AdminController
        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            var p = products.Select(x => new ProductEdit
            {
                ProId = x.ProId,
                ProName = x.ProName,
                ProImage = x.ProImage,
                OpeningQuantity = x.OpeningQuantity,
                Description = x.Description,
                ProPrice = x.ProPrice,
                CategoryId = x.CategoryId,
                EncId = _protector.Protect(x.ProId.ToString())
            }).ToList();
            ViewData["Product"] = products;
            return View(p);
        }

        // GET: AdminController/Details/5
        public IActionResult Details(string id)
        {
            //return Json(id);
            int proid = Convert.ToInt32(_protector.Unprotect(id));

            var u = _context.Products.Where(x => x.ProId == proid).First();
            return View(u);
            //return Json(u);

        }


        // GET: AdminController/Create
        public ActionResult CreateProduct()
        {
            var categories = _context.Categories.ToList();

            ViewData["cat"] = new SelectList(categories, nameof(Category.CatId), nameof(Category.CatName));

            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(ProductEdit p)
        {
            short maxid;
            if (_context.Products.Any())
                maxid = Convert.ToInt16(_context.Products.Max(x => x.ProId) + 1);
            else
                maxid = 1;
            p.ProId = maxid;

            if (p.ProductFile != null)
            {
                string fileName = "ProductImage" + Guid.NewGuid() + Path.GetExtension(p.ProductFile.FileName);
                string filePath = Path.Combine(_env.WebRootPath, "ProductImage", fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    p.ProductFile.CopyTo(stream);
                }
                p.ProImage = fileName;
            }



            Product product = new()
            {
                ProId = p.ProId,
                ProName = p.ProName,
                ProPrice = p.ProPrice,
                ProImage = p.ProImage,
                Description = p.Description,
                OpeningQuantity = p.OpeningQuantity,
                CategoryId = p.CategoryId
            };
           // return Json(product);
            _context.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminController/Edit/5

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product? product = _context.Products.Where(e => e.ProId == id).FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }


            ProductEdit productss = new()
            {
                ProId = product.ProId,
                ProName = product.ProName,
                ProPrice = product.ProPrice,
                OpeningQuantity = product.OpeningQuantity,
                Description = product.Description,
                ProImage = product.ProImage,
                CategoryId = product.CategoryId


            };
            return View(productss);
        }

        /* public ActionResult Edit(ProductEdit pro)
         {
             short maxid;
             try
             {


                 if (_context.Products.Any())
                     maxid = Convert.ToInt16(_context.Products.Max(x => x.ProId + 1));
                 else
                     maxid = 1;
                 pro.ProId = maxid;

                 if (pro.ProductFile != null)
                 {
                     string fileName = Guid.NewGuid() + Path.GetExtension(pro.ProductFile.FileName);
                     // webRootPath is for directory 
                     string filePath = Path.Combine(_env.WebRootPath, "BlogImage", fileName);
                     using (FileStream stream = new FileStream(filePath, FileMode.Create))
                     {
                         pro.ProductFile.CopyTo(stream);
                     }
                     pro.ProImage = fileName;
                 }

                 short cid;
                 if (_context.Categories.Any())
                     cid = Convert.ToInt16(_context.Categories.Max(x => x.CatId + 1));
                 else
                     cid = 1;
                 pro.CategoryId = cid;
                 Category c = new Category()
                 {
                     CatId = pro.CategoryId,
                     CatName = pro.CatName

                 };
                 _context.Add(c);

                 Product p = new()
                 {
                     ProId = pro.ProId,
                     ProName = pro.ProName,
                     ProPrice = pro.ProPrice,
                     ProImage = pro.ProImage,
                     Description = pro.Description,
                     OpeningQuantity = pro.OpeningQuantity,
                     CategoryId = pro.CategoryId
                 };

                 _context.Add(p);
                 _context.SaveChanges();
                 return Content("success");
             }
             catch
             {
                 return View();
             }
         }*/


        [HttpPost]
        public async Task<IActionResult> Edit(ProductEdit p)
        {
            var product = await _context.Products.Where(e => e.ProId == p.ProId).FirstAsync();


            if (p.ProductFile != null)
            {
                string fileName = "ProductImage" + Guid.NewGuid() + Path.GetExtension(p.ProductFile.FileName);
                string filePath = Path.Combine(_env.WebRootPath, "ProductImage", fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    p.ProductFile.CopyTo(stream);
                }
                p.ProImage = fileName;
            }

            product.ProName = p.ProName;
            product.ProPrice = p.ProPrice;
            product.OpeningQuantity = p.OpeningQuantity;
            product.Description = p.Description;
          
            product.ProImage = p.ProImage;
            product.ProPrice = p.ProPrice;
           
            product.CategoryId = product.CategoryId;



            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: AdminController/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            //  return Json(id);
            var u = _context.Products.Where(x => x.ProId == id).FirstOrDefault();

            _context.Products.Remove(u);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
