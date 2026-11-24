Shader "JustSmash/ArenaText" {
Properties { _MainTex ("Font atlas", 2D) = "white" {} }
SubShader {
Tags { "Queue"="Transparent" "RenderType"="Transparent" }
Blend SrcAlpha OneMinusSrcAlpha
Cull Off ZWrite Off ZTest LEqual
Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _MainTex;
struct data { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
data vert(appdata_full v) {
 data o; o.pos=UnityObjectToClipPos(v.vertex); o.uv=v.texcoord.xy; o.color=v.color; return o;
}
fixed4 frag(data i):SV_Target { return fixed4(i.color.rgb, i.color.a * tex2D(_MainTex,i.uv).a); }
ENDCG
}
}
}
