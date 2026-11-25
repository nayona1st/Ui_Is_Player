using System;
using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
using GGW.game._00.Codes.Traps;
using UnityEngine;

namespace GGW.game._00.Codes.CCTV
{
    public class Cctv : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private EventChannelSO findEvent;
        private CctvRange _range;
        
        private AlarmEvent _evtAlarm;

        private void Awake()
        {
            _range = GetComponentInChildren<CctvRange>();
            _evtAlarm = new AlarmEvent();
            _range.InitRange(120, 5, targetLayer);
        }
        
        private void OnEnable()
        {
            _range.FindPlayer += PlayerTrmAlarm;
        }

        private void OnDisable()
        {
            _range.FindPlayer -= PlayerTrmAlarm;
        }

        private void PlayerTrmAlarm(Transform target)
        {
            _evtAlarm.FindTarget(target.transform);
            findEvent.RaiseEvent(_evtAlarm);
        }

    }
}