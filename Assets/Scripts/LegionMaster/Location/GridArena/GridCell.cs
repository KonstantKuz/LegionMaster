using System.Linq;
using LegionMaster.Location.GridArena.Config;
using LegionMaster.Location.GridArena.Model;
using UnityEngine;

namespace LegionMaster.Location.GridArena
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private CellId _id;
        [SerializeField] private Transform _scaleContainer;
        [SerializeField] private ArenaGridConfig _config;
        
        private SpriteRenderer _renderer;
        public bool IsFull { get; set; }
        public CellId Id => _id;
        public void Reset()
        {
            IsFull = false;
        }
        public void SetId(int y, int x)
        {
            _id = new CellId(y, x);
        }
        public void SetScale(Vector3 scale)
        {
            _scaleContainer.localScale = scale;
        }
        public void Highlight(CellHighlight highlight)
        {
            Renderer.color = _config.CellHighlightColors.First(it => it.Highlight == highlight).Color;
        }
        private SpriteRenderer Renderer => _renderer ??= GetComponentInChildren<SpriteRenderer>();

    
    }
}