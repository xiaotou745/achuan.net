using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AC.Tools.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tools;
using tools.Controllers;

namespace tools.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // 排列
            HomeController controller = new HomeController();

            // 操作
            ViewResult result = controller.Index() as ViewResult;

            // 断言
            Assert.AreEqual("修改此模板以快速启动你的 ASP.NET MVC 应用程序。", result.ViewBag.Message);
        }

        [TestMethod]
        public void About()
        {
            // 排列
            HomeController controller = new HomeController();

            // 操作
            ViewResult result = controller.About() as ViewResult;

            // 断言
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            // 排列
            HomeController controller = new HomeController();

            // 操作
            ViewResult result = controller.Contact() as ViewResult;

            // 断言
            Assert.IsNotNull(result);
        }
    }
}
