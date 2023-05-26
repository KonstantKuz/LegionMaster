using LegionMaster.Repository;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged.Message;
using SuperMaxim.Messaging;

namespace LegionMaster.UI.Screen.Squad.UnitPlaceChanged
{
    public class UnitPlaceChangedMediator
    {
        private Analytics.Analytics _analytics;   
        private UnitPlaceChangedRepository _repository;
        
        public UnitPlaceChangedMediator(IMessenger messenger, Analytics.Analytics analytics, UnitPlaceChangedRepository repository)
        {
            _analytics = analytics;
            _repository = repository;
            messenger.Subscribe<UnitPlaceChangedMessage>(OnUnitPlaceChanged);
        }
        
        private void OnUnitPlaceChanged(UnitPlaceChangedMessage msg)
        {
            var placeChanged = UnitPlaceChanged;
            if (placeChanged.Changed) {
                return;
            }
            _analytics.ReportUnitPlaceChanged();
            placeChanged.Changed = true;
            _repository.Set(placeChanged);
        }
        private UnitPlaceChanged UnitPlaceChanged => _repository.Get() ?? new UnitPlaceChanged();
    }
}