using UnityEngine;

namespace GGW.game._00.Codes.Traps
{
    public class Alarm : Trap
    {
        public override void OnTrapped()
        {
            Debug.Log("소리찎찎");
        }
    }
}
