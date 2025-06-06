// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/CubeBlend"
{
Properties
{
    [NoScaleOffset] _TexA ("Cubemap", Cube) = "grey" {}
    [NoScaleOffset] _TexB ("Cubemap", Cube) = "grey" {}
    [NoScaleOffset] _TexC ("Cubemap", Cube) = "grey" {}
    [NoScaleOffset] _TexD ("Cubemap", Cube) = "grey" {}
    _value ("1st_Value", Range (0, 1)) = 0.5
    _value2 ("2nd_Value", Range (0, 1)) = 0.5
    _value3 ("3rd_Value", Range (0, 1)) = 0.5
}

CGINCLUDE

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

half4 _TexA_HDR;
half4 _TexB_HDR;
half4 _TexC_HDR;
half4 _TexD_HDR;

UNITY_DECLARE_TEXCUBE(_TexA);
UNITY_DECLARE_TEXCUBE(_TexB);
UNITY_DECLARE_TEXCUBE(_TexC);
UNITY_DECLARE_TEXCUBE(_TexD);

float _Level;
float _value;
float _value2;
float _value3;

struct appdata_t {
    float4 vertex : POSITION;
    float3 texcoord : TEXCOORD0;
};

struct v2f {
    float4 vertex : SV_POSITION;
    float3 texcoord : TEXCOORD0;
};

v2f vert (appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.texcoord = v.texcoord;
    return o;
}

half4 frag (v2f i) : SV_Target
{
    half3 texA = DecodeHDR (UNITY_SAMPLE_TEXCUBE_LOD (_TexA, i.texcoord, _Level), _TexA_HDR);
    half3 texB = DecodeHDR (UNITY_SAMPLE_TEXCUBE_LOD (_TexB, i.texcoord, _Level), _TexB_HDR);
    half3 texC = DecodeHDR (UNITY_SAMPLE_TEXCUBE_LOD (_TexC, i.texcoord, _Level), _TexC_HDR);
    half3 texD = DecodeHDR (UNITY_SAMPLE_TEXCUBE_LOD (_TexD, i.texcoord, _Level), _TexD_HDR);

    // Blend TexA and TexB based on 1st_Value
    half3 texAB = lerp(texA, texB, _value);
    
    // If 2nd_Value is 1, prioritize TexC, otherwise blend with TexAB
    half3 texABC = lerp(texAB, texC, _value2);
    
    // If 3rd_Value is 1, prioritize TexD, otherwise blend with texABC
    half3 res = lerp(texABC, texD, _value3);
    
    return half4(res, 1.0);
}
ENDCG

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" }
    Cull back ZWrite Off Fog { Mode Off }
    Pass
    {
        CGPROGRAM
        #pragma target 3.0
        ENDCG
    }
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" }
    Cull back ZWrite Off Fog { Mode Off }

    Pass
    {
        CGPROGRAM
        #pragma target 2.0
        ENDCG
    }
}

Fallback Off

}
