using System.Data;
using System.IO;
using NPOI.SS.UserModel;

namespace AC.Tools.Service
{
    public class ExcelUtils
    {
        public static DataTable GetDatatableFromExcel(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(file);

                ISheet sheet = workbook.GetSheetAt(0);

                var table = new DataTable(sheet.SheetName);

                IRow headerRow = sheet.GetRow(0); //第一行为标题行
                int cellCount = headerRow.LastCellNum; //LastCellNum = PhysicalNumberOfCells
                int rowCount = sheet.LastRowNum; //LastRowNum = PhysicalNumberOfRows - 1

                //handling header.
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    
                    if (row != null)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                dataRow[j] = row.GetCell(j).ToString();
                            }
                            else
                            {
                                dataRow[j] = null;
                            }
                        }
                    }

                    table.Rows.Add(dataRow);
                }
                return table;
            }
        }
    }
}