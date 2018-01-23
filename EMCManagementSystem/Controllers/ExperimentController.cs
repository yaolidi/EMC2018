using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Word;

namespace EMCManagementSystem.Controllers
{
    // Token: 0x02000016 RID: 22
    public class ExperimentController : Controller
    {
        // Token: 0x060000F1 RID: 241 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult Index()
        {
            return base.View();
        }

        // Token: 0x060000F2 RID: 242 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult ExperimentalArrangement()
        {
            return base.View();
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult selectExperimentalArrangement()
        {
            return base.View();
        }

        // Token: 0x060000F4 RID: 244 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult InformationRecord()
        {
            return base.View();
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult InsertInformationRecord()
        {
            return base.View();
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult testTecord()
        {
            return base.View();
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult InserttestTecord()
        {
            return base.View();
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult generateOriginalRecord()
        {
            return base.View();
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult selectequipmentUseRecord()
        {
            return base.View();
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult AllequipmentUseRecord()
        {
            return base.View();
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult equipmentUseRecord()
        {
            return base.View();
        }

        // Token: 0x060000FC RID: 252 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult generationReport()
        {
            return base.View();
        }

        // Token: 0x060000FD RID: 253 RVA: 0x00004108 File Offset: 0x00002308
        public ActionResult GetWordContent(string path)
        {
            if (!Directory.Exists("C:\\文档勿动"))
            {
                Directory.CreateDirectory("C:\\文档勿动");
            }
            Stream responseStream = (((HttpWebRequest)WebRequest.Create("http://119.29.8.149:8060/php/0163.docx")).GetResponse() as HttpWebResponse).GetResponseStream();
            Stream stream = new FileStream("C:\\文档勿动\\0163.docx", FileMode.Create);
            byte[] array = new byte[1024];
            for (int i = responseStream.Read(array, 0, array.Length); i > 0; i = responseStream.Read(array, 0, array.Length))
            {
                stream.Write(array, 0, i);
            }
            stream.Close();
            responseStream.Close();
            return base.View("");
        }

        // Token: 0x060000FE RID: 254 RVA: 0x000041A0 File Offset: 0x000023A0
        public ActionResult write()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(base.Server.MapPath("../templateExcel/附件模板.xls"));
                base.Response.Clear();
                base.Response.Buffer = true;
                base.Response.Charset = "GB2312";
                base.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", HttpUtility.UrlEncode("附件模板.xls")));
                base.Response.AppendHeader("Content-Length", fileInfo.Length.ToString());
                base.Response.ContentEncoding = Encoding.Default;
                base.Response.ContentType = "application/ms-excel";
                base.Response.WriteFile(fileInfo.FullName);
                base.Response.Flush();
                base.Response.End();
                this.wordApp = new ApplicationClass();
                this.wordDoc = this.wordApp.Documents.Add(ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing);
                object obj = "C:\\文档勿动\\123.doc";
                object obj2 = false;
                object obj3 = true;
                this.wordDoc = this.wordApp.Documents.Open(ref obj, ref this.Nothing, ref obj2, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref obj3, ref this.Nothing, ref this.Nothing, ref this.Nothing, ref this.Nothing);
                object obj4 = "book123";
                if (this.wordApp.ActiveDocument.Bookmarks.Exists("book123"))
                {
                    this.wordDoc.Bookmarks[ref obj4].Range.Text = "insert text";
                }
                _Document arg_1E3_0 = this.wordDoc;
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                object missing3 = Type.Missing;
                arg_1E3_0.Close(ref missing, ref missing2, ref missing3);
            }
            catch
            {
            }
            return base.View();
        }

        // Token: 0x060000FF RID: 255 RVA: 0x00003FD5 File Offset: 0x000021D5
        public ActionResult previewWord()
        {
            return base.View();
        }

        // Token: 0x04000073 RID: 115
        private object defaultTemplate;

        // Token: 0x04000074 RID: 116
        private Application wordApp;

        // Token: 0x04000075 RID: 117
        private Document wordDoc;

        // Token: 0x04000076 RID: 118
        private object Nothing = Missing.Value;
    }
}
