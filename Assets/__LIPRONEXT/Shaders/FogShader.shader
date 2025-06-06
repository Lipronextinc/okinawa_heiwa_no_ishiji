Shader"Custom/SmoothFogByDistance"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogStart ("Fog Start Distance", Float) = 10
        _FogRange ("Fog Range Distance", Float) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 pos : SV_POSITION;
    float3 worldPos : TEXCOORD1;
};

sampler2D _MainTex;
float4 _FogColor;
float _FogStart;
float _FogRange;
float3 _PlayerPos;

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float distanceToPlayer = distance(i.worldPos, _PlayerPos);
                
                // smoothstep で滑らかにフェード
    float fogFactor = smoothstep(_FogStart, _FogStart + _FogRange, distanceToPlayer);

    float4 texColor = tex2D(_MainTex, i.uv);
    return lerp(texColor, _FogColor, fogFactor);
}
            ENDCG
        }
    }
}
