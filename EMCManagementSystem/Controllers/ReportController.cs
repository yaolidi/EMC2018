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

namespace EMCManagementSystem.Controllers
{
    // Token: 0x0200001B RID: 27
    public class ReportController : Controller
    {
        // Token: 0x06000111 RID: 273 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Index()
        {
            return base.View();
        }
        Models.EMCEntities dbEMCEntities = new EMCEntities();
        // Token: 0x06000112 RID: 274 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult InsertReport()
        {
            return base.View();
        }

        // Token: 0x06000113 RID: 275 RVA: 0x000043D0 File Offset: 0x000025D0
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
                            time = tbCT.time.ToString().Trim()
                        }).Distinct().ToList();
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        // Token: 0x06000114 RID: 276 RVA: 0x00004790 File Offset: 0x00002990
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
                               AV_RV_img = tbPath.AV_RV_img.ToString().Trim()
                           }).Distinct().ToList();
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
            string FileName;
            string savePath;

            HttpPostedFileBase file = base.Request.Files["files"];
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
                    if (text4 != "")
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
            list.Add(item);
            list.Add(Time);
            return base.Json(list, JsonRequestBehavior.AllowGet);
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
            return base.Json("上传成功," + text, JsonRequestBehavior.AllowGet);
        }

        // Token: 0x0600011C RID: 284 RVA: 0x000075A0 File Offset: 0x000057A0
        [HttpPost]
        public ActionResult StationImport3(HttpPostedFileBase filebase)
        {
            string text = "";
            string item = "合格";
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
            //if (!arg_EF_0.Contains(extension))
            //{
            //    if (ReportController.<> o__12.<> p__1 == null)
            //    {
            //        ReportController.<> o__12.<> p__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "error", typeof(ReportController), new CSharpArgumentInfo[]
            //        {
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
            //        }));
            //    }
            //    ReportController.<> o__12.<> p__1.Target(ReportController.<> o__12.<> p__1, base.ViewBag, "文件类型不对，只能导入xls和xlsx格式的文件");
            //    return base.Json("文件类型不对", JsonRequestBehavior.AllowGet);
            //}
            //if (contentLength >= num)
            //{
            //    if (ReportController.<> o__12.<> p__2 == null)
            //    {
            //        ReportController.<> o__12.<> p__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "error", typeof(ReportController), new CSharpArgumentInfo[]
            //        {
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
            //        }));
            //    }
            //    ReportController.<> o__12.<> p__2.Target(ReportController.<> o__12.<> p__2, base.ViewBag, "上传文件超过40M，不能上传");
            //    return base.Json("超出大小", JsonRequestBehavior.AllowGet);
            //}
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
                    if (text4 != "")
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
