float AlphaValue;
sampler2D clrSampler;

float4 ApplyAlphaValue(float2 Tex : TEXCOORD0) : COLOR0
{
	float4 Color;
	Color = tex2D(clrSampler, Tex.xy);
	Color.a *= AlphaValue;
    return Color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 ApplyAlphaValue();
    }
}
