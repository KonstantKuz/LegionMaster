using System.Collections;
using System.Collections.Generic;
using LegionMaster.DroppingLoot.Message;
using SuperMaxim.Messaging;
using UnityEngine;
using Zenject;

namespace LegionMaster.DroppingLoot.Component
{
    public class DroppingLootMessageStorage : MonoBehaviour
    {
        private readonly List<UiLootReceivedMessage> _storedMessages = new List<UiLootReceivedMessage>();
        
        [Inject] private IMessenger _messenger;

        private void OnEnable()
        {
            StartCoroutine(PublishSavedLootAfterUpdate());
        }
        public void OnDisable()
        {
            _storedMessages.Clear();
            _messenger.Subscribe<UiLootReceivedMessage>(OnDroppingLootReceived);
        }
        private IEnumerator PublishSavedLootAfterUpdate()
        {
            _messenger.Unsubscribe<UiLootReceivedMessage>(OnDroppingLootReceived);
            yield return null;
            _storedMessages.ForEach(it => _messenger.Publish(it));
            _storedMessages.Clear();
        }
        private void OnDroppingLootReceived(UiLootReceivedMessage msg)
        {
            _storedMessages.Add(msg);
        }
    }
}