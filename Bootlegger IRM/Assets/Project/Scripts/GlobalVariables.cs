using FirstPersonView;
using UnityEngine;

namespace Bootlegger
{
    public static class GlobalVariables
    {
        public static float gravity = 20f;
        public static float slidingGravity = 40f;
        public static float maxSpeed = 12.0f;

        public static float waterSpeed = 10.0f;
        public static float sinkSpeed = 3f;
        public static float waterFriction = 12.0f;
        public static float waterLevelToSwim = 0.65f;
        public static float waterLevelToJumpOut = 0.8f;
        public static float waterJumpOutPower = 7.0f;
        public static float waterJumpPower = 3.0f;

        // Noclip
        public static float noclipSpeed = 30.0f;
        public static float noclipFriction = 6.0f;

        // Ladder
        public static float climbingSpeed = 6.0f;

        // Crouch
        public static float duckMultiplier = 0.20f;
        public static float checkRadius = 0.45f;
        public static float checkHeight = 0.65f;
        public static float playerHeight = 2.0f;
        public static float crouchHeight = 1.1f;
        public static float crouchTransitionSpeed = 4.0f;

        // FOV
        public static float minFovValue = 45f;
        public static float maxFov = 120;
        public static float maxViewModelFov = 75;

        // Jumping
        public static float jumpForce = 1.0f;
        public static int coyoteTimeInTicks = 10;
        public static int jumpCooldownInTicks = 5;

        // Other values
        public static float airSpeedCap = 0.8f;
        public static float slopeLimit = 65.0f;
        public static float groundFriction = 6f;
        public static float maxGroundSpeed = 7.5f;
        public static float airSpeed = 9.0f;
        public static float acceleration = 5.0f;
        public static float airAcceleration = 60.0f;
        public static float shiftMultiplier = 1.4f;

        //public static void SetViewmodelFOV(int fov)
        //{
        //    fov = Mathf.Clamp(fov, 30, 120);
        //
        //    SettingsManager.GameSettings.ViewModelFOV = fov;
        //    FPV.firstPersonCamera.GetCamera().fieldOfView = SettingsManager.GameSettings.ViewModelFOV;
        //}
        //
        //public static void SetFOV(int fov)
        //{
        //    fov = Mathf.Clamp(fov, 30, 120);
        //
        //    SettingsManager.GameSettings.FOV = fov;
        //    FPV.mainCamera.GetCamera().fieldOfView = SettingsManager.GameSettings.FOV;
        //}

        public static void SetMaxFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
    }
}
