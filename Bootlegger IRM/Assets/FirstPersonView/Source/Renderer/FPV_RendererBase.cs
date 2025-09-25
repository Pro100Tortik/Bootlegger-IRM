using UnityEngine;

namespace FirstPersonView
{
    public abstract class FPV_RendererBase : MonoBehaviour, IFPV_Renderer
    {
        /// <summary>
        /// The FPV Object that this renderer is part of
        /// </summary>
        protected IFPV_Object _parent;

        /// <summary>
        /// The renderer of this object
        /// </summary>
        protected Renderer _render;
        
        /// <summary>
        /// Setup method for this class
        /// </summary>
        /// <param name="render"></param>
        /// <param name="parent"></param>
        public virtual void Setup(IFPV_Object parent, Renderer render)
        {
            _parent = parent;
            _render = render;
        }

        /// <summary>
        /// In case we want to update some value, ex. new type of shadowCastMode or layer, then call this method.
        /// </summary>
        public virtual void ReSetupComponent()
        {

        }

        /// <summary>
        /// Set this renderer's layer as First Person Object and set the flag isFirstPersonObject to TRUE
        /// </summary>
        public abstract void SetAsFirstPersonObject();

        /// <summary>
        /// Remove this renderer's layer from First Person Object to a world object and set the flag isFirstPersonObject to FALSE
        /// </summary>
        public abstract void RemoveAsFirstPersonObject();

        // ----- Unity Callbacks -----

        void OnDestroy()
        {
            _parent.RemoveRenderer(transform);
        }

    }
}
