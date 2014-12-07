using System.Collections.Generic;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Code.IBuilder
{
    public interface IBuilderSQL
    {
        #region 公有属性

        IDbObject DbObject { set; get; }

        /// <summary>
        /// 库名
        /// </summary>
        string DbName { set; get; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { set; get; }

        /// <summary>
        /// 选择的字段集合
        /// </summary>
        List<ColumnInfo> Fieldlist { set; get; }

        #endregion

        string CreateSQL();
    }
}