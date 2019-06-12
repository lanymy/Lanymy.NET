using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheFirstBuildLanymyNetSolution.Models
{

    public class ConsoleArgumentModel
    {

        /// <summary>
        /// Lanymy.NET解决方案根文件夹全路径
        /// </summary>
        public string LanymyNetSolutionRootDirectoryFullPath { get; set; }

        /// <summary>
        /// 0关闭调试模式;1开启调试模式
        /// </summary>
        public bool IsDebugModel { get; set; }

    }

}
