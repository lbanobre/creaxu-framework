using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;

namespace Creaxu.Framework.Services
{
    public interface IQueueService
    {
        Task<bool> ClearAsync(string queueName);
        Task DeleteAsync(string queueName);
        Task<bool> ExistsAsync(string queueName);
        Task<string> GetAsync(string queueName);
        Task<bool> PutAsync(string queueName, string content);
        Task<bool> PutAsync(string queueName, string content, TimeSpan initialVisibilityDelay);
    }

    public class QueueService : IQueueService
    {
        private readonly CloudQueueClient _queueClient;

        private readonly IConfiguration _configuration;

        public QueueService(IConfiguration configuration)
        {
            _configuration = configuration;

            var storageAccount = CloudStorageAccount.Parse(_configuration["AppSettings:Storage"]);

            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        public async Task<bool> PutAsync(string queueName, string content)
        {
            try
            {
                var queue = _queueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();

                await queue.AddMessageAsync(new CloudQueueMessage(content));
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> PutAsync(string queueName, string content, TimeSpan initialVisibilityDelay)
        {
            try
            {
                var queue = _queueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();

                await queue.AddMessageAsync(new CloudQueueMessage(content), null, initialVisibilityDelay, null, null);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetAsync(string queueName)
        {
            var queue = _queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();

            var message = await queue.GetMessageAsync();
            if (message != null)
            {
                var result = message.AsString;
                await queue.DeleteMessageAsync(message);

                return result;
            }
            return null;
        }

        public async Task<bool> ExistsAsync(string queueName)
        {
            try
            {
                var queue = _queueClient.GetQueueReference(queueName);
                return await queue.ExistsAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteAsync(string queueName)
        {
            try
            {
                var queue = _queueClient.GetQueueReference(queueName);
                await queue.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ClearAsync(string queueName)
        {
            try
            {
                var queue = _queueClient.GetQueueReference(queueName);
                await queue.ClearAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}