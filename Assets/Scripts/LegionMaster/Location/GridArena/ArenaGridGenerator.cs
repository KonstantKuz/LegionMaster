using LegionMaster.Location.GridArena.Config;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LegionMaster.Location.GridArena
{
    public class ArenaGridGenerator : MonoBehaviour
    {
        private const string CELL_NAME_PATTERN = "Cell_{0}_{1}";

        [SerializeField] private ArenaGridConfig _config;

        public void DestroyAllCells()
        {
#if UNITY_EDITOR            
            if (PrefabUtility.IsPartOfPrefabInstance(gameObject)) {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            } 
#endif            
            for (int i = transform.childCount; i > 0; --i) {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
        public void Generate()
        {
            var verticalOffset = transform.forward * _config.CellStep * _config.Dimensions.y / 2;
            var horizontalOffset = transform.right * _config.CellStep * _config.Dimensions.x / 2;
            Vector3 bottomLeft;
            bottomLeft = transform.position - verticalOffset - horizontalOffset;
            
            for (int y = 0; y < _config.Dimensions.y; y++)
            {
                for (int x = 0; x < _config.Dimensions.x; x++)
                {
                    verticalOffset = transform.forward * _config.CellStep * (y + 0.5f);
                    horizontalOffset = transform.right * _config.CellStep * (x + 0.5f);
                    
                    var cell = CreateCell(y, x);
                    
                    cell.transform.position = bottomLeft + horizontalOffset + verticalOffset;
                }
            }
        }
        private GridCell CreateCell(int y, int x)
        {
            var cell = Instantiate(_config.CellPrefab, gameObject.transform, false);
            cell.name = string.Format(CELL_NAME_PATTERN, y, x);
            cell.SetId(y, x);   
            cell.SetScale(_config.CellScale);
            return cell;
        }
    }
}
