using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderDao
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

        string GetDaoCode();

        /// <summary>
        /// 获取Create Code
        /// </summary>
        /// <returns></returns>
        string GenerateInsertCode();

        /// <summary>
        /// 获取Modify Code
        /// </summary>
        /// <returns></returns>
        string GenerateUpdateCode();

        /// <summary>
        /// 获取Remove Code
        /// </summary>
        /// <returns></returns>
        string GenerateDeleteCode();

        /// <summary>
        /// 获取Query Code
        /// </summary>
        /// <returns></returns>
        string GenerateQueryCode();

        /// <summary>
        /// 获取GetById Code
        /// </summary>
        /// <returns></returns>
        string GenerateGetByIdCode();
    }
}