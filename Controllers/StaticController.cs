using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class StaticController : Controller
    {
        private readonly InventoryManagementContext _context;

        public StaticController(InventoryManagementContext context) { 
        _context = context;
        }
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
                CategoryId = x.CategoryId
            }).ToList();
            ViewData["Product"] = products;
            return View(p);
        }

        public IActionResult About()
        {
            return View();
        }

    }
}
