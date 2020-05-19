using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Creaxu.Framework.Shared.CosmosDb;
using Humanizer;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Creaxu.Framework.Services
{
    public interface ICosmosDbService<T>
    {
        Task<T> CreateItemAsync(T item);
        Task UpsertItemAsync(T item);
        Task DeleteItemAsync(Guid id, string partitionKey);
        Task<T> GetItemAsync(Guid id, string partitionKey);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetItemsAsync();
    }

    public class CosmosDbService<T> : ICosmosDbService<T> where T : BaseItem
    {
        private readonly CosmosClient _client;
        private readonly Container _container;

        public CosmosDbService(IConfiguration configuration)
        {
            string accountEndpoint = configuration["CosmosDb:AccountEndpoint"];
            string accountKey = configuration["CosmosDb:AccountKey"];

            var clientBuilder = new CosmosClientBuilder(accountEndpoint, accountKey);
            _client = clientBuilder.WithConnectionModeDirect().Build();

            string databaseId = configuration["CosmosDb:DatabaseId"];
            string containerId = typeof(T).Name.Pluralize();

            CreateDatabase(databaseId, containerId).GetAwaiter().GetResult();

            _container = _client.GetContainer(databaseId, containerId);
        }

        private async Task CreateDatabase(string databaseId, string containerId)
        {
            var database = await _client.CreateDatabaseIfNotExistsAsync(databaseId); // autopilot 4000

            await database.Database.CreateContainerIfNotExistsAsync(containerId, "/pk");
        }

        public async Task<T> CreateItemAsync(T item)
        {
            return (await _container.CreateItemAsync(item, new PartitionKey(item.PartitionKey))).Resource;
        }

        public async Task UpsertItemAsync(T item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(item.PartitionKey), new ItemRequestOptions { IfMatchEtag = item._etag });
        }

        public async Task DeleteItemAsync(Guid id, string partitionKey)
        {
            await _container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(partitionKey));
        }

        public async Task<T> GetItemAsync(Guid id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id.ToString(), new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            var queryable = _container.GetItemLinqQueryable<T>();

            return queryable.Where(predicate).AsEnumerable().SingleOrDefault();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var queryable = _container.GetItemLinqQueryable<T>();

            return queryable.Where(predicate).AsEnumerable().FirstOrDefault();
        }

        public async Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var queryable = _container.GetItemLinqQueryable<T>();

            var results = new List<T>();
            string continuationToken = null;
            do
            {
                var feedIterator = queryable.Where(predicate).ToFeedIterator();

                while (feedIterator.HasMoreResults)
                {
                    var feedResponse = await feedIterator.ReadNextAsync();
                    continuationToken = feedResponse.ContinuationToken;

                    results.AddRange(feedResponse.ToList());
                }
            } while (continuationToken != null);

            return results;
        }

        public async Task<List<T>> GetItemsAsync()
        {
            var queryable = _container.GetItemLinqQueryable<T>();

            var results = new List<T>();
            string continuationToken = null;
            do
            {
                var feedIterator = queryable.ToFeedIterator();

                while (feedIterator.HasMoreResults)
                {
                    var feedResponse = await feedIterator.ReadNextAsync();
                    continuationToken = feedResponse.ContinuationToken;

                    results.AddRange(feedResponse.ToList());
                }
            } while (continuationToken != null);

            return results;
        }
    }
}
