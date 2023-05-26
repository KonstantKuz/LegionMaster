using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace LegionMaster.Tutorial.UI
{
    public class CutoutMaskImage : Image 
    {
        public override Material materialForRendering
        {
            get
            {
                Material result = new Material(base.materialForRendering);
                result.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return result;
            }
        }
    }
}