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
        [field:SerializeField] public LayerMask TargetLayer { get; private set; }
        
        public BehaviorGraphAgent BehaviorAgent {get; private set;}
        
        private BlackboardVariable<ChangeStateEvent> changeEvent;

        protected override void Awake()
        {
            base.Awake();
            BehaviorAgent = GetComponent<BehaviorGraphAgent>();
            
            AlarmEvent.AddListener<AlarmEvent>(Listen);
            
            BehaviorAgent.BlackboardReference.GetVariable(
                "ChangeEvent",
                out changeEvent
            );
        }

        public void Listen(AlarmEvent evt)
        {
            BehaviorAgent.SetVariableValue("State", PoliceState.SEARCH);
            BehaviorAgent.SetVariableValue("SearchPosition", evt.TargetPos);
            changeEvent.Value.SendEventMessage(PoliceState.SEARCH);
        }
    }
}