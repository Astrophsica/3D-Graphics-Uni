#version 330

in vec3 vPosition;
in vec2 vTexCoords;

out vec2 oTexCoords;


void main()
{
	gl_Position = vec4(vPosition.x, vPosition.y, 0.0, 1.0);
	oTexCoords = vTexCoords;
}