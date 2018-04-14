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
    public partial class FrmMerchInfo : Form
    {
        public FrmMerchInfo()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select ID,MerchName,OPName,Mobile,case State when '0' then '未激活' when '1' then '正常' else '停用' end as State from Base_MerchInfo order by MerchName");
            gdView.DataSource = dt;
            gdView.Refresh();
        }

        private void FrmMerchInfo_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtMerchName.Text == "" || txtMobile.Text == "" || txtOPName.Text == "")
            {
                MessageBox.Show("信息不全");
                return;
            }
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select * from Base_MerchInfo where Mobile='" + txtMobile.Text + "'");
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("手机号已存在");
                return;
            }
            ac.Execute("insert into Base_MerchInfo values ('" + txtMerchName.Text + "','" + txtOPName.Text + "','" + txtMobile.Text + "','','0')");
            LoadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gdView.CurrentRow != null)
            {
                DialogResult dr = MessageBox.Show("是否删除当前选中？", "莘宸科技", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No) return;

                string ID = gdView.CurrentRow.Cells["ID"].Value.ToString();
                DataAccess ac = new DataAccess();
                ac.Execute("delete from Base_MerchInfo where ID='" + ID + "'");
                LoadData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (gdView.CurrentRow != null)
            {
                string ID = gdView.CurrentRow.Cells["ID"].Value.ToString();
                DataAccess ac = new DataAccess();
                ac.Execute("update Base_MerchInfo set MerchName='" + txtMerchName.Text + "',OPName='" + txtOPName.Text + "' where ID='" + ID + "'");
                LoadData();
            }
        }
    }
}
