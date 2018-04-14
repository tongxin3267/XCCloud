namespace CloudRS232Server
{
    partial class FrmMerchInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtMerchName = new System.Windows.Forms.TextBox();
            this.txtOPName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MerchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mobile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.OPName,
            this.Mobile,
            this.State});
            this.gdView.Location = new System.Drawing.Point(12, 62);
            this.gdView.MultiSelect = false;
            this.gdView.Name = "gdView";
            this.gdView.ReadOnly = true;
            this.gdView.RowTemplate.Height = 23;
            this.gdView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdView.Size = new System.Drawing.Size(798, 274);
            this.gdView.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "商户名称";
            // 
            // txtMerchName
            // 
            this.txtMerchName.Location = new System.Drawing.Point(69, 9);
            this.txtMerchName.Name = "txtMerchName";
            this.txtMerchName.Size = new System.Drawing.Size(100, 21);
            this.txtMerchName.TabIndex = 2;
            // 
            // txtOPName
            // 
            this.txtOPName.Location = new System.Drawing.Point(241, 12);
            this.txtOPName.Name = "txtOPName";
            this.txtOPName.Size = new System.Drawing.Size(100, 21);
            this.txtOPName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "负责人";
            // 
            // txtMobile
            // 
            this.txtMobile.Location = new System.Drawing.Point(419, 12);
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.Size = new System.Drawing.Size(100, 21);
            this.txtMobile.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(360, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "手机号码";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(551, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "新增";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(642, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(735, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "删除";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            // FrmMerchInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 353);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtMobile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOPName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMerchName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gdView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMerchInfo";
            this.Text = "商户信息维护";
            this.Load += new System.EventHandler(this.FrmMerchInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gdView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMerchName;
        private System.Windows.Forms.TextBox txtOPName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMobile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn MerchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mobile;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
    }
}