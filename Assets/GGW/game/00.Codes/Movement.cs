using System;
using System.Collections;
using System.Collections.Generic;
using GGW.game._00.Codes.Traps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGW.game._00.Codes
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private SlowTrap trapEvent;

        private Rigidbody2D _rb;
        private Vector2 _moveDir;
        public float speed = 5f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            trapEvent.Trapped += StartSlowTr;
        }

        private void OnDisable()
        {
            trapEvent.Trapped -= StartSlowTr;
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveDir.normalized * speed;
        }

        private void OnMove(InputValue value)
        {
            _moveDir = value.Get<Vector2>();
        }
        
        private IEnumerator SlowTr()
        {
            speed *= 0.75f;
            Debug.Log("둔화");
            yield return new WaitForSeconds(4f);
            speed = 5f;
            Debug.Log("둔화제거");
        }

        private void StartSlowTr()
        {
            StartCoroutine(SlowTr());
        }
    }
}
