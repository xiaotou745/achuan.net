using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCM.Tools.Common;
using SCM.Tools.Common.SysVersionWCF;


namespace WYC.Tools.Controllers
{
    public class SysVersionController : BaseController
    {


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSysVersion(int sysName)
        {
            string version = SysVersionHelper.GetAviVersion((SysNameEnum)sysName);
            return GetResultTip(1, version);
        }

        public ActionResult Submit(FormCollection fc)
        {
            try
            {
                string sysName = fc["SysName"];
                SysNameEnum sysNameEnum = (SysNameEnum)int.Parse(sysName);
                string sysVersion = fc["SysVersion"];
                DateTime pubDate = Convert.ToDateTime(fc["PubDate"]);
                string remark = fc["Remark"];
                string aviVersion = SysVersionHelper.GetAviVersion(sysNameEnum);
                if (!Enum.IsDefined(typeof(SysNameEnum), int.Parse(sysName)))
                {
                    return GetResultTip(-1, "请选择要发版的系统");
                }
                if (string.IsNullOrWhiteSpace(sysVersion))
                {
                    return GetResultTip(-1, "请输入版本号");
                }
                if (string.IsNullOrWhiteSpace(remark))
                {
                    return GetResultTip(-1, "请输入更新内容");
                }
                if (CompareVersion(aviVersion, sysVersion) > 0)
                {
                    return GetResultTip(-1, string.Format("版本号不能小于{0}", aviVersion));
                }

                SysVersionLogDTO sysVersionLogDto = new SysVersionLogDTO()
                                                        {
                                                            SysName = sysNameEnum,
                                                            SysVersion = sysVersion,
                                                            PubDate = pubDate,
                                                            Remark = remark,
                                                            CreateBy = LoginId
                                                        };
                int r = SysVersionHelper.Insert(sysVersionLogDto);
                if (r > 0)
                    return GetResultTip(1, "提交成功");
                else
                    return GetResultTip(-1, "提交失败");
            }
            catch (Exception ex)
            {
                return GetResultTip(-1, ex.Message);
            }
        }
        /// <summary>
        /// 比较版本号 
        /// </summary>
        /// <param name="version1">版本号1 格式：x.x.x.x </param>
        /// <param name="version2">版本号2</param>
        /// <returns>返回值 1:version1大于version2  返回值0:version1等于version2 返回值-1: version1 小于 version2 </returns>
        private int CompareVersion(string version1, string version2)
        {
            if (string.IsNullOrWhiteSpace(version1) || string.IsNullOrWhiteSpace(version2))
                throw new ArgumentException("版本号格式不符合x.x.x.x格式");
            string[] arrVersion1 = version1.Split('.');
            if (arrVersion1.Length != 4)
                throw new ArgumentException("参数版本号1格式不符合x.x.x.x格式");
            string[] arrVersion2 = version2.Split('.');
            if (arrVersion2.Length != 4)
                throw new ArgumentException("参数版本号2格式不符合x.x.x.x格式");
            if (arrVersion1[0] != arrVersion2[0])
                return Convert.ToInt32(arrVersion1[0]).CompareTo(Convert.ToInt32(arrVersion2[0]));
            else if (arrVersion1[1] != arrVersion2[1])
                return Convert.ToInt32(arrVersion1[1]).CompareTo(Convert.ToInt32(arrVersion2[1]));
            else if (arrVersion1[2] != arrVersion2[2])
                return Convert.ToInt32(arrVersion1[2]).CompareTo(Convert.ToInt32(arrVersion2[2]));
            else if (arrVersion1[3] != arrVersion2[3])
                return Convert.ToInt32(arrVersion1[3]).CompareTo(Convert.ToInt32(arrVersion2[3]));
            else
                return 0;
        }


        public ActionResult VersionManage()
        {
            int sysName = -1;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["key"]))
                sysName = Convert.ToInt32(Request.QueryString["key"]);
            ViewData["SysName"] = sysName;

            return View();
        }

        public ActionResult QueryData(FormCollection fc)
        {
            try
            {
                string sysName = fc["SysName"];
                string strPubDateStart = fc["PubDateStart"];
                string strPubDateEnd = fc["PubDateEnd"];
                SysVersionLogQueryCriteria queryCriteria = new SysVersionLogQueryCriteria();
                if (!string.IsNullOrWhiteSpace(sysName) && Enum.IsDefined(typeof(SysNameEnum), int.Parse(sysName)))
                {
                    queryCriteria.SysName = (SysNameEnum)int.Parse(sysName);
                }
                if (!string.IsNullOrWhiteSpace(strPubDateStart))
                    queryCriteria.PubDateStart = DateTime.Parse(strPubDateStart).Date;
                if (!string.IsNullOrWhiteSpace(strPubDateEnd))
                    queryCriteria.PubDateEnd = DateTime.Parse(strPubDateEnd).Date;
                queryCriteria.PageSize = Convert.ToInt32(fc["PageSize"]);
                queryCriteria.PageIndex = Convert.ToInt32(fc["PageIndex"]);
                var list = SysVersionHelper.QueryByPage(queryCriteria);
                var result = (from item in list
                              select new
                                         {
                                             Id = item.Id,
                                             SysName = EnumHelper.GetEnumDescription(item.SysName),
                                             SysVersion = item.SysVersion,
                                             PubDate = item.PubDate.ToString("yyyy/MM/dd"),
                                             CreateBy = item.CreateBy,
                                             CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                                             Remark = item.Remark.Replace("\n", "<br/>"),
                                             Operation = string.Empty
                                         }).ToList();
                return Json(new { ReturnCode = 1, Rows = result, TotalCount = queryCriteria.TotalCount, PageCount = queryCriteria.PageCount, PageIndex = queryCriteria.PageIndex });
            }
            catch (Exception ex)
            {
                return GetResultTip(-1, ex.Message);
            }
        }
        public ActionResult DelVersion(int id)
        {
            try
            {
                int r = SysVersionHelper.Delete(id);
                if (r == 1)
                    return GetResultTip(1, "删除成功");
                return GetResultTip(-1, "删除失败");
            }
            catch (Exception ex)
            {
                return GetResultTip(-1, ex.Message);
            }
        }

    }
}
