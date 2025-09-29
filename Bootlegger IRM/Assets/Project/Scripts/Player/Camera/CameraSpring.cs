using UnityEngine;

namespace Bootlegger
{
    public class CameraSpring : PausableBehaviour
    {
        [SerializeField] private float frequency = 18f;
        [SerializeField] private float halflife = 0.075f;
        [SerializeField] private float angularDisplacement = 1f;
        [SerializeField] private float linearDisplacement = 0.05f;
        private Vector3 _springPosition;
        private Vector3 _springVelocity;

        protected override void OnFixedUpdate()
        {
            UpdateSpring();
        }

        private void UpdateSpring()
        {
            transform.localPosition = Vector3.zero;

            Spring(ref _springPosition, ref _springVelocity, transform.position, halflife, frequency, Time.fixedDeltaTime);

            Vector3 localSpringPos = _springPosition - transform.position;
            float springHeight = Vector3.Dot(localSpringPos, transform.up);

            transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0, 0);
            transform.localPosition = localSpringPos * linearDisplacement;
            transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, linearDisplacement);
        }

        private void Spring(ref Vector3 current, ref Vector3 velocity, Vector3 target, float halfLife, float frequency, float timeStep)
        {
            float dampingRatio = -Mathf.Log(0.5f) / (frequency * halfLife);
            float f = 1.0f + 2.0f * timeStep * dampingRatio * frequency;
            float oo = frequency * frequency;
            float hoo = timeStep * oo;
            float hhoo = timeStep * hoo;
            float detInv = 1.0f / (f + hhoo);
            Vector3 detX = f * current + timeStep * velocity + hhoo * target;
            Vector3 detV = velocity + hoo * (target - current);
            current = detX * detInv;
            velocity = detV * detInv;
        }
    }
}
