
using EMCManagementSystem.Models;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EMCManagementSystem.Controllers
{
    public class SystemController : Controller
    {        
        /// <summary>
        /// 时间：2017/10/18
        /// 创建人：姚礼迪
        /// 模块：文件管理模块（原来为体系管理，现在改为文件管理）
        /// </summary>
        /// <returns></returns>
        // GET: System
        public ActionResult Index(string url)
        {
             url = @"/file/作文.docx";
             string physicalPath = Server.MapPath(Server.UrlDecode(url));
           // string physicalPath = @"D:\中汽\EMCManagementSystem\EMCManagementSystem\file\作文.docx";
            string extension = Path.GetExtension(physicalPath);

            string htmlUrl = "";
            switch (extension.ToLower())
            {
                case ".xls":
                case ".xlsx":
                    //htmlUrl = PreviewExcel(physicalPath, url);
                    break;
                case ".doc":
                case ".docx":
                    htmlUrl = PreviewWord(physicalPath, url);
                    break;
                case ".txt":
                    htmlUrl = PreviewTxt(physicalPath, url);
                    break;
                case ".pdf":
                    htmlUrl = PreviewPdf(physicalPath, url);
                    break;
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".png":
                    htmlUrl = PreviewImg(physicalPath, url);
                    break;
                default:
                    htmlUrl = PreviewOther(physicalPath, url);
                    break;
            }

            return Redirect(Url.Content(htmlUrl));
        }
         
        /// <summary>
        /// 体系管理主界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Main()
        {
            return View();
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        public ActionResult upload()
        {
           
            return View();
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <returns></returns>
        public ActionResult download()
        {
            return View();
        }
        
        /// <summary>
        /// 报告模板
        /// </summary>
        /// <returns></returns>
        public ActionResult reportTemplate()
        {
            return View();
        }
        /// <summary>
        /// 新增报告模板
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertreportTemplate()
        {
            return View();
        }
        /// <summary>
        /// 原始数据
        /// </summary>
        /// <returns></returns>
        public ActionResult rawData()
        {
            return View();
        }
        /// <summary>
        /// 新增原始数据
        /// </summary>
        /// <returns></returns>
        public ActionResult insertRawData()
        {
            return View();
        }

     




        #region Index页面  
        /// <summary>  
        /// Index页面  
        /// </summary>  
        /// <paramname="url">例：/uploads/......XXX.xls</param>  

        #endregion

        #region 预览Excel  
        /// <summary>  
        /// 预览Excel  
        /// </summary>  
        //public string PreviewExcel(stringphysicalPath, string url)
        //{
        //    Microsoft.Office.Interop.Excel.Application application = null;
        //    Microsoft.Office.Interop.Excel.Workbook workbook = null;
        //    application = new Microsoft.Office.Interop.Excel.Application();
        //    objectmissing = Type.Missing;
        //    objecttrueObject = true;
        //    application.Visible = false;
        //    application.DisplayAlerts = false;
        //    workbook = application.Workbooks.Open(physicalPath, missing, trueObject, missing, missing, missing,
        //       missing, missing, missing, missing, missing, missing, missing, missing, missing);
        //    //Save Excelto Html  
        //    objectformat = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
        //    stringhtmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
        //    StringoutputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
        //    workbook.SaveAs(outputFile, format, missing, missing, missing,
        //                      missing, XlSaveAsAccessMode.xlNoChange, missing,
        //                      missing, missing, missing, missing);
        //    workbook.Close();
        //    application.Quit();
        //    return Path.GetDirectoryName(Server.UrlDecode(url)) + "\\" + htmlName;
        //}
        #endregion

        #region 预览Word  
        /// <summary>  
        /// 预览Word  
        /// </summary>  
        public string PreviewWord(string physicalPath, string url)
        {
            Microsoft.Office.Interop.Word._Application application = null;
            Microsoft.Office.Interop.Word._Document doc = null;
            application = new Microsoft.Office.Interop.Word.Application();
            object missing = Type.Missing;
            object trueObject = true;
            application.Visible = false;
            application.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            doc = application.Documents.Open(physicalPath, missing, trueObject, missing, missing, missing,
              missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
            //Save Excel to Html  
            object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;
            string htmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
            String outputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
            //doc.SaveAs(outputFile, format, missing, missing, missing,
            //         missing, XlSaveAsAccessMode.xlNoChange, missing,
            //         missing, missing, missing, missing);
            doc.Close();
            application.Quit();
            return Path.GetDirectoryName(Server.UrlDecode(url)) + "\\" + htmlName;
        }
        #endregion

        #region 预览Txt  
        /// <summary>  
        /// 预览Txt  
        /// </summary>  
        public string PreviewTxt(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 预览Pdf  
        /// <summary>  
        /// 预览Pdf  
        /// </summary>  
        public string PreviewPdf(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 预览图片  
        /// <summary>  
        /// 预览图片  
        /// </summary>  
        public string PreviewImg(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 预览其他文件  
        /// <summary>  
        /// 预览其他文件  
        /// </summary>  
        public string PreviewOther(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 添加报告
        /// <summary>  
        /// 添加报告  
        /// </summary>  
        public ActionResult addReport(Report tableReport)
        {
            dbEMCEntities.Entry(tableReport).State = System.Data.Entity.EntityState.Added;
            int i = dbEMCEntities.SaveChanges();
            if (i > 0)
            {
                //+Upload
                //int reportID = tableReport.ReportID;
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 验证依据标准是否存在
        /// <summary>  
        /// 验证依据标准是否存在  
        /// </summary>  
        public ActionResult validateStandardExists(string BaseOnStandard)
        {
            var result = from table1 in dbEMCEntities.Report
                         where table1.BaseOnStandard == BaseOnStandard
                         select table1;

            if (result.ToList().Count > 0)
            {
                return Json(result.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NotExist", JsonRequestBehavior.AllowGet);
            }            
        }
        #endregion

        #region 查询所有报告
        /// <summary>  
        /// 查询所有报告  
        /// </summary>  
        public ActionResult getAllReports()
        {
            var result = from table1 in dbEMCEntities.Report
                         select new
                         {
                             Number = 0,
                             table1.ReportID,
                             table1.DocNumber,
                             table1.DocName,
                             table1.State,
                             Operation = "无"
                         };

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除一条报告
        /// <summary>  
        /// 删除一条报告  
        /// </summary>  
        public ActionResult destroyAReport(int ReportID)
        {
            Report report = dbEMCEntities.Report.Find(ReportID);
            report.State = "作废";
            dbEMCEntities.Entry(report).State = System.Data.Entity.EntityState.Modified;
            int i = dbEMCEntities.SaveChanges();
            if (i > 0)
            {                                
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        private EMCEntities dbEMCEntities = new EMCEntities();
    }
}