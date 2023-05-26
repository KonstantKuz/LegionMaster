using System;
using System.Collections.Generic;
using LegionMaster.Location.GridArena.Model;
using UnityEngine;

namespace LegionMaster.Location.GridArena.Config
{
    [Serializable]
    public class ArenaGridConfig : MonoBehaviour
    {
        public GridCell CellPrefab;
        public Vector2 Dimensions;
        public float CellStep;
        public Vector3 CellScale;
        
        public int UnitRowsCount = 3;
        public List<CellHighlightColor> CellHighlightColors = new List<CellHighlightColor>();
    }

    [Serializable]
    public struct CellHighlightColor
    {
        public CellHighlight Highlight;
        public Color Color;
    }
}