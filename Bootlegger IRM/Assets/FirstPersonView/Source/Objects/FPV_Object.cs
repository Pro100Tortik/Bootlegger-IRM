using System;
using UnityEngine;

namespace FirstPersonView
{
    [AddComponentMenu("FPV/FPV Object")]
    public class FPV_Object : FPV_ObjectBase<IFPV_Renderer>, IFPV_Object
    {
        /// <summary>
        /// Add a new IFPV_Renderer to the given gameObject, specific to the FPV_Object implementation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="render"></param>
        /// <returns></returns>
        protected override IFPV_Renderer AddFPVRendererComponent(GameObject obj, Renderer render)
        {
            //Add FPV_SM_Renderer to a renderer that only has 1 material. Add MultiMaterials to a multi-material renderer.
            if(render.sharedMaterials.Length == 1)
            {
                return obj.AddComponent<FPV_Renderer>();
            }
            else
            {
                return obj.AddComponent<FPV_Renderer_MultiMaterials>();
            }
        }

    }
}
