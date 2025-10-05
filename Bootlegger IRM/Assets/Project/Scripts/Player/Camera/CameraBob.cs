using UnityEngine;

namespace Bootlegger
{
    public class CameraBob : PausableBehaviour
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private Transform head;
        [SerializeField] private Transform cam;
        [SerializeField] private float minimumMoveSpeed = 0.3f;
        [SerializeField] private float minimumSprintSpeed = 7;
        [SerializeField] private float idleMultiplier = 1.75f;
        [SerializeField] private float transitionSpeed = 3;
        [SerializeField] private float normalBobAmount = 0.015f;
        [SerializeField] private float sprintBobAmount = 0.05f;
        [SerializeField] private float xOffsetMagnitude = 4f;
        [SerializeField] private float yOffsetMagnitude = 1f;
        [SerializeField] private float rotationDegrees = 2f;
        private Transform _transform;
        private float _timer = Mathf.PI / 2;
        private float _bobSpeed;
        private float _bobAmount;
        private bool _isSprint;
        private bool _isMoving;
        private Vector3 _originalLocalPosition;

        private void Awake()
        {
            _transform = transform;
            _originalLocalPosition = _transform.localPosition; // Сохраняем исходную позицию
        }

        protected override void OnFixedUpdate()
        {
            if (movementController == null)
                return;

            float speed = movementController.GetVelocity().magnitude;

            _isMoving = speed >= minimumMoveSpeed;
            _isSprint = speed >= minimumSprintSpeed;
            _bobSpeed = speed >= 0.3f ? Mathf.Min(speed * 1.5f, 10) : 1;
            _bobAmount = _isSprint ? sprintBobAmount : normalBobAmount;

            if (movementController.IsGrounded)
                CalculateBob();

            cam.LookAt(head.position + head.forward * 100f);
        }

        private void CalculateBob()
        {
            Vector3 targetPosition = _originalLocalPosition; // Начинаем с исходной позиции

            if (_isMoving)
            {
                _timer += _bobSpeed * Time.fixedDeltaTime;

                // Добавляем смещение к исходной позиции
                targetPosition += new Vector3(
                    Mathf.Sin(_timer) * _bobAmount * xOffsetMagnitude,
                    Mathf.Sin(_timer * 2) * _bobAmount * yOffsetMagnitude,
                    0
                );

                transform.localRotation = Quaternion.Euler(Mathf.Sin(_timer * 2) * _bobAmount * rotationDegrees
                    , transform.localRotation.y, transform.localRotation.z);
            }
            else
            {
                _timer += _bobSpeed * Time.fixedDeltaTime;

                // Только вертикальное покачивание в покое
                targetPosition += new Vector3(
                    0,
                    Mathf.Sin(_timer * 2) * _bobAmount * yOffsetMagnitude * idleMultiplier,
                    0
                );

                transform.localRotation = Quaternion.Euler(Mathf.Sin(_timer * 2) * normalBobAmount * rotationDegrees
                    , transform.localRotation.y, transform.localRotation.z);
            }

            // Плавно интерполируем к целевой позиции
            _transform.localPosition = Vector3.Lerp(
                _transform.localPosition,
                targetPosition,
                Time.fixedDeltaTime * transitionSpeed
            );

            if (_timer > Mathf.PI * 2)
                _timer = 0;
        }

        private void ResetToOriginalPosition()
        {
            // Плавно возвращаем камеру в исходную позицию
            _transform.localPosition = Vector3.Lerp(
                _transform.localPosition,
                _originalLocalPosition,
                Time.fixedDeltaTime * transitionSpeed
            );

            // Сбрасываем таймер для плавного перехода
            _timer = Mathf.PI / 2;
        }
    }
}
