using System.Collections.Generic;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;

namespace AC.Code.Builder
{
    public class BuilderSQL : IBuilderSQL
    {
        #region IBuilderSQL Members

        public IDbObject DbObject { get; set; }
        public string DbName { get; set; }
        public string TableName { get; set; }
        public List<ColumnInfo> Fieldlist { get; set; }

        public string CreateSQL()
        {
            var sql = new StringPlus();
            sql.AppendLine("use " + TableName);
            sql.AppendLine("go");
            sql.AppendLine(string.Empty);

            var primaryKey = new StringPlus();

            sql.AppendLine("create table dbo." + TableName + " (");
            foreach (ColumnInfo columnInfo in Fieldlist)
            {
                if(columnInfo.IsPK)
                {
                    primaryKey.Append(columnInfo.ColumnName);
                }
                StringPlus field = new StringPlus();
                field.Append(columnInfo.ColumnName + " " + columnInfo.TypeName);
                if(!columnInfo.cisNull)
                {
                    field.Append(" not null");
                }

            }
            sql.AppendLine(")");
            return sql.ToString();
        }

        #endregion
    }
}