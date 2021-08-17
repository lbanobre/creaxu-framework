using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Creaxu.Core.Services
{
    public interface IQueueService
    {
        Task PutAsync(string queueName, string message);
        Task PutAsync(string queueName, string message, TimeSpan visibilityTimeout);
        Task<string> GetAsync(string queueName);
    }

    public class QueueService : IQueueService
    {
        private readonly IConfiguration _configuration;

        public QueueService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        private QueueClient CreateQueueClient(string queueName)
        {
            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };
            
            var queueClient = new QueueClient(_configuration["AppSettings:Storage"], queueName, options);
            queueClient.CreateIfNotExists();

            return queueClient;
        }

        public async Task PutAsync(string queueName, string message)
        {
            var queueClient = CreateQueueClient(queueName);
            await queueClient.SendMessageAsync(message);
        }
        
        public async Task PutAsync(string queueName, string message, TimeSpan visibilityTimeout)
        {
            var queueClient = CreateQueueClient(queueName);
            await queueClient.SendMessageAsync(message, visibilityTimeout);
        }

        public async Task<string> GetAsync(string queueName)
        {
            var queueClient = CreateQueueClient(queueName);

            QueueMessage message = await queueClient.ReceiveMessageAsync();
            if (message == null) 
                return null;
            
            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

            return message.MessageText;
        }
    }
}