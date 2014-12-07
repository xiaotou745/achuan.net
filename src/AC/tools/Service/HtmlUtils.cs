using System.Xml;

namespace AC.Tools.Service
{
    public class HtmlUtils
    {
        protected XmlDocument document;

        public HtmlUtils(XmlDocument document)
        {
            this.document = document;
        }

        public XmlElement CreateBlock()
        {
            return document.CreateElement("div");
        }

        public XmlElement CreateSection()
        {
            return document.CreateElement("section");
        }

        public XmlElement CreateBookmark(string name)
        {
            XmlElement basicLink = document.CreateElement("a");
            basicLink.SetAttribute("name", name);
            return basicLink;
        }

        public XmlElement CreateHeader1()
        {
            return document.CreateElement("h1");
        }

        public XmlElement CreateHeader2()
        {
            return document.CreateElement("h2");
        }

        public XmlElement CreateStrikeout()
        {
            return document.CreateElement("s");
        }

        public XmlElement CreateHyperlink(string internalDestination)
        {
            XmlElement basicLink = document.CreateElement("a");
            basicLink.SetAttribute("href", internalDestination);
            return basicLink;
        }

        public XmlElement CreateImage(string src)
        {
            XmlElement result = document.CreateElement("img");
            result.SetAttribute("src", src);
            return result;
        }

        public XmlElement CreateLineBreak()
        {
            return document.CreateElement("br");
        }

        public XmlElement CreateListItem()
        {
            return document.CreateElement("li");
        }

        public XmlElement CreateParagraph()
        {
            return document.CreateElement("p");
        }

        public XmlElement CreateTable()
        {
            return document.CreateElement("table");
        }


        public XmlElement CreateTableBody()
        {
            return document.CreateElement("tbody");
        }

        public XmlElement CreateTableCell()
        {
            return document.CreateElement("td");
        }

        public XmlElement CreateTableColumn()
        {
            return document.CreateElement("col");
        }

        public XmlElement CreateTableColumnGroup()
        {
            return document.CreateElement("colgroup");
        }

        public XmlElement CreateTableHeader()
        {
            return document.CreateElement("thead");
        }

        public XmlElement CreateTableHeaderCell()
        {
            return document.CreateElement("th");
        }

        public XmlElement CreateCode()
        {
            return document.CreateElement("code");
        }

        public XmlElement CreateTableRow()
        {
            return document.CreateElement("tr");
        }

        public XmlText CreateText(string data)
        {
            return document.CreateTextNode(data);
        }

        public XmlElement CreateUnorderedList()
        {
            return document.CreateElement("ul");
        }
        public XmlElement CreateStrong()
        {
            return document.CreateElement("strong");
        }
    }
}