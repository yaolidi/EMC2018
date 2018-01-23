using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMCManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        /// <summary>
        /// 时间：2017/10/18
        /// 创建人：姚礼迪
        /// 模块：任务管理模块
        /// </summary>
        /// <returns></returns>
        // GET: Task
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Addtask()
        {
            return View();
        }

        

    }
}