using KinematicCharacterController;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Bootlegger
{
    public class MovementController : SaveableBehaviour, ICharacterController
    {
        private enum MoveType
        {
            Walk,
            Ladder,
            Swim,
            Noclip
        }

        public event Action LeavedGround;
        public event Action Landed;
        public event Action Jumped;
        public event Action LandedInWater;
        public event Action GotOnLadder;
        public event Action<Collider> OnTriggerEnter;
        public event Action<Collider> OnTriggerExit;

        public Transform GetCameraTarget() => cameraTarget;
        public Vector3 GetCurrentAcceleration() => _acceleration;
        public float GetWaterDepth() => _waterDepth;
        public float GetRadius() => motor.Capsule.radius;
        public LayerMask GetCollisionMask() => motor.CollidableLayers;

        public static bool AutoJump { get; set; } = false;
        public bool IsGrounded => motor.GroundingStatus.IsStableOnGround;
        public bool IsCrouching => _isCrouching;

        [SerializeField] private MoveType moveType = MoveType.Walk;
        [SerializeField] private KinematicCharacterMotor motor;
        [SerializeField] private HeadRotator headRotator;
        [SerializeField] private Transform root;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float standingCameraHeight = 1.65f;
        [SerializeField] private float crouchingCameraHeight = 0.825f;
        [SerializeField] private float crouchingSmoothness = 10f;

        [SerializeField] private float interpolation = 0.5f;

        private Vector3 _acceleration;
        private Vector3 _ladderNormal;
        private bool _isCrouching;
        private bool _isOnLadder;
        private float _currentAcceleration;
        private float _coyoteTimer;
        private float _jumpTimer;
        private float _ignoreFrictionTimer;
        private float _waterDepth;

        private bool _needToReleaseJump = false;

        #region Movement Modifiers
        private float _gravityModifier = 1f;
        private float _movementSpeedModifier = 1f;
        private bool _isMovementAllowed = true;
        #endregion

        private Quaternion _requestedRotation;
        private Vector3 _requestedMovement;
        private Vector3 _lastVelocity;
        private bool _requestedJump;
        private bool _requestedRun;
        private bool _requestedCrouch;
        private bool _wasGrounded;

        private Collider[] _overlapBuffer = new Collider[8];
        private readonly HashSet<Collider> _currentTriggers = new();
        private readonly HashSet<Collider> _triggersThisFrame = new();

        protected override void OnPause(in PauseRequest request)
        {
            _lastVelocity = motor.BaseVelocity;
            motor.BaseVelocity = Vector3.zero;
        }

        protected override void OnResume(in PauseRequest request)
        {
            motor.BaseVelocity = _lastVelocity;
        }

        private void Awake()
        {
            motor.CharacterController = this;
        }

        protected override void Start()
        {
            base.Start();

            if (root.TryGetComponent(out MeshRenderer rootRenderer))
                rootRenderer.enabled = false;

            for (int i = 0; i < root.childCount; i++)
            {
                //root.GetChild(i).gameObject.SetActive(false);
                //if (root.GetChild(i).TryGetComponent(out MeshRenderer renderer))
                //    renderer.enabled = false;
            }
        }

        //public void ToggleNoclip()
        //{
        //    moveType = moveType != MoveType.Noclip ? MoveType.Noclip : MoveType.Walk;
        //    motor.SetMovementCollisionsSolvingActivation(moveType != MoveType.Noclip);
        //}

        protected override void OnFixedUpdate()
        {
            UpdateBody();
        }

        public void UpdateInput(ControllerInput input)
        {
            _requestedRotation = cameraTarget.rotation;

            _requestedMovement = _isMovementAllowed ? new Vector3(input.MoveInput.x, 0, input.MoveInput.y) : Vector3.zero;
            _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

            _requestedJump = _isMovementAllowed ? input.Jump : false;
            _requestedRun = input.Run;
            _requestedCrouch = input.Crouch;
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {

        }

        // Fixed Update in KCC
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (IsPaused)
                return;

            CalculateMovement(ref currentVelocity, deltaTime);
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            // Reset movement variables
            if (motor.GroundingStatus.IsStableOnGround && _jumpTimer <= 0)
            {
                _coyoteTimer = Time.fixedDeltaTime * GlobalVariables.coyoteTimeInTicks + 0.01f;
            }
            else if (_coyoteTimer > 0)
            {
                _coyoteTimer -= deltaTime;
            }

            if (_jumpTimer > 0)
                _jumpTimer -= deltaTime;

            if (_ignoreFrictionTimer > 0 && motor.GroundingStatus.IsStableOnGround)
                _ignoreFrictionTimer -= deltaTime;

            HandleTriggerEnterAndStay();
        }

        public void PostGroundingUpdate(float deltaTime) { }

        public void AfterCharacterUpdate(float deltaTime)
        {
            if (moveType == MoveType.Noclip)
                return;

            // TODO: do other solution to check if player is on ladder
            int found = Physics.OverlapSphereNonAlloc(motor.InitialTickPosition, motor.Capsule.radius + 0.15f, _overlapBuffer, motor.CollidableLayers, QueryTriggerInteraction.Collide);

            Ladder ladder = null;

            for (int i = 0; i < found; i++)
            {
                if (_overlapBuffer[i].TryGetComponent(out ladder))
                    break;
            }

            if (ladder == null)
            {
                _ladderNormal = Vector3.zero;
                _isOnLadder = false;
            }

            HandleTriggerExit();
        }

        private void HandleTriggerEnterAndStay()
        {
            int found = motor.CharacterOverlap(
                motor.InitialTickPosition,
                Quaternion.identity,
                _overlapBuffer,
                motor.CollidableLayers,
                QueryTriggerInteraction.Collide
            );

            _waterDepth = 0;
            _triggersThisFrame.Clear();

            for (int i = 0; i < found; i++)
            {
                var collider = _overlapBuffer[i];
                if (collider == null || collider.isTrigger == false)
                    continue;

                _triggersThisFrame.Add(collider);

                if (_currentTriggers.Add(collider))
                    OnTriggerEnter?.Invoke(collider);

                if (!collider.TryGetComponent(out Ladder water))
                    continue;

                if (_isMovementAllowed == false)
                    _isMovementAllowed = true;

                if (moveType != MoveType.Swim && _waterDepth >= 0 && Mathf.Abs(motor.BaseVelocity.y) > 5f)
                    LandedInWater?.Invoke();

                _waterDepth = 0.1f;
                Vector3 headPoint = cameraTarget.position;
                Bounds bounds = collider.bounds;

                if (bounds.Contains(headPoint))
                {
                    _waterDepth = 1.0f;
                }
                else
                {
                    Vector3 closestPoint = collider.ClosestPoint(headPoint);
                    float dist = Vector3.Distance(headPoint, closestPoint);
                    float relativeDepth = (cameraTarget.localPosition.y - dist) / cameraTarget.localPosition.y;
                    _waterDepth = Mathf.Clamp(Mathf.Max(0.1f, relativeDepth), 0f, 1f);
                }
            }

            if (_waterDepth >= GlobalVariables.waterLevelToSwim)
                moveType = MoveType.Swim;
            else if (moveType == MoveType.Swim)
                moveType = MoveType.Walk;
        }

        private void HandleTriggerExit()
        {
            foreach (var previous in _currentTriggers.ToArray())
            {
                if (!_triggersThisFrame.Contains(previous))
                {
                    OnTriggerExit?.Invoke(previous);
                    _currentTriggers.Remove(previous);
                }
            }
        }

        public bool IsColliderValidForCollisions(Collider coll) => true;

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // Ladder detection
            if (hitCollider.TryGetComponent(out Ladder _) && Vector3.Angle(motor.CharacterUp, hitNormal) > GlobalVariables.slopeLimit)
            {
                if (_isMovementAllowed == false)
                    _isMovementAllowed = true;

                if (moveType != MoveType.Ladder)
                    GotOnLadder?.Invoke();

                _ladderNormal = hitNormal;
                _isOnLadder = true;
                moveType = MoveType.Ladder;
                motor.ForceUnground(0f);
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

        public void OnDiscreteCollisionDetected(Collider hitCollider) { }

        public void UpdateBody()
        {
            float currentHeight = _isCrouching ? crouchingCameraHeight : standingCameraHeight;
            float normalizedHeight = _isCrouching ? 0.45f : 1f;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, Vector3.up * currentHeight, 1f - Mathf.Exp(-crouchingSmoothness * Time.fixedDeltaTime));
                root.localScale = root.localScale.FlattenVector() + Vector3.up * normalizedHeight;
            }
            else
            {
                cameraTarget.localPosition = Vector3.up * currentHeight;
                root.localScale = root.localScale.FlattenVector() + Vector3.up * normalizedHeight;
            }

            root.localPosition = root.localPosition.FlattenVector() + Vector3.up * 0f;
        }

        private void CalculateMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_wasGrounded && motor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity = Vector3.ProjectOnPlane(_lastVelocity, motor.GroundingStatus.GroundNormal);

                if (_gravityModifier < 0f)
                    motor.ForceUnground(0f);

                if (_isMovementAllowed == false)
                    _isMovementAllowed = true;

                Landed?.Invoke();
            }

            if (_wasGrounded && !motor.GroundingStatus.IsStableOnGround)
                LeavedGround?.Invoke();

            _wasGrounded = motor.GroundingStatus.IsStableOnGround;

            Vector3 movementVelocity = currentVelocity;

            _currentAcceleration = GlobalVariables.acceleration;

            if (_isCrouching && (moveType != MoveType.Noclip))
                _currentAcceleration *= GlobalVariables.duckMultiplier;

            _acceleration = Vector3.zero;

            if (moveType != MoveType.Noclip)
            {
                if (!_isOnLadder && moveType == MoveType.Ladder)
                {
                    moveType = MoveType.Walk;
                }
            }

            CheckCrouch();
            JumpQueue();

            motor.SetGroundSolvingActivation(moveType == MoveType.Walk || moveType == MoveType.Ladder);

            switch (moveType)
            {
                case MoveType.Walk:
                    WalkMovement(ref movementVelocity, deltaTime);
                    break;

                case MoveType.Ladder:
                    LadderMovement(ref movementVelocity);
                    break;

                case MoveType.Noclip:
                    NoclipMovement(ref movementVelocity, deltaTime);
                    break;

                case MoveType.Swim:
                    WaterMovement(ref movementVelocity, deltaTime);
                    break;
            }

            _acceleration = (movementVelocity - currentVelocity) / deltaTime;

            currentVelocity = movementVelocity;
            _lastVelocity = currentVelocity;
        }

        private void WalkMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 moveDirection = _requestedRotation.FlattenRotation(motor.CharacterUp) * _requestedMovement;

            if (_requestedJump && !_needToReleaseJump)
            {
                Jump(ref currentVelocity);
            }

            _acceleration = (motor.Velocity - currentVelocity) / deltaTime;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                if (_ignoreFrictionTimer <= 0)
                {
                    moveDirection.ProjectOnPlane(motor.GroundingStatus.GroundNormal);
                    moveDirection.Normalize();

                    float targetSpeed = GlobalVariables.maxGroundSpeed;

                    if (_requestedRun)
                        targetSpeed *= GlobalVariables.shiftMultiplier;

                    currentVelocity += GroundAccelerate(currentVelocity, moveDirection, targetSpeed * _movementSpeedModifier, _currentAcceleration, deltaTime);
            
                    ApplyFriction(ref currentVelocity, deltaTime);
                }
            }
            else
            {
                currentVelocity += AirAccelerate(currentVelocity, moveDirection, GlobalVariables.airSpeed, GlobalVariables.airAcceleration, GlobalVariables.airSpeedCap, deltaTime);
            
                ApplyGravity(ref currentVelocity, deltaTime);
            }
        }

        private void ApplyGravity(ref Vector3 currentVelocity, float deltaTime) => currentVelocity += deltaTime * -GlobalVariables.gravity * _gravityModifier * motor.CharacterUp;

        private void JumpQueue()
        {
            if (AutoJump)
            {
                _needToReleaseJump = false;
                return;
            }

            if (!_requestedJump)
            {
                _needToReleaseJump = false;
            }
        }

        private void Jump(ref Vector3 currentVelocity)
        {
            if (_jumpTimer > 0 || _coyoteTimer <= 0)
                return;

            motor.ForceUnground(0f);

            // Player can jump once per 5 ticks
            _jumpTimer = Time.fixedDeltaTime * GlobalVariables.jumpCooldownInTicks + 0.01f;
            _coyoteTimer = 0f;

            _needToReleaseJump = true;

            Jumped?.Invoke();

            float jumpSpeed = Mathf.Sqrt(-2f * -GlobalVariables.gravity * GlobalVariables.jumpForce);

            float alignedSpeed = Vector3.Dot(currentVelocity, motor.GroundingStatus.GroundNormal);
            if (alignedSpeed > 0f)
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);

            currentVelocity += motor.CharacterUp * jumpSpeed;

            // Ignore friction for exactly 1 tick (also disables Ground movement)
            _ignoreFrictionTimer = Time.fixedDeltaTime + 0.01f;
        }

        private void CheckCrouch()
        {
            if (moveType == MoveType.Noclip)
                return;

            if (!_requestedCrouch && _isCrouching)
            {
                Uncrouch();
            }
            else if (_requestedCrouch && !_isCrouching)
            {
                Crouch();
            }
        }

        private void Crouch()
        {
            float crouchDistance = Mathf.Abs(GlobalVariables.playerHeight - GlobalVariables.crouchHeight);
            float crouchHalf = GlobalVariables.crouchHeight * 0.5f;

            motor.SetCapsuleDimensions(motor.Capsule.radius, motor.Capsule.height - crouchDistance, crouchHalf);

            if (!motor.GroundingStatus.IsStableOnGround)
            {
                motor.SetPosition(motor.InitialTickPosition + new Vector3(0, crouchHalf, 0));
            }

            _isCrouching = true;
        }

        private void Uncrouch()
        {
            float crouchDistance = Mathf.Abs(GlobalVariables.playerHeight - GlobalVariables.crouchHeight);
            float standingCenter = GlobalVariables.playerHeight * 0.5f;

            if (CanGoUp(crouchDistance) == false)
                return;

            motor.SetCapsuleDimensions(motor.Capsule.radius, motor.Capsule.height + crouchDistance, standingCenter);

            if (!motor.GroundingStatus.IsStableOnGround)
            {
                if (FloorCast(crouchDistance, out var toGround) && toGround.collider != null)
                {
                    motor.SetPosition(new Vector3(motor.InitialTickPosition.x, toGround.point.y, motor.InitialTickPosition.z));
                }
                else
                {
                    motor.SetPosition(motor.InitialTickPosition - new Vector3(0, standingCenter * 0.5f, 0));
                }
            }

            _isCrouching = false;
        }

        private void LadderMovement(ref Vector3 currentVelocity)
        {
            moveType = MoveType.Ladder;

            currentVelocity.y = 0f;

            if (_requestedJump)
            {
                _isOnLadder = false;
                currentVelocity = _ladderNormal * GlobalVariables.climbingSpeed;
                _needToReleaseJump = true;
            }
            else
            {
                Vector3 moveDirection = _requestedRotation * _requestedMovement;

                Vector3 moveSpeed = moveDirection.normalized * GlobalVariables.climbingSpeed;

                if (moveSpeed.magnitude != 0)
                {
                    Vector3 ladderRight = Vector3.Cross(Vector3.up, _ladderNormal).normalized;
                    float normalProjection = Vector3.Dot(moveSpeed, _ladderNormal);
                    Vector3 normalComponent = _ladderNormal * normalProjection;
                    Vector3 tangentialVelocity = moveSpeed - normalComponent;

                    currentVelocity = tangentialVelocity + -normalProjection * Vector3.Cross(_ladderNormal, ladderRight);

                    if (motor.GroundingStatus.IsStableOnGround && normalProjection > 0)
                    {
                        currentVelocity += GlobalVariables.climbingSpeed * _ladderNormal;
                    }
                }
                else
                    currentVelocity = Vector3.zero;
            }
        }

        private void NoclipMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 moveDirection = _requestedRotation * _requestedMovement;

            currentVelocity += GroundAccelerate(currentVelocity, moveDirection, GlobalVariables.noclipSpeed, _currentAcceleration, deltaTime);

            if (_requestedJump)
                currentVelocity += GroundAccelerate(currentVelocity, transform.up, GlobalVariables.noclipSpeed, _currentAcceleration, deltaTime);

            if (_requestedCrouch)
                currentVelocity += GroundAccelerate(currentVelocity, -transform.up, GlobalVariables.noclipSpeed, _currentAcceleration, deltaTime);

            ApplyFriction(ref currentVelocity, deltaTime);
        }

        private void WaterMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 wishvel = _requestedRotation * _requestedMovement;
            wishvel.Normalize();
            wishvel *= GlobalVariables.waterSpeed;

            if (_requestedJump)
            {
                currentVelocity.y = GlobalVariables.waterJumpPower;
                wishvel.y += GlobalVariables.maxSpeed;
            }
            else if (_requestedMovement.magnitude < 0.1f)
            {
                wishvel.y -= GlobalVariables.sinkSpeed;
            }

            // Jump out, ledge check
            if (_waterDepth <= GlobalVariables.waterLevelToJumpOut && _requestedJump)
            {
                Collider[] ledgeCheck = new Collider[8];
                int collided = Physics.OverlapSphereNonAlloc(motor.InitialTickPosition + motor.CharacterUp * motor.Capsule.height * 0.5f, motor.Capsule.radius + 0.1f, ledgeCheck, motor.CollidableLayers, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < collided; i++)
                {
                    if (ledgeCheck[i] == null)
                        continue;

                    if (ledgeCheck[i] == motor.Capsule)
                        continue;

                    currentVelocity.y = GlobalVariables.waterJumpOutPower;
                    _needToReleaseJump = true;
                    break;
                }
            }

            Vector3 wishdir = wishvel;
            float wishspeed = wishdir.magnitude;

            if (wishspeed >= GlobalVariables.maxSpeed)
            {
                wishvel *= GlobalVariables.maxSpeed / wishspeed;
                wishspeed = GlobalVariables.maxSpeed;
            }

            wishspeed *= 0.8f;
            float speed = currentVelocity.magnitude;

            // Water friction
            float newspeed;
            if (speed > 0)
            {
                newspeed = speed - deltaTime * GlobalVariables.waterFriction;
                if (newspeed < 0.1f)
                {
                    newspeed = 0;
                }
                currentVelocity *= newspeed / speed;
            }
            else
            {
                newspeed = 0;
            }

            // Water acceleration
            if (wishspeed > 0.1f)
            {
                float addspeed = wishspeed - newspeed;
                if (addspeed > 0)
                {
                    wishvel.Normalize();
                    float accelspeed = GlobalVariables.acceleration * wishspeed * deltaTime;
                    if (accelspeed > addspeed)
                    {
                        accelspeed = addspeed;
                    }

                    currentVelocity += accelspeed * wishvel;
                }
            }
        }

        private Vector3 GroundAccelerate(Vector3 wishvel, Vector3 wishdir, float wishspeed, float accel, float timeDelta)
        {
            float curspeed = Vector3.Dot(wishvel, wishdir);

            float addspeed = wishspeed - curspeed;

            return Accelerate(wishdir, addspeed, accel, wishspeed, timeDelta);
        }

        private Vector3 AirAccelerate(Vector3 wishvel, Vector3 wishdir, float wishspeed, float accel, float speedCap, float timeDelta)
        {
            if (wishspeed > speedCap)
                wishspeed = speedCap;

            float currentspeed = Vector3.Dot(wishvel, wishdir);

            float addspeed = wishspeed - currentspeed;

            return Accelerate(wishdir, addspeed, accel, wishspeed, timeDelta);
        }

        private Vector3 Accelerate(Vector3 wishdir, float addspeed, float accel, float wishspeed, float timeDelta)
        {
            if (addspeed <= 0)
                return Vector3.zero;

            float accelspeed = accel * wishspeed * timeDelta;

            if (accelspeed > addspeed)
                accelspeed = addspeed;

            return wishdir * accelspeed;
        }

        public bool FloorCast(float distance, out RaycastHit hit)
        {
            Vector3 origin = motor.InitialTickPosition + motor.CharacterUp * motor.Capsule.radius;

            if (Physics.SphereCast(origin, motor.Capsule.radius, -motor.CharacterUp,
                out hit, distance, motor.CollidableLayers, QueryTriggerInteraction.Ignore))
            {
                return hit.collider.enabled;
            }

            return false;
        }

        private bool CanGoUp(float height)
        {
            float topOffset = motor.Capsule.height - motor.Capsule.radius;
            Vector3 capsuleTop = motor.InitialTickPosition + motor.CharacterUp * topOffset;

            height = Mathf.Max(0f, height - 0.1f);

            return !Physics.SphereCast(capsuleTop, motor.Capsule.radius,
                    motor.CharacterUp, out RaycastHit _, height + 0.1f, motor.CollidableLayers, QueryTriggerInteraction.Ignore);
        }

        private void ApplyFriction(ref Vector3 currentVelocity, float deltaTime)
        {
            float friction = 0;

            switch (moveType)
            {
                case MoveType.Noclip:
                    friction = GlobalVariables.noclipFriction;
                    break;

                case MoveType.Walk:
                    friction = GlobalVariables.groundFriction;
                    break;
            }

            if (_requestedMovement.magnitude <= 0.1f)
                friction *= 1.5f;

            currentVelocity -= currentVelocity * (friction * deltaTime);
        }

        public void AddImpulse(Vector3 direction, float power)
        {
            motor.ForceUnground(0f);
            motor.BaseVelocity += direction * power;
        }

        public void SetVelocity(Vector3 velocity)
        {
            motor.ForceUnground(0f);
            motor.BaseVelocity = velocity;
        }

        public void SetPosition(Vector3 position, bool teleport)
        {
            if (teleport)
                motor.SetPosition(position);
            else
                motor.MoveCharacter(position);
        }

        public void SetGravityModifier(float gravityModifier)
        {
            // In case when gravity force pulling player up, but he is on ground
            motor.ForceUnground(0f);
            _gravityModifier = gravityModifier;
        }

        public void SetMovementSpeedModifier(float speedModifier) => _movementSpeedModifier = speedModifier;

        public Vector3 GetPosition() => motor.InitialTickPosition;

        public Vector3 GetVelocity() => motor.Velocity;

        public void AddImpulse(Vector3 position, Vector3 direction, float power)
        {

        }

        public void SetRotation(Vector3 eulerAngles)
        {
            //headRotator.SetEulerAngles(eulerAngles);
        }
    }
}
