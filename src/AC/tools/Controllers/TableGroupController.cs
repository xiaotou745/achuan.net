using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AC.Code.Config;
using AC.Tools.Service;
using AC.Util;

namespace AC.Tools.Controllers
{
    public class TableGroupController : Controller
    {
        //
        // GET: /TableGroup/

        public ActionResult Index()
        {
            string path = Path.Combine(Server.MapPath("~"), @"Content\Data\tableGroup.xml");
            var tableGroupService = new TableGroupService(path);
            List<TableGroupInfo> tableGroupInfos = tableGroupService.GetGroups();
            ViewData["lstTableGroups"] = tableGroupInfos;
            return View();
        }

        public ActionResult Details(string groupName)
        {
            string path = Path.Combine(Server.MapPath("~"), @"Content\Data\tableGroup.xml");
            var tableGroupService = new TableGroupService(path);
            TableGroupInfo tableGroupInfo = tableGroupService.GetGroupByName(groupName);
            ViewData["groupInfo"] = tableGroupInfo;
            ViewData["groupName"] = groupName;

            IList<KeyValuePair<string, string>> lstDaoStyleDesc = EnumUtils.GetEnumDescriptions(typeof (DaoStyle));
            IList<KeyValuePair<string, string>> lstCallStyleDesc = EnumUtils.GetEnumDescriptions(typeof (CallStyle));
            IList<KeyValuePair<string, string>> lstCodeLayerDesc = EnumUtils.GetEnumDescriptions(typeof (CodeLayer));
            ViewData["lstDaoStyleDesc"] = lstDaoStyleDesc;
            ViewData["lstCallStyleDesc"] = lstCallStyleDesc;
            ViewData["lstCodeLayerDesc"] = lstCodeLayerDesc;
            return View();
        }
    }
}
