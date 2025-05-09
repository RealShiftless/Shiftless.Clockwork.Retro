#version 450

uniform sampler2D frameBuffer;

in vec2 uvCoord;

out vec4 fragColor;

void main()
{
	fragColor = texture(frameBuffer, uvCoord);
}