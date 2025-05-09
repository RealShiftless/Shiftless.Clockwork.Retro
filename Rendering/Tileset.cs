using OpenTK.Graphics.OpenGL4;
using Shiftless.Clockwork.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Rendering
{
    public sealed class Tileset
    {
        // Constants
        public const int MAX_TEXTURES = 256;

        public const int TILE_PIXEL_AREA = 8;
        public const int TILE_BYTE_SIZE = TILE_PIXEL_AREA / PIXELS_PER_BYTE * TILE_PIXEL_AREA;
        public const int PIXELS_PER_BYTE = 4;

        public const TextureUnit TEXTURE_UNIT = TextureUnit.Texture0;


        // Values
        private int _handle;

        private readonly byte[] _freeFlags = new byte[MAX_TEXTURES / 8];
        private int _textures;


        // Properties
        public int Textures => _textures;


        // Constructor
        internal Tileset() { }


        // Func
        internal void Initialize()
        {
            _handle = GL.GenTexture();

            GL.ActiveTexture(TEXTURE_UNIT);
            GL.BindTexture(TextureTarget.Texture2DArray, _handle);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            byte[] data = new byte[MAX_TEXTURES * TILE_BYTE_SIZE];

            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.R8ui, TILE_PIXEL_AREA / PIXELS_PER_BYTE, TILE_PIXEL_AREA, MAX_TEXTURES, 0, PixelFormat.RedInteger, PixelType.UnsignedByte, data);
        }

        private byte GetNextFreeIndex()
        {
            byte i = 0;
            while (IsTextureOccupied(i))
            {
                i++;

                if (i == 0)
                {
                    throw new OutOfMemoryException("No texture was free!");
                }
            }

            return i;
        }

        public bool IsTextureOccupied(byte index)
        {
            int freeFlagByte = index / 8;
            int freeFlagBit = 7 - index % 8;

            return (_freeFlags[freeFlagByte] >> freeFlagBit & 0b1) != 0;
        }

        public byte LoadTexture(byte[] data, byte index, bool checkFree = true, bool markOccupied = true)
        {
            // Check for some errors
            if (data.Length != TILE_BYTE_SIZE)
                throw new InvalidDataException("LoadTexture expects data array of size 16, packed 4 pixels per byte.");

            if (checkFree && IsTextureOccupied(index))
                throw new ArgumentException($"Texture at index {index} was not free!");

            // Set the data
            GL.ActiveTexture(TEXTURE_UNIT);
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, index, TILE_PIXEL_AREA / PIXELS_PER_BYTE, TILE_PIXEL_AREA, 1, PixelFormat.RedInteger, PixelType.UnsignedByte, data);

            // And mark this texture as unocupied
            if (markOccupied)
                SetTextureOcupiedFlag(index, true);

            return index;
        }

        private static Texture2DData LoadTextureData(string path)
        {
            // Replace the path with apriopriate path per system
            path = path.Replace('\\', Path.DirectorySeparatorChar);

            // Make the path local to the Clockwork Asset Pipeline ting
            path = Path.Combine("assets", "bin", $"{path}.bin");

            // Load the data
            Texture2DData data = Texture2DData.LoadFromFile(path);

            // Now do some format checking
            if (data.Width % TILE_PIXEL_AREA != 0 || data.Height % TILE_PIXEL_AREA != 0)
                throw new Exception($"Invallid tile size! ({data.Width}, {data.Height})");

            if (data.ColorMode != ColorMode.Palette2)
                throw new Exception($"Invallid color mode! ({data.ColorMode})");

            return data;
        }
        private void LoadTextures(Texture2DData data, byte[] indices, bool markOccupied)
        {
            // Get the raw pixel data
            byte[] rawData = data.Raw.ToArray();

            // Lets loop over all the tiles to load them
            for (int i = 0; i < indices.Length; i++)
            {
                byte[] bytes = rawData[(i * TILE_BYTE_SIZE)..(i * TILE_BYTE_SIZE + TILE_BYTE_SIZE)];
                LoadTexture(bytes, indices[i], false, markOccupied);
            }
        }
        public byte[] LoadTextures(string path, byte[] indices, bool checkTexturesOccupied = true)
        {
            if (checkTexturesOccupied)
            {
                foreach (byte index in indices)
                    if (IsTextureOccupied(index))
                        throw new Exception($"Texture at index {index} was not free!");
            }

            // Get the data
            Texture2DData data = LoadTextureData(path);

            // Now lets see how many textures are in this badboy
            int textures = data.Width / TILE_PIXEL_AREA * (data.Height / TILE_PIXEL_AREA);

            // Check if the data size and the texture count matches
            if (indices.Length != textures)
                throw new ArgumentException($"Indicices length did not match texture count! ({indices.Length} != {textures})");

            LoadTextures(data, indices, true);

            return indices;
        }
        public byte[] LoadTextures(string path)
        {
            // Get the data
            Texture2DData data = LoadTextureData(path);

            // Now lets see how many textures are in this badboy
            int textures = data.Width / TILE_PIXEL_AREA * (data.Height / TILE_PIXEL_AREA);

            // Lets create the thing that stores our textures indices
            byte[] indices = new byte[textures];

            // Loop thru the textures and get the next free index
            for (int i = 0; i < textures; i++)
            {
                indices[i] = GetNextFreeIndex();
                SetTextureOcupiedFlag(indices[i], true);
            }

            // And now just load the data direct thru a function that only exists so that we can also pre declare indices
            LoadTextures(data, indices, false);

            /*
            // Get the raw pixel data
            byte[] rawData = data.Raw.ToArray();

            // Lets loop over all the tiles to load them
            for (int i = 0; i < textures; i++)
            {
                // Get the next free index
                if (!GetNextIndex(out byte index))
                    throw new OutOfMemoryException("No texture was free!");

                SetTextureOcupiedFlag(index, true);

                byte[] bytes = rawData[(i * TILE_BYTE_SIZE)..(i * TILE_BYTE_SIZE + TILE_BYTE_SIZE)];
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, index, TILE_PIXEL_AREA / PIXELS_PER_BYTE, TILE_PIXEL_AREA, 1, PixelFormat.RedInteger, PixelType.UnsignedByte, bytes);

                textureHandles[i] = index;
            }
            */

            // Return the texture handles
            return indices;
        }



        public byte LoadTexture(string path, byte index, bool checkFree = true)
        {
            // First check if its free if we're asked to
            if (checkFree && IsTextureOccupied(index))
                throw new Exception($"Texture at index {index} was not free!");

            /*
            // Activate its unit
            GL.ActiveTexture(TEXTURE_UNIT);

            // Replace the path with apriopriate path per system
            path = path.Replace('\\', Path.DirectorySeparatorChar);

            // Make the path local to the Clockwork Asset Pipeline ting
            path = Path.Combine("assets", "bin", $"{path}.bin");

            // Load the data
            Texture2DData data = Texture2DData.LoadFromFile(path);
            */

            Texture2DData data = LoadTextureData(path);
            LoadTexture(data.Raw.ToArray(), index, false);

            // Aaand return the index, its already given, but its easier for loading n shi
            return index;
        }
        public byte LoadTexture(string path) => LoadTexture(path, GetNextFreeIndex(), false);

        public void SetTextureOcupiedFlag(int index, bool val)
        {
            if (val)
            {
                _freeFlags[index / 8] |= (byte)(1 << 7 - index % 8);
                _textures++;
            }
            else
            {
                int freeFlagBit = 7 - index % 8;

                _freeFlags[index / 8] &= (byte)~(1 << freeFlagBit);
                _textures--;
            }
        }
    }
}
