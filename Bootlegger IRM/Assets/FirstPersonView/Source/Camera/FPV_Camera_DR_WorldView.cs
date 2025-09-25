using UnityEngine;
using UnityEngine.Rendering;

namespace FirstPersonView
{
    /// <summary>
    /// Component for the World Camera in this FPV Shader-Material Setup
    /// </summary>
    [AddComponentMenu("FPV/Default Renderer/FPV Default Renderer Camera World")]
    public class FPV_Camera_DR_WorldView : FPV_CameraBase
	{
		public bool castCorrectFpvShadows;

		[Header("FPV Shader Override")]
		public bool FpvShadersAlwaysOn = false;

        private bool d3d = true;
        private bool usesReversedZBuffer = false;

        private CommandBuffer _commandBufferBefore;
        private CommandBuffer _commandBufferAfter;

		public override void Setup() {
            base.Setup();

            usesReversedZBuffer = SystemInfo.usesReversedZBuffer;

			d3d = !(SystemInfo.graphicsDeviceVersion.IndexOf("OpenGL") > -1);

            if (castCorrectFpvShadows) {
                PrepareLightingCommandBuffer();
            }

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
			return 0.3f;
		}

		void OnPreCull() {
            //Before rendering the scene, update the global first person projection on the shaders.
            Matrix4x4 fpvProjection = FPV.firstPersonCamera.GetProjectionMatrix();
			Matrix4x4 fpvProjectionCustom = fpvProjection;

			Matrix4x4 worldProjection = GetProjectionMatrix();

			fpvProjection.m00 = Mathf.Lerp(worldProjection.m00, fpvProjection.m00, _fpvFovPercentage);
			fpvProjection.m11 = Mathf.Lerp(worldProjection.m11, fpvProjection.m11, _fpvFovPercentage);

			//DIRECTX
			if (d3d) {
                fpvProjection.m11 *= -1; // Invert Y value of the MVP vertex position
                fpvProjectionCustom.m11 *= -1; // Invert Y value of the MVP vertex position

                if (usesReversedZBuffer) { //Inverted Z-Buffer
                    fpvProjection.m22 *= 0.3f; // Shorten the Z value of the MVP vertex position
                    fpvProjection.m23 *= -1;
                    fpvProjectionCustom.m23 *= -1;
                }
                else { //Z-Buffer not inverted
                    fpvProjection.m22 *= 0.7f; // Shorten the Z value of the MVP vertex position
                }
            }
            //OPEN GL
            else {
                if (usesReversedZBuffer) { //Inverted Z-Buffer
                    fpvProjection.m22 *= 0.3f; // Shorten the Z value of the MVP vertex position
                    fpvProjection.m23 *= -1;
                    fpvProjectionCustom.m23 *= -1;
                }
                else { //Z-Buffer not inverted
                    fpvProjection.m22 = fpvProjection.m22 * 0.7f + 0.3f; // Shorten the Z value of the MVP vertex position
                }
            }
			
			//Update the global shader variable
			Shader.SetGlobalMatrix("_firstPersonProjectionMatrix", fpvProjection);
			Shader.SetGlobalMatrix("_firstPersonProjectionMatrixCustom", fpvProjection);
		}

		private void PrepareLightingCommandBuffer() {
            _commandBufferBefore = new CommandBuffer();
            _commandBufferAfter = new CommandBuffer();

            _commandBufferBefore.EnableShaderKeyword("FPV_LIGHT");
            _commandBufferAfter.DisableShaderKeyword("FPV_LIGHT");

            _camera.AddCommandBuffer(CameraEvent.BeforeLighting, _commandBufferBefore);
            _camera.AddCommandBuffer(CameraEvent.AfterLighting, _commandBufferAfter);
        }
    }
}