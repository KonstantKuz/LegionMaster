using System.Runtime.CompilerServices;
using LegionMaster.UI.Components;
using UnityEngine;

namespace LegionMaster.UI.Overlay
{
    public class LockOverlay : MonoBehaviour
    {
        private const string DEFAULT_LOCK_LOCALIZATION_ID = "Waiting";

        [SerializeField]
        private TextMeshProLocalization _message;

        private int _lockCount;

        public void Lock(string localizationId = DEFAULT_LOCK_LOCALIZATION_ID, [CallerMemberName] string memberName = "")
        {
            Debug.Log($"Lock screen from {memberName}, lockCount {_lockCount}");
            _message.LocalizationId = localizationId;
            LockCount++;
        }

        public void Unlock([CallerMemberName] string memberName = "")
        {
            Debug.Log($"Unlock screen from {memberName}, lockCount {_lockCount}");
            if (_lockCount == 0) {
                Debug.LogWarning($"Screen not locked, but try unlock from {memberName}");
                return;
            }
            LockCount--;
        }

        private int LockCount
        {
            get => _lockCount;
            set
            {
                _lockCount = value;
                gameObject.SetActive(_lockCount >= 1);
            }
        }
    }
}