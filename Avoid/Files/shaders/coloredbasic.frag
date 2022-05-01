#version 330
out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D texture0;
uniform vec4 color;

void main()
{
	vec4 colorT = texture(texture0, texCoord);

    outputColor = vec4(1, 1, 1, colorT.a) * color + vec4(vec3(colorT.a), 0);
}