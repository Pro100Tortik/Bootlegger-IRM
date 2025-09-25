using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace FirstPersonView
{
    /// <summary>
    /// Interface for all Camera components of FirstPersonView
    /// </summary>
    public interface IFPV_Camera
    {
		bool AreShadowsOverriden();

		ShadowCastingMode GetNormalShadowCastingMode();
		ShadowCastingMode GetFpvShadowCastingMode();

		/// <summary>
		/// Setup every needed variable on this component
		/// </summary>
		void Setup();

        /// <summary>
        /// Manualy update the static camera variable of this component.
        /// </summary>
        void UpdateStaticCamera();

        /// <summary>
        /// Get the camera component of this camera.
        /// </summary>
        /// <returns></returns>
        Camera GetCamera();

        /// <summary>
        /// Get the projection matrix of this camera
        /// </summary>
        /// <returns></returns>
        Matrix4x4 GetProjectionMatrix();

		void SetFpvAlwaysOn(bool IsOn);

		IEnumerator TransitionFirstPersonView(float seconds, bool toFirstPerson, bool translateFov, bool translateDepth);

		void SetFirstPersonViewPercentage(float percentage);
		void SetFirstPersonViewFovPercentage(float percentage);
		void SetFirstPersonViewDepthPercentage(float percentage);

		float GetFpvDepthMultiplier();
	}
}
