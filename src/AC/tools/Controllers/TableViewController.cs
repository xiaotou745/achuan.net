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

            IDbObject dbObject = DbSetting.CreateDbObject(dbServerSelected);

            ViewData["DbNameSource"] = JsonConvert.SerializeObject(dbObject.GetDBList());

            ViewData["DbTableSource"] = string.IsNullOrEmpty(dbNameSelected)
                                            ? string.Empty
                                            : JsonConvert.SerializeObject(dbObject.GetTables(dbNameSelected));
            return View();
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
                                                                "描述"
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
                commonTableInfo.AddRow(tableRow);
            }
            return TableHtmlHelper.GetHtml(commonTableInfo);
        }

        #endregion
    }
}