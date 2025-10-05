using UnityEngine;

namespace Bootlegger
{
    public interface IDraggable
    {
        bool CanBeDragged { get; }

        void StartDrag(Vector3 mousePosition);
        void UpdateDrag(Vector3 mousePosition);
        void StopDrag(Vector3 mousePosition);

        void Select();
        void Deselect();
    }
}
