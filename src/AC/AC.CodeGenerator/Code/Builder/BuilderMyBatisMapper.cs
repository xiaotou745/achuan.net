using System.Collections.Generic;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;
using AC.Code.JavaBuilder;

namespace AC.Code.Builder
{
    public class MyBatisMapperBuilder
    {
        private List<ColumnInfo> colFields;
        private string dbName;
        private IDbObject dbObject;
        private CodeGenerateConfig generateConfig;
        private List<ColumnInfo> keys;
        private string tableName;
        
        #region Create

        private MyBatisMapperBuilder()
        {
        }

        public static MyBatisMapperBuilder Create()
        {
            var serviceBuilder = new MyBatisMapperBuilder();
            return serviceBuilder;
        }
        #endregion

        #region Set

        public MyBatisMapperBuilder SetGenerateConfig(CodeGenerateConfig codeGenerateConfig)
        {
            generateConfig = codeGenerateConfig;
            return this;
        }

        public MyBatisMapperBuilder SetDbObj(IDbObject idbObj)
        {
            dbObject = idbObj;
            return this;
        }

        public MyBatisMapperBuilder SetDbName(string dbname)
        {
            dbName = dbname;
            return this;
        }

        public MyBatisMapperBuilder SetTableName(string tablename)
        {
            tableName = tablename;
            return this;
        }

        public MyBatisMapperBuilder SetColFields(List<ColumnInfo> fields)
        {
            colFields = fields;
            return this;
        }

        public MyBatisMapperBuilder SetKeys(List<ColumnInfo> colKeys)
        {
            keys = colKeys;
            return this;
        }

        #endregion

        #region Get

        public IBuilderMyBatisMapper GetBulider()
        {
            if (generateConfig == null)
            {
                return null;
            }
            return new BuilderMyBatisMapper(dbObject, dbName, tableName, colFields, keys, generateConfig);
        }

        #endregion
    }
}