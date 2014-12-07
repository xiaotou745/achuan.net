namespace AC.Code.Helper
{
    /// <summary>
    /// 表的详细信息
    /// </summary>
    public class TableInfo
    {
        private string _tabDate = "";
        private string _tabName = "";
        private string _tabType = "";
        private string _tabUser = "";

        /// <summary>
        /// 表名
        /// </summary>
        public string TabName
        {
            set { _tabName = value; }
            get { return _tabName; }
        }

        public string TabUser
        {
            set { _tabUser = value; }
            get { return _tabUser; }
        }

        public string TabType
        {
            set { _tabType = value; }
            get { return _tabType; }
        }

        public string TabDate
        {
            set { _tabDate = value; }
            get { return _tabDate; }
        }
    }
}