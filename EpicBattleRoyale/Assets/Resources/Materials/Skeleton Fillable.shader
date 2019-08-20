// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spine/SkeletonFill" {
    Properties {
        _FillColor ("FillColor", Color) = (1,1,1,1)
        _FillAlpha ("FillAlpha", Range(0, 1)) = 0
        [NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Blend One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex;
            uniform float4 _FillColor;
            uniform float _FillAlpha;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (VertexOutput i) : COLOR {
                float4 rawColor = tex2D(_MainTex,i.uv0);
                float finalAlpha = (rawColor.a * i.vertexColor.a * _FillColor.a);

                float3 finalColor = lerp((rawColor.rgb * i.vertexColor.rgb * finalAlpha), (_FillColor.rgb * finalAlpha), _FillAlpha);
	            return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}