using System;
using LegionMaster.Core.Config;
using LegionMaster.Tutorial.UI;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Squad.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Squad.View
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(TutorialUiElement))]
    public class UnitButton : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private Analytics.Analytics _analytics;
        
        [SerializeField] private Image _image;       
        [SerializeField] private GameObject _disableShadow;        
        [SerializeField] private GameObject _checkMark;
        [SerializeField] private ActivatableObject _notification;
        [SerializeField] private SpriteListView _factionIcons;

        private Action _onClick;
        private Button _button;
        private string _iconPath;
        private TutorialUiElement _tutorialUiElement;

        public bool IsUnitPlaced => _checkMark.activeSelf;
        private Button Button => _button ??= GetComponent<Button>();

        public void Init(UnitButtonModel model)
        {
            SetIcon(model.IconPath);
            SetInteractable(model.Interactable);
            SetEnabled(model.Enabled);
            _checkMark.SetActive(model.CheckMark);
            _onClick = model.OnClick;
            
            //TODO: Split to two different buttons
            if (_notification != null)
            {
                _notification.Init(model.ShowNotification);
            }

            if (_factionIcons != null && AppConfig.FactionEnabled)
            {
                _factionIcons.Init(model.FactionIcons);
            }

            TutorialUiElement.Id = GetTutorialId(model.UnitId);
        }

        public static string GetTutorialId(string unitId)
        {
            return $"UnitButton_{unitId}";
        }

        public void SetEnabled(bool isEnabled)
        { 
            _disableShadow.SetActive(!isEnabled);
        }

        private void SetInteractable(bool interactable)
        {
            Button.interactable = interactable;
        }

        private void SetIcon(string iconPath)
        {
            if (_iconPath == iconPath) return;
            _iconPath = iconPath;
            _image.sprite = Resources.Load<Sprite>(_iconPath);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Button.interactable) return;
            _onClick?.Invoke();
            
            if (_checkMark.activeSelf) _analytics.PlaceUnitByTap();
        }

        private TutorialUiElement TutorialUiElement => _tutorialUiElement ??= GetComponent<TutorialUiElement>();
    }
}