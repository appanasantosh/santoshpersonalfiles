/*
 * Created by SharpDevelop.
 * User: sandipan
 * Date: 6/13/2014
 * Time: 3:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PMDataMigration
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.txtProjectId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSection = new System.Windows.Forms.ComboBox();
            this.btnStartRestore = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.Label();
            this.richTxtLog = new System.Windows.Forms.RichTextBox();
            this.clrLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Old Procon Project ID";
            // 
            // txtProjectId
            // 
            this.txtProjectId.Location = new System.Drawing.Point(138, 19);
            this.txtProjectId.Name = "txtProjectId";
            this.txtProjectId.ReadOnly = true;
            this.txtProjectId.Size = new System.Drawing.Size(410, 20);
            this.txtProjectId.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(73, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Section";
            // 
            // cmbSection
            // 
            this.cmbSection.FormattingEnabled = true;
            this.cmbSection.Items.AddRange(new object[] {
            "RFI",
            "CertifiedPayRoll",
            "PunchList",
            "CloseOut",
            "Transmittal",
            "Letter",
            "Instruction",
            "FieldReport",
            "Conversation",
            "Submittals",
            "COPR",
            "Meetings",
            "Documents"});
            this.cmbSection.Location = new System.Drawing.Point(138, 56);
            this.cmbSection.Name = "cmbSection";
            this.cmbSection.Size = new System.Drawing.Size(410, 21);
            this.cmbSection.TabIndex = 3;
            // 
            // btnStartRestore
            // 
            this.btnStartRestore.Location = new System.Drawing.Point(599, 56);
            this.btnStartRestore.Name = "btnStartRestore";
            this.btnStartRestore.Size = new System.Drawing.Size(188, 25);
            this.btnStartRestore.TabIndex = 4;
            this.btnStartRestore.Text = "Start Data Migration";
            this.btnStartRestore.UseVisualStyleBackColor = true;
            this.btnStartRestore.Click += new System.EventHandler(this.BtnStartRestore_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(73, 91);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(725, 17);
            this.txtMessage.TabIndex = 6;
            this.txtMessage.Text = "Data Migration is in progress ....Be patient.This process might take some time. W" +
    "atch the log below to be Updated.";
            this.txtMessage.Visible = false;
            // 
            // richTxtLog
            // 
            this.richTxtLog.Location = new System.Drawing.Point(12, 124);
            this.richTxtLog.Name = "richTxtLog";
            this.richTxtLog.Size = new System.Drawing.Size(1459, 678);
            this.richTxtLog.TabIndex = 7;
            this.richTxtLog.Text = "";
            // 
            // clrLog
            // 
            this.clrLog.Location = new System.Drawing.Point(950, 57);
            this.clrLog.Name = "clrLog";
            this.clrLog.Size = new System.Drawing.Size(174, 23);
            this.clrLog.TabIndex = 8;
            this.clrLog.Text = "Clear Log";
            this.clrLog.UseVisualStyleBackColor = true;
            this.clrLog.Click += new System.EventHandler(this.clrLog_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1512, 831);
            this.Controls.Add(this.clrLog);
            this.Controls.Add(this.richTxtLog);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnStartRestore);
            this.Controls.Add(this.cmbSection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProjectId);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "PMDataMigration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.RichTextBox richTxtLog;
		private System.Windows.Forms.Label txtMessage;
		private System.Windows.Forms.Button btnStartRestore;
		private System.Windows.Forms.ComboBox cmbSection;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtProjectId;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button clrLog;
	}
}
