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
    public partial class FrmBind : Form
    {
        public FrmBind()
        {
            InitializeComponent();
        }

        void LoadMerch()
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select ID,MerchName,OPName,Mobile,case State when '0' then '未激活' when '1' then '正常' else '停用' end as State from Base_MerchInfo order by MerchName");
            gdView.DataSource = dt;
        }

        void LoadFreeSegment()
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select ID as FreeSID,SN,DeviceName from Base_DeviceInfo where MerchID=0 and Status<>2 and DeviceType=0");
            gdFreeSegment.DataSource = dt;
        }
        void LoadFreeDevice()
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select ID as FreeDID,SN as FreeSN,case DeviceType when '0' then '路由器/控制器' when '1' then '提/售币机' when '2' then '存币机' when '3' then '出票器' else '卡头' end DeviceType from Base_DeviceInfo where MerchID=0 and Status<>2 and DeviceType<>0");
            gdFreeDevice.DataSource = dt;
            gdFreeDevice.Refresh();
        }

        void LoadExistSegment(string MerchID)
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select ID as SegmentID,SN,DeviceName from Base_DeviceInfo where MerchID='" + MerchID + "' and DeviceType='0'");
            gdRoute.DataSource = dt;
        }

        void LoadExistGroup(string SegmentID)
        {
            DataAccess ac = new DataAccess();
            string sql = string.Format(@"select GroupID,GroupName,'分组' as GroupType from Data_GameInfo where DeviceID='{0}'
union all
select t.ID as GroupID,d.DeviceName,'外设' as GroupType from Base_DeviceInfo d,Data_MerchDevice t where d.ID=t.DeviceID and t.ParentID='{0}'", SegmentID);
            DataTable dt = ac.ExecuteQueryReturnTable(sql);
            gdGroup.DataSource = dt;
        }

        void LoadTerminal(string GroupID)
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable(string.Format("select t.ID as ExistDID,case d.DeviceType when '0' then '路由器/控制器' when '1' then '提/售币机' when '2' then '存币机' when '3' then '出票器' else '卡头' end DeviceType,d.SN from Base_DeviceInfo d,Data_MerchSegment t where d.ID=t.DeviceID and t.GroupID='{0}'", GroupID));
            gdTerminal.DataSource = dt;
        }

        private void FrmBind_Load(object sender, EventArgs e)
        {
            LoadMerch();
            LoadFreeSegment();
            LoadFreeDevice();
        }

        private void gdView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string merchID = gdView.CurrentRow.Cells["ID"].Value.ToString();
            LoadExistSegment(merchID);
        }

        private void gdRoute_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string SegmentID = gdRoute.CurrentRow.Cells["SegmentID"].Value.ToString();
            LoadExistGroup(SegmentID);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string SegmentID = gdFreeSegment.CurrentRow.Cells["FreeSID"].Value.ToString();
            string merchID = gdView.CurrentRow.Cells["ID"].Value.ToString();
            string sql = string.Format("update Base_DeviceInfo set MerchID='{0}',Status=1 where ID='{1}'", merchID, SegmentID);
            DataAccess ac = new DataAccess();
            ac.Execute(sql);
            LoadFreeSegment();
            LoadExistSegment(merchID);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (txtDeviceAddress.Text == "" || txtDeviceName.Text == "")
            {
                MessageBox.Show("地址和设备别称不能为空");
                return;
            }
            string SegmentID = gdRoute.CurrentRow.Cells["SegmentID"].Value.ToString();
            string merchID = gdView.CurrentRow.Cells["ID"].Value.ToString();
            string FreeDID = gdFreeDevice.CurrentRow.Cells["FreeDID"].Value.ToString();
            string sql = string.Format("insert into Data_MerchDevice values ('{0}','{1}','{2}');  update Base_DeviceInfo set MerchID='{3}',DeviceName='{4}',Status=1 where ID='{1}';", SegmentID, FreeDID, txtDeviceAddress.Text, merchID, txtDeviceName.Text);
            DataAccess ac = new DataAccess();
            ac.Execute(sql);
            LoadFreeDevice();
            LoadExistGroup(SegmentID);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtDeviceAddress.Text == "" || txtDeviceName.Text == "")
            {
                MessageBox.Show("地址和设备别称不能为空");
                return;
            }
            string SegmentID = gdRoute.CurrentRow.Cells["SegmentID"].Value.ToString();
            string merchID = gdView.CurrentRow.Cells["ID"].Value.ToString();
            string GroupID = gdGroup.CurrentRow.Cells["GroupID"].Value.ToString();
            string FreeDID = gdFreeDevice.CurrentRow.Cells["FreeDID"].Value.ToString();
            string sql = string.Format("insert into Data_MerchSegment values ('{0}','{5}','{1}','{2}');  update Base_DeviceInfo set MerchID='{3}',DeviceName='{4}',Status=1 where ID='{1}';", SegmentID, FreeDID, txtDeviceAddress.Text, merchID, txtDeviceName.Text, GroupID);
            DataAccess ac = new DataAccess();
            ac.Execute(sql);
            LoadFreeDevice();
            LoadExistGroup(SegmentID);
            LoadTerminal(GroupID);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string GroupID = gdGroup.CurrentRow.Cells["GroupID"].Value.ToString();
            string SegmentID = gdRoute.CurrentRow.Cells["SegmentID"].Value.ToString();
            string GroupType = gdGroup.CurrentRow.Cells["GroupType"].Value.ToString();
            if (GroupType != "外设")
            {
                MessageBox.Show("只能解除外设绑定");
                return;
            }
            if (MessageBox.Show("是否解除绑定？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = string.Format("update Base_DeviceInfo set MerchID='0',DeviceName='',Status=0 from Base_DeviceInfo,Data_MerchDevice where Base_DeviceInfo.ID=Data_MerchDevice.DeviceID and Data_MerchDevice.ID='{0}'; delete from Data_MerchDevice where ID='" + GroupID + "'", GroupID);
                DataAccess ac = new DataAccess();
                ac.Execute(sql);
                LoadFreeDevice();
                LoadExistGroup(SegmentID);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtGroupName.Text == "" || txtCoin.Text == "" || cbGroupType.Text == "")
            {
                MessageBox.Show("分组信息有为空");
                return;
            }
            string SegmentID = gdRoute.CurrentRow.Cells["SegmentID"].Value.ToString();
            string sql = string.Format("insert into Data_GameInfo (DeviceID,GroupName,GroupType,PushReduceFromCard,PushLevel,LotteryMode,OnlyExitLottery,ReadCat,chkCheckGift) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                SegmentID, txtGroupName.Text, cbGroupType.SelectedIndex, txtCoin.Text,
                rbCOHigh.Checked ? "1" : "0",
                rbLotteryEN.Checked ? "1" : "0",
                rbOnlyLotteryEN.Checked ? "1" : "0",
                rbReadcatEN.Checked ? "1" : "0",
                rbGiftEN.Checked ? "1" : "0"
                );
            DataAccess ac = new DataAccess();
            ac.Execute(sql);
            LoadExistGroup(SegmentID);
        }

        private void gdGroup_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string GroupID = gdGroup.CurrentRow.Cells["GroupID"].Value.ToString();
            string GroupType = gdGroup.CurrentRow.Cells["GroupType"].Value.ToString();
            if (GroupType == "分组")
            {
                LoadTerminal(GroupID);
            }
        }
    }
}
