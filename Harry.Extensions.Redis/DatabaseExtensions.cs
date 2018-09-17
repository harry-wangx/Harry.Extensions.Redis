using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Extensions.Redis
{
    public static class DatabaseExtensions
    {
        public static JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" };

        public static T Get<T>(this IDatabase db, string key, Func<T> func = null, TimeSpan? expiry = null, JsonSerializerSettings settings = null) where T : class
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            settings = settings ?? DefaultJsonSerializerSettings;

            var redisValue = db.StringGet(key);
            if (redisValue.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(redisValue.ToString(), settings);
            }

            if (func != null)
            {
                lock (key)
                {
                    redisValue = db.StringGet(key);
                    if (redisValue.HasValue)
                    {
                        return JsonConvert.DeserializeObject<T>(redisValue.ToString(), settings);
                    }

                    T result = func.Invoke();
                    if (result == null)
                        return result;

                    db.StringSet(key, JsonConvert.SerializeObject(result, settings), expiry);

                    return result;
                }
            }
            return null;
        }
    }
}
