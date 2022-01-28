#version 330

struct DirectionalLightProperties {
	int Enabled;
	vec4 Direction;

	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

struct PointLightProperties {
	int Enabled;
	vec4 Position;

	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

struct SpotLightProperties {
	int Enabled;
	vec4 Position;

	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;

	float CutOff;
	vec4 Direction;
};

struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform PointLightProperties uPointLight[3];
uniform SpotLightProperties uSpotLight[3];
uniform DirectionalLightProperties uDirectionalLight[3];

uniform MaterialProperties uMaterial;
uniform vec4 uEyePosition;

in vec4 oNormal;
in vec4 oSurfacePosition;

out vec4 FragColour;

void main()
{
	vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);

	// Point light
	for (int i = 0; i < 3; ++i)
	{
		if (uPointLight[i].Enabled == 1)
		{
			float lightDistance  = length(uPointLight[i].Position - oSurfacePosition);
			float attenuation = 1.0 / (1.0 * 0.5 * lightDistance * 0.0075 * (lightDistance * lightDistance));

			vec4 lightDir = normalize(uPointLight[i].Position - oSurfacePosition);
			vec4 reflectedVector = reflect(-lightDir, oNormal);
			float diffuseFactor = max(dot(oNormal, lightDir), 0);
			float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);

			FragColour = FragColour + vec4(uPointLight[i].AmbientLight * uMaterial.AmbientReflectivity * attenuation +
									   uPointLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor * attenuation +
									   uPointLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor * attenuation, 1);
		}

	}

	// Directional light
	for (int i = 0; i < 3; ++i)
	{
		if (uDirectionalLight[i].Enabled == 1)
		{
			vec4 lightDir = normalize(-uDirectionalLight[i].Direction);
			vec4 reflectedVector = reflect(-lightDir, oNormal);
			float diffuseFactor = max(dot(oNormal, lightDir), 0);
			float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);

			FragColour = FragColour + vec4(uDirectionalLight[i].AmbientLight * uMaterial.AmbientReflectivity +
									   uDirectionalLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor+
									   uDirectionalLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 1);
		}

	}

	// Spot light
	for (int i = 0; i < 1; i++)
	{
		if (uSpotLight[i].Enabled == 1)
		{
			float lightDistance  = length(uSpotLight[i].Position - oSurfacePosition);
			float attenuation = 1.0 / (1.0 * 0.5 * lightDistance * 0.0075 * (lightDistance * lightDistance));

			vec4 lightDir = normalize(uSpotLight[i].Position - oSurfacePosition);
			float theta = dot(lightDir.xyz, normalize(-uSpotLight[i].Direction.xyz));

			if (theta > uSpotLight[i].CutOff)
			{
				vec4 reflectedVector = reflect(-lightDir, oNormal);
				float diffuseFactor = max(dot(oNormal, lightDir), 0);
				float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);

				FragColour = FragColour + vec4(uSpotLight[i].AmbientLight * uMaterial.AmbientReflectivity * attenuation +
										   uSpotLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor * attenuation +
										   uSpotLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor * attenuation , 1);
			}
			else
			{
				FragColour = FragColour + (vec4(uSpotLight[i].AmbientLight * uMaterial.AmbientReflectivity * attenuation, 1));
			}
		}
	}



}