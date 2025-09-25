
//UNITY_SHADER_NO_UPGRADE

#ifndef FPV_OVERRIDE_INCLUDED
#define FPV_OVERRIDE_INCLUDED

float4x4 _firstPersonProjectionMatrix;

#if defined(BOOLEAN_FIRSTPERSONVIEW_ON) || defined(BOOLEAN_FPV_ALWAYS_ON)
  #ifdef FPV_LIGHT // Used to enable/disable the projection matrix used during shadows calculations
    #define UNITY_MATRIX_P glstate_matrix_projection

  #else
    #define UNITY_MATRIX_P _firstPersonProjectionMatrix
  #endif

  #define UNITY_MATRIX_VP mul(UNITY_MATRIX_P, UNITY_MATRIX_V)
  #define UNITY_MATRIX_MVP mul(UNITY_MATRIX_VP, unity_ObjectToWorld)
#endif

#endif
