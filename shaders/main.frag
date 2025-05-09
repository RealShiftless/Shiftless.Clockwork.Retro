#version 450

const int NATIVE_WIDTH = 240;
const int NATIVE_HEIGHT = 135;

const int TILEMAP_WIDTH = 60;
const int TILEMAP_HEIGHT = 36;

const int X_OFFSET_BYTE = 0;
const int Y_OFFSET_BYTE = 2;

const int TILE_SIZE = 8;

const uint LAYERS = 4;

uniform usampler2DArray tileset;
uniform usampler2DArray tilemap;
uniform usampler1DArray palette;
uniform usampler1DArray info;

in vec2 uvCoord;

out vec4 fragColor;


vec4 unpack565(uint color) 
{
    uint r = (color >> 11) & 31;
    uint g = (color >> 5) & 63;
    uint b = color & 31;

    return vec4(r / 31., g / 63., b / 31., 1);
}


void main()
{
	

	// Now we go layer by layer to check it's color, if the layers pixel is #FF00FF we continue
	uint packedColor = 63519; // 63519 is equal to #FF00FF in RGB565
	for(uint i = 0; i < LAYERS; i++) 
	{
		// Get the layer
		uint layer = LAYERS - i - 1;

		uint xOffset = (texelFetch(info, ivec2(X_OFFSET_BYTE + 1, layer), 0).r << 8) | texelFetch(info, ivec2(X_OFFSET_BYTE + 0, layer), 0).r;
		uint yOffset = (texelFetch(info, ivec2(Y_OFFSET_BYTE + 1, layer), 0).r << 8) | texelFetch(info, ivec2(Y_OFFSET_BYTE + 0, layer), 0).r;

		int x = int(mod(uvCoord.x * NATIVE_WIDTH  + xOffset, TILEMAP_WIDTH * TILE_SIZE));
		int y = int(mod(uvCoord.y * NATIVE_HEIGHT + yOffset, TILEMAP_HEIGHT * TILE_SIZE));

		// First we get the raw pixel coord on the screen, we need to add a camera to this aswell
		ivec2 pixelCoord = ivec2(x, y);

		// Get the tile position
		ivec2 tileCoord = pixelCoord / TILE_SIZE;

		// Get the local position within the tile
		ivec2 localPixCoord = pixelCoord % TILE_SIZE;

		// Get the packed tile value
		uint packedTile = texelFetch(tilemap, ivec3(tileCoord, layer), 0).r;

		// Extract the needed data from said tile
		uint tileIndex = packedTile >> 8;
		uint transform = packedTile >> 4 & 15;
		uint paletteId = packedTile & 15;

		bool flipHorizontal = (transform & 8) != 0;
		bool flipVertical   = (transform & 4) != 0;
		uint rotation = transform & 0x3u;

		// Apply flip
		if(flipHorizontal)
		{
			localPixCoord.x = TILE_SIZE - 1 - localPixCoord.x;
		}
		if(flipVertical)
		{
			localPixCoord.y = TILE_SIZE - 1 - localPixCoord.y;
		}

		// Apply the rotation
		if (rotation == 1u) { // 90 degrees
			localPixCoord = ivec2(localPixCoord.y, TILE_SIZE - 1 - localPixCoord.x);
		} else if (rotation == 2u) { // 180 degrees
		    localPixCoord = ivec2(TILE_SIZE - 1 - localPixCoord.x, TILE_SIZE - 1 - localPixCoord.y);
		} else if (rotation == 3u) { // 270 degrees
		    localPixCoord = ivec2(TILE_SIZE - 1 - localPixCoord.y, localPixCoord.x);
		}

		// Get packed value shiz
		int packedX = localPixCoord.x / 4;
		int shift = (3 - (localPixCoord.x % 4)) * 2;

		// Fetch from the correct tile layer
		uint packedByte = texelFetch(tileset, ivec3(packedX, localPixCoord.y, tileIndex), 0).r;
		
		// Extract 2-bit value
		uint value = (packedByte >> shift) & 0x3u;

		// We get the color from the palette
		packedColor = texelFetch(palette, ivec2(value, paletteId), 0).r;

		// Now we check if we set packed color to something else than #FF00FF
		if(packedColor != 63519)
		{
			break;
		}
	}

	vec4 color = unpack565(packedColor);
	if(packedColor == 63519)
	{
		color.xyz = vec3(0, 0, 0);
	}

	fragColor = color;
}