using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutomaticEmail
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            //try
            //{

                using (System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceInstaller1.ServiceName))
                {
                    service.Start();
                }
            //}
            //catch(Exception ex)
            //{
            //    this.WriteFile(ex.Message.ToString());
            //}
        }

        //private void WriteFile(string text)
        //{
        //    string path = "Desktop:\\serviceopt.txt";
        //    using (StreamWriter writer = new StreamWriter(path, true))
        //    {
        //        writer.WriteLine(text);
        //        writer.Close();
        //    }
        //}
    }
}
