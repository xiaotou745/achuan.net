using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderServiceImpl
    {
        #region 公有属性

        /// <summary>
        /// 选择的字段集合
        /// </summary>
        List<ColumnInfo> Fieldlist { set; get; }

        /// <summary>
        /// 主键或条件字段列表 
        /// </summary>
        List<ColumnInfo> Keys { set; get; }

        CodeGenerateConfig GenerateConfig { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DbType DbType { set; get; }

        #endregion

        string GetServiceImplCode();

        /// <summary>
        /// 获取Create Code
        /// </summary>
        /// <returns></returns>
        string GenerateCreateCode();

        /// <summary>
        /// 获取Modify Code
        /// </summary>
        /// <returns></returns>
        string GenerateModifyCode();

        /// <summary>
        /// 获取Remove Code
        /// </summary>
        /// <returns></returns>
        string GenerateRemoveCode();

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