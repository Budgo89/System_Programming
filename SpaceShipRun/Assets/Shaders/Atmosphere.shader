Shader "Unlit/Atmosphere"
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

		_Diffuse("Diffuse",Color) = (1,1,1,1)
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(8.0,256)) = 8.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
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
					color = tex2D(_Tex3, i.uv) * _MainTex;
					color = color * _MainColor;
					UNITY_APPLY_FOG(i.fogCoord, color);
					return color;
				}
				ENDCG
			}

			//Pass
			//{
			//	Tags{"LightMode" = "ForwardBase"}
			//	CGPROGRAM
			//	#pragma multi_compile_fwdbase 
			//	#pragma vertex vert
			//	#pragma fragment frag

			//	#include "UnityCG.cginc"
			//	#include "Lighting.cginc"
			//	#include "AutoLight.cginc"
			//	fixed4 _Diffuse;
			//	fixed4 _Specular;
			//	fixed _Gloss;

			//	struct appdata
			//	{
			//		float4 vertex : POSITION;
			//		float2 uv : TEXCOORD0;
			//		float3 normal:NORMAL;
			//	};

			//	struct v2f
			//	{
			//		float4 pos : SV_POSITION;
			//		float3 worldNormal : TEXCOORD0;
			//		float3 worldPos:TEXCOORD1;
			//		float3 vertexLight : TEXCOORD2;
			//		SHADOW_COORDS(3)
			//	};

			//	sampler2D _MainTex;
			//	float4 _MainTex_ST;

			//	v2f vert(appdata v)
			//	{
			//		v2f o;
			//		o.pos = UnityObjectToClipPos(v.vertex);
			//		o.worldNormal = UnityObjectToWorldNormal(v.normal);
			//		o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
			//		#ifdef LIGHTMAP_OFF
			//		float shLight = ShadeSH9(float4(v.normal,1.0));
			//		o.vertexLight = shLight;
			//		#ifdef VERTEXLIGHT_ON 
			//		float3 vertexLight = Shade4PointLights(unity_4LightPosX0,unity_4LightPosY0,unity_4LightPosZ0,
			//		unity_LightColor[0].rgb,unity_LightColor[1].rgb,unity_LightColor[2].rgb,unity_LightColor[3].rgb,
			//		unity_4LightAtten0,o.worldPos,o.worldNormal);
			//		o.vertexLight += vertexLight;
			//		#endif
			//		#endif
			//		TRANSFER_SHADOW(o);
			//		return o;
			//	}

			//	fixed4 frag(v2f i) : SV_Target
			//	{
			//		fixed3 worldNormal = normalize(i.worldNormal);
			//		fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
			//		fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			//		//fixed3 halfLambert = dot(worldNormal , worldLightDir) * 0.5 + 0.5;
			//		fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0,dot(worldNormal,worldLightDir));


			//		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
			//		fixed3 halfDir = normalize(worldLightDir + viewDir);


			//		fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(worldNormal,halfDir)),_Gloss);

			//		UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);


			//		return fixed4((ambient + (diffuse + specular) * atten + i.vertexLight), 1);
			//	}
			//	ENDCG
			//}

			//Pass
			//{
			//	Tags{"LightMode" = "ForwardAdd"}
			//	Blend One One

			//	CGPROGRAM
			//	#pragma multi_compile_fwdadd_fullshadows
			//	#pragma vertex vert
			//	#pragma fragment frag

			//	#include "Lighting.cginc"
			//	#include "AutoLight.cginc"

			//	fixed4 _Diffuse;
			//	fixed4 _Specular;
			//	fixed _Gloss;

			//	struct a2v
			//	{
			//		float4 vertex:POSITION;
			//		float3 normal:NORMAL;
			//	};
			//	struct v2f
			//	{
			//		float4 pos:SV_POSITION;
			//		float3 worldNormal:TEXCOORD0;
			//		float3 worldPos: TEXCOORD1;
			//		LIGHTING_COORDS(2,3)

			//	};
			//	v2f vert(a2v v)
			//	{
			//		v2f o;
			//		o.pos = UnityObjectToClipPos(v.vertex);
			//		o.worldNormal = UnityObjectToWorldNormal(v.normal);
			//		o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
			//		TRANSFER_VERTEX_TO_FRAGMENT(o);
			//		return o;

			//	}
			//	fixed4 frag(v2f i) :SV_Target
			//	{
			//		fixed3 worldNormal = normalize(i.worldNormal);
			//		fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

			//		//fixed3 halfLambert = dot(worldNormal,worldLightDir) *0.5 + 0.5;
			//		fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0,dot(worldNormal,worldLightDir));

			//		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
			//		fixed3 halfDir = normalize(worldLightDir + viewDir);
			//		fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(viewDir,halfDir)),_Gloss);

			//		//fixed3 atten = LIGHT_ATTENUATION(i);
			//		UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);

			//		return fixed4((diffuse + specular) * atten,1.0);
			//	}

			//	ENDCG
			//}

					/*FallBack "Diffuse"*/



		}
}
