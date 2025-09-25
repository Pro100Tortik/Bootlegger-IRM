using UnityEngine;

namespace FirstPersonView
{
    public interface IFPV_Renderer
    {
        /// <summary>
        /// Setup the renderer
        /// </summary>
        /// <param name="render"></param>
        void Setup(IFPV_Object parent, Renderer render);

        /// <summary>
        /// In case we want to update some value, ex. new type of shadowCastMode/ Material/ layer, then call this method.
        /// </summary>
        void ReSetupComponent();


        /// <summary>
        /// Set this renderer as First Person Object
        /// </summary>
        void SetAsFirstPersonObject();

        /// <summary>
        /// Set this renderer as First Person Object
        /// </summary>
        void RemoveAsFirstPersonObject();
    }
}
