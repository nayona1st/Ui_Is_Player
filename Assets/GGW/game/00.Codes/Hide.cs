using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGW.game._00.Codes
{
    public class Hide : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField]private float _interactRange = 10f;

        private bool isHidden;

        private void Update()
        {
            CheckingRange();
        }

        private void CheckingRange()
        {
            var hit2D = Physics2D.OverlapCircle(transform.position, _interactRange);
            if(hit2D != null &&  hit2D.gameObject.CompareTag("Player"))
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    HideFunc();
                }
            }
        }

        private void HideFunc()
        {
            if (!_player.TryGetComponent(out SpriteRenderer sr)) return;
            if(!_player.TryGetComponent(out Movement mover)) return;
            
            isHidden = !isHidden;

            if (isHidden)
            {
                sr.color = new  Color(0f, 255f, 0f, 0f);
                mover.speed = 0f; // 플레이어를 멈추게만 하면 상관 ㄴ
            }
            else
            {
                sr.color = new  Color(0f, 255f, 0f, 1f);
                mover.speed = 5f; // 이하동문
            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _interactRange);
        }
    }
}