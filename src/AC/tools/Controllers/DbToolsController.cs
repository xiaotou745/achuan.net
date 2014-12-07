using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using LTP.CodeHelper;
using LTP.Helper;
using Newtonsoft.Json;
using SCM.Builder;
using SCM.Code.Config;
using SCM.Code.DbObjects;
using SCM.IBuilder;
using SCM.Tools.Common;
using WYC.Tools.Service;
using WYC.Tools.Service.HtmlHelper;
using IDbObject = LTP.IDBO.IDbObject;

namespace WYC.Tools.Controllers
{
    public class CodeGenerateParams
    {
        public string DbServer { get; set; }
        public string DbName { get; set; }
        public string TableName { get; set; }
        public string ServiceName { get; set; }
        public string SubNamespace { get; set; }
        public ServiceMethodConfig ServiceMethodConfig { get; set; }
        public bool IsCustomNamespace { get; set; }
    }

    public class DbToolsController : BaseController
    {
        private DbSettingsBuilder dbSettings;
        /// <summary>
        /// 数据库工厂对象
        /// </summary>
        protected DbSettingsBuilder DbSetting
        {
            get
            {
                if(dbSettings == null)
                {
                    string filePath = Path.Combine(Server.MapPath("~"), @"Content\Data\dbservers.xml");
                    dbSettings = DbSettingsBuilder.Create(filePath);
                }
                return dbSettings;
            }
        }

        public string GetHtmlTableInfo(string dbServer, string dbName, string dbTable)
        {
            SCM.Code.DbObjects.IDbObject dbObject = DbSetting.CreateDbObject(dbServer);
            List<SCM.Code.Helper.ColumnInfo> lstColumnInfo = dbObject.GetColumnInfoList(dbName, dbTable);
            //IDbObject dbObject = DbObjHelper.CreatDbObj(dbServer, dbName, DbConnString.GetConnString(dbServer));
            //List<ColumnInfo> lstColumnInfo = dbObject.GetColumnInfoList(dbName, dbTable);
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
            foreach (SCM.Code.Helper.ColumnInfo columnInfo in lstColumnInfo)
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

        public ActionResult CodeGenerate()
        {
            IList<string> dbServers = DbSetting.GetDbServers();
            ViewData["DbServers"] = dbServers;//服务器列表

            //当前选择的服务器,如果初始，则默认选中第一个服务器
            string dbServerSelected = string.IsNullOrEmpty(Request["dbServer"]) ? dbServers[0] : Request["dbServer"];
            ViewData["DbServer"] = dbServerSelected;

            string dbNameSelected = string.IsNullOrEmpty(Request["dbName"]) ? string.Empty : Request["dbName"];//当前选择的数据库，如果初始，则空；
            ViewData["DbName"] = dbNameSelected;

            SCM.Code.DbObjects.IDbObject dbObject = DbSetting.CreateDbObject(dbServerSelected);

            ViewData["DbNameSource"] = JsonConvert.SerializeObject(dbObject.GetDBList());

            ViewData["DbTableSource"] = string.IsNullOrEmpty(dbNameSelected)
                                            ? string.Empty
                                            : JsonConvert.SerializeObject(dbObject.GetTables(dbNameSelected));

            return View();
        }

        public ActionResult GetColumnInfo(CodeGenerateParams param)
        {
            if (param == null)
            {
                return Json(new {IsSuccess = false, Error = "参数不允许为空."});
            }
            if (string.IsNullOrEmpty(param.DbServer))
            {
                return Json(new {IsSuccess = false, Error = "数据库服务器不允许为空."});
            }
            if (string.IsNullOrEmpty(param.DbName))
            {
                return Json(new {IsSuccess = false, Error = "数据库名称不允许为空."});
            }
            if (string.IsNullOrEmpty(param.TableName))
            {
                return Json(new {IsSuccess = false, Error = "表名称不允许为空."});
            }
            param.TableName = param.TableName[0].ToString(CultureInfo.InvariantCulture).ToUpper() +
                              param.TableName.Substring(1);

            IDbObject dbObj = DbObjHelper.CreatDbObj(param.DbServer, param.DbName,
                                                     DbConnString.GetConnString(param.DbServer));
            List<ColumnInfo> lstCols = dbObj.GetColumnInfoList(param.DbName, param.TableName);

            var serviceName = new ServiceNameManager(string.Empty, "I" + param.TableName + "Service");
            return Json(new {IsSuccess = true, Rows = lstCols, Names = serviceName});
        }

        public ActionResult GetServiceNameInfo(CodeGenerateParams param)
        {
            if (param == null)
            {
                return Json(new {IsSuccess = false, Error = "参数不允许为空."});
            }
            if (string.IsNullOrEmpty(param.ServiceName))
            {
                return Json(new {IsSuccess = false, Error = "ServiceName不允许为空."});
            }
            if (string.IsNullOrEmpty(param.SubNamespace))
            {
                param.SubNamespace = string.Empty;
            }
            var serviceName = new ServiceNameManager(param.SubNamespace, param.ServiceName);
            if (param.IsCustomNamespace)
            {
                serviceName.SetCustomNameSpace(param.SubNamespace);
            }
            return Json(new {IsSuccess = true, Names = serviceName});
        }

        public ActionResult GenerateCode(string dbServer, string dbName, string tableName)
        {
            //SCM.Code.DbObjects.IDbObject dbObj = DbSetting.CreateDbObject(dbServer);


            IDbObject dbObj = DbObjHelper.CreatDbObj(dbServer, dbName, DbConnString.GetConnString(dbServer));

            var serviceNameManager = new ServiceNameManager(string.Empty, "I" + tableName + "Service");

            var serviceMethodConfig = new ServiceMethodConfig
                                          {
                                              CreateMethodNeeded = true,
                                              GetByIdMethodNeeded = true,
                                              QueryMethodNeeded = true
                                          };
            List<ColumnInfo> lstCols = dbObj.GetColumnInfoList(dbName, tableName);
            List<ColumnInfo> lstKeys = GetKeyFields(dbObj, dbName, tableName);

            IBuilderServiceDTO dtoService = new BuilderServiceDTO(serviceNameManager, lstCols);
            dtoService.DataTypeFilePath = Server.MapPath("/datatype.ini");
            string serviceDtoCode = dtoService.CreatServiceDTO();

            IBuilderService builderService = new BuilderService(lstKeys, serviceNameManager);
            string serviceCode = builderService.GetServiceCode(serviceMethodConfig);

            IBuilderServiceImpl serviceImpl = new BuilderServiceImpl(lstKeys, serviceNameManager);
            string serviceImplCode = serviceImpl.GetServiceCode(serviceMethodConfig);

            IBuilderDomain domainService = new BuilderDomain(lstKeys, serviceNameManager);
            string domainCode = domainService.GetDomainCode(serviceMethodConfig);

            IBuilderDao daoService = new BuilderDao(dbObj, dbName, tableName, lstCols, lstKeys, serviceNameManager);
            string daoCode = daoService.GetDaoCode(serviceMethodConfig);

            return Json(new
                            {
                                IsSuccess = true,
                                Rows = lstCols,
                                Names = serviceNameManager,
                                ServiceDTOCode = serviceDtoCode,
                                ServiceCode = serviceCode,
                                ServiceImplCode = serviceImplCode,
                                DomainCode = domainCode,
                                DaoCode = daoCode
                            });
        }

        /// <summary>
        /// 得到主键的对象信息
        /// </summary>
        /// <returns></returns>
        private List<ColumnInfo> GetKeyFields(IDbObject dbobj, string dbName, string tableName)
        {
            List<ColumnInfo> collist = dbobj.GetColumnInfoList(dbName, tableName);

            IEnumerable<ColumnInfo> columnInfos = from columnInfo in collist
                                                  where columnInfo.IsPK
                                                  select columnInfo;
            return columnInfos.ToList();
        }
    }
}