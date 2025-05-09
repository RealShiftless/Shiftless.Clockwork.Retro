using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Rendering
{
    public sealed class Tilemap
    {
        // Constants
        public const int WIDTH = 60;
        public const int HEIGHT = 36;

        public const int LAYERS = 4;

        public const int INFO_SIZE = 4;
        public const int LAYER_OFFSET_X_BYTE = 0;
        public const int LAYER_OFFSET_Y_BYTE = 2;

        public const int DEFAULT_OFFSET_X = 0;
        public const int DEFAULT_OFFSET_Y = 0;

        public const int PIXEL_WIDTH = WIDTH * Tileset.TILE_PIXEL_AREA;
        public const int PIXEL_HEIGHT = HEIGHT * Tileset.TILE_PIXEL_AREA;

        public const TextureUnit TEXTURE_UNIT = TextureUnit.Texture1;
        public const TextureUnit INFO_UNIT = TextureUnit.Texture3;

        private const ushort INDEX_CLEAR_MASK = 0b0000000011111111;
        private const ushort TRANSFORM_CLEAR_MASK = 0b1111111100001111;
        private const ushort PALETTE_CLEAR_MASK = 0b1111111111110000;


        // Values
        private int _textureHandle;
        private int _layerInfoHandle; // We use a texture to store the individual info per layer, like its offset

        private int _minEditX;
        private int _minEditY;
        private int _maxEditX;
        private int _maxEditY;
        private int _minEditLayer;
        private int _maxEditLayer;

        private readonly ushort[] _data = new ushort[WIDTH * HEIGHT * LAYERS];

        private readonly byte[] _layerInfoBuffer = new byte[LAYERS * INFO_SIZE];


        // Constructor
        internal Tilemap()
        {
        }


        // Func
        internal void Initialize()
        {
            // First we create the texture to store the tilemap to
            _textureHandle = GL.GenTexture();

            // First we bind it to its unit
            GL.ActiveTexture(TEXTURE_UNIT);
            GL.BindTexture(TextureTarget.Texture2DArray, _textureHandle);

            // We setup some parameters for the texture
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Now refresh to actually fill with data
            Refresh();


            // To store the layers individual info we also create a texture, this stores little bytes like the offset, lets first fill our buffer with the default data
            for (int layerIndex = 0; layerIndex < LAYERS; layerIndex++)
            {
                int infoIndex = layerIndex * INFO_SIZE;

                _layerInfoBuffer[infoIndex + LAYER_OFFSET_X_BYTE] = DEFAULT_OFFSET_X;
                _layerInfoBuffer[infoIndex + LAYER_OFFSET_Y_BYTE] = DEFAULT_OFFSET_Y;
            }

            // After this we must create a little info section for each layer
            _layerInfoHandle = GL.GenTexture();

            // We activate the unit and fill up some default data
            GL.ActiveTexture(INFO_UNIT);
            GL.BindTexture(TextureTarget.Texture1DArray, _layerInfoHandle);

            // We setup some parameters for the texture
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // And we fill it with all of the actual data
            GL.TexImage2D(TextureTarget.Texture1DArray, 0, PixelInternalFormat.R8ui, INFO_SIZE, LAYERS, 0, PixelFormat.RedInteger, PixelType.UnsignedByte, _layerInfoBuffer);
        }

        public void Refresh()
        {
            // Because we always keep the tilemap bound we can just activate its unit :)
            GL.ActiveTexture(TEXTURE_UNIT);

            // And here we can just set all the data at once
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.R16ui, WIDTH, HEIGHT, LAYERS, 0, PixelFormat.RedInteger, PixelType.UnsignedShort, _data);
        }

        public Vec2i GetOffset(byte layer)
        {
            int index = layer * INFO_SIZE;

            ushort x = (ushort)(_layerInfoBuffer[index + LAYER_OFFSET_X_BYTE + 1] << 8 | _layerInfoBuffer[index + LAYER_OFFSET_X_BYTE + 0]);
            ushort y = (ushort)(_layerInfoBuffer[index + LAYER_OFFSET_Y_BYTE + 1] << 8 | _layerInfoBuffer[index + LAYER_OFFSET_Y_BYTE + 0]);

            return new Vec2i(x, y);
        }
        public void SetOffset(byte layer, int x, int y)
        {
            int index = layer * INFO_SIZE;

            x = MHelp.Mod(x, PIXEL_WIDTH);
            y = MHelp.Mod(y, PIXEL_HEIGHT);

            byte x1 = (byte)(x & 0xFF);
            byte x2 = (byte)(x >> 8 & 0xFF);
            byte y1 = (byte)(y & 0xFF);
            byte y2 = (byte)(y >> 8 & 0xFF);

            _layerInfoBuffer[index + LAYER_OFFSET_X_BYTE + 0] = x1;
            _layerInfoBuffer[index + LAYER_OFFSET_X_BYTE + 1] = x2;
            _layerInfoBuffer[index + LAYER_OFFSET_Y_BYTE + 0] = y1;
            _layerInfoBuffer[index + LAYER_OFFSET_Y_BYTE + 1] = y2;

            byte[] data = [x1, x2, y1, y2];

            GL.ActiveTexture(INFO_UNIT);
            GL.TexSubImage2D(TextureTarget.Texture1DArray, 0, 0, layer, 4, 1, PixelFormat.RedInteger, PixelType.UnsignedByte, data);
        }
        public void SetOffset(byte layer, Vec2i offset) => SetOffset(layer, offset.X, offset.Y);

        public void Set(int index, byte? tileIndex = null, TileTransform? transform = null, PaletteIndex? paletteId = null)
        {
            tileIndex ??= GetTile(index);
            transform ??= GetTransform(index);
            paletteId ??= GetPalette(index);

            ushort packedTile = (ushort)(tileIndex << 8 | (int)transform << 4 | (int)paletteId);
            _data[index] = packedTile;
        }
        public void Set(int x, int y, int layer, byte? tileIndex = null, TileTransform? transform = null, PaletteIndex? paletteId = null) => Set(CalculateIndex(x, y, layer), tileIndex, transform, paletteId);
        public void Set(Point8 pos, int layer, byte? tileIndex = null, TileTransform? transform = null, PaletteIndex? paletteId = null) => Set(pos.X, pos.Y, layer, tileIndex, transform, paletteId);

        public void SetTile(int index, byte tileIndex) => Set(index, tileIndex: tileIndex);
        public void SetTile(int x, int y, int layer, byte tileIndex) => SetTile(CalculateIndex(x, y, layer), tileIndex);
        public void SetTile(Point8 pos, int layer, byte tileIndex) => SetTile(pos.X, pos.Y, layer, tileIndex);

        public void SetTransform(int index, TileTransform transform) => Set(index, transform: transform);
        public void SetTransform(int x, int y, int layer, TileTransform transform) => SetTransform(CalculateIndex(x, y, layer), transform);
        public void SetTransform(Point8 pos, int layer, TileTransform transform) => SetTransform(pos.X, pos.Y, layer, transform);

        public void SetPalette(int index, PaletteIndex paletteId) => Set(index, paletteId: paletteId);
        public void SetPalette(int x, int y, int layer, PaletteIndex paletteId) => SetPalette(CalculateIndex(x, y, layer), paletteId);
        public void SetPalette(Point8 pos, int layer, PaletteIndex paletteId) => SetPalette(pos.X, pos.Y, layer, paletteId);

        public (byte tileId, TileTransform transform, PaletteIndex paletteId) Get(int index)
        {
            ushort packedValue = _data[index];

            byte tileId = (byte)(packedValue >> 8);
            TileTransform transform = (TileTransform)(packedValue >> 4 & 0xF);
            PaletteIndex paletteId = (PaletteIndex)(packedValue & 0xF);

            return (tileId, transform, paletteId);
        }
        public (byte tileId, TileTransform transform, PaletteIndex paletteId) Get(int x, int y, int layer) => Get(CalculateIndex(x, y, layer));

        public byte GetTile(int index) => (byte)(_data[index] >> 8);
        public byte GetTile(int x, int y, int layer) => GetTile(CalculateIndex(x, y, layer));

        public TileTransform GetTransform(int index) => (TileTransform)(_data[index] >> 4 & 0xF);
        public TileTransform GetTransform(int x, int y, int layer) => GetTransform(CalculateIndex(x, y, layer));

        public PaletteIndex GetPalette(int index) => (PaletteIndex)(_data[index] & 0xF);
        public PaletteIndex GetPalette(int x, int y, int layer) => GetPalette(CalculateIndex(x, y, layer));

        private static int CalculateIndex(int x, int y, int layer)
        {
            if (x < 0 || x >= WIDTH)
                x = MHelp.Mod(x, WIDTH);
            if (y < 0 || y >= HEIGHT)
                y = MHelp.Mod(y, HEIGHT);

            return y * WIDTH + x + layer * WIDTH * HEIGHT;
        }
    }
}
