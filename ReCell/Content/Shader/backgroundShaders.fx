uniform extern texture ScreenTexture;	

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};




float Timer : TIME;

float4 Grain(float2 Tex : TEXCOORD0 ) : COLOR0 
{
	float x =  Tex.x * Tex.y * 123456 * Timer;
	x = fmod(x,13) * fmod(x,123);
	float dx = fmod(x,0.01);
	float dy = fmod(x,0.01);
	float4 color = tex2D(ScreenS, Tex + float2(dx,dy));		
	return color;
} 




float4 DiffuseGrain(float2 Tex : TEXCOORD0 ) : COLOR0 
{
	float x =  Tex.x * Tex.y * 123456 * Timer;
	x = fmod(x,13) * fmod(x,123);
	float dx = fmod(x,0.01);
	float dy = fmod(x,0.01);
	float4 color = tex2D(ScreenS, Tex + float2(dx,dy));
	float4 original = tex2D(ScreenS, Tex);
	
	color = color / 1.2f;					//Make the new color diffuse
	color = (color + original) / 2;
			
	return color;
} 




float calmWave;							// pi/.75 is a good default
float calmDistortion;					// 1 is a good default
float2 calmCenterCoord;					// 0.5,0.5 is the screen center

float wave2;							// pi/.75 is a good default
float distortion2;					// 1 is a good default
float2 centerCoord2;					// 0.5,0.5 is the screen center

float4 Ripple(float2 texCoord: TEXCOORD0) : COLOR0
{
    float2 distance = abs(texCoord - calmCenterCoord);
    float scalar = length(distance);
    scalar = abs(1 - scalar);
    
    float sinoffset = sin(calmWave / scalar);
    sinoffset = clamp(sinoffset, 0, 1);
    sinoffset = sinoffset * calmDistortion / 32;
    
    float sinsign = cos(calmWave / scalar);    
    
    
    /*
    float2 distance2 = abs(texCoord - centerCoord2);
	float scalar2 = length(distance2);
	float sinoffset2, sinsign2;
	if(scalar2 < 0.75)
	{
		// invert the scale so 1 is centerpoint
		scalar2 = abs(1 - scalar2);
		
		// calculate how far to distort for this pixel	
		sinoffset2 = sin(wave2 / scalar2);
		sinoffset2 = clamp(sinoffset2, 0, 1);
	
		// calculate which direction to distort
		sinsign2 = cos(wave2 / scalar2);	
	
		// reduce the distortion effect
		sinoffset2 = sinoffset2 * distortion2 / 32;
	}
	else
	{
		sinoffset2 = 0;
		sinsign2 = 0;
	}	
    */
    
    float4 color = tex2D(ScreenS, texCoord + (sinoffset*sinsign)/* + (sinoffset2*sinsign2)*/);   
    return color;
}


technique Grain
{ 
	pass Pass_0 
	{ 
		PixelShader = compile ps_2_0 Grain(); 
	} 
} 

technique DiffuseGrain
{ 
	pass Pass_0 
	{ 
		PixelShader = compile ps_2_0 DiffuseGrain(); 
	} 
} 

technique Ripple
{
	pass Pass_0
	{ 
		PixelShader = compile ps_2_0 Ripple(); 
    } 
} 	