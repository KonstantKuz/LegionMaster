using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegionMaster.Util.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AdvancedAnimator : MonoBehaviour
    {
        private readonly Dictionary<string, bool> _boolVars = new Dictionary<string, bool>();
        private readonly List<string> _triggersVars = new List<string>();
        private readonly Dictionary<string, int> _intVars = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _floatVars = new Dictionary<string, float>();

        private readonly Dictionary<string, List<Action>> _stateStartHandlers = new Dictionary<string, List<Action>>();
        private readonly Dictionary<string, List<Action>> _stateCompleteHandlers = new Dictionary<string, List<Action>>();
        
        private Animator _animator;

        private int _prevStateId;
        private string _prevStateName;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

        }

        private void OnEnable()
        {
            _prevStateId = 0;
            _prevStateName = "";
            _animator.enabled = true;
            WriteVariables();
        }

        private void Update()
        {
            if (!IsActive) {
                return;
            }
            ProcessHandlers();
        }

        public void ResetVars()
        {
            _animator.Rebind();
            _boolVars.Clear();
            _triggersVars.Clear();
            _intVars.Clear();
            _floatVars.Clear();
        }

        public void SetTrigger(string parameterName)
        {
            _animator.enabled = true;
            if (gameObject.activeSelf) {
                if (IsAnimatorValid) {
                    _animator.SetTrigger(parameterName);
                }
            } else {
                _triggersVars.Add(parameterName);
            }
        }

        public void ResetTrigger(string parameterName)
        {
            _animator.enabled = true;
            _triggersVars.Remove(parameterName);
            if (IsAnimatorValid) {
                _animator.ResetTrigger(parameterName);
            }
        }

        public void SetBool(string parameterName, bool value)
        {
            _animator.enabled = true;
            _boolVars[parameterName] = value;
            if (IsAnimatorValid) {
                _animator.SetBool(parameterName, value);
            }
        }

        public void SetInteger(string parameterName, int value)
        {
            _animator.enabled = true;
            _intVars[parameterName] = value;
            if (IsAnimatorValid) {
                _animator.SetInteger(parameterName, value);
            }
        }

        public void SetFloat(string parameterName, float value)
        {
            _animator.enabled = true;
            _floatVars[parameterName] = value;
            if (IsAnimatorValid) {
                _animator.SetFloat(parameterName, value);
            }
        }

        public void RegisterStateStartHandler(string stateName, Action stateStartCallback)
        {
            if (!_stateStartHandlers.ContainsKey(stateName)) {
                _stateStartHandlers[stateName] = new List<Action>();
            }
            _stateStartHandlers[stateName].Add(stateStartCallback);
        }

        public void UnregisterStateStartHandler(string stateName, Action stateStartCallback)
        {
            if (!_stateStartHandlers.ContainsKey(stateName)) {
                return;
            }
            _stateStartHandlers[stateName].Remove(stateStartCallback);
        }

        public void RegisterStateCompleteHandler(string stateName, Action stateCompleteCallback)
        {
            if (!_stateCompleteHandlers.ContainsKey(stateName)) {
                _stateCompleteHandlers[stateName] = new List<Action>();
            }
            _stateCompleteHandlers[stateName].Add(stateCompleteCallback);
        }

        public void UnregisterStateCompleteHandler(string stateName, Action stateCompleteCallback)
        {
            if (!_stateCompleteHandlers.ContainsKey(stateName)) {
                return;
            }
            _stateCompleteHandlers[stateName].Remove(stateCompleteCallback);
        }

        private void WriteVariables()
        {
            foreach (KeyValuePair<string, bool> pair in _boolVars) {
                _animator.SetBool(pair.Key, pair.Value);
            }
            foreach (string trigger in _triggersVars) {
                _animator.SetTrigger(trigger);
            }
            _triggersVars.Clear();
            foreach (KeyValuePair<string, int> pair in _intVars) {
                _animator.SetInteger(pair.Key, pair.Value);
            }
            foreach (KeyValuePair<string, float> pair in _floatVars) {
                _animator.SetFloat(pair.Key, pair.Value);
            }
        }

        private void ProcessHandlers()
        {
            bool inTransition = _animator.IsInTransition(0);
            AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            int currentStateId = currentAnimatorStateInfo.fullPathHash;
            string currentStateName = currentStateId == _prevStateId ? _prevStateName : GetCurrentStateName(ref currentAnimatorStateInfo);

            if (ShouldInvokeStartHandler(currentStateName, _prevStateId, inTransition, ref currentAnimatorStateInfo)) {
                _stateStartHandlers[currentStateName].ToList().ForEach(h => h.Invoke());
            }

            if (ShouldInvokeCompleteHandler(_prevStateName, _prevStateId, inTransition, ref currentAnimatorStateInfo)) {
                _stateCompleteHandlers[_prevStateName].ToList().ForEach(h => h.Invoke());
            }

            if (inTransition) {
                return;
            }
            _prevStateId = currentStateId;
            _prevStateName = currentStateName;
        }

        private bool ShouldInvokeStartHandler(string currentStateName,
                                              int prevStateId,
                                              bool inTransition,
                                              ref AnimatorStateInfo currentAnimatorStateInfo)
        {
            if (string.IsNullOrEmpty(currentStateName) || !_stateStartHandlers.ContainsKey(currentStateName)) {
                return false;
            }
            return !inTransition && currentAnimatorStateInfo.fullPathHash != prevStateId;
        }

        private bool ShouldInvokeCompleteHandler(string prevStateName,
                                                 int prevStateId,
                                                 bool inTransition,
                                                 ref AnimatorStateInfo currentAnimatorStateInfo)
        {
            if (string.IsNullOrEmpty(prevStateName) || !_stateCompleteHandlers.ContainsKey(prevStateName)) {
                return false;
            }
            return currentAnimatorStateInfo.normalizedTime >= 1 || prevStateId != currentAnimatorStateInfo.fullPathHash;
        }

        private string GetCurrentStateName(ref AnimatorStateInfo currentAnimatorStateInfo)
        {
            foreach (string key in _stateStartHandlers.Keys) {
                if (currentAnimatorStateInfo.IsName(key)) {
                    return key;
                }
            }
            foreach (string key in _stateCompleteHandlers.Keys) {
                if (currentAnimatorStateInfo.IsName(key)) {
                    return key;
                }
            }
            return "";
        }

        public bool HasAnimationParam(string paramName)
        {
            return _animator.parameters.Any(p => p.name == paramName);
        }
        

        public bool IsActive
        {
            get { return IsAnimatorValid && gameObject.activeInHierarchy && _animator.enabled; }
        }

        private bool IsAnimatorValid
        {
            get { return _animator.runtimeAnimatorController != null; }
        }
    }
}