using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace PalletService.Utility.PeopleCard
{
    public class PeopleCardModel
    {
        public string 身份证号 { get; set; }
        public string 姓名 { get; set; }
        public string 性别 { get; set; }
        public string 民族 { get; set; }
        public string 生日 { get; set; }
        public string 住址 { get; set; }
        public string 签发机关 { get; set; }
        public string 发证日期 { get; set; }
        public string 有效期限 { get; set; }
        public Bitmap 照片 { get; set; }
    }
}
