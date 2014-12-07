using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AC.Tools.Service
{
    /// <summary>
    /// 表分组信息
    /// </summary>
    public class TableGroupInfo
    {
        private readonly List<TableInfo> lstTables = new List<TableInfo>();

        /// <summary>
        /// 表业务分组名称，如中转仓
        /// </summary>
        public string GroupName { get; set; }

        public List<TableInfo> Tables
        {
            get { return lstTables; }
        }
    }

    /// <summary>
    /// 分组下表的详细信息
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 数据库服务器
        /// </summary>
        public string DbServer { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表信息描述
        /// </summary>
        public string TableDesc { get; set; }
    }

    /// <summary>
    /// 表分组服务
    /// </summary>
    public class TableGroupService
    {
        private readonly XmlDocument xmlDocument;

        public TableGroupService(string tableGroupPath)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.Load(tableGroupPath);
        }

        /// <summary>
        /// 根据表分组名称获取分组详细信息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public TableGroupInfo GetGroupByName(string groupName)
        {
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/TableGroups/TableGroup");

            if (xmlNodeList == null || xmlNodeList.Count == 0)
            {
                return null;
            }

            return (from XmlNode xmlNode in xmlNodeList where xmlNode.Attributes["GroupName"].Value == groupName select GetTableGroupInfo(xmlNode)).FirstOrDefault();
        }

        /// <summary>
        /// 获取所有分组信息
        /// </summary>
        /// <returns></returns>
        public List<TableGroupInfo> GetGroups()
        {
            XmlNodeList groupNodes = xmlDocument.SelectNodes("/TableGroups/TableGroup");
            if (groupNodes == null || groupNodes.Count == 0)
            {
                return new List<TableGroupInfo>();
            }
            var result = new List<TableGroupInfo>();
            foreach (XmlNode groupNode in groupNodes)
            {
                TableGroupInfo groupInfo = GetTableGroupInfo(groupNode);
                if (groupInfo != null)
                {
                    result.Add(groupInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据XML分组节点获取分组对象
        /// </summary>
        /// <param name="groupNode"></param>
        /// <returns></returns>
        private TableGroupInfo GetTableGroupInfo(XmlNode groupNode)
        {
            if (groupNode == null)
            {
                return null;
            }
            XmlAttributeCollection groupNodeAttr = groupNode.Attributes;
            var groupInfo = new TableGroupInfo {GroupName = groupNodeAttr["GroupName"].Value};
            if (groupNode.HasChildNodes)
            {
                foreach (XmlNode tableNode in groupNode.ChildNodes)
                {
                    TableInfo tableInfo = GetTableInfo(tableNode);
                    if (tableInfo != null)
                    {
                        groupInfo.Tables.Add(tableInfo);
                    }
                }
            }
            return groupInfo;
        }

        /// <summary>
        /// 根据table节点获取table对象
        /// </summary>
        /// <param name="tableNode"></param>
        /// <returns></returns>
        private TableInfo GetTableInfo(XmlNode tableNode)
        {
            if (tableNode == null)
            {
                return null;
            }
            var result = new TableInfo();

            XmlAttributeCollection tableAttr = tableNode.Attributes;
            result.DbServer = tableAttr["DbServer"] == null ? string.Empty : tableAttr["DbServer"].Value;
            result.DbName = tableAttr["DbName"] == null ? string.Empty : tableAttr["DbName"].Value;
            result.TableName = tableAttr["TableName"] == null ? string.Empty : tableAttr["TableName"].Value;
            result.TableDesc = tableAttr["TableDes"] == null ? string.Empty : tableAttr["TableDes"].Value;

            return result;
        }
    }
}