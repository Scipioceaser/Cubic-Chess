// Taken from https://www.youtube.com/watch?v=SlTkBe4YNbo

Shader "Outline"
{
    Properties
    {
		_Color("Main color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline width", Range(1.0, 5.0)) = 1.01
    }
	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : POSITION;
		float3 normal : NORMAL;
		float4 color : COLOR;
	};

	float _OutlineWidth;
	float4 _OutlineColor;

	v2f vert(appdata v)
	{
		v2f o;

		UNITY_INITIALIZE_OUTPUT(v2f, o);
		v.vertex.xyz *= _OutlineWidth;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.color = _OutlineColor;
		return o;
	}

	ENDCG

	SubShader
	{
		Tags{"Queue" = "Transparent"}

		Pass // render outline
		{
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			half4 frag(v2f i) : COLOR {
				return _OutlineColor;
			}

			ENDCG
		}

		Pass // render normal model
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}			

			Lighting On

			SetTexture[_MainTex] {
				ConstantColor[_Color]
			}

			SetTexture[_MainTex] {
				Combine previous * primary DOUBLE
			}
		}
	}
}
