using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Common.Enum;

namespace XCCloudService.WorkFlow.Base
{
    public abstract partial class BaseWorkFlow<TState, TTrigger>
    {
        protected StateMachine<TState, TTrigger> _machine;
        protected string _token = Guid.NewGuid().ToString("N");
        protected DateTime _lastUpdate;
        protected string _description;

        public TState State
        {
            get
            {
                return _machine.State;
            }            
        }

        public string Token
        {
            get 
            {
                return _token;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
            }
        }

        public bool CanFire(TTrigger trigger)
        {
            return _machine.CanFire(trigger);
        }

        public void Print()
        {
            LogHelper.SaveLog(TxtLogType.WorkFlow, string.Format("[Status:] {0}", _machine));
        }

        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }        
    }
}
