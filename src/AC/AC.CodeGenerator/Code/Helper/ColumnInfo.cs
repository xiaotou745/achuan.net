namespace AC.Code.Helper
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public class ColumnInfo
    {
        private string _deText = "";
        private string _defaultVal = "";
        private string _length = "";
        private string _preci = "";
        private string _scale = "";
        private string _typeName = "";

        /// <summary>
        /// 序号
        /// </summary>
        public string Colorder { set; get; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { set; get; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string TypeName
        {
            set { _typeName = value; }
            get { return _typeName; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public string Length
        {
            set { _length = value; }
            get { return _length; }
        }

        /// <summary>
        /// 精度
        /// </summary>
        public string Preci
        {
            set { _preci = value; }
            get { return _preci; }
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        public string Scale
        {
            set { _scale = value; }
            get { return _scale; }
        }

        /// <summary>
        /// 是否是标识列
        /// </summary>
        public bool IsIdentity { set; get; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPK { set; get; }

        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool cisNull { set; get; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultVal
        {
            set { _defaultVal = value; }
            get { return _defaultVal; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string DeText
        {
            set { _deText = value; }
            get { return _deText; }
        }
    }
}