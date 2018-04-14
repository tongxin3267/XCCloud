using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloudRS232Server
{
    public partial class FrmMain : Form
    {
        UDPServerHelper server = new UDPServerHelper();

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Info.HeadInfo.InitDeviceInfo();
        }

        delegate void DelegShowMsg(string msg);
        void server_OnShowMsg(string msg)
        {
            if (this.InvokeRequired)
            {
                DelegShowMsg a = new DelegShowMsg(server_OnShowMsg);
                this.Invoke(a, msg);
            }
            else
            {
                if (lstRecv.Items.Count > 100) lstRecv.Items.Clear();
                lstRecv.Items.Add(msg);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtRouteCode.Text == "") return;
            if (txtHeadAddress.Text == "") return;
            server.机头锁定解锁指令(txtRouteCode.Text, txtHeadAddress.Text, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            Info.PushRule.RefreshRule(DateTime.Now);

            server.Init(Convert.ToInt32(txtPort.Text));
            server.OnShowMsg += new UDPServerHelper.消息显示(server_OnShowMsg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lstRecv.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtRouteCode.Text == "") return;
            if (txtHeadAddress.Text == "") return;
            server.机头锁定解锁指令(txtRouteCode.Text, txtHeadAddress.Text, false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (txtRouteCode.Text == "") return;
            if (txtMCUID.Text == "") return;
            server.设置机头长地址(txtRouteCode.Text, txtMCUID.Text);
        }

        private void lstRecv_Click(object sender, EventArgs e)
        {
            if (lstRecv.SelectedItem != null)
            {
                txtRecv.Text = lstRecv.SelectedItem.ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            server.远程投币上分(txtRouteCode.Text, txtHeadAddress.Text, txtICCard.Text, Convert.ToInt32(txtCoins.Text));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            server.远程退币(txtRouteCode.Text, txtHeadAddress.Text);
        }

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            Info.PushRule.RefreshRule(DateTime.Now);
        }
    }
}
