using System.Collections.Generic;
using LegionMaster.Core.Config;
using LegionMaster.Extension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Components
{
    public class SpriteListView : MonoBehaviour
    {
        [SerializeField] private SpriteView _prefab;
        [Inject] private DiContainer _container;
        
        public void Init(IEnumerable<Sprite> sprites)
        {
            transform.DestroyAllChildren();
            gameObject.SetActive(sprites != null);
            if (sprites == null) return;
            foreach (var sprite in sprites)
            {
                var spriteView = _container.InstantiatePrefabForComponent<SpriteView>(_prefab, transform);
                spriteView.Init(sprite);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    
        }
    }
}