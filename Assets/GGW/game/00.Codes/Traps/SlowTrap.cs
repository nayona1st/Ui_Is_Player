using System;
using System.Collections;
using UnityEngine;

namespace GGW.game._00.Codes.Traps
{
   public class SlowTrap : Trap
   {

      public Action Trapped;
      
      public override void OnTrapped()
      {
         Trapped?.Invoke();
      }

      
   }
}
