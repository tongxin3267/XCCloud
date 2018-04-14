using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using SQLDAL;
using BLL;
using System.Data;
using Model;
using System.Windows.Forms;

namespace ICGame
{
    /// <summary>
    /// 售币机业务处理类
    /// </summary>
    public class SerialAPI
    {
        #region--属性和委托--
        /// <summary>
        /// 1号电机启用
        /// </summary>
        public static string CanWork1 = "0";
        /// <summary>
        /// 2号电机启用
        /// </summary>
        public static string CanWork2 = "0";
        /// <summary>
        /// 串口号
        /// </summary>
        public static string SaleCoinerCOM = "COM1";
        /// <summary>
        /// 串口波特率57600
        /// </summary>
        public static string SaleCoinerPort = "57600";
        /// <summary>
        /// 马达1币数
        /// </summary>
        public static int Motor1Coin = 1;
        /// <summary>
        /// 马达2币数
        /// </summary>
        public static int Motor2Coin = 1;
        /// <summary>
        /// 售币机类型1四位2六位
        /// </summary>
        public static int WorkType = 1;
        /// <summary>
        /// 是否允许数字币
        /// </summary>
        public static bool DigitEn = false;
        /// <summary>
        /// 准备销售的数字币
        /// </summary>
        public List<UInt32> ReadySaleCoin = new List<uint>();

        Thread processThread = null;                        //数据处理线程
        //static Queue<string> RecieveQueue = new Queue<string>();   //接收队列
        static Queue<List<byte>> RecieveByteQueue = new Queue<List<byte>>();   //接收队列
        bool threadRuning = false;                          //线程运行标志 
        SerialPort com = new SerialPort();

        int sn = 0; //当前接收指令流水号

        public string MustCoin { get; set; }

        public string OutCoin { get; set; }
        /// <summary>
        /// 是否启用双光眼
        /// </summary>
        public static bool AllowDouble = false;
        /// <summary>
        /// 1#马达数币状态，0缺币 1完成 2常开光眼故障 3常闭光眼故障
        /// </summary>
        public int CurMotor1Status = 0;
        /// <summary>
        /// 2#马达数币状态，0缺币 1完成 2常开光眼故障 3常闭光眼故障
        /// </summary>
        public int CurMotor2Status = 0;

        /// <summary>
        /// 售币机在线状态
        /// </summary>
        public bool SaleCoinerState { get; set; }
        /// <summary>
        /// 是否正在售币，售币期间发送售币指令
        /// </summary>
        public bool SaleCoinerRunning { get; set; }

        public class CoinStatus
        {
            public bool bit0马达1缺币或卡币 = false;
            public bool bit1马达2缺币或卡币 = false;
            public bool bit2马达1常开光眼故障 = false;
            public bool bit3马达1常闭光眼故障 = false;
            public bool bit4马达2常开光眼故障 = false;
            public bool bit5马达2常闭光眼故障 = false;
            public bool bit6马达1币量不足 = false;
            public bool bit7马达2币量不足 = false;
        }
        /// <summary>
        /// 当前数币马达状态
        /// </summary>
        public CoinStatus curCoinStatus = new CoinStatus();

        public delegate void DelegShowRecvCoin(int CurCount);
        public event DelegShowRecvCoin OnShowRecvCoin;
        public void ShowRecvCoin(int CurCount)
        {
            if (OnShowRecvCoin != null)
            {
                OnShowRecvCoin(CurCount);
            }
        }

        public delegate void OutCoinSuccess();
        /// <summary>
        /// 出币完成事件
        /// </summary>
        public event OutCoinSuccess OutCoinSuccessMessage;

        public delegate void OutCoinError();
        /// <summary>
        /// 出币错误事件
        /// </summary>
        public event OutCoinError OutCoinErrorMessage;

        public delegate void CoinerState();
        /// <summary>
        /// 初始化售币机返回结果事件
        /// </summary>
        public event CoinerState CoinerStateMessage;

        #endregion

        #region 重发处理机制

        class RecvItem
        {
            /// <summary>
            /// 流水号
            /// </summary>
            public UInt16 SN = 0;
            /// <summary>
            /// 指令类别
            /// </summary>
            public byte CmdType = 0;
            /// <summary>
            /// 最后重发时间
            /// </summary>
            public DateTime ResendTime;
        }
        /// <summary>
        /// 重发指令列表
        /// </summary>
        List<RecvItem> RepeatList = new List<RecvItem>();

        bool CheckRepaeat(UInt16 sn, byte cmdType)
        {
            DateTime d = DateTime.Now;
            lock (RepeatList)
            {
                foreach (RecvItem item in RepeatList)
                {
                    if (item.CmdType == cmdType && item.SN == sn)
                    {
                        item.ResendTime = d;
                        return true;
                    }
                }
                RecvItem r = new RecvItem();
                r.CmdType = cmdType;
                r.SN = sn;
                r.ResendTime = d;
                RepeatList.Add(r);
            }
            return false;
        }

        Thread rpThread = null;

        /// <summary>
        /// 初始化重复检查线程
        /// </summary>
        void InitRepeat()
        {
            if (rpThread == null)
            {
                rpThread = new Thread(new ThreadStart(CheckTimeout));
                rpThread.IsBackground = true;
                rpThread.Start();
            }
        }
        void CheckTimeout()
        {
            while (true)
            {
                DateTime d = DateTime.Now;
                lock (RepeatList)
                {
                line1:
                    foreach (RecvItem item in RepeatList)
                    {
                        if (item.ResendTime.AddSeconds(30) < d)
                        {
                            //超时清除重发队列
                            RepeatList.Remove(item);
                            goto line1;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }


        #endregion

        #region--事件及方法--
        /// <summary>
        /// 获取售币机的设置值
        /// </summary>
        public static void GetSaleCoinerParameter()
        {
            DeviceCoin m = new DeviceCoin();
            string sql = String.Format("select id from t_device where WorkStation='{0}' and type='售币机' and ConnType='串口通讯' and state='启用'", CommonValue.WorkStation);
            DataSet set = WickyDAL.Query(sql);
            if (set.Tables[0].Rows.Count == 0) return;
            DataRow r = set.Tables[0].Rows[0];
            m = (DeviceCoin)new DeviceBLL().GetDevice(r["id"].ToString());
            SaleCoinerCOM = String.IsNullOrEmpty(m.port_name) ? "COM1" : m.port_name;
            SaleCoinerPort = String.IsNullOrEmpty(m.baute_rate) ? "57600" : m.baute_rate;
            CanWork1 = String.IsNullOrEmpty(m.motor1) ? "0" : m.motor1;
            CanWork2 = String.IsNullOrEmpty(m.motor2) ? "0" : m.motor2;
            WorkType = String.IsNullOrEmpty(m.nixie_tube_type) ? 1 : Convert.ToInt32(m.nixie_tube_type);
            Motor1Coin = Convert.ToInt32(m.motor1_coin);
            Motor2Coin = Convert.ToInt32(m.motor2_coin);
            DigitEn = (m.DigitCoin == "1");
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select * from t_parameters where system='chkDouble'");
            if (dt.Rows.Count > 0)
            {
                AllowDouble = (dt.Rows[0]["ParameterValue"].ToString() == "1");
            }
        }
        /// <summary>
        /// 初始化串口
        /// </summary>
        public SerialAPI()
        {
            try
            {
                com.PortName = SaleCoinerCOM;
                com.BaudRate = int.Parse(SaleCoinerPort);
                com.ReceivedBytesThreshold = 1;
                com.Open();
                com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);

                processThread = new Thread(new ThreadStart(ProcessMethod));         //数据处理线程实例化
                processThread.Start();                                              //数据处理线程启动
                threadRuning = true;

                InitRepeat();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 串口接收事件
        /// </summary>
        private void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (com.BytesToRead > 0)
                {
                    byte readByte = (byte)com.ReadByte();
                    Console.Write(Convert.ToString(readByte, 16) + " ");
                    CheckRecvData(readByte);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// 去头尾
        /// </summary>
        private static string GetContent(List<byte> list)
        {
            string sRec = "";
            for (int i = 1; i < list.Count - 3; i++)
            {
                sRec += list[i].ToString("X2") + " ";
            }
            return sRec;
        }
        /// <summary>
        /// 校验crc
        /// </summary>
        private static bool CheckCRC(List<byte> list, out List<byte> read)
        {
            read = new List<byte>();

            int len = list[2];

            //List<byte> sendData = new List<byte>();
            //for (int i = 0; i < list.Count - 4; i++)
            //{
            //    sendData.Add(list[i]);
            //}

            List<byte> sendData = new List<byte>();
            for (int i = 0; i < len + 3; i++)
            {
                sendData.Add(list[i]);
            }
            ushort crcValue = CRC.Crc16(sendData.ToArray(), (byte)sendData.Count);
            byte[] b = BitConverter.GetBytes(crcValue);
            if (b[0] == list[len + 3] && b[1] == list[len + 4])
            {
                read.AddRange(list.GetRange(0, len + 6));
                read.Add(0xfe);
                return true;
            }
            return false;
        }

        static int i_BlankCount = 0;  //收到引导字符个数
        static bool b_IsStart = false;  //是否收到帧头
        static bool b_IsEnd = false;    //是否收到帧尾
        static List<byte> recvBuf = new List<byte>();
        static int RecvFlag = 0;        //接收状态

        static void CheckRecvData(byte data)
        {
            switch (RecvFlag)
            {
                case 0:
                    if (data == 0xfe)
                        RecvFlag++;
                    break;
                case 1:
                    if (data == 0x68)
                    {
                        RecvFlag++;         //检测到有效帧头准备接收数据
                        recvBuf.Clear();
                        recvBuf.Add(data);
                    }
                    else if (data != 0xfe)
                        RecvFlag = 0;
                    break;
                case 2:
                    recvBuf.Add(data);
                    if (recvBuf.Count > 6)
                    {
                        if (recvBuf[recvBuf.Count - 1] == 0xfe && recvBuf[recvBuf.Count - 2] == 0x16)
                        {
                            //检测到完整的帧
                            string sRec = GetContent(recvBuf);
                            Console.WriteLine("接收：" + sRec);
                            Console.WriteLine();
                            List<byte> ReadList = new List<byte>();
                            //RecieveQueue.Enqueue(sRec);
                            if (CheckCRC(recvBuf, out ReadList))
                            {
                                List<byte> GetData = new List<byte>(ReadList);
                                RecieveByteQueue.Enqueue(GetData);
                            }
                            else
                            {
                                Console.WriteLine("校验失败");
                            }
                            recvBuf.Clear();
                            RecvFlag = 0;
                        }
                        else if (recvBuf[recvBuf.Count - 1] == 0xfe && recvBuf[recvBuf.Count - 2] == 0x16)
                        {
                            Console.WriteLine();
                            recvBuf.Clear();
                            RecvFlag = 0; ;
                        }
                        else if (recvBuf[recvBuf.Count - 2] == 0xfe && recvBuf[recvBuf.Count - 1] == 0x68)
                        {
                            Console.WriteLine();
                            recvBuf.Clear();
                            recvBuf.Add(0x68);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void SerialClose()
        {
            threadRuning = false;
            com.Close();
            com.Dispose();
        }

        static string BytesToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16) + " ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 数据处理线程方法
        /// </summary>
        private void ProcessMethod()
        {
            while (threadRuning)                //线程开关
            {
                if (RecieveByteQueue.Count > 0)
                {
                    List<byte> recv = RecieveByteQueue.Dequeue();   //接收队列出队
                    if (recv.Count > 0)
                    {
                        switch (recv[1])
                        {
                            case 0xf1:  //初始化应答
                                ProcessF1();
                                break;
                            case 0xf2:  //售币指令接收应答
                                ProcessF2(recv);
                                break;
                            case 0x03:  //售币机停止工作
                                byte[] b = new byte[2];
                                b[1] = recv[6];
                                b[0] = recv[7];
                                AskData(0xf3, b);
                                UInt16 snNum = BitConverter.ToUInt16(b, 0);
                                if (sn != snNum)
                                {
                                    sn = snNum;
                                    Console.WriteLine("售币结束");
                                    Process03(recv);
                                }
                                break;
                            case 0x04:
                                Process04();
                                break;
                            case 0xf5:  //数字币卡号上报
                                ProcessF5(recv);
                                break;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        void ProcessF5(List<byte> Data)
        {
            int n = 0;
            int coins = Data[3];
            int finish = Data[4];

            List<byte> dataList = new List<byte>();
            List<byte> sendData = new List<byte>();
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            dataList.Add(0x68);
            dataList.Add(0x05);
            dataList.Add(0x02);
            dataList.Add(Data[Data.Count - 6]);
            dataList.Add(Data[Data.Count - 5]);
            ushort crcValue = CRC.Crc16(dataList.ToArray(), (byte)dataList.Count);
            dataList.AddRange(BitConverter.GetBytes(crcValue));
            sendData.AddRange(dataList);
            sendData.Add(0x16);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);

            com.Write(sendData.ToArray(), 0, sendData.Count);

            if (!CheckRepaeat(BitConverter.ToUInt16(Data.ToArray(), Data.Count - 6), 0xf5))
            {
                for (int i = 0; i < coins; i++)
                {
                    if (i * 4 + 5 + 4 < Data.Count)
                    {
                        n++;
                        ReadySaleCoin.Add(BitConverter.ToUInt32(Data.ToArray(), i * 4 + 5));
                    }
                    else
                        OutCoinSuccessMessage();
                }

                Console.WriteLine("接收 " + n + " 个数字币");
                //ShowRecvCoin(n);

                if (finish == 1)
                    OutCoinSuccessMessage();
            }
            else
            {
                Console.WriteLine("重复数据检测失败");
            }
        }

        private void Process04()
        {
            SendInit();
        }

        #endregion

        #region--指令收发--
        /// <summary>
        /// 指令应答
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="data"></param>
        void AskData(byte cmdType, byte[] data)
        {
            List<byte> sendData = new List<byte>();
            List<byte> dataList = new List<byte>();
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            dataList.Add(0x68);
            dataList.Add(cmdType);
            dataList.Add((byte)data.Length);
            dataList.AddRange(data);
            ushort crcValue = CRC.Crc16(dataList.ToArray(), (byte)dataList.Count);
            dataList.AddRange(BitConverter.GetBytes(crcValue));
            sendData.AddRange(dataList);
            sendData.Add(0x16);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);
            sendData.Add(0xfe);

            com.Write(sendData.ToArray(), 0, sendData.Count);
        }
        /// <summary>
        /// 加帧头帧尾
        /// </summary>
        private List<byte> AddHeadEnd(List<byte> list)
        {
            List<byte> sendData = new List<byte>();
            sendData.Add(0xFE);
            sendData.Add(0xFE);
            sendData.Add(0xFE);
            sendData.Add(0xFE);
            //sendData.Add(0x68);
            for (int i = 0; i < list.Count; i++)
            {
                sendData.Add(list[i]);
            }
            sendData.Add(0x16);
            sendData.Add(0xFE);
            sendData.Add(0xFE);
            sendData.Add(0xFE);
            sendData.Add(0xFE);
            return sendData;
        }
        /// <summary>
        /// 发送指令:售币机初始化
        /// </summary>
        public void SendInit()
        {
            recvBuf = new List<byte>();

            List<byte> sendData = new List<byte>();
            sendData.Add(0x68);
            sendData.Add(0x01);
            sendData.Add(0x06);
            sendData.Add((byte)(SerialAPI.CanWork1 == "1" ? 1 : 0));
            sendData.Add((byte)(SerialAPI.CanWork2 == "1" ? 1 : 0));
            sendData.Add((byte)Motor1Coin);
            sendData.Add((byte)Motor2Coin);
            sendData.Add((byte)WorkType);//1四位2六位
            sendData.Add((byte)(DigitEn ? 1 : 0));  //数字币二合一售币机
            ushort crcValue = CRC.Crc16(sendData.ToArray(), (byte)sendData.Count);
            sendData.AddRange(BitConverter.GetBytes(crcValue));
            sendData = AddHeadEnd(sendData);

            com.Write(sendData.ToArray(), 0, sendData.Count);
        }
        /// <summary>
        /// 接收指令:售币机初始化状态
        /// </summary>
        private void ProcessF1()
        {
            SaleCoinerState = true;
            CoinerStateMessage();
        }
        /// <summary>
        /// 是否开始出币,售币机返回F2时为true
        /// </summary>
        public static bool StartSaleCoin = false;
        public static bool SaleCoinError = false;

        private void ProcessF2(List<byte> Data)
        {
            //F2010133
            if (Data[3] == 1)
            {
                SaleCoinError = false;
                StartSaleCoin = true;
                SaleCoinerRunning = true;
            }
            else
            {
                SaleCoinError = true;
                //OutCoinErrorMessage();
            }
        }

        /// <summary>
        /// 发送指令:售币机售币
        /// </summary>
        public void SendOutCoin(object iMustCoin)
        {
            StartSaleCoin = false;
            for (int i = 0; i < 3; i++)
            {
                if (!StartSaleCoin)
                {
                    //Application.DoEvents();
                    //Thread t = new Thread(new ParameterizedThreadStart(SaleCoin));
                    //t.Start(iMustCoin);
                    SaleCoin(iMustCoin);
                }
                else
                {
                    break;
                }
                Thread.Sleep(2000);
            }
            if (SaleCoinError)
            {
                WXDControl.WXDMessageBox.Show("售币机出错，请点重连按钮进行复位");
                return;
            }
            if (!StartSaleCoin)
            {
                WXDControl.WXDMessageBox.Show("售币机无响应,请检查串口线和电源");
            }
        }

        public void SaleDigitCoin(object iMustCoin)
        {
            sn = 0;
            SaleCoinerRunning = true;

            List<byte> sendData = new List<byte>();
            sendData.Add(0x68);
            sendData.Add(0x02);
            sendData.Add(0x03);
            sendData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(iMustCoin)));
            sendData.Add(0x01); //出数字币

            ushort crcValue = CRC.Crc16(sendData.ToArray(), (byte)sendData.Count);
            sendData.AddRange(BitConverter.GetBytes(crcValue));
            sendData = AddHeadEnd(sendData);

            com.Write(sendData.ToArray(), 0, sendData.Count);
        }

        public void SaleCoin(object iMustCoin)
        {
            sn = 0;

            List<byte> sendData = new List<byte>();
            sendData.Add(0x68);
            sendData.Add(0x02);
            sendData.Add(0x03);
            sendData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(iMustCoin)));
            sendData.Add(0x00); //出实物币

            ushort crcValue = CRC.Crc16(sendData.ToArray(), (byte)sendData.Count);
            sendData.AddRange(BitConverter.GetBytes(crcValue));
            sendData = AddHeadEnd(sendData);

            com.Write(sendData.ToArray(), 0, sendData.Count);
        }

        /// <summary>
        /// 接收指令:售币机出币完成
        /// </summary>
        private void Process03(List<byte> Data)
        {
            try
            {
                Console.WriteLine("售币结束事件");
                SaleCoinerRunning = false;
                string statusbit = Convert.ToString(Data[3], 2);    //转换2进制
                int b = 8 - statusbit.Length;
                for (int i = 0; i < b; i++)
                {
                    statusbit = "0" + statusbit;
                }
                curCoinStatus.bit0马达1缺币或卡币 = (statusbit.Substring(7, 1) == "1");
                curCoinStatus.bit1马达2缺币或卡币 = (statusbit.Substring(6, 1) == "1");
                curCoinStatus.bit2马达1常开光眼故障 = (statusbit.Substring(5, 1) == "1");
                curCoinStatus.bit3马达1常闭光眼故障 = (statusbit.Substring(4, 1) == "1");
                curCoinStatus.bit4马达2常开光眼故障 = (statusbit.Substring(3, 1) == "1");
                curCoinStatus.bit5马达2常闭光眼故障 = (statusbit.Substring(2, 1) == "1");
                curCoinStatus.bit6马达1币量不足 = (statusbit.Substring(1, 1) == "1");
                curCoinStatus.bit7马达2币量不足 = (statusbit.Substring(0, 1) == "1");

                OutCoin = BitConverter.ToUInt16(Data.ToArray(), 4).ToString();
                OutCoinSuccessMessage();
            }
            catch (Exception)
            { }
        }

        #endregion

    }
}
