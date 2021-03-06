﻿Shader "Sprites/Lighting/TilesShader+RimLight"
{
	Properties
	{
		[PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		UsePass "Sprites/Lighting/TilesShader/FORWARD"
		UsePass "Sprites/Lighting/Cats/FORWARDADD"
	}

	FallBack "Sprites/Lighting/TilesShader"
}
