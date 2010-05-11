uniform extern texture ScreenTexture;	

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};




float Timer : TIME;

float4 Grain(float2 Tex : TEXCOORD0 ) : COLOR0 
{
	float x =  Tex.x * Tex.y * 31337 * Timer;
	x = fmod(x,13) * fmod(x,123);
	float dx = fmod(x,0.03);
	float dy = fmod(x,0.03);
	float4 color = tex2D(ScreenS, Tex + float2(dx,dy));		
	return color;
} 



float wave;							// pi/.75 is a good default
float distortion;					// 1 is a good default
float centerCoordX;					// 0.5,0.5 is the screen center
float centerCoordY;

float4 Ripple(float2 texCoord: TEXCOORD0) : COLOR0
{
    float2 distance = abs(texCoord - (centerCoordX, centerCoordY));
    float scalar = length(distance);

    // invert the scale so 1 is centerpoint
    scalar = abs(1 - scalar);
        
    // calculate how far to distort for this pixel    
    float sinoffset = sin(wave / scalar);
    sinoffset = clamp(sinoffset, 0, 1);
    
    // calculate which direction to distort
    float sinsign = cos(wave / scalar);    
    
    // reduce the distortion effect
    sinoffset = sinoffset * distortion / 32;
    
    // pick a pixel on the screen for this pixel, based on
    // the calculated offset and direction
    float4 color = tex2D(ScreenS, texCoord + (sinoffset*sinsign));    
            
    return color;
}


technique Grain
{ 
	pass Pass_0 
	{ 
		PixelShader = compile ps_2_0 Grain(); 
	} 
} 

technique Ripple
{
	pass Pass_0
	{ 
		PixelShader = compile ps_2_0 Ripple(); 
    } 
} 	