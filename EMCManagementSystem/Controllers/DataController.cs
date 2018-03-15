using System;
using System.Linq;
using System.Web.Mvc;
using EMCManagementSystem.Models;
using System.Collections.Generic;
using System.Collections;

namespace EMCManagementSystem.Controllers
{
    
    public class DataController : Controller
    {
       //数据2
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
        public ActionResult selectCarQualification_18387()
        {
            var data = (from tbCT in this.dbEMCEntities.CarQualification_18387
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
        /// <summary>
        /// 汽车类型
        /// </summary>
        /// <returns></returns>
        public ActionResult category()
        {
            return base.View();
        }
        /// <summary>
        /// 检测项合格率
        /// </summary>
        /// <returns></returns>
        public ActionResult qualified()
        {
            return base.View();
        }
        /// <summary>
        /// 全部分类的合格率
        /// </summary>
        /// <returns></returns>
        public ActionResult qualified_all()
        {
            return base.View();
        }
        public ActionResult selectYearCarQualification()
        {
            var data = (from tbCT in this.dbEMCEntities.CarYearQualification
                        select new
                        {
                            name = tbCT.carName,
                          
                        }).Distinct().ToList();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (var one in data.ToList())
            {
                Dictionary<string, object> p = new Dictionary<string, object>();
                ArrayList arraylist = new ArrayList();
                p.Add("name", one.name.ToString().Trim());
                string name = one.name.ToString().Trim();
                var data2 = (from tbCT in this.dbEMCEntities.CarYearQualification
                             orderby tbCT.year descending
                             where tbCT.carName== name
                             select new
                            {
                                  tbCT.Qualification,
                                 tbCT.year

                             }).Distinct().ToList();
               
                foreach (var one2 in data2.ToList())
                {
                    arraylist.Add(one2.Qualification);
                }
                    
                p.Add("data", arraylist);
                list.Add(p);
            }
           return Json(list, JsonRequestBehavior.AllowGet);
        }
        // Token: 0x04000072 RID: 114
        private EMCEntities dbEMCEntities = new EMCEntities();
    }
}
