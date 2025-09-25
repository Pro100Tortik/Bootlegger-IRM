//UNITY_SHADER_NO_UPGRADE

#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

#include "Assets/FirstPersonView/ShaderGraph/FPVUtility.cginc"

void FpvNode_float(float3 Position, float FpvFieldOfView, float FpvScale, out float3 OutPosition) {

  OutPosition = FPV_ConvertPositionToFPV(Position, FpvFieldOfView, FpvScale);
}

#endif// MYHLSLINCLUDE_INCLUDED