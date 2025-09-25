using UnityEngine;

namespace FirstPersonView
{
    /// <summary>
    /// Interface class for all FPV Objects
    /// </summary>
    public interface IFPV_Object
    {
        /// <summary>
        /// Is this object a First Person type object.
        /// </summary>
        /// <returns></returns>
        bool IsFirstPersonObject();

        /// <summary>
        /// Add a new renderer to this FPV_Object
        /// </summary>
        /// <param name="trans"></param>
        void AddRenderer(Transform trans);

        /// <summary>
        /// Remove a renderer from the list of renderers of this object.
        /// This might be costly if this FPV_Object has too many renderers. Should be fine for small number of renderers
        /// </summary>
        /// <param name="trans"></param>
        void RemoveRenderer(Transform trans);

        /// <summary>
        /// Set this and all objects inside as First Person Object objects.
        /// </summary>
        void SetAsFirstPersonObject();

        /// <summary>
        /// Remove this and all objects inside as First Person Object objects.
        /// </summary>
        void RemoveAsFirstPersonObject();
    }
}
