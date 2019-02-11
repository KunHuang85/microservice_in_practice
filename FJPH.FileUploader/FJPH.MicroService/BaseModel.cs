using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService
{
    public class BaseModel
    {

        /// <summary>
        /// 使用JSON 序列化这个Model
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string msg = JsonConvert.SerializeObject(this, Formatting.Indented, jsetting);
            return msg;
        }
    }
}
