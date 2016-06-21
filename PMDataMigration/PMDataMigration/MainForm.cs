
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Composition;
using PMImportImplementation;
using PMImportContract;
using PMImportImplementation;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace PMDataMigration
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        [ImportMany]
        private IEnumerable<IProjectManagementImport> pmImport;

        public MainForm()
        {
            InitializeComponent();
            PMMigrationLogger.MessageLogged += new PMMigrationLogger.MessageLogging(PMMigrationLogger_MessageLogged);
           PMMigrationLogger.MessageLoggedWithError += new PMMigrationLogger.ErrorTypeMessageLogging(PMMigrationLogger_MessageLoggedWithError);
        }

        void PMMigrationLogger_MessageLoggedWithError(string message, Color color, FontStyle fontstyle)
        {
            richTxtLog.SelectionColor = color;
            richTxtLog.SelectionFont = new Font("verdana", 10, fontstyle);
            richTxtLog.AppendText(message);
            richTxtLog.ScrollToCaret();
        }

        void PMMigrationLogger_MessageLogged(string message, Color color, FontStyle fontstyle)
        {
           // richTxtLog.SelectionColor = color;
           // richTxtLog.SelectionFont = new Font("verdana", 10, fontstyle);
           // richTxtLog.AppendText(message);
           // richTxtLog.ScrollToCaret();

        }

        void BtnStartRestore_Click(object sender, EventArgs e)
        {
            try
            {
                txtMessage.Visible = true;
                int section = -1; ;
                Guid projectID;
                SqlConnection sqlCon = null;
                MySqlConnection mysqlCon = null;
                string mySqlConnectionString = ConfigurationManager.ConnectionStrings["PROCONDB"].ConnectionString;
                string sqlServerConnectionString = ConfigurationManager.ConnectionStrings["IDBODBTemp"].ConnectionString;
                sqlCon = new SqlConnection(sqlServerConnectionString);
                mysqlCon = new MySqlConnection(mySqlConnectionString);
                if (cmbSection.SelectedItem.ToString() == "PunchList")
                {
                    section = (int)PMEnum.PunchList;
                }
                if (cmbSection.SelectedItem.ToString() == "CertifiedPayRoll")
                {
                    section = (int)PMEnum.CertifiedPayRoll;
                }
                if (cmbSection.SelectedItem.ToString() == "CloseOut")
                {
                    section = (int)PMEnum.CloseOut;
                }
                if (cmbSection.SelectedItem.ToString() == "RFI")
                {
                    section = (int)PMEnum.RFI;
                }
                if (cmbSection.SelectedItem.ToString() == "Transmittal")
                {
                    section = (int)PMEnum.Transmittal;
                }
                if (cmbSection.SelectedItem.ToString() == "Letter")
                {
                    section = (int)PMEnum.Letter;
                }

                if (cmbSection.SelectedItem.ToString() == "Instruction")
                {
                    section = (int)PMEnum.Instruction;
                }

                if (cmbSection.SelectedItem.ToString() == "FieldReport")
                {
                    section = (int)PMEnum.FieldReport;
                }
                if (cmbSection.SelectedItem.ToString() == "Conversation")
                {
                    section = (int)PMEnum.Conversation;
                }
                if (cmbSection.SelectedItem.ToString() == "Submittals")
                {
                    section = (int)PMEnum.Submittals;
                }
                if (cmbSection.SelectedItem.ToString() == "COPR")
                {
                    section = (int)PMEnum.COPR;
                }
                if (cmbSection.SelectedItem.ToString() == "Meetings")
                {
                    section = (int)PMEnum.Meetings;
                }

                if (cmbSection.SelectedItem.ToString() == "Documents")
                {
                    section = (int)PMEnum.Documents;
                }


                Guid.TryParse(txtProjectId.Text.ToString(), out projectID);
                foreach (var element in pmImport)
                {
                    element.Move(projectID, section, mysqlCon, sqlCon);
                }
                MessageBox.Show("Document migration success");

            }
            catch (Exception ex)
            {
                richTxtLog.AppendText(ex.ToString());
                richTxtLog.AppendText(ex.StackTrace);
                PMMigrationLogger.Log(ex.ToString(),Color.Red,FontStyle.Bold);                
                MessageBox.Show("There are some errors, please see the Log file",ex.ToString());

            }

        }



        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PMMigrationLogger.MessageLogged -= new PMMigrationLogger.MessageLogging(PMMigrationLogger_MessageLogged);
            PMMigrationLogger.MessageLoggedWithError -= new PMMigrationLogger.ErrorTypeMessageLogging(PMMigrationLogger_MessageLoggedWithError);
        }

        private void clrLog_Click(object sender, EventArgs e)
        {
            richTxtLog.Clear();
            txtProjectId.Clear();
        }

    }
}
