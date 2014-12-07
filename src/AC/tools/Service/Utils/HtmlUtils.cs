using System.Xml;

namespace AC.Tools.Service.Utils
{
    /// <summary>
    /// Html工具类，主要用来生成各种html元素
    /// </summary>
    public class HtmlUtils
    {
        protected XmlDocument Document;

        #region 构造函数

        public HtmlUtils(XmlDocument document)
        {
            Document = document;
        }

        #endregion

        /// <summary>
        /// 创建一个<code>div</code>元素
        /// </summary>
        /// <returns>返回<div></div></returns>
        public XmlElement CreateBlock()
        {
            return Document.CreateElement("div");
        }

        /// <summary>
        /// 创建一个<code>section</code>元素
        /// </summary>
        /// <returns>返回<section></section></returns>
        public XmlElement CreateSection()
        {
            return Document.CreateElement("section");
        }

        /// <summary>
        /// 以指定名称生成<code>a</code>元素
        /// </summary>
        /// <param name="name"><code>name</code>属性的名称</param>
        /// <returns>返回<a name='name'></a></returns>
        public XmlElement CreateBookmark(string name)
        {
            XmlElement basicLink = Document.CreateElement("a");
            basicLink.SetAttribute("name", name);
            return basicLink;
        }

        /// <summary>
        /// 创建<code>h1</code> html元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateHeader1()
        {
            return Document.CreateElement("h1");
        }

        /// <summary>
        /// 创建<code>h2</code> html元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateHeader2()
        {
            return Document.CreateElement("h2");
        }

        /// <summary>
        /// 创建<code>s</code> html元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateStrikeout()
        {
            return Document.CreateElement("s");
        }

        /// <summary>
        /// 创建一个超链接，指向指定地址<code>&lt;a href=internalDestination&gt;&lt;/a&gt;</code>
        /// </summary>
        /// <param name="internalDestination"></param>
        /// <returns></returns>
        public XmlElement CreateHyperlink(string internalDestination)
        {
            XmlElement basicLink = Document.CreateElement("a");
            basicLink.SetAttribute("href", internalDestination);
            return basicLink;
        }

        /// <summary>
        /// 创建一个<code>img</code>元素
        /// <code>&lt;img src=src&gt;&lt;/img&gt;</code>
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public XmlElement CreateImage(string src)
        {
            XmlElement result = Document.CreateElement("img");
            result.SetAttribute("src", src);
            return result;
        }

        /// <summary>
        /// 创建一个<code>br</code> 元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateLineBreak()
        {
            return Document.CreateElement("br");
        }

        /// <summary>
        /// 创建一个<code>li</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateListItem()
        {
            return Document.CreateElement("li");
        }

        /// <summary>
        /// 创建一个<code>p</code>段落元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateParagraph()
        {
            return Document.CreateElement("p");
        }

        /// <summary>
        /// 创建一个<code>table</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTable()
        {
            return Document.CreateElement("table");
        }

        /// <summary>
        /// 创建一个<code>tbody</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableBody()
        {
            return Document.CreateElement("tbody");
        }

        /// <summary>
        /// 创建一个<code>td</code>单元格元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableCell()
        {
            return Document.CreateElement("td");
        }

        /// <summary>
        /// 创建一个<code>col</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableColumn()
        {
            return Document.CreateElement("col");
        }

        /// <summary>
        /// 创建一个<code>colgroup</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableColumnGroup()
        {
            return Document.CreateElement("colgroup");
        }

        /// <summary>
        /// 创建一个<code>thead</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableHeader()
        {
            return Document.CreateElement("thead");
        }

        /// <summary>
        /// 创建一个<code>th</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableHeaderCell()
        {
            return Document.CreateElement("th");
        }

        /// <summary>
        /// 创建一个<code>code</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateCode()
        {
            return Document.CreateElement("code");
        }

        /// <summary>
        /// 创建一个<code>tr</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateTableRow()
        {
            return Document.CreateElement("tr");
        }

        public XmlElement CreateText(string data)
        {
            //return Document.CreateTextNode(data);
            XmlElement xmlElement = Document.CreateElement("span");
            xmlElement.InnerXml = data;
            return xmlElement;
        }

        /// <summary>
        /// 创建一个<code>ul</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateUnorderedList()
        {
            return Document.CreateElement("ul");
        }

        /// <summary>
        /// 创建一个<code>strong</code>元素
        /// </summary>
        /// <returns></returns>
        public XmlElement CreateStrong()
        {
            return Document.CreateElement("strong");
        }
    }
}