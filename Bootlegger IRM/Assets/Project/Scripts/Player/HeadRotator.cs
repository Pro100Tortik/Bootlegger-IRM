using UnityEngine;

namespace Bootlegger
{
    public class HeadRotator : SaveableBehaviour
    {
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        [Header("Rotator Settings")]
        [SerializeField] private float minVerticalAngle = -90f;
        [SerializeField] private float maxVerticalAngle = 90f;
        private CameraInput _requestedCameraInput;
        private Transform _transform; // Improves performance a bit

        private void Awake()
        {
            _transform = transform;
        }

        public void SetLookDirection(Vector3 lookEulerAngles)
        {
            Pitch = lookEulerAngles.x.NormalizeAngle();
            Yaw = lookEulerAngles.y.NormalizeAngle();

            if (Pitch > 90f)
            {
                Pitch = 180f - Pitch;
                Yaw += 180f;
            }
            else if (Pitch < -90f)
            {
                Pitch = -180f - Pitch;
                Yaw += 180f;
            }

            Yaw = Yaw.NormalizeAngle();

            transform.localRotation = Quaternion.Euler(Pitch, Yaw, 0f);
        }

        public void UpdateInput(CameraInput cameraInput)
        {
            _requestedCameraInput = cameraInput;
        }

        protected override void OnLateUpdate()
        {
            Yaw += _requestedCameraInput.LookInput.x;

            float finalVerticalInput = -1f * _requestedCameraInput.LookInput.y;
            Pitch += finalVerticalInput;

            Pitch = Mathf.Clamp(Pitch, minVerticalAngle, maxVerticalAngle);

            _transform.localRotation = Quaternion.Euler(Pitch, Yaw, 0f);
        }
    }
}
