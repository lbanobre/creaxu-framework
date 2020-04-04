using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Creaxu.Framework.Services
{
    public interface IStorageService
    {
        Task<byte[]> GetBytesAsync(string container, string fileName);
        Task<Stream> GetStreamAsync(string container, string fileName);
        Task<string> GetStringAsync(string container, string fileName);
        Task UploadAsync(string container, string fileName, Stream source);
        Task UploadAsync(string container, string fileName, byte[] source);
        Task DeleteAsync(string container, string fileName);
        Task<List<Uri>> GetDirectoriesAsync(string container, string relativeAddress);
        Task<List<Uri>> GetBlobsAsync(string container);
        Task SetMetadataAsync(string container, string fileName, string metadataKey, string metadataValue);
        string GetMetadata(string container, string fileName, string metadataKey);
    }

    public class StorageService : IStorageService
    {
        readonly CloudBlobClient _blobClient;

        private readonly IConfiguration _configuration;

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;

            var storageAccount = CloudStorageAccount.Parse(_configuration["AppSettings:Storage"]);

            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<byte[]> GetBytesAsync(string container, string fileName)
        {
            try
            {
                var containerRef = _blobClient.GetContainerReference(container);
                var blockBlob = containerRef.GetBlockBlobReference(fileName);

                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    await blockBlob.DownloadToStreamAsync(ms);
                    bytes = ms.ToArray();
                }
                return bytes;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Stream> GetStreamAsync(string container, string fileName)
        {
            try
            {
                var containerRef = _blobClient.GetContainerReference(container);
                var blockBlob = containerRef.GetBlockBlobReference(fileName);

                return await blockBlob.OpenReadAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetStringAsync(string container, string fileName)
        {
            try
            {
                var containerRef = _blobClient.GetContainerReference(container);
                var blockBlob = containerRef.GetBlockBlobReference(fileName);

                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    await blockBlob.DownloadToStreamAsync(ms);
                    bytes = ms.ToArray();
                }
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return null;
            }
        }

        public async Task UploadAsync(string container, string fileName, Stream source)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            var blockBlob = containerRef.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromStreamAsync(source);
        }

        public async Task UploadAsync(string container, string fileName, byte[] source)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            var blockBlob = containerRef.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromByteArrayAsync(source, 0, source.Length);
        }

        public async Task DeleteAsync(string container, string fileName)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            var blockBlob = containerRef.GetBlockBlobReference(fileName);

            await blockBlob.DeleteIfExistsAsync();
        }

        public async Task SetMetadataAsync(string container, string fileName, string metadataKey, string metadataValue)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            var blockBlob = containerRef.GetBlockBlobReference(fileName);

            blockBlob.Metadata[metadataKey] = metadataValue;
            
            await blockBlob.SetMetadataAsync();
        }

        public string GetMetadata(string container, string fileName, string metadataKey)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            var blockBlob = containerRef.GetBlockBlobReference(fileName);
           
            return blockBlob.Metadata[metadataKey];
        }

        public async Task<List<Uri>> GetDirectoriesAsync(string container, string relativeAddress)
        {
            var result = new List<Uri>();

            var containerRef = _blobClient.GetContainerReference(container);
            var directoryRef = containerRef.GetDirectoryReference(relativeAddress);


            BlobContinuationToken continuationToken = null;
            do
            {
                var blobResultSegment = await directoryRef.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = blobResultSegment.ContinuationToken;

                foreach (var item in blobResultSegment.Results)
                {
                    if (item is CloudBlobDirectory)
                    {
                        result.Add(((CloudBlobDirectory)item).Uri);
                    }
                }
               
            } 
            while (continuationToken != null);

            return result;
        }

        public async Task<List<Uri>> GetBlobsAsync(string container)
        {
            var result = new List<Uri>();

            var containerRef = _blobClient.GetContainerReference(container);
            
            BlobContinuationToken continuationToken = null;
            do
            {
                var blobResultSegment = await containerRef.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = blobResultSegment.ContinuationToken;

                foreach (var item in blobResultSegment.Results)
                {
                    if (item is CloudBlockBlob)
                    {
                        result.Add(((CloudBlockBlob)item).Uri);
                    }
                }

            }
            while (continuationToken != null);

            return result;
        }
    }
}
