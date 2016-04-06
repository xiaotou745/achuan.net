using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderMyBatisMapper
    {
        #region 公有属性
        IDbObject DbObject
        {
            set;
            get;
        }
        /// <summary>
        /// 库名
        /// </summary>
        string DbName
        {
            set;
            get;
        }
        /// <summary>
        /// 表名
        /// </summary>
        string TableName
        {
            set;
            get;
        }
        /// <summary>
        /// 生成配置信息
        /// </summary>
        CodeGenerateConfig GenerateConfig { get; set; }
        /// <summary>
        /// 选择的字段集合
        /// </summary>
        List<ColumnInfo> Fieldlist
        {
            set;
            get;
        }
        /// <summary>
        /// 主键或条件字段列表
        /// </summary>
        List<ColumnInfo> Keys
        {
            set;
            get;
        }

        #endregion

        string GetXml();
    }
}