using System;
using System.Collections;
using UnityEngine;

namespace GGW.game._00.Codes.Traps
{
   public class SlowTrap : Trap
   {
      [SerializeField] private float speedMultiplier;
      [SerializeField] private float duration;
      
      public override void OnTrapped(GameObject target)
      {
         if(target.TryGetComponent(out ISlowable slowable))
            slowable.Slowed(speedMultiplier, duration);
      }

      
   }
}
