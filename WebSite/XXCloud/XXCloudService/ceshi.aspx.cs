using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Common;

namespace XXCloudService
{
    public partial class ceshi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void btn1_Click(object sender, EventArgs e)
        {
            if (text1.Text != ""&TextBox2.Text!="")
            {
                string aa = Utils.EncryptDES(text1.Text, TextBox2.Text);
                lbl1.Text = aa;
            }
        }

        protected void btn2_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text != "" & TextBox2.Text != "")
            {
                string aa = Utils.DecryptDES(TextBox1.Text, TextBox2.Text);
                lbl1.Text = aa;
            }
        }
    }
}