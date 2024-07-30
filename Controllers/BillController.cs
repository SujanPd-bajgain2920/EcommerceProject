using InventoryManagementSystem.Models;
using InventoryManagementSystem.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{

    public class BillController : Controller
    {
        private readonly InventoryManagementContext _context;

        public BillController(InventoryManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            var sales = new BillRecordEdit
            {
                BillDetails = new List<BillDetail>()
            };

            foreach (var p in products)
            {
                sales.BillDetails.Add(new BillDetail
                {
                    Product = new Product // Initialize the Product object if needed
                    {
                        ProPrice = p.ProPrice,
                        ProName = p.ProName
                    }
                });
            }

            return View(sales);
        }


        [HttpGet]
        public IActionResult CreateBill()
        {
            var model = new BillRecordEdit
            {
                BillDetails = new List<BillDetail>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill(BillRecordEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Generate new IDs
                var maxBillId = _context.BillRecords.Any() ? _context.BillRecords.Max(x => x.Bid) + 1 : 1;
                var maxBillDetailId = _context.BillDetails.Any() ? _context.BillDetails.Max(x => x.Bdid) + 1 : 1;
                var maxPrintId = _context.BillPrints.Any() ? _context.BillPrints.Max(x => x.PrintId) + 1 : 1;
        

                // Create new BillRecord
                var billRecord = new BillRecord
                {
                    Bid = maxBillId,
                    Bno = model.BillNo,
                    TotalAmount = model.TotalAmount ?? 0,
                    DiscountAmount = 0, // Set discount amount if applicable
                    BillDate = DateTime.Now,
                    TransactionType = model.TransactionType ?? "sales",
                    ReasonForCancel = model.ReasonforCancel,
                    CancelDate = model.CancelDate,
                    CancelByUserId = model.CancelByUserId,
                    EntryByUserId = Convert.ToInt16(User.Identity.Name)
                };

                _context.BillRecords.Add(billRecord);

                // Create BillDetails
                foreach (var detail in model.BillDetails)
                {
                    var billDetail = new BillDetail
                    {
                        Bdid = Convert.ToInt16(maxBillDetailId),
                        Bid = billRecord.Bid,
                        ProductId = detail.Product.ProId,
                        Rate = detail.Product.ProPrice,
                        Quantity = detail.Quantity,
                        Amount = detail.Amount
                    };

                    _context.BillDetails.Add(billDetail);
                    maxBillDetailId++;
                }

                // Create BillPrint
                var billPrint = new BillPrint
                {
                    PrintId = Convert.ToInt16(maxPrintId),
                    Bid = billRecord.Bid,
                    PrintDate = DateOnly.FromDateTime(DateTime.Today),
                    PrintBy = Convert.ToInt16(User.Identity.Name),
                    PrintTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddMinutes(345)),
                };

                _context.BillPrints.Add(billPrint);

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return Content("Success");
            }
            catch (Exception ex)
            {
                // Log exception (implement logging if needed)
                return Content($"Error: {ex.Message}");
            }
        }
    }
}