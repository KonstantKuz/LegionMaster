using LegionMaster.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Squad
{
    [RequireComponent(typeof(RawImage))]
    public class UnitUiOverlay : MonoBehaviour
    {
        private const string OVERLAY_LAYER_NAME = "Overlay";
        private const string DEFAULT_LAYER_NAME = "Default";

        private const int TEXTURE_DEPTH_32_BITS = 32;
        
        private RenderTexture _renderTexture;
        private Camera _camera;

        private void Awake()
        {
            _renderTexture = new RenderTexture(UnityEngine.Screen.width, UnityEngine.Screen.height, TEXTURE_DEPTH_32_BITS, RenderTextureFormat.ARGB32);
            _renderTexture.Create();

            var rawImage = GetComponent<RawImage>();
            rawImage.texture = _renderTexture;
            rawImage.color = Color.white;
        }

        private void OnEnable()
        {
            CreateOverlayCamera();
        }

        private void CreateOverlayCamera()
        {
            _camera = Instantiate(Camera.main, Camera.main.transform, false);
            DisableAudioListener(_camera);

            var cameraTransform = _camera.transform;
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
            cameraTransform.localScale = Vector3.one;
            _camera.cullingMask = LayerMask.GetMask(new[] { OVERLAY_LAYER_NAME });
            _camera.targetTexture = _renderTexture;
            _camera.name = "OverlayCamera";
            _camera.tag = "Untagged";
            _camera.backgroundColor = new Color(0, 0, 0, 0);
            _camera.clearFlags = CameraClearFlags.SolidColor;
        }

        private static void DisableAudioListener(Camera camera)
        {
            var listener = camera.GetComponent<AudioListener>();
            if (listener != null)
            {
                listener.enabled = false;
            }
        }

        private void OnDisable()
        {
            if (_camera != null)
            {
                Destroy(_camera.gameObject);
                _camera = null;
            }
        }

        public void AddUnit(GameObject unit)
        {
            unit.SetLayerRecursively(LayerMask.NameToLayer(OVERLAY_LAYER_NAME));
        }

        public void RemoveUnit(GameObject unit)
        {
            unit.SetLayerRecursively(LayerMask.NameToLayer(DEFAULT_LAYER_NAME));
        }
    }
}