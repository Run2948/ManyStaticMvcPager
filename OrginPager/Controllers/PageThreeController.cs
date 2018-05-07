using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrginPager.Models;
using PagedList;

namespace OrginPager.Controllers
{
    public class PageThreeController : Controller
    {
        // GET: PageThree
        public ActionResult Index(int? page)
        {
            return Query(page);
        }

        [HttpPost]
        public ActionResult Index(int? page, FormCollection form)
        {
            return Query(page, true);
        }

        public ActionResult Query(int? page, bool ajaxQuery = false)
        {
            //具体的页面数 
            int pageIndex = page ?? 1;
            //页面显示条数
            int pageSize = Request["pageSize"] == null ? 10 : Convert.ToInt32(Request["pageSize"]);
            // 数据记录条数
            int totalCount = 0;
            List<Models.Person> list = new OrginPager.BLL.PersonService().GetAllPerson(pageSize, pageIndex, "", out totalCount);
            var pageData = new StaticPagedList<Person>(list, pageIndex, pageSize, totalCount);
            return ajaxQuery ? (ActionResult) PartialView("_Index", pageData) : View("Index", pageData);
        }
    }
}