using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
using GGW.game._00.Codes.Traps;
using NKY.Script.BT;
using Unity.Behavior;
using UnityEngine;

namespace NKY.Script.Agent
{
    public class TestPolice : AbstractAgent, IListenAlarmable
    {
        [field:SerializeField] public EventChannelSO AlarmEvent { get; private set; }
        
        private BehaviorGraphAgent _behaviorAgent;
        
        private BlackboardVariable<ChangeStateEvent> changeEvent;

        protected override void Awake()
        {
            base.Awake();
            _behaviorAgent = GetComponent<BehaviorGraphAgent>();
            
            AlarmEvent.AddListener<AlarmEvent>(Listen);
            
            _behaviorAgent.BlackboardReference.GetVariable(
                "ChangeEvent",
                out changeEvent
            );
        }

        public void Listen(AlarmEvent evt)
        {
            _behaviorAgent.SetVariableValue("State", PoliceState.SEARCH);
            _behaviorAgent.SetVariableValue("SearchPosition", evt.TargetPos);
            changeEvent.Value.SendEventMessage(PoliceState.SEARCH);
        }
    }
}