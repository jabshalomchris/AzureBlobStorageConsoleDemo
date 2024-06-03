using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageConsoleDemo
{
    public class AzureBlobStorageHelper
    {
        private readonly BlobContainerClient _blobContainerClient;

        public AzureBlobStorageHelper(string BlobUrl, string BlobSASKey)
        {
            Uri uri = new Uri(BlobUrl);

            if (uri.Segments.Length < 2)
            {
                throw new ArgumentException("Invalid URL format. Expected at least two segments in the URL.");
            }

            var containerName = uri.Segments[1]; // Segments[1] contains the container name

            UriBuilder sasUri = new UriBuilder(BlobUrl)
            {
                Query = BlobSASKey
            };

            // Initiate a BlobServiceClient
            BlobServiceClient _blobServiceClient = new BlobServiceClient(sasUri.Uri);

            // Get a reference to a container
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        }

        private BlobClient GetBlobClient(string filePath, string blobName) =>
            _blobContainerClient.GetBlobClient($"{filePath}/{blobName}");

        public async Task SaveFileToStorageAsync(string filePath, string blobName, FileStream stream)
        {
            BlobClient blobClient = GetBlobClient(filePath, blobName);

            if (await blobClient.ExistsAsync())
            {
                // File already exists, no need to upload
                return;
            }

            await blobClient.UploadAsync(stream, overwrite: true);
        }

        public async Task<bool> CheckIfFileExistsAsync(string filePath, string blobName) =>
            await GetBlobClient(filePath, blobName).ExistsAsync();

        public async Task<bool> DeleteFileAsync(string filePath, string blobName) =>
            await GetBlobClient(filePath, blobName).DeleteIfExistsAsync();

        public async Task<MemoryStream> GetFileContentAsync(string filePath, string blobName)
        {
            BlobClient blobClient = GetBlobClient(filePath, blobName);
            Response<BlobDownloadInfo> downloadResponse = await blobClient.DownloadAsync();

            MemoryStream stream = new MemoryStream();
            using (Stream fileStream = downloadResponse.Value.Content)
            {
                await fileStream.CopyToAsync(stream);
            }

            stream.Position = 0;
            return stream;
        }
    }
}
