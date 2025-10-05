using UnityEngine;

namespace Bootlegger
{
    public class DraggableCube : DraggableBase
    {
        public override void StartDrag(Vector3 mousePosition)
        {
        }

        public override void UpdateDrag(Vector3 mousePosition)
        {
            Vector3 position = new Vector3(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

            transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        }

        public override void StopDrag(Vector3 mousePosition)
        {
        }
    }
}
