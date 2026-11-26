using System;
using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
using UnityEngine;

namespace NKY.Script
{
    public class TestCatch : MonoBehaviour
    {
        [SerializeField] private EventChannelSO catchEventChannel;

        private void Awake()
        {
            catchEventChannel.AddListener<CatchEvt>(Test);
        }

        private void Test(CatchEvt evt)
        {
            Debug.Log(evt.CatchTarget.name);
            Debug.Log("잡았다");
        }
    }
}