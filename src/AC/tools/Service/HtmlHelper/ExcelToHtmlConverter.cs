using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AC.Tools.Service.HtmlHelper
{
    /// <summary>
    /// Excel转换成Bootstrap格式表格
    /// </summary>
    public class ExcelToHtmlConverter
    {
        private readonly HtmlUtils htmlUtils;

        public ExcelToHtmlConverter()
        {
            var xmlDocument = new XmlDocument();
            htmlUtils = new HtmlUtils(xmlDocument);
        }

        #region Protected Methods

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
                if(row.LastCellNum<=0)
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
                    if(row.GetCell(j) == null || string.IsNullOrEmpty(row.GetCell(j).ToString()))
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
                        if(boldweight == 700) //粗体
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

        protected string GetHtmlOfSheet(bool useSection, ISheet sheet)
        {
            return GetHtmlOfSheet(useSection, sheet, true);
        }

        protected string GetHtmlOfSheet(bool useSection, ISheet sheet, bool useStripted)
        {
            XmlElement root = htmlUtils.CreateBlock();
            XmlElement section = ProcessSection(sheet.SheetName);
            XmlElement pageHeader = ProcessPageHeader(sheet.SheetName);
            section.AppendChild(pageHeader);
            
            XmlElement table = ProcessTable(useStripted);
            XmlElement thead = ProcessThead(sheet.GetRow(0));
            XmlElement tbody = ProcessTbody(sheet);

            table.AppendChild(thead);
            table.AppendChild(tbody);

            if (useSection)
            {
                section.AppendChild(table);
                root.AppendChild(section);
            }
            else
            {
                root.AppendChild(table);
            }
            return root.InnerXml;
        }

        protected XmlElement ProcessSection(string sectionId)
        {
            XmlElement section = htmlUtils.CreateSection();
            section.SetAttribute("id", sectionId);
            return section;
        }

        protected XmlElement ProcessPageHeader(string sectionName)
        {
            XmlElement pageHeader = htmlUtils.CreateBlock();
            pageHeader.SetAttribute("class", "page-header");
            XmlElement header2 = htmlUtils.CreateHeader2();
            header2.AppendChild(htmlUtils.CreateText(sectionName));
            pageHeader.AppendChild(header2);
            return pageHeader;
        }

        protected XmlElement ProcessTable(bool useStriped)
        {
            string tableStriped = useStriped ? " table-striped" : string.Empty;
            XmlElement table = htmlUtils.CreateTable();
            table.SetAttribute("class", string.Format("table table-bordered table-condensed{0}", tableStriped));
            return table;
        }

        #endregion

        #region Public Methods

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

        public Dictionary<string, string> GetAllSheetHtml(string excelFilePath, bool useSection)
        {
            return GetAllSheetHtml(excelFilePath, useSection, true);
        }

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