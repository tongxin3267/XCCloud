using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BLL
{
    /// <summary>
    /// 用于操作读卡器--USB模式(PHILIPH卡)
    /// create by wicky 2015-03-31
    /// </summary>
    public class Dcrf32
    {
        #region ----通用函数----
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, Int32 baud);  //初始化通讯口,port为100表示usb
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_exit(int icdev);  //关闭端口
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_card(int icdev, int _Mode, ref ulong _Snr);   //寻卡，能返回在工作区域内某张卡的序列号(该函数包含了dc_request,dc_anticoll,dc_select的整体功能)
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_request(int icdev, int _Mode, ref uint TagType); //寻卡请求,引用获取卡类型值
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_anticoll(int icdev, int _Bcnt, ref ulong _Snr);  //防卡冲突，返回卡的序列号,需要选执行
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_select(int icdev, ulong _Snr, ref int _Size);  //从多个卡中选取一个给定序列号的卡
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_load_key(int icdev, int mode, int secnr, [In] byte[] nkey);  //密码装载到读写模块中
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_authentication_passaddr(int icdev, int mode, int secnr, byte[] nkey);  //核对密码函数，用此函数时，可以不用执行dc_load_key()函数
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_authentication(int icdev, int _Mode, int _SecNr); //核对密码函数
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_read(int icdev, int adr, [Out] byte[] sdata);   //从卡中读数据
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_read(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_read_hex(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据(转换为16进制)
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_write(int icdev, int adr, [In] byte[] sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_write(int icdev, int adr, [In] string sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据(转换为16进制)
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_halt(int icdev);  //中止对该卡操作
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_des(string key, string sour, string dest, int m);  //DES算法加解密函数
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_changeb3(int icdev, string _SecNr, string _KeyA, int m);  //修改块3的数据(当为M1S70卡时，当扇区号大于31时，修改块15的数据(即一个扇区的最后一块))
        #endregion

        #region ----设备操作函数----
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_beep(int icdev, uint _Msec);  //蜂鸣
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_getver(int icdev, byte[] Databuffer);  //读取硬件版本号
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_srd_eeprom(int icdev, int offset, int length, ref int rec_buffer);  //读取读写器备注信息
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_swr_eeprom(int icdev, int offset, int length, ref int rec_buffer);  //向读写器备注区中写入信息
        [DllImport("dcrf32.dll")]
        public static extern Int16 a_hex(string oldValue, ref string newValue, int len);  //普通字符转换成十六进制字符
        [DllImport("dcrf32.dll")]
        public static extern void hex_a(ref string oldValue, ref string newValue, int len);  //十六进制字符转换成普通字符
        [DllImport("dcrf32.dll")]
        public static extern Int16 dc_reset(int icdev, uint sec);   //射频复位函数
        #endregion

    }
}
