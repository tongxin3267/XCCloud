namespace PalletService
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripMenuItem_EncryptDog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_PeopleCard = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_MemberCard = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_PalmPrints = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Printer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnWhoIs2 = new System.Windows.Forms.Button();
            this.btnWhoIs1 = new System.Windows.Forms.Button();
            this.axwhois_enroll_ocx1 = new Axwhois_enroll_ocxLib.Axwhois_enroll_ocx();
            this.axwhois_feature_extract1 = new Axwhois_feature_extractLib.Axwhois_feature_extract();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSavePrinterName = new System.Windows.Forms.Button();
            this.txtPrinterName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectPrinter = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.tmrCheck = new System.Windows.Forms.Timer(this.components);
            this.tmrTimeOut = new System.Windows.Forms.Timer(this.components);
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_enroll_ocx1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_feature_extract1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_EncryptDog,
            this.toolStripMenuItem_PeopleCard,
            this.toolStripMenuItem_MemberCard,
            this.toolStripMenuItem_PalmPrints,
            this.toolStripMenuItem_Printer,
            this.toolStripMenuItem_Exit});
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(149, 136);
            // 
            // toolStripMenuItem_EncryptDog
            // 
            this.toolStripMenuItem_EncryptDog.Name = "toolStripMenuItem_EncryptDog";
            this.toolStripMenuItem_EncryptDog.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem_EncryptDog.Text = "加密狗";
            // 
            // toolStripMenuItem_PeopleCard
            // 
            this.toolStripMenuItem_PeopleCard.Name = "toolStripMenuItem_PeopleCard";
            this.toolStripMenuItem_PeopleCard.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem_PeopleCard.Text = "身份证识读器";
            // 
            // toolStripMenuItem_MemberCard
            // 
            this.toolStripMenuItem_MemberCard.Name = "toolStripMenuItem_MemberCard";
            this.toolStripMenuItem_MemberCard.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem_MemberCard.Text = "IC卡识读器";
            // 
            // toolStripMenuItem_PalmPrints
            // 
            this.toolStripMenuItem_PalmPrints.Name = "toolStripMenuItem_PalmPrints";
            this.toolStripMenuItem_PalmPrints.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem_PalmPrints.Text = "掌纹识别";
            // 
            // toolStripMenuItem_Printer
            // 
            this.toolStripMenuItem_Printer.Name = "toolStripMenuItem_Printer";
            this.toolStripMenuItem_Printer.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem_Printer.Text = "打印机";
            // 
            // toolStripMenuItem_Exit
            // 
            this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
            this.toolStripMenuItem_Exit.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem_Exit.Text = "退出";
            this.toolStripMenuItem_Exit.Click += new System.EventHandler(this.toolStripMenuItem_Exit_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "莘宸托盘服务程序";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1056, 619);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1048, 593);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "掌纹识别";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnWhoIs2);
            this.panel1.Controls.Add(this.btnWhoIs1);
            this.panel1.Controls.Add(this.axwhois_enroll_ocx1);
            this.panel1.Controls.Add(this.axwhois_feature_extract1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1042, 587);
            this.panel1.TabIndex = 8;
            // 
            // btnWhoIs2
            // 
            this.btnWhoIs2.Location = new System.Drawing.Point(740, 433);
            this.btnWhoIs2.Name = "btnWhoIs2";
            this.btnWhoIs2.Size = new System.Drawing.Size(100, 30);
            this.btnWhoIs2.TabIndex = 6;
            this.btnWhoIs2.Text = "识别掌纹";
            this.btnWhoIs2.UseVisualStyleBackColor = true;
            this.btnWhoIs2.Click += new System.EventHandler(this.btnWhoIs2_Click);
            // 
            // btnWhoIs1
            // 
            this.btnWhoIs1.Location = new System.Drawing.Point(199, 433);
            this.btnWhoIs1.Name = "btnWhoIs1";
            this.btnWhoIs1.Size = new System.Drawing.Size(100, 30);
            this.btnWhoIs1.TabIndex = 5;
            this.btnWhoIs1.Text = "注册掌纹";
            this.btnWhoIs1.UseVisualStyleBackColor = true;
            this.btnWhoIs1.Click += new System.EventHandler(this.btnWhoIs1_Click);
            // 
            // axwhois_enroll_ocx1
            // 
            this.axwhois_enroll_ocx1.Enabled = true;
            this.axwhois_enroll_ocx1.Location = new System.Drawing.Point(44, 77);
            this.axwhois_enroll_ocx1.Name = "axwhois_enroll_ocx1";
            this.axwhois_enroll_ocx1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axwhois_enroll_ocx1.OcxState")));
            this.axwhois_enroll_ocx1.Size = new System.Drawing.Size(450, 320);
            this.axwhois_enroll_ocx1.TabIndex = 3;
            // 
            // axwhois_feature_extract1
            // 
            this.axwhois_feature_extract1.Enabled = true;
            this.axwhois_feature_extract1.Location = new System.Drawing.Point(551, 77);
            this.axwhois_feature_extract1.Name = "axwhois_feature_extract1";
            this.axwhois_feature_extract1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axwhois_feature_extract1.OcxState")));
            this.axwhois_feature_extract1.Size = new System.Drawing.Size(450, 320);
            this.axwhois_feature_extract1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.btnSavePrinterName);
            this.tabPage2.Controls.Add(this.txtPrinterName);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btnSelectPrinter);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1048, 593);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "系统设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(285, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(149, 52);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(130, 21);
            this.textBox1.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "TCP端口号：";
            // 
            // btnSavePrinterName
            // 
            this.btnSavePrinterName.Location = new System.Drawing.Point(285, 20);
            this.btnSavePrinterName.Name = "btnSavePrinterName";
            this.btnSavePrinterName.Size = new System.Drawing.Size(68, 23);
            this.btnSavePrinterName.TabIndex = 4;
            this.btnSavePrinterName.Text = "保存";
            this.btnSavePrinterName.UseVisualStyleBackColor = true;
            // 
            // txtPrinterName
            // 
            this.txtPrinterName.Location = new System.Drawing.Point(149, 21);
            this.txtPrinterName.Name = "txtPrinterName";
            this.txtPrinterName.Size = new System.Drawing.Size(130, 21);
            this.txtPrinterName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "打印机设置：";
            // 
            // btnSelectPrinter
            // 
            this.btnSelectPrinter.Location = new System.Drawing.Point(105, 20);
            this.btnSelectPrinter.Name = "btnSelectPrinter";
            this.btnSelectPrinter.Size = new System.Drawing.Size(38, 23);
            this.btnSelectPrinter.TabIndex = 0;
            this.btnSelectPrinter.Text = "选择";
            this.btnSelectPrinter.UseVisualStyleBackColor = true;
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // tmrCheck
            // 
            this.tmrCheck.Interval = 1000;
            this.tmrCheck.Tick += new System.EventHandler(this.tmrCheck_Tick);
            // 
            // tmrTimeOut
            // 
            this.tmrTimeOut.Enabled = true;
            this.tmrTimeOut.Interval = 1000;
            this.tmrTimeOut.Tick += new System.EventHandler(this.tmrTimeOut_Tick);
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 1000;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 619);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "莘宸服务程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_enroll_ocx1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_feature_extract1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Axwhois_enroll_ocxLib.Axwhois_enroll_ocx axwhois_enroll_ocx1;
        private Axwhois_feature_extractLib.Axwhois_feature_extract axwhois_feature_extract1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PalmPrints;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Exit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnWhoIs2;
        private System.Windows.Forms.Button btnWhoIs1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectPrinter;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.Button btnSavePrinterName;
        private System.Windows.Forms.TextBox txtPrinterName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MemberCard;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PeopleCard;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_EncryptDog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Printer;
        private System.Windows.Forms.Timer tmrCheck;
        private System.Windows.Forms.Timer tmrTimeOut;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}

