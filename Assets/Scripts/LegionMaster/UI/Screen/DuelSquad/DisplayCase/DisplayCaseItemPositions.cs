using LegionMaster.Extension;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.DisplayCase
{
    public class DisplayCaseItemPositions : MonoBehaviour
    {
        [SerializeField] private Transform _tokenAmountPosition;
        [SerializeField] private Transform _shopUpdatePricePosition; 
        [SerializeField] private Transform _unitFactionPositions;  
        [SerializeField] private Transform _unitPricePositions;
        [SerializeField] private Transform _unitUpgradeStatusPositions;
        
        public Transform TokenAmountPosition => _tokenAmountPosition;
        public Transform ShopUpdatePricePosition => _shopUpdatePricePosition;
        public Transform[] UnitFactionPositions => _unitFactionPositions.gameObject.GetComponentsOnlyInChildren<Transform>();      
        public Transform[] UnitPricePositions => _unitPricePositions.gameObject.GetComponentsOnlyInChildren<Transform>();
        public Transform[] UnitUpgradeStatusPositions => _unitUpgradeStatusPositions.gameObject.GetComponentsOnlyInChildren<Transform>();
    }
}