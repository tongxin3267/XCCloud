namespace CloudRS232Server
{
    partial class FrmBind
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
            this.gdView = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MerchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mobile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gdGroup = new System.Windows.Forms.DataGridView();
            this.gdTerminal = new System.Windows.Forms.DataGridView();
            this.gdRoute = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.gdFreeSegment = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.gdFreeDevice = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbGroupType = new System.Windows.Forms.ComboBox();
            this.txtCoin = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbCOHigh = new System.Windows.Forms.RadioButton();
            this.rbCOLow = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbLotteryEN = new System.Windows.Forms.RadioButton();
            this.rbLotteryDS = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbOnlyLotteryEN = new System.Windows.Forms.RadioButton();
            this.rbOnlyLotteryDS = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rbReadcatEN = new System.Windows.Forms.RadioButton();
            this.rbReadcatDS = new System.Windows.Forms.RadioButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rbGiftEN = new System.Windows.Forms.RadioButton();
            this.rbGiftDS = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.FreeSID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SegmentSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SegmentID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label15 = new System.Windows.Forms.Label();
            this.txtDeviceAddress = new System.Windows.Forms.TextBox();
            this.FreeDID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FreeSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.GroupID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button8 = new System.Windows.Forms.Button();
            this.ExistDID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdTerminal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdRoute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdFreeSegment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdFreeDevice)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // gdView
            // 
            this.gdView.AllowUserToAddRows = false;
            this.gdView.AllowUserToDeleteRows = false;
            this.gdView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.MerchName,
            this.OPName,
            this.Mobile,
            this.State});
            this.gdView.Location = new System.Drawing.Point(53, 38);
            this.gdView.MultiSelect = false;
            this.gdView.Name = "gdView";
            this.gdView.ReadOnly = true;
            this.gdView.RowTemplate.Height = 23;
            this.gdView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdView.Size = new System.Drawing.Size(853, 203);
            this.gdView.TabIndex = 1;
            this.gdView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gdView_CellClick);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "编号";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            // 
            // MerchName
            // 
            this.MerchName.DataPropertyName = "MerchName";
            this.MerchName.HeaderText = "商户名称";
            this.MerchName.Name = "MerchName";
            this.MerchName.ReadOnly = true;
            // 
            // OPName
            // 
            this.OPName.DataPropertyName = "OPName";
            this.OPName.HeaderText = "负责人";
            this.OPName.Name = "OPName";
            this.OPName.ReadOnly = true;
            // 
            // Mobile
            // 
            this.Mobile.DataPropertyName = "Mobile";
            this.Mobile.HeaderText = "手机号码";
            this.Mobile.Name = "Mobile";
            this.Mobile.ReadOnly = true;
            // 
            // State
            // 
            this.State.DataPropertyName = "State";
            this.State.HeaderText = "状态";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "选择商户";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(347, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "已存在分组和外设";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(641, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "分组控制终端";
            // 
            // gdGroup
            // 
            this.gdGroup.AllowUserToAddRows = false;
            this.gdGroup.AllowUserToDeleteRows = false;
            this.gdGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdGroup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GroupID,
            this.GroupName,
            this.GroupType});
            this.gdGroup.Location = new System.Drawing.Point(349, 284);
            this.gdGroup.MultiSelect = false;
            this.gdGroup.Name = "gdGroup";
            this.gdGroup.ReadOnly = true;
            this.gdGroup.RowHeadersWidth = 10;
            this.gdGroup.RowTemplate.Height = 23;
            this.gdGroup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdGroup.Size = new System.Drawing.Size(263, 214);
            this.gdGroup.TabIndex = 9;
            this.gdGroup.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gdGroup_CellClick);
            // 
            // gdTerminal
            // 
            this.gdTerminal.AllowUserToAddRows = false;
            this.gdTerminal.AllowUserToDeleteRows = false;
            this.gdTerminal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdTerminal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ExistDID,
            this.DeviceType,
            this.SN});
            this.gdTerminal.Location = new System.Drawing.Point(643, 284);
            this.gdTerminal.MultiSelect = false;
            this.gdTerminal.Name = "gdTerminal";
            this.gdTerminal.ReadOnly = true;
            this.gdTerminal.RowHeadersWidth = 10;
            this.gdTerminal.RowTemplate.Height = 23;
            this.gdTerminal.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdTerminal.Size = new System.Drawing.Size(263, 214);
            this.gdTerminal.TabIndex = 10;
            // 
            // gdRoute
            // 
            this.gdRoute.AllowUserToAddRows = false;
            this.gdRoute.AllowUserToDeleteRows = false;
            this.gdRoute.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdRoute.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SegmentID,
            this.SelectSN,
            this.DeviceName});
            this.gdRoute.Location = new System.Drawing.Point(53, 284);
            this.gdRoute.MultiSelect = false;
            this.gdRoute.Name = "gdRoute";
            this.gdRoute.ReadOnly = true;
            this.gdRoute.RowHeadersWidth = 10;
            this.gdRoute.RowTemplate.Height = 23;
            this.gdRoute.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdRoute.Size = new System.Drawing.Size(263, 214);
            this.gdRoute.TabIndex = 12;
            this.gdRoute.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gdRoute_CellClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 257);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "已存在控制器";
            // 
            // gdFreeSegment
            // 
            this.gdFreeSegment.AllowUserToAddRows = false;
            this.gdFreeSegment.AllowUserToDeleteRows = false;
            this.gdFreeSegment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdFreeSegment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FreeSID,
            this.SegmentSN,
            this.dataGridViewTextBoxColumn2});
            this.gdFreeSegment.Location = new System.Drawing.Point(53, 538);
            this.gdFreeSegment.MultiSelect = false;
            this.gdFreeSegment.Name = "gdFreeSegment";
            this.gdFreeSegment.ReadOnly = true;
            this.gdFreeSegment.RowHeadersWidth = 10;
            this.gdFreeSegment.RowTemplate.Height = 23;
            this.gdFreeSegment.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdFreeSegment.Size = new System.Drawing.Size(263, 214);
            this.gdFreeSegment.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 511);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "空闲控制器";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(241, 758);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "绑定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gdFreeDevice
            // 
            this.gdFreeDevice.AllowUserToAddRows = false;
            this.gdFreeDevice.AllowUserToDeleteRows = false;
            this.gdFreeDevice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdFreeDevice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FreeDID,
            this.dataGridViewTextBoxColumn3,
            this.FreeSN});
            this.gdFreeDevice.Location = new System.Drawing.Point(643, 538);
            this.gdFreeDevice.MultiSelect = false;
            this.gdFreeDevice.Name = "gdFreeDevice";
            this.gdFreeDevice.ReadOnly = true;
            this.gdFreeDevice.RowHeadersWidth = 10;
            this.gdFreeDevice.RowTemplate.Height = 23;
            this.gdFreeDevice.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdFreeDevice.Size = new System.Drawing.Size(263, 214);
            this.gdFreeDevice.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(641, 511);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "空闲终端";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(831, 758);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "绑定分组";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(347, 511);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 19;
            this.label7.Text = "分组名称";
            // 
            // txtGroupName
            // 
            this.txtGroupName.Location = new System.Drawing.Point(407, 508);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(205, 21);
            this.txtGroupName.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(348, 541);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "分组类别";
            // 
            // cbGroupType
            // 
            this.cbGroupType.FormattingEnabled = true;
            this.cbGroupType.Items.AddRange(new object[] {
            "0 娃娃机",
            "1 压分机",
            "2 推土机",
            "3 剪刀机",
            "4 彩票机",
            "5 枪战机",
            "6 VR设备",
            "7 鱼机"});
            this.cbGroupType.Location = new System.Drawing.Point(407, 538);
            this.cbGroupType.Name = "cbGroupType";
            this.cbGroupType.Size = new System.Drawing.Size(205, 20);
            this.cbGroupType.TabIndex = 22;
            // 
            // txtCoin
            // 
            this.txtCoin.Location = new System.Drawing.Point(418, 564);
            this.txtCoin.Name = "txtCoin";
            this.txtCoin.Size = new System.Drawing.Size(194, 21);
            this.txtCoin.TabIndex = 24;
            this.txtCoin.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(347, 567);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "单局扣币数";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(347, 597);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 25;
            this.label10.Text = "投币信号电平";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbCOHigh);
            this.panel1.Controls.Add(this.rbCOLow);
            this.panel1.Location = new System.Drawing.Point(430, 591);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(188, 23);
            this.panel1.TabIndex = 26;
            // 
            // rbCOHigh
            // 
            this.rbCOHigh.AutoSize = true;
            this.rbCOHigh.Location = new System.Drawing.Point(81, 4);
            this.rbCOHigh.Name = "rbCOHigh";
            this.rbCOHigh.Size = new System.Drawing.Size(59, 16);
            this.rbCOHigh.TabIndex = 1;
            this.rbCOHigh.Text = "高电平";
            this.rbCOHigh.UseVisualStyleBackColor = true;
            // 
            // rbCOLow
            // 
            this.rbCOLow.AutoSize = true;
            this.rbCOLow.Checked = true;
            this.rbCOLow.Location = new System.Drawing.Point(4, 4);
            this.rbCOLow.Name = "rbCOLow";
            this.rbCOLow.Size = new System.Drawing.Size(59, 16);
            this.rbCOLow.TabIndex = 0;
            this.rbCOLow.TabStop = true;
            this.rbCOLow.Text = "低电平";
            this.rbCOLow.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbLotteryEN);
            this.panel2.Controls.Add(this.rbLotteryDS);
            this.panel2.Location = new System.Drawing.Point(430, 620);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(188, 23);
            this.panel2.TabIndex = 28;
            // 
            // rbLotteryEN
            // 
            this.rbLotteryEN.AutoSize = true;
            this.rbLotteryEN.Location = new System.Drawing.Point(81, 4);
            this.rbLotteryEN.Name = "rbLotteryEN";
            this.rbLotteryEN.Size = new System.Drawing.Size(47, 16);
            this.rbLotteryEN.TabIndex = 1;
            this.rbLotteryEN.Text = "启用";
            this.rbLotteryEN.UseVisualStyleBackColor = true;
            // 
            // rbLotteryDS
            // 
            this.rbLotteryDS.AutoSize = true;
            this.rbLotteryDS.Checked = true;
            this.rbLotteryDS.Location = new System.Drawing.Point(4, 4);
            this.rbLotteryDS.Name = "rbLotteryDS";
            this.rbLotteryDS.Size = new System.Drawing.Size(47, 16);
            this.rbLotteryDS.TabIndex = 0;
            this.rbLotteryDS.TabStop = true;
            this.rbLotteryDS.Text = "禁用";
            this.rbLotteryDS.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(347, 626);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 27;
            this.label11.Text = "退彩票到卡";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbOnlyLotteryEN);
            this.panel3.Controls.Add(this.rbOnlyLotteryDS);
            this.panel3.Location = new System.Drawing.Point(430, 649);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(188, 23);
            this.panel3.TabIndex = 30;
            // 
            // rbOnlyLotteryEN
            // 
            this.rbOnlyLotteryEN.AutoSize = true;
            this.rbOnlyLotteryEN.Location = new System.Drawing.Point(81, 4);
            this.rbOnlyLotteryEN.Name = "rbOnlyLotteryEN";
            this.rbOnlyLotteryEN.Size = new System.Drawing.Size(47, 16);
            this.rbOnlyLotteryEN.TabIndex = 1;
            this.rbOnlyLotteryEN.Text = "启用";
            this.rbOnlyLotteryEN.UseVisualStyleBackColor = true;
            // 
            // rbOnlyLotteryDS
            // 
            this.rbOnlyLotteryDS.AutoSize = true;
            this.rbOnlyLotteryDS.Checked = true;
            this.rbOnlyLotteryDS.Location = new System.Drawing.Point(4, 4);
            this.rbOnlyLotteryDS.Name = "rbOnlyLotteryDS";
            this.rbOnlyLotteryDS.Size = new System.Drawing.Size(47, 16);
            this.rbOnlyLotteryDS.TabIndex = 0;
            this.rbOnlyLotteryDS.TabStop = true;
            this.rbOnlyLotteryDS.Text = "禁用";
            this.rbOnlyLotteryDS.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(347, 655);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 29;
            this.label12.Text = "只退实物彩票";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(348, 682);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 27;
            this.label13.Text = "刷卡即扣";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rbReadcatEN);
            this.panel4.Controls.Add(this.rbReadcatDS);
            this.panel4.Location = new System.Drawing.Point(430, 678);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(188, 23);
            this.panel4.TabIndex = 31;
            // 
            // rbReadcatEN
            // 
            this.rbReadcatEN.AutoSize = true;
            this.rbReadcatEN.Location = new System.Drawing.Point(81, 4);
            this.rbReadcatEN.Name = "rbReadcatEN";
            this.rbReadcatEN.Size = new System.Drawing.Size(47, 16);
            this.rbReadcatEN.TabIndex = 1;
            this.rbReadcatEN.Text = "启用";
            this.rbReadcatEN.UseVisualStyleBackColor = true;
            // 
            // rbReadcatDS
            // 
            this.rbReadcatDS.AutoSize = true;
            this.rbReadcatDS.Checked = true;
            this.rbReadcatDS.Location = new System.Drawing.Point(4, 4);
            this.rbReadcatDS.Name = "rbReadcatDS";
            this.rbReadcatDS.Size = new System.Drawing.Size(47, 16);
            this.rbReadcatDS.TabIndex = 0;
            this.rbReadcatDS.TabStop = true;
            this.rbReadcatDS.Text = "禁用";
            this.rbReadcatDS.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.rbGiftEN);
            this.panel5.Controls.Add(this.rbGiftDS);
            this.panel5.Location = new System.Drawing.Point(430, 707);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(188, 23);
            this.panel5.TabIndex = 33;
            // 
            // rbGiftEN
            // 
            this.rbGiftEN.AutoSize = true;
            this.rbGiftEN.Location = new System.Drawing.Point(81, 4);
            this.rbGiftEN.Name = "rbGiftEN";
            this.rbGiftEN.Size = new System.Drawing.Size(47, 16);
            this.rbGiftEN.TabIndex = 1;
            this.rbGiftEN.Text = "启用";
            this.rbGiftEN.UseVisualStyleBackColor = true;
            // 
            // rbGiftDS
            // 
            this.rbGiftDS.AutoSize = true;
            this.rbGiftDS.Checked = true;
            this.rbGiftDS.Location = new System.Drawing.Point(4, 4);
            this.rbGiftDS.Name = "rbGiftDS";
            this.rbGiftDS.Size = new System.Drawing.Size(47, 16);
            this.rbGiftDS.TabIndex = 0;
            this.rbGiftDS.TabStop = true;
            this.rbGiftDS.Text = "禁用";
            this.rbGiftDS.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(348, 711);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 32;
            this.label14.Text = "礼品掉落检测";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(537, 758);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 34;
            this.button3.Text = "保存";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(456, 758);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 35;
            this.button4.Text = "新增";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(643, 758);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 36;
            this.button5.Text = "绑定外设";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(53, 758);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 37;
            this.button6.Text = "解除绑定";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // FreeSID
            // 
            this.FreeSID.DataPropertyName = "FreeSID";
            this.FreeSID.HeaderText = "设备编号";
            this.FreeSID.Name = "FreeSID";
            this.FreeSID.ReadOnly = true;
            this.FreeSID.Visible = false;
            // 
            // SegmentSN
            // 
            this.SegmentSN.DataPropertyName = "SN";
            this.SegmentSN.HeaderText = "串码";
            this.SegmentSN.Name = "SegmentSN";
            this.SegmentSN.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn2.HeaderText = "名称";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // SegmentID
            // 
            this.SegmentID.DataPropertyName = "SegmentID";
            this.SegmentID.HeaderText = "设备编号";
            this.SegmentID.Name = "SegmentID";
            this.SegmentID.ReadOnly = true;
            this.SegmentID.Visible = false;
            // 
            // SelectSN
            // 
            this.SelectSN.DataPropertyName = "SN";
            this.SelectSN.HeaderText = "串码";
            this.SelectSN.Name = "SelectSN";
            this.SelectSN.ReadOnly = true;
            // 
            // DeviceName
            // 
            this.DeviceName.DataPropertyName = "DeviceName";
            this.DeviceName.HeaderText = "名称";
            this.DeviceName.Name = "DeviceName";
            this.DeviceName.ReadOnly = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(724, 763);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 38;
            this.label15.Text = "短地址";
            // 
            // txtDeviceAddress
            // 
            this.txtDeviceAddress.Location = new System.Drawing.Point(772, 759);
            this.txtDeviceAddress.Name = "txtDeviceAddress";
            this.txtDeviceAddress.Size = new System.Drawing.Size(53, 21);
            this.txtDeviceAddress.TabIndex = 39;
            // 
            // FreeDID
            // 
            this.FreeDID.DataPropertyName = "FreeDID";
            this.FreeDID.HeaderText = "编号";
            this.FreeDID.Name = "FreeDID";
            this.FreeDID.ReadOnly = true;
            this.FreeDID.Visible = false;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "DeviceType";
            this.dataGridViewTextBoxColumn3.HeaderText = "类别";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // FreeSN
            // 
            this.FreeSN.DataPropertyName = "FreeSN";
            this.FreeSN.HeaderText = "设备串码";
            this.FreeSN.Name = "FreeSN";
            this.FreeSN.ReadOnly = true;
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Location = new System.Drawing.Point(772, 786);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.Size = new System.Drawing.Size(53, 21);
            this.txtDeviceName.TabIndex = 41;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(724, 790);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 40;
            this.label16.Text = "别称";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(643, 784);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 42;
            this.button7.Text = "解除绑定";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // GroupID
            // 
            this.GroupID.DataPropertyName = "GroupID";
            this.GroupID.HeaderText = "分组编号";
            this.GroupID.Name = "GroupID";
            this.GroupID.ReadOnly = true;
            this.GroupID.Visible = false;
            // 
            // GroupName
            // 
            this.GroupName.DataPropertyName = "GroupName";
            this.GroupName.HeaderText = "名称";
            this.GroupName.Name = "GroupName";
            this.GroupName.ReadOnly = true;
            // 
            // GroupType
            // 
            this.GroupType.DataPropertyName = "GroupType";
            this.GroupType.HeaderText = "类别";
            this.GroupType.Name = "GroupType";
            this.GroupType.ReadOnly = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(831, 784);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 43;
            this.button8.Text = "解除绑定";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // ExistDID
            // 
            this.ExistDID.DataPropertyName = "ExistDID";
            this.ExistDID.HeaderText = "设备编号";
            this.ExistDID.Name = "ExistDID";
            this.ExistDID.ReadOnly = true;
            this.ExistDID.Visible = false;
            // 
            // DeviceType
            // 
            this.DeviceType.DataPropertyName = "DeviceType";
            this.DeviceType.HeaderText = "类别";
            this.DeviceType.Name = "DeviceType";
            this.DeviceType.ReadOnly = true;
            // 
            // SN
            // 
            this.SN.DataPropertyName = "SN";
            this.SN.HeaderText = "设备串码";
            this.SN.Name = "SN";
            this.SN.ReadOnly = true;
            // 
            // FrmBind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 818);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.txtDeviceName);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtDeviceAddress);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtCoin);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cbGroupType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtGroupName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.gdFreeDevice);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gdFreeSegment);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gdRoute);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.gdTerminal);
            this.Controls.Add(this.gdGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gdView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmBind";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设备关系绑定";
            this.Load += new System.EventHandler(this.FrmBind_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdTerminal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdRoute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdFreeSegment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdFreeDevice)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gdView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn MerchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mobile;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView gdGroup;
        private System.Windows.Forms.DataGridView gdTerminal;
        private System.Windows.Forms.DataGridView gdRoute;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView gdFreeSegment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView gdFreeDevice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbGroupType;
        private System.Windows.Forms.TextBox txtCoin;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbCOHigh;
        private System.Windows.Forms.RadioButton rbCOLow;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbLotteryEN;
        private System.Windows.Forms.RadioButton rbLotteryDS;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbOnlyLotteryEN;
        private System.Windows.Forms.RadioButton rbOnlyLotteryDS;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton rbReadcatEN;
        private System.Windows.Forms.RadioButton rbReadcatDS;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton rbGiftEN;
        private System.Windows.Forms.RadioButton rbGiftDS;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.DataGridViewTextBoxColumn FreeSID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SegmentSN;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SegmentID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SelectSN;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtDeviceAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn FreeDID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn FreeSN;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupID;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupType;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExistDID;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceType;
        private System.Windows.Forms.DataGridViewTextBoxColumn SN;
    }
}