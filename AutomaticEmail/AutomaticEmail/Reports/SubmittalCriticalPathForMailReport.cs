using System;
using System.Threading.Tasks;

using System.Collections.Generic;
using AutomaticEmail.Entities;
using AutomaticEmail.Repository;

namespace AutomaticEmail.Reports
{
    public partial class SubmittalCriticalPathForMailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public SubmittalCriticalPathForMailReport()
        {
            InitializeComponent();
        }
        public async Task fillData()
        {
            SubmittalsCriticalPathRepository scr = new SubmittalsCriticalPathRepository();
            List<SubmittalCriticalPathItem> submittals = await scr.GetSubmittalCriticalPathRptDataAsync();
            bsAutomaticCriticalPath.DataSource = submittals.FindAll(m => m.AC4 == "0");
            xrLblRptDate.Text = DateTime.Now.ToLocalTime().ToShortDateString(); 
        }
    }
}
