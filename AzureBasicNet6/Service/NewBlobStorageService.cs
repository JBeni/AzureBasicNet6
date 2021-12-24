using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBasicNet6.Models;

namespace AzureBasicNet6.Service
{
    public class NewBlobStorageService
    {
        private readonly string accessKey = string.Empty;
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewBlobStorageService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            accessKey = _configuration["ConnectionStrings:AzureBlobStorage"];
        }

        public async Task<BlobFile> GetBlobByFileUrl(string fileUrl)
        {
            Uri uriObj = new(fileUrl);
            string blobName = Path.GetFileName(uriObj.LocalPath);
            var blobClient = new BlobClient(accessKey, "uploads", blobName);

            return new BlobFile
            {
                Name = blobClient.Name,
                FileUrl = blobClient.Uri.AbsoluteUri,
            };
        }

        public async Task DownloadFile(string fileUrl)
        {
            Uri uriObj = new(fileUrl);
            string blobName = Path.GetFileName(uriObj.LocalPath);
            var blobClient = new BlobClient(accessKey, "uploads", blobName);

            string directory = Path.Combine(_webHostEnvironment.ContentRootPath, "downloads");
            await blobClient.DownloadToAsync(directory);
            //new BlobClient(new Uri(fileUrl)).DownloadTo(directory);
        }

        public async void DeleteBlobData(string fileUrl)
        {
            Uri uriObj = new(fileUrl);
            string blobName = Path.GetFileName(uriObj.LocalPath);
            var blobClient = new BlobClient(accessKey, "uploads", blobName);
            await blobClient.DeleteAsync();
        }

        public async Task UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                BlobContainerClient container = new BlobContainerClient(accessKey, "uploads");
                await container.CreateIfNotExistsAsync();
                await container.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

                BlobClient blob = container.GetBlobClient(strFileName);
                Stream stream = new MemoryStream(fileData);
                await blob.UploadAsync(stream);

                IDictionary<string, string> metadata = new Dictionary<string, string>
                {
                    { "blobUrl", blob.Uri.AbsoluteUri },
                    { "CreatedDate", DateTime.Now.ToString() }
                };
                await blob.SetMetadataAsync(metadata);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlobFile>> GetAllBlobs()
        {
            var blobServiceClient = new BlobServiceClient(accessKey);
            var container = blobServiceClient.GetBlobContainerClient("uploads");

            List<BlobFile> blobNames = new();
            var blobHierarchyItems = container.GetBlobsByHierarchyAsync(BlobTraits.All, BlobStates.None, "/");
            await foreach (var blobHierarchyItem in blobHierarchyItems)
            {
                if (blobHierarchyItem.IsPrefix)
                {
                    // You can also access files under nested folders (you can do a recursive function)
                }
                else
                {
                    blobNames.Add(new BlobFile
                    {
                        Name = blobHierarchyItem.Blob.Name,
                        FileUrl = blobHierarchyItem.Blob.Metadata["blobUrl"],
                        CreatedDate = DateTime.Parse(blobHierarchyItem.Blob.Metadata["CreatedDate"])
                    });
                }
            }
            return blobNames;
        }
    }
}
