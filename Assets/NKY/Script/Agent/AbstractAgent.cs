using _LumenLib.ModuleSystem;
using UnityEngine;

namespace NKY.Script.Agent
{
    public abstract class AbstractAgent : ModuleOwner
    {
        protected MovementModule MoveModule {get; private set;}
        protected override void InitModule()
        {
            base.InitModule();
            MoveModule = GetModule<MovementModule>();
        }
    }
}