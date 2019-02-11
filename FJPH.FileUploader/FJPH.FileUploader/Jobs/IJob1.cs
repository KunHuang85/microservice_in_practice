using System.Threading.Tasks;

namespace FJPH.FileUploader.Jobs
{
    /// <summary>
    /// 一个任务一个接口，避免名称一样，
    /// 定义一个Tastk返回方法
    /// </summary>
    public interface IJob1
    {
        Task DoTask(string s);
    }
}