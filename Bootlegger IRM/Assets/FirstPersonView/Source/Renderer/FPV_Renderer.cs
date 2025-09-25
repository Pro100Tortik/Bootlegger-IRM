using UnityEngine;
using UnityEngine.Rendering;

namespace FirstPersonView
{
    [AddComponentMenu("FPV/FPV Renderer")]
    public class FPV_Renderer : FPV_RendererBase
    {
        public FPV_Material fpvMaterial;

		public bool overrideShadows = false;
		public ShadowCastingMode normalShadowCastingMode = ShadowCastingMode.On;
		public ShadowCastingMode fpvShadowCastingMode = ShadowCastingMode.Off;

		public bool overrideMotionVectors = false;
		public MotionVectorGenerationMode normalMotionVectors = MotionVectorGenerationMode.Object;
		public MotionVectorGenerationMode fpvMotionVectors = MotionVectorGenerationMode.Object;

		/// <summary>
		/// Setup the renderer
		/// </summary>
		/// <param name="render"></param>
		public override void Setup(IFPV_Object parent, Renderer render)
        {
            base.Setup(parent, render);

            InitialteFPVMaterials();
		}

        /// <summary>
        /// Initiate the Material of this renderer
        /// </summary>
        private void InitialteFPVMaterials()
        {
            if (fpvMaterial == null) fpvMaterial = new FPV_Material();

            fpvMaterial.OriginalMaterial = _render.sharedMaterial; //Store the original material

            if(fpvMaterial.FirstPersonViewMaterial == null) //Instance a new material of the original used if none is assigned to the firstpersonmaterial
            {
                fpvMaterial.FirstPersonViewMaterial = _render.material; //Create instance.

                //Reset the original material
                _render.sharedMaterial = fpvMaterial.OriginalMaterial;
            }

            //Enable the Keyword FIRSTPERSONVIEW for this material
            fpvMaterial.FirstPersonViewMaterial.EnableKeyword("BOOLEAN_FIRSTPERSONVIEW_ON");
        }

        /// <summary>
        /// Set this renderer as First Person Object
        /// </summary>
        public override void SetAsFirstPersonObject()
        {
			if(FPV.mainCamera.AreShadowsOverriden())
			{
				_render.shadowCastingMode = FPV.mainCamera.GetFpvShadowCastingMode();
			}
			else if(overrideShadows)
			{
				_render.shadowCastingMode = fpvShadowCastingMode;
			}

			if(overrideMotionVectors)
			{
				_render.motionVectorGenerationMode = fpvMotionVectors;
			}

            _render.sharedMaterial = fpvMaterial.FirstPersonViewMaterial;
		}

        /// <summary>
        /// Set this renderer as First Person Object
        /// </summary>
        public override void RemoveAsFirstPersonObject()
		{
			if (FPV.mainCamera.AreShadowsOverriden())
			{
				_render.shadowCastingMode = FPV.mainCamera.GetNormalShadowCastingMode();
			}
			else if (overrideShadows)
			{
				_render.shadowCastingMode = normalShadowCastingMode;
			}

			if (overrideMotionVectors)
			{
				_render.motionVectorGenerationMode = normalMotionVectors;
			}

			_render.sharedMaterial = fpvMaterial.OriginalMaterial;
		}
    }
}
