using System.Data;

namespace AC.Extension
{
    public static class DataRowExtension
    {
        /// <summary>
        /// 是否含有某列名
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
         public static bool HasColumn(this DataRow dataRow, string columnName)
         {
             if (dataRow == null)
             {
                 return false;
             }
             return dataRow.Table.Columns.Contains(columnName);
         }
    }
}