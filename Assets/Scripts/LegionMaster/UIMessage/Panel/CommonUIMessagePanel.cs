using LegionMaster.UI.Components;
using LegionMaster.UIMessage.Model;
using UnityEngine;

namespace LegionMaster.UIMessage.Panel
{
    public class CommonUIMessagePanel : MonoBehaviour, IUiInitializable<CommonMessageModel>
    {
        public const string PREFAB_PATH = "Content/UI/Message/CommonUIMessage";

        [SerializeField]
        private TextMeshProLocalization _message;
        
        public void Init(CommonMessageModel model)
        {
            Message = model.Message;
        }

        private string Message
        {
            set { _message.LocalizationId = value; }
        }

 
    }
}