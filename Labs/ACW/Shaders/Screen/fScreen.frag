#version 330

in vec2 oTexCoords;

uniform sampler2D uTextureSampler;
uniform int uEffectMode;

out vec4 FragColour;

const float offset = 1.0 / 300.0;


void main()
{
	vec2 offsets[9] = vec2[](
		vec2(-offset, offset),  
		vec2(0.0f,    offset),
		vec2(offset,  offset),
		vec2(-offset, 0.0f),
		vec2(0.0f,    0.0f),
		vec2(offset,  0.0f),
		vec2(-offset, -offset),
		vec2(0.0f, offset),
		vec2(offset, offset)
	);

	// Reference: https://learnopengl.com/Advanced-OpenGL/Framebuffers
	// Inverse
	if (uEffectMode == 1)
	{
		FragColour = vec4(vec3(1.0 - texture(uTextureSampler, oTexCoords)), 1.0);
	}
	// Grayscale
	else if (uEffectMode == 2)
	{
		FragColour = texture(uTextureSampler, oTexCoords);
		float average = (0.2126 * FragColour.r) + (0.7152 * FragColour.g) + (0.0722 * FragColour.b);
		FragColour = vec4(average, average, average, 1.0);
	}
	// Effects using Kernal
	else if(uEffectMode == 3 || uEffectMode == 4 || uEffectMode == 5)
	{
		float kernal[9];
		// Sharpen
		if (uEffectMode == 3)
		{
			kernal = float[](
			-1, -1, -1,
			-1, 9, -1,
			-1, -1, -1
			);
		}
		// Blur
		else if (uEffectMode == 4)
		{
			kernal = float[](
			1.0 / 16, 2.0 / 16, 1.0 / 16,
			2.0 / 16, 4.0 / 16, 2.0 / 16,
			1.0 / 16, 2.0 / 16, 1.0 / 16
			);
		}
		// Edge Detection
		else if (uEffectMode == 5)
		{
			kernal = float[](
			1,  1,  1,
			1, -8,  1,
			1,  1,  1
			);
		}


		vec3 sampleTex[9];
		for(int i = 0; i < 9; i++)
		{
			sampleTex[i] = vec3(texture(uTextureSampler, oTexCoords.st + offsets[i]));
		}
		vec3 col = vec3(0.0);
		for(int i = 0; i < 9; i++)
		{
			col += sampleTex[i] * kernal[i];
		}
		FragColour = vec4(col, 1.0);
	}
	else
	{
		FragColour = texture(uTextureSampler, oTexCoords);
	}
	
}