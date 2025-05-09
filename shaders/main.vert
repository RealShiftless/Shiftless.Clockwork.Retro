#version 450 core

layout(location = 0) in vec2 aPos;

out vec2 uvCoord;

void main(void)
{
	uvCoord = aPos;

	if(uvCoord.x < 0)
	{
		uvCoord.x = 0;
	}
	
	if(uvCoord.y < 0)
	{
		uvCoord.y = 0;
	}

	gl_Position = vec4(aPos, 0, 1);
}