using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AC.Tools.Service.HtmlHelper;
using Newtonsoft.Json;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;

namespace AC.Tools.Controllers
{
    public class TableViewController : Controller
    {
        #region DbSettings

        private DbSettingsBuilder dbSettings;

        /// <summary>
        /// 数据库工厂对象
        /// </summary>
        protected DbSettingsBuilder DbSetting
        {
            get
            {
                if (dbSettings == null)
                {
                    string filePath = Path.Combine(Server.MapPath("~"), @"Content\Data\dbservers.xml");
                    dbSettings = DbSettingsBuilder.Create(filePath);
                }
                return dbSettings;
            }
        }

        #endregion

        #region Controls

        //
        // GET: /TableView/

        public ActionResult Index()
        {
            IList<string> dbServers = DbSetting.GetDbServers();
            ViewData["DbServers"] = dbServers; //服务器列表

            //当前选择的服务器,如果初始，则默认选中第一个服务器
            string dbServerSelected = string.IsNullOrEmpty(Request["dbServer"]) ? dbServers[0] : Request["dbServer"];
            ViewData["DbServer"] = dbServerSelected;

            string dbNameSelected = string.IsNullOrEmpty(Request["dbName"]) ? string.Empty : Request["dbName"];
                //当前选择的数据库，如果初始，则空；
            ViewData["DbName"] = dbNameSelected;
            //如果当前没有选择数据库，那就显示数据库列表，否则显示表列表
            if (string.IsNullOrEmpty(dbNameSelected))
            {
                ViewData["Type"] = 1;
                ViewData["TypeHtml"] = GetHtmlDbList(dbServerSelected);
            }
            else
            {
                ViewData["Type"] = 2;
                ViewData["TypeHtml"] = GetHtmlTableList(dbServerSelected, dbNameSelected);
            }

            IDbObject dbObject = DbSetting.CreateDbObject(dbServerSelected);

            ViewData["DbNameSource"] = JsonConvert.SerializeObject(dbObject.GetDBList());

            ViewData["DbTableSource"] = string.IsNullOrEmpty(dbNameSelected)
                                            ? string.Empty
                                            : JsonConvert.SerializeObject(dbObject.GetTables(dbNameSelected));
            return View();
        }

        private string GetHtmlTableList(string dbServer, string dbName)
        {
            IDbObject dbObject = DbSetting.CreateDbObject(dbServer);
            List<TableInfo> lstTables = dbObject.GetTablesInfo(dbName);
            var commonTableInfo = new CommonTableInfo
            {
                UseSection = false,
                UseStripped = true,
                ColumnNames = new ColumnNames{
                    "TableName",
                    "Owner",
                    "CreateTime",
                    "Desc",
                    "Operation",
                },
            };
            foreach (var tableInfo in lstTables)
            {
                var tableRow = new TableRow();
                tableRow.AddItem(new TableRowItem
                {
                    Name = "tableName",
                    Data = tableInfo.TabName,
                    Class = "J_tableName",
                    AsLink = true.ToString(),
                    LinkHref = "javascript:;",
                });
                tableRow.AddItem(string.Empty, tableInfo.TabUser);
                tableRow.AddItem(string.Empty, tableInfo.TabDate);
                tableRow.AddItem(string.Empty, tableInfo.TabDesc);
                tableRow.AddItem(new TableRowItem
                {
                    Name = string.Empty,
                    Data = "编辑",
                    Class = "J_editTableDesc",
                    AsLink = true.ToString(),
                    LinkHref = "javascript:;",
                });
                commonTableInfo.AddRow(tableRow);
            }
            return TableHtmlHelper.GetHtml(commonTableInfo);
        }

        private string GetHtmlDbList(string dbServer)
        {
            IDbObject dbObject = DbSetting.CreateDbObject(dbServer);
            List<string> lstDbNames = dbObject.GetDBList();
            var commonTableInfo = new CommonTableInfo
            {
                UseSection = false,
                UseStripped = true,
                ColumnNames = new ColumnNames{
                    "DbName",
                },
            };
            foreach (var dbName in lstDbNames)
            {
                var tableRow = new TableRow();
                TableRowItem item = new TableRowItem
                {
                    Name = "dbname",
                    Data = dbName,
                    AsLink = true.ToString(),
                    LinkHref = "javascript:;"
                };
                tableRow.AddItem(item);
                
                commonTableInfo.AddRow(tableRow);
            }
            return TableHtmlHelper.GetHtml(commonTableInfo);
        }

        public string GetHtmlTableInfo(string dbServer, string dbName, string dbTable)
        {
            IDbObject dbObject = DbSetting.CreateDbObject(dbServer);
            List<ColumnInfo> lstColumnInfo = dbObject.GetColumnInfoList(dbName, dbTable);
            var commonTableInfo = new CommonTableInfo
                                      {
                                          UseSection = false,
                                          UseStripped = true,
                                          ColumnNames = new ColumnNames
                                                            {
                                                                "No",
                                                                "列名",
                                                                "类型",
                                                                "长度",
                                                                "精度",
                                                                "Scale",
                                                                "自增",
                                                                "主键",
                                                                "Null",
                                                                "默认值",
                                                                "描述",
                                                                "Oper",
                                                            }
                                      };
            foreach (ColumnInfo columnInfo in lstColumnInfo)
            {
                var tableRow = new TableRow();
                tableRow.AddItem(string.Empty, columnInfo.Colorder);
                tableRow.AddItem(string.Empty, columnInfo.ColumnName);
                tableRow.AddItem(string.Empty, columnInfo.TypeName);
                tableRow.AddItem(string.Empty, columnInfo.Length);
                tableRow.AddItem(string.Empty, columnInfo.Preci);
                tableRow.AddItem(string.Empty, columnInfo.Scale);
                tableRow.AddItem(string.Empty, columnInfo.IsIdentity ? "√" : string.Empty);
                tableRow.AddItem(string.Empty, columnInfo.IsPK ? "√" : string.Empty);
                tableRow.AddItem(string.Empty, columnInfo.cisNull ? "√" : string.Empty);
                tableRow.AddItem(string.Empty, columnInfo.DefaultVal);
                tableRow.AddItem(string.Empty, columnInfo.DeText);
                tableRow.AddItem(new TableRowItem
                {
                    Name = string.Empty,
                    Data = "编辑",
                    Class = "J_editColumnDesc",
                    AsLink = true.ToString(),
                    LinkHref = "javascript:;",
                });
                commonTableInfo.AddRow(tableRow);
            }
            return TableHtmlHelper.GetHtml(commonTableInfo);
        }

        public bool UpdateDesc(string dbServer, string dbName, string dbTable, string columnName, string desc)
        {
            IDbObject dbObject = DbSetting.CreateDbObject(dbServer);
            bool result = dbObject.UpdateProperty(dbName, dbTable, columnName, desc);
            return result;
        }
        #endregion
    }
}