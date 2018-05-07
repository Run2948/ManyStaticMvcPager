using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OrginPager.Controllers
{
    public class PageOneController : Controller
    {
        // GET: PageOne
        public ActionResult Index()
        {
            ViewBag.pageCount = new OrginPager.BLL.PersonService().GetPersonCount().ToString();
            return View();
        }

        [HttpPost]
        public ActionResult GetPagerPerson()
        {
            string result = string.Empty;
            //具体的页面数 
            int pageIndex = Request["pageIndex"] == null ? 1 : Convert.ToInt32(Request["pageIndex"]);
            //页面显示条数
            int pageSize = Request["pageSize"] == null ? 10 : Convert.ToInt32(Request["pageSize"]);
            // 数据记录条数
            int totalCount = 0;
            List<Models.Person> list = new OrginPager.BLL.PersonService().GetAllPerson(pageSize, pageIndex, "", out totalCount);
            StringBuilder sb = new StringBuilder();
            foreach (Models.Person p in list)
            {
                sb.Append("<tr><td>");
                sb.Append(p.Id.ToString());
                sb.Append("</td><td>");
                sb.Append(p.Name);
                sb.Append("</td></tr>");
            }
            result = sb.ToString();
            return Content(result);
        }
    }
}