using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace FirstPersonView
{
    /// <summary>
    /// Abstract class to the Camera component of FPV.
    /// </summary>
    public abstract class FPV_CameraBase : MonoBehaviour, IFPV_Camera
	{
		[Header("FPV Shadow Casting Override")]
		public bool overrideShadowCastingMode = false;

		public ShadowCastingMode normalShadowCastingMode = ShadowCastingMode.On;
		public ShadowCastingMode fpvShadowCastingMode = ShadowCastingMode.Off;

		/// <summary>
		/// Reference to the camera component of this gameobject
		/// </summary>
		protected Camera _camera;

		protected float _fpvFovPercentage = 1.0f;
		protected float _fpvDepthPercentage = 1.0f;

		void Awake()
        {
            Setup();
        }

        /// <summary>
        /// Setup every needed variable on this component
        /// </summary>
        public virtual void Setup()
        {
            _camera = GetComponent<Camera>();
            UpdateStaticCamera();
        }

        /// <summary>
        /// Manualy update the static camera variable of this component.
        /// </summary>
        public abstract void UpdateStaticCamera();

        /// <summary>
        /// Get the camera component of this camera.
        /// </summary>
        /// <returns></returns>
        public Camera GetCamera()
        {
            return _camera;
        }

        /// <summary>
        /// Get the current projection matrix of this camera.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetProjectionMatrix()
        {
            return _camera.projectionMatrix;
        }

		public void SetFpvAlwaysOn(bool IsOn)
		{
			if (IsOn)
			{
				Shader.EnableKeyword("BOOLEAN_FPV_ALWAYS_ON");
			}
			else
			{
				Shader.DisableKeyword("BOOLEAN_FPV_ALWAYS_ON");
			}
		}

		/// <summary>
		/// For Default Renderer, only translateFov will work.
		/// For HDRP and URP, both translateFov and translateDepth will work.
		/// </summary>
		public IEnumerator TransitionFirstPersonView(float seconds, bool toFirstPerson, bool translateFov, bool translateDepth)
		{
			float startTime = Time.unscaledTime;
			float endTime = startTime + seconds;

			while(true)
			{
				float currentTime = Time.unscaledTime;
				float percentage = (currentTime - startTime) / (endTime - startTime);
				float targetPercentage = toFirstPerson ? Mathf.Clamp01(percentage) : 1.0f - Mathf.Clamp01(percentage);
				
				if(translateFov){ SetFirstPersonViewFovPercentage(targetPercentage); }
				if(translateDepth){ SetFirstPersonViewDepthPercentage(targetPercentage); }

				if (percentage > 1.0f){ yield break; }

				yield return null;
			}
		}

		public void SetFirstPersonViewPercentage(float percentage)
		{
			SetFirstPersonViewFovPercentage(percentage);
			SetFirstPersonViewDepthPercentage(percentage);
		}
		public virtual void SetFirstPersonViewFovPercentage(float percentage)
		{
			_fpvFovPercentage = percentage;
		}
		public virtual void SetFirstPersonViewDepthPercentage(float percentage)
		{
			_fpvDepthPercentage = percentage;
		}

		public virtual float GetFpvDepthMultiplier()
		{
			return 1.0f;
		}

		protected virtual void UpdateFieldOfView(bool forceUpdate) { }

		public bool AreShadowsOverriden()
		{
			return overrideShadowCastingMode;
		}

		public ShadowCastingMode GetNormalShadowCastingMode()
		{
			return normalShadowCastingMode;
		}

		public ShadowCastingMode GetFpvShadowCastingMode()
		{
			return fpvShadowCastingMode;
		}
	}
}
