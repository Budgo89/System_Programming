Shader "Unlit/Atmosphere"
{
    Properties
    {
        _Tex1("Texture1", 2D) = "white" {} // ��������1
        _Tex2("Texture2", 2D) = "white" {} // ����� �������
        _Tex3("Texture2", 2D) = "white" {} // ����������
        _Tex4("Texture2", 2D) = "white" {} // ���������
        _MixValue("Mix Value", Range(0,1)) = 0.5 // �������� ���������� �������
        _Color("Main Color", COLOR) = (1,1,1,0.5) // ���� �����������

        //_MainColor("_MainColor", Color) = (1,1,1,1) //���� ���������
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
            #pragma vertex vert // ��������� ��� ��������� ������
            #pragma fragment frag // ��������� ��� ��������� ����������
            #include "UnityCG.cginc" // ���������� � ��������� ���������
            sampler2D _Tex1; // ��������1
            float4 _Tex1_ST;
            sampler2D _Tex2; // ��������2
            float4 _Tex2_ST;
            float _MixValue; // �������� ����������
            float4 _Color; // ����, ������� ����� ������������ �����������
            // ���������, ������� �������� ������������� ������ ������� � ������ ���������



            struct v2f
            {
                float2 uv : TEXCOORD0; // UV-���������� �������
                float4 vertex : SV_POSITION; // ���������� �������

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
                color = color * _Color;
                return color;

            }
            ENDCG
        }

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

            sampler2D _Tex3; // ��������
            float4 _Tex3_ST;
            sampler2D _Tex4; // ��������
            float4 _Tex4_ST;
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
                color += tex2D(_Tex4, i.uv)*(1 - _MainTex);
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            ENDCG
        }
    
    }
}
