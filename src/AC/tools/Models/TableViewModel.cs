using System.Collections.Generic;

namespace AC.Tools.Models
{
    public class TableViewModel
    {
        /// <summary>
        ///     数据库服务器名称
        /// </summary>
        public IList<string> DbServerNames { get; set; }

        /// <summary>
        ///     当前选中的服务器
        /// </summary>
        public string DbServerSelected { get; set; }

        /// <summary>
        ///     当前选中的数据库名称
        /// </summary>
        public string DbNameSelected { get; set; }

        /// <summary>
        ///     当前选中的服务器所有的数据库名序列化字符串
        /// </summary>
        public string SerializedDbNames { get; set; }

        /// <summary>
        ///     当前选中的数据库中所有的表名序列化json字符串
        /// </summary>
        public string SerializedTableNames { get; set; }
    }
}