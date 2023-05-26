using LegionMaster.UI.Components;
using LegionMaster.Units.Component.Hud;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.StarBar
{
    public class StarBarPresenter : MonoBehaviour, IUiInitializable<IStarBarOwner>
    {
        [SerializeField] private StarBarView _view;

        private StarBarModel _model;
        
        public void Init(IStarBarOwner owner)
        {
            _model = new StarBarModel(owner);
            _view.Init(_model);
        }

        private void OnDisable()
        {
            _model = null;
        }
    }
}