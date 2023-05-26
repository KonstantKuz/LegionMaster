﻿using System;
namespace LegionMaster.DroppingLoot.Model
{
    public enum DroppingLootType
    {
        Soft,
        Hard,
        Exp,
        BattlePassExp,
    }
    
    public static class DroppingLootTypeExt
    {
        public static DroppingLootType ValueOf(string name)
        {
            return (DroppingLootType) Enum.Parse(typeof(DroppingLootType), name, true);
        }
    }
}