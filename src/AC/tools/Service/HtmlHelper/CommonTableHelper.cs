using System.Collections.Generic;
using System.Xml;

namespace AC.Tools.Service.HtmlHelper
{
    public class CommonTableHelper
    {
        private const string COLUMN_NAMES = "ColumnNames";
        private const string COLUMN_DATAS = "ColumnDatas";

        public static IList<CommonTableInfo> GetCommonTableInfo(string filePath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            XmlNode root = xmlDocument.DocumentElement;

            if (root == null)
            {
                return null;
            }

            if (root.ChildNodes.Count == 0)
            {
                return null;
            }

            IList<CommonTableInfo> result = new List<CommonTableInfo>();

            foreach (XmlNode tableNode in root.ChildNodes)
            {
                result.Add(GetTableInfo(tableNode));
            }

            return result;
        }

        private static CommonTableInfo GetTableInfo(XmlNode root)
        {
            var commonTableInfo = new CommonTableInfo();
            XmlAttribute useSection = root.Attributes["UseSection"];
            if (useSection != null)
            {
                commonTableInfo.UseSection = bool.Parse(useSection.Value);
            }

            if (commonTableInfo.UseSection)
            {
                commonTableInfo.SectionId = root.Attributes["SectionId"].Value;
                commonTableInfo.SectionName = root.Attributes["SectionName"].Value;
            }
            commonTableInfo.UseStripped = root.Attributes["UseStripped"] == null ? false : bool.Parse(root.Attributes["UseStripped"].Value);
            if (root.HasChildNodes)
            {
                foreach (XmlNode childNode in root.ChildNodes)
                {
                    //如果是标题
                    if (childNode.Name != COLUMN_NAMES && childNode.Name != COLUMN_DATAS)
                    {
                        continue;
                    }

                    if (!childNode.HasChildNodes)
                    {
                        continue;
                    }

                    if (childNode.Name == COLUMN_NAMES)
                    {
                        commonTableInfo.ColumnNames = GetColumnNames(childNode);
                    }
                    if (childNode.Name == COLUMN_DATAS)
                    {
                        commonTableInfo.AddRow(GetRow(childNode, commonTableInfo.ColumnNames));
                    }
                }
            }
            return commonTableInfo;
        }

        private static ColumnNames GetColumnNames(XmlNode xmlNode)
        {
            var result = new ColumnNames();

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                result.Add(node.InnerText);
            }

            return result;
        }

        private static TableRow GetRow(XmlNode xmlNode, ColumnNames columnNames)
        {
            var result = new TableRow();

            int index = 0;
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                var rowItem = new TableRowItem();
                rowItem.Name = columnNames[index++];
                rowItem.Data = node.InnerText;
                rowItem.Class = node.Attributes["Class"] != null ? node.Attributes["Class"].Value : string.Empty;
                rowItem.Rowspan = node.Attributes["Rowspan"] != null ? node.Attributes["Rowspan"].Value : string.Empty;
                rowItem.Colspan = node.Attributes["Colspan"] != null ? node.Attributes["Colspan"].Value : string.Empty;
                rowItem.Display = node.Attributes["Display"] != null ? node.Attributes["Display"].Value : string.Empty;
                rowItem.UseCode = node.Attributes["UseCode"] != null ? node.Attributes["UseCode"].Value : string.Empty;
                rowItem.UseDES = node.Attributes["UseDES"] != null ? node.Attributes["UseDES"].Value : string.Empty;
                rowItem.AsLink = node.Attributes["AsLink"] != null ? node.Attributes["AsLink"].Value : string.Empty;
                if(node.Attributes["Popover"] != null)
                {
                    rowItem.UsePopover = true;
                    rowItem.PopoverOptions = node.Attributes["Popover"].Value;
                }
                rowItem.Properties = node.Attributes["Properties"] != null ? node.Attributes["Properties"].Value : string.Empty;
                result.AddItem(rowItem);
            }

            return result;
        }
    }
}