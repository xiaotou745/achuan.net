using AC.Data.ConnString;
using AC.Transaction.Common;

namespace AC.Transaction
{
    public class DZUtilOfWorkFactory
    {
        public static IUnitOfWork GetUnitOfWorkOfEAdmin()
        {
            return UnitOfWorkFactory.GetAdoNetUnitOfWork(ConnStringUtil.GetConnectionString("dz_d_planEntities"));
        }

        public static IUnitOfWork GetUnitOfWorkOfBuy()
        {
            return UnitOfWorkFactory.GetAdoNetUnitOfWork(ConnStringUtil.GetConnectionString("ConnStringOfDZ"));
        }

        public static IUnitOfWork GetUnitOfWordOfDZ()
        {
            return UnitOfWorkFactory.GetAdoNetUnitOfWork(ConnStringUtil.GetConnectionString("dz"));
        }
        public static IUnitOfWork GetUnitOfWordOfDZNew()
        {
            return UnitOfWorkFactory.GetAdoNetUnitOfWork(ConnStringUtil.GetConnectionString("ConnStringOfDZ"));
        }
    }
}