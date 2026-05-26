using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockBoutique.Data;
using StockBoutique.Models;
using System.Diagnostics;

namespace StockBoutique.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalStockValue = await _context.Products.SumAsync(p => p.Price * p.QuantityInStock);
            var lowStockProducts = await _context.Products.CountAsync(p => p.QuantityInStock < 5);

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalStockValue = totalStockValue;
            ViewBag.LowStockProducts = lowStockProducts;

            var recentTransactions = await _context.StockTransactions
                .Include(t => t.Product)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToListAsync();

            return View(recentTransactions);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
