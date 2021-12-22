using AzureBasicNet6.Database;
using AzureBasicNet6.Models;
using AzureBasicNet6.Service;
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

        // GET: Products
        public IActionResult Index()
        {
            return View(_context.ProductRepo.GetAll());
        }

        // GET: Products/View/5
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

        // GET: Products/Create
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

                #region Read File Content

                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                bool exists = Directory.Exists(uploads);
                if (!exists)
                    Directory.CreateDirectory(uploads);

                //string fileName = Path.GetFileName(product.File.FileName);
                //byte[] fileData;
                //using (var target = new MemoryStream())
                //{
                //    product.File.CopyTo(target);
                //    fileData = target.ToArray();
                //}


                //var fileStream = new FileStream(Path.Combine(uploads, product.File.FileName), FileMode.Create);
                //string mimeType = product.File.ContentType;
                //= new byte[product.File.Length];

                BlobStorageService objBlobService = new BlobStorageService(_configuration);

                //product.ImagePath = objBlobService.UploadFileToBlob(product.File.FileName, fileData, mimeType);
                #endregion

                _context.ProductRepo.Add(product);
                await _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Update/5
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

        // POST: Products/Update/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Products/Delete/5
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = _context.ProductRepo.GetById(id);
            BlobStorageService objBlob = new BlobStorageService(_configuration);
            //objBlob.DeleteBlobData(product.ImagePath);
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
