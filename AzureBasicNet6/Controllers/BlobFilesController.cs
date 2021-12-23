using AzureBasicNet6.Models;
using AzureBasicNet6.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AzureBasicNet6.Controllers
{
    public class BlobFilesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BlobStorageService _blobService;
        private readonly IWebHostEnvironment _env;

        public BlobFilesController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _configuration = configuration;
            _env = env;
            _blobService = new BlobStorageService(_configuration, _env);
        }

        public async Task<IActionResult> Index()
        {
            var blobs = await _blobService.GetAllBlobs();
            return View(blobs);
        }

        public async Task<IActionResult> Download(string fileUrl)
        {
            if (fileUrl == null) return NoContent();
            await _blobService.DownloadFile(fileUrl);
            return RedirectToAction("Index", new { controller = "BlobFiles" });
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(BlobFile blobFile)
        {
            if (ModelState.IsValid)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                bool exists = Directory.Exists(uploads);
                if (!exists) Directory.CreateDirectory(uploads);

                var fileName = Path.GetFileName(blobFile.File.FileName);
                var fileStream = new FileStream(Path.Combine(uploads, blobFile.File.FileName), FileMode.Create);
                string mimeType = blobFile.File.ContentType;
                byte[] fileData = new byte[blobFile.File.Length];

                _blobService.UploadFileToBlob(blobFile.File.FileName, fileData, mimeType);

                return RedirectToAction(nameof(Index));
            }
            return View(blobFile);
        }

        public async Task<IActionResult> Remove(string? fileUrl)
        {
            if (fileUrl == null)
            {
                return NotFound();
            }

            var blobFile = await _blobService.GetBlobByFileUrl(fileUrl);
            if (blobFile == null)
            {
                return NotFound();
            }

            return View(blobFile);
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string fileUrl)
        {
            _blobService.DeleteBlobData(fileUrl);
            return RedirectToAction("Index", new { controller = "BlobFiles" });
        }
    }
}
