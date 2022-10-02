Shader "Unlit/AtmosphereRings"
{
    Properties
    {
        _Tex1("Texture1", 2D) = "white" {} // текстура1
        _Tex2("Texture2", 2D) = "white" {} // карта нормали
        _Tex3("Texture2", 2D) = "white" {} // прозрачная
        _MixValue("Mix Value", Range(0,1)) = 0.5 // параметр смешивания текстур

        _MainColor("_MainColor", Color) = (1,1,1,1) //цвет атмосферы
        _Height("_Height", Range(0,10)) = 0.1
        _MainTex("Color (RGB) Alpha (A)", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert // директива для обработки вершин
            #pragma fragment frag // директива для обработки фрагментов
            #include "UnityCG.cginc" // библиотека с полезными функциями
            sampler2D _Tex1; // текстура1
            float4 _Tex1_ST;
            sampler2D _Tex2; // текстура2
            float4 _Tex2_ST;
            float _MixValue; // параметр смешивания

            struct v2f
            {
                float2 uv : TEXCOORD0; // UV-координаты вершины
                float4 vertex : SV_POSITION; // координаты вершины

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Height;

            v2f vert(appdata_full v)
            {
                v2f result;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _Tex1);
                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color;
                color = tex2D(_Tex1, i.uv) * _MixValue;
                color += tex2D(_Tex2, i.uv) * (1 - _MixValue);
                return color;

            }
            ENDCG
        }
            Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _Tex3; // текстура
            float4 _Tex3_ST;
            float _Height;
            float4 _MainColor;
            float _MainTex;

            v2f vert(appdata_full v)
            {
                v2f result;
                v.vertex.xyz += v.normal * _Height;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _Tex3);
                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color;
                color = tex2D(_Tex3, i.uv)*_MainTex;
                color = color * _MainColor;
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            ENDCG
        }
        
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _Tex2; // текстура
            float4 _Tex2_ST;
            float _Height;
            
            v2f vert(appdata_full v)
            {
                v2f result;
                v.vertex.xz += v.normal * _Height*4;
                v.vertex.y -= v.normal * _Height / 10;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _Tex2);
                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color;
                color = tex2D(_Tex2, i.uv)
               // color = tex2D(_Tex1, i.uv)*_MainTex;
                // color += tex2D(_Tex4, i.uv)*(1 - _MainTex);
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            
            ENDCG
        }
    }
}
