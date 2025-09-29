using UnityEngine;

namespace Bootlegger
{
    public class CameraLean : PausableBehaviour
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private float attackDamping = 0.5f;
        [SerializeField] private float decayDamping = 0.3f;
        [SerializeField] private float strength = 0.075f;
        [SerializeField] private float maxTiltAngle = 5f;
        private Vector3 _dampedAccel;
        private Vector3 _dampedAccelVel;

        protected override void OnFixedUpdate()
        {
            UpdateLean();
        }

        private void UpdateLean()
        {
            if (movementController == null)
                return;

            //if (SettingsManager.GameSettings.EnableCameraTilt == false)
            //{
            //    transform.localRotation = Quaternion.identity;
            //    return;
            //}

            Vector3 planarAccel = Vector3.ProjectOnPlane(movementController.GetCurrentAcceleration(), movementController.transform.up);
            float damping = planarAccel.magnitude > _dampedAccel.magnitude ? attackDamping : decayDamping;
            _dampedAccel = Vector3.SmoothDamp(_dampedAccel, planarAccel, ref _dampedAccelVel, damping, float.PositiveInfinity, Time.deltaTime);

            _dampedAccel = Vector3.ClampMagnitude(_dampedAccel, maxTiltAngle);

            Vector3 leanAxis = Vector3.Cross(_dampedAccel.normalized, movementController.transform.up).normalized;
            transform.localRotation = Quaternion.identity;
            transform.rotation = Quaternion.AngleAxis(_dampedAccel.magnitude * strength, leanAxis) * transform.rotation;
        }
    }
}
