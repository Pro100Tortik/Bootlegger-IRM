using UnityEngine;

namespace Bootlegger
{
    public interface IGrabbable : IInteractable
    {
        Transform Transform { get; }
        Rigidbody RigidBody { get; }
        bool IsGrabbed { get; }

        void Grab();
        void Throw(Vector3 throwForce);

        void UpdatePosition(Vector3 targetPosition);
    }
}
