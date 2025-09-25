using UnityEngine;

namespace FirstPersonView
{
    /// <summary>
    /// Component for the First Person Camera in this FPV Shader-Material Setup
    /// </summary>
    [AddComponentMenu("FPV/FPV Camera First Person")]
    public class FPV_Camera_FirstPerson : FPV_CameraBase
    {
		/// <summary>
		/// Manualy update the static first person view camera variable.
		/// </summary>
		public override void UpdateStaticCamera()
        {
            FPV.firstPersonCamera = this;
        }
    }
}