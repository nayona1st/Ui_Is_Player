using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGW.game._00.Codes
{
    public class Vent : MonoBehaviour
    {
        [SerializeField] private Vent targetVent;
        [SerializeField] private float cooltime = 2f;

        public event Action<Vent, Vent, IUseVentable> OnTeleported;

        private bool _canTeleport = true;
        private IUseVentable _currentUser;

        public bool CanUse => _canTeleport && targetVent != null;

        private void Update()
        {
            if (!CanUse || _currentUser == null) return;

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                UseCurrentVent();
            }
        }

        private void UseCurrentVent()
        {
            IUseVentable user = _currentUser;
            _currentUser = null;
            
            user.UseVent(targetVent.transform);
            StartCooldown();

            if (targetVent.targetVent != null)
            {
                targetVent.StartCooldown();
            }

            OnTeleported?.Invoke(this, targetVent, user);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IUseVentable useVent))
            {
                _currentUser = useVent;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out IUseVentable useVent) && _currentUser == useVent)
            {
                _currentUser = null;
            }
        }

        public void StartCooldown()
        {
            _canTeleport = false;
            DOVirtual.DelayedCall(cooltime, () => _canTeleport = true)
                     .SetLink(gameObject);
        }
    }
}
