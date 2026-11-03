using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField]private float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir.normalized * speed;
    }

    private void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }
}
