using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.StarBar
{
    public class StarBarView : MonoBehaviour
    {
        [SerializeField]
        private Transform _starRoot;
        private CompositeDisposable _disposable;

        public void Init(StarBarModel model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            
            _starRoot.gameObject.SetActive(model.Enabled);
            model.Stars.Subscribe(SetStars).AddTo(_disposable);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void SetStars(int stars)
        {
            for (int i = 0; i < _starRoot.childCount; i++)
            {
                var star = _starRoot.GetChild(i);
                star.gameObject.SetActive(i < stars);
            }
        }
    }
}