namespace CloudRS232Server
{
    partial class FrmDeviceInfo
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
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbDeviceType = new System.Windows.Forms.ComboBox();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkMotor1EN = new System.Windows.Forms.CheckBox();
            this.chkMotor2EN = new System.Windows.Forms.CheckBox();
            this.txtScale1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtScale2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtToCoin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtToCard = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkSSR = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MerchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Token = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QRURL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gdView)).BeginInit();
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
            this.DeviceName,
            this.DeviceType,
            this.SN,
            this.Token,
            this.QRURL,
            this.Status});
            this.gdView.Location = new System.Drawing.Point(11, 122);
            this.gdView.MultiSelect = false;
            this.gdView.Name = "gdView";
            this.gdView.ReadOnly = true;
            this.gdView.RowTemplate.Height = 23;
            this.gdView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdView.Size = new System.Drawing.Size(916, 274);
            this.gdView.TabIndex = 1;
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Location = new System.Drawing.Point(71, 6);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.Size = new System.Drawing.Size(100, 21);
            this.txtDeviceName.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "设备名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(192, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "设备类别";
            // 
            // cbDeviceType
            // 
            this.cbDeviceType.FormattingEnabled = true;
            this.cbDeviceType.Items.AddRange(new object[] {
            "路由器/控制器",
            "提/售币机",
            "存币机",
            "出票器",
            "卡头"});
            this.cbDeviceType.Location = new System.Drawing.Point(251, 6);
            this.cbDeviceType.Name = "cbDeviceType";
            this.cbDeviceType.Size = new System.Drawing.Size(121, 20);
            this.cbDeviceType.TabIndex = 6;
            // 
            // txtSN
            // 
            this.txtSN.Location = new System.Drawing.Point(467, 9);
            this.txtSN.Name = "txtSN";
            this.txtSN.Size = new System.Drawing.Size(188, 21);
            this.txtSN.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(408, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "设备串码";
            // 
            // chkMotor1EN
            // 
            this.chkMotor1EN.AutoSize = true;
            this.chkMotor1EN.Location = new System.Drawing.Point(11, 46);
            this.chkMotor1EN.Name = "chkMotor1EN";
            this.chkMotor1EN.Size = new System.Drawing.Size(54, 16);
            this.chkMotor1EN.TabIndex = 9;
            this.chkMotor1EN.Text = "马达1";
            this.chkMotor1EN.UseVisualStyleBackColor = true;
            // 
            // chkMotor2EN
            // 
            this.chkMotor2EN.AutoSize = true;
            this.chkMotor2EN.Location = new System.Drawing.Point(71, 46);
            this.chkMotor2EN.Name = "chkMotor2EN";
            this.chkMotor2EN.Size = new System.Drawing.Size(54, 16);
            this.chkMotor2EN.TabIndex = 10;
            this.chkMotor2EN.Text = "马达2";
            this.chkMotor2EN.UseVisualStyleBackColor = true;
            // 
            // txtScale1
            // 
            this.txtScale1.Location = new System.Drawing.Point(229, 43);
            this.txtScale1.Name = "txtScale1";
            this.txtScale1.Size = new System.Drawing.Size(100, 21);
            this.txtScale1.TabIndex = 12;
            this.txtScale1.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(140, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "马达1出币比例";
            // 
            // txtScale2
            // 
            this.txtScale2.Location = new System.Drawing.Point(436, 43);
            this.txtScale2.Name = "txtScale2";
            this.txtScale2.Size = new System.Drawing.Size(100, 21);
            this.txtScale2.TabIndex = 14;
            this.txtScale2.Text = "1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(347, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "马达2出币比例";
            // 
            // txtToCoin
            // 
            this.txtToCoin.Location = new System.Drawing.Point(46, 77);
            this.txtToCoin.Name = "txtToCoin";
            this.txtToCoin.Size = new System.Drawing.Size(100, 21);
            this.txtToCoin.TabIndex = 16;
            this.txtToCoin.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "数币";
            // 
            // txtToCard
            // 
            this.txtToCard.Location = new System.Drawing.Point(216, 77);
            this.txtToCard.Name = "txtToCard";
            this.txtToCard.Size = new System.Drawing.Size(100, 21);
            this.txtToCard.TabIndex = 18;
            this.txtToCard.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(157, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "卡里变化";
            // 
            // chkSSR
            // 
            this.chkSSR.AutoSize = true;
            this.chkSSR.Location = new System.Drawing.Point(557, 45);
            this.chkSSR.Name = "chkSSR";
            this.chkSSR.Size = new System.Drawing.Size(108, 16);
            this.chkSSR.TabIndex = 19;
            this.chkSSR.Text = "启动电平（高）";
            this.chkSSR.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(852, 75);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 22;
            this.button3.Text = "删除";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(759, 75);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(668, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "新增";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            // DeviceName
            // 
            this.DeviceName.DataPropertyName = "DeviceName";
            this.DeviceName.HeaderText = "设备别称";
            this.DeviceName.Name = "DeviceName";
            this.DeviceName.ReadOnly = true;
            // 
            // DeviceType
            // 
            this.DeviceType.DataPropertyName = "DeviceType";
            this.DeviceType.HeaderText = "设备类别";
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
            // Token
            // 
            this.Token.DataPropertyName = "Token";
            this.Token.HeaderText = "令牌";
            this.Token.Name = "Token";
            this.Token.ReadOnly = true;
            // 
            // QRURL
            // 
            this.QRURL.DataPropertyName = "QRURL";
            this.QRURL.HeaderText = "二维码地址";
            this.QRURL.Name = "QRURL";
            this.QRURL.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "设备状态";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // FrmDeviceInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 403);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkSSR);
            this.Controls.Add(this.txtToCard);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtToCoin);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtScale2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtScale1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkMotor2EN);
            this.Controls.Add(this.chkMotor1EN);
            this.Controls.Add(this.txtSN);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbDeviceType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDeviceName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gdView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDeviceInfo";
            this.Text = "设备信息维护";
            this.Load += new System.EventHandler(this.FrmDeviceInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gdView;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbDeviceType;
        private System.Windows.Forms.TextBox txtSN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkMotor1EN;
        private System.Windows.Forms.CheckBox chkMotor2EN;
        private System.Windows.Forms.TextBox txtScale1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtScale2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtToCoin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtToCard;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkSSR;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn MerchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceType;
        private System.Windows.Forms.DataGridViewTextBoxColumn SN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Token;
        private System.Windows.Forms.DataGridViewTextBoxColumn QRURL;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}