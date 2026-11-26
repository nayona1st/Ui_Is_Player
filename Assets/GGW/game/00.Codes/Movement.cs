using System;
using System.Collections;
using System.Collections.Generic;
using GGW.game._00.Codes.Traps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGW.game._00.Codes
{
    public class Movement : MonoBehaviour, ISlowable
    {
        [SerializeField] private SlowTrap trapEvent;

        private Rigidbody2D _rb;
        private Vector2 _moveDir;
        public float speed = 5f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveDir.normalized * speed;
        }

        private void OnMove(InputValue value)
        {
            _moveDir = value.Get<Vector2>();
        }

        public void Slowed(float speedMultiplier, float duration)
        {
            Debug.Log("멈춰짐");
        }
    }
}
