using UnityEngine;

namespace Bootlegger
{
    public class Prop : SaveableBehaviour, IGrabbable
    {
        public Transform Transform => transform;
        public Rigidbody RigidBody { get; private set; }
        public bool IsGrabbed { get; protected set; } = false;
        public bool CanInteract => true;
        public bool InteractOnce => false;

        [SerializeField] private bool enableGravityOnRelease = true;
        [SerializeField] private Material outlineMaterial;
        private Material[] _originalMaterials;
        private Renderer _renderer;

        private float _previousDamping;
        private float _previousAngularDamping;

        private void Awake()
        {
            RigidBody = GetComponent<Rigidbody>();
            _renderer = GetComponent<Renderer>();
        }

        public void Grab()
        {
            IsGrabbed = true;

            RigidBody.useGravity = false;
            RigidBody.angularVelocity = Vector3.zero;
            RigidBody.linearVelocity = Vector3.zero;

            _previousDamping = RigidBody.linearDamping;
            _previousAngularDamping = RigidBody.angularDamping;

            RigidBody.linearDamping = 10f;
            RigidBody.angularDamping = 10f;

            Deselect();
        }

        public void Throw(Vector3 throwForce)
        {
            RigidBody.useGravity = enableGravityOnRelease;
            RigidBody.linearVelocity += throwForce;

            RigidBody.linearDamping = _previousDamping;
            RigidBody.angularDamping = _previousAngularDamping;

            IsGrabbed = false;
        }

        public void UpdatePosition(Vector3 targetPosition)
        {
            //Vector3 pointVelocity = (point - _lastPoint) / Time.fixedDeltaTime;
            //_lastPoint = point;

            Vector3 currentPosition = RigidBody.worldCenterOfMass;

            Vector3 direction = targetPosition - currentPosition;
            Vector3 desiredVelocity = direction * 10f;

            RigidBody.linearVelocity = desiredVelocity;// + pointVelocity;
        }

        public void Select()
        {
            if (IsGrabbed)
                return;

            _originalMaterials = _renderer.materials;

            Material[] newMaterials = new Material[_originalMaterials.Length + 1];

            for (int i = 0; i < _originalMaterials.Length; i++)
            {
                newMaterials[i] = _originalMaterials[i];
            }

            newMaterials[^1] = outlineMaterial;

            _renderer.materials = newMaterials;
        }

        public void Deselect()
        {
            if (_originalMaterials != null && _renderer != null)
            {
                _renderer.materials = _originalMaterials;
            }
        }

        public void Interact(IInteractor interactor)
        {

        }
    }
}
