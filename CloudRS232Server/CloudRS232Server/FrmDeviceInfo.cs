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
    public partial class FrmDeviceInfo : Form
    {
        public FrmDeviceInfo()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select d.ID,m.MerchName,d.DeviceName,case d.DeviceType when '0' then '路由器/控制器' when '1' then '提/售币机' when '2' then '存币机' when '3' then '出票器' else '卡头' end DeviceType,d.SN,d.Token,d.QRURL,case d.Status when '0' then '未激活' when '1' then '正常' else '停用' end as Status from Base_DeviceInfo d left join Base_MerchInfo m on d.MerchID=m.ID");
            gdView.DataSource = dt;
            gdView.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                Console.WriteLine(c.GetType().Name);
                if (c.GetType().Name.ToLower() == "textbox")
                {
                    if (c.Text == "" && c.Name != "txtDeviceName")
                    {
                        MessageBox.Show("信息不全");
                        return;
                    }
                }
            }
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select * from Base_DeviceInfo where SN='" + txtSN.Text + "'");
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("设备串码已存在");
                return;
            }
            string token = "";
            while (true)
            {
                Random r = new Random();
                token = Convert.ToString(r.Next(10000000, 2147483647), 16);
                int n = 8 - token.Length;
                for (int i = 0; i < n; i++)
                {
                    token = "0" + token;
                }

                dt = ac.ExecuteQueryReturnTable("select * from Base_DeviceInfo where token='" + token + "'");
                if (dt.Rows.Count == 0)
                    break;
            }

            string url = "http://mp.4000051530.com/c/" + token;

            string sql = string.Format("insert into Base_DeviceInfo values ('0','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')",
                txtDeviceName.Text,
                cbDeviceType.SelectedIndex,
                txtSN.Text,
                token,
                url,
                "0",
                chkMotor1EN.Checked ? "1" : "0",
                chkMotor2EN.Checked ? "1" : "0",
                "2",
                txtScale1.Text,
                txtScale2.Text,
                txtToCoin.Text,
                txtToCard.Text,
                chkSSR.Checked ? "1" : "0",
                "60000"
                );
            ac.Execute(sql);
            LoadData();
        }

        private void FrmDeviceInfo_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (gdView.CurrentRow != null)
            {
                foreach (Control c in this.Controls)
                {
                    Console.WriteLine(c.GetType().Name);
                    if (c.GetType().Name.ToLower() == "textbox")
                    {
                        if (c.Text == "" && c.Name != "txtDeviceName")
                        {
                            MessageBox.Show("信息不全");
                            return;
                        }
                    }
                }
                DataAccess ac = new DataAccess();
                string ID = gdView.CurrentRow.Cells["ID"].Value.ToString();
                string sql = string.Format("update Base_DeviceInfo set DeviceName='{0}',DeviceType='{1}',motor1='{2}',motor2='{3}',motor1_coin='{4}',motor2_coin='{5}',FromDevice='{6}',ToCard='{7}',SSR='{8}' where ID='{9}'",
               txtDeviceName.Text,
               cbDeviceType.SelectedIndex,
               chkMotor1EN.Checked ? "1" : "0",
               chkMotor2EN.Checked ? "1" : "0",
               txtScale1.Text,
               txtScale2.Text,
               txtToCoin.Text,
               txtToCard.Text,
               chkSSR.Checked ? "1" : "0",
               ID
               );
                ac.Execute(sql);
                LoadData();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gdView.CurrentRow != null)
            {
                DialogResult dr = MessageBox.Show("是否删除当前选中？", "莘宸科技", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No) return;

                string ID = gdView.CurrentRow.Cells["ID"].Value.ToString();
                DataAccess ac = new DataAccess();
                ac.Execute("delete from Base_DeviceInfo where ID='" + ID + "'");
                LoadData();
            }
        }
    }
}
