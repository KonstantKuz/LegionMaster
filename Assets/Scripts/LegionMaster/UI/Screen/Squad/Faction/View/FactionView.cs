using System.Collections.Generic;
using LegionMaster.Core.Config;
using LegionMaster.Extension;
using LegionMaster.UI.Screen.Squad.Faction.Model;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.Faction.View
{
    public class FactionView : MonoBehaviour
    {
        [SerializeField] private FactionItemView _factionItemPrefab;
        [SerializeField] private Transform _factionsRoot;
        
        private CompositeDisposable _disposable;
        
        public void Init(IReadOnlyReactiveProperty<IReadOnlyCollection<FactionItemModel>> fractions)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            fractions.Subscribe(CreateFactionItems).AddTo(_disposable);
            gameObject.SetActive(AppConfig.FactionEnabled);
        }

        private void CreateFactionItems(IReadOnlyCollection<FactionItemModel> factionItemModels)
        {
            RemoveFractions();
            factionItemModels.ForEach(it => {
                var factionItemView = Instantiate(_factionItemPrefab, _factionsRoot, false);
                factionItemView.Init(it);
            });
        }

        private void OnDisable()
        {
            RemoveFractions();
            _disposable?.Dispose();
            _disposable = null;
        }

        private void RemoveFractions()
        {
            _factionsRoot.DestroyAllChildren();
        }
    }
}