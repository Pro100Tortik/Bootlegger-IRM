
//UNITY_SHADER_NO_UPGRADE

#ifndef FPV_DEPTHNORMALSTEXTURE_OVERRIDE_INCLUDED
#define FPV_DEPTHNORMALSTEXTURE_OVERRIDE_INCLUDED

struct v2f {
    float4 pos : SV_POSITION;
    float4 nz : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

float4x4 _firstPersonProjectionMatrixCustom;

v2f vert( appdata_base v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.pos = mul(mul(_firstPersonProjectionMatrixCustom, UNITY_MATRIX_V), mul(unity_ObjectToWorld, v.vertex));
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
fixed4 frag(v2f i) : SV_Target {
    return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}

#endif
