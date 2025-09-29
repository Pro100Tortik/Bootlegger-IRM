namespace Bootlegger
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        bool InteractOnce { get; }

        void Interact(IInteractor interactor);

        void Select();
        void Deselect();
    }
}