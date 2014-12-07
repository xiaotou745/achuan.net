using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AC.Tools.Service.Utils;

namespace AC.ToolWeb.Controllers
{
    public class CommonToolsController : Controller
    {
        //
        // GET: /CommonTools/

        public ActionResult DES()
        {
            return View();
        }

        public ActionResult MD5()
        {
            return View();
        }

        public ActionResult DbServerInfo()
        {
            return View();
        }

        public ActionResult Employees()
        {
            string filePath = Path.Combine(Server.MapPath("~"), @"Content\Data\employee.xls");

            var excelToHtml = new ExcelToHtmlConverter();
            Dictionary<string, string> tableHtmls = excelToHtml.GetAllSheetHtml(filePath, true, false);
            //Dictionary<string, string> tableHtmls = HtmlUtilsOfExcel.Create().GetTableHtmls(filePath, true);
            ViewData["PhoneBooks"] = tableHtmls;
            //string employeeHtml = excelToHtml.GetFirstSheetHtml(filePath, true);
            //string employeeHtml = HtmlUtilsOfExcel.Create().GetTableHtmlOfFirstSheet(filePath, true);
            //ViewData["Employee"] = employeeHtml;

            return View();
        }

        public ActionResult Servers()
        {
            string filePath = Path.Combine(Server.MapPath("~"), @"Content\Data\server.xls");

            var excelToHtml = new ExcelToHtmlConverter();
            Dictionary<string, string> tableHtmls = excelToHtml.GetAllSheetHtml(filePath, true, false);
            ViewData["Servers"] = tableHtmls;

            return View();
        }

        public ActionResult SysInfo()
        {
            string title = Request.Params["t"];
            if (string.IsNullOrEmpty(title))
            {
                title = "empty";
            }
            ViewData["t"] = title;


            return View();
        }

        public string DESAndMd5()
        {
            string type = Request["type"];
            string txtSource = Request["txtSource"];
            if (string.IsNullOrEmpty(type))
            {
                return "没有选择加解密类型？不可能吧？";
            }
            if (string.IsNullOrEmpty(txtSource))
            {
                return "不会吧？加解密字符串为空？客户端干啥吃的？";
            }
            switch (type)
            {
                case "ENC":
                    return AC.Security.DES.Encrypt3DES(txtSource);
                case "DEC":
                    return AC.Security.DES.Decrypt3DES(txtSource);
                case "MD5":
                    return AC.Security.MD5.Encrypt(txtSource);
                default:
                    return string.Empty;
            }
        }
    }
}