using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using AC.Security;
using NPOI.SS.UserModel;

namespace AC.Tools.Service.HtmlHelper
{
    public class HtmlTableBase
    {
        protected string TableStart(bool useStriped)
        {
            string tableStriped = useStriped ? " table-striped" : string.Empty;
            return string.Format("<table class='table table-bordered table-condensed{0}'>", tableStriped);
        }

        protected string TableOver()
        {
            return "</table>";
        }

        protected string GetPageHeader(string sectionName)
        {
            return @"
        <div class='page-header'>
			<h2>" + sectionName + @"</h2>
		</div>";
        }

        protected string SectionStart(string sectionId)
        {
            return @"<section id='" + sectionId + @"'>";
        }

        protected string SectionOver()
        {
            return "</section>";
        }

        private static string GetTbody(CommonTableInfo table)
        {
            var result = new StringBuilder();
            result.AppendLine("<tbody>");
            foreach (TableRow dataRow in table.DataRows)
            {
                result.AppendLine("<tr>");
                foreach (TableRowItem rowItem in dataRow)
                {
                    //如果Display属性为false，则不显示
                    if (!string.IsNullOrEmpty(rowItem.Display) &&
                        rowItem.Display.ToLower() == false.ToString().ToLower())
                    {
                        continue;
                    }

                    //添加class属性
                    string properties = string.Empty;
                    if (!string.IsNullOrEmpty(rowItem.Class))
                    {
                        properties = " class='" + rowItem.Class + "'";
                    }
                    //添加Rowspan到td
                    if (!string.IsNullOrEmpty(rowItem.Rowspan))
                    {
                        properties += " rowspan='" + rowItem.Rowspan + "'";
                    }
                    //添加colspan到td
                    if (!string.IsNullOrEmpty(rowItem.Colspan))
                    {
                        properties += " colspan='" + rowItem.Colspan + "'";
                    }
                    //是否使用了加密
                    if (!string.IsNullOrEmpty(rowItem.UseDES) && bool.Parse(rowItem.UseDES))
                    {
                        rowItem.Data = DES.Decrypt3DES(rowItem.Data);
                    }
                    if (!string.IsNullOrEmpty(rowItem.AsLink) && bool.Parse(rowItem.AsLink))
                    {
                        rowItem.Data = "<a href='" + rowItem.Data + "' target='_blank'>" + rowItem.Data + "</a>";
                    }
                    //是否使用<code>
                    if (!string.IsNullOrEmpty(rowItem.UseCode) && bool.Parse(rowItem.UseCode))
                    {
                        rowItem.Data = "<code>" + rowItem.Data + "</code>";
                    }

                    if (rowItem.UsePopover)
                    {
                        rowItem.Data = "<a href='#' rel='popover' " + rowItem.PopoverOptions +
                                       " data-content='" + rowItem.Data + "'>详细信息</a>";
                    }
                    result.AppendLine("<td" + properties + " " + rowItem.Properties + ">" + rowItem.Data + "</td>");
                }
                result.AppendLine("</tr>");
            }
            result.AppendLine(@"</tbody>");
            return result.ToString();
        }

        private static string GetTbody(DataTable table)
        {
            var result = new StringBuilder();
            result.AppendLine("<tbody>");
            foreach (DataRow dataRow in table.Rows)
            {
                result.AppendLine("<tr>");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.AppendLine("<td>" + dataRow[i] + "</td>");
                }
                result.AppendLine("</tr>");
            }

            result.AppendLine(@"</tbody>");
            return result.ToString();
        }
    }

    public class HtmlUtilsOfExcel : HtmlTableBase
    {
        private HtmlUtilsOfExcel()
        {
        }

        public static HtmlUtilsOfExcel Create()
        {
            var htmlUtilsOfExcel = new HtmlUtilsOfExcel();
            return htmlUtilsOfExcel;
        }

        private string GetThead(IRow headerRow)
        {
            var result = new StringBuilder();
            result.AppendLine(@"
            <thead>
				<tr>");
            int cellCount = headerRow.LastCellNum; //LastCellNum = PhysicalNumberOfCells

            //handling header.
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                result.AppendLine(@"<th>" + headerRow.GetCell(i).StringCellValue + "</th>");
            }

            result.AppendLine(@"
				</tr>
			</thead>");

            return result.ToString();
        }

        private string GetTbody(ISheet sheet)
        {
            IRow headerRow = sheet.GetRow(0); //第一行为标题行
            int cellCount = headerRow.LastCellNum; //LastCellNum = PhysicalNumberOfCells
            int rowCount = sheet.LastRowNum; //LastRowNum = PhysicalNumberOfRows - 1

            var result = new StringBuilder();
            result.AppendLine("<tbody>");
            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);

                if (row != null)
                {
                    result.AppendLine("<tr>");
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        short color = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).Color;
                        bool isStrikeout = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).IsStrikeout;

                        string cellValue = "{0}";
                        if (color == 10 && isStrikeout)
                        {
                            cellValue = "<code><s>{0}</s></code>";
                        }
                        else if (color == 10)
                        {
                            cellValue = "<code>{0}</code>";
                        }
                        else if (isStrikeout)
                        {
                            cellValue = "<s>{0}</s>";
                        }

                        if (row.GetCell(j) != null)
                        {
                            result.AppendLine("<td>" + string.Format(cellValue, row.GetCell(j)) + "</td>");
                        }
                        else
                        {
                            result.AppendLine("<td>" + string.Format(cellValue, string.Empty) + "</td>");
                        }
                    }
                    result.AppendLine("</tr>");
                }
            }

            result.AppendLine(@"</tbody>");
            return result.ToString();
        }

        public Dictionary<string, string> GetTableHtmls(string excelFilePath, bool useSection)
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
                    string htmlOfSheet = GetHtmlOfSheet(true, sheet);
                    result.Add(sheet.SheetName, htmlOfSheet);
                }
            }
            return result;
        }

        public string GetTableHtmlOfFirstSheet(string excelFilePath, bool useSection)
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

        private string GetHtmlOfSheet(bool useSection, ISheet sheet)
        {
            var result = new StringBuilder();
            if (useSection)
            {
                result.AppendLine(SectionStart(sheet.SheetName));
                result.AppendLine(GetPageHeader(sheet.SheetName));
            }
            result.AppendLine(TableStart(true));

            IRow headerRow = sheet.GetRow(0); //第一行为标题行
            result.AppendLine(GetThead(headerRow));

            result.AppendLine(GetTbody(sheet));
            result.AppendLine(TableOver());

            if (useSection)
            {
                result.AppendLine(SectionOver());
            }
            return result.ToString();
        }
    }

    public class TableHtmlHelper
    {
        #region Private

        private static string GetThead(CommonTableInfo table)
        {
            var result = new StringBuilder();

            result.AppendLine(@"
            <thead>
				<tr>");
            foreach (string colName in table.ColumnNames)
            {
                result.AppendLine(@"<th>" + colName + "</th>");
            }
            result.AppendLine(@"				
				</tr>
			</thead>");

            return result.ToString();
        }

        private static string GetThead(DataTable table)
        {
            var result = new StringBuilder();

            result.AppendLine(@"
            <thead>
				<tr>");
            foreach (DataColumn column in table.Columns)
            {
                result.AppendLine(@"<th>" + column.ColumnName + "</th>");
            }
            result.AppendLine(@"				
				</tr>
			</thead>");

            return result.ToString();
        }

        private static string TableStart(bool useStriped)
        {
            string tableStriped = useStriped ? " table-striped" : string.Empty;
            return string.Format("<table class='table table-bordered table-condensed{0}'>", tableStriped);
        }

        private static string TableOver()
        {
            return "</table>";
        }

        private static string GetPageHeader(string sectionName)
        {
            return @"
        <div class='page-header'>
			<h2>" + sectionName + @"</h2>
		</div>";
        }

        private static string SectionStart(string sectionId)
        {
            return @"<section id='" + sectionId + @"'>";
        }

        private static string SectionOver()
        {
            return "</section>";
        }

        private static string GetTbody(CommonTableInfo table)
        {
            var result = new StringBuilder();
            result.AppendLine("<tbody>");
            foreach (TableRow dataRow in table.DataRows)
            {
                result.AppendLine("<tr>");
                foreach (TableRowItem rowItem in dataRow)
                {
                    //如果Display属性为false，则不显示
                    if (!string.IsNullOrEmpty(rowItem.Display) &&
                        rowItem.Display.ToLower() == false.ToString().ToLower())
                    {
                        continue;
                    }

                    //添加class属性
                    string properties = string.Empty;
                    if (!string.IsNullOrEmpty(rowItem.Name))
                    {
                        properties += " name='" + rowItem.Name + "'";
                    }
                    if (!string.IsNullOrEmpty(rowItem.Class))
                    {
                        properties += " class='" + rowItem.Class + "'";
                    }
                    //添加Rowspan到td
                    if (!string.IsNullOrEmpty(rowItem.Rowspan))
                    {
                        properties += " rowspan='" + rowItem.Rowspan + "'";
                    }
                    //添加colspan到td
                    if (!string.IsNullOrEmpty(rowItem.Colspan))
                    {
                        properties += " colspan='" + rowItem.Colspan + "'";
                    }
                    //是否使用了加密
                    if (!string.IsNullOrEmpty(rowItem.UseDES) && bool.Parse(rowItem.UseDES))
                    {
                        rowItem.Data = DES.Decrypt3DES(rowItem.Data);
                    }
                    if (!string.IsNullOrEmpty(rowItem.AsLink) && bool.Parse(rowItem.AsLink))
                    {
                        var href = string.IsNullOrEmpty(rowItem.LinkHref) ? rowItem.Data : rowItem.LinkHref;
                        rowItem.Data = "<a href='" + href + "' target='_blank'>" + rowItem.Data + "</a>";
                    }
                    //是否使用<code>
                    if (!string.IsNullOrEmpty(rowItem.UseCode) && bool.Parse(rowItem.UseCode))
                    {
                        rowItem.Data = "<code>" + rowItem.Data + "</code>";
                    }

                    if (rowItem.UsePopover)
                    {
                        rowItem.Data = "<a href='#' rel='popover' " + rowItem.PopoverOptions +
                                       " data-content='" + rowItem.Data + "'>详细信息</a>";
                    }
                    result.AppendLine("<td" + properties + " " + rowItem.Properties + ">" + rowItem.Data + "</td>");
                }
                result.AppendLine("</tr>");
            }
            result.AppendLine(@"</tbody>");
            return result.ToString();
        }

        private static string GetTbody(DataTable table)
        {
            var result = new StringBuilder();
            result.AppendLine("<tbody>");
            foreach (DataRow dataRow in table.Rows)
            {
                result.AppendLine("<tr>");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.AppendLine("<td>" + dataRow[i] + "</td>");
                }
                result.AppendLine("</tr>");
            }

            result.AppendLine(@"</tbody>");
            return result.ToString();
        }

        #endregion

        public static string GetHtml(string xmlFileName)
        {
            IList<CommonTableInfo> commonTableInfos = CommonTableHelper.GetCommonTableInfo(xmlFileName);

            return GetHtml(commonTableInfos);
        }

        public static string GetHtml(IList<CommonTableInfo> lstTables)
        {
            var result = new StringBuilder();
            foreach (CommonTableInfo commonTableInfo in lstTables)
            {
                result.AppendLine(GetHtml(commonTableInfo));
            }
            return result.ToString();
        }

        public static string GetHtml(CommonTableInfo table)
        {
            var result = new StringBuilder();
            if (table.UseSection)
            {
                result.AppendLine(SectionStart(table.SectionId));
                result.AppendLine(GetPageHeader(table.SectionName));
            }
            result.AppendLine(TableStart(table.UseStripped));
            result.AppendLine(GetThead(table));
            result.AppendLine(GetTbody(table));
            result.AppendLine(TableOver());
            if (table.UseSection)
            {
                result.AppendLine(SectionOver());
            }
            return result.ToString();
        }

        public static string GetHtml(DataTable dataTable, bool useSection)
        {
            var result = new StringBuilder();
            if (useSection)
            {
                result.AppendLine(SectionStart(dataTable.TableName));
                result.AppendLine(GetPageHeader(dataTable.TableName));
            }

            result.AppendLine(TableStart(true));
            result.AppendLine(GetThead(dataTable));
            result.AppendLine(GetTbody(dataTable));
            result.AppendLine(TableOver());

            if (useSection)
            {
                result.AppendLine(SectionOver());
            }
            return result.ToString();
        }
    }
}