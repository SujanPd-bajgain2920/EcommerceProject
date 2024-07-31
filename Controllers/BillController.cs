using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

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
                    Product = new Product
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
            var products = _context.Products.ToList();
            ViewBag.Products = products;

            var model = new BillRecordEdit
            {
                BillDetails = products.Select(p => new BillDetail
                {
                    Product = new Product { ProId = p.ProId, ProName = p.ProName, ProPrice = p.ProPrice },
                    Quantity = 0,
                    Amount = 0
                }).ToList()
            };

            return View(model);
        }       

        [HttpPost]
        public async Task<IActionResult> CreateBill(BillRecordEdit model)
        {
            try
            {
                // Validate BillDate
                var billDate = model.BillDate > DateTime.MinValue ? model.BillDate : DateTime.Today;

                // Validate CancelDate
                var cancelDate = model.CancelDate > DateTime.MinValue ? model.CancelDate : DateTime.Today;

                // Validate PrintDate
                var printDate = DateOnly.FromDateTime(DateTime.Today);

                // Check User.Identity
                if (User.Identity == null || !int.TryParse(User.Identity.Name, out int userId))
                {
                    return Content("Error: Invalid user identity.");
                }

                var maxBillId = _context.BillRecords.Any() ? _context.BillRecords.Max(x => x.Bid) + 1 : 1;
                var maxBillDetailId = _context.BillDetails.Any() ? _context.BillDetails.Max(x => x.Bdid) + 1 : 1;
                var maxPrintId = _context.BillPrints.Any() ? _context.BillPrints.Max(x => x.PrintId) + 1 : 1;

                var billRecord = new BillRecord
                {
                    Bid = maxBillId,
                    Bno = model.BillNo,
                    TotalAmount = model.TotalAmount ?? 0,
                    DiscountAmount = 0,
                    BillDate = billDate,
                    TransactionType = model.TransactionType ?? "sales",
                    ReasonForCancel = model.ReasonforCancel,
                    CancelDate = cancelDate,
                    CancelByUserId = model.CancelByUserId,
                    EntryByUserId = Convert.ToInt16(User.Identity.Name)
                };

                _context.Add(billRecord);

                foreach (var detail in model.BillDetails)
                {
                    if (detail.Quantity > 0)
                    {
                        var existingProduct = await _context.Products.FindAsync(detail.ProductId);
                        if (existingProduct != null)
                        {
                            detail.Product = existingProduct;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Product with ID {detail.ProductId} not found.");
                            return View(model);
                        }

                        detail.Bid = billRecord.Bid;
                        detail.Bdid = Convert.ToInt16(maxBillDetailId++);
                        _context.Add(detail);
                    }
                }

                var billPrint = new BillPrint
                {
                    PrintId = Convert.ToInt16(maxPrintId),
                    Bid = billRecord.Bid,
                    PrintDate = printDate,
                    PrintBy = Convert.ToInt16(User.Identity.Name),
                    PrintTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddMinutes(345))
                };

                _context.Add(billPrint);

                await _context.SaveChangesAsync();

                return Content("Success");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }



    }
}