using FJPH.MicroService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FJPH.FileUploader.Model
{
    /// <summary>
    ///  配置文件中的一节 
    /// </summary>
    public class StorageConfigModel : BaseModel
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string StoragePath { get; set; }


        /// <summary>
        /// 基础URL
        /// </summary>
        public string BaseUrl { get; set; }


    }
}
