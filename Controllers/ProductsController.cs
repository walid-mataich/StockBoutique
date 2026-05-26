using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockBoutique.Data;
using StockBoutique.Models;

namespace StockBoutique.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,SKU,Price,QuantityInStock,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                
                // Add initial stock transaction if quantity > 0
                if (product.QuantityInStock > 0)
                {
                    _context.StockTransactions.Add(new StockTransaction
                    {
                        ProductId = product.Id,
                        Type = "In",
                        Quantity = product.QuantityInStock,
                        Reason = "Initial Stock"
                    });
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,SKU,Price,QuantityInStock,CategoryId")] Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }
        
        // POST: Products/AddStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(int id, int quantity, string reason)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (quantity > 0)
            {
                product.QuantityInStock += quantity;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = id,
                    Type = "In",
                    Quantity = quantity,
                    Reason = reason
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Products/RemoveStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStock(int id, int quantity, string reason)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (quantity > 0 && product.QuantityInStock >= quantity)
            {
                product.QuantityInStock -= quantity;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = id,
                    Type = "Out",
                    Quantity = quantity,
                    Reason = reason
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
