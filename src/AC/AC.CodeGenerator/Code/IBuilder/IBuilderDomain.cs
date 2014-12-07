using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderDomain
    {
        #region 公有属性


        /// <summary>
        /// 主键或条件字段列表 
        /// </summary>
        List<ColumnInfo> Keys { set; get; }

        /// <summary>
        /// 配置类
        /// </summary>
        CodeGenerateConfig GenerateConfig { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DbType DbType { set; get; }

        #endregion

        string GetDomainCode();

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