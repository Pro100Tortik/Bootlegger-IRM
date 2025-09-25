using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonView
{
    /// <summary>
    /// Static Class for the First Person View.
    /// </summary>
    public static class FPV
    {
        /// <summary>
        /// Reference to the Main Camera that renders the environment
        /// </summary>
        public static IFPV_Camera mainCamera { get; set; }

        /// <summary>
        /// Reference to the First Person Camera.
        /// </summary>
        public static IFPV_Camera firstPersonCamera { get; set; }
        
        /// <summary>
        /// Convert a First Person View point to World View point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 FPVPointToWorldPoint(Vector3 point)
        {
            point = firstPersonCamera.GetCamera().WorldToScreenPoint(point);
			point.z *= mainCamera.GetFpvDepthMultiplier();
            return mainCamera.GetCamera().ScreenToWorldPoint(point);
        }

        /// <summary>
        /// Transform a point and a direction from First Person View to World View
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="resPoint"></param>
        /// <param name="resDirection"></param>
        public static void FPVToWorld(Vector3 point, Vector3 direction, out Vector3 resPoint, out Vector3 resDirection)
        {
            resPoint = FPVPointToWorldPoint(point);

            Vector3 pointForward = point + direction;
            pointForward = FPVPointToWorldPoint(pointForward);
            resDirection = pointForward - resPoint;
        }

        /// <summary>
        /// Transform a point and a direction based on a Transform from First Person View to World View
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="resPoint"></param>
        /// <param name="resDirection"></param>
        public static void FPVToWorld(Transform trans, out Vector3 resPoint, out Vector3 resDirection)
        {
            FPVToWorld(trans.position, trans.forward, out resPoint, out resDirection);
        }
    }
}