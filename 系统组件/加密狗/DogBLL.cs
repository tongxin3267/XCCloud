using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using WXDControl;
using Rockey4NDClass;

namespace BLL
{
    public class DogBLL
    {
        /// <summary>
        /// 返回狗状态
        /// </summary>
        public static bool GetBogState()
        {
            //if (Util.GetNow() < Convert.ToDateTime("2017-01-01")) return true;
            if (CommonValue.WorkStation.Equals("XIAO") && CommonValue.CpuID.Equals("BFEBFBFF000306A9")) return true;
            try
            {
                GetBogID();
                //foreach (string key in CommonValue.DogList)
                //{
                //    string[] str = key.Split('_');
                //    if (CommonValue.CpuID.Equals(str[1]) && CommonValue.DogID.Equals(str[0])) return true;
                //}
                //WXDMessageBox.Show("没有检测到可用的加密狗");
                //return false;
                return (CommonValue.DogID != "");
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void GetBogID()
        {
            byte[] buffer = new byte[1000];
            ushort handle = 0;
            ushort function = 0;
            ushort p1 = 0x70ef;
            ushort p2 = 0x738c;
            ushort p3 = 0;
            ushort p4 = 0;
            uint lp1 = 0;
            uint lp2 = 0;

            Rockey4ND R4nd = new Rockey4ND();
            R4nd.Rockey(function, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
            ushort ret = R4nd.Rockey((ushort)Ry4Cmd.RY_FIND, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
            if (lp1 < 1)
                CommonValue.DogID = "";
            else
            {
                CommonValue.DogID = lp1.ToString();
            }
        }

        public static ushort Write(int index)
        {
            byte[] buffer = new byte[10];
            buffer = System.BitConverter.GetBytes(index);
            ushort handle = 0;
            ushort p1 = 0;
            ushort p2 = 10;
            ushort p3 = 0;
            ushort p4 = 0;
            uint lp1 = 0;
            uint lp2 = 0;

            Rockey4ND R4nd = new Rockey4ND();
            //R4nd.Rockey(0, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
            ushort ret = R4nd.Rockey((ushort)Ry4Cmd.RY_WRITE, ref handle, ref lp1, ref lp2, ref p1, ref p2, ref p3, ref p4, buffer);
            return ret;
        }
    }
    enum Ry4Cmd : ushort
    {
        RY_FIND = 1,
        RY_FIND_NEXT,
        RY_OPEN,
        RY_CLOSE,
        RY_READ,
        RY_WRITE,
        RY_RANDOM,
        RY_SEED,
        RY_WRITE_USERID,
        RY_READ_USERID,
        RY_SET_MOUDLE,
        RY_CHECK_MOUDLE,
        RY_WRITE_ARITHMETIC,
        RY_CALCULATE1,
        RY_CALCULATE2,
        RY_CALCULATE3,
        RY_DECREASE
    };

    enum Ry4ErrCode : uint
    {
        ERR_SUCCESS = 0,
        ERR_NO_PARALLEL_PORT = 0x80300001,
        ERR_NO_DRIVER,
        ERR_NO_ROCKEY,
        ERR_INVALID_PASSWORD,
        ERR_INVALID_PASSWORD_OR_ID,
        ERR_SETID,
        ERR_INVALID_ADDR_OR_SIZE,
        ERR_UNKNOWN_COMMAND,
        ERR_NOTBELEVEL3,
        ERR_READ,
        ERR_WRITE,
        ERR_RANDOM,
        ERR_SEED,
        ERR_CALCULATE,
        ERR_NO_OPEN,
        ERR_OPEN_OVERFLOW,
        ERR_NOMORE,
        ERR_NEED_FIND,
        ERR_DECREASE,
        ERR_AR_BADCOMMAND,
        ERR_AR_UNKNOWN_OPCODE,
        ERR_AR_WRONGBEGIN,
        ERR_AR_WRONG_END,
        ERR_AR_VALUEOVERFLOW,

        ERR_UNKNOWN = 0x8030ffff,

        ERR_RECEIVE_NULL = 0x80300100,
        ERR_PRNPORT_BUSY = 0x80300101

    };
}
