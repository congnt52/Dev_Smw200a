using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _5GAutoTool.Reports;

namespace _5GAutoTool.Forms
{
    public partial class SetupREPORT : Form
    {
        private Form5GAT mainForm = null;

        private string _reportFolder = @"C:\Reports";
        private string _reportTemplate = @"C:\test3gpp\OQC\32T32R10W\templates\Phieudokiem.xlsx";

        public SetupREPORT(Form callingForm)
        {
            mainForm = callingForm as Form5GAT;
            InitializeComponent();
            txtRootFolder.Text = (System.IO.Directory.Exists(_reportFolder)) ? _reportFolder : "";
            txtTemplateFile.Text = (System.IO.File.Exists(_reportTemplate)) ? _reportTemplate : "";

        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtRootFolder.Text = fbd.SelectedPath;
                }
            }

        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx;*.xls";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    txtTemplateFile.Text = ofd.FileName;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            mainForm.ReportFolder = txtRootFolder.Text;
            mainForm.ReportTemplate = txtTemplateFile.Text;

            this.Close();
        }
        public void GetReportSetting(out string reportFolder, out string reportTemplate)
        {
            reportFolder = txtRootFolder.Text;
            reportTemplate = txtTemplateFile.Text;
        }
    }
}
