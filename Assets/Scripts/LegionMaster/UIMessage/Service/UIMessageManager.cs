using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.UIMessage.Model;
using UnityEngine;

namespace LegionMaster.UIMessage.Service
{
    public class UIMessageManager : MonoBehaviour
    {
        private const int DEFAULT_EXPIRED_TIME = 20;

        private readonly List<UIMessage> _messagesList = new List<UIMessage>();

        private UIMessage _currentMessage;
        private IEnumerator _timeoutCoroutine;

        private void Update()
        {
            RemoveExpiredMessages();
            if (_currentMessage != null || _messagesList.Count == 0) {
                return;
            }
            ShowNext();
        }

        private void RemoveExpiredMessages()
        {
            _messagesList.RemoveAll(ntf => {
                if (ntf.Equals(_currentMessage)) {
                    return false;
                }
                if (!ntf.Expired) {
                    return false;
                }
                Destroy(ntf.gameObject);
                return true;
            });
        }

        private void Hide()
        {
            if (_currentMessage == null) {
                return;
            }
            _currentMessage.Hide();
            Remove(_currentMessage);
            _currentMessage = null;
        }

        public void Add(GameObject messageObject, int timeout, MessageType messageType, bool canClose = true, int livingTime = DEFAULT_EXPIRED_TIME)
        {
            var message = messageObject.AddComponent<UIMessage>();
            message.transform.SetParent(gameObject.transform, false);
            var livingDateTime = DateTime.Now.AddSeconds(livingTime);
            if (canClose) {
                message.Init(timeout, livingDateTime, messageType, Hide);
            } else {
                message.Init(timeout, livingDateTime, messageType);
            }
            _messagesList.Add(message);
        }

        private void ShowNext()
        {
            _currentMessage = FindNext();
            Show(_currentMessage);
        }

        private UIMessage FindNext() => _messagesList.FirstOrDefault();

        private void Show(UIMessage message)
        {
            message.Show();
            if (message.Timeout <= 0) {
                return;
            }
            _timeoutCoroutine = CloseAfterTimeout(message.Timeout);
            StartCoroutine(_timeoutCoroutine);
        }

        private IEnumerator CloseAfterTimeout(float timeout)
        {
            yield return new WaitForSecondsRealtime(timeout);
            Hide();
        }

        private void Remove(UIMessage message)
        {
            StopTimeoutCoroutine();
            _messagesList.Remove(message);
        }

        private void StopTimeoutCoroutine()
        {
            if (_timeoutCoroutine != null) {
                StopCoroutine(_timeoutCoroutine);
            }
        }
    }
}