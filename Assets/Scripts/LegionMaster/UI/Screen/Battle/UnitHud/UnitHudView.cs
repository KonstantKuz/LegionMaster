using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.UnitHud
{
    public class UnitHudView : MonoBehaviour
    {
        private UnitHudModel _model;
        
        public void Init(UnitHudModel model)
        {
            _model = model;
            _model.DeathCallback += DisableBar;  
            Update();
        }

        public void Term()
        {
            if (_model == null) return;
            _model.DeathCallback -= DisableBar;
            _model = null;
        }
        
        private void DisableBar()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_model == null) return;
            transform.position = Camera.main.WorldToScreenPoint(_model.Position.position);
        }

        private void OnDisable()
        {
            Term();
        }
    }
}