using UnityEngine;

namespace Bootlegger
{
    public abstract class InteractionBase : SaveableBehaviour, IInteractable
    {
        [field: SerializeField] public bool IsOn { get; protected set; } = true;
        [field: SerializeField] public bool CanInteract { get; private set; } = true;
        [field: SerializeField] public bool InteractOnce { get; private set; } = false;

        [SerializeField] protected Renderer objectRenderer;
        [SerializeField] protected Material outlineMaterial;
        protected Material[] originalMaterials;

        protected bool alreadyInteracted = false;

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

        public virtual void Interact(IInteractor interactor)
        {
            if (InteractOnce && alreadyInteracted)
                return;

            ToggleState();
            OnInteract();
            alreadyInteracted = true;
        }

        private void ToggleState()
        {
            IsOn = !IsOn;
        }

        protected abstract void OnInteract();
    }
}
