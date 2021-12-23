using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBasicNet6.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBasicNet6.Service
{
    public class BlobStorageService
    {
        readonly string accessKey = string.Empty;
        public IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlobStorageService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            accessKey = _configuration["ConnectionStrings:AzureBlobStorage"];
        }

        public string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                var _task = Task.Run(() => UploadFileToBlobAsync(strFileName, fileData, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BlobFile> GetBlobByFileUrl(string fileUrl)
        {
            Uri uriObj = new(fileUrl);
            string BlobName = Path.GetFileName(uriObj.LocalPath);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "uploads";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(BlobName);

            return new BlobFile
            {
                Name = blockBlob.Name,
                FileUrl = blockBlob.Uri.AbsoluteUri,
            };
        }

        public async Task DownloadFile(string fileUrl)
        {
            Uri uriObj = new(fileUrl);
            string blobName = Path.GetFileName(uriObj.LocalPath);

            var blobClient = new BlobClient(accessKey, "uploads", blobName);
            string directory = Path.Combine(_webHostEnvironment.ContentRootPath, "downloads");
            await blobClient.DownloadToAsync(directory);
        }

        public async void DeleteBlobData(string fileUrl)
        {
            Uri uriObj = new(fileUrl);
            string BlobName = Path.GetFileName(uriObj.LocalPath);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "uploads";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(BlobName);

            await blockBlob.DeleteAsync();
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = "uploads";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
                }

                if (strFileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(strFileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);

                    cloudBlockBlob.Metadata.Add("blobUrl", cloudBlockBlob.Uri.AbsoluteUri);
                    cloudBlockBlob.Metadata.Add("CreatedDate", DateTime.Now.ToString());
                    await cloudBlockBlob.SetMetadataAsync();

                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
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
                    // You can also access files under nested folders in this way,
                    // of course you will need to create a function accordingly (you can do a recursive function)
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
