using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrginPager.Controllers
{
    public class PageFourController : Controller
    {
        // GET: PageFour
        private readonly int pageSize = 10;

        public ActionResult Index()
        {
            //页面显示条数
            ViewBag.PageSize = pageSize;
            // 数据记录条数
            ViewBag.TotalCount = new OrginPager.BLL.PersonService().GetPersonCount();
            return View();
        }

        public ActionResult AjaxPaging(int pageIndex = 1)
        {
            int totalCount = 0;
            var persons = new OrginPager.BLL.PersonService().GetAllPerson(pageSize, pageIndex, "", out totalCount);
            return PartialView("_Index", persons);
        }
    }
}