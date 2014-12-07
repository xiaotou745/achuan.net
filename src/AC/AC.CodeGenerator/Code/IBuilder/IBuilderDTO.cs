using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderDTO
    {
        #region 公有属性

        /// <summary>
        /// 主键或条件字段列表 
        /// </summary>
        List<ColumnInfo> Fieldlist { set; get; }

        /// <summary>
        /// 命名管理类
        /// </summary>
        CodeGenerateConfig GenerateConfig { get; set; }

        #endregion

        #region 生成完整单个Model类

        /// <summary>
        /// 生成完整单个Model类
        /// </summary>		
        string GetServiceDTOCode();

        #endregion

        #region 生成Model属性部分

        /// <summary>
        /// 生成实体类的属性
        /// </summary>
        /// <returns></returns>
        string GeneratePropertiesCode();

        #endregion
    }
}