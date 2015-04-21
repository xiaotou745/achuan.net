namespace AC.Tools.Service.HtmlHelper
{
    public class TableRowItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// <td>Data</td>
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// td class属性
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Rowspan { get; set; }
        public string Colspan { get; set; }
        public string Display { get; set; }
        public string UseCode { get; set; }
        public string UseDES { get; set; }
        /// <summary>
        /// a标签
        /// </summary>
        public string AsLink { get; set; }
        /// <summary>
        /// a标签href属性
        /// </summary>
        public string LinkHref { get; set; }
        public bool UsePopover { get; set; }
        public string PopoverTitle { get; set; }
        public string PopoverContent { get; set; }
        public string PopoverOptions { get; set; }
        public string Properties { get; set; }
    }
}