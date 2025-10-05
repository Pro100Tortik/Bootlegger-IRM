using UnityEngine;

namespace Bootlegger
{
    public class DraggableBase : MonoBehaviour, IDraggable
    {
        [field: SerializeField] public bool CanBeDragged { get; protected set; }

        [SerializeField] protected Renderer objectRenderer;
        [SerializeField] protected Material outlineMaterial;
        protected Material[] originalMaterials;

        public virtual void Select()
        {
            originalMaterials = objectRenderer.materials;

            Material[] newMaterials = new Material[originalMaterials.Length + 1];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                newMaterials[i] = originalMaterials[i];
            }

            newMaterials[^1] = outlineMaterial;

            objectRenderer.materials = newMaterials;
        }

        public virtual void Deselect()
        {
            if (originalMaterials != null && objectRenderer != null)
            {
                objectRenderer.materials = originalMaterials;
            }
        }

        public virtual void StartDrag(Vector3 mousePosition) { }

        public virtual void UpdateDrag(Vector3 mousePosition) { }

        public virtual void StopDrag(Vector3 mousePosition) { }
    }
}
