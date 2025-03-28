using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface ICacheRedis
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key); 
        Task<bool> ExistsAsync(string key); 
        Task SetListAsync<T>(string key, IEnumerable<T> values, TimeSpan? expiry = null);
        Task<IEnumerable<T>> GetListAsync<T>(string key);
    }
}
