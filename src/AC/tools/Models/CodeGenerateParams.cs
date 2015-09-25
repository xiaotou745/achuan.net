using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AC.Tools.Models
{
    public class CodeGenerateParams
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public string dbServer { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public string dbName { get; set; }

        /// <summary>
        /// 表
        /// </summary>
        public string tableName { get; set; }

        public string modelName { get; set; }
        public string callStyle { get; set; }
        public string daoStyle { get; set; }
        public string codeLayer { get; set; }
    }
}