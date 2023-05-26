using JetBrains.Annotations;
using LegionMaster.ABTest.Loadable;
using LegionMaster.Core.IoC;
using LegionMaster.Core.Loadable;
using LegionMaster.UI.Screen;
using UnityEditor;
using UnityEngine;
using Zenject;
#if UNITY_IOS
using LegionMaster.IOSTransparency.Loadable;      
#endif

namespace LegionMaster
{
    public class GameApplication : MonoBehaviour
    {
        [SerializeField] private BaseScreen _firstScreen;
        [PublicAPI]
        public static GameApplication Instance { get; private set; }

        [Inject]
        public DiContainer Container;

        private void Awake()
        {
            Instance = this;
            AppContext.Container = Container;

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += HandleOnPlayModeChanged;
            void HandleOnPlayModeChanged(PauseState pauseState)
            {
                
            }
#endif
            DontDestroyOnLoad(gameObject);
            RunLoadableChains();
        }
        private void RunLoadableChains()
        {
            var loadableChain = gameObject.AddComponent<AppLoadableChain>();
#if UNITY_IOS
            loadableChain.AddLoadable<IosATTLoadable>();
#endif
            loadableChain.AddLoadable<ABTestLoadable>();
            loadableChain.AddLoadable<StartGameLoadable>(new object[] {_firstScreen.Url});
            loadableChain.Next();
        }
    }
}