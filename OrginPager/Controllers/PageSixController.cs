using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrginPager.Controllers
{
    public class PageSixController : Controller
    {
        // GET: PageFour
        public ActionResult Index(int? id)
        {
            //具体的页面数 
            int pageIndex = id ?? 1;
            //页面显示条数
            int pageSize = Request["pageSize"] == null ? 10 : Convert.ToInt32(Request["pageSize"]);
            // 数据记录条数
            int totalCount = 0;
            List<Models.Person> list = new OrginPager.BLL.PersonService().GetAllPerson(pageSize, pageIndex, "", out totalCount);
            ViewBag.totalCount = totalCount;
            return View(list);
        }
    }
}