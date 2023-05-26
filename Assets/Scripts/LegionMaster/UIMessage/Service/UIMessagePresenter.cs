using LegionMaster.UI.Components;
using LegionMaster.UI.Loader;
using LegionMaster.UIMessage.Model;
using UnityEngine;
using Zenject;

namespace LegionMaster.UIMessage.Service
{
    public class UIMessagePresenter : MonoBehaviour
    {
        [Inject]
        private UIMessageManager _uiMessageManager;
        [Inject]
        private UILoader _uiLoader;

        public void ShowCommon<TPanel>(CommonMessageModel model)
                where TPanel : MonoBehaviour, IUiInitializable<CommonMessageModel>
        {
            var panel = _uiLoader.Load(UIModel<TPanel, CommonMessageModel>.Create(model)
                                                                          .Container(gameObject.transform).Path(model.Prefab));
            _uiMessageManager.Add(panel.gameObject, model.Timeout, model.MessageType);
        }
    }
}