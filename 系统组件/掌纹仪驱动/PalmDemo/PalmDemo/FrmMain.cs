using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PalmDemo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.axwhois_enroll_ocx1.start("", "");
            button1.Enabled = false;
        }

        private void axwhois_enroll_ocx1_EnrollFinish(object sender, Axwhois_enroll_ocxLib._Dwhois_enroll_ocxEvents_EnrollFinishEvent e)
        {
            button1.Enabled = true;
            Console.WriteLine("掌纹数据：" + e.feature_str);
            axwhois_enroll_ocx1.stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.axwhois_feature_extract1.start("", "");
        }

        private void axwhois_feature_extract1_FeatureGot(object sender, Axwhois_feature_extractLib._Dwhois_feature_extractEvents_FeatureGotEvent e)
        {
            Console.WriteLine("掌纹数据：" + e.feature_str);
            axwhois_feature_extract1.stop();
        }
    }
}
