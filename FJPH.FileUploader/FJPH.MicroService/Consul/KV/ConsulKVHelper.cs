using Consul;
using FJPH.MicroService.Util.Env;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FJPH.MicroService
{
    public class ConsulKVHelper : IConsulKVHelper
    {
        private readonly ConsulClient client;
        private readonly EnvModel envModel;
        private readonly string keyPrefix;
               

        public ConsulKVHelper(ConsulClient client)
        {
            this.envModel = EnvHelper.GetEnv();
            this.client = client;
            this.keyPrefix = $"apps/{envModel.APP_BUILD_NAME}/{envModel.ASPNETCORE_ENVIRONMENT}/";
        }

        public async Task<QueryResult<KVPair[]>> GetKvList(string prefix, CancellationToken ct = default(CancellationToken))
        {
            return await client.KV.List(prefix, ct);
        }

        public async Task<QueryResult<string[]>> GetKvKeys(string prefix, CancellationToken ct = default(CancellationToken))
        {
            return await client.KV.Keys(prefix, ct);
        }


        public async Task<string> GetValueAsync(string key)
        {

            var getPair = await client.KV.Get(key);
            string strValue = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
            return strValue;
        }

        public async Task<bool> SetValueAsync(string key, string value)
        {
            var putPair = new KVPair(key)
            {
                Value = Encoding.UTF8.GetBytes(value)
            };
            var putAttempt = await client.KV.Put(putPair);
            return putAttempt.Response;

        }


        public async Task<T> GetJsonModelAsync<T>(string key) where T : class
        {
            Console.WriteLine($"DEBUG: GET KV Key=[{key}]");
            string strValue = await GetValueAsync(key);
            Console.WriteLine($"DEBUG: GET KV Value=[{strValue}]");
            T t = JsonConvert.DeserializeObject<T>(strValue);
            return t;
        }

        public async Task<bool> SetJsonModelAsync<T>(string key, T t) where T : class
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string msg = JsonConvert.SerializeObject(t, Formatting.Indented, jsetting);
            return await SetValueAsync(key, msg);
        }


        public async Task<T> GetAppJsonModelAsync<T>(string section) where T : class
        {
            string fullKey = this.keyPrefix + section;
            return await this.GetJsonModelAsync<T>(fullKey);
        }


        public async Task<bool> SetAppJsonModelAsync<T>(string section, T t) where T : class
        {
            string fullKey = this.keyPrefix + section;
            return await this.SetJsonModelAsync<T>(fullKey, t);
        }
    }
}
