Shader "Sprites/Outline"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		// Add values to determine if outlining is enabled and outline color.
		[PerRendererData] _Outline("Outline", Float) = 0
		[PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSizeX("Outline Size X", int) = 1
		[PerRendererData] _OutlineSizeY("Outline Size Y", int) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
#pragma vertex SpriteVert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_instancing
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
#include "UnitySprites.cginc"

			float _Outline;
			fixed4 _OutlineColor;
			int _OutlineSizeX;
			int _OutlineSizeY;
			float4 _MainTex_TexelSize;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

				// If outline is enabled and there is a pixel, try to draw an outline.
				if (_Outline > 0 && c.a != 0) {
					float totalAlpha = 1.0;
					float totalAlpha1 = 1.0;
					float totalAlpha2 = 1.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSizeX + 1; i++) {
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

						totalAlpha = totalAlpha * pixelRight.a * pixelLeft.a;
					}
					fixed4 pixelRight1 = tex2D(_MainTex, IN.texcoord + fixed2(_OutlineSizeX * _MainTex_TexelSize.x, 0));
					fixed4 pixelLeft1 = tex2D(_MainTex, IN.texcoord - fixed2(-_OutlineSizeX * _MainTex_TexelSize.x, 0));
					totalAlpha1 = totalAlpha * pixelRight1.a * pixelLeft1.a;
					fixed4 pixelRight2 = tex2D(_MainTex, IN.texcoord + fixed2((_OutlineSizeX + 1) * _MainTex_TexelSize.x, 0));
					fixed4 pixelLeft2 = tex2D(_MainTex, IN.texcoord - fixed2(-(_OutlineSizeX + 1) * _MainTex_TexelSize.x, 0));
					totalAlpha2 = totalAlpha1 * pixelRight2.a * pixelLeft2.a;

					[unroll(16)]
					for (int i = 1; i < _OutlineSizeY + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i *  _MainTex_TexelSize.y));

						totalAlpha = totalAlpha * pixelUp.a * pixelDown.a;
					}
					fixed4 pixelUp1 = tex2D(_MainTex, IN.texcoord + fixed2(0, _OutlineSizeY * _MainTex_TexelSize.y));
					fixed4 pixelDown1 = tex2D(_MainTex, IN.texcoord - fixed2(0, -_OutlineSizeY * _MainTex_TexelSize.y));
					totalAlpha1 = totalAlpha1 * pixelUp1.a * pixelDown1.a;
					fixed4 pixelUp2 = tex2D(_MainTex, IN.texcoord + fixed2(0, (_OutlineSizeY + 1) * _MainTex_TexelSize.y));
					fixed4 pixelDown2 = tex2D(_MainTex, IN.texcoord - fixed2(0, -(_OutlineSizeY + 1) * _MainTex_TexelSize.y));
					totalAlpha2 = totalAlpha2 * pixelUp2.a * pixelDown2.a;

					if (totalAlpha < 0.1) {
						c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
					}
					else if (totalAlpha1 < 0.1) {
						c.r = c.r * 0.33 + _OutlineColor.r * 0.67;
						c.g = c.g * 0.33 + _OutlineColor.g * 0.67;
						c.b = c.b * 0.33 + _OutlineColor.b * 0.67;
					}
					else if (totalAlpha2 < 0.1) {
						c.r = c.r * 0.67 + _OutlineColor.r * 0.33;
						c.g = c.g * 0.67 + _OutlineColor.g * 0.33;
						c.b = c.b * 0.67 + _OutlineColor.b * 0.33;
					}
				}

				c.a = (SampleSpriteTexture(IN.texcoord) * IN.color).a;
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
