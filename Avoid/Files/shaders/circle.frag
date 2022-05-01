#version 330
out vec4 outputColor;
in vec2 texCoord;
uniform vec2 innerResolution;

uniform sampler2D texture0;
uniform vec4 color;

float innerGrad(float f){
	return f * f * 5.25;
}


void main()
{
	float l = (texCoord.y - 0.5) * (texCoord.y - 0.5) + (texCoord.x - 0.5) * (texCoord.x - 0.5);
    outputColor = color + vec4(vec3(0.0), texCoord.y * 0.22) + vec4(vec3(0.0), texCoord.x * 0.2) - vec4(vec3(0.0), 0.22) - innerGrad(0.25-l);
	
    if(l > 0.25){
        outputColor = vec4(0.0);
    }
}