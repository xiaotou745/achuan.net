using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderService
    {
        #region 公有属性
        
        /// <summary>
        /// 主键或条件字段列表 
        /// </summary>
        List<ColumnInfo> Keys { set; get; }

        /// <summary>
        /// 命名管理类
        /// </summary>
        CodeGenerateConfig GenerateConfig { get; set; }

        DbType DbType { get; set; }
        #endregion

        string GetServiceCode();

        string GenerateCreateCode();
        string GenerateModifyCode();
        string GenerateRemoveCode();
        string GenerateQueryCode();
        string GenerateGetByIdCode();

    }
}