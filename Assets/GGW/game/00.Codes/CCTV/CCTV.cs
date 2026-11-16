using UnityEngine;

namespace GGW.game._00.Codes.CCTV
{
    public class Cctv : MonoBehaviour
    {
        private CctvRange _range;

        private void Awake()
        {
            _range = GetComponentInChildren<CctvRange>();
        }

        private void OnEnable()
        {
            _range._findPlayer += PlayerTrmAlarm;
        }

        private void OnDisable()
        {
            _range._findPlayer -= PlayerTrmAlarm;
        }

        private void PlayerTrmAlarm(Transform target)
        {
            Debug.Log($"{target}에 플레이어 있음");
        }

    }
}