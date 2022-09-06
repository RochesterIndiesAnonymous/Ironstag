//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that SpriteBatch will try to set before drawing
texture2D ScreenTexture;
float AlphaValue;
// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader will simply look up the color of the texture at the
// requested point, and turns it into a shade of gray
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = ScreenTexture.Sample(TextureSampler, texCoord.xy);

	/*float value = (color.r + color.g + color.b) / 3;
	color.r = value;
	color.g = value;
	color.b = value;
	*/
	color.a *= AlphaValue;
	return color;
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
#if OPENGL
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif
technique BlackAndWhite
{
	pass Pass1
	{
		PixelShader = compile ps_3_0  PixelShaderFunction();
	}
}
