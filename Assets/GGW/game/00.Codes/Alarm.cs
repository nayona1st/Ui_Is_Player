using System;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("소리 찎찎");
    }
}
