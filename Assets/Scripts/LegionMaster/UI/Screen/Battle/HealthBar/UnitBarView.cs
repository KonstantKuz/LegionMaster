using LegionMaster.Extension;
using LegionMaster.UI.Components;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.HealthBar
{
    public class UnitBarView : MonoBehaviour
    {
        [SerializeField] protected ProgressBarView _bar;
        
        private CompositeDisposable _disposable;
        
        public virtual void Init(UnitBarModel model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable(); 
            gameObject.SetActive(model.Enabled);
            if (!model.Enabled) {
                return;
            }
            model.Percent.Subscribe(SetValue).AddTo(_disposable);
        }

        protected virtual void SetValue(float value)
        {
            _bar.SetData(value);
        }
        
        protected virtual void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}