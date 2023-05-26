using LegionMaster.UI.Screen;
using UnityEngine;

namespace LegionMaster.UI
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField]
        private SafeAreaContainer _safeAreaContainer;
        [SerializeField]
        private GameObject _droppingLootContainer;

        public SafeAreaContainer SafeAreaContainer => _safeAreaContainer;

        public GameObject DroppingLootContainer => _droppingLootContainer;
    }
}