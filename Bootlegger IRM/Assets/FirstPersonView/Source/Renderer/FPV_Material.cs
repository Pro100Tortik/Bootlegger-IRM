using UnityEngine;

namespace FirstPersonView
{
    [System.Serializable]
    public class FPV_Material
    {
        /// <summary>
        /// Original Material of the assigned material in the Renderer.
        /// </summary>
        [HideInInspector]
        public Material OriginalMaterial;

        /// <summary>
        /// The first person material that will be used for the first person object.
        /// </summary>
        [Header("Assign an FPV Material to use a custom material. Leave empty to use original material.")]
        public Material FirstPersonViewMaterial;
    }
}
