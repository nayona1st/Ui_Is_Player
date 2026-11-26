using System;
using GGW.game._00.Codes.Traps;
using UnityEngine;

public class StunWall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Stun(other.gameObject);
    }

    private void Stun(GameObject target)
    {
        if (target.TryGetComponent(out ISlowable slowable))
        {
            slowable.Slowed(0,1);
        }
    }
}
