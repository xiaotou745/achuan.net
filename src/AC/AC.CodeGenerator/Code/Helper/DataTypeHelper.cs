using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace AC.Code.Helper
{
    public class DataTypeHelper
    {
        private readonly Dictionary<string, Dictionary<string, string>> DatatypeDic =
            new Dictionary<string, Dictionary<string, string>>();

        private DataTypeHelper()
        {
        }

        private void Init()
        {
            Assembly assembly = GetType().Assembly;
            Stream datatypeStream = assembly.GetManifestResourceStream(GetType().Namespace + @".datatype.xml");
            var xml = new XmlDocument();
            xml.Load(datatypeStream);

            XmlNodeList sectionNodes = xml.GetElementsByTagName("section");

            foreach (XmlNode sectionNode in sectionNodes)
            {
                string sectionName = sectionNode.Attributes["value"].Value;
                DatatypeDic.Add(sectionName, new Dictionary<string, string>());
                foreach (XmlNode xmlNode in sectionNode.ChildNodes)
                {
                    string name = xmlNode.Attributes["name"].Value;
                    if (!DatatypeDic[sectionName].ContainsKey(name))
                    {
                        DatatypeDic[sectionName].Add(name, xmlNode.Attributes["value"].Value);
                    }
                }
            }
        }

        public static DataTypeHelper Create()
        {
            var result = new DataTypeHelper();
            result.Init();
            return result;
        }

        public string DbTypeToCS(string dbtype)
        {
            string csType = "string";
            if (DatatypeDic["DbToCS"].ContainsKey(dbtype.ToLower().Trim()))
            {
                string val = DatatypeDic["DbToCS"][dbtype.ToLower().Trim()];
                csType = string.IsNullOrEmpty(val) ? dbtype.ToLower().Trim() : val;
            }
            return csType;
        }

        /// <summary>
        /// 是否c#中的值类型
        /// </summary>
        public bool IsValueType(string cstype)
        {
            bool isval = false;
            if (DatatypeDic["isValueType"].ContainsKey(cstype))
            {
                string val = DatatypeDic["isValueType"][cstype.Trim()];
                if (val == "true")
                {
                    isval = true;
                }
            }
            return isval;
        }

        public string CSToProcTypeSQL(string cstype)
        {
            string result = cstype;
            if (DatatypeDic["ToSQLProc"].ContainsKey(cstype.ToLower().Trim()))
            {
                string val = DatatypeDic["ToSQLProc"][cstype.ToLower().Trim()];
                result = string.IsNullOrEmpty(val) ? cstype.ToLower().Trim() : val;
            }
            return result;
        }

        public string CSToProcTypeOra(string cstype)
        {
            string result = cstype;
            if (DatatypeDic["ToOraProc"].ContainsKey(cstype.ToLower().Trim()))
            {
                string val = DatatypeDic["ToOraProc"][cstype.ToLower().Trim()];
                result = string.IsNullOrEmpty(val) ? cstype.ToLower().Trim() : val;
            }
            return result;
        }

        public string CSToProcTypeMySQL(string cstype)
        {
            string result = cstype;
            if (DatatypeDic["ToMySQLProc"].ContainsKey(cstype.ToLower().Trim()))
            {
                string val = DatatypeDic["ToMySQLProc"][cstype.ToLower().Trim()];
                result = string.IsNullOrEmpty(val) ? cstype.ToLower().Trim() : val;
            }
            return result;
        }

        public string CSToProcTypeOleDb(string cstype)
        {
            string result = cstype;
            if (DatatypeDic["ToOleDbProc"].ContainsKey(cstype.ToLower().Trim()))
            {
                string val = DatatypeDic["ToOleDbProc"][cstype.ToLower().Trim()];
                result = string.IsNullOrEmpty(val) ? cstype.ToLower().Trim() : val;
            }
            return result;
        }

        /// <summary>
        /// 该数据类型是否加单引号
        /// </summary>
        /// <param name="columnType">数据库类型</param>
        /// <returns></returns>
        public bool IsAddMark(string columnType)
        {
            bool isadd = false;
            if (DatatypeDic["IsAddMark"].ContainsKey(columnType.ToLower().Trim()))
            {
                string val = DatatypeDic["IsAddMark"][columnType.ToLower().Trim()];
                if (val != "")
                {
                    isadd = true;
                }
            }
            return isadd;
        }
    }
}