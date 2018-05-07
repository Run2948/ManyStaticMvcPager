using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrginPager.Models;
using Webdiyer.WebControls.Mvc;

namespace OrginPager.Controllers
{
    public class PageFiveController : Controller
    {
        // GET: PageFive

        public ActionResult Index(int? page, string name)
        {
            //具体的页面数 
            int pageIndex = page ?? 1;
            //页面显示条数
            int pageSize = Request["pageSize"] == null ? 10 : Convert.ToInt32(Request["pageSize"]);
            // 数据记录条数
            int totalCount = 0;
            List<Models.Person> list = new OrginPager.BLL.PersonService().GetAllPerson(new OrginPager.BLL.PersonService().GetPersonCount(), 1, "", out totalCount);
            PagedList<Person> pageList = list.ToPagedList<Person>(pageIndex, pageSize);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Index", pageList);
            }
            return View(pageList);
        }

        /// <summary>
        /// 是否是Ajax请求
        /// </summary>
        /// <returns></returns>
        public bool IsAjax()
        {
            //return HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}