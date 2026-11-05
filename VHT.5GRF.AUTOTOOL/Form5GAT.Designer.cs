namespace _5GAutoTool
{
    partial class Form5GAT
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form5GAT));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.menuStripControl = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsDL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUL = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmExport = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InstrumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VSAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VSGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RFSwitchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AttenuatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diagramsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tOOLSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createATTFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtRRUSerial = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.grpControl = new System.Windows.Forms.GroupBox();
            this.lblPassFail = new System.Windows.Forms.Label();
            this.lblStatusProgress = new System.Windows.Forms.Label();
            this.lblRuntime = new System.Windows.Forms.Label();
            this.picLogoVHT = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.timerProcess = new System.Windows.Forms.Timer(this.components);
            this.process1 = new System.Diagnostics.Process();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lvwResult = new System.Windows.Forms.ListView();
            this.clPort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clClause = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clSpec = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clMeas = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clTmp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clPassFail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clTctTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grdParams = new System.Windows.Forms.DataGridView();
            this.tabLogs = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lvwLog = new _5GAutoTool.DoubleBufferedListView();
            this.runningTask = new System.Windows.Forms.Label();
            this.proLoad = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.grpConfig = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            //this.txtOperatingFreqTo = new System.Windows.Forms.TextBox();
            //this.txtOperatingFreqFrom = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cbbRruType = new System.Windows.Forms.ComboBox();
            this.textBandwidth = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtPWR = new System.Windows.Forms.TextBox();
            this.txtFrequency = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPower = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chckExcludeCoLoRange = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.deltaFoob = new System.Windows.Forms.TextBox();
            this.chkOOB3 = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtOOB2 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtOOB1 = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkPort32 = new System.Windows.Forms.CheckBox();
            this.chkPort31 = new System.Windows.Forms.CheckBox();
            this.chkPort30 = new System.Windows.Forms.CheckBox();
            this.chkPort29 = new System.Windows.Forms.CheckBox();
            this.chkPort28 = new System.Windows.Forms.CheckBox();
            this.chkPort27 = new System.Windows.Forms.CheckBox();
            this.chkPort26 = new System.Windows.Forms.CheckBox();
            this.chkPort25 = new System.Windows.Forms.CheckBox();
            this.chkPort24 = new System.Windows.Forms.CheckBox();
            this.chkPort23 = new System.Windows.Forms.CheckBox();
            this.chkPort22 = new System.Windows.Forms.CheckBox();
            this.chkPort21 = new System.Windows.Forms.CheckBox();
            this.chkPort20 = new System.Windows.Forms.CheckBox();
            this.chkPort19 = new System.Windows.Forms.CheckBox();
            this.chkPort18 = new System.Windows.Forms.CheckBox();
            this.chkPort17 = new System.Windows.Forms.CheckBox();
            this.chkPort16 = new System.Windows.Forms.CheckBox();
            this.chkPort15 = new System.Windows.Forms.CheckBox();
            this.chkPort14 = new System.Windows.Forms.CheckBox();
            this.chkPort13 = new System.Windows.Forms.CheckBox();
            this.chkPort12 = new System.Windows.Forms.CheckBox();
            this.chkPort11 = new System.Windows.Forms.CheckBox();
            this.chkPort10 = new System.Windows.Forms.CheckBox();
            this.chkPort9 = new System.Windows.Forms.CheckBox();
            this.chkALL = new System.Windows.Forms.CheckBox();
            this.chkPort8 = new System.Windows.Forms.CheckBox();
            this.chkPort4 = new System.Windows.Forms.CheckBox();
            this.chkPort7 = new System.Windows.Forms.CheckBox();
            this.chkPort6 = new System.Windows.Forms.CheckBox();
            this.chkPort3 = new System.Windows.Forms.CheckBox();
            this.chkPort5 = new System.Windows.Forms.CheckBox();
            this.chkPort2 = new System.Windows.Forms.CheckBox();
            this.chkPort1 = new System.Windows.Forms.CheckBox();
            this.realTimeClockLabel1 = new _5GAutoTool.RealTimeClockLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.trvTestCases = new _5GAutoTool.DoubleBufferedTreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblProgressVal = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStripControl.SuspendLayout();
            this.grpControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogoVHT)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdParams)).BeginInit();
            this.tabLogs.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpConfig.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripControl
            // 
            this.menuStripControl.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.InstrumentToolStripMenuItem,
            this.HelpToolStripMenuItem,
            this.diagramsToolStripMenuItem,
            this.tOOLSToolStripMenuItem});
            this.menuStripControl.Location = new System.Drawing.Point(0, 0);
            this.menuStripControl.Name = "menuStripControl";
            this.menuStripControl.Padding = new System.Windows.Forms.Padding(6, 2, 0, 2);
            this.menuStripControl.Size = new System.Drawing.Size(1721, 28);
            this.menuStripControl.TabIndex = 0;
            this.menuStripControl.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmsDL,
            this.tsmUL,
            this.tsmExport,
            this.exportToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.FileToolStripMenuItem.Text = "FILE";
            // 
            // tmsDL
            // 
            this.tmsDL.Name = "tmsDL";
            this.tmsDL.Size = new System.Drawing.Size(233, 26);
            this.tmsDL.Text = "Load Setup Downlink";
            this.tmsDL.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // 
            // tsmUL
            // 
            this.tsmUL.Name = "tsmUL";
            this.tsmUL.Size = new System.Drawing.Size(233, 26);
            this.tsmUL.Text = "Load Setup Uplink";
            this.tsmUL.Click += new System.EventHandler(this.tsmUL_Click);
            // 
            // tsmExport
            // 
            this.tsmExport.Name = "tsmExport";
            this.tsmExport.Size = new System.Drawing.Size(233, 26);
            this.tsmExport.Text = "Export Report";
            this.tsmExport.Click += new System.EventHandler(this.tsmiExport_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(233, 26);
            this.exportToolStripMenuItem.Text = "Export listview";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // InstrumentToolStripMenuItem
            // 
            this.InstrumentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VSAToolStripMenuItem,
            this.VSGToolStripMenuItem,
            this.DUToolStripMenuItem,
            this.RFSwitchToolStripMenuItem,
            this.AttenuatorToolStripMenuItem,
            this.toolStripMenuItem1});
            this.InstrumentToolStripMenuItem.Name = "InstrumentToolStripMenuItem";
            this.InstrumentToolStripMenuItem.Size = new System.Drawing.Size(113, 24);
            this.InstrumentToolStripMenuItem.Text = "INSTRUMENT";
            // 
            // VSAToolStripMenuItem
            // 
            this.VSAToolStripMenuItem.Name = "VSAToolStripMenuItem";
            this.VSAToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.VSAToolStripMenuItem.Text = "VSA";
            this.VSAToolStripMenuItem.Click += new System.EventHandler(this.VSAToolStripMenuItem_Click);
            // 
            // VSGToolStripMenuItem
            // 
            this.VSGToolStripMenuItem.Name = "VSGToolStripMenuItem";
            this.VSGToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.VSGToolStripMenuItem.Text = "VSG";
            this.VSGToolStripMenuItem.Click += new System.EventHandler(this.VSGToolStripMenuItem_Click);
            // 
            // DUToolStripMenuItem
            // 
            this.DUToolStripMenuItem.Name = "DUToolStripMenuItem";
            this.DUToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.DUToolStripMenuItem.Text = "DU";
            this.DUToolStripMenuItem.Click += new System.EventHandler(this.DUToolStripMenuItem_Click);
            // 
            // RFSwitchToolStripMenuItem
            // 
            this.RFSwitchToolStripMenuItem.Name = "RFSwitchToolStripMenuItem";
            this.RFSwitchToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.RFSwitchToolStripMenuItem.Text = "RF SWITCH";
            this.RFSwitchToolStripMenuItem.Click += new System.EventHandler(this.RFSwitchToolStripMenuItem_Click);
            // 
            // AttenuatorToolStripMenuItem
            // 
            this.AttenuatorToolStripMenuItem.Name = "AttenuatorToolStripMenuItem";
            this.AttenuatorToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.AttenuatorToolStripMenuItem.Text = "ATTENUATOR";
            this.AttenuatorToolStripMenuItem.Click += new System.EventHandler(this.AttenuatorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(182, 26);
            this.toolStripMenuItem1.Text = "RRU";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.HelpToolStripMenuItem.Text = "HELP";
            this.HelpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // diagramsToolStripMenuItem
            // 
            this.diagramsToolStripMenuItem.Name = "diagramsToolStripMenuItem";
            this.diagramsToolStripMenuItem.Size = new System.Drawing.Size(98, 24);
            this.diagramsToolStripMenuItem.Text = "DIAGRAMS";
            this.diagramsToolStripMenuItem.Click += new System.EventHandler(this.diagramsToolStripMenuItem_Click);
            // 
            // tOOLSToolStripMenuItem
            // 
            this.tOOLSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateFromFileToolStripMenuItem,
            this.createATTFilesToolStripMenuItem});
            this.tOOLSToolStripMenuItem.Name = "tOOLSToolStripMenuItem";
            this.tOOLSToolStripMenuItem.Size = new System.Drawing.Size(67, 24);
            this.tOOLSToolStripMenuItem.Text = "TOOLS";
            // 
            // duplicateFromFileToolStripMenuItem
            // 
            this.duplicateFromFileToolStripMenuItem.Name = "duplicateFromFileToolStripMenuItem";
            this.duplicateFromFileToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.duplicateFromFileToolStripMenuItem.Text = "Duplicate From File";
            this.duplicateFromFileToolStripMenuItem.Click += new System.EventHandler(this.duplicateFromFileToolStripMenuItem_Click);
            // 
            // createATTFilesToolStripMenuItem
            // 
            this.createATTFilesToolStripMenuItem.Name = "createATTFilesToolStripMenuItem";
            this.createATTFilesToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.createATTFilesToolStripMenuItem.Text = "Create ATT Files";
            this.createATTFilesToolStripMenuItem.Click += new System.EventHandler(this.createATTFilesToolStripMenuItem_Click);
            // 
            // txtRRUSerial
            // 
            this.txtRRUSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRRUSerial.Location = new System.Drawing.Point(102, 146);
            this.txtRRUSerial.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRRUSerial.Name = "txtRRUSerial";
            this.txtRRUSerial.Size = new System.Drawing.Size(258, 46);
            this.txtRRUSerial.TabIndex = 10;
            this.txtRRUSerial.Text = "MRU2146";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(9, 159);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(91, 25);
            this.label26.TabIndex = 1;
            this.label26.Text = "RRU S/N";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpControl
            // 
            this.grpControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControl.Controls.Add(this.lblPassFail);
            this.grpControl.Controls.Add(this.lblStatusProgress);
            this.grpControl.Controls.Add(this.lblRuntime);
            this.grpControl.Controls.Add(this.picLogoVHT);
            this.grpControl.Controls.Add(this.groupBox1);
            this.grpControl.Controls.Add(this.label26);
            this.grpControl.Controls.Add(this.txtRRUSerial);
            this.grpControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpControl.Location = new System.Drawing.Point(4, 4);
            this.grpControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpControl.Name = "grpControl";
            this.grpControl.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpControl.Size = new System.Drawing.Size(367, 304);
            this.grpControl.TabIndex = 11;
            this.grpControl.TabStop = false;
            this.grpControl.Text = "OQC STATION";
            // 
            // lblPassFail
            // 
            this.lblPassFail.AutoSize = true;
            this.lblPassFail.Font = new System.Drawing.Font("Arial Rounded MT Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassFail.ForeColor = System.Drawing.Color.Blue;
            this.lblPassFail.Location = new System.Drawing.Point(130, 198);
            this.lblPassFail.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPassFail.Name = "lblPassFail";
            this.lblPassFail.Size = new System.Drawing.Size(98, 34);
            this.lblPassFail.TabIndex = 16;
            this.lblPassFail.Text = "STOP";
            // 
            // lblStatusProgress
            // 
            this.lblStatusProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatusProgress.AutoSize = true;
            this.lblStatusProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblStatusProgress.Location = new System.Drawing.Point(10, 276);
            this.lblStatusProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusProgress.Name = "lblStatusProgress";
            this.lblStatusProgress.Size = new System.Drawing.Size(40, 20);
            this.lblStatusProgress.TabIndex = 14;
            this.lblStatusProgress.Text = "N/A";
            this.lblStatusProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRuntime
            // 
            this.lblRuntime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRuntime.AutoSize = true;
            this.lblRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRuntime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblRuntime.Location = new System.Drawing.Point(118, 245);
            this.lblRuntime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuntime.Name = "lblRuntime";
            this.lblRuntime.Size = new System.Drawing.Size(122, 25);
            this.lblRuntime.TabIndex = 14;
            this.lblRuntime.Text = "00 : 00 : 00";
            this.lblRuntime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picLogoVHT
            // 
            this.picLogoVHT.Image = ((System.Drawing.Image)(resources.GetObject("picLogoVHT.Image")));
            this.picLogoVHT.Location = new System.Drawing.Point(8, 32);
            this.picLogoVHT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picLogoVHT.Name = "picLogoVHT";
            this.picLogoVHT.Size = new System.Drawing.Size(105, 110);
            this.picLogoVHT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogoVHT.TabIndex = 15;
            this.picLogoVHT.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Location = new System.Drawing.Point(120, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(232, 80);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(174, 20);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(51, 48);
            this.btnStop.TabIndex = 5;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
            this.btnConnect.Location = new System.Drawing.Point(8, 20);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(51, 48);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnStart
            // 
            this.btnStart.AllowDrop = true;
            this.btnStart.AutoSize = true;
            this.btnStart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.Location = new System.Drawing.Point(92, 20);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(38, 38);
            this.btnStart.TabIndex = 7;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // timerProcess
            // 
            this.timerProcess.Interval = 1000;
            this.timerProcess.Tick += new System.EventHandler(this.timerProcess_Tick);
            // 
            // process1
            // 
            this.process1.StartInfo.Domain = "";
            this.process1.StartInfo.LoadUserProfile = false;
            this.process1.StartInfo.Password = null;
            this.process1.StartInfo.StandardErrorEncoding = null;
            this.process1.StartInfo.StandardOutputEncoding = null;
            this.process1.StartInfo.UserName = "";
            this.process1.SynchronizingObject = this;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(445, 253);
            this.tabControl1.TabIndex = 30;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lvwResult);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(437, 216);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RESULT VIEW";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lvwResult
            // 
            this.lvwResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clPort,
            this.clClause,
            this.clSpec,
            this.clMin,
            this.clMeas,
            this.clMax,
            this.clTmp,
            this.clTime,
            this.clPassFail,
            this.clTctTime});
            this.lvwResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwResult.GridLines = true;
            this.lvwResult.HideSelection = false;
            this.lvwResult.Location = new System.Drawing.Point(4, 4);
            this.lvwResult.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lvwResult.Name = "lvwResult";
            this.lvwResult.Size = new System.Drawing.Size(429, 208);
            this.lvwResult.TabIndex = 7;
            this.lvwResult.UseCompatibleStateImageBehavior = false;
            this.lvwResult.View = System.Windows.Forms.View.Details;
            // 
            // clPort
            // 
            this.clPort.Text = "PORT";
            this.clPort.Width = 80;
            // 
            // clClause
            // 
            this.clClause.Text = "CLAUSE";
            this.clClause.Width = 195;
            // 
            // clSpec
            // 
            this.clSpec.Text = "SPEC";
            this.clSpec.Width = 80;
            // 
            // clMin
            // 
            this.clMin.Text = "MIN";
            this.clMin.Width = 80;
            // 
            // clMeas
            // 
            this.clMeas.Text = "MEAS";
            this.clMeas.Width = 120;
            // 
            // clMax
            // 
            this.clMax.Text = "MAX";
            this.clMax.Width = 80;
            // 
            // clTmp
            // 
            this.clTmp.Text = "";
            this.clTmp.Width = 115;
            // 
            // clTime
            // 
            this.clTime.Text = "Time";
            this.clTime.Width = 82;
            // 
            // clPassFail
            // 
            this.clPassFail.Text = "PASS/FAIL";
            this.clPassFail.Width = 135;
            // 
            // clTctTime
            // 
            this.clTctTime.Text = "TACT TIME";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chart1);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(437, 213);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "CHART";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 8);
            this.chart1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(4050, 2335);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            title1.Name = "Power";
            this.chart1.Titles.Add(title1);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.grdParams);
            this.tabPage3.Location = new System.Drawing.Point(4, 33);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Size = new System.Drawing.Size(437, 213);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PARAMS";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // grdParams
            // 
            this.grdParams.BackgroundColor = System.Drawing.SystemColors.Window;
            this.grdParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdParams.Location = new System.Drawing.Point(4, 4);
            this.grdParams.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grdParams.Name = "grdParams";
            this.grdParams.RowHeadersWidth = 51;
            this.grdParams.Size = new System.Drawing.Size(429, 205);
            this.grdParams.TabIndex = 0;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.tabPage4);
            this.tabLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabLogs.Location = new System.Drawing.Point(0, 0);
            this.tabLogs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.SelectedIndex = 0;
            this.tabLogs.Size = new System.Drawing.Size(895, 253);
            this.tabLogs.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lvwLog);
            this.tabPage4.Location = new System.Drawing.Point(4, 33);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Size = new System.Drawing.Size(887, 216);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "LOG";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lvwLog
            // 
            this.lvwLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwLog.HideSelection = false;
            this.lvwLog.Location = new System.Drawing.Point(4, 4);
            this.lvwLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvwLog.Name = "lvwLog";
            this.lvwLog.Size = new System.Drawing.Size(879, 208);
            this.lvwLog.TabIndex = 0;
            this.lvwLog.UseCompatibleStateImageBehavior = false;
            // 
            // runningTask
            // 
            this.runningTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.runningTask.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.runningTask, 2);
            this.runningTask.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runningTask.ForeColor = System.Drawing.Color.Black;
            this.runningTask.Location = new System.Drawing.Point(4, 569);
            this.runningTask.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.runningTask.Name = "runningTask";
            this.runningTask.Size = new System.Drawing.Size(37, 38);
            this.runningTask.TabIndex = 15;
            this.runningTask.Text = "N/A";
            this.runningTask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // proLoad
            // 
            this.proLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.proLoad.Location = new System.Drawing.Point(4, 6);
            this.proLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.proLoad.Name = "proLoad";
            this.proLoad.Size = new System.Drawing.Size(473, 22);
            this.proLoad.TabIndex = 12;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 375F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 284F));
            this.tableLayoutPanel2.Controls.Add(this.grpConfig, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.realTimeClockLabel1, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.groupBox4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.grpControl, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.runningTask, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.splitContainer1, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1721, 607);
            this.tableLayoutPanel2.TabIndex = 33;
            // 
            // grpConfig
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.grpConfig, 3);
            this.grpConfig.Controls.Add(this.groupBox6);
            this.grpConfig.Controls.Add(this.groupBox3);
            this.grpConfig.Controls.Add(this.groupBox5);
            this.grpConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpConfig.Location = new System.Drawing.Point(379, 4);
            this.grpConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpConfig.Name = "grpConfig";
            this.grpConfig.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpConfig.Size = new System.Drawing.Size(1338, 304);
            this.grpConfig.TabIndex = 7;
            this.grpConfig.TabStop = false;
            this.grpConfig.Text = "CONFIGURATION";
            // 
            // groupBox6
            //// 
            //this.groupBox6.Controls.Add(this.txtOperatingFreqTo);
            //this.groupBox6.Controls.Add(this.txtOperatingFreqFrom);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.cbbRruType);
            this.groupBox6.Controls.Add(this.textBandwidth);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.txtPWR);
            this.groupBox6.Controls.Add(this.txtFrequency);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.txtPower);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Location = new System.Drawing.Point(546, 29);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Size = new System.Drawing.Size(468, 266);
            this.groupBox6.TabIndex = 30;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "RRU Config";
            // 
            // txtOperatingFreqTo
            // 
            //this.txtOperatingFreqTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.txtOperatingFreqTo.Location = new System.Drawing.Point(326, 214);
            //this.txtOperatingFreqTo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            //this.txtOperatingFreqTo.Name = "txtOperatingFreqTo";
            //this.txtOperatingFreqTo.Size = new System.Drawing.Size(83, 30);
            //this.txtOperatingFreqTo.TabIndex = 30;
            //this.txtOperatingFreqTo.Text = "2400";
            //// 
            //// txtOperatingFreqFrom
            //// 
            //this.txtOperatingFreqFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.txtOperatingFreqFrom.Location = new System.Drawing.Point(179, 214);
            //this.txtOperatingFreqFrom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            //this.txtOperatingFreqFrom.Name = "txtOperatingFreqFrom";
            //this.txtOperatingFreqFrom.Size = new System.Drawing.Size(83, 30);
            //this.txtOperatingFreqFrom.TabIndex = 30;
            //this.txtOperatingFreqFrom.Text = "2300";
            //// 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 216);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 25);
            this.label1.TabIndex = 29;
            this.label1.Text = "Operating Freq.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(285, 216);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 25);
            this.label4.TabIndex = 31;
            this.label4.Text = " -";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(416, 216);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 25);
            this.label2.TabIndex = 31;
            this.label2.Text = "MHz";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(416, 121);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 25);
            this.label11.TabIndex = 26;
            this.label11.Text = "mW";
            // 
            // cbbRruType
            // 
            this.cbbRruType.FormattingEnabled = true;
            this.cbbRruType.Items.AddRange(new object[] {
            "32T32R",
            "8T8R"});
            this.cbbRruType.Location = new System.Drawing.Point(179, 32);
            this.cbbRruType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbbRruType.Name = "cbbRruType";
            this.cbbRruType.Size = new System.Drawing.Size(109, 32);
            this.cbbRruType.TabIndex = 28;
            this.cbbRruType.SelectedIndexChanged += new System.EventHandler(this.cbbRruType_SelectedIndexChanged);
            // 
            // textBandwidth
            // 
            this.textBandwidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBandwidth.Location = new System.Drawing.Point(179, 162);
            this.textBandwidth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBandwidth.Name = "textBandwidth";
            this.textBandwidth.Size = new System.Drawing.Size(83, 30);
            this.textBandwidth.TabIndex = 21;
            this.textBandwidth.Text = "50";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 165);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 25);
            this.label7.TabIndex = 20;
            this.label7.Text = "Bandwildth";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(8, 35);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 25);
            this.label16.TabIndex = 27;
            this.label16.Text = "Type";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPWR
            // 
            this.txtPWR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPWR.Location = new System.Drawing.Point(344, 174);
            this.txtPWR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPWR.Name = "txtPWR";
            this.txtPWR.Size = new System.Drawing.Size(80, 30);
            this.txtPWR.TabIndex = 25;
            this.txtPWR.Text = "1000";
            this.txtPWR.TextChanged += new System.EventHandler(this.txtPWR_TextChanged);
            // 
            // txtFrequency
            // 
            this.txtFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFrequency.Location = new System.Drawing.Point(179, 71);
            this.txtFrequency.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFrequency.Name = "txtFrequency";
            this.txtFrequency.Size = new System.Drawing.Size(83, 30);
            this.txtFrequency.TabIndex = 16;
            this.txtFrequency.Text = "2350";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(271, 121);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 25);
            this.label10.TabIndex = 25;
            this.label10.Text = "dBm";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 74);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 25);
            this.label3.TabIndex = 12;
            this.label3.Text = "Frequency ";
            // 
            // txtPower
            // 
            this.txtPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPower.Location = new System.Drawing.Point(179, 119);
            this.txtPower.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPower.Name = "txtPower";
            this.txtPower.Size = new System.Drawing.Size(83, 30);
            this.txtPower.TabIndex = 24;
            this.txtPower.Text = "30";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(271, 74);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 25);
            this.label6.TabIndex = 19;
            this.label6.Text = "MHz";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(8, 121);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 25);
            this.label9.TabIndex = 23;
            this.label9.Text = "Power";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(271, 165);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 25);
            this.label8.TabIndex = 22;
            this.label8.Text = "MHz";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chckExcludeCoLoRange);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.deltaFoob);
            this.groupBox3.Controls.Add(this.chkOOB3);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.txtOOB2);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.txtOOB1);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(1022, 29);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(368, 266);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Out of band Blocking";
            // 
            // chckExcludeCoLoRange
            // 
            this.chckExcludeCoLoRange.AutoSize = true;
            this.chckExcludeCoLoRange.Checked = true;
            this.chckExcludeCoLoRange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chckExcludeCoLoRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckExcludeCoLoRange.Location = new System.Drawing.Point(8, 146);
            this.chckExcludeCoLoRange.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chckExcludeCoLoRange.Name = "chckExcludeCoLoRange";
            this.chckExcludeCoLoRange.Size = new System.Drawing.Size(270, 29);
            this.chckExcludeCoLoRange.TabIndex = 35;
            this.chckExcludeCoLoRange.Text = "Exclude Co-location Bands";
            this.chckExcludeCoLoRange.UseVisualStyleBackColor = true;
            this.chckExcludeCoLoRange.CheckedChanged += new System.EventHandler(this.chckExcludeCoLoRange_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(229, 64);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 25);
            this.label14.TabIndex = 34;
            this.label14.Text = "MHz";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(2, 62);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(119, 25);
            this.label15.TabIndex = 33;
            this.label15.Text = "Delta_Foob:";
            // 
            // deltaFoob
            // 
            this.deltaFoob.Enabled = false;
            this.deltaFoob.Location = new System.Drawing.Point(142, 60);
            this.deltaFoob.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.deltaFoob.Name = "deltaFoob";
            this.deltaFoob.Size = new System.Drawing.Size(62, 29);
            this.deltaFoob.TabIndex = 32;
            this.deltaFoob.Text = "60";
            // 
            // chkOOB3
            // 
            this.chkOOB3.AutoSize = true;
            this.chkOOB3.Checked = true;
            this.chkOOB3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOOB3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkOOB3.Location = new System.Drawing.Point(8, 29);
            this.chkOOB3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkOOB3.Name = "chkOOB3";
            this.chkOOB3.Size = new System.Drawing.Size(181, 29);
            this.chkOOB3.TabIndex = 31;
            this.chkOOB3.Text = "CW Freq. Range";
            this.chkOOB3.UseVisualStyleBackColor = true;
            this.chkOOB3.CheckedChanged += new System.EventHandler(this.chkOOB3_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(308, 100);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 25);
            this.label18.TabIndex = 28;
            this.label18.Text = "MHz";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(212, 100);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(19, 25);
            this.label17.TabIndex = 27;
            this.label17.Text = "-";
            // 
            // txtOOB2
            // 
            this.txtOOB2.Enabled = false;
            this.txtOOB2.Location = new System.Drawing.Point(238, 99);
            this.txtOOB2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOOB2.Name = "txtOOB2";
            this.txtOOB2.Size = new System.Drawing.Size(62, 29);
            this.txtOOB2.TabIndex = 25;
            this.txtOOB2.Text = "12750";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(2, 100);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(134, 25);
            this.label13.TabIndex = 23;
            this.label13.Text = "Meas. Range:";
            // 
            // txtOOB1
            // 
            this.txtOOB1.Enabled = false;
            this.txtOOB1.Location = new System.Drawing.Point(142, 99);
            this.txtOOB1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOOB1.Name = "txtOOB1";
            this.txtOOB1.Size = new System.Drawing.Size(62, 29);
            this.txtOOB1.TabIndex = 20;
            this.txtOOB1.Text = "1";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkPort32);
            this.groupBox5.Controls.Add(this.chkPort31);
            this.groupBox5.Controls.Add(this.chkPort30);
            this.groupBox5.Controls.Add(this.chkPort29);
            this.groupBox5.Controls.Add(this.chkPort28);
            this.groupBox5.Controls.Add(this.chkPort27);
            this.groupBox5.Controls.Add(this.chkPort26);
            this.groupBox5.Controls.Add(this.chkPort25);
            this.groupBox5.Controls.Add(this.chkPort24);
            this.groupBox5.Controls.Add(this.chkPort23);
            this.groupBox5.Controls.Add(this.chkPort22);
            this.groupBox5.Controls.Add(this.chkPort21);
            this.groupBox5.Controls.Add(this.chkPort20);
            this.groupBox5.Controls.Add(this.chkPort19);
            this.groupBox5.Controls.Add(this.chkPort18);
            this.groupBox5.Controls.Add(this.chkPort17);
            this.groupBox5.Controls.Add(this.chkPort16);
            this.groupBox5.Controls.Add(this.chkPort15);
            this.groupBox5.Controls.Add(this.chkPort14);
            this.groupBox5.Controls.Add(this.chkPort13);
            this.groupBox5.Controls.Add(this.chkPort12);
            this.groupBox5.Controls.Add(this.chkPort11);
            this.groupBox5.Controls.Add(this.chkPort10);
            this.groupBox5.Controls.Add(this.chkPort9);
            this.groupBox5.Controls.Add(this.chkALL);
            this.groupBox5.Controls.Add(this.chkPort8);
            this.groupBox5.Controls.Add(this.chkPort4);
            this.groupBox5.Controls.Add(this.chkPort7);
            this.groupBox5.Controls.Add(this.chkPort6);
            this.groupBox5.Controls.Add(this.chkPort3);
            this.groupBox5.Controls.Add(this.chkPort5);
            this.groupBox5.Controls.Add(this.chkPort2);
            this.groupBox5.Controls.Add(this.chkPort1);
            this.groupBox5.Location = new System.Drawing.Point(8, 29);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(531, 266);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Measuring Port";
            // 
            // chkPort32
            // 
            this.chkPort32.AutoSize = true;
            this.chkPort32.Location = new System.Drawing.Point(461, 135);
            this.chkPort32.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort32.Name = "chkPort32";
            this.chkPort32.Size = new System.Drawing.Size(64, 28);
            this.chkPort32.TabIndex = 32;
            this.chkPort32.Text = "P32";
            this.chkPort32.UseVisualStyleBackColor = true;
            // 
            // chkPort31
            // 
            this.chkPort31.AutoSize = true;
            this.chkPort31.Location = new System.Drawing.Point(461, 101);
            this.chkPort31.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort31.Name = "chkPort31";
            this.chkPort31.Size = new System.Drawing.Size(64, 28);
            this.chkPort31.TabIndex = 33;
            this.chkPort31.Text = "P31";
            this.chkPort31.UseVisualStyleBackColor = true;
            // 
            // chkPort30
            // 
            this.chkPort30.AutoSize = true;
            this.chkPort30.Location = new System.Drawing.Point(461, 66);
            this.chkPort30.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort30.Name = "chkPort30";
            this.chkPort30.Size = new System.Drawing.Size(64, 28);
            this.chkPort30.TabIndex = 34;
            this.chkPort30.Text = "P30";
            this.chkPort30.UseVisualStyleBackColor = true;
            // 
            // chkPort29
            // 
            this.chkPort29.AutoSize = true;
            this.chkPort29.Location = new System.Drawing.Point(461, 32);
            this.chkPort29.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort29.Name = "chkPort29";
            this.chkPort29.Size = new System.Drawing.Size(64, 28);
            this.chkPort29.TabIndex = 35;
            this.chkPort29.Text = "P29";
            this.chkPort29.UseVisualStyleBackColor = true;
            // 
            // chkPort28
            // 
            this.chkPort28.AutoSize = true;
            this.chkPort28.Location = new System.Drawing.Point(398, 135);
            this.chkPort28.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort28.Name = "chkPort28";
            this.chkPort28.Size = new System.Drawing.Size(64, 28);
            this.chkPort28.TabIndex = 28;
            this.chkPort28.Text = "P28";
            this.chkPort28.UseVisualStyleBackColor = true;
            // 
            // chkPort27
            // 
            this.chkPort27.AutoSize = true;
            this.chkPort27.Location = new System.Drawing.Point(398, 101);
            this.chkPort27.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort27.Name = "chkPort27";
            this.chkPort27.Size = new System.Drawing.Size(64, 28);
            this.chkPort27.TabIndex = 29;
            this.chkPort27.Text = "P27";
            this.chkPort27.UseVisualStyleBackColor = true;
            // 
            // chkPort26
            // 
            this.chkPort26.AutoSize = true;
            this.chkPort26.Location = new System.Drawing.Point(398, 66);
            this.chkPort26.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort26.Name = "chkPort26";
            this.chkPort26.Size = new System.Drawing.Size(64, 28);
            this.chkPort26.TabIndex = 30;
            this.chkPort26.Text = "P26";
            this.chkPort26.UseVisualStyleBackColor = true;
            // 
            // chkPort25
            // 
            this.chkPort25.AutoSize = true;
            this.chkPort25.Location = new System.Drawing.Point(398, 32);
            this.chkPort25.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort25.Name = "chkPort25";
            this.chkPort25.Size = new System.Drawing.Size(64, 28);
            this.chkPort25.TabIndex = 31;
            this.chkPort25.Text = "P25";
            this.chkPort25.UseVisualStyleBackColor = true;
            // 
            // chkPort24
            // 
            this.chkPort24.AutoSize = true;
            this.chkPort24.Location = new System.Drawing.Point(334, 135);
            this.chkPort24.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort24.Name = "chkPort24";
            this.chkPort24.Size = new System.Drawing.Size(64, 28);
            this.chkPort24.TabIndex = 24;
            this.chkPort24.Text = "P24";
            this.chkPort24.UseVisualStyleBackColor = true;
            // 
            // chkPort23
            // 
            this.chkPort23.AutoSize = true;
            this.chkPort23.Location = new System.Drawing.Point(334, 101);
            this.chkPort23.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort23.Name = "chkPort23";
            this.chkPort23.Size = new System.Drawing.Size(64, 28);
            this.chkPort23.TabIndex = 25;
            this.chkPort23.Text = "P23";
            this.chkPort23.UseVisualStyleBackColor = true;
            // 
            // chkPort22
            // 
            this.chkPort22.AutoSize = true;
            this.chkPort22.Location = new System.Drawing.Point(334, 66);
            this.chkPort22.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort22.Name = "chkPort22";
            this.chkPort22.Size = new System.Drawing.Size(64, 28);
            this.chkPort22.TabIndex = 26;
            this.chkPort22.Text = "P22";
            this.chkPort22.UseVisualStyleBackColor = true;
            // 
            // chkPort21
            // 
            this.chkPort21.AutoSize = true;
            this.chkPort21.Location = new System.Drawing.Point(334, 32);
            this.chkPort21.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort21.Name = "chkPort21";
            this.chkPort21.Size = new System.Drawing.Size(64, 28);
            this.chkPort21.TabIndex = 27;
            this.chkPort21.Text = "P21";
            this.chkPort21.UseVisualStyleBackColor = true;
            // 
            // chkPort20
            // 
            this.chkPort20.AutoSize = true;
            this.chkPort20.Location = new System.Drawing.Point(270, 134);
            this.chkPort20.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort20.Name = "chkPort20";
            this.chkPort20.Size = new System.Drawing.Size(64, 28);
            this.chkPort20.TabIndex = 20;
            this.chkPort20.Text = "P20";
            this.chkPort20.UseVisualStyleBackColor = true;
            // 
            // chkPort19
            // 
            this.chkPort19.AutoSize = true;
            this.chkPort19.Location = new System.Drawing.Point(270, 100);
            this.chkPort19.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort19.Name = "chkPort19";
            this.chkPort19.Size = new System.Drawing.Size(64, 28);
            this.chkPort19.TabIndex = 21;
            this.chkPort19.Text = "P19";
            this.chkPort19.UseVisualStyleBackColor = true;
            // 
            // chkPort18
            // 
            this.chkPort18.AutoSize = true;
            this.chkPort18.Location = new System.Drawing.Point(270, 65);
            this.chkPort18.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort18.Name = "chkPort18";
            this.chkPort18.Size = new System.Drawing.Size(64, 28);
            this.chkPort18.TabIndex = 22;
            this.chkPort18.Text = "P18";
            this.chkPort18.UseVisualStyleBackColor = true;
            // 
            // chkPort17
            // 
            this.chkPort17.AutoSize = true;
            this.chkPort17.Location = new System.Drawing.Point(270, 31);
            this.chkPort17.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort17.Name = "chkPort17";
            this.chkPort17.Size = new System.Drawing.Size(64, 28);
            this.chkPort17.TabIndex = 23;
            this.chkPort17.Text = "P17";
            this.chkPort17.UseVisualStyleBackColor = true;
            // 
            // chkPort16
            // 
            this.chkPort16.AutoSize = true;
            this.chkPort16.Location = new System.Drawing.Point(202, 135);
            this.chkPort16.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort16.Name = "chkPort16";
            this.chkPort16.Size = new System.Drawing.Size(64, 28);
            this.chkPort16.TabIndex = 16;
            this.chkPort16.Text = "P16";
            this.chkPort16.UseVisualStyleBackColor = true;
            // 
            // chkPort15
            // 
            this.chkPort15.AutoSize = true;
            this.chkPort15.Location = new System.Drawing.Point(202, 101);
            this.chkPort15.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort15.Name = "chkPort15";
            this.chkPort15.Size = new System.Drawing.Size(64, 28);
            this.chkPort15.TabIndex = 17;
            this.chkPort15.Text = "P15";
            this.chkPort15.UseVisualStyleBackColor = true;
            // 
            // chkPort14
            // 
            this.chkPort14.AutoSize = true;
            this.chkPort14.Location = new System.Drawing.Point(202, 66);
            this.chkPort14.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort14.Name = "chkPort14";
            this.chkPort14.Size = new System.Drawing.Size(64, 28);
            this.chkPort14.TabIndex = 18;
            this.chkPort14.Text = "P14";
            this.chkPort14.UseVisualStyleBackColor = true;
            // 
            // chkPort13
            // 
            this.chkPort13.AutoSize = true;
            this.chkPort13.Location = new System.Drawing.Point(202, 32);
            this.chkPort13.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort13.Name = "chkPort13";
            this.chkPort13.Size = new System.Drawing.Size(64, 28);
            this.chkPort13.TabIndex = 19;
            this.chkPort13.Text = "P13";
            this.chkPort13.UseVisualStyleBackColor = true;
            // 
            // chkPort12
            // 
            this.chkPort12.AutoSize = true;
            this.chkPort12.Location = new System.Drawing.Point(138, 134);
            this.chkPort12.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort12.Name = "chkPort12";
            this.chkPort12.Size = new System.Drawing.Size(64, 28);
            this.chkPort12.TabIndex = 12;
            this.chkPort12.Text = "P12";
            this.chkPort12.UseVisualStyleBackColor = true;
            // 
            // chkPort11
            // 
            this.chkPort11.AutoSize = true;
            this.chkPort11.Location = new System.Drawing.Point(138, 101);
            this.chkPort11.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort11.Name = "chkPort11";
            this.chkPort11.Size = new System.Drawing.Size(64, 28);
            this.chkPort11.TabIndex = 13;
            this.chkPort11.Text = "P11";
            this.chkPort11.UseVisualStyleBackColor = true;
            // 
            // chkPort10
            // 
            this.chkPort10.AutoSize = true;
            this.chkPort10.Location = new System.Drawing.Point(138, 65);
            this.chkPort10.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort10.Name = "chkPort10";
            this.chkPort10.Size = new System.Drawing.Size(64, 28);
            this.chkPort10.TabIndex = 14;
            this.chkPort10.Text = "P10";
            this.chkPort10.UseVisualStyleBackColor = true;
            // 
            // chkPort9
            // 
            this.chkPort9.AutoSize = true;
            this.chkPort9.Location = new System.Drawing.Point(138, 31);
            this.chkPort9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort9.Name = "chkPort9";
            this.chkPort9.Size = new System.Drawing.Size(54, 28);
            this.chkPort9.TabIndex = 15;
            this.chkPort9.Text = "P9";
            this.chkPort9.UseVisualStyleBackColor = true;
            // 
            // chkALL
            // 
            this.chkALL.AutoSize = true;
            this.chkALL.Checked = true;
            this.chkALL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkALL.Location = new System.Drawing.Point(18, 168);
            this.chkALL.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkALL.Name = "chkALL";
            this.chkALL.Size = new System.Drawing.Size(65, 28);
            this.chkALL.TabIndex = 1;
            this.chkALL.Text = "ALL";
            this.chkALL.UseVisualStyleBackColor = true;
            this.chkALL.CheckedChanged += new System.EventHandler(this.chkAllPort_CheckedChanged);
            // 
            // chkPort8
            // 
            this.chkPort8.AutoSize = true;
            this.chkPort8.Checked = true;
            this.chkPort8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort8.Location = new System.Drawing.Point(76, 135);
            this.chkPort8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort8.Name = "chkPort8";
            this.chkPort8.Size = new System.Drawing.Size(54, 28);
            this.chkPort8.TabIndex = 2;
            this.chkPort8.Text = "P8";
            this.chkPort8.UseVisualStyleBackColor = true;
            // 
            // chkPort4
            // 
            this.chkPort4.AutoSize = true;
            this.chkPort4.Checked = true;
            this.chkPort4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort4.Location = new System.Drawing.Point(18, 134);
            this.chkPort4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort4.Name = "chkPort4";
            this.chkPort4.Size = new System.Drawing.Size(54, 28);
            this.chkPort4.TabIndex = 3;
            this.chkPort4.Text = "P4";
            this.chkPort4.UseVisualStyleBackColor = true;
            // 
            // chkPort7
            // 
            this.chkPort7.AutoSize = true;
            this.chkPort7.Checked = true;
            this.chkPort7.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort7.Location = new System.Drawing.Point(76, 101);
            this.chkPort7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort7.Name = "chkPort7";
            this.chkPort7.Size = new System.Drawing.Size(54, 28);
            this.chkPort7.TabIndex = 4;
            this.chkPort7.Text = "P7";
            this.chkPort7.UseVisualStyleBackColor = true;
            // 
            // chkPort6
            // 
            this.chkPort6.AutoSize = true;
            this.chkPort6.Checked = true;
            this.chkPort6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort6.Location = new System.Drawing.Point(76, 66);
            this.chkPort6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort6.Name = "chkPort6";
            this.chkPort6.Size = new System.Drawing.Size(54, 28);
            this.chkPort6.TabIndex = 5;
            this.chkPort6.Text = "P6";
            this.chkPort6.UseVisualStyleBackColor = true;
            // 
            // chkPort3
            // 
            this.chkPort3.AutoSize = true;
            this.chkPort3.Checked = true;
            this.chkPort3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort3.Location = new System.Drawing.Point(18, 100);
            this.chkPort3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort3.Name = "chkPort3";
            this.chkPort3.Size = new System.Drawing.Size(54, 28);
            this.chkPort3.TabIndex = 6;
            this.chkPort3.Text = "P3";
            this.chkPort3.UseVisualStyleBackColor = true;
            // 
            // chkPort5
            // 
            this.chkPort5.AutoSize = true;
            this.chkPort5.Checked = true;
            this.chkPort5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort5.Location = new System.Drawing.Point(76, 32);
            this.chkPort5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort5.Name = "chkPort5";
            this.chkPort5.Size = new System.Drawing.Size(54, 28);
            this.chkPort5.TabIndex = 7;
            this.chkPort5.Text = "P5";
            this.chkPort5.UseVisualStyleBackColor = true;
            // 
            // chkPort2
            // 
            this.chkPort2.AutoSize = true;
            this.chkPort2.Checked = true;
            this.chkPort2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort2.Location = new System.Drawing.Point(18, 65);
            this.chkPort2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort2.Name = "chkPort2";
            this.chkPort2.Size = new System.Drawing.Size(54, 28);
            this.chkPort2.TabIndex = 8;
            this.chkPort2.Text = "P2";
            this.chkPort2.UseVisualStyleBackColor = true;
            // 
            // chkPort1
            // 
            this.chkPort1.AutoSize = true;
            this.chkPort1.Checked = true;
            this.chkPort1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPort1.Location = new System.Drawing.Point(18, 32);
            this.chkPort1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPort1.Name = "chkPort1";
            this.chkPort1.Size = new System.Drawing.Size(54, 28);
            this.chkPort1.TabIndex = 9;
            this.chkPort1.Text = "P1";
            this.chkPort1.UseVisualStyleBackColor = true;
            // 
            // realTimeClockLabel1
            // 
            this.realTimeClockLabel1.AutoSize = true;
            this.realTimeClockLabel1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.realTimeClockLabel1.Location = new System.Drawing.Point(1441, 569);
            this.realTimeClockLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.realTimeClockLabel1.Name = "realTimeClockLabel1";
            this.realTimeClockLabel1.Size = new System.Drawing.Size(112, 32);
            this.realTimeClockLabel1.TabIndex = 36;
            this.realTimeClockLabel1.Text = "00:00:00";
            this.realTimeClockLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.realTimeClockLabel1.TimeFormat = "dd/MM/yyyy HH:mm:ss";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.trvTestCases);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(4, 316);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(367, 249);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "TREE VIEW";
            // 
            // trvTestCases
            // 
            this.trvTestCases.CheckBoxes = true;
            this.trvTestCases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTestCases.Location = new System.Drawing.Point(4, 26);
            this.trvTestCases.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trvTestCases.Name = "trvTestCases";
            this.trvTestCases.Size = new System.Drawing.Size(359, 219);
            this.trvTestCases.TabIndex = 0;
            this.trvTestCases.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trvTestCases_AfterCheck);
            this.trvTestCases.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvTestCases_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblProgressVal);
            this.panel1.Controls.Add(this.proLoad);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(908, 571);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(527, 34);
            this.panel1.TabIndex = 16;
            // 
            // lblProgressVal
            // 
            this.lblProgressVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressVal.AutoSize = true;
            this.lblProgressVal.BackColor = System.Drawing.Color.Transparent;
            this.lblProgressVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressVal.ForeColor = System.Drawing.Color.Black;
            this.lblProgressVal.Location = new System.Drawing.Point(479, 8);
            this.lblProgressVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProgressVal.Name = "lblProgressVal";
            this.lblProgressVal.Size = new System.Drawing.Size(33, 20);
            this.lblProgressVal.TabIndex = 16;
            this.lblProgressVal.Text = "0%";
            this.lblProgressVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.splitContainer1, 3);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(377, 314);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabLogs);
            this.splitContainer1.Size = new System.Drawing.Size(1342, 253);
            this.splitContainer1.SplitterDistance = 445;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 37;
            // 
            // Form5GAT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1721, 635);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.menuStripControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripControl;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form5GAT";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VHT-TOOL ĐO KIỂM RRU 5G";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form5GAT_FormClosing);
            this.Load += new System.EventHandler(this.Form5GAT_Load);
            this.menuStripControl.ResumeLayout(false);
            this.menuStripControl.PerformLayout();
            this.grpControl.ResumeLayout(false);
            this.grpControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogoVHT)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdParams)).EndInit();
            this.tabLogs.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grpConfig.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripControl;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InstrumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem VSAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem VSGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RFSwitchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AttenuatorToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblRuntime;
        private System.Windows.Forms.PictureBox picLogoVHT;
        private System.Windows.Forms.ToolStripMenuItem tmsDL;
        private System.Windows.Forms.Timer timerProcess;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatusProgress;
        private System.Diagnostics.Process process1;
        private System.Windows.Forms.ToolStripMenuItem diagramsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmExport;
        private System.Windows.Forms.TextBox txtRRUSerial;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ToolStripMenuItem tsmUL;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lvwResult;
        private System.Windows.Forms.ColumnHeader clPort;
        private System.Windows.Forms.ColumnHeader clClause;
        private System.Windows.Forms.ColumnHeader clSpec;
        private System.Windows.Forms.ColumnHeader clMin;
        private System.Windows.Forms.ColumnHeader clMeas;
        private System.Windows.Forms.ColumnHeader clMax;
        private System.Windows.Forms.ColumnHeader clTmp;
        private System.Windows.Forms.ColumnHeader clTime;
        private System.Windows.Forms.ColumnHeader clPassFail;
        private System.Windows.Forms.ColumnHeader clTctTime;
        private System.Windows.Forms.ToolStripMenuItem tOOLSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createATTFilesToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView grdParams;
        private System.Windows.Forms.ProgressBar proLoad;
        private System.Windows.Forms.Label runningTask;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblProgressVal;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabControl tabLogs;
        private System.Windows.Forms.TabPage tabPage4;
        private DoubleBufferedListView lvwLog;
        private RealTimeClockLabel realTimeClockLabel1;
        private DoubleBufferedTreeView trvTestCases;
        private System.Windows.Forms.GroupBox grpConfig;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbbRruType;
        private System.Windows.Forms.TextBox textBandwidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtPWR;
        private System.Windows.Forms.TextBox txtFrequency;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPower;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chckExcludeCoLoRange;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox deltaFoob;
        private System.Windows.Forms.CheckBox chkOOB3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtOOB2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtOOB1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkPort32;
        private System.Windows.Forms.CheckBox chkPort31;
        private System.Windows.Forms.CheckBox chkPort30;
        private System.Windows.Forms.CheckBox chkPort29;
        private System.Windows.Forms.CheckBox chkPort28;
        private System.Windows.Forms.CheckBox chkPort27;
        private System.Windows.Forms.CheckBox chkPort26;
        private System.Windows.Forms.CheckBox chkPort25;
        private System.Windows.Forms.CheckBox chkPort24;
        private System.Windows.Forms.CheckBox chkPort23;
        private System.Windows.Forms.CheckBox chkPort22;
        private System.Windows.Forms.CheckBox chkPort21;
        private System.Windows.Forms.CheckBox chkPort20;
        private System.Windows.Forms.CheckBox chkPort19;
        private System.Windows.Forms.CheckBox chkPort18;
        private System.Windows.Forms.CheckBox chkPort17;
        private System.Windows.Forms.CheckBox chkPort16;
        private System.Windows.Forms.CheckBox chkPort15;
        private System.Windows.Forms.CheckBox chkPort14;
        private System.Windows.Forms.CheckBox chkPort13;
        private System.Windows.Forms.CheckBox chkPort12;
        private System.Windows.Forms.CheckBox chkPort11;
        private System.Windows.Forms.CheckBox chkPort10;
        private System.Windows.Forms.CheckBox chkPort9;
        private System.Windows.Forms.CheckBox chkALL;
        private System.Windows.Forms.CheckBox chkPort8;
        private System.Windows.Forms.CheckBox chkPort4;
        private System.Windows.Forms.CheckBox chkPort7;
        private System.Windows.Forms.CheckBox chkPort6;
        private System.Windows.Forms.CheckBox chkPort3;
        private System.Windows.Forms.CheckBox chkPort5;
        private System.Windows.Forms.CheckBox chkPort2;
        private System.Windows.Forms.CheckBox chkPort1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblPassFail;
        //private System.Windows.Forms.TextBox txtOperatingFreqTo;
        //private System.Windows.Forms.TextBox txtOperatingFreqFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
    }
}

