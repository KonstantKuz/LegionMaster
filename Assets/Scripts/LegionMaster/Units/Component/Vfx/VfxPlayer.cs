using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegionMaster.Units.Component.Vfx
{
    public class VfxPlayer : MonoBehaviour
    {
        [Serializable]
        private struct VfxParam
        {
            public Vfx Prefab;
            public Transform Container;
        }
        
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private List<VfxParam> _vfxes;
        
        public void PlayVfx(VfxType type)
        {
            var vfxParam = _vfxes.First(it => it.Prefab.Type == type);
            Instantiate(vfxParam.Prefab, vfxParam.Container != null ? vfxParam.Container : _root);
        }
        public void StopVfx(VfxType type)
        {
            var activeVfx = transform.GetComponentsInChildren<Vfx>()
                                     .FirstOrDefault(it => it.Type == type);
            if (activeVfx == null) {
                Debug.LogWarning($"Vfx already Destroyed, vfxType:= {type}");
                return;
            } 
            Destroy(activeVfx.gameObject);
        }
        
    }
}