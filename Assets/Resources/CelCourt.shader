Shader "JustSmash/CelCourt" {
Properties { _MainTex ("Texture", 2D) = "white" {} _Color ("Color", Color) = (1,1,1,1) _Flat ("Flat", Float) = 0 _Outline ("Outline", Float) = 0 _Unlit ("Unlit", Range(0,1)) = 0 }
SubShader {
Tags { "RenderType"="Opaque" }
Pass {
Cull Front
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
float _Outline;
float4 vert(appdata_base v) : SV_POSITION {
 float3 p=mul(unity_ObjectToWorld,v.vertex).xyz + UnityObjectToWorldNormal(v.normal)*_Outline;
 return mul(UNITY_MATRIX_VP,float4(p,1));
}
fixed4 frag() : SV_Target { return fixed4(.025,.06,.14,1); }
ENDCG
}
Pass {
Tags { "LightMode"="ForwardBase" }
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
sampler2D _MainTex; float4 _MainTex_ST; fixed4 _Color; float _Flat, _Unlit;
struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float3 normal:TEXCOORD1; float3 world:TEXCOORD2; SHADOW_COORDS(3) };
v2f vert(appdata_base v) {
 v2f o; o.pos=UnityObjectToClipPos(v.vertex); o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);
 o.normal=UnityObjectToWorldNormal(v.normal); o.world=mul(unity_ObjectToWorld,v.vertex).xyz; TRANSFER_SHADOW(o); return o;
}
fixed4 frag(v2f i):SV_Target {
 UNITY_LIGHT_ATTENUATION(atten,i,i.world);
 float light=saturate(dot(normalize(i.normal),normalize(_WorldSpaceLightPos0.xyz)))*atten;
 float shade=light>.62?1:light>.18?.78:.54;
 float3 albedo=lerp(tex2D(_MainTex,i.uv).rgb,1,saturate(_Flat))*_Color.rgb;
 return float4(albedo*lerp(shade,1,saturate(_Unlit)),1);
}
ENDCG
}
UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
}
Fallback "Diffuse"
}
