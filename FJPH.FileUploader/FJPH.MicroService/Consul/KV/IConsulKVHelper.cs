using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FJPH.MicroService
{
    public interface IConsulKVHelper
    {

        /// <summary>
        /// 获取所有的Key值
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<QueryResult<string[]>> GetKvKeys(string prefix, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// 获取所有的KeyValue
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<QueryResult<KVPair[]>> GetKvList(string prefix, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// 设置 Key/Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> SetValueAsync(string key, string value);

        /// <summary>
        /// 根据Key 获取Value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValueAsync(string key);

        /// <summary>
        /// 根据Key 获取Value，并反序列化为泛型T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetJsonModelAsync<T>(string key) where T : class;

        /// <summary>
        /// 写入KV值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> SetJsonModelAsync<T>(string key, T t) where T : class;

        /// <summary>
        /// 根据Key 获取Value，并反序列化为泛型T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAppJsonModelAsync<T>(string section) where T : class;

        /// <summary>
        /// 写入KV值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> SetAppJsonModelAsync<T>(string section, T t) where T : class;







    }
}
