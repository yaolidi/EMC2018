using System;
using System.Linq;
using System.Web.Mvc;
using EMCManagementSystem.Models;

namespace EMCManagementSystem.Controllers
{
    // Token: 0x02000013 RID: 19
    public class DataController : Controller
    {
        // Token: 0x060000DA RID: 218 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Index()
        {
            return base.View();
        }

        // Token: 0x060000DB RID: 219 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult DataAnalysis()
        {
            return base.View();
        }

        // Token: 0x060000DC RID: 220 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult RankingLis()
        {
            return base.View();
        }

        // Token: 0x060000DD RID: 221 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult RankingLisDetails()
        {
            return base.View();
        }

        // Token: 0x060000DE RID: 222 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult carAnalysis()
        {
            return base.View();
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00003FE0 File Offset: 0x000021E0
        public ActionResult selectCarQualification()
        {
            var data = (from tbCT in this.dbEMCEntities.CarQualification
                        select new
                        {
                            name = tbCT.CarName.ToString().Trim(),
                            y = tbCT.Qualification
                        }).Distinct().ToList();
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult zhexian()
        {
            return base.View();
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult zhexian2()
        {
            return base.View();
        }

        // Token: 0x04000072 RID: 114
        private EMCEntities dbEMCEntities = new EMCEntities();
    }
}
