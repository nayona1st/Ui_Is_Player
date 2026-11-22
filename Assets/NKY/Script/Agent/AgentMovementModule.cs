using _LumenLib.ModuleSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKY.Script.Agent
{
    public class AgentMovementModule : MonoModule
    {
        public PathAgent PathAgent { get; private set; }

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            PathAgent = owner.GetComponent<PathAgent>();
            if(PathAgent == null)
                Debug.LogWarning("AgentMovementModule: PathAgent is null");
        }

        public bool SetDestination(Vector2 destination)
        {
            return PathAgent.SetDestination(destination);
        }
        
        public void StopMovement()
        {
            PathAgent.StopMovement();
        }
    }
}