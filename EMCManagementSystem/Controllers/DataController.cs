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
        //试验各项合格比例
        public ActionResult selectqualifiedratio(int judgenum)
        {

            ArrayList qualifiedlist = new ArrayList();
            if (judgenum == 1)
            {
                int AV_LH_qualifiedcount = 0;
                int AV_LV_qualifiedcount = 0;
                int AV_RH_qualifiedcount = 0;
                int AV_RV_qualifiedcount = 0;
                int AK_LH_qualifiedcount = 0;
                int AK_LV_qualifiedcount = 0;
                int AK_RH_qualifiedcount = 0;
                int AK_RV_qualifiedcount = 0;
                var data = (from tbpath in this.dbEMCEntities.path
                            select new
                            {
                                AV_LH_qualified = tbpath.AV_LH_qualified,
                                AV_LV_qualified = tbpath.AV_LV_qualified,
                                AV_RH_qualified = tbpath.AV_RH_qualified,
                                AV_RV_qualified = tbpath.AV_RV_qualified,
                                AK_LH_qualified = tbpath.AK_LH_qualified,
                                AK_LV_qualified = tbpath.AK_LV_qualified,
                                AK_RH_qualified = tbpath.AK_RH_qualified,
                                AK_RV_qualified = tbpath.AK_RV_qualified

                            }).ToList();
                foreach (var qualified14023 in data.ToList())
                {//'AV_LH', 'AV_LV', 'AV_RH', 'AV_RV','AK_LH', 'AK_LV', 'AK_RH', 'AK_RV'
                    if (qualified14023.AV_LH_qualified != null && qualified14023.AV_LH_qualified.Trim() == "合格")
                    {
                        AV_LH_qualifiedcount++;
                    }
                    if (qualified14023.AV_LV_qualified != null && qualified14023.AV_LV_qualified.Trim() == "合格")
                    {
                        AV_LV_qualifiedcount++;
                    }
                    if (qualified14023.AV_RH_qualified != null && qualified14023.AV_RH_qualified.Trim() == "合格")
                    {
                        AV_RH_qualifiedcount++;
                    }
                    if (qualified14023.AV_RV_qualified != null && qualified14023.AV_RV_qualified.Trim() == "合格")
                    {
                        AV_RV_qualifiedcount++;
                    }
                    if (qualified14023.AK_LH_qualified != null && qualified14023.AK_LH_qualified.Trim() == "合格")
                    {
                        AK_LH_qualifiedcount++;
                    }
                    if (qualified14023.AK_LV_qualified != null && qualified14023.AK_LV_qualified.Trim() == "合格")
                    {
                        AK_LV_qualifiedcount++;
                    }
                    if (qualified14023.AK_RH_qualified != null && qualified14023.AK_RH_qualified.Trim() == "合格")
                    {
                        AK_RH_qualifiedcount++;
                    }
                    if (qualified14023.AK_RV_qualified != null && qualified14023.AK_RV_qualified.Trim() == "合格")
                    {
                        AK_RV_qualifiedcount++;
                    }
                }
                qualifiedlist.Add(AV_LH_qualifiedcount);
                qualifiedlist.Add(AV_LV_qualifiedcount);
                qualifiedlist.Add(AV_RH_qualifiedcount);
                qualifiedlist.Add(AV_RV_qualifiedcount);
                qualifiedlist.Add(AK_LH_qualifiedcount);
                qualifiedlist.Add(AK_LV_qualifiedcount);
                qualifiedlist.Add(AK_RH_qualifiedcount);
                qualifiedlist.Add(AK_RV_qualifiedcount);
            }
            else if (judgenum == 2)
            {
                //合格率
                int ROD16_qualifiedcount = 0;
                int ROD70_qualifiedcount = 0;
                int LOOP_16y_qualifiedcount = 0;
                int LOOP_16x_qualifiedcount = 0;
                int LOOP_70y_qualifiedcount = 0;
                int LOOP_70x_qualifiedcount = 0;
                var data = (from tbpath in this.dbEMCEntities.path_18387
                            select new
                            {
                                ROD16_qualified = tbpath.ROD16_qualified,
                                ROD70_qualified = tbpath.ROD70_qualified,
                                LOOP_16y_qualified = tbpath.LOOP_16y_qualified,
                                LOOP_16x_qualified = tbpath.LOOP_16x_qualified,
                                LOOP_70y_qualified = tbpath.LOOP_70y_qualified,
                                LOOP_70x_qualified = tbpath.LOOP_70x_qualified,
                            }).ToList();
                foreach (var qualified18387 in data.ToList())
                {
                    if (qualified18387.ROD16_qualified != null && qualified18387.ROD16_qualified.Trim() == "合格")
                    {
                        ROD16_qualifiedcount++;
                    }
                    if (qualified18387.ROD70_qualified != null && qualified18387.ROD70_qualified.Trim() == "合格")
                    {
                        ROD70_qualifiedcount++;
                    }
                    if (qualified18387.LOOP_16y_qualified != null && qualified18387.LOOP_16y_qualified.Trim() == "合格")
                    {
                        LOOP_16y_qualifiedcount++;
                    }
                    if (qualified18387.LOOP_16x_qualified != null && qualified18387.LOOP_16x_qualified.Trim() == "合格")
                    {
                        LOOP_16x_qualifiedcount++;
                    }
                    if (qualified18387.LOOP_70y_qualified != null && qualified18387.LOOP_70y_qualified.Trim() == "合格")
                    {
                        LOOP_70y_qualifiedcount++;
                    }
                    if (qualified18387.LOOP_70x_qualified != null && qualified18387.LOOP_70x_qualified.Trim() == "合格")
                    {
                        LOOP_70x_qualifiedcount++;
                    }
                }
                qualifiedlist.Add(ROD16_qualifiedcount);
                qualifiedlist.Add(ROD70_qualifiedcount);
                qualifiedlist.Add(LOOP_16x_qualifiedcount);
                qualifiedlist.Add(LOOP_16y_qualifiedcount);
                qualifiedlist.Add(LOOP_70x_qualifiedcount);
                qualifiedlist.Add(LOOP_70y_qualifiedcount);
            }
            return Json(qualifiedlist, JsonRequestBehavior.AllowGet);
        }
        // Token: 0x04000072 RID: 114
        private EMCEntities dbEMCEntities = new EMCEntities();
    }
}
