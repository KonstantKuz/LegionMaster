using System.Collections.Generic;
using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Extension;
using LegionMaster.Player.Progress.Model;

namespace LegionMaster.Migration.PlayerProgress
{
    public class PlayerProgressV3
    {
        public Dictionary<GameMode, BattleCountInfo> BattleCountInfos;
        public PlayerProgressV3()
        {
            BattleCountInfos = EnumExt.Values<GameMode>()
                                      .ToDictionary(mode => mode, mode => new BattleCountInfo());
        }
        
    }
}