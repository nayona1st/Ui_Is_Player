using System;
using UnityEngine;

namespace GGW.game._00.Codes.Traps
{
    public abstract class Trap : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {   
            OnTrapped(other.gameObject);
            Destroy(gameObject);
        }

        public abstract void OnTrapped(GameObject target);
    }
}