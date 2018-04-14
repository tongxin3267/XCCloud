namespace PalmDemo
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.axwhois_enroll_ocx1 = new Axwhois_enroll_ocxLib.Axwhois_enroll_ocx();
            this.axwhois_feature_extract1 = new Axwhois_feature_extractLib.Axwhois_feature_extract();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_enroll_ocx1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_feature_extract1)).BeginInit();
            this.SuspendLayout();
            // 
            // axwhois_enroll_ocx1
            // 
            this.axwhois_enroll_ocx1.Enabled = true;
            this.axwhois_enroll_ocx1.Location = new System.Drawing.Point(31, 41);
            this.axwhois_enroll_ocx1.Name = "axwhois_enroll_ocx1";
            this.axwhois_enroll_ocx1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axwhois_enroll_ocx1.OcxState")));
            this.axwhois_enroll_ocx1.Size = new System.Drawing.Size(358, 278);
            this.axwhois_enroll_ocx1.TabIndex = 0;
            this.axwhois_enroll_ocx1.EnrollFinish += new Axwhois_enroll_ocxLib._Dwhois_enroll_ocxEvents_EnrollFinishEventHandler(this.axwhois_enroll_ocx1_EnrollFinish);
            // 
            // axwhois_feature_extract1
            // 
            this.axwhois_feature_extract1.Enabled = true;
            this.axwhois_feature_extract1.Location = new System.Drawing.Point(467, 41);
            this.axwhois_feature_extract1.Name = "axwhois_feature_extract1";
            this.axwhois_feature_extract1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axwhois_feature_extract1.OcxState")));
            this.axwhois_feature_extract1.Size = new System.Drawing.Size(365, 278);
            this.axwhois_feature_extract1.TabIndex = 1;
            this.axwhois_feature_extract1.FeatureGot += new Axwhois_feature_extractLib._Dwhois_feature_extractEvents_FeatureGotEventHandler(this.axwhois_feature_extract1_FeatureGot);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(242, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(147, 48);
            this.button1.TabIndex = 2;
            this.button1.Text = "注册掌纹";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(685, 350);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(147, 48);
            this.button2.TabIndex = 3;
            this.button2.Text = "识别掌纹";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 436);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.axwhois_feature_extract1);
            this.Controls.Add(this.axwhois_enroll_ocx1);
            this.Name = "FrmMain";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_enroll_ocx1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axwhois_feature_extract1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Axwhois_enroll_ocxLib.Axwhois_enroll_ocx axwhois_enroll_ocx1;
        private Axwhois_feature_extractLib.Axwhois_feature_extract axwhois_feature_extract1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

