using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrginPager.Models;
using PagedList;

namespace OrginPager.Controllers
{
    public class PageTwoController : Controller
    {
        // GET: PageTwo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxPage(int? page)
        {
            //具体的页面数 
            int pageIndex = page ?? 1;
            //页面显示条数
            int pageSize = Request["pageSize"] == null ? 10 : Convert.ToInt32(Request["pageSize"]);
            // 数据记录条数
            int totalCount = 0;
            List<Models.Person> list = new OrginPager.BLL.PersonService().GetAllPerson(pageSize, pageIndex, "", out totalCount);
            var personsAsIPageList = new StaticPagedList<Person>(list, pageIndex, pageSize, totalCount);
            return Json(new { persons = personsAsIPageList, pager = personsAsIPageList.GetMetaData() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ajax()

        {
            return View();
        }
    }
}