using System.IO;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;

namespace GameServers
{
    public class StartLog
    {
        public static ILoggerRepository Repository { get; set; }
        public IConfiguration Configuration { get; } //构造函数注入：Configuration用于读取配置文件的
        public StartLog(IConfiguration configuration)
        {
            Configuration = configuration;
 
            Repository = LogManager.CreateRepository("GameServer"); //我的项目名称叫NetCoreApp
            //指定配置文件
            XmlConfigurator.Configure(Repository, new FileInfo("log4net.config"));
        }


    }
}