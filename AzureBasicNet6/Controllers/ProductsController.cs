using AzureBasicNet6.Database;
using AzureBasicNet6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureBasicNet6.Controllers
{
    public class ProductsController : Controller
    {
        private UnitOfWork _context;
        private ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;

        public ProductsController(IWebHostEnvironment env, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _context = new UnitOfWork(_applicationDbContext);
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.ProductRepo.GetAll());
        }

        public IActionResult View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.ProductRepo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.ProductRepo.Add(product);
                await _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.ProductRepo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ProductId,Name,UnitPrice,Description,ImageName,ImagePath,CreatedDate,UpdatedDate")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.ProductRepo.Update(product);
                    await _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.ProductRepo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = _context.ProductRepo.GetById(id);
            _context.ProductRepo.Delete(product);
            await _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.ProductRepo.GetAll().Any(e => e.ProductId == id);
        }
    }
}
