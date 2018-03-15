using System;
using System.Web.Mvc;
using EMCManagementSystem.Models;
using System.Linq;
using System.Globalization;

namespace EMCManagementSystem.Controllers
{
    // Token: 0x02000017 RID: 23
    public class MainController : Controller
    {

        EMCEntities myEMCEntities = new EMCEntities();


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

        //  首页2
        #region 查询 日历安排
        public ActionResult CalendarArrangement() {

            var data = (from myCalendarArrangement in myEMCEntities.CalendarArrangement
                        select new
                        {
                            CalendarArrangement_id = myCalendarArrangement.CalendarArrangement_id.ToString().Trim(),
                            CalendarArrangement_title = myCalendarArrangement.CalendarArrangement_title.ToString().Trim(),
                            StartTime = myCalendarArrangement.StartTime.ToString().Trim(),
                            EndTime = myCalendarArrangement.EndTime.ToString().Trim(),
                            Colorclass = myCalendarArrangement.Colorclass.ToString().Trim(),

                        }).Distinct().ToList();


            return base.Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 保存日历事件

        public ActionResult SaveCalendar(string title,string start,string className)
        {

            DateTime startStr = Convert.ToDateTime(start);
            CalendarArrangement myCalendarArrangement = new CalendarArrangement();
            //sdfsdf.SexID = sexid;
            myCalendarArrangement.CalendarArrangement_title = title;
            myCalendarArrangement.StartTime = startStr;
          
            myCalendarArrangement.Colorclass = className;
            myCalendarArrangement.InsertTime = Convert.ToDateTime(DateTime.Now.TimeOfDay.ToString());

            myEMCEntities.CalendarArrangement.Add(myCalendarArrangement);
            myEMCEntities.SaveChanges();
            int EMCFT = myCalendarArrangement.CalendarArrangement_id;
            return Json(EMCFT, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 修改日历事件(开始时间不变 改变结束时间)

        public ActionResult UpdataSaveCalendarOne(int CalendarArrangement_id,string end)
        {
            DateTime endStr = Convert.ToDateTime(end);
            CalendarArrangement myCalendarArrangement = new CalendarArrangement();
            var AlarmNotice = myEMCEntities.CalendarArrangement.Find(CalendarArrangement_id);
            AlarmNotice.EndTime = endStr;
            AlarmNotice.InsertTime = Convert.ToDateTime(DateTime.Now.TimeOfDay.ToString()); ;

            int EMCFT = myEMCEntities.SaveChanges();
            return Json(EMCFT, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 修改日历事件(改变开始时间 改变结束时间)

        public ActionResult UpdataSaveCalendarTow(int CalendarArrangement_id,string start, string end)
        {
            DateTime startStr = Convert.ToDateTime(start);
            DateTime endStr = Convert.ToDateTime(end);
            CalendarArrangement myCalendarArrangement = new CalendarArrangement();
            var AlarmNotice = myEMCEntities.CalendarArrangement.Find(CalendarArrangement_id);
            AlarmNotice.StartTime = startStr;
            AlarmNotice.EndTime = endStr;
            AlarmNotice.InsertTime = Convert.ToDateTime(DateTime.Now.TimeOfDay.ToString()); ;

            int EMCFT = myEMCEntities.SaveChanges();
            return Json(EMCFT, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 修改日历标题

        public ActionResult UpdataTitle(int CalendarArrangement_id, string title)
        {
            CalendarArrangement myCalendarArrangement = new CalendarArrangement();
            var AlarmNotice = myEMCEntities.CalendarArrangement.Find(CalendarArrangement_id);
            AlarmNotice.CalendarArrangement_title = title;
            AlarmNotice.InsertTime = Convert.ToDateTime(DateTime.Now.TimeOfDay.ToString()); ;

            int EMCFT = myEMCEntities.SaveChanges();
            return Json(EMCFT, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 删除日历

        public ActionResult DelectSaveCalendar(int CalendarArrangement_id)
        {

            var AlarmNotice = myEMCEntities.CalendarArrangement.Where(m => m.CalendarArrangement_id == CalendarArrangement_id).Single();

            myEMCEntities.CalendarArrangement.Remove(AlarmNotice);

            int EMCFT = myEMCEntities.SaveChanges();
            return Json(EMCFT, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}