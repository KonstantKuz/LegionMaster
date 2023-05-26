﻿using LegionMaster.DroppingLoot.Config;
using LegionMaster.DroppingLoot.Message;
using LegionMaster.DroppingLoot.Model;
using LegionMaster.UI;
using LegionMaster.UI.Loader;
using SuperMaxim.Messaging;
using UnityEngine;
using Zenject;

namespace LegionMaster.DroppingLoot.Component
{
    public class DroppingLootReceiver : MonoBehaviour
    {
        [SerializeField]
        private DroppingLootType _lootType;    
        [SerializeField]
        private RectTransform _receivingContainer;    
        [SerializeField]
        private DroppingLootConfig _config;  
      
        [Inject] private IMessenger _messenger;    
        [Inject] private UILoader _uiLoader;         
        [Inject] private UIRoot _uiRoot;      
        
        private DroppingLootVfx _vfx;
        
        private void OnEnable()
        {
            _messenger.Subscribe<UiLootReceivedMessage>(OnDroppingLootReceived);
        }
        private void OnDroppingLootReceived(UiLootReceivedMessage msg)
        {
            if (msg.Type != _lootType) {
                return;
            }
            PlayVfx(msg);
        }

        private void PlayVfx(UiLootReceivedMessage msg)
        {
            DroppingLootVfx.Play(DroppingLootModel.FromReceivedMessage(msg, _receivingContainer.position));
        }
        public void OnDisable()
        {
            _messenger.Unsubscribe<UiLootReceivedMessage>(OnDroppingLootReceived);
        }

        private DroppingLootVfx DroppingLootVfx => _vfx ??= gameObject.AddComponent<DroppingLootVfx>().Init(_uiLoader, _config, _uiRoot.DroppingLootContainer);
    }
}