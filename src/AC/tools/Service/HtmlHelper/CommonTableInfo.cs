using System.Collections.Generic;

namespace AC.Tools.Service.HtmlHelper
{
    public class CommonTableInfo
    {
        #region ¹¹Ôìº¯Êý

        public IList<TableRow> DataRows;

        public CommonTableInfo()
        {
            UseSection = false;
            DataRows = new List<TableRow>();
            ColumnNames = new ColumnNames();
        }

        #endregion

        #region Section

        public bool UseSection { get; set; }

        public string SectionId { get; set; }
        public string SectionName { get; set; }

        #endregion

        public bool UseStripped { get; set; }
        public ColumnNames ColumnNames { get; set; }

        public bool HasColumns
        {
            get { return ColumnNames.Count > 0; }
        }

        public void AddRow(TableRow tableRow)
        {
            DataRows.Add(tableRow);
        }
    }
}