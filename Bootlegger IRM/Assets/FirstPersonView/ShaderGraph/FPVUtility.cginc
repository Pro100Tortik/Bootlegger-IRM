//UNITY_SHADER_NO_UPGRADE

#ifndef FPV_UTILITIES_INCLUDED
#define FPV_UTILITIES_INCLUDED

inline float3 ConvertPositionToFPV(float3 Position, float FpvFieldOfView, float FpvScale)
{
  float FovProj = 1.0 / (-1.0 * tan((FpvFieldOfView / 360.0) * 3.14159265359f) * UNITY_MATRIX_P[1][1]);

  float3 Pos_View = mul(mul(UNITY_MATRIX_V, UNITY_MATRIX_M), float4(Position, 1.0)).xyz;
  Pos_View.x *= FovProj;
  Pos_View.y *= FovProj;

  Pos_View.x *= -_ProjectionParams.x;
  Pos_View.y *= -_ProjectionParams.x;

  Pos_View *= FpvScale;
  Pos_View = mul(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V), float4(Pos_View, 1.0)).xyz;

  return Pos_View;
}

inline float3 FPV_ConvertPositionToFPV(float3 Position, float FpvFieldOfView, float FpvScale) {
#if defined(BOOLEAN_FIRSTPERSONVIEW_ON) || defined(BOOLEAN_FPV_ALWAYS_ON)
  return ConvertPositionToFPV(Position, FpvFieldOfView, FpvScale);
#else
  return Position;
#endif
}

#endif
