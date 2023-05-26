using UnityEngine;

namespace LegionMaster.Vfx
{
    public class FaceTowardCameraXZ : MonoBehaviour
    {
        private void Update()
        {
            var dirToCamera = Camera.main.transform.position - transform.position;
            dirToCamera = new Vector3(dirToCamera.x, 0, dirToCamera.z).normalized;
            transform.rotation = Quaternion.LookRotation(dirToCamera);
        }
    }
}