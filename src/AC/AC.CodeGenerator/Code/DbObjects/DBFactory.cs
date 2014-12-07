using System;

namespace AC.Code.DbObjects
{
    /// <summary>
    /// 数据库信息类实例工厂,利用反射动态创建对象。
    /// </summary>
    public class DbFactory
    {
        private static readonly Cache Cache = new Cache();

        #region 同一程序集内反射

        public static IDbObject CreateDbObj(DbType dbType)
        {
            object dbObj = GetDbObj(dbType);

            return (IDbObject) dbObj;
        }

        public static IDbObject CreateDbObj(DbType dbType, string connectionString)
        {
            object dbObj = GetDbObj(dbType);
            var dbObject = dbObj as IDbObject;
            if (dbObject != null)
            {
                dbObject.DbConnectStr = connectionString;
            }
            return (IDbObject) dbObj;
        }

        public static IDbObject CreateDbObj(DbType dbType, bool sspi, string ip, string user, string pwd)
        {
            object dbObj = GetDbObj(dbType);
            var dbObject = dbObj as IDbObject;
            if (dbObject != null)
            {
                dbObject.ReSetConnString(sspi, ip, user, pwd);
            }
            return (IDbObject) dbObj;
        }

        private static object GetDbObj(DbType dbType)
        {
            object dbObj = Cache.GetObject(dbType);
            if (dbObj == null)
            {
                switch (dbType)
                {
                    case DbType.SQL2005:
                        dbObj = new DbObject();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("dbType");
                }
                Cache.SaveCache(dbType, dbObj); // 写入缓存
            }
            return dbObj;
        }

        #endregion
    }
}