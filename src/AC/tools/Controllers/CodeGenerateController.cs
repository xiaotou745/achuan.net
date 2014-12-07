using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using AC.IO;
using Newtonsoft.Json;
using AC.Code.Builder;
using AC.Code.Config;
using AC.Code.DbObjects;
using AC.Code.Helper;
using AC.Code.IBuilder;
using AC.Util;

namespace AC.Tools.Controllers
{
    public class CodeGenerateController : Controller
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

        #region Controler

        //
        // GET: /CodeGenerate/

        public ActionResult Index()
        {
            IList<string> dbServers = DbSetting.GetDbServers();
            ViewData["DbServers"] = dbServers;//服务器列表

            //当前选择的服务器,如果初始，则默认选中第一个服务器
            string dbServerSelected = string.IsNullOrEmpty(Request["dbServer"]) ? dbServers[0] : Request["dbServer"];
            ViewData["DbServer"] = dbServerSelected;

            string dbNameSelected = string.IsNullOrEmpty(Request["dbName"]) ? string.Empty : Request["dbName"];//当前选择的数据库，如果初始，则空；
            ViewData["DbName"] = dbNameSelected;

            AC.Code.DbObjects.IDbObject dbObject = DbSetting.CreateDbObject(dbServerSelected);

            ViewData["DbNameSource"] = JsonConvert.SerializeObject(dbObject.GetDBList());

            ViewData["DbTableSource"] = string.IsNullOrEmpty(dbNameSelected)
                                            ? string.Empty
                                            : JsonConvert.SerializeObject(dbObject.GetTables(dbNameSelected));

            IList<KeyValuePair<string, string>> lstDaoStyleDesc = EnumUtils.GetEnumDescriptions(typeof(DaoStyle));
            IList<KeyValuePair<string, string>> lstCallStyleDesc = EnumUtils.GetEnumDescriptions(typeof(CallStyle));
            IList<KeyValuePair<string, string>> lstCodeLayerDesc = EnumUtils.GetEnumDescriptions(typeof(CodeLayer));
            ViewData["lstDaoStyleDesc"] = lstDaoStyleDesc;
            ViewData["lstCallStyleDesc"] = lstCallStyleDesc;
            ViewData["lstCodeLayerDesc"] = lstCodeLayerDesc;

            return View();
        }

        #endregion

        #region Private Methods

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

        #endregion

        public void Zip(string sourceDirectory,string zipFileName)
        {
            try
            {
                // Depending on the directory this could be very large and would require more attention
                // in a commercial package.
                string[] filenames = Directory.GetFiles(sourceDirectory);

                // 'using' statements guarantee the stream is closed properly which is a big source
                // of problems otherwise.  Its exception safe as well which is great.
                using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(zipFileName)))
                {

                    s.SetLevel(9); // 0 - store only to 9 - means best compression

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {

                        // Using GetFileName makes the result compatible with XP
                        // as the resulting path is not absolute.
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        // Setup the entry data as required.

                        // Crc and size are handled by the library for seakable streams
                        // so no need to do them here.

                        // Could also use the last write time or similar for the file.
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);

                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {

                            // Using a fixed size buffer here makes no noticeable difference for output
                            // but keeps a lid on memory usage.
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } 
                            while (sourceBytes > 0);
                        }
                    }

                    // Finish/Close arent needed strictly as the using statement does this automatically
                    // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                    // the created file would be invalid.
                    s.Finish();

                    // Close is important to wrap things up and unlock the file.
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during processing {0}", ex);
                // No need to rethrow the exception as for our purposes its handled.
            }
        }
        public ActionResult GenerateCodeAndDownload(string dbServer, string dbName, string tableName, string modelName,
                                                    string callStyle, string daoStyle, string codeLayer)
        {
            var codeGenerateConfig = new CodeGenerateConfig
            {
                CallStyleHashCode = callStyle,
                CodeLayerHashCode = codeLayer,
                DaoStyleHashCode = daoStyle,
                ModelName = modelName
            };

            IDbObject dbObj = DbSetting.CreateDbObject(dbServer);

            //获取数据库表的详细列信息
            List<ColumnInfo> lstColumns = dbObj.GetColumnInfoList(dbName, tableName);
            //获取主键列信息
            List<ColumnInfo> lstKeys = (from columnInfo in lstColumns
                                        where columnInfo.IsPK
                                        select columnInfo).ToList();
            string directoryPath = Path.Combine(Server.MapPath("~"), "Codes");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    fileInfo.Delete();
                }
            }
            string[] strings = Directory.GetFiles(directoryPath);
            //Service  代码生成
            IBuilderService serviceBuilder = ServiceBuilder.Create()
                .SetGenerateConfig(codeGenerateConfig)
                .SetKeys(lstKeys)
                .GetServiceBuilder();
            string serviceCode = serviceBuilder.GetServiceCode();

            string serviceFile = directoryPath + "\\" + codeGenerateConfig.CodeName.ServiceName + ".cs";
            FileIO.SaveTextFile(serviceFile, serviceCode, Encoding.Default);

            //Dao 代码生成
            IBuilderDao daoBuilder = DaoBuilder.Create()
                .SetDbObj(dbObj)
                .SetDbName(dbName)
                .SetTableName(tableName)
                .SetColFields(lstColumns)
                .SetKeys(lstKeys)
                .SetGenerateConfig(codeGenerateConfig)
                .GetDaoBuilder();
            string daoCode = daoBuilder.GetDaoCode();

            string daoFile = directoryPath + "\\" + codeGenerateConfig.CodeName.DaoName + ".cs";
            FileIO.SaveTextFile(daoFile, daoCode, Encoding.Default);

            //Service DTO 代码生成
            IBuilderDTO serviceDTOBuilder = new BuilderDTO(codeGenerateConfig, lstColumns);
            string serviceDTOCode = serviceDTOBuilder.GetServiceDTOCode();

            FileIO.SaveTextFile(directoryPath + "\\" + codeGenerateConfig.CodeName.ServiceDTOName + ".cs",
                                serviceDTOCode, Encoding.Default);

            //如果是Service五层架构，则继续生成Domain和ServiceImpl层
            if (codeGenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithDomain)
            {
                //生成Domain层代码
                IBuilderDomain builderDomain = new BuilderDomain(lstKeys, codeGenerateConfig);
                string domainCode = builderDomain.GetDomainCode();
                FileIO.SaveTextFile(directoryPath + "\\" + codeGenerateConfig.CodeName.DomainName + ".cs",
                                    domainCode, Encoding.Default);
                //生成ServiceImpl层代码
                IBuilderServiceImpl builderServiceImpl = new BuilderServiceImpl(lstKeys, codeGenerateConfig);
                string serviceImplCode = builderServiceImpl.GetServiceImplCode();
                FileIO.SaveTextFile(directoryPath + "\\" + codeGenerateConfig.CodeName.ServiceImplName + ".cs",
                                    serviceImplCode, Encoding.Default);
            }
            //如果是Service不带Domain层代码，只需要生成ServiceImpl层代码
            if (codeGenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithoutDomain)
            {
                IBuilderServiceImpl builderServiceImpl = new BuilderServiceImpl(lstKeys, codeGenerateConfig);
                string serviceImplCode = builderServiceImpl.GetServiceImplCode();
                FileIO.SaveTextFile(directoryPath + "\\" + codeGenerateConfig.CodeName.ServiceImplName + ".cs",
                                    serviceImplCode, Encoding.Default);
            }
            Zip(directoryPath, "code.zip");
            return Json(new
            {
                IsSuccess = true,
                FilePath="\\\\192.168.96.86\\"+directoryPath.Substring(directoryPath.IndexOf("Codes"))+"\\"
            });
        }

        public ActionResult GenerateCode(string dbServer, string dbName, string tableName, string modelName,
                                         string callStyle, string daoStyle, string codeLayer)
        {
            var codeGenerateConfig = new CodeGenerateConfig
                                         {
                                             CallStyleHashCode = callStyle,
                                             CodeLayerHashCode = codeLayer,
                                             DaoStyleHashCode = daoStyle,
                                             ModelName = modelName
                                         };

            IDbObject dbObj = DbSetting.CreateDbObject(dbServer);

            //获取数据库表的详细列信息
            List<ColumnInfo> lstColumns = dbObj.GetColumnInfoList(dbName, tableName);
            //获取主键列信息
            List<ColumnInfo> lstKeys = (from columnInfo in lstColumns
                                        where columnInfo.IsPK
                                        select columnInfo).ToList();

            //Service  代码生成
            IBuilderService serviceBuilder = ServiceBuilder.Create()
                .SetGenerateConfig(codeGenerateConfig)
                .SetKeys(lstKeys)
                .GetServiceBuilder();
            string serviceCode = serviceBuilder.GetServiceCode();

            //Dao 代码生成
            IBuilderDao daoBuilder = DaoBuilder.Create()
                .SetDbObj(dbObj)
                .SetDbName(dbName)
                .SetTableName(tableName)
                .SetColFields(lstColumns)
                .SetKeys(lstKeys)
                .SetGenerateConfig(codeGenerateConfig)
                .GetDaoBuilder();
            string daoCode = daoBuilder.GetDaoCode();

            //Service DTO 代码生成
            IBuilderDTO serviceDTOBuilder = new BuilderDTO(codeGenerateConfig, lstColumns);
            string serviceDTOCode = serviceDTOBuilder.GetServiceDTOCode();

            //如果是Service五层架构，则继续生成Domain和ServiceImpl层
            if (codeGenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithDomain)
            {
                //生成Domain层代码
                IBuilderDomain builderDomain = new BuilderDomain(lstKeys, codeGenerateConfig);
                string domainCode = builderDomain.GetDomainCode();

                //生成ServiceImpl层代码
                IBuilderServiceImpl builderServiceImpl = new BuilderServiceImpl(lstKeys, codeGenerateConfig);
                string serviceImplCode = builderServiceImpl.GetServiceImplCode();

                return Json(new
                                {
                                    IsSuccess = true,
                                    ServiceDTOCode = serviceDTOCode,
                                    ServiceCode = serviceCode,
                                    ServiceImplCode = serviceImplCode,
                                    DomainCode = domainCode,
                                    DaoCode = daoCode
                                });
            }
            //如果是Service不带Domain层代码，只需要生成ServiceImpl层代码
            if (codeGenerateConfig.CodeLayer == CodeLayer.ServiceLayerWithoutDomain)
            {
                IBuilderServiceImpl builderServiceImpl = new BuilderServiceImpl(lstKeys, codeGenerateConfig);
                string serviceImplCode = builderServiceImpl.GetServiceImplCode();
                return Json(new
                                {
                                    IsSuccess = true,
                                    ServiceDTOCode = serviceDTOCode,
                                    ServiceCode = serviceCode,
                                    ServiceImplCode = serviceImplCode,
                                    DaoCode = daoCode
                                });
            }

            return Json(new
                            {
                                IsSuccess = true,
                                ServiceDTOCode = serviceDTOCode,
                                ServiceCode = serviceCode,
                                DaoCode = daoCode
                            });
        }
    }
}