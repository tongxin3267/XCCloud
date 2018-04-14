using Stateless;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.SocketService.TCP.Business;
using XCCloudService.WorkFlow.Base;

namespace XCCloudService.WorkFlow
{
    public enum StockTrigger
    {
        /// <summary>
        /// 预警
        /// </summary>
        [Description("预警")]
        Warn,
        /// <summary>
        /// 申请
        /// </summary>
        [Description("申请")]
        Request,
        /// <summary>
        /// 中止
        /// </summary>
        [Description("中止")]
        Abort,
        /// <summary>
        /// 延期
        /// </summary>
        [Description("延期")]
        Defer,
        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Refuse,
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Store
    }

    public enum StockState
    {
        Normal,
        Warned,
        Requested,
        Aborted,
        Deferred,
        Refused
    }

    /// <summary>
    /// 入库工作流
    /// </summary>
    public class StockWorkFlow : BaseWorkFlow<StockState, StockTrigger>
    {

        StockState _state = StockState.Normal;

        StateMachine<StockState, StockTrigger>.TriggerWithParameters<string, string> _setWarnTrigger;
        StateMachine<StockState, StockTrigger>.TriggerWithParameters<string, int> _setRequestTrigger;
        StateMachine<StockState, StockTrigger>.TriggerWithParameters<string> _setRefuseTrigger;

        string _sender;
        string _requester;
        string _handler;
        int _count;
        const int _upper = 500;
        
        public StockWorkFlow()
        {
            _machine = new StateMachine<StockState, StockTrigger>(() => _state, s => _state = s);

            _setWarnTrigger = _machine.SetTriggerParameters<string, string>(StockTrigger.Warn);
            _setRequestTrigger = _machine.SetTriggerParameters<string, int>(StockTrigger.Request);
            _setRefuseTrigger = _machine.SetTriggerParameters<string>(StockTrigger.Refuse);

            _machine.Configure(StockState.Normal)
                .OnEntry(t => OnStored(), "入库成功")
                .Permit(StockTrigger.Warn, StockState.Warned);

            _machine.Configure(StockState.Warned)
                .OnEntryFrom(_setWarnTrigger, (sender, requester) => OnWarned(sender, requester), "向门店发出预警")
                .Permit(StockTrigger.Request, StockState.Requested);
            
            _machine.Configure(StockState.Requested)
                .OnEntryFrom(_setRequestTrigger, (handler, count) => OnRequested(handler, count), "向商户提交申请")
                //.PermitReentry(StockTrigger.Request)
                .Permit(StockTrigger.Abort, StockState.Aborted)
                .Permit(StockTrigger.Defer, StockState.Deferred)
                .Permit(StockTrigger.Refuse, StockState.Refused)
                .Permit(StockTrigger.Store, StockState.Normal);

            _machine.Configure(StockState.Aborted)
                .OnEntry(t => OnAborted(), "请求中止")
                .Permit(StockTrigger.Request, StockState.Requested);
            
            _machine.Configure(StockState.Deferred)
                .OnEntry(t => OnDeferred(), "延期处理")
                .Permit(StockTrigger.Refuse, StockState.Refused)
                .Permit(StockTrigger.Store, StockState.Normal);

            _machine.Configure(StockState.Refused)
                .OnEntryFrom(_setRefuseTrigger, description => OnRefused(description), "返回拒绝理由")
                .Permit(StockTrigger.Request, StockState.Requested);

        }

        #region 属性
        public new StockState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        #endregion

        #region 事件

        void OnWarned(string sender, string requester)
        {
            _sender = sender;
            _requester = requester;
            _description = _sender + "向门店发出预警, 接收人:" + _requester;
            WorkFlowServiceBusiness.Send(_sender, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        void OnRequested(string handler, int count)
        {
            _handler = handler;
            _count = (int)count;
            _description = "检查入库限额, 当前申请数:" + _count;
            WorkFlowServiceBusiness.Send(_requester, new { answerMsg=_description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });

            if (_count > _upper)
            {
                _machine.Fire(StockTrigger.Abort);
            }
            else
            {
                _description = _requester + "向商户提交申请, 处理人:" + _handler + ", 请求数:" + _count;
                WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
        }

        void OnAborted()
        {
            _description = "超过入库限额数:" + _upper;
            WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }
               
        void OnDeferred()
        {
            _description = "延期处理";
            WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            WorkFlowServiceBusiness.Send(_handler, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }
        

        void OnRefused(string description)
        {
            _description = description;
            WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            WorkFlowServiceBusiness.Send(_handler, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        void OnStored()
        {
            _description = "入库成功, 处理人:" + _handler + ", 处理数:" + _count;
            WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            WorkFlowServiceBusiness.Send(_handler, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        #endregion

        #region 方法

        public void Warn(string sender, string requester)
        {            
            _machine.Fire(_setWarnTrigger, sender, requester);
        }

        public void Request(string handler, int count)
        {            
            if (_machine.CanFire(StockTrigger.Request))
            {
                _machine.Fire(_setRequestTrigger, handler, (int)count);
            }
            else
            {
                _description = "不能重复申请";
                WorkFlowServiceBusiness.Send(_requester, new { answerMsg = _description, answerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
        }

        public void Defer()
        {
            _machine.Fire(StockTrigger.Defer);
        }

        public void Refuse()
        {
            _machine.Fire(StockTrigger.Refuse);
        }

        public void Store()
        {
            _machine.Fire(StockTrigger.Store);
        }

        #endregion

    }
}
