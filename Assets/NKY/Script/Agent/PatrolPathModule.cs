using System.Collections.Generic;
using _LumenLib.ModuleSystem;
using UnityEngine;

namespace NKY.Script.Agent
{
    public class PatrolPathModule : MonoModule
    {
        [Tooltip("리스트의 순서가 순찰 도는 순서")]
        [SerializeField] private List<Transform> patrolPath;
        
        private int _nextPatrolPointIndex = 0;

        public Transform GetNextPatrolPoint()
        {
            Transform nextPatrolPoint = patrolPath[_nextPatrolPointIndex];
            _nextPatrolPointIndex =  (_nextPatrolPointIndex + 1) % patrolPath.Count;
            return nextPatrolPoint;
        }
    }
}