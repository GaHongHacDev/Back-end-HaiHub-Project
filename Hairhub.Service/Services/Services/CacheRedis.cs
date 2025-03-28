using Google.Apis.Json;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class CacheRedis : ICacheRedis
    {
        private readonly StackExchange.Redis.IDatabase _database;

        public CacheRedis(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value)!;
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<IEnumerable<T>>(value)!;
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
        }

        public async Task SetListAsync<T>(string key, IEnumerable<T> values, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(values), expiry);
        }
    }
}
