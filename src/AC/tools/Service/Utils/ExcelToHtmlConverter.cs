using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.HSSF.Converter;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace AC.Tools.Service.Utils
{
    /// <summary>
    /// Excel转换成Bootstrap格式表格
    /// </summary>
    public class ExcelToHtmlConverter
    {
        /// <summary>
        /// html工具对象
        /// </summary>
        private readonly HtmlUtils htmlUtils;

        #region 构造函数

        public ExcelToHtmlConverter()
        {
            var xmlDocument = new XmlDocument();
            htmlUtils = new HtmlUtils(xmlDocument);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 创建thead元素，包含thead元素内的标题行,默认sheet中第一行内容为标题行
        /// </summary>
        /// <param name="headerRow">标题行内容</param>
        /// <returns>
        /// 返回如下所示html:
        /// <thead>
        ///     <tr>
        ///         <th>001</th>
        ///         <th>002</th>
        ///         <th>003</th>
        ///     </tr>
        /// </thead>
        /// </returns>
        protected XmlElement ProcessThead(IRow headerRow)
        {
            XmlElement thead = htmlUtils.CreateTableHeader();
            XmlElement tr = htmlUtils.CreateTableRow();
            for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
            {
                XmlElement th = htmlUtils.CreateTableHeaderCell();
                th.AppendChild(htmlUtils.CreateText(headerRow.GetCell(i).StringCellValue));
                tr.AppendChild(th);
            }
            thead.AppendChild(tr);
            return thead;
        }

        /// <summary>
        /// 创建tbody元素，包含tbody元素中的表格内容
        /// </summary>
        /// <param name="sheet">sheet内容</param>
        /// <returns>
        /// 返回如下所示的html:
        /// <tbody>
        ///     <tr>
        ///         <td>001</td>
        ///         <td>002</td>
        ///         <td>003</td>
        ///     </tr>
        /// </tbody>
        /// </returns>
        protected XmlElement ProcessTbody(ISheet sheet)
        {
            CellRangeAddress[][] mergedRanges = ExcelToHtmlUtils.BuildMergedRangesMap(sheet as HSSFSheet);

            XmlElement tbody = htmlUtils.CreateTableBody();

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.LastCellNum <= 0)
                {
                    continue;
                }

                XmlElement tr = htmlUtils.CreateTableRow();
                for (int j = row.FirstCellNum; j < (int) row.LastCellNum; j++)
                {
                    short color = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).Color;
                    bool isStrikeout = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).IsStrikeout;
                    short boldweight = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).Boldweight;
                    CellRangeAddress range = ExcelToHtmlUtils.GetMergedRange(mergedRanges, row.RowNum, j);
                    if (range != null && (range.FirstColumn != j || range.FirstRow != row.RowNum))
                    {
                        continue;
                    }

                    XmlElement td = htmlUtils.CreateTableCell();
                    if (range != null)
                    {
                        if (range.FirstColumn != range.LastColumn)
                            td.SetAttribute("colspan", (range.LastColumn - range.FirstColumn + 1).ToString());
                        if (range.FirstRow != range.LastRow)
                            td.SetAttribute("rowspan", (range.LastRow - range.FirstRow + 1).ToString());
                    }
                    if (row.GetCell(j) == null || string.IsNullOrEmpty(row.GetCell(j).ToString()))
                    {
                        td.AppendChild(htmlUtils.CreateText(string.Empty));
                    }
                    else
                    {
                        if (color == 10) //红色
                        {
                            td.AppendChild(htmlUtils.CreateCode());
                        }
                        if (isStrikeout) //删除线
                        {
                            td.AppendChild(htmlUtils.CreateStrikeout());
                        }
                        if (boldweight == 700) //粗体
                        {
                            td.AppendChild(htmlUtils.CreateStrong());
                        }
                        td.AppendChild(htmlUtils.CreateText(row.GetCell(j).ToString()));
                    }

                    tr.AppendChild(td);
                }
                tbody.AppendChild(tr);
            }
            return tbody;
        }

        /// <summary>
        /// 获取一个excel中的sheet对应的bootstrap格式的html表格文本，默认启用间隔背景色
        /// </summary>
        /// <param name="useSection">是否在表格上显示章节</param>
        /// <param name="sheet">excel的一个sheet</param>
        /// <returns>返回sheet对应的bootstrap格式的html表格文本</returns>
        protected string GetHtmlOfSheet(bool useSection, ISheet sheet)
        {
            return GetHtmlOfSheet(useSection, sheet, true);
        }

        /// <summary>
        /// 获取一个excel中的sheet对应的bootstrap格式的html表格文本
        /// </summary>
        /// <param name="useSection">是否在表格上显示章节</param>
        /// <param name="sheet">excel的一个sheet</param>
        /// <param name="useStripted">是否在表格上启用间隔背景色（奇数行为浅灰色）</param>
        /// <returns>返回sheet对应的bootstrap格式的html表格文本</returns>
        protected string GetHtmlOfSheet(bool useSection, ISheet sheet, bool useStripted)
        {
            //创建一个div元素
            XmlElement root = htmlUtils.CreateBlock();

            /* 下面代码为创建一个section片段，最终html如下：
             * <div id="sheetname">
             *      <div class="page-header">
             *          <h2>sheetName</h2>
             *      </div>
             * </div>*/
            //创建一个section元素，并将sheetName指定为此section的id属性值
            XmlElement section = ProcessSection(sheet.SheetName);
            //创建一段代码
            XmlElement pageHeader = ProcessPageHeader(sheet.SheetName);
            section.AppendChild(pageHeader);

            //创建table元素
            XmlElement table = ProcessTable(useStripted);
            //创建thead元素，包括标题行
            XmlElement thead = ProcessThead(sheet.GetRow(0));
            //创建tbody元素，包括表格内容
            XmlElement tbody = ProcessTbody(sheet);

            table.AppendChild(thead);
            table.AppendChild(tbody);

            if (useSection) //如果需要使用章节，则把表格html追加到section内
            {
                section.AppendChild(table);
                root.AppendChild(section);
            }
            else //否则，表格html追加到div后
            {
                root.AppendChild(table);
            }
            return root.InnerXml;
        }

        /// <summary>
        /// 创建表格的section元素
        /// </summary>
        /// <param name="sectionId">指定的sectionId</param>
        /// <returns>
        /// 返回如下所示html代码
        /// &lt;section id=""&gt;&lt;/section&gt;
        /// </returns>
        protected XmlElement ProcessSection(string sectionId)
        {
            XmlElement section = htmlUtils.CreateSection();
            section.SetAttribute("id", sectionId);
            return section;
        }

        /// <summary>
        /// 创建一个pageheader元素段落，用来显示一个section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns>
        /// <div class="page-header">
        ///     <h2>sectionName</h2>
        /// </div>
        /// </returns>
        protected XmlElement ProcessPageHeader(string sectionName)
        {
            XmlElement pageHeader = htmlUtils.CreateBlock();
            pageHeader.SetAttribute("class", "page-header");
            XmlElement header2 = htmlUtils.CreateHeader2();
            header2.AppendChild(htmlUtils.CreateText(sectionName));
            pageHeader.AppendChild(header2);
            return pageHeader;
        }

        /// <summary>
        /// 创建table元素
        /// </summary>
        /// <param name="useStriped">是否启用table-striped属性</param>
        /// <returns>返回<table class=""></table>表示的html</returns>
        protected XmlElement ProcessTable(bool useStriped)
        {
            string tableStriped = useStriped ? " table-striped" : string.Empty;
            XmlElement table = htmlUtils.CreateTable();
            table.SetAttribute("class", string.Format("table table-bordered table-condensed{0}", tableStriped));
            return table;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取Excel第一个sheet转换成bootstrap格式表格之后的html table表格
        /// </summary>
        /// <param name="excelFilePath">excel文件路径</param>
        /// <param name="useSection">是否在表格上显示章节</param>
        /// <returns>返回以bootstrap格式表示的html字符串</returns>
        public string GetFirstSheetHtml(string excelFilePath, bool useSection)
        {
            if (!File.Exists(excelFilePath))
            {
                throw new FileNotFoundException("file not found:" + excelFilePath);
            }

            using (var fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);

                ISheet sheet = workbook.GetSheetAt(0);

                return GetHtmlOfSheet(useSection, sheet);
            }
        }

        /// <summary>
        /// 获取excel所有sheet转换成bootstrap格式表格之后和html文本
        /// </summary>
        /// <param name="excelFilePath">excel文件路径</param>
        /// <param name="useSection">是否在表格上显示章节</param>
        /// <returns>返回一个数据字典，表示sheetname，html表示的字典结果</returns>
        public Dictionary<string, string> GetAllSheetHtml(string excelFilePath, bool useSection)
        {
            return GetAllSheetHtml(excelFilePath, useSection, true);
        }

        /// <summary>
        /// 获取excel所有sheet转换成bootstrap格式表格之后和html文本
        /// </summary>
        /// <param name="excelFilePath">excel文件路径</param>
        /// <param name="useSection">是否在表格上显示章节</param>
        /// <param name="useStriped">是否在表格上启用间隔背景色（奇数行为浅灰色）</param>
        /// <returns>返回一个数据字典，表示sheetname，html表示的字典结果</returns>
        public Dictionary<string, string> GetAllSheetHtml(string excelFilePath, bool useSection, bool useStriped)
        {
            if (!File.Exists(excelFilePath))
            {
                throw new FileNotFoundException("file not found:" + excelFilePath);
            }
            var result = new Dictionary<string, string>();
            using (var fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    string htmlOfSheet = GetHtmlOfSheet(true, sheet, useStriped);
                    result.Add(sheet.SheetName, htmlOfSheet);
                }
            }
            return result;
        }

        #endregion
    }
}