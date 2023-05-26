using UnityEngine;

namespace LegionMaster.Core
{
    public class MoveToSceneRoot : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetParent(null);
        }
    }
}