using LegionMaster.BattlePass.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public class TakenRewardModel
    {
        public BattlePassRewardId Id { get; }
        public Vector2 DroppingLootPosition { get; }

        public TakenRewardModel(BattlePassRewardId id, Vector2 droppingLootPosition)
        {
            Id = id;
            DroppingLootPosition = droppingLootPosition;
        }
    }
}