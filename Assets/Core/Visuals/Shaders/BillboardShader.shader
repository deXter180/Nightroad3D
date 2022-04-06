Shader "Billboard"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+500" }
        LOD 100

        Pass
        {
                ZTest Off
                //Cull Off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)


            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.


                float4 worldOrigin = mul(UNITY_MATRIX_M, float4(0, 0, 0, 1));
                float4 viewOrigin = float4(UnityObjectToViewPos(float3(0, 0, 0)), 1);
                float4 worldPos = mul(UNITY_MATRIX_M, v.vertex);
                float4 viewPos = worldPos - worldOrigin + viewOrigin; // the tutorial's way of doing it for billboarding
                //float4 viewPos = mul(UNITY_MATRIX_V, worldPos); // the normal way, basically turns it into an unlit shader
                float4 clipsPos = mul(UNITY_MATRIX_P, viewPos);
                o.vertex = clipsPos;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * tex2D(_MainTex, i.uv);

            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);

            UNITY_SETUP_INSTANCE_ID(i); // necessary only if any instanced properties are going to be accessed in the fragment Shader.


            return col;

            //return col;
            }
            ENDCG

        }
    }
}






//Shader "Billboards" {
//   Properties {
//      _MainTex ("Texture Image", 2D) = "white" {}
//      _ScaleX ("Scale X", Float) = 1.0
//      _ScaleY ("Scale Y", Float) = 1.0
//   }
//   SubShader {
//
//        Tags
//        {
//            "Queue" = "Transparent"
//            "SortingLayer" = "Resources_Sprites"
//            "IgnoreProjector" = "True"
//            "RenderType" = "Transparent"
//            "PreviewType" = "Plane"
//            "CanUseSpriteAtlas" = "True"
//            "DisableBatching" = "True"
//        } 
//
//         Cull Off
//         Lighting Off
//         ZWrite Off
//         Blend One OneMinusSrcAlpha
//
//      Pass {   
//         CGPROGRAM
// 
//         #pragma vertex vert  
//         #pragma fragment frag
//
//         // User-specified uniforms            
//         uniform sampler2D _MainTex;        
//         uniform float _ScaleX;
//         uniform float _ScaleY;
//
//         struct vertexInput {
//            float4 vertex : POSITION;
//            float4 tex : TEXCOORD0;
//         };
//         struct vertexOutput {
//            float4 pos : SV_POSITION;
//            float4 tex : TEXCOORD0;
//         };       
//         
//         vertexOutput vert(vertexInput input) 
//         {
//            vertexOutput output;
//            
//            output.pos = mul(UNITY_MATRIX_P,
//                mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
//                + float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
//              * float4(_ScaleX, _ScaleY, 1.0, 1.0));
// 
//            output.tex = input.tex;
//
//            return output;
//         }
// 
//         float4 frag(vertexOutput input) : COLOR
//         {
//            return tex2D(_MainTex, float2(input.tex.xy));   
//         }
// 
//         ENDCG
//      }
//   }
//}