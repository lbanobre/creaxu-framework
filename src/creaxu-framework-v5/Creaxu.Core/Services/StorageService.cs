using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Creaxu.Core.Services
{
    public interface IStorageService
    {
        Task<byte[]> GetBytesAsync(string containerName, string fileName);
        Task<Stream> GetStreamAsync(string containerName, string fileName);
        Task<string> GetStringAsync(string containerName, string fileName);
        Task UploadAsync(string containerName, string fileName, Stream source);
        Task UploadAsync(string containerName, string fileName, byte[] source);
        Task DeleteAsync(string containerName, string fileName);
    }

    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<BlobContainerClient> CreateContainerAsync(string containerName)
        {
            var container   = new BlobContainerClient(_configuration["AppSettings:Storage"], containerName);
            await container.CreateAsync();

            return container;
        }
        
        public async Task<byte[]> GetBytesAsync(string containerName, string fileName)
        {
            try
            {
                var container = await CreateContainerAsync(containerName);
                var blob = container.GetBlobClient(fileName);

                BlobDownloadInfo download = await blob.DownloadAsync();
                
                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    bytes = ms.ToArray();
                }
                return bytes;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Stream> GetStreamAsync(string containerName, string fileName)
        {
            try
            {
                var container = await CreateContainerAsync(containerName);
                var blob = container.GetBlobClient(fileName);

                BlobDownloadInfo download = await blob.DownloadAsync();
                
                return download.Content;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetStringAsync(string containerName, string fileName)
        {
            var bytes = await GetBytesAsync(containerName, fileName);

                return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public async Task UploadAsync(string containerName, string fileName, Stream source)
        {
            var container = await CreateContainerAsync(containerName);
            await container.UploadBlobAsync(fileName, source);
        }

        public async Task UploadAsync(string containerName, string fileName, byte[] source)
        {
            await UploadAsync(containerName, fileName, new MemoryStream(source));
        }

        public async Task DeleteAsync(string containerName, string fileName)
        {
            var container = await CreateContainerAsync(containerName);
            var blob = container.GetBlobClient(fileName);

            await blob.DeleteIfExistsAsync();
        }
    }
}
