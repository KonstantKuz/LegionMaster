using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.GridArena;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor.Scripts
{
    [CustomEditor(typeof(ArenaGridGenerator))]
    public class ArenaGridGeneratorEditor : UnityEditor.Editor
    {
        private readonly List<string> _serializedPropertyNames = new List<string>() {
                "_config",
        };
        private List<SerializedProperty> _serializedProperties;
        private ArenaGridGenerator _arenaGridGenerator;
     
        private void OnEnable()
        {
            _arenaGridGenerator = (ArenaGridGenerator) target;
            _serializedProperties = _serializedPropertyNames.Select(name => serializedObject.FindProperty(name)).ToList();
        }
        
        public override void OnInspectorGUI()
        {
            UpdateModifiedProperties();
            
            if (GUILayout.Button("Destroy grid")) {
                _arenaGridGenerator.DestroyAllCells();
                MarkTargetDirty();
            }  
            if (GUILayout.Button("Generate grid")) {
                _arenaGridGenerator.DestroyAllCells();
                _arenaGridGenerator.Generate();
                MarkTargetDirty();
            }
        }

        private void UpdateModifiedProperties()
        {
            serializedObject.Update();
            _serializedProperties.ForEach(s => { EditorGUILayout.PropertyField(s); });
            serializedObject.ApplyModifiedProperties();
        }
        private void MarkTargetDirty()
        {
            if (Application.isPlaying) {
                return;
            }
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(_arenaGridGenerator.gameObject.scene);  
        }
    }
}