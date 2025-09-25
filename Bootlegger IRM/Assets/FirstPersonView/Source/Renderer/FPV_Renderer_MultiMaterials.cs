using System;
using UnityEngine;

namespace FirstPersonView
{
    [AddComponentMenu("FPV/FPV Renderer Multi-Material")]
    public class FPV_Renderer_MultiMaterials: FPV_RendererBase
    {
        /// <summary>
        /// Array equal to the number of materials in this renderer.
        /// Each object contains the data to handle the FPV Renderer.
        /// </summary>
        [Header("Custom First Person Materials")]
        public FPV_Material[] fpvMaterials;

        // It's not possible to change a single material on the renderer, so we need to store every material in an array and then assign to the sharedMaterials of the renderer.
        /// <summary>
        /// Array containing all original materials of the renderer
        /// </summary>
        private Material[] originalMaterials;
        /// <summary>
        /// Array containing all First Person Materials of the renderer
        /// </summary>
        private Material[] firstPersonMaterials;

        public override void Setup(IFPV_Object parent, Renderer render)
        {
            base.Setup(parent, render);

            PrepareFPVMaterials();
            InitialteFPVMaterials();
        }

        /// <summary>
        /// Prepare the FPVMaterials variable.
        /// This is necessay in case the array in the inspector is not consistent with the number of materials in the renderer.
        /// </summary>
        private void PrepareFPVMaterials()
        {
            if(fpvMaterials == null || fpvMaterials.Length == 0)
            {
                //Simply create new array of same length if nothing has been assigned

                fpvMaterials = new FPV_Material[_render.sharedMaterials.Length];
                for (int i = 0; i < fpvMaterials.Length; i++)
                {
                    fpvMaterials[i] = new FPV_Material();
                }
            }
            else if (fpvMaterials.Length != _render.sharedMaterials.Length)
            {
                //Inconsistent number of items in the fpvMaterials and the number of materials of this renderer.
                //Create new Array for the fpvMaterials and copy/clamp the materials onto the array.

                FPV_Material[] newFpvMaterials = new FPV_Material[_render.sharedMaterials.Length];

                for (int i = 0; i < fpvMaterials.Length; i++)
                {
                    if(i < newFpvMaterials.Length)// copy data from fpvMaterials
                    {
                        newFpvMaterials[i] = fpvMaterials[i];
                    }
                    else
                    {
                        newFpvMaterials[i] = new FPV_Material();
                    }
                }

                fpvMaterials = newFpvMaterials;
            }
        }

        /// <summary>
        /// Initiate the Material of this renderer
        /// </summary>
        private void InitialteFPVMaterials()
        {
            //Keep the original materials
            originalMaterials = _render.sharedMaterials;

            //Create an instance of the materials
            firstPersonMaterials = _render.materials;

            for (int i = 0; i < fpvMaterials.Length; i++)
            {
                FPV_Material mat = fpvMaterials[i];
                mat.OriginalMaterial = originalMaterials[i];

                if (mat.FirstPersonViewMaterial == null) //Instance a new material from the original.
                {
                    mat.FirstPersonViewMaterial = firstPersonMaterials[i];
                }
                else //Use the FirstPersonViewMaterial assigned in the inspector.
                {
                    firstPersonMaterials[i] = mat.FirstPersonViewMaterial;
                }

                //Enable the Keyword FIRSTPERSONVIEW for this material
                mat.FirstPersonViewMaterial.EnableKeyword("BOOLEAN_FIRSTPERSONVIEW_ON");
            }

            _render.sharedMaterials = originalMaterials;
        }

        /// <summary>
        /// Set this renderer as First Person Object
        /// </summary>
        public override void SetAsFirstPersonObject()
        {
            _render.sharedMaterials = firstPersonMaterials;
        }

        /// <summary>
        /// Set this renderer as First Person Object
        /// </summary>
        public override void RemoveAsFirstPersonObject()
        {
            _render.sharedMaterials = originalMaterials;
        }

    }
}