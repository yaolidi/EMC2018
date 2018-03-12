using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using EMCManagementSystem.Models;
using Microsoft.CSharp.RuntimeBinder;
using System.Text.RegularExpressions;

namespace EMCManagementSystem.Controllers
{
    // Token: 0x0200001B RID: 27
    public class ReportController : Controller
    {
        // Token: 0x06000111 RID: 273 RVA: 0x00003FD5 File Offset: 0x000021D5
        /// <summary>
        /// 报告视图
        /// author：yld
        /// time:2018/03/01
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return base.View();
        }
        Models.EMCEntities dbEMCEntities = new EMCEntities();
        // Token: 0x06000112 RID: 274 RVA: 0x00003FD5 File Offset: 0x000021D5
        /// <summary>
        /// 新增报告视图
        /// author：yld
        /// time:2018/03/01
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertReport()
        {
            return base.View();
        }

        // Token: 0x06000113 RID: 275 RVA: 0x000043D0 File Offset: 0x000025D0
        /// <summary>
        /// 查询报告数据
        /// author：yld
        /// time:2018/03/01
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectReport()
        {
            var data = (from tbCT in this.dbEMCEntities.OriginalRecord
                        select new
                        {
                            brand = tbCT.brand.ToString().Trim(),
                            taskNumber = tbCT.taskNumber.ToString().Trim(),
                            introduce = tbCT.introduce.ToString().Trim(),
                            ModelCarModel = tbCT.ModelCarModel.ToString().Trim(),
                            OriginalRecordID = tbCT.OriginalRecordID,
                            Remarks = tbCT.Remarks.ToString().Trim(),
                            ResultID = tbCT.ResultID,
                            taskAllName = tbCT.taskAllName,
                            VIN = tbCT.VIN.ToString().Trim(),
                            time = tbCT.time.ToString().Trim(),
                            tbCT.category
                        }).Distinct().ToList();
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 14023
        /// </summary>
        /// <param name="tbOriginalRecord"></param>
        /// <param name="RegistrationName"></param>
        /// <param name="OriginalRecordName"></param>
        /// <returns></returns>
        public ActionResult InsertReportContent(OriginalRecord tbOriginalRecord, string RegistrationName, string OriginalRecordName)
        {
            OriginalRecord originalRecord = new OriginalRecord();
            originalRecord.taskNumber = tbOriginalRecord.taskNumber;
            originalRecord.time = tbOriginalRecord.time;
            originalRecord.brand = tbOriginalRecord.brand.ToString().Trim();
            originalRecord.introduce = tbOriginalRecord.introduce;
            originalRecord.ModelCarModel = tbOriginalRecord.ModelCarModel;
            originalRecord.Remarks = tbOriginalRecord.Remarks;
            originalRecord.ResultID = tbOriginalRecord.ResultID;
            originalRecord.taskAllName = tbOriginalRecord.taskAllName;
            originalRecord.VIN = tbOriginalRecord.VIN;
            originalRecord.category = "14023";
            this.dbEMCEntities.OriginalRecord.Add(originalRecord);
            this.dbEMCEntities.SaveChanges();
            int originalRecordID = originalRecord.OriginalRecordID;
            if (originalRecordID > 0)
            {
                string CarName = tbOriginalRecord.brand.ToString().Trim();
                var list = (from tbCT in this.dbEMCEntities.CarQualification
                            where tbCT.CarName == CarName
                            select new
                            {
                                tbCT.CarID,
                                tbCT.Qualification,
                                tbCT.qualificationNumber,
                                tbCT.UnqualifiedTimes
                            }).Distinct().ToList();
                if (list.Count > 0)
                {
                    ///车型合格率
                    int id = list[0].CarID;
                    if ((from tbReport in this.dbEMCEntities.CarQualification
                         where tbReport.CarID == id
                         select tbReport).ToList<CarQualification>().Count > 0)
                    {
                        CarQualification carQualification = this.dbEMCEntities.CarQualification.Find(new object[]
                        {
                            id
                        });
                        if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                        {
                            carQualification.qualificationNumber = list[0].qualificationNumber + 1;
                            double num = Convert.ToDouble(list[0].qualificationNumber + 1);
                            double num2 = Convert.ToDouble(list[0].UnqualifiedTimes);
                            CarQualification arg_3EF_0 = carQualification;
                            double expr_3D5 = num;
                            arg_3EF_0.Qualification = new double?(Math.Round(expr_3D5 / (expr_3D5 + num2) * 100.0, 2));
                        }
                        else
                        {
                            carQualification.UnqualifiedTimes = list[0].UnqualifiedTimes + 1;
                            double num3 = Convert.ToDouble(list[0].qualificationNumber);
                            double num4 = Convert.ToDouble(list[0].UnqualifiedTimes + 1);
                            CarQualification arg_4AC_0 = carQualification;
                            double expr_492 = num3;
                            arg_4AC_0.Qualification = new double?(Math.Round(expr_492 / (expr_492 + num4) * 100.0, 2));
                        }
                        this.dbEMCEntities.SaveChanges();
                    }
                }
                else
                {
                    CarQualification carQualification2 = new CarQualification();
                    carQualification2.CarName = CarName;
                    if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                    {
                        carQualification2.qualificationNumber = new int?(1);
                        carQualification2.UnqualifiedTimes = new int?(0);
                        carQualification2.Qualification = new double?((double)100);
                    }
                    else
                    {
                        carQualification2.qualificationNumber = new int?(0);
                        carQualification2.UnqualifiedTimes = new int?(1);
                        carQualification2.Qualification = new double?(0.0);
                    }
                    this.dbEMCEntities.CarQualification.Add(carQualification2);
                    this.dbEMCEntities.SaveChanges();
                }
                //年合格率
                int time2 =Convert.ToInt32( Convert.ToDateTime(tbOriginalRecord.time).Year.ToString());
                var list2 = (from tbCT in this.dbEMCEntities.CarYearQualification
                             where tbCT.carName == CarName && tbCT.year == time2
                             select new
                            {
                                tbCT.CarYearQualificationID,
                                tbCT.Qualification,
                                tbCT.qualificationNumber,
                                tbCT.UnqualifiedTimes,
                                tbCT.year
                            }).Distinct().ToList();
                if (list2.Count > 0)
                {
                    int id = list2[0].CarYearQualificationID;
                   
                        if ((from tbReport in dbEMCEntities.CarYearQualification
                         where tbReport.CarYearQualificationID == id
                         select tbReport).ToList<CarYearQualification>().Count > 0)
                    {
                        CarYearQualification carQualification = this.dbEMCEntities.CarYearQualification.Find(new object[]
                        {
                            id
                        });
                        if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                        {
                            carQualification.qualificationNumber = list2[0].qualificationNumber + 1;
                            double num = Convert.ToDouble(list2[0].qualificationNumber + 1);
                            double num2 = Convert.ToDouble(list2[0].UnqualifiedTimes);
                            CarYearQualification arg_3EF_0 = carQualification;
                            double expr_3D5 = num;
                            arg_3EF_0.Qualification = new double?(Math.Round(expr_3D5 / (expr_3D5 + num2)*100, 2));
                        }
                        else
                        {
                            carQualification.UnqualifiedTimes = list2[0].UnqualifiedTimes + 1;
                            double num3 = Convert.ToDouble(list2[0].qualificationNumber);
                            double num4 = Convert.ToDouble(list2[0].UnqualifiedTimes + 1);
                            CarYearQualification arg_4AC_0 = carQualification;
                            double expr_492 = num3;
                            arg_4AC_0.Qualification = new double?(Math.Round(expr_492 / (expr_492 + num4) * 100, 2));
                        }
                        this.dbEMCEntities.SaveChanges();
                    }
                }
                else
                {
                    CarYearQualification carQualification2 = new CarYearQualification();
                    carQualification2.carName = CarName;
                    if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                    {
                        int  time = Convert.ToInt32 (Convert.ToDateTime(tbOriginalRecord.time).Year.ToString());
                        carQualification2.year = time;
                        carQualification2.qualificationNumber = new int?(1);
                        carQualification2.UnqualifiedTimes = new int?(0);
                        carQualification2.Qualification = new double?((double)100);
                    }
                    else
                    {
                        int time = Convert.ToInt32(Convert.ToDateTime(tbOriginalRecord.time).Year.ToString());
                        carQualification2.year = time;
                        carQualification2.qualificationNumber = new int?(0);
                        carQualification2.UnqualifiedTimes = new int?(1);
                        carQualification2.Qualification = new double?(0.0);
                    }
                    this.dbEMCEntities.CarYearQualification.Add(carQualification2);
                    this.dbEMCEntities.SaveChanges();
                }
                path path = new path();
                path.OriginalRecordID = new int?(originalRecordID);
                if (base.Request.Cookies["PK_file_LH"] != null)
                {
                    path.AK_LH = base.Server.HtmlEncode(base.Request.Cookies["PK_file_LH"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AK_LH_qualified"].Value) == "0")
                    {
                        path.AK_LH_qualified = "不合格";
                    }
                    else
                    {
                        path.AK_LH_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["PK_file_LV"] != null)
                {
                    path.AK_LV = base.Server.HtmlEncode(base.Request.Cookies["PK_file_LV"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AK_LV_qualified"].Value) == "0")
                    {
                        path.AK_LV_qualified = "不合格";
                    }
                    else
                    {
                        path.AK_LV_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["PK_file_RH"] != null)
                {
                    path.AK_RH = base.Server.HtmlEncode(base.Request.Cookies["PK_file_RH"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AK_RH_qualified"].Value) == "0")
                    {
                        path.AK_RH_qualified = "不合格";
                    }
                    else
                    {
                        path.AK_RH_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["PK_file_RV"] != null)
                {
                    path.AK_RV = base.Server.HtmlEncode(base.Request.Cookies["PK_file_RV"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AK_RV_qualified"].Value) == "0")
                    {
                        path.AK_RV_qualified = "不合格";
                    }
                    else
                    {
                        path.AK_RV_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["fileCSJB"] != null)
                {
                    path.AV_LH = base.Server.HtmlEncode(base.Request.Cookies["fileCSJB"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AV_LH_qualified"].Value) == "0")
                    {
                        path.AV_LH_qualified = "不合格";
                    }
                    else
                    {
                        path.AV_LH_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["file_LV"] != null)
                {
                    path.AV_LV = base.Server.HtmlEncode(base.Request.Cookies["file_LV"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AV_LV_qualified"].Value) == "0")
                    {
                        path.AV_LV_qualified = "不合格";
                    }
                    else
                    {
                        path.AV_LV_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["file_RH"] != null)
                {
                    path.AV_RH = base.Server.HtmlEncode(base.Request.Cookies["file_RH"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AV_RH_qualified"].Value) == "0")
                    {
                        path.AV_RH_qualified = "不合格";
                    }
                    else
                    {
                        path.AV_RH_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["file_RV"] != null)
                {
                    path.AV_RV = base.Server.HtmlEncode(base.Request.Cookies["file_RV"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["AV_RV_qualified"].Value) == "0")
                    {
                        path.AV_RV_qualified = "不合格";
                    }
                    else
                    {
                        path.AV_RV_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["PK_file_LH2"] != null)
                {
                    path.AK_LH_img = base.Server.HtmlEncode(base.Request.Cookies["PK_file_LH2"].Value);
                }
                if (base.Request.Cookies["PK_file_LV2"] != null)
                {
                    path.AK_LV_img = base.Server.HtmlEncode(base.Request.Cookies["PK_file_LV2"].Value);
                }
                if (base.Request.Cookies["PK_file_RH2"] != null)
                {
                    path.AK_RH_img = base.Server.HtmlEncode(base.Request.Cookies["PK_file_RH2"].Value);
                }
                if (base.Request.Cookies["PK_file_RV2"] != null)
                {
                    path.AK_RV_img = base.Server.HtmlEncode(base.Request.Cookies["PK_file_RV2"].Value);
                }
                if (base.Request.Cookies["fileCSJB2"] != null)
                {
                    path.AV_LH_img = base.Server.HtmlEncode(base.Request.Cookies["fileCSJB2"].Value);
                }
                if (base.Request.Cookies["file_LV2"] != null)
                {
                    path.AV_LV_img = base.Server.HtmlEncode(base.Request.Cookies["file_LV2"].Value);
                }
                if (base.Request.Cookies["file_RH2"] != null)
                {
                    path.AV_RH_img = base.Server.HtmlEncode(base.Request.Cookies["file_RH2"].Value);
                }
                if (base.Request.Cookies["file_RV2"] != null)
                {
                    path.AV_RV_img = base.Server.HtmlEncode(base.Request.Cookies["file_RV2"].Value);
                }
                this.dbEMCEntities.path.Add(path);
                this.dbEMCEntities.SaveChanges();
                string[] array = RegistrationName.Split(new char[]
                {
                    '#'
                });
                for (int i = 0; i < array.Length - 1; i++)
                {
                    RegistrationPath registrationPath = new RegistrationPath();
                    registrationPath.OriginalRecordID = new int?(originalRecordID);
                    registrationPath.name = array[i].ToString().Trim();
                    registrationPath.Route = "/File/" + array[i].ToString().Trim();
                    this.dbEMCEntities.RegistrationPath.Add(registrationPath);
                    this.dbEMCEntities.SaveChanges();
                }
                string[] array2 = OriginalRecordName.Split(new char[]
                {
                    '#'
                });
                for (int j = 0; j < array2.Length - 1; j++)
                {
                    OriginalRecordPath originalRecordPath = new OriginalRecordPath();
                    originalRecordPath.OriginalRecordID = new int?(originalRecordID);
                    originalRecordPath.name = array2[j].ToString().Trim();
                    originalRecordPath.Route = "/File/" + array2[j].ToString().Trim();
                    this.dbEMCEntities.OriginalRecordPath.Add(originalRecordPath);
                    this.dbEMCEntities.SaveChanges();
                }
                return base.Json("成功", JsonRequestBehavior.AllowGet);
            }
            return base.Json("失败", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 18387
        /// </summary>
        /// <param name="tbOriginalRecord"></param>
        /// <param name="RegistrationName"></param>
        /// <param name="OriginalRecordName"></param>
        /// <returns></returns>
        public ActionResult InsertReportContent_18387(OriginalRecord tbOriginalRecord, string RegistrationName, string OriginalRecordName,string surface,string surface2)
        {
            OriginalRecord originalRecord = new OriginalRecord();
            originalRecord.taskNumber = tbOriginalRecord.taskNumber;
            originalRecord.time = tbOriginalRecord.time;
            originalRecord.brand = tbOriginalRecord.brand.ToString().Trim();
            originalRecord.introduce = tbOriginalRecord.introduce;
            originalRecord.ModelCarModel = tbOriginalRecord.ModelCarModel;
            originalRecord.Remarks = tbOriginalRecord.Remarks;
            originalRecord.ResultID = tbOriginalRecord.ResultID;
            originalRecord.taskAllName = tbOriginalRecord.taskAllName;
            originalRecord.VIN = tbOriginalRecord.VIN;
            originalRecord.category = "18387";
            this.dbEMCEntities.OriginalRecord.Add(originalRecord);
            this.dbEMCEntities.SaveChanges();
            int originalRecordID = originalRecord.OriginalRecordID;
            if (originalRecordID > 0)
            {
                string CarName = tbOriginalRecord.brand.ToString().Trim();
                var list = (from tbCT in this.dbEMCEntities.CarQualification
                            where tbCT.CarName == CarName
                            select new
                            {
                                tbCT.CarID,
                                tbCT.Qualification,
                                tbCT.qualificationNumber,
                                tbCT.UnqualifiedTimes
                            }).Distinct().ToList();
                if (list.Count > 0)
                {
                    ///车型合格率
                    int id = list[0].CarID;
                    if ((from tbReport in this.dbEMCEntities.CarQualification
                         where tbReport.CarID == id
                         select tbReport).ToList<CarQualification>().Count > 0)
                    {
                        CarQualification carQualification = this.dbEMCEntities.CarQualification.Find(new object[]
                        {
                            id
                        });
                        if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                        {
                            carQualification.qualificationNumber = list[0].qualificationNumber + 1;
                            double num = Convert.ToDouble(list[0].qualificationNumber + 1);
                            double num2 = Convert.ToDouble(list[0].UnqualifiedTimes);
                            CarQualification arg_3EF_0 = carQualification;
                            double expr_3D5 = num;
                            arg_3EF_0.Qualification = new double?(Math.Round(expr_3D5 / (expr_3D5 + num2) * 100.0, 2));
                        }
                        else
                        {
                            carQualification.UnqualifiedTimes = list[0].UnqualifiedTimes + 1;
                            double num3 = Convert.ToDouble(list[0].qualificationNumber);
                            double num4 = Convert.ToDouble(list[0].UnqualifiedTimes + 1);
                            CarQualification arg_4AC_0 = carQualification;
                            double expr_492 = num3;
                            arg_4AC_0.Qualification = new double?(Math.Round(expr_492 / (expr_492 + num4) * 100.0, 2));
                        }
                        this.dbEMCEntities.SaveChanges();
                    }
                }
                else
                {
                    CarQualification carQualification2 = new CarQualification();
                    carQualification2.CarName = CarName;
                    if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                    {
                        carQualification2.qualificationNumber = new int?(1);
                        carQualification2.UnqualifiedTimes = new int?(0);
                        carQualification2.Qualification = new double?((double)100);
                    }
                    else
                    {
                        carQualification2.qualificationNumber = new int?(0);
                        carQualification2.UnqualifiedTimes = new int?(1);
                        carQualification2.Qualification = new double?(0.0);
                    }
                    this.dbEMCEntities.CarQualification.Add(carQualification2);
                    this.dbEMCEntities.SaveChanges();
                }
                //年合格率
                int time2 = Convert.ToInt32(Convert.ToDateTime(tbOriginalRecord.time).Year.ToString());
                var list2 = (from tbCT in this.dbEMCEntities.CarYearQualification
                             where tbCT.carName == CarName && tbCT.year == time2
                             select new
                             {
                                 tbCT.CarYearQualificationID,
                                 tbCT.Qualification,
                                 tbCT.qualificationNumber,
                                 tbCT.UnqualifiedTimes,
                                 tbCT.year
                             }).Distinct().ToList();
                if (list2.Count > 0)
                {
                    int id = list2[0].CarYearQualificationID;

                    if ((from tbReport in dbEMCEntities.CarYearQualification
                         where tbReport.CarYearQualificationID == id
                         select tbReport).ToList<CarYearQualification>().Count > 0)
                    {
                        CarYearQualification carQualification = this.dbEMCEntities.CarYearQualification.Find(new object[]
                        {
                            id
                        });
                        if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                        {
                            carQualification.qualificationNumber = list2[0].qualificationNumber + 1;
                            double num = Convert.ToDouble(list2[0].qualificationNumber + 1);
                            double num2 = Convert.ToDouble(list2[0].UnqualifiedTimes);
                            CarYearQualification arg_3EF_0 = carQualification;
                            double expr_3D5 = num;
                            arg_3EF_0.Qualification = new double?(Math.Round(expr_3D5 / (expr_3D5 + num2) * 100, 2));
                        }
                        else
                        {
                            carQualification.UnqualifiedTimes = list2[0].UnqualifiedTimes + 1;
                            double num3 = Convert.ToDouble(list2[0].qualificationNumber);
                            double num4 = Convert.ToDouble(list2[0].UnqualifiedTimes + 1);
                            CarYearQualification arg_4AC_0 = carQualification;
                            double expr_492 = num3;
                            arg_4AC_0.Qualification = new double?(Math.Round(expr_492 / (expr_492 + num4) * 100, 2));
                        }
                        this.dbEMCEntities.SaveChanges();
                    }
                }
                else
                {
                    CarYearQualification carQualification2 = new CarYearQualification();
                    carQualification2.carName = CarName;
                    if (tbOriginalRecord.ResultID.ToString().Trim() == "合格")
                    {
                        int time = Convert.ToInt32(Convert.ToDateTime(tbOriginalRecord.time).Year.ToString());
                        carQualification2.year = time;
                        carQualification2.qualificationNumber = new int?(1);
                        carQualification2.UnqualifiedTimes = new int?(0);
                        carQualification2.Qualification = new double?((double)100);
                    }
                    else
                    {
                        int time = Convert.ToInt32(Convert.ToDateTime(tbOriginalRecord.time).Year.ToString());
                        carQualification2.year = time;
                        carQualification2.qualificationNumber = new int?(0);
                        carQualification2.UnqualifiedTimes = new int?(1);
                        carQualification2.Qualification = new double?(0.0);
                    }
                    this.dbEMCEntities.CarYearQualification.Add(carQualification2);
                    this.dbEMCEntities.SaveChanges();
                }
                path_18387 path = new path_18387();
                path.RODsurface = surface;
                path.LOOPsurface = surface2;
                path.OriginalRecordID = new int?(originalRecordID);
                #region //电场
                if (base.Request.Cookies["rod_16"] != null)
                {
                    path.ROD16 = base.Server.HtmlEncode(base.Request.Cookies["rod_16"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["rod_16_qualified"].Value) == "0")
                    {
                        path.ROD16_qualified = "不合格";
                    }
                    else
                    {
                        path.ROD16_qualified = "合格";
                    }
                }

                if (base.Request.Cookies["rod_70"] != null)
                {
                    path.ROD70 = base.Server.HtmlEncode(base.Request.Cookies["rod_70"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["rod_70_qualified"].Value) == "0")
                    {
                        path.ROD70_qualified = "不合格";
                    }
                    else
                    {
                        path.ROD70_qualified = "合格";
                    }
                }
                if (base.Request.Cookies["rod_img"] != null)
                {
                    path.RODsurface_img = base.Server.HtmlEncode(base.Request.Cookies["rod_img"].Value);
                }
                if (base.Request.Cookies["rod_16_Electrical"] != null)
                {
                    path.ROD16_img = base.Server.HtmlEncode(base.Request.Cookies["rod_16_Electrical"].Value);
                }
                if (base.Request.Cookies["rod_70_Electrical"] != null)
                {
                    path.ROD70_img = base.Server.HtmlEncode(base.Request.Cookies["rod_70_Electrical"].Value);
                }
                if (base.Request.Cookies["DUI_Electrical"] != null)
                {
                    path.ROD_DUI_img = base.Server.HtmlEncode(base.Request.Cookies["DUI_Electrical"].Value);
                }
                #endregion
                #region //磁场
                if (base.Request.Cookies["Magnetic_x_16"] != null)
                {
                    path.LOOP_16x = base.Server.HtmlEncode(base.Request.Cookies["Magnetic_x_16"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["Magnetic_x_16_qualified"].Value) == "0")
                    {
                        path.LOOP_16x_qualified = "不合格";
                    }
                    else
                    {
                        path.LOOP_16x_qualified = "合格";
                    }
  
                }
                if (base.Request.Cookies["Magnetic_x_70"] != null)
                {
                    path.LOOP_70x = base.Server.HtmlEncode(base.Request.Cookies["Magnetic_x_70"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["Magnetic_x_70_qualified"].Value) == "0")
                    {
                        path.LOOP_70x_qualified = "不合格";
                    }
                    else
                    {
                        path.LOOP_70x_qualified = "合格";
                    }

                }
                if (base.Request.Cookies["Magnetic_y_16"] != null)
                {
                    path.LOOP_16y = base.Server.HtmlEncode(base.Request.Cookies["Magnetic_y_16"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["Magnetic_y_16_qualified"].Value) == "0")
                    {
                        path.LOOP_16y_qualified = "不合格";
                    }
                    else
                    {
                        path.LOOP_16y_qualified = "合格";
                    }

                }
                if (base.Request.Cookies["Magnetic_y_70"] != null)
                {
                    path.LOOP_70y = base.Server.HtmlEncode(base.Request.Cookies["Magnetic_y_70"].Value);
                    if (base.Server.HtmlEncode(base.Request.Cookies["Magnetic_y_70_qualified"].Value) == "0")
                    {
                        path.LOOP_70y_qualified = "不合格";
                    }
                    else
                    {
                        path.LOOP_70y_qualified = "合格";
                    }

                }
                if (base.Request.Cookies["LOOP_16x_IMG"] != null)
                {
                    path.LOOP_16x_img = base.Server.HtmlEncode(base.Request.Cookies["LOOP_16x_IMG"].Value);
                }
                if (base.Request.Cookies["LOOP_70x_IMG"] != null)
                {
                    path.LOOP_70x_img = base.Server.HtmlEncode(base.Request.Cookies["LOOP_70x_IMG"].Value);
                }
                if (base.Request.Cookies["LOOP_16y_IMG"] != null)
                {
                    path.LOOP_16y_img = base.Server.HtmlEncode(base.Request.Cookies["LOOP_16y_IMG"].Value);
                }
                if (base.Request.Cookies["LOOP_70y_IMG"] != null)
                {
                    path.LOOP_70y_img = base.Server.HtmlEncode(base.Request.Cookies["LOOP_70y_IMG"].Value);
                }
                if (base.Request.Cookies["DUI_Magnetic"] != null)
                {
                    path.LOOP_DUI_img = base.Server.HtmlEncode(base.Request.Cookies["DUI_Magnetic"].Value);
                }
                if (base.Request.Cookies["Magnetic_img"] != null)
                {
                    path.LOOPsurface_img = base.Server.HtmlEncode(base.Request.Cookies["Magnetic_img"].Value);
                }
                #endregion
                this.dbEMCEntities.path_18387.Add(path);
                this.dbEMCEntities.SaveChanges();
                string[] array = RegistrationName.Split(new char[]
                {
                    '#'
                });
                for (int i = 0; i < array.Length - 1; i++)
                {
                    RegistrationPath registrationPath = new RegistrationPath();
                    registrationPath.OriginalRecordID = new int?(originalRecordID);
                    registrationPath.name = array[i].ToString().Trim();
                    registrationPath.Route = "/File/" + array[i].ToString().Trim();
                    this.dbEMCEntities.RegistrationPath.Add(registrationPath);
                    this.dbEMCEntities.SaveChanges();
                }
                string[] array2 = OriginalRecordName.Split(new char[]
                {
                    '#'
                });
                for (int j = 0; j < array2.Length - 1; j++)
                {
                    OriginalRecordPath originalRecordPath = new OriginalRecordPath();
                    originalRecordPath.OriginalRecordID = new int?(originalRecordID);
                    originalRecordPath.name = array2[j].ToString().Trim();
                    originalRecordPath.Route = "/File/" + array2[j].ToString().Trim();
                    this.dbEMCEntities.OriginalRecordPath.Add(originalRecordPath);
                    this.dbEMCEntities.SaveChanges();
                }
                return base.Json("成功", JsonRequestBehavior.AllowGet);
            }
            return base.Json("失败", JsonRequestBehavior.AllowGet);
        }
        // Token: 0x06000115 RID: 277 RVA: 0x00005500 File Offset: 0x00003700
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string fileName = file.FileName;
            string path = base.Server.MapPath(string.Format("~/{0}", "File"));
            file.SaveAs(Path.Combine(path, fileName));
            return Content("<script>alert('上传成功');history.go(-1);</script>");
        }

        // Token: 0x06000116 RID: 278 RVA: 0x00005547 File Offset: 0x00003747
        public ActionResult updateReport(int id)
        {
            base.Session["OriginalRecordID"] = id;
            return base.View();
        }

        // Token: 0x06000117 RID: 279 RVA: 0x00005568 File Offset: 0x00003768
        public ActionResult selectupdateReport()
        {
            int id = (int)base.Session["OriginalRecordID"];
            #region 14023
            var result2 = (from tbCT in this.dbEMCEntities.OriginalRecord
                           join tbPath in this.dbEMCEntities.path on (int?)tbCT.OriginalRecordID equals tbPath.OriginalRecordID
                           where tbCT.OriginalRecordID == id
                           select new
                           {
                               brand = tbCT.brand.ToString().Trim(),
                               taskNumber = tbCT.taskNumber.ToString().Trim(),
                               introduce = tbCT.introduce.ToString().Trim(),
                               ModelCarModel = tbCT.ModelCarModel.ToString().Trim(),
                               OriginalRecordID = tbCT.OriginalRecordID,
                               Remarks = tbCT.Remarks.ToString().Trim(),
                               ResultID = tbCT.ResultID,
                               taskAllName = tbCT.taskAllName,
                               VIN = tbCT.VIN.ToString().Trim(),
                               time = tbCT.time.ToString().Trim(),
                               AK_LH = tbPath.AK_LH.ToString().Trim(),
                               AK_LV = tbPath.AK_LV.ToString().Trim(),
                               AK_RH = tbPath.AK_RH.ToString().Trim(),
                               AK_RV = tbPath.AK_RV.ToString().Trim(),
                               AV_LH = tbPath.AV_LH.ToString().Trim(),
                               AV_LV = tbPath.AV_LV.ToString().Trim(),
                               AV_RH = tbPath.AV_RH.ToString().Trim(),
                               AV_RV = tbPath.AV_RV.ToString().Trim(),
                               AK_LH_img = tbPath.AK_LH_img.ToString().Trim(),
                               AK_LV_img = tbPath.AK_LV_img.ToString().Trim(),
                               AK_RH_img = tbPath.AK_RH_img.ToString().Trim(),
                               AK_RV_img = tbPath.AK_RV_img.ToString().Trim(),
                               AV_LH_img = tbPath.AV_LH_img.ToString().Trim(),
                               AV_LV_img = tbPath.AV_LV_img.ToString().Trim(),
                               AV_RH_img = tbPath.AV_RH_img.ToString().Trim(),
                               AV_RV_img = tbPath.AV_RV_img.ToString().Trim(),
                               Difference = "14023"
                           }).Distinct().ToList();
            if (result2.Count <= 0)
            {
                var result3 = (from tbCT in this.dbEMCEntities.OriginalRecord
                               join tbPath in this.dbEMCEntities.path_18387 on (int?)tbCT.OriginalRecordID equals tbPath.OriginalRecordID
                               where tbCT.OriginalRecordID == id
                               select new
                               {
                                   brand = tbCT.brand.ToString().Trim(),
                                   taskNumber = tbCT.taskNumber.ToString().Trim(),
                                   introduce = tbCT.introduce.ToString().Trim(),
                                   ModelCarModel = tbCT.ModelCarModel.ToString().Trim(),
                                   OriginalRecordID = tbCT.OriginalRecordID,
                                   Remarks = tbCT.Remarks.ToString().Trim(),
                                   ResultID = tbCT.ResultID,
                                   taskAllName = tbCT.taskAllName,
                                   VIN = tbCT.VIN.ToString().Trim(),
                                   time = tbCT.time.ToString().Trim(),
                                   LOOPsurface= tbPath.LOOPsurface,
                                   RODsurface=tbPath.RODsurface,
                                   ROD16 =tbPath.ROD16,
                                   ROD16_img=tbPath.ROD16_img,
                                   ROD16_qualified=tbPath.ROD16_qualified,
                                   ROD70=tbPath.ROD70,
                                   ROD70_img=tbPath.ROD70_img,
                                   ROD70_qualified=tbPath.ROD70_qualified,
                                   ROD_DUI_img=tbPath.ROD_DUI_img,
                                   LOOP_16x=tbPath.LOOP_16x,
                                   LOOP_16x_img=tbPath.LOOP_16x_img,
                                   LOOP_16x_qualified=tbPath.LOOP_16x_qualified,
                                   LOOP_16y=tbPath.LOOP_16y,
                                   LOOP_16y_img=tbPath.LOOP_16y_img,
                                   LOOP_16y_qualified= tbPath.LOOP_16y_qualified,
                                   LOOP_70x=tbPath.LOOP_70x,
                                   LOOP_70x_img=tbPath.LOOP_70x_img,
                                   LOOP_70x_qualified=tbPath.LOOP_70x_qualified,
                                   LOOP_70y= tbPath.LOOP_70y,
                                   LOOP_70y_img=tbPath.LOOP_70y_img,
                                   LOOP_70y_qualified=tbPath.LOOP_70y_qualified,
                                   LOOP_DUI_img=tbPath.LOOP_DUI_img,
                                   Difference="18387",
                                   RODsurface_img=tbPath.RODsurface_img,
                                   LOOPsurface_img= tbPath.LOOPsurface_img,
                               }).Distinct().ToList();
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (var one in result3.ToList())
                {

                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    dictionary.Add("RODsurface_img", one.RODsurface_img);
                    dictionary.Add("LOOPsurface_img", one.LOOPsurface_img);
                    dictionary.Add("LOOPsurface", one.LOOPsurface);
                    dictionary.Add("RODsurface", one.RODsurface);
                    dictionary.Add("brand", one.brand);
                    dictionary.Add("taskNumber", one.taskNumber);
                    dictionary.Add("ROD16", one.ROD16);
                    dictionary.Add("ROD16_img", one.ROD16_img);
                    dictionary.Add("ROD16_qualified", one.ROD16_qualified);
                    dictionary.Add("ROD70", one.ROD70);
                    dictionary.Add("ROD70_img", one.ROD70_img);
                    dictionary.Add("ROD70_qualified", one.ROD70_qualified);
                    dictionary.Add("ROD_DUI_img", one.ROD_DUI_img);
                    dictionary.Add("LOOP_16x", one.LOOP_16x);
                    dictionary.Add("LOOP_16x_img", one.LOOP_16x_img);
                    dictionary.Add("LOOP_16x_qualified", one.LOOP_16x_qualified);
                    dictionary.Add("LOOP_16y", one.LOOP_16y);
                    dictionary.Add("LOOP_16y_img", one.LOOP_16y_img);
                    dictionary.Add("LOOP_16y_qualified", one.LOOP_16y_qualified);
                    dictionary.Add("LOOP_70x", one.LOOP_70x);
                    dictionary.Add("LOOP_70x_img", one.LOOP_70x_img);
                    dictionary.Add("LOOP_70x_qualified", one.LOOP_70x_qualified);
                    dictionary.Add("LOOP_70y", one.LOOP_70y);
                    dictionary.Add("LOOP_70y_img", one.LOOP_70y_img);
                    dictionary.Add("LOOP_70y_qualified", one.LOOP_70y_qualified);
                    dictionary.Add("LOOP_DUI_img", one.LOOP_DUI_img);
                    dictionary.Add("ModelCarModel", one.ModelCarModel);
                    dictionary.Add("OriginalRecordID", one.OriginalRecordID);
                    dictionary.Add("Remarks", one.Remarks);
                    dictionary.Add("ResultID", one.ResultID);
                    dictionary.Add("taskAllName", one.taskAllName);
                    dictionary.Add("VIN", one.VIN);
                    dictionary.Add("introduce", one.introduce);
                    dictionary.Add("time", one.time);
                    dictionary.Add("Difference", one.Difference);
                    var arg_12DA_0 = (from tbCT in this.dbEMCEntities.OriginalRecordPath
                                      where tbCT.OriginalRecordID == (int?)one.OriginalRecordID
                                      select new
                                      {
                                          name = tbCT.name.ToString().Trim(),
                                          Route = tbCT.Route.ToString().Trim(),
                                          OriginalRouteID = tbCT.OriginalRouteID
                                      }).Distinct().ToList();
                    string text = "";
                    foreach (var current in arg_12DA_0.ToList())
                    {
                        text = text + current.name + " ";
                    }
                    dictionary.Add("OriginalRecordName", text);
                    var arg_1517_0 = (from tbCT in this.dbEMCEntities.RegistrationPath
                                      where tbCT.OriginalRecordID == (int?)one.OriginalRecordID
                                      select new
                                      {
                                          name = tbCT.name.ToString().Trim(),
                                          Route = tbCT.Route.ToString().Trim(),
                                          OriginalRouteID = tbCT.RegistrationRouteID
                                      }).Distinct().ToList();
                    string text2 = "";
                    foreach (var current2 in arg_1517_0.ToList())
                    {
                        text2 = text2 + current2.name + " ";
                    }
                    dictionary.Add("RegistrationName", text2);
                    list.Add(dictionary);

                }
                return base.Json(list, JsonRequestBehavior.AllowGet);
            }
            else {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (var one in result2.ToList())
                {

                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                   
                    dictionary.Add("brand", one.brand);
                    dictionary.Add("taskNumber", one.taskNumber);
                    dictionary.Add("AK_LH", one.AK_LH);
                    dictionary.Add("AK_LV", one.AK_LV);
                    dictionary.Add("AK_RH", one.AK_RH);
                    dictionary.Add("AK_RV", one.AK_RV);
                    dictionary.Add("AV_LH", one.AV_LH);
                    dictionary.Add("AV_LV", one.AV_LV);
                    dictionary.Add("AV_RH", one.AV_RH);
                    dictionary.Add("AV_RV", one.AV_RV);
                    dictionary.Add("AK_LH_img", one.AK_LH_img);
                    dictionary.Add("AK_LV_img", one.AK_LV_img);
                    dictionary.Add("AK_RH_img", one.AK_RH_img);
                    dictionary.Add("AK_RV_img", one.AK_RV_img);
                    dictionary.Add("AV_LH_img", one.AV_LH_img);
                    dictionary.Add("AV_LV_img", one.AV_LV_img);
                    dictionary.Add("AV_RH_img", one.AV_RH_img);
                    dictionary.Add("AV_RV_img", one.AV_RV_img);
                    dictionary.Add("ModelCarModel", one.ModelCarModel);
                    dictionary.Add("OriginalRecordID", one.OriginalRecordID);
                    dictionary.Add("Remarks", one.Remarks);
                    dictionary.Add("ResultID", one.ResultID);
                    dictionary.Add("taskAllName", one.taskAllName);
                    dictionary.Add("VIN", one.VIN);
                    dictionary.Add("introduce", one.introduce);
                    dictionary.Add("time", one.time);
                    dictionary.Add("Difference", one.Difference);
                    var arg_12DA_0 = (from tbCT in this.dbEMCEntities.OriginalRecordPath
                                      where tbCT.OriginalRecordID == (int?)one.OriginalRecordID
                                      select new
                                      {
                                          name = tbCT.name.ToString().Trim(),
                                          Route = tbCT.Route.ToString().Trim(),
                                          OriginalRouteID = tbCT.OriginalRouteID
                                      }).Distinct().ToList();
                    string text = "";
                    foreach (var current in arg_12DA_0.ToList())
                    {
                        text = text + current.name + " ";
                    }
                    dictionary.Add("OriginalRecordName", text);
                    var arg_1517_0 = (from tbCT in this.dbEMCEntities.RegistrationPath
                                      where tbCT.OriginalRecordID == (int?)one.OriginalRecordID
                                      select new
                                      {
                                          name = tbCT.name.ToString().Trim(),
                                          Route = tbCT.Route.ToString().Trim(),
                                          OriginalRouteID = tbCT.RegistrationRouteID
                                      }).Distinct().ToList();
                    string text2 = "";
                    foreach (var current2 in arg_1517_0.ToList())
                    {
                        text2 = text2 + current2.name + " ";
                    }
                    dictionary.Add("RegistrationName", text2);
                    list.Add(dictionary);

                }
                return base.Json(list, JsonRequestBehavior.AllowGet);
            }
           
            #endregion
            #region 18387

            #endregion
            return base.Json("", JsonRequestBehavior.AllowGet);
        }

        // Token: 0x06000118 RID: 280 RVA: 0x00006B58 File Offset: 0x00004D58
        public ActionResult DelectReport(int OriginalRecordID)
        {
            ActionResult result;
           
                var entity = (from tbR in this.dbEMCEntities.OriginalRecord
                                         where tbR.OriginalRecordID == OriginalRecordID
                                         select tbR).ToList().Single();
               
               
                string name = entity.brand.ToString().Trim();
            var entity2 = dbEMCEntities.CarQualification.First(c => c.CarName == name);

           
            if (entity.ResultID.ToString().Trim() == "合格")
                {
                   
                    double num = Convert.ToDouble(entity2.qualificationNumber -1);
                    double num2 = Convert.ToDouble(entity2.UnqualifiedTimes);
                entity2.qualificationNumber = entity2.qualificationNumber - 1;
                entity2.Qualification = Math.Round(num / (num + num2) * 100.0, 2);
                dbEMCEntities.SaveChanges();
            }
                else {
                   
                    double num = Convert.ToDouble(entity2.UnqualifiedTimes - 1);
                    double num2 = Convert.ToDouble(entity2.qualificationNumber);
                entity2.UnqualifiedTimes = entity2.UnqualifiedTimes - 1;
                entity2.Qualification = Math.Round(num2 / (num + num2) * 100.0, 2);
                dbEMCEntities.SaveChanges();
            }
                dbEMCEntities.OriginalRecord.Remove(entity);
                dbEMCEntities.SaveChanges();
                result = base.Json("Yes", JsonRequestBehavior.AllowGet);
              return result;
        }
            
          
   

        // Token: 0x06000119 RID: 281 RVA: 0x00006C38 File Offset: 0x00004E38
        public ActionResult UpdataReport(OriginalRecord tbOriginalRecord, string RegistrationName, string OriginalRecordName)
        {
            int id = (int)base.Session["OriginalRecordID"];
            if ((from tbReport in this.dbEMCEntities.OriginalRecord
                 where tbReport.OriginalRecordID == id
                 select tbReport).ToList<OriginalRecord>().Count > 0)
            {
                OriginalRecord expr_C2 = this.dbEMCEntities.OriginalRecord.Find(new object[]
                {
                    id
                });
                expr_C2.taskNumber = tbOriginalRecord.taskNumber;
                expr_C2.time = new DateTime?(DateTime.Now);
                expr_C2.brand = tbOriginalRecord.brand;
                expr_C2.introduce = tbOriginalRecord.introduce;
                expr_C2.ModelCarModel = tbOriginalRecord.ModelCarModel;
                expr_C2.Remarks = tbOriginalRecord.Remarks;
                expr_C2.ResultID = tbOriginalRecord.ResultID;
                expr_C2.taskAllName = tbOriginalRecord.taskAllName;
                expr_C2.VIN = tbOriginalRecord.VIN;
                this.dbEMCEntities.SaveChanges();
                if (RegistrationName != "")
                {
                    foreach (RegistrationPath current in (from table1 in this.dbEMCEntities.RegistrationPath
                                                          where table1.OriginalRecordID == (int?)id
                                                          select table1).ToList<RegistrationPath>())
                    {
                        this.dbEMCEntities.Entry<RegistrationPath>(current).State = EntityState.Deleted;
                    }
                    string[] array = RegistrationName.Split(new char[]
                    {
                        '#'
                    });
                    for (int i = 0; i < array.Length - 1; i++)
                    {
                        RegistrationPath registrationPath = new RegistrationPath();
                        registrationPath.OriginalRecordID = new int?(id);
                        registrationPath.name = array[i].ToString().Trim();
                        registrationPath.Route = "/File/" + array[i].ToString().Trim();
                        this.dbEMCEntities.RegistrationPath.Add(registrationPath);
                        this.dbEMCEntities.SaveChanges();
                    }
                }
                if (OriginalRecordName != "")
                {
                    foreach (OriginalRecordPath current2 in (from table1 in this.dbEMCEntities.OriginalRecordPath
                                                             where table1.OriginalRecordID == (int?)id
                                                             select table1).ToList<OriginalRecordPath>())
                    {
                        this.dbEMCEntities.Entry<OriginalRecordPath>(current2).State = EntityState.Deleted;
                    }
                    string[] array2 = OriginalRecordName.Split(new char[]
                    {
                        '#'
                    });
                    for (int j = 0; j < array2.Length - 1; j++)
                    {
                        OriginalRecordPath originalRecordPath = new OriginalRecordPath();
                        originalRecordPath.OriginalRecordID = new int?(id);
                        originalRecordPath.name = array2[j].ToString().Trim();
                        originalRecordPath.Route = "/File/" + array2[j].ToString().Trim();
                        this.dbEMCEntities.OriginalRecordPath.Add(originalRecordPath);
                        this.dbEMCEntities.SaveChanges();
                    }
                }
                return base.Json("Yes", JsonRequestBehavior.AllowGet);
            }
            return base.Json("No", JsonRequestBehavior.AllowGet);
        }

        // Token: 0x0600011A RID: 282 RVA: 0x0000707C File Offset: 0x0000527C
        [HttpPost]
        public ActionResult StationImport(HttpPostedFileBase filebase)
        {
            string Time = "";
            string item = "合格";
            string filename_p = "";
            string FileName;
            string savePath;

            HttpPostedFileBase file =Request.Files["files"];
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";
                return View();
            }
            else
            {
                string filename = Path.GetFileName(file.FileName);
                int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                int Maxsize = 40000 * 1024;//定义上传文件的最大空间大小为4M
                string FileType = ".xls,.xlsx";//定义上传文件的类型字符串
                Time = DateTime.Now.ToString("yyyyMMddhhmmss");
                FileName = Time+ NoFileName + fileEx;
                filename_p= NoFileName + fileEx;
                if (!FileType.Contains(fileEx))
                {
                    ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                    return View();
                }
                if (filesize >= Maxsize)
                {
                    ViewBag.error = "上传文件超过4M，不能上传";
                    return View();
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + "file/";
                savePath = Path.Combine(path, FileName);
                file.SaveAs(savePath);
            }

         
            //int contentLength = httpPostedFileBase.ContentLength;
            //string extension = Path.GetExtension(fileName);
            //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            //int num = 40960000;
            //string arg_E9_0 = ".xls,.xlsx";
            //text = DateTime.Now.ToString("yyyyMMddhhmmss");
            //string path = text + fileNameWithoutExtension + extension;
           
            //string text2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "File/", path);
            //httpPostedFileBase.SaveAs(text2);
            string text3 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + savePath + ";Extended Properties=Excel 8.0";
            new OleDbConnection(text3).Open();
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("select * from [Sheet1$]", text3);
            DataSet dataSet = new DataSet();
            try
            {
                oleDbDataAdapter.Fill(dataSet, "ExcelInfo");
            }
            catch (Exception ex)
            {

            }
            DataTable dataTable = dataSet.Tables["ExcelInfo"].DefaultView.ToTable();
            List<object> list = new List<object>();
           
            using (TransactionScope transactionScope = new TransactionScope())
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    List<double> list2 = new List<double>();
                    string text4 = dataTable.Rows[i][0].ToString();
                    bool b = Regex.IsMatch(text4, @"[^ 0 - 9.-]");
                    if (text4 != ""&& b == true)
                    {
                        list2.Add(Convert.ToDouble(dataTable.Rows[i][0].ToString().Trim()));
                        list2.Add(Convert.ToDouble(dataTable.Rows[i][1].ToString().Trim()));
                        list.Add(list2);
                        string text5 = dataTable.Rows[i][1].ToString();
                        if (Convert.ToDouble(text4) <= 230.0)
                        {
                            if (text5 != "" && Convert.ToDouble(text5) > 28.0)
                            {
                                item = "不合格";
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text5) > 35.0)
                        {
                            item = "不合格";
                        }
                    }
                }
                transactionScope.Complete();
            }
            list.Add(filename_p);
            list.Add(item);
            list.Add(Time);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 18387电场
        /// </summary>
        /// <param name="filebase"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StationImport_18387Electrical(HttpPostedFileBase filebase)
        {
            string Time = "";
            string item = "合格";
            string filename_p = "";
            string FileName;
            string savePath;

            HttpPostedFileBase file = Request.Files["files"];
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";
                return View();
            }
            else
            {
                string filename = Path.GetFileName(file.FileName);
                int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                int Maxsize = 40000 * 1024;//定义上传文件的最大空间大小为4M
                string FileType = ".xls,.xlsx";//定义上传文件的类型字符串
                Time = DateTime.Now.ToString("yyyyMMddhhmmss");
                FileName = Time + NoFileName + fileEx;
                filename_p = NoFileName + fileEx;
                if (!FileType.Contains(fileEx))
                {
                    ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                    return View();
                }
                if (filesize >= Maxsize)
                {
                    ViewBag.error = "上传文件超过4M，不能上传";
                    return View();
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + "file/";
                savePath = Path.Combine(path, FileName);
                file.SaveAs(savePath);
            }


           
            string text3 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + savePath + ";Extended Properties=Excel 8.0";
            new OleDbConnection(text3).Open();
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("select * from [Sheet1$]", text3);
            DataSet dataSet = new DataSet();
            try
            {
                oleDbDataAdapter.Fill(dataSet, "ExcelInfo");
            }
            catch (Exception ex)
            {

            }
            DataTable dataTable = dataSet.Tables["ExcelInfo"].DefaultView.ToTable();
            List<object> list = new List<object>();

            using (TransactionScope transactionScope = new TransactionScope())
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                  //  List<double> list2 = new List<double>();
                    string text4 = dataTable.Rows[i][0].ToString();
                    bool b = Regex.IsMatch(text4, @"[^ 0 - 9.-]");
                    if (text4 != "" && b == true)
                    {
                      
                       
                        string text5 = dataTable.Rows[i][1].ToString();
                        if (Convert.ToDouble(text4) >= 0.15 && Convert.ToDouble(text4) <4.77)
                        {
                            double num2 = Convert.ToDouble(text4);

                            if (text5 != "")
                            {
                                if (Math.Round( 88.89 - 20 * Math.Log10(num2 )) < Convert.ToDouble(text5))
                                {
                                    item = "不合格";
                                    break;
                                }
                               
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text4) >= 4.77 && Convert.ToDouble(text4) < 15.92)
                        {
                            double num2 = Convert.ToDouble(text4);
                            if (Math.Round(116.05 - 60 * Math.Log10(num2)) < Convert.ToDouble(text5))
                            {
                                item = "不合格";
                                break;
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text4) >= 15.92 && Convert.ToDouble(text4) < 20)
                        {
                            double num2 = Convert.ToDouble(text4);
                            if (Math.Round(67.98 - 20 * Math.Log10(num2)) < Convert.ToDouble(text5))
                            {
                                item = "不合格";
                                break;
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text4) >= 20 && Convert.ToDouble(text4) <= 30)
                        {
                            double num2 = Convert.ToDouble(text4);
                            if (41.96 < Convert.ToDouble(text5))
                            {
                                item = "不合格";
                                break;
                            }
                        }
                    }
                }
                transactionScope.Complete();
            }
            list.Add(filename_p);
            list.Add(item);
            list.Add(Time);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StationImport_18387Magnetic(HttpPostedFileBase filebase)
        {
            string Time = "";
            string item = "合格";
            string filename_p = "";
            string FileName;
            string savePath;

            HttpPostedFileBase file = Request.Files["files"];
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";
                return View();
            }
            else
            {
                string filename = Path.GetFileName(file.FileName);
                int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                int Maxsize = 40000 * 1024;//定义上传文件的最大空间大小为4M
                string FileType = ".xls,.xlsx";//定义上传文件的类型字符串
                Time = DateTime.Now.ToString("yyyyMMddhhmmss");
                FileName = Time + NoFileName + fileEx;
                filename_p = NoFileName + fileEx;
                if (!FileType.Contains(fileEx))
                {
                    ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                    return View();
                }
                if (filesize >= Maxsize)
                {
                    ViewBag.error = "上传文件超过4M，不能上传";
                    return View();
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + "file/";
                savePath = Path.Combine(path, FileName);
                file.SaveAs(savePath);
            }



            string text3 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + savePath + ";Extended Properties=Excel 8.0";
            new OleDbConnection(text3).Open();
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("select * from [Sheet1$]", text3);
            DataSet dataSet = new DataSet();
            try
            {
                oleDbDataAdapter.Fill(dataSet, "ExcelInfo");
            }
            catch (Exception ex)
            {

            }
            DataTable dataTable = dataSet.Tables["ExcelInfo"].DefaultView.ToTable();
            List<object> list = new List<object>();

            using (TransactionScope transactionScope = new TransactionScope())
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    //  List<double> list2 = new List<double>();
                    string text4 = dataTable.Rows[i][0].ToString();
                    bool b = Regex.IsMatch(text4, @"[^ 0 - 9.-]");
                    if (text4 != "" && b == true)
                    {


                        string text5 = dataTable.Rows[i][1].ToString();
                        if (Convert.ToDouble(text4) >= 0.15 && Convert.ToDouble(text4) < 4.77)
                        {
                            double num2 = Convert.ToDouble(text4);

                            if (text5 != "")
                            {
                                if (Math.Round(37.36 - 20 * Math.Log10(num2)) < Convert.ToDouble(text5))
                                {
                                    item = "不合格";
                                    break;
                                }

                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text4) >= 4.77 && Convert.ToDouble(text4) < 15.92)
                        {
                            double num2 = Convert.ToDouble(text4);
                            if (Math.Round(64.52 - 60 * Math.Log10(num2)) < Convert.ToDouble(text5))
                            {
                                item = "不合格";
                                break;
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text4) >= 15.92 && Convert.ToDouble(text4) < 20)
                        {
                            double num2 = Convert.ToDouble(text4);
                            if (Math.Round(16.45 - 20 * Math.Log10(num2)) < Convert.ToDouble(text5))
                            {
                                item = "不合格";
                                break;
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text4) >= 20 && Convert.ToDouble(text4) <= 30)
                        {
                            double num2 = Convert.ToDouble(text4);
                            if (-9.57 < Convert.ToDouble(text5))
                            {
                                item = "不合格";
                                break;
                            }
                        }
                    }
                }
                transactionScope.Complete();
            }
            list.Add(filename_p);
            list.Add(item);
            list.Add(Time);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        // Token: 0x0600011B RID: 283 RVA: 0x000074E4 File Offset: 0x000056E4
        [HttpPost]
        public ActionResult StationImport2(HttpPostedFileBase filebase)
        {
            HttpPostedFileBase httpPostedFileBase = base.Request.Files["files"];
            if (httpPostedFileBase == null || httpPostedFileBase.ContentLength <= 0)
            {
                return base.Json("文件不能为空", JsonRequestBehavior.AllowGet);
            }
            string fileName = Path.GetFileName(httpPostedFileBase.FileName);
            int arg_48_0 = httpPostedFileBase.ContentLength;
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string text = DateTime.Now.ToString("yyyyMMddhhmmss");
            string path = text + fileNameWithoutExtension + extension;
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "File/", path);
            httpPostedFileBase.SaveAs(filename);
            return base.Json(path, JsonRequestBehavior.AllowGet);
        }

        // Token: 0x0600011C RID: 284 RVA: 0x000075A0 File Offset: 0x000057A0
        [HttpPost]
        public ActionResult StationImport3(HttpPostedFileBase filebase)
        {
            string text = "";
            string item = "合格";
            string filename_p = "";
            HttpPostedFileBase httpPostedFileBase = base.Request.Files["files"];
            //if (httpPostedFileBase == null || httpPostedFileBase.ContentLength <= 0)
            //{
            //    if (ReportController.<> o__12.<> p__0 == null)
            //    {
            //        ReportController.<> o__12.<> p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "error", typeof(ReportController), new CSharpArgumentInfo[]
            //        {
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
            //        }));
            //    }
            //    ReportController.<> o__12.<> p__0.Target(ReportController.<> o__12.<> p__0, base.ViewBag, "文件不能为空");
            //    return base.Json("文件不能为空", JsonRequestBehavior.AllowGet);
            //}
            string fileName = Path.GetFileName(httpPostedFileBase.FileName);
            int contentLength = httpPostedFileBase.ContentLength;
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            int num = 40960000;
            string arg_EF_0 = ".xls,.xlsx";
            text = DateTime.Now.ToString("yyyyMMddhhmmss");
            string path = text + fileNameWithoutExtension + extension;
            filename_p = fileNameWithoutExtension + extension;
            string text2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "File/", path);
            httpPostedFileBase.SaveAs(text2);
            string text3 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + text2 + ";Extended Properties=Excel 8.0";
            new OleDbConnection(text3).Open();
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("select * from [Sheet1$]", text3);
            DataSet dataSet = new DataSet();
            try
            {
                oleDbDataAdapter.Fill(dataSet, "ExcelInfo");
            }
            catch (Exception ex)
            {
               
            }
            DataTable dataTable = dataSet.Tables["ExcelInfo"].DefaultView.ToTable();
            List<object> list = new List<object>();
            using (TransactionScope transactionScope = new TransactionScope())
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    List<double> list2 = new List<double>();
                    string text4 = dataTable.Rows[i][0].ToString();
                    bool b = Regex.IsMatch(text4, @"[^ 0 - 9.-]");
                    if (text4 != ""  && b == true )
                    {
                        list2.Add(Convert.ToDouble(dataTable.Rows[i][0].ToString().Trim()));
                        list2.Add(Convert.ToDouble(dataTable.Rows[i][1].ToString().Trim()));
                        list.Add(list2);
                        string text5 = dataTable.Rows[i][1].ToString();
                        if (Convert.ToDouble(text4) <= 75.0)
                        {
                            if (text5 != "" && Convert.ToDouble(text5) > 52.0)
                            {
                                item = "不合格";
                                break;
                            }
                        }
                        else if (Convert.ToDouble(text4) > 75.0 && Convert.ToDouble(text4) < 400.0)
                        {
                            if (text5 != "")
                            {
                                double num2 = Convert.ToDouble(text4);
                                double num3 = 75.0;
                                if (52.0 + 15.13 * Math.Log10(num2 / num3) < Convert.ToDouble(text5))
                                {
                                    item = "不合格";
                                    break;
                                }
                            }
                        }
                        else if (text5 != "" && Convert.ToDouble(text5) > 63.0)
                        {
                            item = "不合格";
                            break;
                        }
                    }
                }
                transactionScope.Complete();
            }
            list.Add(filename_p);
            list.Add(item);
            list.Add(text);
            return base.Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult selectTree()
        {
            var entity = (from tbR in this.dbEMCEntities.taskDetails
                        
                          select new {
                              id=tbR.taskDetailsID,
                              pId=tbR.taskID,
                              name=tbR.taskDetailsName.ToString().Trim(),
                              open=true

                          }).ToList();

            return Json(entity,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 修改树形
        /// </summary>
        /// <returns></returns>
        public ActionResult selectTree_update(string allId)
        {
            var entity = (from tbR in this.dbEMCEntities.taskDetails

                          select new
                          {
                              id = tbR.taskDetailsID,
                              pId = tbR.taskID,
                              name = tbR.taskDetailsName.ToString().Trim(),
                              open = true
                          }).ToList();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
          string [] arr = allId.Split(',');
            foreach (var one in entity.ToList())
            {
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("id", one.id);
                p.Add("pId", one.pId);
                p.Add("name", one.name);
                p.Add("open", one.open);
                if (Array.IndexOf(arr, one.id.ToString()) == -1)//不存在
                {

                }
                else//存在
                {
                    p.Add("checked", true);
                }
                list.Add(p);
            }
                return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
