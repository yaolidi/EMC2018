using System;
using System.Web.Mvc;

namespace EMCManagementSystem.Controllers
{
    // Token: 0x02000015 RID: 21
    public class EquipmentController : Controller
    {
        // Token: 0x060000E9 RID: 233 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Index()
        {
            return base.View();
        }

        // Token: 0x060000EA RID: 234 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Equipmentinformation()
        {
            return base.View();
        }

        // Token: 0x060000EB RID: 235 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult equipmentInsert()
        {
            return base.View();
        }

        // Token: 0x060000EC RID: 236 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Usage()
        {
            return base.View();
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult InsertUsage()
        {
            return base.View();
        }

        // Token: 0x060000EE RID: 238 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult ProblemRecord()
        {
            return base.View();
        }

        // Token: 0x060000EF RID: 239 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult InsertProblemRecord()
        {
            return base.View();
        }
    }
}
