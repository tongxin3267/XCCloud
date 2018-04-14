using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PalletService.Utility.MemberCard;
using DeviceUtility.Utility.MemberCard.Model;
using System.Text.RegularExpressions;



namespace DeviceUtility.Utility.MemberCard
{
    /// <summary>
    /// 因读卡器要改为非接触式,故需要对此类做较大改动
    /// 底层dll操作方法只是做一个简单包装,防止改动太大
    /// 密码保存在块3
    /// update by wicky 2015-04-01
    /// </summary>
    public class ICCardUtility
    {

        #region----字段----
        /// <summary>
        /// 卡号写入的块位置4
        /// </summary>
        private const int ic_postion = 4;
        /// <summary>
        /// 密码写入的块位置7
        /// </summary>
        private const int pwd_postion = 7;
        /// <summary>
        /// 自己写入的卡号所在扇区1
        /// </summary>
        private const int secnr_index = 1;
        /// <summary>
        /// 原始IC卡号所在的扇区0
        /// </summary>
        private const int secnr_index_ic = 0;
        /// <summary>
        /// 空白卡0区初始密码:778852013144
        /// 000000000000FF078069778852013144    
        /// 
        /// 10000602000000000000000000000000    数字币
        /// 
        /// 10000113000000000000000000000000    ic卡
        /// </summary>
        public const string ICPass = "778852013144";
        /// <summary>
        /// 1区初始密码:FFFFFFFFFFFF
        /// </summary>
        public const string ICPass_one = "FFFFFFFFFFFF";
        /// <summary>
        /// 密码模式4
        /// </summary>
        public const int pwd_mode = 4;
        /// <summary>
        /// 空白卡初始密码
        /// </summary>
        public static byte[] B_ICPass = new byte[6] { 0x77, 0x88, 0x52, 0x01, 0x31, 0x44 };
        /// <summary>
        /// 卡号长度(含动态码)
        /// </summary>
        public static int len = 9;
        #endregion

        #region----操作底层dll方法----
        /// <summary>
        /// 初始化端口
        /// </summary>
        private static int IC_InitComm(short Port)
        {
            return Dcrf32.dc_init(Port, 57600);
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        private static short IC_ExitComm(int icdev)
        {
            return Dcrf32.dc_exit(icdev);
        }

        /// <summary>
        /// 防卡冲突，返回卡的序列号
        /// </summary>
        private static short dc_anticoll(int icdev,ref ulong _Snr)
        {
            return Dcrf32.dc_anticoll(icdev, 0, ref _Snr);
        }

        /// <summary>
        /// 卡下电(要重新审核密码才能继续操作),此方法已经无效
        /// </summary>
        private static short IC_Down(int icdev)
        {
            return 0;
        }

        /// <summary>
        /// 初始化卡型,此方法已经无效
        /// </summary>
        private static short IC_InitType(int icdev, int cardType)
        {
            return 0;
        }

        //判断连接是否成功,<0 ,连接不成功.0可以读写,1连接成功,但是没插卡.
        private static short IC_Status(int icdev)
        {
            uint tagType = 0;
            Int16 result = Dcrf32.dc_request(icdev, 2, ref tagType); //获取卡类型,为4,表示mifare卡
            return result;
        }

        /// <summary>
        /// 发出声音
        /// </summary>
        private static short IC_DevBeep(int icdev, uint intime)
        {
            return Dcrf32.dc_beep(icdev, intime);
        }

        /// <summary>
        /// 读设备硬件版本号
        /// </summary>
        private static int IC_ReadVer(int icdev, byte[] Databuffer)
        {
            return Dcrf32.dc_getver(icdev, Databuffer);
        }

        /// <summary>
        /// 检查原始密码(不等于0为校验失败)
        /// </summary>
        private static short IC_CheckPass_4442hex(int icdev, string Password, int index = secnr_index)
        {
            Int16 result = 0;
            ulong icCardNo = 0;
            //B_ICPass = new byte[6] { 0x77, 0x88, 0x52, 0x01, 0x31, 0x45 };
            for (int i = 0; i < 6; i++)
            {
                B_ICPass[i] = Convert.ToByte(Password.Substring(i*2,2),16);
            }
            //B_ICPass = Encoding.ASCII.GetBytes(Password); 
            result = Dcrf32.dc_load_key(icdev, pwd_mode, index, B_ICPass);    //密码加载到设备
            result = Dcrf32.dc_card(icdev, pwd_mode, ref icCardNo);    //读取卡号,校验之前必须调用
            result = Dcrf32.dc_authentication(icdev, pwd_mode, index);     //校验出厂密码
            return result;

        }

        /// <summary>
        /// 更改密码(0为更改成功)
        /// </summary>
        private static short IC_ChangePass_4442hex(int icdev, int pwd_postion,string oldPassWord, string newPassWord)
        {
            Int16 result = 0;
            result = Dcrf32.dc_authentication(icdev, pwd_mode, secnr_index);
            result = ICCardUtility.IC_CheckPass_4442hex(icdev, oldPassWord, secnr_index); //校验出厂密码
            if (result != 0) return result;
            string data = newPassWord + "FF078069" + newPassWord;
            result = Dcrf32.dc_write_hex(icdev, pwd_postion, data);
            return result;
        }

        /// <summary>
        /// 更改密码(0为更改成功)
        /// </summary>
        private static short IC_ChangePass(int icdev, int pwd_postion,int secnr_index, string oldPassWord, string newPassWord)
        {
            Int16 result = 0;
            result = Dcrf32.dc_authentication(icdev, pwd_mode, secnr_index);
            result = ICCardUtility.IC_CheckPass_4442hex(icdev, oldPassWord, secnr_index); //校验出厂密码
            if (result != 0) return result;
            string data = newPassWord + "FF078069" + newPassWord;
            result = Dcrf32.dc_write_hex(icdev, pwd_postion, data);
            return result;
        }

        /// <summary>
        /// 在固定的位置写入固定长度的数据
        /// </summary>
        private static short IC_Write_hex(int icdev, int offset, int Length, string Databuffer)
        {
            Int16 result = 0;
            result = Dcrf32.dc_authentication(icdev, pwd_mode, getSecNr(offset));    //校验第0套密码;
            if (result != 0) return result;
            string data = addZero(Databuffer, 32, true);
            result = Dcrf32.dc_write_hex(icdev, offset, data);
            return result;
        }
        /// <summary>
        /// 在固定的位置写入固定长度的数据
        /// </summary>
        private static short IC_Write(int icdev, int offset, int Length, byte[] Databuffer)
        {
            Int16 result = 0;
            result = Dcrf32.dc_authentication(icdev, pwd_mode, getSecNr(offset));    //校验第0套密码;
            if (result != 0) return result;
            result = Dcrf32.dc_write(icdev, offset, Databuffer);
            return result;
        }

        /// <summary>
        /// 在固定的位置读出固定长度的数据
        /// </summary>
        private static short IC_Read(int icdev, int offset, int len, byte[] Databuffer)
        {
            Int16 result = 0;
            result = Dcrf32.dc_authentication(icdev, pwd_mode, getSecNr(offset));    //校验第0套密码;
            if (result != 0) return result;
            result = Dcrf32.dc_read(icdev, offset, Databuffer);     //获取数据
            return result;
        }
        /// <summary>
        /// 在固定的位置读出固定长度的数据
        /// </summary>
        private static short IC_Read_hex(int icdev, int offset, int len, StringBuilder Databuffer)
        {
            Int16 result = 0;
            result = Dcrf32.dc_authentication(icdev, pwd_mode, getSecNr(offset));    //校验第0套密码;
            if (result != 0) return result;
            result = Dcrf32.dc_read_hex(icdev, offset, Databuffer);     //获取数据
            return result;
        }
        #endregion

        #region----私有辅助方法----
        /// <summary>
        /// 根据块地址0-63计算扇区0-15
        /// </summary>
        private static int getSecNr(int offset)
        {
            double offsetf = offset;
            double secNr = offsetf / 4.0;
            if (secNr < 0) secNr = 0;
            if (secNr > 63) secNr = 15;
            return (int)secNr;
        }

        /// <summary>
        /// 将字符串转成byte[]并在后面补齐16字节
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        private static byte[] get16Byte(string Password)
        {
            byte[] a = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] b = Encoding.ASCII.GetBytes(Password);
            for (int i = 0; i < b.Length; i++)
            {
                a[i] = b[i];
            }
            return a;
        }
        /// <summary>
        /// 补0
        /// 参数：
        ///     str,需要补0的字符串
        ///     len,返回的字符串的长度
        ///     isBack,是否在后面补0
        /// </summary>
        private static string addZero(string str, int len, bool isBack = false)
        {
            string temp = str;
            int count = len - temp.Length;
            if (isBack)
            {
                //后面补0
                for (int i = 0; i < count; i++)
                {
                    temp += "0";
                }
            }
            else
            {
                string s = "";
                //前面补0
                for (int i = 0; i < count; i++)
                {
                    s += "0";
                }
                temp = s + temp;
            }
            return temp;
        }

        /// <summary>
        /// 基础校验,检查读卡器与IC卡是否都存在
        /// 默认蜂鸣true
        /// 存在返回ok,不存在返回错误信息
        /// </summary>
        private static string check(int icdev, bool isBeep = true)
        {
            if (icdev <= 0)
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
                return "提示：IC读卡器USB初始化失败！";
            }
            int iReadDev = -1;
            byte[] sdata = new byte[9];

            iReadDev = ICCardUtility.IC_ReadVer(icdev, sdata);
            if (iReadDev < 0)
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
                return "提示：无法找到IC读卡器硬件版本号！";
            }
            short st = 0;
            st = ICCardUtility.IC_InitType(icdev, 16);
            if (st < 0)
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
                return "提示：设置IC卡类型失败！";
            }
            if (isBeep) st = ICCardUtility.IC_DevBeep(icdev, 10);//等待10毫秒
            st = ICCardUtility.IC_Status(icdev);
            if (st == 1)
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
                return "提示：读卡器中没有插卡";
            }
            if (st != 0)
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
                return "提示：无法连接到读卡器";
            }

            return "ok";
        }

        private static string toASCII(string iccard)
        {
            string str = "";
            byte[] array = System.Text.Encoding.ASCII.GetBytes(iccard);
            for (int i = 0; i < array.Length; i++)
            {
                int asciicode = (int)(array[i]);
                str += Convert.ToString(asciicode);
            }
            return str;
        }
        /// <summary>
        /// 删除1区数据并恢复出厂密码FFFFFFFFFFFF
        /// </summary>
        /// <param name="icdev">读卡器ID</param>
        /// <param name="pwd">1区密码</param>
        /// <returns>成功true</returns>
        private static bool DeleteICCaredData(int icdev,string pwd)
        {
            byte[] b = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int st = ICCardUtility.IC_CheckPass_4442hex(icdev, pwd);     //下载密码到读卡器
            if (st == 0)
            {
                st = ICCardUtility.IC_Write(icdev, ic_postion, len, b);  //清空卡号
                int st1 = ICCardUtility.IC_ChangePass_4442hex(icdev, pwd_postion, pwd, ICPass_one); //恢复原出厂密码
                if (st == 0 && st1 == 0)
                {
                    return true;
                }
            }
            
            return false;
        }
        #endregion

        #region----公开方法----
        /// <summary>
        /// 检查0区与1区卡号是否一致
        /// </summary>
        public static bool CheckOneTwo(string storePassword)
        {
            string one = GetCardID(0, 1, storePassword);
            string two = GetCardID(1, 4, storePassword);
            if (!one.Equals(two)) return false;
            return true;
        }
        public static string GetUid()
        {
            int icdev = 0;
            icdev = ICCardUtility.IC_InitComm(100);
            string checkStr = check(icdev,false);
            if (!checkStr.Equals("ok")) return checkStr;
            //获得IC卡UID
            ulong uid = 0;
            int st = dc_anticoll(icdev, ref uid);
            ICCardUtility.IC_ExitComm(icdev);
            return uid.ToString();
        }
        /// <summary>
        /// 初始化0区,参数:0区新卡号和原密码
        /// </summary>
        public static string WriteICCard(string card,string pwd)
        {
            int icdev = 0;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                string checkStr = check(icdev);
                if (!checkStr.Equals("ok")) return checkStr;
                int st = ICCardUtility.IC_CheckPass_4442hex(icdev, pwd, 0);     //下载密码到读卡器
                if (st == 0)
                {
                    st = ICCardUtility.IC_Write_hex(icdev, 1, len, card);  //写卡号
                    int st1 = ICCardUtility.IC_ChangePass(icdev, 3, 0, pwd, ICPass); //写密码
                    if (st == 0 && st1 == 0)
                    {
                        return "Success";
                    }
                }
                return "操作失败";
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 读卡器蜂鸣
        /// </summary>
        public static void Beep(int time)
        {
            int icdev = 0;
            icdev = ICCardUtility.IC_InitComm(100); //初始化usb
            ICCardUtility.IC_DevBeep(icdev, (uint)time);//等待10毫秒
            ICCardUtility.IC_ExitComm(icdev);
        }
        /// <summary>
        /// 修改动态密码
        /// </summary>
        public static string ChangeRandomCode(string sNewICCard, string sRepeatCode, string storePassword,bool isBeep = true)
        {
            int icdev = 0;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                string checkStr = "";
                checkStr = check(icdev, isBeep);  //基本检查

                if (!checkStr.Equals("ok")) return checkStr;

                short st = 0;
                //校验出厂密码
                //1区的出厂密码和0区出厂密码不同,0区778852013144 1区FFFFFFFFFFFF
                st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass_one);
                if (st != 0)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword); //校验本店密码
                    if (st != 0)
                    {
                        return "提示：无法通过密码校验，可能不是本店的卡。";
                    }
                }
                byte[] b1 = new byte[16];
                byte[] b2 = Encoding.ASCII.GetBytes(sNewICCard);
                Array.Copy(b2, b1, b2.Length);
                b1[b2.Length] = (byte)Convert.ToInt32(sRepeatCode);
                st = ICCardUtility.IC_Write(icdev, ic_postion, len, b1);
                if (st != 0)
                {
                    return "提示：无法写入动态密码。";
                }
                
                return "Success";
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 获取当前读卡器上的IC卡号码,返回8位的卡号或错误信息
        /// 此方法自行处理卡号+加动态码,返回最终卡号,与ReadICCard不同
        /// </summary>
        public static bool GetICCardID(string storePassword,out string icCardId,out string repeatCode,out string errMsg,bool isBeep = true, bool isCreate = false)
        {
            icCardId = string.Empty;
            errMsg = string.Empty;
            string sICCardID = string.Empty;
            if (!ICCardUtility.ReadICCard(storePassword, out sICCardID, out repeatCode,out errMsg,isBeep, isCreate))
            {
                return false;
            }

            if (IsValidCardNo(sICCardID))
            {
                try
                {
                    string sRepeatCode = Convert.ToInt32(sICCardID.Substring(8), 16).ToString();
                    sICCardID = sICCardID.Substring(0, 8);
                    int iICCardID = 0;
                    if (ICCardUtility.isNumberic(sICCardID, out iICCardID))
                    {
                        icCardId = iICCardID.ToString();
                    }
                    else
                    {
                        icCardId = sICCardID;
                    }
                    return true;
                }
                catch
                {
                    errMsg = "读取卡片出错";
                    return false;
                }
            }
            else
            {
                errMsg = sICCardID;
                return false;
            }

            return true;
        }
        /// <summary>
        /// 仅仅读取卡号(不校验)参数扇区号0-15，块号0-63
        /// </summary>
        public static string GetCardID(int index, int postion, string storePassword, bool isBeep = false)
        {
            string sICCardID = "";
            string checkStr = "";
            int icdev = 1;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  
                checkStr = check(icdev, isBeep);  //基本检查
                if (!checkStr.Equals("ok")) return checkStr;

                short st = 0;
                if (index == 0)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass, index);
                    if (st != 0)
                    {
                        st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass_one, index);
                        if (st != 0) return "提示：无法读取IC卡信息。";
                    }
                    StringBuilder data = new StringBuilder();
                    st = ICCardUtility.IC_Read_hex(icdev, postion, 8, data); //新卡块4无卡号,改为读块1的原始卡号
                    if (st == 0) return data.ToString().Substring(0, 8);
                    return "提示：无法读取IC卡信息。";
                }
                else
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword, index);
                    if (st != 0)
                    {
                        st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass_one, index);
                        if (st != 0) return "提示：无法读取IC卡信息。";
                    }
                    byte[] data = new byte[8];
                    st = ICCardUtility.IC_Read(icdev, postion, 8, data);
                    if (st == 0)
                    {
                        sICCardID = Encoding.ASCII.GetString(data).Replace("\0", "0");
                        return sICCardID;
                    }
                    return "提示：无法读取IC卡信息。";
                }
                
            }
            finally
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 读卡器按会员IC卡号写一张新卡
        /// </summary>
        public static bool CreateICCard(string sNewICCard, string sRepeatCode, string storePassword, out string errMsg, bool isBeep = true)
        {
            int icdev = 0;
            errMsg = string.Empty;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                string checkStr = "";
                checkStr = check(icdev, isBeep);  //基本检查

                if (!checkStr.Equals("ok"))
                {
                    errMsg = checkStr;
                    return false; 
                } 

                short st = 0;
                //获得IC卡UID
                ulong uid = 0;
                st = dc_anticoll(icdev, ref uid);

                //校验出厂密码
                //1区的出厂密码和0区出厂密码不同,0区778852013144 1区FFFFFFFFFFFF
                st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass_one); 
                if (st != 0)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword); //校验本店密码
                    if (st == 0)
                    {
                        errMsg = "提示：此卡正在使用，请换一张空白卡。";
                        return false;
                    }
                    else
                    {
                        errMsg = "提示：无法通过密码校验，可能不是本店的卡。";
                        return false;
                    }
                }

                byte[] b1 = new byte[16];
                byte[] b2 = Encoding.ASCII.GetBytes(sNewICCard);
                Array.Copy(b2, b1, b2.Length);
                b1[b2.Length] = (byte)Convert.ToInt32(sRepeatCode);
                st = ICCardUtility.IC_Write(icdev, ic_postion, len, b1);
                if (st != 0)
                {
                    errMsg = "提示：无法写入IC卡号码。";
                    return false;
                }
                st = ICCardUtility.IC_ChangePass_4442hex(icdev, pwd_postion, ICPass_one, storePassword);
                if (st != 0)
                {
                    errMsg = "提示：无法设置IC卡密码。";
                    return false;
                }
                return true;
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 读卡器按会员IC卡号写一张新卡
        /// </summary>
        public static string WriteICCardID(string sNewICCard, string sRepeatCode, string storePassword, bool isBeep = true)
        {
            int icdev = 0;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                string checkStr = "";
                checkStr = check(icdev, isBeep);  //基本检查

                if (!checkStr.Equals("ok")) return checkStr;

                short st = 0;
               
                //校验出厂密码
                //1区的出厂密码和0区出厂密码不同,0区778852013144 1区FFFFFFFFFFFF
                st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass_one);
                if (st != 0)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword); //校验本店密码
                    if (st != 0)
                    {
                        return "提示：无法通过密码校验，可能不是本店的卡。";
                    }
                }
                else
                {
                    st = ICCardUtility.IC_ChangePass_4442hex(icdev, pwd_postion, ICPass_one, storePassword);
                    if (st != 0)
                    {
                        return "提示：无法设置IC卡密码。";
                    }
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword); //校验本店密码
                }
                byte[] b1 = new byte[16];
                byte[] b2 = Encoding.ASCII.GetBytes(sNewICCard);
                Array.Copy(b2, b1, b2.Length);
                b1[b2.Length] = (byte)Convert.ToInt32(sRepeatCode);
                st = ICCardUtility.IC_Write(icdev, ic_postion, len, b1);
                if (st != 0)
                {
                    return "提示：无法写入IC卡号码。";
                }
                return "Success";
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 读取16进制的卡号(含动态码的卡号)
        /// 参数：isBeep 是否需要蜂鸣,
        /// isCreate是入库或入会类型的操作,此类操作空白卡需要返回卡号
        /// 校验逻辑：1检查设备 2.检查密码 3.检查是不是空白卡
        /// 返回包含动态码的完整卡号,开新卡的业务只返回卡号
        /// </summary>
        public static bool ReadICCard(string storePassword,out string icCardId,out string repeatCode,out string errMsg,bool isBeep = true, bool isCreate = false)
        {
            string checkStr = "";
            icCardId = string.Empty;
            repeatCode = string.Empty;
            errMsg = string.Empty;
            int icdev = 0;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  
                checkStr = check(icdev, isBeep);  //基本检查
                if (!checkStr.Equals("ok"))
                {
                    errMsg = checkStr;
                    return false;
                }

                short st = 0;

                //开新卡
                if (isCreate)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword, secnr_index);
                    if (st == 0)
                    {
                        errMsg = "提示：此卡正在使用。";
                        return false;
                    }
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass, secnr_index_ic);
                    if (st != 0)
                    {
                        errMsg = "提示：无法读取IC卡信息。";
                        return false;
                    }

                    StringBuilder data = new StringBuilder();
                    st = ICCardUtility.IC_Read_hex(icdev, 1, len - 1 , data); //新卡块4无卡号,改为读块1的原始卡号
                    if (st == 0)
                    {
                        errMsg = data.ToString().Substring(0, len - 1);
                        return true;
                    }
                }
                else
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass_one, secnr_index);
                    if (st == 0)
                    {
                        errMsg = "提示：此卡是一张空白卡，不需要处理。";
                        return false;
                    }
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword, secnr_index);
                    if (st != 0)
                    {
                        errMsg = "提示：无法通过密码校验，可能不是本店的卡。";
                        return false;
                    }

                    byte[] data = new byte[len];
                    st = ICCardUtility.IC_Read(icdev, ic_postion, len, data);
                    if (st == 0)
                    {
                        byte[] b1 = new byte[len - 1];
                        Array.Copy(data, b1, b1.Length);
                        byte b2 = data[len - 1];
                        string s = Encoding.ASCII.GetString(b1) + b2.ToString("X2");                        
                        repeatCode = Convert.ToInt32(s.Substring(8), 16).ToString();
                        string sICCardID = s.Substring(0, 8).Replace("\0", "");
                        icCardId = s;
                        return true;

                    }
                }
                errMsg = "提示：无法读取IC卡信息。";
                return false;
            }
            finally
            {
                ICCardUtility.IC_Down(icdev);
                ICCardUtility.IC_ExitComm(icdev);
            }

        }
        /// <summary>
        /// 判断读卡器中插入的是一张空卡
        /// </summary>
        public static bool CheckNullCard(string storePassword)
        {
            int icdev = 0;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                if (icdev <= 0)
                {
                    return false;
                }

                int iReadDev = -1;
                byte[] sdata = new byte[9];

                iReadDev = ICCardUtility.IC_ReadVer(icdev, sdata);
                if (iReadDev < 0)
                {
                    return false;
                }

                short st = 0;
                st = ICCardUtility.IC_InitType(icdev, 16);
                if (st < 0)
                {
                    return false;
                }

                st = ICCardUtility.IC_DevBeep(icdev, 10);//等待10毫秒
                st = ICCardUtility.IC_Status(icdev);
                if (st == 1)
                {
                    return false;
                }
                if (st != 0)
                {
                    return false;
                }

                st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass);
                if (st != 0)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword);
                    if (st == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 读卡器回收卡
        /// </summary>
        public static bool RecoveryICCard(string storePassword,out string errMsg)
        {
            int icdev = 0;
            errMsg = string.Empty;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                string checkStr = check(icdev);
                if (!checkStr.Equals("ok"))
                {
                    errMsg = checkStr;
                    return false;
                }

                if (DeleteICCaredData(icdev, ICPass_one))
                {
                    return true;
                }
                if (DeleteICCaredData(icdev, storePassword))
                {
                    return true;
                }
                if (DeleteICCaredData(icdev, ICPass))
                {
                    return true;
                }
                errMsg = "此卡不是新卡也不是本店的卡,无法通过密码校验";
                return true;
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        
        /// <summary>
        /// 每次插卡时生成新的IC卡序列号
        /// </summary>
        public static string RefreshICCard(string sNewICCard, string sRepeatCode, string storePassword)
        {
            int icdev = 0;
            try
            {
                icdev = ICCardUtility.IC_InitComm(100); //初始化usb  

                string checkStr = check(icdev);
                if (!checkStr.Equals("ok")) return checkStr;

                short st = 0;

                st = ICCardUtility.IC_CheckPass_4442hex(icdev, ICPass);
                if (st != 0)
                {
                    st = ICCardUtility.IC_CheckPass_4442hex(icdev, storePassword);
                    if (st != 0)
                    {
                        return "无法通过密码校验，可能不是本店的卡。";
                    }
                }
                else
                {
                    return "此卡是一张空白卡不能刷新。";
                }

                byte[] b1 = new byte[16];
                byte[] b2 = Encoding.ASCII.GetBytes(sNewICCard);
                Array.Copy(b2, b1, b2.Length);
                b1[b2.Length] = (byte)Convert.ToInt32(sRepeatCode);
                st = ICCardUtility.IC_Write(icdev, ic_postion, len, b1);
                if (st != 0)
                {
                    return "无法写入IC卡号码。";
                }
                return "Success";
            }
            finally
            {
                //对卡下电，对于逻辑加密卡，下电后必须密码变为有效，即要写卡必须重新校验密码。
                ICCardUtility.IC_Down(icdev);
                //关闭端口，在Windows系统中，同时只能有一个设备使用端口，所以在退出系统时，请关闭端口，以便于其它设备使用。
                ICCardUtility.IC_ExitComm(icdev);
            }
        }
        /// <summary>
        /// 会员编号是数字
        /// </summary>
        public static bool isNumberic(string message, out int result)
        {
            //判断是否为整数字符串
            //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
            result = -1;   //result 定义为out 用来输出值
            try
            {
                //当数字字符串的为是少于4时，以下三种都可以转换，任选一种
                //如果位数超过4的话，请选用Convert.ToInt32() 和int.Parse()

                //result = int.Parse(message);
                //result = Convert.ToInt16(message);
                result = Convert.ToInt32(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 会员编号是数字
        /// </summary>
        public static bool isNumberic(string message)
        {
            //判断是否为整数字符串
            //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
            int result = -1;
            try
            {
                //当数字字符串的为是少于4时，以下三种都可以转换，任选一种
                //如果位数超过4的话，请选用Convert.ToInt32() 和int.Parse()

                //result = int.Parse(message);
                //result = Convert.ToInt16(message);
                result = Convert.ToInt32(message);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 金额判定
        /// </summary>
        public static bool isMoney(string message)
        {
            //判断是否为整数字符串
            //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
            decimal result = -1;   //result 定义为out 用来输出值
            try
            {
                //当数字字符串的为是少于4时，以下三种都可以转换，任选一种
                //如果位数超过4的话，请选用Convert.ToInt32() 和int.Parse()

                result = Convert.ToDecimal(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        public static bool IsValidCardNo(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9a-zA-Z]*[.]?[0-9a-zA-Z]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

    }
}
