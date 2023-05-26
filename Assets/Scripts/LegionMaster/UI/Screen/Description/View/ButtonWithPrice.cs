using System;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.UI.Screen.LootBoxShop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Description.View
{
    public class ButtonWithPrice : MonoBehaviour
    {
        private const string FONT_COLORS_CONFIG_PATH = "Content/UI/FontColorsConfig";

        [FormerlySerializedAs("_currencyLabel")] [SerializeField]
        private TMP_Text _priceTMP;
        [SerializeField] 
        private Text _priceText;
        [SerializeField]
        private ActionButton _actionButton; 
        [SerializeField]
        private Image _currencyIcon;

        private FontColorsConfig _fontColors;

        private CompositeDisposable _disposable;
        public ActionButton Button => _actionButton;
        public void Init(PriceButtonModel model, Action action)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            
            _actionButton.Init(action);
            Enabled = model.Enabled;
            if (!model.Enabled) {
                return;
            }
            CurrencyLabel = model.PriceLabel;
            model.CanBuy.Subscribe(SetСanBuyState).AddTo(_disposable);
            
            SetCurrencyActive(model.ShowIcon);
            if (model.ShowIcon) {
                SetIcon(model.CurrencyIconPath);
            }
        }
        protected virtual void SetСanBuyState(bool canBuy)
        {
            _actionButton.Button.interactable = canBuy;
            CurrencyColor = canBuy ? Color.white : FontColors.NotEnoughCurrencyColor;
        }

        private void SetCurrencyActive(bool value)
        {
            _currencyIcon.gameObject.SetActive(value);
        }
        
        private void SetIcon(string iconPath)
        {
            _currencyIcon.sprite = Resources.Load<Sprite>(iconPath);
        }
        protected Color CurrencyColor
        {
            set
            {
                if (_priceTMP != null)
                {
                    _priceTMP.color = value;
                } else if (_priceText != null)
                {
                    _priceText.color = value;
                }
            }
        }
        private string CurrencyLabel
        {
            set
            {
                if (_priceTMP != null)
                {
                    _priceTMP.text = value;
                }
                else
                {
                    _priceText.text = value;
                }
            }
        }
        private bool Enabled
        {
            set => gameObject.SetActive(value);
        }

        private void OnDisable() => Dispose();

        private void OnDestroy() => Dispose();
        private void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        protected FontColorsConfig FontColors => _fontColors ??= Resources.Load<FontColorsConfig>(FONT_COLORS_CONFIG_PATH);
       
    }
}