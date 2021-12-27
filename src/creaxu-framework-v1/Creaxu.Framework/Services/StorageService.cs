using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Creaxu.Framework.Helpers;

namespace Creaxu.Core.Services
{
    public interface IStorageService
    {
        Task<byte[]> GetBytesAsync(string containerName, string fileName);
        Task<Stream> GetStreamAsync(string containerName, string fileName);
        Task<string> GetStringAsync(string containerName, string fileName);
        Task<string> GetUrlAsync(string containerName, string fileName);
        Task UploadAsync(string containerName, string fileName, Stream stream, string contentType = null, bool reportProgress = false);
        Task UploadAsync(string containerName, string fileName, byte[] source, string contentType = null);
        Task DeleteAsync(string containerName, string fileName);
    }

    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<BlobContainerClient> CreateIfNotExistsAsync(string containerName)
        {
            var container = new BlobContainerClient(_configuration["AppSettings:Storage"], containerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }

        public async Task<byte[]> GetBytesAsync(string containerName, string fileName)
        {
            try
            {
                var container = await CreateIfNotExistsAsync(containerName);
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
                var container = await CreateIfNotExistsAsync(containerName);
                var blob = container.GetBlobClient(fileName);

                return await blob.OpenReadAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetUrlAsync(string containerName, string fileName)
        {
            try
            {
                var container = await CreateIfNotExistsAsync(containerName);
                var blob = container.GetBlobClient(fileName);

                return container.Uri.AbsoluteUri + "/" + blob.Name;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetStringAsync(string containerName, string fileName)
        {
            var bytes = await GetBytesAsync(containerName, fileName);

            if (bytes == null)
                return null;

            return Encoding.UTF8.GetString(bytes);
        }

        public async Task UploadAsync(string containerName, string fileName, Stream stream, string contentType = null, bool reportProgress = false)
        {
            var container = await CreateIfNotExistsAsync(containerName);
            var blockBlobClient = container.GetBlockBlobClient(fileName);

            BlobClient blobProgress = null;

            const int blockSize = 1 * 1024 * 1024; //1 MB Block
            const int offset = 0;
            var counter = 0;
            var blockIds = new List<string>();

            var bytesRemaining = stream.Length;
            do
            {
                if (reportProgress)
                {
                    if (blobProgress == null)
                    {
                        blobProgress = container.GetBlobClient($"{fileName}.progress");
                    }

                    var p = (double)(stream.Length - bytesRemaining) / stream.Length * 100;
                    await blobProgress.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(p.ToString(CultureInfo.InvariantCulture))), true);
                }

                var dataToRead = Math.Min(bytesRemaining, blockSize);
                var data = new byte[dataToRead];
                var dataRead = await stream.ReadAsync(data.AsMemory(offset, (int)dataToRead));
                bytesRemaining -= dataRead;

                if (dataRead > 0)
                {
                    var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(counter.ToString("d6")));
                    await blockBlobClient.StageBlockAsync(blockId, new MemoryStream(data));
                    blockIds.Add(blockId);
                    counter++;
                }
            }
            while (bytesRemaining > 0);

            await blockBlobClient.CommitBlockListAsync(blockIds);

            if (string.IsNullOrEmpty(contentType))
            {
                await blockBlobClient.SetHttpHeadersAsync(new BlobHttpHeaders
                { ContentType = MimeTypeMap.GetMimeType(Path.GetExtension(fileName)) });
            }
            else
            {
                await blockBlobClient.SetHttpHeadersAsync(new BlobHttpHeaders
                { ContentType = contentType });
            }

            if (blobProgress != null)
            {
                await blobProgress.DeleteIfExistsAsync();
            }
        }

        public async Task UploadAsync(string containerName, string fileName, byte[] source, string contentType = null)
        {
            await UploadAsync(containerName, fileName, new MemoryStream(source), contentType, false);
        }

        public async Task DeleteAsync(string containerName, string fileName)
        {
            var container = await CreateIfNotExistsAsync(containerName);
            var blob = container.GetBlobClient(fileName);

            await blob.DeleteIfExistsAsync();
        }
    }
}