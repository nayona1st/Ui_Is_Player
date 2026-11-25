using System;
using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
using UnityEngine;

namespace GGW.game._00.Codes.Traps
{
    public class AlarmEvent : GameEvent
    {
        public Vector2 TargetPos {get; private set;}
        public void FindTarget(Transform target)
        {
            TargetPos = target.position;
        }
    }
    
    public class Alarm : Trap
    {
        [SerializeField] private EventChannelSO alarmEvent;

        private AlarmEvent _evtAlarm;
        
        private void Awake()
        {
            _evtAlarm = new AlarmEvent();
        }

        public override void OnTrapped(GameObject target)
        {
            _evtAlarm.FindTarget(target.transform);
            alarmEvent.RaiseEvent(_evtAlarm);
        }
    }
}
