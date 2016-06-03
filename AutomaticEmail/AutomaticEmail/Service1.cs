using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using AutomaticEmail.Entities;
using AutomaticEmail.Repository;
using AutomaticEmail.Reports;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using System.Net.Mail;

namespace AutomaticEmail
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            this.WriteToFile("service started");
            ScheduleService();
        }

        protected override void OnStop()
        {
            this.WriteToFile("timer disposed");
            this.Schedular.Dispose();
        }

        public Timer Schedular; 

        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                DateTime scheduledTime = DateTime.MinValue;
                if (mode == "DAILY")
                {
                    scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTime"]);
                    if (DateTime.Now > scheduledTime)
                    {
                        scheduledTime = scheduledTime.AddDays(1.00);
                    }

                }
                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);
                Schedular.Change(dueTime, Timeout.Infinite);
                
            }
            catch(Exception ex)
            {
                WriteToFile(ex.Message.ToString());
                using (System.ServiceProcess.ServiceController s1 = new ServiceController("AutomaticEmail"))
                {
                    s1.Stop();
                }
            }
        }

        private async void SchedularCallback(object e)
        {
            //SubmittalsCriticalPathRepository scr = new SubmittalsCriticalPathRepository();
            //List<SubmittalCriticalPathItem> criticalitems = await scr.GetSubmittalCriticalPathRptDataAsync();
            SubmittalCriticalPathForMailReport scpr = new SubmittalCriticalPathForMailReport();
            await scpr.fillData();
            //DocumentPreviewWindow previewWindow = new DocumentPreviewWindow() { Model = new XtraReportPreviewModel(scpr) };
            //ReportPrintTool print = new ReportPrintTool(scpr);
            //print.ShowPreviewDialog();
            //scpr.CreateDocument();
            //previewWindow.ShowInTaskbar = true;
            //previewWindow.Show();
            string path = System.IO.Path.Combine("F:\\","IDBOAutomaticEmailDocuments\\");
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = "SubmittalCriticalPathAutomaticReport" + DateTime.Now.Date.ToShortDateString().Replace("/","")+".pdf";
            string filePath = Path.Combine(path, fileName);
            scpr.ExportToPdf(filePath);

            MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["From"].ToString(), "santosh@proconstructor.com");
            mail.Subject = "Automatic critical path email";
            Attachment att = new Attachment(filePath);
            mail.Attachments.Add(att);
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
            credentials.UserName = "jimmyappana@gmail.com";
            credentials.Password = "9030476311";
            client.Credentials = credentials;
            client.Port = 587;
            client.Send(mail);
            
            WriteToFile("Automatic email code called.");
            this.ScheduleService();
        }

        private void WriteToFile(string text)
        {
            string path = "F:\\serviceopt.txt";
            FileStream fs = new FileStream(path, FileMode.Create);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }
    }
}
