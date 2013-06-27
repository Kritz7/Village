Shader "New Shader" {
Properties {
_MainTex ("Base (RGB)", 2D) = "white
" {}
}
_Color ("Main Color", Color) = (1,1,1,0.5)
SubShader {
Pass {
CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma vertex vert
#pragma fra
gment frag
#include "UnityCG.cginc"
struct vIn {
float4 vertex:POSITION;
float3 normal:NORMAL;
float2 texcoord:TEXCOORD0;
};
struct v2f {
float4 pos:POSITION;
float2 uv:TEXCOORD0;
};
uniform float4 _Color;
uniform sampler2D _MainTex;
v2f
vert (vIn v)
{
v2f o;
o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
o.uv = v.texcoord;
return o;
}
float4 frag (v2f i) : COLOR
{
float4 c = tex2D( _MainTex, i.uv );
return c;
}
ENDCG
}
}

FallBack "Diffuse"
}
