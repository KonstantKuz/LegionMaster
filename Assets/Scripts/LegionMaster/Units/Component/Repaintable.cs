using System;
using System.Linq;
using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component
{
    public class Repaintable : MonoBehaviour
    {
        [Serializable]
        public struct PaintConfig
        {
            public UnitType Type;
            public Material Material;
        }
        
        [SerializeField]
        private RepaintPart[] _repaintParts;
        [SerializeField]
        private PaintConfig[] _configs;

        public void Repaint(UnitType type)
        {
            var config = _configs.FirstOrDefault(it => it.Type == type);
            if (config.Material == null)
            {
                Debug.LogError($"No repaint config for unit {gameObject.name} for unit type {type}");
                return;
            }
            foreach (var repaintPart in _repaintParts)
            {
                var partRenderer = repaintPart._renderer;
                var materials = partRenderer.materials;
                materials[repaintPart._materialIndex] = config.Material;
                partRenderer.materials = materials;
            }
        }

        [Serializable]
        private class RepaintPart
        {
            public Renderer _renderer;
            public int _materialIndex;
        }
    }
}