using UnityEngine;
using UnityEngine.Rendering;

namespace FirstPersonView
{
	public class FPV_LightCommandBuffer : MonoBehaviour
	{
		[Header("Only use this script when using Forward Rendering")]
		[SerializeField]
		private Light _light;

		private CommandBuffer _commandBufferBefore;
		private CommandBuffer _commandBufferAfter;

		void Start() {
			_commandBufferBefore = new CommandBuffer();
			_commandBufferAfter = new CommandBuffer();

			_commandBufferBefore.EnableShaderKeyword("FPV_LIGHT");
			_commandBufferAfter.DisableShaderKeyword("FPV_LIGHT");

			_light.AddCommandBuffer(LightEvent.BeforeShadowMapPass, _commandBufferBefore);
			_light.AddCommandBuffer(LightEvent.AfterShadowMapPass, _commandBufferAfter);
		}
	}
}