using DG.Tweening;
using GGW.game._00.Codes.Traps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKY.Script.Agent
{
    public class TestPlayer : AbstractAgent, ISlowable
    {
        private Camera _cam;

        protected override void Awake()
        {
            base.Awake();
            _cam = Camera.main;
        }

        public void Slowed(float multiplier, float duration)
        {
            float speed = MoveModule.MoveSpeed * (1 - multiplier);
            if (MoveModule.CurrentSpeed < speed)
                return;
            
            Sequence seq = DOTween.Sequence();
            MoveModule.SetSpeed(speed);
            seq.AppendInterval(duration);
            seq.AppendCallback(MoveModule.InitSpeed);
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                MoveTo();
            }
        }

        private void MoveTo()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPos = _cam.ScreenToWorldPoint(mousePosition);
            
            MoveModule.SetDestination(worldPos);
        }
    }
}