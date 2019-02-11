using FJPH.MicroService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FJPH.FileUploader.Model
{

    /// <summary>
    /// 文件上传的返回值模型
    /// </summary>
    public class OperationResultModel : BaseModel
    {
        /// <summary>
        /// 写文件用时
        /// </summary>
        public double WriteFileTimeSpanMs { get; set; }

        /// <summary>
        /// 最终文件访问路径
        /// </summary>
        public string FileInternalAccessUrl { get; set; }

        /// <summary>
        /// APP目录
        /// </summary>
        public string AppDirectory { get; set; }

        /// <summary>
        /// 去掉域名的路径名称
        /// </summary>
        public string SubPathFileName { get; set; }
        public string Msg { get; set; }
    }
}
