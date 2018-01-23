using System;
using System.Web.Mvc;

namespace EMCManagementSystem.Controllers
{
    // Token: 0x02000017 RID: 23
    public class MainController : Controller
    {
        // Token: 0x06000101 RID: 257 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Index()
        {
            return base.View();
        }

        // Token: 0x06000102 RID: 258 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Home()
        {
            return base.View();
        }

        // Token: 0x06000103 RID: 259 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Home2()
        {
            return base.View();
        }
    }
}
