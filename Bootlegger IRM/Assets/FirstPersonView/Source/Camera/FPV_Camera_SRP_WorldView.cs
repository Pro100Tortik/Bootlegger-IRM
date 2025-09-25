using UnityEngine;
using UnityEngine.Rendering;

namespace FirstPersonView
{
	/// <summary>
	/// Component for the World Camera in this FPV Shader-Material Setup
	/// </summary>
	[AddComponentMenu("FPV/HDRP_URP/FPV SRP World Camera")]
	public class FPV_Camera_SRP_WorldView : FPV_CameraBase
	{
		[Header("FPV Shader Override")]
		public bool FpvShadersAlwaysOn = false;

		public float FpvScale = 0.45f;

		private float _lastFieldOfView = -1.0f;
		private float _lastScale = -1.0f;
		
		public override void Setup() {
			base.Setup();
			Shader.SetGlobalFloat("_FPV_Scale", FpvScale);

			SetFpvAlwaysOn(FpvShadersAlwaysOn);
		}
		/// <summary>
		/// Manualy update the static first person view camera variable.
		/// </summary>
		public override void UpdateStaticCamera() {
			FPV.mainCamera = this;
		}

		public override float GetFpvDepthMultiplier()
		{
			return Mathf.Lerp(1.0f, FpvScale, _fpvDepthPercentage);
		}

		void LateUpdate() {
			UpdateFieldOfView(false);
		}

		protected override void UpdateFieldOfView(bool forceUpdate) {
			float currentFieldOfView = Mathf.Lerp(_camera.fieldOfView, FPV.firstPersonCamera.GetCamera().fieldOfView, _fpvFovPercentage);
			float currentScale = Mathf.Lerp(1.0f, FpvScale, _fpvDepthPercentage);

			if ((currentFieldOfView != _lastFieldOfView) || (currentScale != _lastScale) || forceUpdate) {
				_lastFieldOfView = currentFieldOfView;
				_lastScale = currentScale;
				Shader.SetGlobalFloat("_FPV_FieldOfView", currentFieldOfView);
				Shader.SetGlobalFloat("_FPV_Scale", currentScale);
			}
		}
	}
}