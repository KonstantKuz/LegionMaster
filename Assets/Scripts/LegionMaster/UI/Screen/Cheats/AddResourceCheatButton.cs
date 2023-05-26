using System.Linq;
using LegionMaster.Cheats;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Units.Config;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Cheats
{
    [RequireComponent(typeof(Button))]
    public class AddResourceCheatButton : MonoBehaviour
    {
        [Inject] private CheatsManager _cheatsManager;
        [Inject] private UnitCollectionConfig _unitCollectionConfig;

        [SerializeField] private int _amount;
        [SerializeField] private Resource _resource;
        [SerializeField] private Dropdown _dropdown;
        
        private Button _button;
        private Text _buttonText;

        private void OnEnable()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
            
            _buttonText = GetComponentInChildren<Text>();
            _buttonText.text = $"+{_amount } {_resource}";

            if (_dropdown != null && _resource == Resource.Shards) InitDropdown();
        }

        private void OnDisable() => _button.onClick.RemoveListener(OnClick);

        private void OnClick()
        {
            switch (_resource)
            {
                case Resource.Soft:
                    _cheatsManager.AddCurrency(CurrencyExt.ValueOf(_resource.ToString()), _amount);
                    break;
                case Resource.Hard:
                    _cheatsManager.AddCurrency(CurrencyExt.ValueOf(_resource.ToString()), _amount);
                    break;
                case Resource.Exp:
                    _cheatsManager.AddExp(_amount);
                    break;
                case Resource.Shards:
                    _cheatsManager.AddFragments(_dropdown.options[_dropdown.value].text, _amount);
                    break;
            }
        }

        private void InitDropdown()
        {
            _dropdown.ClearOptions();
            var allUnits = _unitCollectionConfig.AllUnitIds.ToList();
            _dropdown.AddOptions(allUnits);
        }
    }
}
