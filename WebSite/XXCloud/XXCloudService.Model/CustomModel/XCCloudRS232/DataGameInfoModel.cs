using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloudRS232
{
    [DataContract]
    public class DataGameInfoModelList
    {
        //[DataMember(Name = "Lists", Order = 1)]
        //public List<DataGameInfoModel> Lists { set; get; }

        //[DataMember(Name = "PushSpeedList", Order = 2)]
        //public List<string> PushSpeedList { set; get; }

        //[DataMember(Name = "PushPulseList", Order = 3)]
        //public List<string> PushPulseList { set; get; }

        //[DataMember(Name = "PushStartIntervalList", Order = 4)]
        //public List<string> PushStartIntervalList { set; get; }

        //[DataMember(Name = "SecondSpeedlList", Order = 5)]
        //public List<string> SecondSpeedlList { set; get; }
        //[DataMember(Name = "SecondPulseList", Order = 6)]
        //public List<string> SecondPulseList { set; get; }

        //[DataMember(Name = "SecondStartIntervalList", Order = 7)]
        //public List<string> SecondStartIntervalList { set; get; }

        //[DataMember(Name = "OutSpeedlList", Order = 8)]
        //public List<string> OutSpeedList { set; get; }

        //[DataMember(Name = "OutPulseList", Order = 9)]
        //public List<string> OutPulseList { set; get; }
        //[DataMember(Name = "GroupTypeModelList", Order = 10)]
        //public List<GroupTypeModel> GroupTypeModelList { set; get; }
        
    }
    [DataContract]
    public class DataGameInfoModel
    {
        [DataMember(Name = "groupID", Order = 1)]
        public int GroupID { set; get; }
        [DataMember(Name = "groupName", Order = 2)]
        public string GroupName { set; get; }

        [DataMember(Name = "groupType", Order = 3)]
        public int GroupType { set; get; }

        [DataMember(Name = "pushReduceFromCard", Order = 4)]
        public int PushReduceFromCard { set; get; }

        [DataMember(Name = "lotteryMode", Order = 5)]
        public int LotteryMode { set; get; }

        [DataMember(Name = "readCat", Order = 6)]
        public int ReadCat { set; get; }
        [DataMember(Name = "chkCheckGift", Order = 7)]
        public int chkCheckGift { set; get; }
        [DataMember(Name = "returnCheck", Order = 8)]
        public int ReturnCheck { set; get; }
        [DataMember(Name = "outsideAlertCheck", Order = 9)]
        public int OutsideAlertCheck { set; get; }
        [DataMember(Name = "icticketOperation", Order = 10)]
        public int ICTicketOperation { set; get; }
        [DataMember(Name = "notGiveBack", Order = 11)]
        public int NotGiveBack { set; get; }
        [DataMember(Name = "allowElecPush", Order = 12)]
        public int AllowElecPush { set; get; }
        [DataMember(Name = "allowDecuplePush", Order = 13)]
        public int AllowDecuplePush { set; get; }
        [DataMember(Name = "guardConvertCard", Order = 14)]
        public int GuardConvertCard { set; get; }
        [DataMember(Name = "allowRealPush", Order = 15)]
        public int AllowRealPush { set; get; }
        [DataMember(Name = "banOccupy", Order = 16)]
        public int BanOccupy { set; get; }
        [DataMember(Name = "strongGuardConvertCard", Order = 17)]
        public int StrongGuardConvertCard { set; get; }
        [DataMember(Name = "allowElecOut", Order = 18)]
        public int AllowElecOut { set; get; }
        [DataMember(Name = "nowExit", Order = 19)]
        public int NowExit { set; get; }
        [DataMember(Name = "bOLock", Order = 20)]
        public int BOLock { set; get; }
        [DataMember(Name = "allowRealOut", Order = 21)]
        public int AllowRealOut { set; get; }
        [DataMember(Name = "bOKeep", Order = 22)]
        public int BOKeep { set; get; }
        [DataMember(Name = "pushAddToGame", Order = 23)]
        public int PushAddToGame { set; get; }
        [DataMember(Name = "pushSpeed", Order = 24)]
        public int PushSpeed { set; get; }
        [DataMember(Name = "pushPulse", Order = 25)]
        public int PushPulse { set; get; }
        [DataMember(Name = "pushStartInterval", Order = 26)]
        public int PushStartInterval { set; get; }

        [DataMember(Name = "useSecondPush", Order = 27)]
        public int UseSecondPush { set; get; }

        [DataMember(Name = "secondReduceFromCard", Order = 28)]
        public int SecondReduceFromCard { set; get; }

        [DataMember(Name = "secondAddToGame", Order = 29)]
        public int SecondAddToGame { set; get; }

        [DataMember(Name = "secondSpeed", Order = 30)]
        public int SecondSpeed { set; get; }

        [DataMember(Name = "secondPulse", Order = 31)]
        public int SecondPulse { set; get; }

        [DataMember(Name = "secondLevel", Order = 32)]
        public int SecondLevel { set; get; }

        [DataMember(Name = "secondStartInterval", Order = 33)]
        public int SecondStartInterval { set; get; }

        [DataMember(Name = "outSpeed", Order = 34)]
        public int OutSpeed { set; get; }

        [DataMember(Name = "outPulse", Order = 35)]
        public int OutPulse { set; get; }
        [DataMember(Name = "countLevel", Order = 36)]
        public int CountLevel { set; get; }

        [DataMember(Name = "outLevel", Order = 37)]
        public int OutLevel { set; get; }
        [DataMember(Name = "outReduceFromGame", Order = 38)]
        public int OutReduceFromGame { set; get; }

        [DataMember(Name = "outAddToCard", Order = 39)]
        public int OutAddToCard { set; get; }
        [DataMember(Name = "onceOutLimit", Order = 40)]
        public int OnceOutLimit { set; get; }

        [DataMember(Name = "oncePureOutLimit", Order = 41)]
        public int OncePureOutLimit { set; get; }
        [DataMember(Name = "ssrtimeOut", Order = 42)]
        public int SSRTimeOut { set; get; }

        [DataMember(Name = "exceptOutTest", Order = 43)]
        public int ExceptOutTest { set; get; }
        [DataMember(Name = "exceptOutSpeed", Order = 44)]
        public int ExceptOutSpeed { set; get; }

        [DataMember(Name = "frequency", Order = 45)]
        public int Frequency { set; get; }
        [DataMember(Name = "pushLevel", Order = 46)]
        public int PushLevel { set; get; }
        [DataMember(Name = "onlyExitLottery", Order = 47)]
        public int OnlyExitLottery { set; get; }
        
    }
    [DataContract]

    public class GroupTypeModel
    {
        [DataMember(Name = "Type", Order = 1)]
        public string Type { set; get; }

        [DataMember(Name = "Value", Order = 2)]
        public int Value { set; get; }
    }


}
