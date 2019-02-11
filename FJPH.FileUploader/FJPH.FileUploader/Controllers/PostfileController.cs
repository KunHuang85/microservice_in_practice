using FJPH.FileUploader.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// 获取请求参数
/// http://www.cnblogs.com/jhxk/articles/9951094.html
/// IFormCollection 或者 Iquery Collection 使用
/// </summary>

namespace FJPH.FileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostfileController : ControllerBase
    {
        //private readonly OracleDbModel oracleDbModel;
        private readonly StorageConfigModel _storageConfigModel;
        private readonly ILogger<PostfileController> _logger;


        public PostfileController(StorageConfigModel storageConfigModel, ILogger<PostfileController> logger)
        {
            this._storageConfigModel = storageConfigModel;
            _logger = logger;
        }



        /// <summary>
        /// 输出一些版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var info = new
            {
                version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                directory = "POST BODY参数 - 设定的目录名称",
                file = "POST BODY参数 - file : 文件",
                ContentType = "Header [Content-Type:multipart/form-data] ",
                method = "POST",
                appsetting = this._storageConfigModel.ToString(),
                existRoot = $"exis[{this._storageConfigModel.StoragePath}]=[{Directory.Exists(this._storageConfigModel.StoragePath)}]"
            };

            return Ok(info);
        }


        /// <summary>
        /// 单次上传的最大值为500M
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [RequestSizeLimit(500_000_000)]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, IFormCollection form)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            _logger.LogDebug($"Postfile id={id}");
            string domainUrl = String.Empty;

            #region  基础请求内容的校验

            string strSubPath = form["sub_dir_path"]; // 请求参数 -  子目录
            string strIsCreateSubPath = form["is_create_sub_dir"]; //是创建子目录
            string strFile_exist_handle = form["file_exist_handle_option"]; // 请求参数，遇到重名文件时候的处理


            // 应用存储目录=存储+分配
            string strAppMainPath = "";

            // 应用存储目录+子目录
            string strFullDirectoryPath = "";

            //  判断 bse url
            if (!this._storageConfigModel.BaseUrl.EndsWith("/"))
            {
                return new UnprocessableEntityObjectResult(new
                {
                    err_code = -101,
                    err_msg = $"基础域名 BaseUrl=[{this._storageConfigModel.BaseUrl}]配置不正确，名称需要以[/]结尾,请联系管理员"
                });
            }

            //  判断 必填 strFile_exist_handle
            if (string.IsNullOrEmpty(strFile_exist_handle))
            {
                return new BadRequestObjectResult(new
                {
                    err_code = -104,
                    err_msg = $"请求方的Post Form 必须包含属性[file_exist_handle_option],取值可选项有[Overwrite,Rename]"
                });
            }

            // 判断 是否存在上传文件以及文件大小
            if (form.Files.Count < 1 || form.Files[0].Length < 1)
            {
                return new BadRequestObjectResult(new
                {
                    err_code = -103,
                    err_msg = $"请求方的Post Form表单未设置文件对象，或者文件对象长度为0"
                });
            }


            //判断根目录是否存在，不存则配置错误
            if (!Directory.Exists(this._storageConfigModel.StoragePath))
            {
                return new UnprocessableEntityObjectResult(new
                {
                    err_code = -100,
                    err_msg = "根存储目录路径无法访问，根存储路径配置错误或没有权限，请联系管理员."
                });
            }

            //判断 应用目录 长度
            if (id.Length > 127)
            {
                return new BadRequestObjectResult(new
                {
                    err_code = -101,
                    err_msg = $"应用存储目录名称[{id}]不正确，目名不能以 / 开头结尾，不能使用除[-] [_] 之类特殊字符,最大长度128字符"
                });
            }


            // 判断应用的存储目录，是否真实存在
            strAppMainPath = Path.Combine(this._storageConfigModel.StoragePath, id);
            if (!Directory.Exists(strAppMainPath))
            {
                return new BadRequestObjectResult(new
                {
                    err_code = -102,
                    err_msg = $"应用存储目录路径{id}无法访问，可能未创建该目录或没权限，请联系管理员."
                });
            }


            _logger.LogDebug($"Base Url={_storageConfigModel.BaseUrl}");

            // id合成域名
            domainUrl = String.Format(_storageConfigModel.BaseUrl, id);

            _logger.LogDebug($"合成域名 Url={domainUrl}");

            #endregion

            #region 处理目录路径 - 子目录

            // 获取子目录
            strIsCreateSubPath = string.IsNullOrEmpty(strIsCreateSubPath) ? "false" : strIsCreateSubPath.ToLowerInvariant();

            // 当子目录为空时, 不使用
            if (string.IsNullOrEmpty(strSubPath))
            {
                strSubPath = "";
                strFullDirectoryPath = Path.Combine(strAppMainPath, strSubPath);
            }
            // 不为空的时候 
            else
            {
                // 判断格式
                if (strSubPath.StartsWith("/"))
                {
                    return new BadRequestObjectResult(new
                    {
                        err_code = -103,
                        err_msg = $"请求方的Post Form表单参数[sub_dir_path]=[{strSubPath}]设置错误,子目录路径不能以字符/开头"
                    });
                }

                // 判断 实际的存储目录
                strFullDirectoryPath = Path.Combine(strAppMainPath, strSubPath);

                // 目标目录不存在
                if (!Directory.Exists(strFullDirectoryPath))
                {
                    if (strIsCreateSubPath.Equals("true"))
                    {
                        Directory.CreateDirectory(strFullDirectoryPath);
                        Console.WriteLine($"创建目录{strFullDirectoryPath}");
                    }
                    else
                    {
                        return new BadRequestObjectResult(new
                        {
                            err_code = -104,
                            err_msg = $"请求方的Post Form 中的子目录[sub_dir_path]={strSubPath}不存在，如需创建，需指定is_create_sub_dir"
                        });
                    }
                }
            }// end 子目录不为空时的处理

            #endregion

            #region 文件处理

            Console.WriteLine(strSubPath);

            string strRawfilename = form.Files[0].FileName;

            if (strRawfilename.Length > 127)
            {
                return new BadRequestObjectResult(new
                {
                    err_code = -200,
                    err_msg = $"请求方的Post Form 的原始文件名不能超过128个字符"
                });
            }

            // 获取扩展名
            string strFileExtension = Path.GetExtension(strRawfilename);
            string strFileGuid = Guid.NewGuid().ToString();
            string strFileName = strFileGuid + strFileExtension;

            // 最终带有Guid文件名
            string strTargetFileName = Path.Combine(strFullDirectoryPath, strFileName);

            using (var stream = new FileStream(strTargetFileName, FileMode.CreateNew))
            {
                await form.Files[0].CopyToAsync(stream);
            }

            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;

            OperationResultModel operationResultModel = new OperationResultModel
            {
                WriteFileTimeSpanMs = ts2.TotalMilliseconds,
                AppDirectory = id,
                SubPathFileName = Path.Combine(strSubPath, strFileName),
                Msg = "写入新文件成功"
            };

            operationResultModel.FileInternalAccessUrl = $"{domainUrl}{operationResultModel.SubPathFileName}";
            _logger.LogDebug(operationResultModel.ToString());
            return Ok(operationResultModel);


            #endregion

        }// end Controller



        /// <summary>
        /// 文件删除?filename=nn/aaa ,第一个字符不包含/
        /// </summary>
        /// <param name="id">完整的文件名称</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            string strAppDirectory = id;
            string strQueryfilename = Request.Query["filename"].ToString();
            string strFullSubPath = Path.Combine(strAppDirectory, strQueryfilename);

            string strFullFilePath = Path.Combine(_storageConfigModel.StoragePath, strFullSubPath);

            if (System.IO.File.Exists(strFullFilePath))
            {
                // Console.WriteLine("exist");
                System.IO.File.Move(strFullFilePath, strFullFilePath + ".delete");
                _logger.LogInformation($"删除文件操作：文件：[{ strFullSubPath}] 存在，删除成功");
                return Ok(new { msg = $"删除文件操作：文件：[{ strFullSubPath}] 存在，删除成功" });
            }

            _logger.LogInformation($"删除文件操作：[{ strFullSubPath}]不存在");
            return Ok(new { msg = $"删除文件操作：[{ strFullSubPath}]不存在" });
        }



    }
}
