#version 330
out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D texture0;
uniform float opacity;

void main()
{
	vec4 color = texture(texture0, texCoord);
	color.a -= opacity;
    outputColor = vec4(vec3(1), color.a * 2);
}