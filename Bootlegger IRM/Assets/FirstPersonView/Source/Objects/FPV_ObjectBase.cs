using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonView
{
    /// <summary>
    /// Base class for FPV_Objects.
    /// Type T must be of type IFPV_Renderer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FPV_ObjectBase<T> : MonoBehaviour, IFPV_Object where T : IFPV_Renderer
    {
        /// <summary>
        /// Is this object a First Person type object.
        /// </summary>
        protected bool _isFirstPersonObject;

        /// <summary>
        /// Container of all renderers inside this object.
        /// </summary>
        protected List<T> _renderers;
        

        void Awake()
        {
            Setup();
        }

        /// <summary>
        /// Is this object a First Person type object.
        /// </summary>
        /// <returns></returns>
        public bool IsFirstPersonObject()
        {
            return _isFirstPersonObject;
        }

        /// <summary>
        /// Setup method for the object.
        /// </summary>
        protected virtual void Setup()
        {
            _isFirstPersonObject = false;
            SetRenderers();
        }

        /// <summary>
        /// Set all the renderers inside this GameObject
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SetRenderers()
        {
            _renderers = new List<T>();
            Stack<Transform> toProcess = new Stack<Transform>();

            toProcess.Push(transform);

            while (toProcess.Count > 0)
            {
                Transform t = toProcess.Pop();

                AddRenderer(t);

                for (int i = 0; i < t.childCount; i++) //Add childs to process
                {
                    //Don't add if this is another FPV_Object
                    if (t.GetChild(i).GetComponent<IFPV_Object>() != null) continue;

                    toProcess.Push(t.GetChild(i));
                }
            }
        }

        /// <summary>
        /// Add a new IFPV_Renderer to the container if the transform contains any.
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void AddRenderer(Transform trans)
        {
            Renderer render = trans.GetComponent<Renderer>();

            if (render != null) //Add new IFPV_Renderer only if the object contains a Renderer.
            {
                T fpvRender = trans.GetComponent<T>();

                if (fpvRender == null) //Create new IFPV_Renderer if it doesn't contain one
                {
                    fpvRender = AddFPVRendererComponent(trans.gameObject, render);
                }

                fpvRender.Setup(this, render);
                _renderers.Add(fpvRender);
            }
        }

        /// <summary>
        /// Add a new IFPV_Renderer to the given gameObject, specific to the FPV_Object implementation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="render"></param>
        /// <returns></returns>
        protected abstract T AddFPVRendererComponent(GameObject obj, Renderer render);

        /// <summary>
        /// Remove a renderer from the list of renderers of this object.
        /// This might be costly if this FPV_Object has too many renderers. Should be fine for small number of renderers
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void RemoveRenderer(Transform trans)
        {
            _renderers.Remove(trans.GetComponent<T>());
        }

        /// <summary>
        /// Set this and all objects inside as First Person Render objects.
        /// </summary>
        public virtual void SetAsFirstPersonObject()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].SetAsFirstPersonObject();
            }
            _isFirstPersonObject = true;
        }

        /// <summary>
        /// Remove this and all objects inside from First Person Render objects.
        /// </summary>
        public virtual void RemoveAsFirstPersonObject()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].RemoveAsFirstPersonObject();
            }
            _isFirstPersonObject = false;
        }
        
    }
}
