using System;
using System.Collections;
using UnityEngine;

namespace GGW.game._00.Codes.Traps
{
   public class SlowTrap : Trap
   {
      [SerializeField, Range(0f, 1f)] private float speedMultiplier;
      [SerializeField, Min(0f)] private float duration;
      
      public override void OnTrapped(GameObject target)
      {
         if(target.TryGetComponent(out ISlowable slowable))
            slowable.Slowed(speedMultiplier, duration);
      }

      
   }
}
