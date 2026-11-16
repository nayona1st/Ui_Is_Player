using UnityEngine;

namespace GGW.game._00.Codes
{
    public class Vent : MonoBehaviour
    {
        [SerializeField] private Transform[] vents;
        [SerializeField] private Vector2 ventRaySize;

        private bool onceChange = false;
        private float _boolTimer;

        private void Update()
        {
            _boolTimer += Time.deltaTime;
            foreach (Transform t in vents)
            {
                var hit = Physics2D.OverlapBox((Vector2)t.position,new Vector2(ventRaySize.x,ventRaySize.y),0);
                if (hit != null && t == vents[0] && !onceChange)
                {
                    onceChange = true;
                    hit.gameObject.transform.position = vents[1].position;
                }
                if (hit != null && t == vents[1] && !onceChange)
                {
                    onceChange = true;
                    hit.gameObject.transform.position = vents[0].position;
                }
            }

            if (_boolTimer >= 2f)
            {
                onceChange = false;
                _boolTimer = 0f;
            }
        
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (Transform t in vents)
            {
                Gizmos.DrawWireCube(t.position,ventRaySize);
            }
        }
    }
}
